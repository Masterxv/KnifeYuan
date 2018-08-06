using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KnifeGame
{
    public class TargetController : MonoBehaviour
    {
        [SerializeField] private float _stopTimeMin = 2;
        [SerializeField] private float _stopTimeMax = 5;
        [SerializeField] [Range(0, 1)] private float _directionChance;
        [SerializeField] private float _speedUp = 0.1f;
        [SerializeField] private float _slowdownSpeed = 2f;
        [SerializeField] private float _rotationSpeed = 5;
        [SerializeField] private float _stopTime;

        private float _timeToStop;
        private bool _IsSpeedUp;
        private int _direction = 1;
        private float _speed;
        private float _stopTimeVar;


        void Start()
        {
            _timeToStop = Random.Range(_stopTimeMin, _stopTimeMax / 2);
            _speed = _rotationSpeed;
            _stopTimeVar = _stopTime;
        }

        void Update()
        {
            if (GameState.GetGameState() == State.Paused)
                return;
            RotateTarget();
        }

        void RotateTarget()
        {
            if (_IsSpeedUp)
            {
                _speed += _speedUp * Time.deltaTime;
                transform.Rotate(Vector3.forward * _direction * _speed * Time.deltaTime);
                if (_speed > _rotationSpeed)
                {
                    _speed = _rotationSpeed;
                    _IsSpeedUp = false;
                }

                return;
            }

            if (_timeToStop <= 0.0f)
            {
                if (_speed <= 0.1f)
                {
                    if (_stopTimeVar <= 0)
                    {
                        _timeToStop = Random.Range(_stopTimeMin, _stopTimeMax);
                        _stopTimeVar = _stopTime;
                        _speed += _speedUp * Time.deltaTime;
                        if (Random.value <= _directionChance) _direction = -_direction;
                        transform.Rotate(Vector3.forward * _direction * _speed * Time.deltaTime);
                        _IsSpeedUp = true;
                    }
                    else
                        _stopTimeVar -= Time.deltaTime;
                }
                else
                {
                    _speed -= _slowdownSpeed * Time.deltaTime;
                    transform.Rotate(Vector3.forward * _direction * _speed * Time.deltaTime);
                }
            }
            else
            {
                _timeToStop -= Time.deltaTime;
                transform.Rotate(Vector3.forward * _direction * _rotationSpeed * Time.deltaTime);
            }
        }

        public void PlayHitImpact()
        {
            transform.DOPunchScale(new Vector3(1, 1, 1), 0.2f, 5, 10).SetEase(Ease.InElastic);
        }

        public void KnifeHitTarget()
        {
            transform.DOShakeScale(duration: 0.1f, strength: 0.5f, vibrato: 50, randomness: 1, fadeOut: true)
                .SetEase(Ease.InSine);
        }
    }
}