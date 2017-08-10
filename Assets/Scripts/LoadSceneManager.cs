using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGameLevel(int level)
    {
        GameManager.Instance.SetCurrentLevel(level);
        SceneManager.LoadScene("Game");
    }

    public void LoadProgressGameLevel()
    {
        GameManager.Instance.SetCurrentLevel(PlayerPrefs.GetInt("ProgressLevel", 0));
        SceneManager.LoadScene("Game");
    }

    public void FailAndReloadLevel()
    {
        LoadGameLevel(GameManager.Instance.GetCurrentLevel());
    }

    public void UnlockAndLoadNextLevel()
    {
        GameManager.Instance.UnlockNextLevel();
        LoadGameLevel(GameManager.Instance.GetCurrentLevel());
    }
}
