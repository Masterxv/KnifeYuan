using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KnifeGame
{
    public class CanvasManager : MonoBehaviourHelper
    {
        public Text PauseText;
        public Text AppleText;
        public Text ScoreText;
        [SerializeField] private Image _fillTheScene;
        [SerializeField] private Image _blinkBackground;
        [SerializeField] private GameObject _levelAndButtonGroup;
        private Sequence _sequence;
        public Color BlinkStartColor;
        public Color BlinkEndColor;
        private Sequence _blinkSequence;
        [HideInInspector] public int _blink = 0; // use this to end blink function

        private void Awake()
        {
        }

        private void Start()
        {
            _levelAndButtonGroup.SetActive(false);
            PauseText.gameObject.SetActive(false);
//            FadeBackground();

            AppleText.text = gameManager.Apple.ToString();
            ScoreText.text = gameManager.Score.ToString();

            // animate the fill-level image
            ZoomImageIn();

            // blink background image
            _blinkBackground.color = new Color(0, 0, 0, 0);
        }

//        public void FadeBackground()
//        {
//            if (_backgroundFade.color.a > 0)
//            {
//                FadeOut();
//            }
//            else
//            {
//                FadeIn();
//            }
//        }

//        private void FadeOut() // mo dan
//        {
//            _backgroundFade.DOFade(0, 0.5f).SetEase(Ease.InQuart);
//        }
//
//        private void FadeIn()
//        {
//            _backgroundFade.DOFade(1, 1f).SetEase(Ease.InQuart).OnComplete(FadeOut);
//        }

        public void ZoomImageIn()
        {
            _blinkBackground.color = new Color(0, 0, 0, 0); // set blick background's color to 0

            var endColor = Color.white;
            var startColor = constant.RandomBrightColor();
            startColor.a = 1;

            _fillTheScene.color = gameManager.winLevel ? startColor : Color.black;

            _sequence?.Kill();
            _blinkSequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_fillTheScene.rectTransform.DOScale(transform.localScale * 5, 0.7f).SetEase(Ease.InCirc));
            _sequence.SetLoops(1);

            _sequence.Append(_fillTheScene.DOColor(endColor, 0.5f).SetEase(Ease.InCirc));
            _sequence.OnStepComplete(ZoomImageOut);
            _sequence.Play();
        }

        private void ZoomImageOut()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_fillTheScene.rectTransform.DOScale(new Vector3(1, 1, 1), 1.2f).SetEase(Ease.InExpo));
            _sequence.PrependInterval(0.1f); // add interval time at the begining of sequence 
            _sequence.SetLoops(1);
            _sequence.OnStepComplete(gameManager.CreateGame);
            _sequence.Play();
        }

        public void BlinkBackground() // blink use for game over
        {
            // check a condition to stop 
            _blink++;
            _blinkSequence?.Kill();
            _blinkSequence = DOTween.Sequence();
            _blinkSequence.Append(_blinkBackground.DOColor(BlinkStartColor, 0.2f).SetEase(Ease.InCirc));
            _blinkSequence.SetLoops(1, LoopType.Incremental);
            _blinkSequence.OnComplete(BlinkBackgroundEnd);
        }

        private void BlinkBackgroundEnd()
        {
            _blink++;
            _blinkSequence?.Kill();
            _blinkSequence = DOTween.Sequence();
            _blinkSequence.Append(_blinkBackground.DOColor(BlinkEndColor, 0.4f).SetEase(Ease.InCirc));
            _blinkSequence.SetLoops(1, LoopType.Incremental);
            if (_blink > 6)
            {
                gameManager.GarbageCollect();
                _blinkSequence.OnComplete(ZoomImageIn);
            }
            else
            {
                _blinkSequence.OnComplete(BlinkBackground);
            }
        }

        public void ResetBlink()
        {
            _blink = 0;
        }
        public void ShowLevel()
        {
            _levelAndButtonGroup.SetActive(true);
        }

        private void SetLevelName()
        {
        }

        private void OnDestroy()
        {
            _blinkSequence.Kill();
            _sequence.Kill();
        }
    }
}