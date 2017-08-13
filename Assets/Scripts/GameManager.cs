using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [SerializeField] private int m_currentLevel = 0;

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

        #if !UNITY_EDITOR
        m_currentLevel = PlayerPrefs.GetInt("ProgressLevel", 0);
        #endif
    }

    public int GetCurrentLevel()
    {
        return m_currentLevel;
    }

    public void SetCurrentLevel(int currentLevel)
    {
        m_currentLevel = currentLevel;
    }

    public void UnlockNextLevel()
    {
        m_currentLevel++;

        if (m_currentLevel > PlayerPrefs.GetInt("ProgressLevel", 0))
        {
            PlayerPrefs.SetInt("ProgressLevel", m_currentLevel);
        }
    }
}
