using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelText : MonoBehaviour
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

    private void Update()
    {
        if (currentLevel != GameManager.Instance.GetCurrentLevel() && !TileManager.Instance.GetHasWon())
        {
            currentLevel = GameManager.Instance.GetCurrentLevel();
            if (m_text != null)
            {
                m_text.text = "Level " + (currentLevel + 1).ToString();
            }
        }
    }
}
