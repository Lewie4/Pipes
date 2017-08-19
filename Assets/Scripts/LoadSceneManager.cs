using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.Events;

public class LoadSceneManager : MonoBehaviour
{
    [SerializeField] private UnityEvent m_notEnoughLives;

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGameLevel(int level)
    {
        if (GameManager.Instance.GetCurrentLives() > 0)
        {
            GameManager.Instance.SetCurrentLevel(level);
            SendLevelStartedAnalytic();
            SceneManager.LoadScene("Game");
        }
        else
        {
            m_notEnoughLives.Invoke();
        }
    }

    public void LoadProgressGameLevel()
    {
        if (GameManager.Instance.GetCurrentLives() > 0)
        {
            GameManager.Instance.SetCurrentLevel(PlayerPrefs.GetInt("ProgressLevel", 0));
            SendLevelStartedAnalytic();
            SceneManager.LoadScene("Game");
        }
        else
        {
            m_notEnoughLives.Invoke();
        }
    }

    public void FailAndReloadLevel()
    {
        if (GameManager.Instance.GetCurrentLives() > 0)
        {
            if (AdsManager.Instance.CheckRandomInterstitialAd())
            {
                AdsManager.Instance.ShowInterstitialAd();
            }

            TileManager.Instance.LoadLevelFromGame(GameManager.Instance.GetCurrentLevel());
        }
        else
        {
            m_notEnoughLives.Invoke();
        }
    }

    public void UnlockAndLoadNextLevel()
    {
        if (AdsManager.Instance.CheckRandomInterstitialAd())
        {
            AdsManager.Instance.ShowInterstitialAd();
        }

        GameManager.Instance.UnlockNextLevel();
        TileManager.Instance.LoadLevelFromGame(GameManager.Instance.GetCurrentLevel());
    }

    public void SendLevelStartedAnalytic()
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>();
        eventData.Add("Level", GameManager.Instance.GetCurrentLevel());

        Analytics.CustomEvent("LevelStarted", eventData);
    }
}
