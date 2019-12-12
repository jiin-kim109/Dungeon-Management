using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class IAP_Product: MonoBehaviour
{
    [SerializeField]
    private string productID;
    public TextMeshProUGUI priceTextMesh;

    [Space(8)]
    public UnityEvent Init;
    public UnityEvent Apply;
    public UnityEvent Load;

    public void BuyConsumable()
    {
        UIHandler.Instance.IAP.BuyProductID(productID);
    }
    public string GetProductID() { return productID; }
}
