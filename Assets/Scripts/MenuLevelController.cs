using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLevelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_levelContainers;

    private int m_progressLevel = -1;

    private void Start()
    {
        SetupLevelSelect();
    }

    private void Update()
    {
        if (m_progressLevel != GameManager.Instance.GetCurrentLevel())
        {
            SetupLevelSelect();
        }
    }

    private void SetupLevelSelect()
    {
        int count = 0;

        m_progressLevel = PlayerPrefs.GetInt("ProgressLevel", 0);
        int unlockedLevel = m_progressLevel + 1;

        for (int i = 0; i < m_levelContainers.Count; i++)
        {
            foreach (Transform child in m_levelContainers[i].transform)
            {
                child.GetComponent<Button>().interactable = (count < unlockedLevel);
                
                count++;
            }
        }
    }
}
