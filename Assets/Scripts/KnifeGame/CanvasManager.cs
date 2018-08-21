using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KnifeGame
{
    public class CanvasManager : MonoBehaviourHelper
    {
        public Text PauseText;
        public Text AppleText;
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

        private Sequence _sequence;
        private Sequence _blinkSequence;

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
            PreviousButton.gameObject.SetActive(false);
            NextButton.gameObject.SetActive(false);
            
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

            var endColor = Color.white;
            var startColor = constant.RandomBrightColor();
            startColor.a = 1;

            FillTheScene.color = gameManager.winLevel ? startColor : Color.black;

            _sequence?.Kill();
            _blinkSequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(FillTheScene.rectTransform.DOScale(transform.localScale * 6, 0.7f).SetEase(Ease.InCirc));
            _sequence.SetLoops(1);

            _sequence.Append(FillTheScene.DOColor(endColor, 0.5f).SetEase(Ease.InCirc));
            _sequence.OnStepComplete(ZoomImageOut);
            _sequence.Play();
        }

        private void ZoomImageOut()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(FillTheScene.rectTransform.DOScale(new Vector3(1, 1, 1), 1.2f).SetEase(Ease.InExpo));
            _sequence.PrependInterval(0.1f); // add interval time at the begining of sequence 
            _sequence.SetLoops(1);
            _sequence.OnStepComplete(gameManager.CreateGame);
            _sequence.Play();
        }

        public void ShowReadyText()
        {
            ReadyText.gameObject.SetActive(true);
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(ReadyText.DOColor(ReadyTextStartColor, 0.1f));
            _sequence.SetEase(Ease.OutExpo).SetLoops(1, LoopType.Incremental);
            _sequence.OnComplete(ShowReadyCore);
        }

        private void ShowReadyCore()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(ReadyText.DOColor(ReadyTextEndColor, 0.2f));
            _sequence.SetEase(Ease.OutCirc).SetLoops(3, LoopType.Yoyo).OnComplete(DeactiveReadyText);
        }

        private void DeactiveReadyText()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(ReadyText.DOFade(0, 0.1f).SetEase(Ease.InBounce).OnComplete(gameManager.SetGameReady));
        }

        public void BlinkBackGround()
        {
            _blinkSequence?.Kill();
            _blinkSequence = DOTween.Sequence();

            _blinkSequence.Append(BlinkBackground.DOColor(BlinkStartColor, 0.12f).SetEase(Ease.OutExpo));
            _blinkSequence.SetLoops(1, LoopType.Incremental).OnComplete(BlinkCore);
        }

        private void BlinkCore()
        {
            _blinkSequence?.Kill();
            _blinkSequence = DOTween.Sequence();

            _blinkSequence.Append(BlinkBackground.DOColor(BlinkEndColor, 0.5f).SetEase(Ease.OutCirc));
            _blinkSequence.SetLoops(5, LoopType.Yoyo).OnComplete(BlinkBackgroundEnd);
        }

        private void BlinkBackgroundEnd()
        {
            _blinkSequence?.Kill();
            _blinkSequence = DOTween.Sequence();
            _blinkSequence.Append(BlinkBackground.DOFade(0, 0.1f)).SetEase(Ease.OutBounce);
            _blinkSequence.OnComplete(ZoomImageIn);
        }

        public void ShowGroup(bool value)
        {
            LevelAndButtonGroup.SetActive(value);
            ScoreAndAppleGroup.SetActive(value);
            SetLevelName();
        }

        private void SetLevelName()
        {
            LevelName.text = "Level " + gameManager.LevelIndex;
        }

        public void CheckToShowButton()
        {
            PreviousButton.gameObject.SetActive(Util.ActiveButtonPrevious());

            NextButton.gameObject.SetActive(Util.ActiveButtonNext());
        }

        private void AddButtonListener()
        {
            PreviousButton.onClick.AddListener(PreviousPress);
            NextButton.onClick.AddListener(NextPress);
        }

        private void NextPress()
        {
            print("next");
        }

        private void PreviousPress()
        {
            print("previous");
        }

        private void OnDestroy()
        {
            _blinkSequence.Kill();
            _sequence.Kill();
        }
    }
}