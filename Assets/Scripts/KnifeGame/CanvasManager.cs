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

        private void Awake()
        {
        }

       private void Start()
        {
            PauseText.gameObject.SetActive(false);
            FadeBackground();

            AppleText.text = gameManager.Apple.ToString();
            ScoreText.text = gameManager.Score.ToString();
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
            _backgroundFade.DOFade(1, 1f).SetEase(Ease.InQuart).OnComplete(FadeOut)
                .OnComplete(gameManager.SetGameToReady);
        }
    }
}