using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KnifeGame
{
    public class MainMenuController : MonoBehaviourHelper
    {
        public GameObject ChooseLevelPrefab;
        public GameObject MainMenuCanvas;

        public Button PlayButton;
        public Button ChooseButton;
        public Button ShopButton;
        public Button SettingButton;

        private GameObject _chooseLevelObject;

        private void Awake()
        {
//            Application.targetFrameRate = Application.isMobilePlatform ? 30 : 60;
            ButtonAddListener();
        }

        private void PlayGame()
        {
            audioManager.PlayButtonClick();
            Util.ChooseLevelBool = false;
            var nextScene = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextScene);
            audioManager.BackgroundGamePlay();
        }

        private void ChooseLevelPressed()
        {
            audioManager.PlayButtonClick();
            if (_chooseLevelObject == null)
                _chooseLevelObject = Instantiate(ChooseLevelPrefab, Vector3.zero, Quaternion.identity);
            else
                _chooseLevelObject.gameObject.SetActive(true);

            MainMenuCanvas.SetActive(false);
        }

        private void ButtonAddListener()
        {
            PlayButton.onClick.AddListener(PlayGame);
            ChooseButton.onClick.AddListener(ChooseLevelPressed);
            SettingButton.onClick.AddListener(SettingPressed);
            ShopButton.onClick.AddListener(ShopPressed);
        }

        private void ShopPressed()
        {
            audioManager.PlayButtonClick();
//            print("shop");
        }

        private void SettingPressed()
        {
            audioManager.PlayButtonClick();
//            print("setting");
        }

        public void HomePressed()
        {
            audioManager.PlayButtonClick();
            _chooseLevelObject.SetActive(false);
            MainMenuCanvas.SetActive(true);
        }

        void Update()
        {
        }
    }
}