using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoText : MonoBehaviour 
{
    [SerializeField] private Text m_text;

    private int currentLevel = -1;

    private void Start()
    {
        if (m_text == null)
        {
            m_text = this.GetComponent<Text>();
        }
    }

    private void Update () 
    {
        if (currentLevel != GameManager.Instance.GetCurrentLevel())
        {
            currentLevel = GameManager.Instance.GetCurrentLevel();
            if (m_text != null)
            {
                m_text.text = "Difficulty:\n" + TileManager.Instance.GetLevelDifficulty() + "\n" +
                    "Board size:\n" + TileManager.Instance.GetBoardSize().ToString() + "x" + TileManager.Instance.GetBoardSize().ToString() + "\n" +
                    "Water flows in:\n" + TileManager.Instance.GetTimeToStart().ToString() + " sec";
            }
        }
    }
}
