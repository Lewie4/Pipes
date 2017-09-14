using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections.Generic;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance = null;

    private string m_playFabID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {
        PlayFabSettings.TitleId = "B5F"; 

        #if UNITY_EDITOR
        var request = new LoginWithCustomIDRequest { CustomId = "Editor", CreateAccount = true, TitleId = PlayFabSettings.TitleId };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        #elif UNITY_ANDROID
        var request = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = SystemInfo.deviceUniqueIdentifier, OS = SystemInfo.operatingSystem, AndroidDevice = SystemInfo.deviceModel, CreateAccount = true, TitleId = PlayFabSettings.TitleId };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnLoginSuccess, OnLoginFailure);
        #endif
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("PlayFab::OnLoginSuccess");
        m_playFabID = result.PlayFabId;

        GetUserData();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("PlayFab::OnLoginFailure");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void GetUserData()
    {
        GetUserDataRequest request = new GetUserDataRequest()
        {
            PlayFabId = m_playFabID,
            Keys = null
        };

        PlayFabClientAPI.GetUserData(request, (result) =>
            {
                Debug.Log("PlayFab::GetUserData Success");
                if ((result.Data == null) || (result.Data.Count == 0))
                {
                    Debug.Log("PlayFab::GetUserData No user data available");
                    ForceUpdateRemote();
                }
                else
                {
                    UpdateLocalProfile(result);
                }
            }, (error) =>
            {
                Debug.Log("PlayFab::GetUserData Error");
                Debug.Log(error.ErrorMessage);
            });
    }

    private void UpdateLocalProfile(GetUserDataResult result)
    {
        UserDataRecord dataRecord;

        result.Data.TryGetValue("ProgressLevel", out dataRecord);
        if (dataRecord != null)
        {
            int progressLevel = -1;

            if (int.TryParse(dataRecord.Value, out progressLevel) && progressLevel > PlayerPrefs.GetInt("ProgressLevel", 0))
            {
                PlayerPrefs.SetInt("ProgressLevel", progressLevel);
                GameManager.Instance.SetCurrentLevel(progressLevel);
            }
            else
            {
                SetCurrentLevel();
            }
        }
        else
        {
            SetCurrentLevel();
        }

        dataRecord = null;
        result.Data.TryGetValue("UnlimitedLives", out dataRecord);
        if (dataRecord != null)
        {
            if (dataRecord.Value.Equals("true"))
            {
                GameManager.Instance.SetUnlimitedLives();
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("HasUnlimitedLives", 0) == 1)
            {
                SetUnlimitedLives();
            }
        }

        dataRecord = null;
        result.Data.TryGetValue("CurrentCoins", out dataRecord);
        if (dataRecord != null)
        {
            int currentCoins = -1;

            if (int.TryParse(dataRecord.Value, out currentCoins) && currentCoins > PlayerPrefs.GetInt("CurrentCoins", 0))
            {
                PlayerPrefs.SetInt("CurrentCoins", currentCoins);
                GameManager.Instance.SetCoins(currentCoins);
            }
            else
            {
                SetCurrentCoins();
            }
        }
        else
        {
            SetCurrentCoins();
        }
    }

    private void ForceUpdateRemote()
    {
        SetCurrentLevel();
        if (PlayerPrefs.GetInt("HasUnlimitedLives", 0) == 1)
        {
            SetUnlimitedLives();
        }
        SetCurrentCoins();
    }

    public void SetCurrentLevel()
    {
        int progressLevel = PlayerPrefs.GetInt("ProgressLevel", 0);

        var request = new UpdateUserDataRequest()
        { 
            Data = new Dictionary<string, string>()
            {
                { "ProgressLevel", progressLevel.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("PlayFab::SetProgressLevel Success");
            }, (error) =>
            {
                Debug.Log("PlayFab::SetProgressLevel Error");
                Debug.Log(error.ErrorDetails);
            });
    }

    public void SetUnlimitedLives()
    {
        var request = new UpdateUserDataRequest()
        { 
            Data = new Dictionary<string, string>()
            {
                { "UnlimitedLives", "true" }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("PlayFab::SetUnlimitedLives Success");
            }, (error) =>
            {
                Debug.Log("PlayFab::SetUnlimitedLives Error");
                Debug.Log(error.ErrorDetails);
            });
    }

    public void SetCurrentCoins()
    {
        int currentCoins = PlayerPrefs.GetInt("CurrentCoins", 0);

        var request = new UpdateUserDataRequest()
        { 
            Data = new Dictionary<string, string>()
            {
                { "CurrentCoins", currentCoins.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("PlayFab::SetCurrentCoins Success");
            }, (error) =>
            {
                Debug.Log("PlayFab::SetCurrentCoins Error");
                Debug.Log(error.ErrorDetails);
            });
    }

    public void GetLevelData(int level)
    {
        GetUserDataRequest request = new GetUserDataRequest()
        {
            PlayFabId = m_playFabID,
            Keys = new List<string>()
                    { "Level" + level.ToString() }
        };

        PlayFabClientAPI.GetUserData(request, (result) =>
            {
                Debug.Log("PlayFab::GetLevelData Success");
                if ((result.Data == null) || (result.Data.Count == 0))
                {
                    Debug.Log("PlayFab::GetLevelData No user data available");
                }
                else
                {
                    UserDataRecord dataRecord;

                    result.Data.TryGetValue("Level" + level.ToString(), out dataRecord);
                    if (dataRecord != null)
                    {
                        int time = -1;

                        if (int.TryParse(dataRecord.Value, out time))
                        {
                            TileManager.Instance.SetBestTime(time);
                        }
                    }
                            
                }
            }, (error) =>
            {
                Debug.Log("PlayFab::GetLevelData Error");
                Debug.Log(error.ErrorMessage);
            });
    }

    public void SetLevelData(int level, int time)
    {
        var request = new UpdateUserDataRequest()
        { 
            Data = new Dictionary<string, string>()
            {
                { "Level" + level.ToString(), time.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("PlayFab::SetLevelData Success");
            }, (error) =>
            {
                Debug.Log("PlayFab::SetLevelData Error");
                Debug.Log(error.ErrorDetails);
            });
    }
}