using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoText : MonoBehaviour
{
    [SerializeField] private Text m_text;
    [SerializeField] private Text m_bestTimeText;

    private int m_currentLevel = -1;
    private float m_timeToStart = -1;

    private void Start()
    {
        if (m_text == null)
        {
            m_text = this.GetComponent<Text>();
        }
    }

    private void Update()
    {
        if (m_currentLevel != GameManager.Instance.GetCurrentLevel() || m_timeToStart != TileManager.Instance.GetTimeToStart())
        {
            m_currentLevel = GameManager.Instance.GetCurrentLevel();
            m_timeToStart = TileManager.Instance.GetTimeToStart();
            if (m_text != null)
            {
                m_text.text = "Difficulty:\n" + TileManager.Instance.GetLevelDifficulty() + "\n" +
                "Board size:\n" + TileManager.Instance.GetBoardSize().ToString() + "x" + TileManager.Instance.GetBoardSize().ToString() + "\n" +
                "Water flows in:\n";
                if (PlayerPrefs.GetInt("Level" + m_currentLevel + "Time", 0) > 6)
                {
                    m_text.text += "Unlimited sec";
                }
                else
                {
                    m_text.text += TileManager.Instance.GetTimeToStart().ToString() + " sec";
                }
            }

            m_bestTimeText.text = PlayerPrefs.GetInt("Level" + m_currentLevel.ToString(), 0).ToString() + " sec";
        }
    }
}
