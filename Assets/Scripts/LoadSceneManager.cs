using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.Events;

public class LoadSceneManager : MonoBehaviour
{
    [SerializeField] private UnityEvent m_enoughLives;
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
            SceneManager.LoadScene("Game");
        }
        else
        {
            m_notEnoughLives.Invoke();
        }
    }

    public void LoadCurrentLevel(bool adChance)
    {
        if (adChance && AdsManager.Instance.CheckRandomInterstitialAd())
        {
            AdsManager.Instance.ShowInterstitialAd();
        }

        TileManager.Instance.LoadLevelFromGame(GameManager.Instance.GetCurrentLevel());
    }

    public void StartLevel()
    {
        if (GameManager.Instance.GetCurrentLives() > 0)
        {
            SendLevelStartedAnalytic();
            m_enoughLives.Invoke();
            GameManager.Instance.SpendLives(1);
        }
        else
        {
            m_notEnoughLives.Invoke();
        }
    }

    public void SendLevelStartedAnalytic()
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>();
        eventData.Add("Level", GameManager.Instance.GetCurrentLevel());

        Analytics.CustomEvent("LevelStarted", eventData);
    }
}
