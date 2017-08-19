using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLevelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_levelContainers;

    private void Start()
    {
        int count = 0;

        int unlockedLevel = PlayerPrefs.GetInt("ProgressLevel", 0) + 1;

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
