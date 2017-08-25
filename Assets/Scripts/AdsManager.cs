using UnityEngine;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
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
        if (Time.timeScale == 0)
        {
            #if UNITY_ADS 
            if (!Advertisement.isShowing)
            #endif
            {
                #if UNITY_EDITOR
                if (DebugController.Instance != null && !DebugController.Instance.m_active)
                {
                    Time.timeScale = 1;
                }
                #else
                Time.timeScale = 1;
                #endif
            }
        }
    }

    public bool CheckRewardedAd()
    {
        #if UNITY_ADS
        if (Advertisement.IsReady("rewardedVideo"))
        {
            return true;
        }
        return false;
        #else
        return true;
        #endif
    }

    public void ShowRewardedAd(UnityEvent actions = null)
    {
        #if UNITY_ADS
        if (Advertisement.IsReady("rewardedVideo"))
        {
            m_rewardCompleteActions = actions;
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
            Time.timeScale = 0;
        }
        #else
        AwardLives(1);
        #endif
    }

    public bool CheckInterstitialAd()
    {
        #if UNITY_ADS
        if (!GameManager.Instance.GetUnlimitedLives())
        {
            if (Advertisement.IsReady("video"))
            {
                return true;
            }
        }
        else
        {
            return true;
        }
        return false;
        #else
        return true;
        #endif
    }

    public void ShowInterstitialAd()
    {
        #if UNITY_ADS
        if (!GameManager.Instance.GetUnlimitedLives())
        {
            if (Advertisement.IsReady("video"))
            {
                Advertisement.Show("video");
                Time.timeScale = 0;
            }
        }
        #endif
    }

    #if UNITY_ADS
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                AwardLives(1);
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
    #endif

    public void AwardLives(int lives)
    {
        GameManager.Instance.GainLives(lives);
        m_rewardCompleteActions.Invoke();
        m_rewardCompleteActions = null;
    }

    public bool CheckRandomInterstitialAd()
    {
        #if UNITY_ADS
        if (Random.Range(0, 100) <= m_adChance && AdsManager.Instance.CheckInterstitialAd() && CheckInterstitialAd())
        {
            return true;
        }
        #endif
        return false;
    }
}
