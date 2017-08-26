using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class LivesManager : MonoBehaviour
{
    [SerializeField] private GameObject m_addLives;
    
    [SerializeField] private Sprite m_fullHeart;
    [SerializeField] private Sprite m_emptyHeart;

    [SerializeField] private List<Image> m_hearts = new List<Image>();

    [SerializeField] private Text m_timerText;

    [SerializeField] private UnityEvent m_rewardOneLives;
    [SerializeField] private UnityEvent m_rewardUnlimitedLives;
    [SerializeField] private UnityEvent m_rewardExtraTime;

    private int m_currentLives = 0;
    private int m_fakeLevelLives = 0;

    private void Start()
    {
        SetupHearts();
    }

    private void Update()
    {
        if (m_currentLives != GameManager.Instance.GetCurrentLives())
        {
            SetupHearts();
        }

        if (m_currentLives != GameManager.Instance.GetMaxLives())
        {
            int livesTimer = GameManager.Instance.GetLivesTimer();
            if (livesTimer > 0)
            {
                m_timerText.text = string.Format("{0}:{1}", ((int)(livesTimer / 60)).ToString("D2"), (livesTimer % 60).ToString("D2"));
            }
            else
            {
                m_timerText.text = "Full!";
            }
        }
        else
        {
            m_timerText.text = "Full!";
        }
    }

    private void SetupHearts()
    {
        m_currentLives = GameManager.Instance.GetCurrentLives();

        m_currentLives += m_fakeLevelLives;

        for (int i = 0; i < GameManager.Instance.GetMaxLives(); i++)
        {
            if (i < m_hearts.Count && i < m_currentLives)
            {
                m_hearts[i].sprite = m_fullHeart;
            }
            else
            {
                m_hearts[i].sprite = m_emptyHeart;
            }
        }

        if (m_addLives != null)
        {
            m_addLives.SetActive(m_currentLives < GameManager.Instance.GetMaxLives());
        }
    }

    public void SpendLives(int lives)
    {
        GameManager.Instance.SpendLives(lives);
    }

    public void GainLives(int lives)
    {
        GameManager.Instance.GainLives(lives);
    }

    public void WatchAdForLife()
    {
        AdsManager.Instance.ShowRewardedAd(m_rewardOneLives);
    }

    public void WatchAdForTime()
    {
        AdsManager.Instance.ShowRewardedAd(m_rewardExtraTime);
    }

    public void BuyUnlimitedLives()
    {
        Purchaser.Instance.BuyUnlimitedLives(m_rewardUnlimitedLives);
    }

    public void SetFakeLevelLives(int lives)
    {
        m_fakeLevelLives = lives;
    }
}
