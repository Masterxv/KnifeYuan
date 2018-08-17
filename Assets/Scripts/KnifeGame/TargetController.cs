using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KnifeGame
{
    public enum RotateType
    {
        LEFT,
        RIGHT,
        BOTH,
        LEFT_SHAKE,
        RIGHT_SHAKE,
        LEFT_RIGHT_SHAKE
    }

    public class TargetController : MonoBehaviourHelper
    {
        [SerializeField] private float _stopTimeMin = 2;
        [SerializeField] private float _stopTimeMax = 5;

        [SerializeField]
        [Tooltip("The velocity of rotation, fast or slow, this * 360 degrees, should set from 0.5 to 1.2")]
        private float _velocity;


        [SerializeField] private float _delayTime;
        [SerializeField] private RotateType _rotateType = RotateType.LEFT;
        [SerializeField] private Ease _easeType = Ease.Linear;
        [SerializeField] private LoopType _loopType = LoopType.Incremental;

        [Tooltip("The circle appears with this type")] [SerializeField]
        private Ease _circleEaseType = Ease.OutBounce;


        /// <summary>
        /// private parameters, variables
        /// </summary>
        private float _interval; // time to add between tween loop

        private Ease _shakeEaseType;
        private Sequence _sequence;
        private float _angle;
        private float _timeToStop; // this is MAIN parameter to rotate the target
        private Vector3 _rotateVector = new Vector3(0, 0, 1);

//        [SerializeField] [Range(0, 1)] private float _directionChance;
//        [SerializeField] private float _speedUp = 0.1f;
//        [SerializeField] private float _slowdownSpeed = 2f;
//        [SerializeField] private float _rotationSpeed = 5;
//        [SerializeField] private float _stopTime;
//        private bool _IsSpeedUp;
//        private int _direction = 1;
//        private float _speed;
//        private float _stopTimeVar;
        private void Awake()
        {
            transform.localScale *= 0.5f;

            transform.DOScale(Vector3.one, 1f).SetDelay(0.5f).SetEase(_circleEaseType).OnComplete(LaunchTheRotation);
        }

        private void LaunchTheRotation()
        {
            gameManager.SetGameToReady(); // set the game to ready
            DOTween.Kill(transform); // kill tween from doScale

            switch (_rotateType)
            {
                case RotateType.LEFT:
                    RotateToLeft(); // note that ease type: Out Expo will make rotation smoothly
                    break;
                case RotateType.RIGHT:
                    RotateToRight();
                    break;
                case RotateType.BOTH:
                    RotateLeftRight();
                    break;
                case RotateType.LEFT_SHAKE:
                    RotateLeftAndShake();
                    break;
                case RotateType.RIGHT_SHAKE:
                    RotateRightAndShake();
                    break;
                case RotateType.LEFT_RIGHT_SHAKE:
                    RotateLRShake();
                    break;
            }
        }

        private void RotateLRShake()
        {
            ConfigureRotate();
            _rotateVector *= -1;

            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_easeType));

            _sequence.PrependInterval(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(ShakeLR);
            _sequence.Play();
        }

        private void ShakeLR()
        {
            ConfigureShake();
            _rotateVector *= -1;
            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_shakeEaseType));

            _sequence.PrependInterval(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateLRShake);
            _sequence.Play();
        }

        private void RotateRightAndShake()
        {
            ConfigureRotate();

            _sequence.Append(transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_easeType));

            _sequence.PrependInterval(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(ShakerRight);
            _sequence.Play();
        }

        private void ShakerRight()
        {
            ConfigureShake();
            _sequence.Append(transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_shakeEaseType));

            _sequence.PrependInterval(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateRightAndShake);
            _sequence.Play();
        }

        private void RotateLeftAndShake()
        {
            ConfigureRotate();

            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_easeType));

            _sequence.PrependInterval(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(ShakerLeft);
            _sequence.Play();
        }

        private void ShakerLeft()
        {
            ConfigureShake();

            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_shakeEaseType));

            _sequence.PrependInterval(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateLeftAndShake);
            _sequence.Play();
        }

        private void ConfigureShake()
        {
            if (GameState.GetGameState() == State.Paused)
                return;
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _timeToStop = Random.Range(_stopTimeMin, _stopTimeMax * 0.6f);
            _angle = 360f * _velocity * _timeToStop;
            _angle += Random.Range(0f, 15f);

            _interval = Random.Range(0f, _delayTime * 0.5f);
            _shakeEaseType = Random.value > 0.5f ? Ease.InBounce : Ease.OutBounce;
        }

        private void RotateLeftRight()
        {
            ConfigureRotate();
            _rotateVector *= -1;

            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop,
                RotateMode.FastBeyond360).SetEase(_easeType));

            _sequence.PrependInterval(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateLeftRight);
            _sequence.Play();
        }

        private void RotateToRight()
        {
            ConfigureRotate();
            _sequence.Append(transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop,
                RotateMode.FastBeyond360).SetEase(_easeType));

            _sequence.PrependInterval(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateToRight);
            _sequence.Play();
        }

        private void RotateToLeft()
        {
            ConfigureRotate();
            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop,
                RotateMode.FastBeyond360).SetEase(_easeType));

            _sequence.PrependInterval(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateToLeft);
            _sequence.Play();
        }

        private void ConfigureRotate()
        {
            if (GameState.GetGameState() == State.Paused)
                return;
            DOTween.Kill(transform);
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _timeToStop = Random.Range(_stopTimeMin, _stopTimeMax);
            _angle = 360f * _velocity * _timeToStop;
            _angle += Random.Range(0f, 30f);
            _interval = Random.Range(0, _delayTime);
        }

        void Start()
        {
        }

        void Update()
        {
        }

        void RotateTarget()
        {
//            if (Time.timeSinceLevelLoad < _delayTime)
//            {
//                return;
//            }
//
//            if (_IsSpeedUp)
//            {
//                _speed += _speedUp * Time.deltaTime;
//                transform.Rotate(Vector3.forward * _direction * _speed * Time.deltaTime);
//                if (_speed > _rotationSpeed)
//                {
//                    _speed = _rotationSpeed;
//                    _IsSpeedUp = false;
//                }
//
//                return;
//            }
//
//            if (_timeToStop <= 0.0f)
//            {
//                if (_speed <= 0.1f)
//                {
//                    if (_stopTimeVar <= 0)
//                    {
//                        _timeToStop = Random.Range(_stopTimeMin, _stopTimeMax);
//                        _stopTimeVar = _stopTime;
//                        _speed += _speedUp * Time.deltaTime;
//                        if (Random.value <= _directionChance) _direction = -_direction;
//                        transform.Rotate(Vector3.forward * _direction * _speed * Time.deltaTime);
//                        _IsSpeedUp = true;
//                    }
//                    else
//                        _stopTimeVar -= Time.deltaTime;
//                }
//                else
//                {
//                    _speed -= _slowdownSpeed * Time.deltaTime;
//                    transform.Rotate(Vector3.forward * _direction * _speed * Time.deltaTime);
//                }
//            }
//            else
//            {
//                _timeToStop -= Time.deltaTime;
//                transform.Rotate(Vector3.forward * _direction * _rotationSpeed * Time.deltaTime);
//            }
        }

        public void PlayHitImpact()
        {
            transform.DOPunchScale(new Vector3(1, 1, 1), 0.2f, 5, 1).SetEase(Ease.InElastic);
        }

        public void KnifeHitTarget()
        {
            transform.DOShakeScale(duration: 0.1f, strength: 0.1f, vibrato: 10, randomness: 1, fadeOut: true)
                .SetEase(Ease.InSine);
        }

        private void OnDestroy()
        {
            _sequence.Kill();
        }
    }
}