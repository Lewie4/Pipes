using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TopBarController : MonoBehaviour
{
    [SerializeField] private GameObject m_homeContainer;
    [SerializeField] private GameObject m_livesContainer;

    private bool m_onMenu;

    private void Awake()
    {
        m_onMenu = SceneManager.GetActiveScene().name == "MainMenu";

        m_homeContainer.SetActive(!m_onMenu);
    }

    public void SetHomeButton(bool active)
    {
        m_homeContainer.SetActive(active);
    }
}
