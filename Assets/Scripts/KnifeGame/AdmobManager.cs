using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace KnifeGame
{
    public class AdmobManager : MonoBehaviour
    {
        private RewardBasedVideoAd _rewardBasedVideo;
        private string _appId;
        private string _adUnitId;

        private void Start()
        {
#if UNITY_ANDROID
            _appId = "ca-app-pub-3346638026613039~7552764882";
#elif UNITY_IPHONE
            _appId = "ca-app-pub-3940256099942544~1458002511";
        #else
            _appId = "unexpected_platform";
        #endif

            MobileAds.Initialize(_appId);
            _rewardBasedVideo = RewardBasedVideoAd.Instance;

            _rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
            _rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            _rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
            _rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
            _rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
            _rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
            _rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

            RequestRewardBasedVideo();
        }

        private void RequestRewardBasedVideo()
        {
#if UNITY_ANDROID
            _adUnitId = "ca-app-pub-3346638026613039/9604213150";
#elif UNITY_IPHONE
            _adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
            _adUnitId = "unexpected_platform";
        #endif

            var request = new AdRequest.Builder().Build();
            _rewardBasedVideo.LoadAd(request, _adUnitId);
        }

        public void ShowAds()
        {
            if (_rewardBasedVideo.IsLoaded())
            {
                _rewardBasedVideo.Show();
            }
        }

        private void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            print("HandleRewardBasedVideoLoaded event received");
        }

        private void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            print(
                "HandleRewardBasedVideoFailedToLoad event received with message: "
                + args.Message);
        }

        private void HandleRewardBasedVideoOpened(object sender, EventArgs args)
        {
            print("HandleRewardBasedVideoOpened event received");
        }

        private void HandleRewardBasedVideoStarted(object sender, EventArgs args)
        {
            print("HandleRewardBasedVideoStarted event received");
        }

        private void HandleRewardBasedVideoClosed(object sender, EventArgs args)
        {
            print("HandleRewardBasedVideoClosed event received");
        }

        private void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            print(
                "HandleRewardBasedVideoRewarded event received for "
                + amount.ToString() + " " + type);
        }

        private void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
        {
            print("HandleRewardBasedVideoLeftApplication event received");
        }
    }
}