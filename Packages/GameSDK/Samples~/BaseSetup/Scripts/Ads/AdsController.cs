using System;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
// using Manager;
using R3;
using SDK;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.Events;
#pragma warning disable CS0162 // Unreachable code detected

public class AdsController : Singleton<AdsController>
{
    public static Action EventShowBanner { get; set; }
    public static Action EventHideBanner { get; set; }
    public static bool IsRemoveAds {get; private set;}
    
    [field: SerializeField] public float InterCapping {get; private set;}
    [field: SerializeField] public float InterCappingAfterReward {get; private set;}
    [field: SerializeField] public float InterCooldown {get; private set;}
    [field: SerializeField] public bool InterReady { get; private set; } = true;
    [field: SerializeField] public static bool InterJustShowed { get; set; } = false;

    protected override void Awake()
    {
        base.Awake();
        AddEvent();

        Initialize();
    }
    private void OnDestroy()
    {
        RemoveEvent();
    }

    public void Initialize()
    {
        EventAdsManager.ShowInterstitialAds += ShowAdsInter;
        EventAdsManager.ShowRewardAds += ShowAdsReward;
    }

    public void GetRemoteConfig()
    {
        InterCapping = (float)FirebaseManager.Instance.GetConfigValue(Keys.key_inter_capping).DoubleValue;
        InterCappingAfterReward = (float)FirebaseManager.Instance.GetConfigValue(Keys.key_inter_capping_after_reward).DoubleValue;
    }

    private void ClearEvent()
    {
        EventShowBanner = null;
        EventHideBanner = null;
    }
    
    public void AddEvent()
    {
        ClearEvent();
        EventShowBanner += ShowBanner;
        EventHideBanner += HideBanner;
    }
    public void RemoveEvent()
    {
        EventShowBanner -= ShowBanner;
        EventHideBanner -= HideBanner;
    }

    private void CheckRemoveAds(float value)
    {
        IsRemoveAds = value > 0;
        if(IsRemoveAds)
            HideBanner();
    }

    public bool CheckCanShowAds()
    {
        return true;
    }
    
    [Button]
    public void ShowAdsReward(Action successAction, Action failAction, Action closeAction, bool isSkipCapping = false, string eventName = "", params AnalyticsParameter[] parameters)
    {
        AdsManager.Instance.ShowRewardVideo(
        () =>
            {
                successAction?.Invoke();
                InterCooldown += InterCappingAfterReward;
                InterCooldown = Mathf.Clamp(InterCooldown, 0, InterCapping);
                EventTrackingManager.TrackEventFirebase?.Invoke(eventName, parameters);
                AppsFlyerManager.TrackRewarded_Displayed();
            },
            () =>
            {
                closeAction?.Invoke();
            },
            () =>
            {
                FailToShowdAds(failAction);
            }
        );
    }

    private void FailToShowdAds(Action failAction = null)
    {
        failAction?.Invoke();

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Fail to show reward ads because of no internet connection");
            // ActivityBlockContext.Events.CallNotification?.Invoke(528);
        }
        else
        {
            Debug.Log("Fail to show reward ads because of other reason");
            // ActivityBlockContext.Events.CallNotification?.Invoke(535);
        }
    }
    
    
    [Button]
    public void ShowAdsInter(Action successAction, Action failAction, Action closeAction, bool isSkipCapping = false, bool isBackfillInter = false, string eventName = "", params AnalyticsParameter[] parameters)
    {
        SDKDebugLogger.Log($"ShowAdsInter: {placementId}");

        // Check Remove Ads

        if (!InterReady && !isSkipCapping)
        {
            successAction?.Invoke();
            return;
        }
        AdsManager.Instance.ShowInterstitial(
            () =>
            {
                InterCooldown = InterCapping;
                successAction?.Invoke();
                InterCooldown = Mathf.Clamp(InterCooldown, 0, InterCapping);
                EventTrackingManager.TrackEventFirebase?.Invoke(eventName, parameters);
                AppsFlyerManager.TrackInterstitial_Displayed();
            },
            () =>
            {
                closeAction?.Invoke();
            },
            () =>
            {
                FailToShowdAds(failAction);
            }
        );
        InterCooldown = InterCapping;
        InterJustShowed = true;
    }
    [Button]
    public void ShowBanner()
    {
        // if (!IsRemoveAds /*&& GameManager.Instance.ShowBanner*/
        //     && GameManager.Instance.Level.Value >= BannerStartShowLevel)
        // {
#if CHEAT_ONLY
            AdsManager.Instance.ShowBannerAds();
            return;
#endif
            AdsManager.Instance.ShowBannerAds();
        // }
    }
    [Button]
    public void HideBanner()
    {
        AdsManager.Instance.HideBannerAds();
    }

    private void Update()
    {
        if (InterCooldown > 0)
        {
            InterCooldown -= Time.deltaTime;
        }   
        InterReady = InterCooldown <= 0;
    }
}
