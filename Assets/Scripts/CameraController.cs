using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private RectTransform m_RT;

    private bool m_startingLevel = false;

    private void Update()
    {
        if (!m_startingLevel)
        {
            this.transform.position = new Vector3((m_RT.position.x / -1000), 0, 0);
        }
    }

    public void SetLevelStart()
    {
        m_startingLevel = true;
    }
}
