using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelText : MonoBehaviour 
{
    [SerializeField] private Text m_text;

	private void Start () 
    {
        if (m_text == null)
        {
            m_text = this.GetComponent<Text>();
        }

        if (m_text != null)
        {
            m_text.text = "Level " + (GameManager.Instance.GetCurrentLevel() + 1).ToString();
        }
	}
}
