using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image m_loadingbar;
    [SerializeField] private Text m_textIndicator;

    private float m_timeToStart;
    private float m_currentAmount;
    private bool m_goSet = false;
    private bool m_inLevel = true;

    private void Start()
    {
        if (TileManager.Instance != null)
        {
            m_timeToStart = TileManager.Instance.GetTimeToStart();
            m_currentAmount = TileManager.Instance.GetStartFillTime();
        }
        else
        {
            m_inLevel = false;
            m_timeToStart = 15;
            m_currentAmount = 0;
        }
    }

    private void OnEnable()
    {
        if (!m_inLevel)
        {
            m_timeToStart = 15;
            m_currentAmount = 0;  
        }
    }

    private void Update()
    {
        if (m_inLevel)
        {
            if (TileManager.Instance.GetUnlimitedTime() || m_currentAmount != TileManager.Instance.GetStartFillTime())
            {
                m_currentAmount = TileManager.Instance.GetStartFillTime();
                m_goSet = false;
            }
        }

        if (!m_goSet)
        {
            if (m_inLevel)
            {
                if (m_timeToStart != TileManager.Instance.GetTimeToStart())
                {
                    m_timeToStart = TileManager.Instance.GetTimeToStart();
                }
            }
            else
            {
                m_currentAmount += Time.deltaTime;
            }

            if (m_currentAmount < m_timeToStart)
            {
                if (!TileManager.Instance.GetUnlimitedTime())
                {
                    m_textIndicator.text = (m_timeToStart - m_currentAmount).ToString("F0");
                }
                else
                {
                    m_textIndicator.text = "Unlimited";
                }
            }
            else
            {
                m_textIndicator.text = "GO!";
                m_goSet = true;
            }
            m_loadingbar.fillAmount = m_currentAmount / m_timeToStart;
        }
    }
}
