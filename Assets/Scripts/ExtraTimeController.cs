using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraTimeController : MonoBehaviour
{
    [SerializeField] private Text m_currentTimeText;
    [SerializeField] private Text m_extraTimeText;
    [SerializeField] private Text m_coinsText;

    [SerializeField] private Button m_plusButton;
    [SerializeField] private Button m_minusButton;
    [SerializeField] private Button m_getButton;
    [SerializeField] private Button m_adButton;

    private int m_startingExtraTimeCount = 0;
    private int m_extraTimeCount = 0;
    private int m_currentLevel = -1;

    private void Update()
    {
        if (m_currentLevel != GameManager.Instance.GetCurrentLevel())
        {
            m_currentLevel = GameManager.Instance.GetCurrentLevel();
            m_startingExtraTimeCount = PlayerPrefs.GetInt("Level" + m_currentLevel + "Time", 0);
            TileManager.Instance.AddTimeToStart(m_startingExtraTimeCount * 5);
            m_extraTimeCount = 0;

            SetCurrentTimeText();
            SetExtraTimeText();
            SetCoinsText();
        }
    }

    public void AddSeconds()
    {
        if (m_extraTimeCount <= 0)
        {
            m_minusButton.interactable = true;
        }        
        m_extraTimeCount++;

        if (m_extraTimeCount > 6 - m_startingExtraTimeCount)
        {
            m_plusButton.interactable = false;
        }

        SetExtraTimeText();
        SetCoinsText();
    }

    public void RemoveSeconds()
    {
        if (m_extraTimeCount > 6 - m_startingExtraTimeCount)
        {
            m_plusButton.interactable = true;
        }        
        m_extraTimeCount--;

        if (m_extraTimeCount <= 0)
        {
            m_minusButton.interactable = false;
        }

        SetExtraTimeText();
        SetCoinsText();
    }

    public void GetSeconds()
    {
        if (GameManager.Instance.GetCurrentCoins() >= GetCurrentCost())
        {
            GameManager.Instance.AddCoins(-GetCurrentCost());

            m_startingExtraTimeCount += m_extraTimeCount;
            PlayerPrefs.SetInt("Level" + m_currentLevel + "Time", m_startingExtraTimeCount);

            TileManager.Instance.AddTimeToStart(m_startingExtraTimeCount * 5);
            m_extraTimeCount = 0;

            SetCurrentTimeText();
            SetExtraTimeText();
            SetCoinsText();
        }
    }

    public void SetCurrentTimeText()
    {
        if (m_extraTimeCount <= 0)
        {
            m_minusButton.interactable = false;
        }
        else
        {
            m_minusButton.interactable = true; 
        }

        if (m_extraTimeCount > 6 - m_startingExtraTimeCount)
        {
            m_plusButton.interactable = false;
            m_getButton.interactable = false;
            m_adButton.interactable = false;
        }
        else
        {
            m_plusButton.interactable = true;
            m_getButton.interactable = true;
            m_adButton.interactable = true;
        }

        if (m_startingExtraTimeCount > 6)
        {
            m_currentTimeText.text = "Unlimited Seconds";
        }
        else
        {
            m_currentTimeText.text = (TileManager.Instance.GetTimeToStart()).ToString() + " Seconds";
        }
    }

    private void SetExtraTimeText()
    {
        if (m_extraTimeCount > 6 - m_startingExtraTimeCount)
        {
            m_extraTimeText.text = "Unlimited";
        }
        else
        {
            m_extraTimeText.text = (m_extraTimeCount * 5).ToString();
        }
    }

    private void SetCoinsText()
    {
        m_coinsText.text = (GetCurrentCost()).ToString();
    }

    private int GetCurrentCost()
    {
        return m_extraTimeCount * 15;
    }
}
