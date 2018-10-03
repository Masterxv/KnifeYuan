using UnityEngine;
using UnityEngine.Audio;

namespace KnifeGame
{
    public class AudioManager : MonoBehaviourHelper
    {
        [SerializeField] private AudioMixerSnapshot _mainMenuSnapshot;
        [SerializeField] private AudioMixerSnapshot _gamePlaySnapshot;

        [SerializeField] private AudioSource _backgroundAu;
        [SerializeField] private AudioSource _effectAu;

        [SerializeField] private AudioClip _bgMenu;
        [SerializeField] private AudioClip _bgGamePlay;
        [SerializeField] private AudioClip _hitFruitClip;
        [SerializeField] private AudioClip _hitCircleClip;
        [SerializeField] private AudioClip _hitDeathFruitClip;
        [SerializeField] private AudioClip _hitObstacleClip;
        [SerializeField] private AudioClip _hitObstacleShapeClip;
        [SerializeField] private AudioClip _hitKnifeClip;
        [SerializeField] private AudioClip _clickClip;


        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            BackgroundMenu();
        }

        private void BackgroundMenu()
        {
            _backgroundAu.clip = _bgMenu;
            _backgroundAu.loop = true;
            _mainMenuSnapshot.TransitionTo(0.01f);
            _backgroundAu.Play();
        }

        public void BackgroundGamePlay()
        {
            _backgroundAu.clip = _bgGamePlay;
            _backgroundAu.loop = true;
            _gamePlaySnapshot.TransitionTo(0.01f);
            _backgroundAu.Play();
        }

        public void PlayButtonClick()
        {
            _effectAu.PlayOneShot(_clickClip);
        }

        public void PlayHitFruitSound()
        {
            _effectAu.PlayOneShot(_hitFruitClip);
        }

        public void PlayHitCircleSound()
        {
            _effectAu.PlayOneShot(_hitCircleClip);
        }

        public void PlayHitDeathFruitSound()
        {
            _effectAu.PlayOneShot(_hitDeathFruitClip);
        }

        public void PlayHitKnifeSound()
        {
            _effectAu.PlayOneShot(_hitKnifeClip);
        }

        public void PlayObstacleSound()
        {
            _effectAu.PlayOneShot(_hitObstacleClip);
        }

        public void PlayObstacleShapeSound()
        {
            _effectAu.PlayOneShot(_hitObstacleShapeClip);
        }
    }
}