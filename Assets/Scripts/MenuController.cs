using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private float m_snapSpeed = 10;

    private RectTransform m_RT;
    private ScrollRect m_scrollRect;

    private Vector3 m_destination;

    private void Awake()
    {
        m_RT = this.GetComponent<RectTransform>();
        m_RT.sizeDelta = new Vector2(Screen.width * this.transform.childCount, Screen.height);
        m_RT.position = new Vector3(-Screen.width, m_RT.position.y, 0);

        m_scrollRect = this.transform.parent.GetComponent<ScrollRect>();


        m_destination = Vector3.one;
    }

    private void Update()
    {
        if (m_destination != Vector3.one)
        {
            m_RT.position = Vector3.Lerp(m_RT.position, m_destination, Time.deltaTime * m_snapSpeed);
        }
    }

    public void DragEnd()
    {
        float mod = ((m_RT.position.x % Screen.width) + Screen.width) % Screen.width;

        if (mod < (Screen.width / 2))
        {
            m_scrollRect.velocity = Vector2.zero;
            m_destination = new Vector3(m_RT.position.x - mod, m_RT.position.y, 0);
        }
        else
        {
            m_scrollRect.velocity = Vector2.zero;
            m_destination = new Vector3(m_RT.position.x - (m_RT.position.x % Screen.width), m_RT.position.y, 0);
        }
    }

    public void DragStart()
    {
        m_destination = Vector3.one;
    }

    public void LevelSelect()
    {
        m_scrollRect.velocity = Vector2.zero;
        m_destination = new Vector3(-Screen.width * 2, m_RT.position.y, 0); 
    }

    public void CreditsSelect()
    {
        m_scrollRect.velocity = Vector2.zero;
        m_destination = new Vector3(0, m_RT.position.y, 0); 
    }
}
