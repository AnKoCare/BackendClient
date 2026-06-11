using System;using NUnit.Framework.Internal;
using UnityEngine;

public delegate string GetPriceEvent(string productID);
public delegate void OnPurchaseEvent(string productID, Action onSuccess, Action onFailure);
public static class IAPEvent
{
    public static Action RestorePurchases { get; set; }
    public static GetPriceEvent GetPrice { get; set; }
    public static OnPurchaseEvent OnPurchase { get; set; }
}