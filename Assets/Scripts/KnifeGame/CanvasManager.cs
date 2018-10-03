using System;
using System.Collections;
using System.Threading;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KnifeGame
{
    public class CanvasManager : MonoBehaviourHelper
    {
        public Text PauseText;
        public TextMeshProUGUI AppleText;
        public Text ScoreText;
        public Image FillTheScene;
        public Image BlinkBackground;
        public GameObject LevelAndButtonGroup;
        public GameObject ScoreAndAppleGroup;
        public Color BlinkStartColor;
        public Color BlinkEndColor;
        public TextMeshProUGUI ReadyText;
        public Color ReadyTextStartColor;
        public Color ReadyTextEndColor;
        public Text LevelName;
        public Button PreviousButton;
        public Button NextButton;
        public Image HeadBackground;
        public GameObject TimerGroup;
        public Text Timer;

        private float _timeLeft;
        private float _min, _sec;

        private void Start()
        {
            ShowGroup(false);
            PauseText.gameObject.SetActive(false);

            AppleText.text = Util.GetApplePlayerGet().ToString();
            ResetScoreText();

            // animate the fill-level image
            ZoomImageIn();

            // blink background image
            BlinkBackground.color = new Color(0, 0, 0, 0);

            ReadyText.gameObject.SetActive(false);
            PreviousButton.GetComponent<Image>().enabled = false;
            NextButton.GetComponent<Image>().enabled = false;
            TimerGroup.SetActive(false);

            AddButtonListener();
        }

        public void ResetScoreText()
        {
            ScoreText.text = "0";
        }

        public void ZoomImageIn()
        {
            gameManager.GarbageCollect();
            BlinkBackground.color = new Color(0, 0, 0, 0); // set blick background's color to 0
            var startColor = constant.RandomBrightColor();
            startColor.a = 1;
            var looseColor = new Color(0.5f, 0.5f, 0.5f);
            FillTheScene.color = gameManager.winLevel ? startColor : looseColor;
            FillTheScene.rectTransform.DOScale(transform.localScale * 10, 0.8f).SetEase(Ease.InCirc)
                .OnComplete(ChangeColor);
        }

        private void ChangeColor()
        {
            var endColor = Color.white;
            FillTheScene.DOColor(endColor, 0.5f).SetEase(Ease.InCirc).OnComplete(ZoomImageOut);
        }

        private void ZoomImageOut()
        {
            var scale = new Vector3(1, 1, 1);
            FillTheScene.rectTransform.DOScale(scale, 1f).SetEase(Ease.OutCirc).OnComplete(gameManager.CreateGame);
        }

        public void ShowReadyText()
        {
            ReadyText.gameObject.SetActive(true);
            ReadyText.DOColor(ReadyTextStartColor, 0.1f).SetEase(Ease.OutExpo).OnComplete(ShowReadyCore);
        }

        private void ShowReadyCore()
        {
            ReadyText.DOColor(ReadyTextEndColor, 0.2f).SetEase(Ease.OutCirc).SetLoops(3, LoopType.Yoyo)
                .OnComplete(DeactiveReadyText);
        }

        private void DeactiveReadyText()
        {
            ReadyText.DOFade(0, 0.1f).SetEase(Ease.InBounce).OnComplete(gameManager.SetGameReady);
        }

        public void BlinkBackGround()
        {
            BlinkBackground.DOColor(BlinkStartColor, 0.12f).SetEase(Ease.OutExpo).OnComplete(BlinkCore);
        }

        private void BlinkCore()
        {
            BlinkBackground.DOColor(BlinkEndColor, 0.5f).SetEase(Ease.OutCirc).SetLoops(5, LoopType.Yoyo)
                .OnComplete(BlinkBackgroundEnd);
        }

        private void BlinkBackgroundEnd()
        {
            BlinkBackground.DOFade(0, 0.1f).SetEase(Ease.OutBounce).OnComplete(ZoomImageIn);
        }

        public void ShowGroup(bool value)
        {
            LevelAndButtonGroup.SetActive(value);
            ScoreAndAppleGroup.SetActive(value);
            HeadBackground.gameObject.SetActive(value);
            SetLevelName();
        }

        private void SetLevelName()
        {
            LevelName.text = "Level " + (gameManager.LevelIndex + 1);
        }

        public void StartClock(float t)
        {
            TimerGroup.SetActive(true);
            _timeLeft = t * 60;
            CalculateTime(t);
            StartCoroutine(CountDown(t));
        }

        public void StopClock()
        {
            StopAllCoroutines();
            TimerGroup.SetActive(false);
        }

        private void CalculateTime(float t)
        {
            _min = Mathf.FloorToInt(_timeLeft / 60);
            _sec = Mathf.FloorToInt(_timeLeft % 60);
            Timer.text = _min.ToString("00") + ":" + _sec.ToString("00");
        }

        private IEnumerator CountDown(float t)
        {
            _timeLeft = t * 60;
            _timeLeft++;
            while (_timeLeft > 0)
            {
                _timeLeft--;
                CalculateTime(_timeLeft);
                yield return new WaitForSeconds(1);
            }

            if (Math.Abs(_timeLeft) < 1)
            {
                gameManager.GameOver();
            }
        }

        public void CheckToShowButton()
        {
            PreviousButton.GetComponent<Image>().enabled = Util.ActiveButtonPrevious();
            NextButton.GetComponent<Image>().enabled = Util.ActiveButtonNext();
        }

        private void AddButtonListener()
        {
            PreviousButton.onClick.AddListener(PreviousPress);
            NextButton.onClick.AddListener(NextPress);
        }

        private void NextPress()
        {
            var level = Util.GetLastLevelPlayed();
            level++;
            Util.SetLastLevelPlayed(level);
            gameManager.winLevel = true;
            canvasManager.StopClock();
            canvasManager.ShowGroup(false);
            canvasManager.ZoomImageIn();
        }

        private void PreviousPress()
        {
            var level = Util.GetLastLevelPlayed();
            level--;
            Util.SetLastLevelPlayed(level);
            gameManager.winLevel = true;
            canvasManager.StopClock();
            canvasManager.ShowGroup(false);
            canvasManager.ZoomImageIn();
        }

        private void OnDestroy()
        {
            DOTween.Kill(transform);
        }
    }
}