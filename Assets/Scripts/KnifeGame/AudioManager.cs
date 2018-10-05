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

        [SerializeField] private AudioMixer _audioMixer;
        private const float LowerVolumeBound = -80.0f;
        private const float UpperVolumeBound = 0.0f;
        private bool _muted;
        private float _volumeBeforMute;

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
            _effectAu.Stop();
            _effectAu.PlayOneShot(_clickClip);
        }

        public void PlayHitFruitSound()
        {
            _effectAu.Stop();
            _effectAu.PlayOneShot(_hitFruitClip);
        }

        public void PlayHitCircleSound()
        {
            _effectAu.Stop();
            _effectAu.PlayOneShot(_hitCircleClip);
        }

        public void PlayHitDeathFruitSound()
        {
            _effectAu.Stop();
            _effectAu.PlayOneShot(_hitDeathFruitClip);
        }

        public void PlayHitKnifeSound()
        {
            _effectAu.Stop();
            _effectAu.PlayOneShot(_hitKnifeClip);
        }

        public void PlayObstacleSound()
        {
            _effectAu.Stop();
            _effectAu.PlayOneShot(_hitObstacleClip);
        }

        public void PlayObstacleShapeSound()
        {
            _effectAu.Stop();
            _effectAu.PlayOneShot(_hitObstacleShapeClip);
        }

        public void ChangeVolume(string volName, float value)
        {
            _audioMixer.SetFloat(volName, value);
        }

        public void GetVolume(string volName)
        {
            var res = 0.0f;
            _audioMixer.GetFloat(volName, out res);
        }

        #region AUDIO_MIXER_CONTROLLER

//        public float Volume
//        {
//            get
//            {
//                if (_muted)
//                    return _volumeBeforMute;
//                else
//                    return VolumeNoMutedGuard;
//            }
//            set
//            {
//                var newVolume = Mathf.Clamp01(value);
//                if (_muted)
//                    _volumeBeforMute = newVolume;
//                else
//                    VolumeNoMutedGuard = newVolume;
//            }
//        }
//
//        private float VolumeNoMutedGuard
//        {
//            get
//            {
//                var volume = 0.0f;
//                if (_audioMixer)
//                {
//                    _audioMixer.GetFloat("", out volume);
//                }
//
//                volume = Mathf.InverseLerp(LowerVolumeBound, UpperVolumeBound, volume);
//                return volume;
//            }
//            set
//            {
//                if (_audioMixer)
//                {
//                    _audioMixer.SetFloat("", Mathf.Lerp(LowerVolumeBound, UpperVolumeBound, value));
//                }
//            }
//        }
//
//        public bool Muted
//        {
//            get { return _muted; }
//            set
//            {
//                _muted = value;
//
//                if (_muted)
//                {
//                    _volumeBeforMute = VolumeNoMutedGuard;
//                    VolumeNoMutedGuard = 0.0f;
//                }
//                else
//                {
//                    VolumeNoMutedGuard = _volumeBeforMute;
//                }
//            }
//        }
//
//        private void OnEnable()
//        {
//            _muted = false;
//        }

        #endregion
    }
}