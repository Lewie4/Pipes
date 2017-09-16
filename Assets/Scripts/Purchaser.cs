using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Events;

public class Purchaser : MonoBehaviour, IStoreListener
{
    public static Purchaser Instance = null;

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    private UnityEvent m_currentPurchaseAction;

    public static string m_unlimitedLives = "com.kingcat.pipes.unlimitedlives";
    public static string m_coinsSmall = "com.kingcat.pipes.coinssmall";
    public static string m_coinsMedium = "com.kingcat.pipes.coinsmedium";
    public static string m_coinsLarge = "com.kingcat.pipes.coinslarge";

    private int m_coinsSmallAmount = 150;
    private int m_coinsMediumAmount = 750;
    private int m_coinsLargeAmount = 2000;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }  
    }

    private void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }           
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(m_unlimitedLives, ProductType.Consumable);
        builder.AddProduct(m_coinsSmall, ProductType.Consumable);
        builder.AddProduct(m_coinsMedium, ProductType.Consumable);
        builder.AddProduct(m_coinsLarge, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void MakePurchase(string productID, UnityEvent action = null)
    {
        m_currentPurchaseAction = action;
        BuyProductID(productID);
    }

    private void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);           

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {

            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public ProductMetadata GetProductMetadata(string productId)
    {
        Product product = m_StoreController.products.WithID(productId); 
        return product.metadata;
    }

    public int GetProductAmount(string productID)
    {
        if (productID.Equals(m_coinsSmall))
        {
            return m_coinsSmallAmount;
        }
        else if (productID.Equals(m_coinsMedium))
        {
            return m_coinsMediumAmount;
        }
        else if (productID.Equals(m_coinsLarge))
        {
            return m_coinsLargeAmount;
        }
        return 0;
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
                {
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, m_unlimitedLives, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            GameManager.Instance.SetUnlimitedLives();
            PlayFabManager.Instance.SetUnlimitedLives();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, m_coinsSmall, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            GameManager.Instance.AddCoins(m_coinsSmallAmount);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, m_coinsMedium, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            GameManager.Instance.AddCoins(m_coinsMediumAmount);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, m_coinsLarge, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            GameManager.Instance.AddCoins(m_coinsLargeAmount);
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        m_currentPurchaseAction.Invoke();

        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}