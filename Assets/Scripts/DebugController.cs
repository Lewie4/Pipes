#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public static DebugController Instance = null;

    public bool m_active = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        m_active = true;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
        m_active = false;
    }

    public void ClearLevel()
    {
        TileManager.Instance.ClearCurrentLevel();
    }

    public void ReloadLevel()
    {
        TileManager.Instance.LoadLevelFromGame(GameManager.Instance.GetCurrentLevel());
    }
}
#endif