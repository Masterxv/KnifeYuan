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
        [SerializeField] private Image _backgroundFade;
        [SerializeField] private Image _fillTheScene;
        private Sequence _sequence;

        private void Awake()
        {
        }

        private void Start()
        {
            PauseText.gameObject.SetActive(false);
            FadeBackground();

            AppleText.text = gameManager.Apple.ToString();
            ScoreText.text = gameManager.Score.ToString();

            // animate the fill-level image
//            MovePositionImage();
            ZoomImageIn();
        }

        public void FadeBackground()
        {
            if (_backgroundFade.color.a > 0)
            {
                FadeOut();
            }
            else
            {
                FadeIn();
            }
        }

        private void FadeOut() // mo dan
        {
            _backgroundFade.DOFade(0, 0.5f).SetEase(Ease.InQuart);
        }

        private void FadeIn()
        {
            _backgroundFade.DOFade(1, 1f).SetEase(Ease.InQuart).OnComplete(FadeOut);
        }

        private void ZoomImageIn()
        {
            var endColor = Color.white;
            var startColor = constant.RandomBrightColor();
            startColor.a = 1;

            _fillTheScene.color = gameManager.winLevel ? startColor : Color.black;
            
            _sequence?.Kill();
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
    }
}