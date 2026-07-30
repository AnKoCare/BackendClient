using System;
using TW.Utility.DesignPattern;

public delegate void ShowInterstitialAds(Action success, Action failed, Action closed, Action skip, bool isSkipCapping, bool isBackfillInter, string eventName, params AnalyticsParameter[] parameters);
public delegate void ShowRewardAds(Action success, Action failed, Action closed, bool isSkipCapping, string eventName, params AnalyticsParameter[] parameters);
public delegate void ShowBannerAds(bool isShow);

public class EventAdsManager : Singleton<EventAdsManager>
{
    public static ShowInterstitialAds ShowInterstitialAds;
    public static ShowRewardAds ShowRewardAds;
    public static ShowBannerAds ShowBannerAds;
}
