using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance = null;

    private UnityEvent m_rewardCompleteActions;

    private int m_adChance = 10;

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

    private void Update()
    {
        if (Time.timeScale == 0 && !Advertisement.isShowing && !DebugController.Instance.m_active)
        {
            Time.timeScale = 1;
        }
    }

    public bool CheckRewardedAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            return true;
        }
        return false;
    }

    public void ShowRewardedAd(UnityEvent actions = null)
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            m_rewardCompleteActions = actions;
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
            Time.timeScale = 0;
        }
    }

    public bool CheckInterstitialAd()
    {
        if (Advertisement.IsReady("video"))
        {
            return true;
        }
        return false;
    }

    public void ShowInterstitialAd()
    {
        if (Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
            Time.timeScale = 0;
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                GameManager.Instance.GainLives(1);
                m_rewardCompleteActions.Invoke();
                m_rewardCompleteActions = null;
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }

    public bool CheckRandomInterstitialAd()
    {
        if (Random.Range(0, 100) <= m_adChance && AdsManager.Instance.CheckInterstitialAd() && CheckInterstitialAd())
        {
            return true;
        }
        return false;
    }
}
