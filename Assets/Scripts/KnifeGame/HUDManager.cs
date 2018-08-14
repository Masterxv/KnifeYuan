using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KnifeGame
{
    public class HUDManager : MonoBehaviour
    {
        public Text PauseText;
        public Text AppleText;
        public Text ScoreText;
        [SerializeField] private Image _backgroundFade;
        public Button RestartButton;

        private static HUDManager _instance;

        public static HUDManager Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }

        void Start()
        {
            PauseText.gameObject.SetActive(false);
            // add function to restart button at Inspector
            RestartButton.gameObject.SetActive(false);
            FadeBackground();

            AppleText.text = GameManager.Instance.Apple.ToString();
            ScoreText.text = GameManager.Instance.Score.ToString();
        }

        void Update()
        {
        }

        public void FadeBackground()
        {
            if (_backgroundFade.color.a > 0)
            {
                FadeOut();
            }
            else
            {
                print("fade in");

                FadeIn();
            }
        }

        private void FadeOut() // mo dan
        {
            _backgroundFade.DOFade(0, 0.5f).SetEase(Ease.InQuart).OnComplete(GameManager.Instance.SetGameToReady);
        }

        private void FadeIn()
        {
            _backgroundFade.DOFade(1, 1f).SetEase(Ease.InQuart).OnComplete(FadeOut)/*.OnComplete(GameManager.Instance.SetGameToReady)*/;
        }
    }
}