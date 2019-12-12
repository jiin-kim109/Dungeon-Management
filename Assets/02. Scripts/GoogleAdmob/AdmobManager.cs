using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdmobManager : MonoBehaviour {

    private BannerView bannerView;

    private string appID;
    private string bannerID;

    private bool isTest;

    void Start()
    {
        appID = "ca-app-pub-5147846202532909~3403360134";
        bannerID = "ca-app-pub-5147846202532909/5729499940";
        isTest = false;

        MobileAds.Initialize(appID);
        this.RequestBanner();
    }

    private void RequestBanner()
    {
        bannerView = new BannerView(bannerID, AdSize.Banner, AdPosition.Bottom);

        /*if (isTest)
        {
            request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator)
                .AddTestDevice("TEST").Build();
        }*/
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
        bannerView.Show();
    }

    public void OnClick_Banner()
    {

    }

    public void DestroyAdmob()
    {
        bannerView.Destroy();
    }
}
