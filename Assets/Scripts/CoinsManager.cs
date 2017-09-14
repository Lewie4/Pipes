using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsManager : MonoBehaviour
{
    [SerializeField] private Text m_coinsText;

    private int m_coins;
    private bool m_counting = false;

    private void Start()
    {
        m_coins = GameManager.Instance.GetCurrentCoins();
        m_coinsText.text = m_coins.ToString();
    }

    private void Update()
    {
        if (!m_counting && m_coins != GameManager.Instance.GetCurrentCoins())
        {       
            m_counting = true;
            StartCoroutine(CountCoins());
        }
    }

    private IEnumerator CountCoins()
    {        
        int currentCoins = GameManager.Instance.GetCurrentCoins();

        float tickTime = 0;

        while (m_coins != currentCoins)
        {      
            if (tickTime >= 0.1)
            {
                tickTime = 0;

                currentCoins = GameManager.Instance.GetCurrentCoins();

                int diff = currentCoins - m_coins;

                int coinsToAdd = (int)(diff / 10);

                m_coins += coinsToAdd != 0 ? coinsToAdd : (diff > 0 ? 1 : -1);

                m_coinsText.text = m_coins.ToString();                
            }
            tickTime += Time.deltaTime;
            yield return null;
        }
        m_counting = false;
    }
}
