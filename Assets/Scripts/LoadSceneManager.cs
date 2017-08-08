using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour 
{
    public static LoadSceneManager Instance = null;

    public void Awake()
    {
        /*if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }*/
    }

    public void LoadSceneByName (string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
