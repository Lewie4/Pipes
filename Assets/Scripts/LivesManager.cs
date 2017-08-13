using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesManager : MonoBehaviour {

    public void SpendLives(int lives)
    {
        GameManager.Instance.SpendLives(lives);
    }
}
