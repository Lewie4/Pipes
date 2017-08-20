using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherManger : MonoBehaviour
{
    public void ButtonPress()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ButtonPress();
        }
    }
}
