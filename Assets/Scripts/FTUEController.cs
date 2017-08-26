using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FTUEController : MonoBehaviour
{
    [System.Serializable]
    public class FTUEStep
    {
        public int m_ftueLevel = -1;
        public GameObject m_ftueObject;
    }

    [SerializeField] private List<FTUEStep> m_FTUESteps;
    private int m_currentLevel = -1;

    private void Update()
    {
        if (m_currentLevel != GameManager.Instance.GetCurrentLevel())
        {
            m_currentLevel = GameManager.Instance.GetCurrentLevel();
            for (int i = 0; i < m_FTUESteps.Count; i++)
            {
                if (m_FTUESteps[i].m_ftueLevel == m_currentLevel)
                {
                    m_FTUESteps[i].m_ftueObject.SetActive(true);
                }
                else
                {
                    m_FTUESteps[i].m_ftueObject.SetActive(false);
                }
            }
        }
    }
}
