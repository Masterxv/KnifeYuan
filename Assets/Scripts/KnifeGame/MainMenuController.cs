using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KnifeGame
{
    public class MainMenuController : MonoBehaviourHelper
    {
        public GameObject ChooseLevelPrefab;
        public GameObject MainMenuGroup;

        public Button PlayButton;
        public Button ChooseButton;
        public Button ShopButton;
        public Button SettingButton;

        private GameObject _chooseLevelObject;

        private void Awake()
        {
            Application.targetFrameRate = Application.isMobilePlatform ? 30 : 60;
        }

        void Start()
        {
            ButtonAddListener();
        }

        private void PlayGame()
        {
            Util.ChooseLevelBool = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private void ChooseLevelPressed()
        {
            if (_chooseLevelObject == null)
                _chooseLevelObject = Instantiate(ChooseLevelPrefab, Vector3.zero, Quaternion.identity);
            else
                _chooseLevelObject.gameObject.SetActive(true);

            MainMenuGroup.SetActive(false);
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
            print("shop");
        }

        private void SettingPressed()
        {
            print("setting");
        }

        public void HomePressed()
        {
            _chooseLevelObject.SetActive(false);
            MainMenuGroup.SetActive(true);
        }

        void Update()
        {
        }
    }
}