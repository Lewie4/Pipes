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


    void Start()
    {
        m_timeToStart = TileManager.Instance.GetTimeToStart();
        m_currentAmount = TileManager.Instance.GetStartFillTime();
    }

    void Update()
    {
        if (!m_goSet)
        {
            if (m_timeToStart != TileManager.Instance.GetTimeToStart())
            {
                m_timeToStart = TileManager.Instance.GetTimeToStart();
            }

            if (m_currentAmount != TileManager.Instance.GetStartFillTime())
            {
                m_currentAmount = TileManager.Instance.GetStartFillTime();
            }

            if (m_currentAmount < m_timeToStart)
                {
                m_textIndicator.text = (m_timeToStart - m_currentAmount).ToString("F0");
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
