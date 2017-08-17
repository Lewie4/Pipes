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

    [SerializeField] private UnityEvent m_rewardCompleteActions;

    private int currentLives = 0;

    private void Start()
    {
        SetupHearts();
    }

    private void Update()
    {
        if (currentLives != GameManager.Instance.GetCurrentLives())
        {
            SetupHearts();
        }

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

    private void SetupHearts()
    {
        currentLives = GameManager.Instance.GetCurrentLives();

        for (int i = 0; i < GameManager.Instance.GetMaxLives(); i++)
        {
            if (i < m_hearts.Count && i < currentLives)
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
            m_addLives.SetActive(currentLives < GameManager.Instance.GetMaxLives());
        }
    }

    public void SpendLives(int lives)
    {
        GameManager.Instance.SpendLives(lives);
    }

    public void WatchAdForLife()
    {
        AdsManager.Instance.ShowRewardedAd(m_rewardCompleteActions);
    }
}
