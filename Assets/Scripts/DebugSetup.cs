#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSetup : MonoBehaviour
{
    private void Start()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
}
#endif