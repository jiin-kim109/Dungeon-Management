using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using TMPro;

public class IAPManager : MonoBehaviour, IStoreListener
{

    [SerializeField]
    private List<IAP_Product> products = new List<IAP_Product>();

    private static IStoreController m_storeController;           //Unity Purchase System
    private static IExtensionProvider m_storeExtensionProvider;  //The store-specific Purchasing Subsystem

    private static IAP_Product curr;

    public void Initialize()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach(IAP_Product p in products)
        {
            builder.AddProduct(p.GetProductID(), ProductType.Consumable, new IDs()
            {
            { p.GetProductID(), GooglePlay.Name },
            });
            p.Init.Invoke();
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyProductID(string product_id)
    {
        curr = SearchProduct(product_id);
        if(curr == null) { return; }

        Product product = m_storeController.products.WithID(product_id);

        if (product != null && product.availableToPurchase)
        {
            m_storeController.InitiatePurchase(product);
        }
        else
        {
            curr = null;
            return;
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("faild to intialize products");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (string.Equals(e.purchasedProduct.definition.id, curr.GetProductID(), System.StringComparison.Ordinal))
        {
            curr.Apply.Invoke();
            curr = null;
        }
        else
        {

        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log("faild to purchase products");
        curr = null;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_storeController = controller;
        m_storeExtensionProvider = extensions;
        foreach(IAP_Product p in products)
        {
            p.priceTextMesh.text = m_storeController.products.WithID(p.GetProductID()).metadata.localizedPrice.ToString();
        }
    }

    private IAP_Product SearchProduct(string productID)
    {
        foreach(IAP_Product p in products)
        {
            if(p.GetProductID() == productID)
            {
                return p;
            }
        }
        return null;
    }

    //==========================================================

    public void OnClick_Load()
    {
        foreach(IAP_Product p in products)
        {
            p.Load.Invoke();
        }
        GameManager.Instance.SaveGame();
        UIHandler.Instance.UpdateUI();
    }
}
