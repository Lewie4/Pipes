using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    [SerializeField] private float m_speed;

    private void Update()
    {
        this.transform.position = new Vector3(this.transform.position.x + (Time.deltaTime * m_speed), this.transform.position.y, this.transform.position.z);
    }
}
