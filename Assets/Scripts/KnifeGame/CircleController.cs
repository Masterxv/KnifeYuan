using System.Runtime.Serialization.Formatters;
using DG.Tweening;
using TMPro;
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

    public enum MoveType
    {
        NONE,
        VERTICLE,
        HORIZONTAL,
        BOTH
    }

    public class CircleController : MonoBehaviourHelper
    {
        #region PUBLIC

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

        [SerializeField] private MoveType _moveType = MoveType.NONE;
        [SerializeField] private float _moveXValue;
        [HideInInspector] public Vector3 CenterOfCircle;

        #endregion

        #region PRIVATE

        private Collider2D _collider2D;
        private float _interval; // time to add between tween loop
        private Ease _shakeEaseType;
        private Sequence _sequence;
        private Sequence _sequenceImpact;
        private Sequence _sequenceMoving;
        private float _angle;
        private float _timeToStop; // this is MAIN parameter to rotate the target
        private Vector3 _rotateVector = new Vector3(0, 0, 1);
        private float _xMoving;

        #endregion


        private void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
            transform.localScale *= 0.5f;
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _sequence.Append(transform.DOScale(Vector3.one, 1f).SetDelay(0.5f).SetEase(_circleEaseType));
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(LaunchTheRotation);
            _sequence.Play();

            if (_moveType != MoveType.NONE)
            {
                MoveTheCircle();
            }
        }

        private void MoveTheCircle()
        {
            if (_moveType == MoveType.HORIZONTAL)
                MoveHorizontalStart();
            if (_moveType == MoveType.VERTICLE)
                MoveVerticalStart();
        }

        private void MoveVerticalStart()
        {
            throw new System.NotImplementedException();
        }

        private void MoveHorizontalStart()
        {
            _xMoving = -_moveXValue;
            _sequenceMoving?.Kill();
            _sequenceMoving = DOTween.Sequence();
            if (!gameManager.WaitToMove) _sequenceMoving.SetDelay(3f);
            _sequenceMoving.Append(transform.DOMoveX(_xMoving, 2)).SetLoops(1, LoopType.Incremental)
                .OnStepComplete(MoveHorizontalEnd);
        }

        private void MoveHorizontalEnd()
        {
            gameManager.WaitToMove = true;
            _xMoving = _moveXValue;
            _sequenceMoving?.Kill();
            _sequenceMoving = DOTween.Sequence();
            _sequenceMoving.SetDelay(0.1f);
            _sequenceMoving.Append(transform.DOMoveX(_xMoving, 4)).SetLoops(1, LoopType.Incremental)
                .OnStepComplete(MoveHorizontalStart);
        }

        private void LaunchTheRotation()
        {
            DOTween.Kill(transform); // kill tween from doScale
            canvasManager.ShowReadyText();
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
                default: break;
            }
        }

        private void RotateLRShake()
        {
            ConfigureRotate();
            _rotateVector *= -1;

            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_easeType));

            _sequence.SetDelay(_interval);
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

            _sequence.SetDelay(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateLRShake);
            _sequence.Play();
        }

        private void RotateRightAndShake()
        {
            ConfigureRotate();

            _sequence.Append(transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_easeType));

            _sequence.SetDelay(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(ShakerRight);
            _sequence.Play();
        }

        private void ShakerRight()
        {
            ConfigureShake();
            _sequence.Append(transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_shakeEaseType));

            _sequence.SetDelay(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateRightAndShake);
            _sequence.Play();
        }

        private void RotateLeftAndShake()
        {
            ConfigureRotate();

            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_easeType));

            _sequence.SetDelay(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(ShakerLeft);
            _sequence.Play();
        }

        private void ShakerLeft()
        {
            ConfigureShake();

            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_shakeEaseType));

            _sequence.SetDelay(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateLeftAndShake);
            _sequence.Play();
        }

        private void ConfigureShake()
        {
            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
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

            _sequence.SetDelay(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateLeftRight);
            _sequence.Play();
        }

        private void RotateToRight()
        {
            ConfigureRotate();
            _sequence.Append(transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop,
                RotateMode.FastBeyond360).SetEase(_easeType));

            _sequence.SetDelay(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateToRight);
            _sequence.Play();
        }

        private void RotateToLeft()
        {
            ConfigureRotate();
            _sequence.Append(transform.DOLocalRotate(_rotateVector * _angle, _timeToStop,
                RotateMode.FastBeyond360).SetEase(_easeType));

            _sequence.SetDelay(_interval);
            _sequence.SetLoops(1, _loopType);
            _sequence.OnStepComplete(RotateToLeft);
            _sequence.Play();
        }

        private void ConfigureRotate()
        {
            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
                return;
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            _timeToStop = Random.Range(_stopTimeMin, _stopTimeMax);
            _angle = 360f * _velocity * _timeToStop;
            _angle += Random.Range(0f, 30f);
            _interval = Random.Range(0, _delayTime);
        }

        public void PlayHitImpact()
        {
            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
                return;
            _sequenceImpact?.Kill();
            _sequenceImpact = DOTween.Sequence();
            _sequenceImpact.Append(transform.DOPunchScale(new Vector3(1, 1, 1), 0.2f, 5, 1).SetEase(Ease.InElastic));
        }

        public void PlayKnifeHitTarget()
        {
            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
                return;
            _sequenceImpact?.Kill();
            _sequenceImpact = DOTween.Sequence();
            _sequenceImpact.Append(transform
                .DOShakeScale(duration: 0.1f, strength: 0.1f, vibrato: 10, randomness: 1, fadeOut: true)
                .SetEase(Ease.InSine));
        }

        private void Update()
        {
            CenterOfCircle = _collider2D.bounds.center;
        }

        private void OnDestroy()
        {
            _sequence.Kill();
            _sequenceImpact.Kill();
            _sequenceMoving.Kill();
        }
    }
}