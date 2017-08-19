using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TopBarController : MonoBehaviour
{
    [SerializeField] private GameObject m_homeContainer;
    [SerializeField] private GameObject m_livesContainer;

    [SerializeField] private LoadSceneManager m_loadSceneManager;
    [SerializeField] private MenuController m_menuController;

    private bool m_onMenu;

    private void Awake()
    {
        m_onMenu = SceneManager.GetActiveScene().name == "MainMenu";

        m_homeContainer.SetActive(!m_onMenu);
    }

    public void HomeButtonPressed()
    {
        if (m_onMenu)
        {
            if (m_menuController != null)
            {
                m_menuController.HomeSelect();
            }
        }
        else
        {
            m_loadSceneManager.LoadSceneByName("MainMenu");
        }
    }

    public void SetHomeButton(bool active)
    {
        m_homeContainer.SetActive(active);
    }
}
