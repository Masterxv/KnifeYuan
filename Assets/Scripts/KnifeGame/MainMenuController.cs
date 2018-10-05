using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KnifeGame
{
    public class MainMenuController : MonoBehaviourHelper
    {
        public GameObject ChooseLevelPrefab;
        public GameObject MainMenuCanvas;
        public GameObject SettingCanvas;

        public Button PlayButton;
        public Button ChooseButton;
        public Button ShopButton;
        public Button SettingButton;

        private GameObject _chooseLevelObject;
        private bool _musicIsMuted;
        private bool _soundIsMuted;

        private void Awake()
        {
//            Application.targetFrameRate = Application.isMobilePlatform ? 30 : 60;
            MainMenuCanvas.SetActive(true);
            SettingCanvas.SetActive(false);
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
        }

        private void SettingPressed()
        {
            audioManager.PlayButtonClick();
            MainMenuCanvas.SetActive(false);
            SettingCanvas.SetActive(true);
        }

        public void HomePressed()
        {
            audioManager.PlayButtonClick();
            _chooseLevelObject.SetActive(false);
            MainMenuCanvas.SetActive(true);
        }

        public void SettingBackHome()
        {
            audioManager.PlayButtonClick();
            MainMenuCanvas.SetActive(true);
            SettingCanvas.SetActive(false);
        }

        public void MusicToggle(Toggle t)
        {
            _musicIsMuted = t.isOn;
            if (_musicIsMuted)
            {
                audioManager.ChangeVolume(TagAndString.MUSIC_VOLUME, -80);
            }
            else
            {
                var v = Util.GetLastMusicVolume();
                audioManager.ChangeVolume(TagAndString.MUSIC_VOLUME, v);
            }
        }

        public void MusicSlider(Slider s)
        {
            var res = s.value;
            if (_musicIsMuted)
            {
                Util.SetLastMusicVolume(res);
                audioManager.ChangeVolume(TagAndString.MUSIC_VOLUME, -80);
            }
            else
            {
                Util.SetLastMusicVolume(res);
                audioManager.ChangeVolume(TagAndString.MUSIC_VOLUME, res);
            }
        }

        public void SoundFxToggle(Toggle t)
        {
            _soundIsMuted = t.isOn;
            if (_soundIsMuted)
            {
                audioManager.ChangeVolume(TagAndString.SOUND_VOLUME, -80);
            }
            else
            {
                var v = Util.GetLastSoundVolume();
                audioManager.ChangeVolume(TagAndString.SOUND_VOLUME, v);
            }
        }

        public void SoundFxSlider(Slider s)
        {
            var res = s.value;
            if (_soundIsMuted)
            {
                Util.SetLastSoundVolume(res);
                audioManager.ChangeVolume(TagAndString.SOUND_VOLUME, -80);
            }
            else
            {
                Util.SetLastSoundVolume(res);
                audioManager.ChangeVolume(TagAndString.SOUND_VOLUME, res);
            }
        }

        void Update()
        {
        }
    }
}