using JustTrack;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Newtonsoft.Json.Linq;

public class InAppPurchasesButtonController : MonoBehaviour, IDetailedStoreListener
{
    private static IStoreController storeController;

#if UNITY_IOS
    private const string ConsumableId = "consumable_purchase";
    private const string NonConsumableId = "non_consumable_purchase";
    private const string AutoRenewingSubscriptionId = "auto_renewable_subscription";
    private const string NonRenewingSubscriptionId = "non_renewing_subscription";
#else
    private const string ConsumableId = "product_2";
    private const string NonConsumableId = "product_1";
    private const string AutoRenewingSubscriptionId = "sub_1";
#endif

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Unity Services initialization failed: {e}");
        }
        InitializePurchasing();
    }

    private void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(ConsumableId, UnityEngine.Purchasing.ProductType.Consumable);
        builder.AddProduct(NonConsumableId, UnityEngine.Purchasing.ProductType.NonConsumable);
        builder.AddProduct(AutoRenewingSubscriptionId, UnityEngine.Purchasing.ProductType.Subscription);
#if UNITY_IOS
        builder.AddProduct(NonRenewingSubscriptionId, UnityEngine.Purchasing.ProductType.Subscription);
#endif

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnClickConsumablePurchase()
    {
        BuyProductId(ConsumableId);
    }

    public void OnClickNonConsumablePurchase()
    {
        BuyProductId(NonConsumableId);
    }

    public void OnClickNonRenewingSubscription()
    {
#if UNITY_IOS
        BuyProductId(NonRenewingSubscriptionId);
#else
        Debug.Log("Non-renewing subscriptions are only available on iOS.");
#endif
    }

    public void OnClickAutoRenewableSubscription()
    {
        BuyProductId(AutoRenewingSubscriptionId);
    }

    public void OnClickClose()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"OnInitializeFailed InitializationFailureReason:{error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log($"OnInitializeFailed InitializationFailureReason:{error}, {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"Product purchased! Transaction id: {args.purchasedProduct.transactionID}, Product id: {args.purchasedProduct.definition.id}");
#if UNITY_IOS
        JustTrackSDK.ForwardTransactionId(args.purchasedProduct.transactionID, args.purchasedProduct.definition.id, 1);
#endif

#if UNITY_ANDROID
        string receipt = args.purchasedProduct.receipt;
        var receiptWrapper = JObject.Parse(receipt);
        var payload = receiptWrapper["Payload"]?.ToString();

        var googleReceipt = JObject.Parse(payload);
        var json = JObject.Parse(googleReceipt["json"]?.ToString());
        string token = json["purchaseToken"]?.ToString();

        JustTrack.ProductType jtType = args.purchasedProduct.definition.type switch
        {
            UnityEngine.Purchasing.ProductType.Consumable => JustTrack.ProductType.INAPP,
            UnityEngine.Purchasing.ProductType.NonConsumable => JustTrack.ProductType.INAPP,
            UnityEngine.Purchasing.ProductType.Subscription => JustTrack.ProductType.SUBS,
            _ => JustTrack.ProductType.INAPP
        };

        JustTrackSDK.ForwardTransaction(token, args.purchasedProduct.definition.id, new Money(10.0, "USD"), jtType);
#endif

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"OnPurchaseFailed: FAIL. Product: {product.definition.storeSpecificId}, PurchaseFailureReason: {failureReason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"OnPurchaseFailed: FAIL. Product id: {failureDescription.item.Product.definition.id}, Message: {failureDescription.message}");
    }

    private void BuyProductId(string productId)
    {
        if (storeController == null)
        {
            Debug.Log("BuyProductId: FAIL. Store not initialized yet.");
            return;
        }

        var product = storeController.products.WithID(productId);

        if (product?.availableToPurchase == true)
        {
            Debug.Log($"Purchasing product id: {product.definition.id}");
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.Log("BuyProductId: FAIL. Not purchasing product, either is not found or is not available for purchase");
        }
    }
}