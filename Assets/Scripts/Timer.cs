using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour 
{
    public Transform Loadingbar;
    public Transform TextIndicator;

    private float timeToStart;

    [SerializeField] private float currentAmount;

    void Start()
    {
        timeToStart = TileManager.Instance.GetTimeToStart();
        currentAmount = timeToStart;
    }

    void Update()
    {
        if (currentAmount > 0)
        {
            currentAmount -= Time.deltaTime;
            //if (currentAmount >= 10)
            {
                TextIndicator.GetComponent<Text>().text = ((int)currentAmount).ToString();
            }
            /*
            else
            {
                TextIndicator.GetComponent<Text>().text = (currentAmount).ToString("F1");
            }
            */
        }
        else
        {
            TextIndicator.GetComponent<Text>().text = "GO!";
        }
        Loadingbar.GetComponent<Image>().fillAmount = currentAmount / timeToStart;
    }
}
