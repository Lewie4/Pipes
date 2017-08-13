using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class LoadSceneManager : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGameLevel(int level)
    {
        GameManager.Instance.SetCurrentLevel(level);
        SendLevelStartedAnalytic();
        SceneManager.LoadScene("Game");
    }

    public void LoadProgressGameLevel()
    {
        GameManager.Instance.SetCurrentLevel(PlayerPrefs.GetInt("ProgressLevel", 0));
        SendLevelStartedAnalytic();
        SceneManager.LoadScene("Game");
    }

    public void FailAndReloadLevel()
    {
        if (AdsManager.Instance.CheckRandomInterstitialAd())
        {
            AdsManager.Instance.ShowInterstitialAd();
        }

        LoadGameLevel(GameManager.Instance.GetCurrentLevel());
    }

    public void UnlockAndLoadNextLevel()
    {
        if (AdsManager.Instance.CheckRandomInterstitialAd())
        {
            AdsManager.Instance.ShowInterstitialAd();
        }

        GameManager.Instance.UnlockNextLevel();
        LoadGameLevel(GameManager.Instance.GetCurrentLevel());
    }

    public void SendLevelStartedAnalytic()
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>();
        eventData.Add("Level", GameManager.Instance.GetCurrentLevel());

        Analytics.CustomEvent("LevelStarted", eventData);
    }
}
