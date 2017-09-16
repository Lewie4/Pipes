using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Events;

public class ShopHandler : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public Button m_button;
        public Text m_priceText;
        public Text m_amountText;
    }

    [SerializeField] private ShopItem m_unlimitedLives;
    [SerializeField] private ShopItem m_coinsSmall;
    [SerializeField] private ShopItem m_coinsMedium;
    [SerializeField] private ShopItem m_coinsLarge;

    private ProductMetadata m_unlimitedLivesMetadata;
    private ProductMetadata m_coinsSmallMetadata;
    private ProductMetadata m_coinsMediumMetadata;
    private ProductMetadata m_coinsLargeMetadata;

    [SerializeField] private UnityEvent m_rewardUnlimitedLives;
    [SerializeField] private UnityEvent m_rewardCoinsSmall;
    [SerializeField] private UnityEvent m_rewardCoinsMedium;
    [SerializeField] private UnityEvent m_rewardCoinsLarge;

    private bool m_isInitialized = false;

    private void Update()
    {
        if (!m_isInitialized && Purchaser.Instance.IsInitialized())
        {
            m_unlimitedLivesMetadata = Purchaser.Instance.GetProductMetadata(Purchaser.m_unlimitedLives);
            m_unlimitedLives.m_priceText.text = m_unlimitedLivesMetadata.localizedPriceString;
            m_unlimitedLives.m_button.onClick.AddListener(delegate
                {
                    Purchaser.Instance.MakePurchase(Purchaser.m_unlimitedLives, m_rewardUnlimitedLives);
                });
            if (GameManager.Instance.GetUnlimitedLives())
            {
                m_unlimitedLives.m_button.interactable = false;
            }

            m_coinsSmallMetadata = Purchaser.Instance.GetProductMetadata(Purchaser.m_coinsSmall);
            m_coinsSmall.m_priceText.text = m_coinsSmallMetadata.localizedPriceString;
            m_coinsSmall.m_amountText.text = Purchaser.Instance.GetProductAmount(Purchaser.m_coinsSmall).ToString();
            m_coinsSmall.m_button.onClick.AddListener(delegate
                {
                    Purchaser.Instance.MakePurchase(Purchaser.m_coinsSmall, m_rewardCoinsSmall);
                });

            m_coinsMediumMetadata = Purchaser.Instance.GetProductMetadata(Purchaser.m_coinsMedium);
            m_coinsMedium.m_priceText.text = m_coinsMediumMetadata.localizedPriceString;
            m_coinsMedium.m_amountText.text = Purchaser.Instance.GetProductAmount(Purchaser.m_coinsMedium).ToString();
            m_coinsMedium.m_button.onClick.AddListener(delegate
                {
                    Purchaser.Instance.MakePurchase(Purchaser.m_coinsMedium, m_rewardCoinsMedium);
                });

            m_coinsLargeMetadata = Purchaser.Instance.GetProductMetadata(Purchaser.m_coinsLarge);
            m_coinsLarge.m_priceText.text = m_coinsLargeMetadata.localizedPriceString;
            m_coinsLarge.m_amountText.text = Purchaser.Instance.GetProductAmount(Purchaser.m_coinsLarge).ToString();
            m_coinsLarge.m_button.onClick.AddListener(delegate
                {
                    Purchaser.Instance.MakePurchase(Purchaser.m_coinsLarge, m_rewardCoinsLarge);
                });

            m_isInitialized = true;
        }
    }
}
