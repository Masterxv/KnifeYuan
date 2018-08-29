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
        HORIZONTAL
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

        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private MoveType _moveType = MoveType.NONE;
        [SerializeField] private float _moveXValue;
        [SerializeField] private float _moveYValue;
        public Ease HorizontalEase = Ease.Linear;
        public Ease VerticalEase = Ease.Linear;
        public Vector3 CenterOfCircle { get; private set; }

        #endregion

        #region PRIVATE

        private float _interval; // time to add between tween loop
        private Ease _shakeEaseType;
        private float _angle;
        private float _timeToStop; // this is MAIN parameter to rotate the target

        private Vector3 _rotateVector = new Vector3(0, 0, 1);
        private float _targetPos;
        private Vector3 _beginPosition;

        #endregion


        private void Awake()
        {
            _beginPosition = transform.position;
            if (_moveType == MoveType.NONE)
                CenterOfCircle = _collider2D.bounds.center;
            else
                MoveTheCircle();

            transform.localScale *= 0.5f;
            transform.DOScale(Vector3.one, 1f).SetDelay(0.5f).SetEase(_circleEaseType).OnComplete(LaunchTheRotation);
        }

        private void MoveTheCircle()
        {
            if (_moveType == MoveType.HORIZONTAL)
                MoveHorizontalStart();
            if (_moveType == MoveType.VERTICLE)
                MoveVertical();
        }

        private void MoveVertical()
        {
            transform.DOMoveY(transform.position.y - _moveYValue, 2).SetLoops(-1, LoopType.Yoyo).SetEase(VerticalEase);
        }

        private void MoveHorizontalStart()
        {
            _targetPos = _beginPosition.x - _moveXValue;
            transform.DOMoveX(_targetPos, 2).SetEase(HorizontalEase).OnComplete(MoveHorizontalEnd);
        }

        private void MoveHorizontalEnd()
        {
            _targetPos = _beginPosition.x + _moveXValue;
            transform.DOMoveX(_targetPos, 4).SetEase(HorizontalEase).OnComplete(MoveHorizontalStart);
        }

        private void LaunchTheRotation()
        {
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
            }
        }

        private void RotateLRShake()
        {
            ConfigureRotate();
            _rotateVector *= -1;

            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_easeType).SetDelay(_interval).OnComplete(ShakeLR);
        }

        private void ShakeLR()
        {
            ConfigureShake();
            _rotateVector *= -1;
            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_shakeEaseType).SetDelay(_interval).OnComplete(RotateLRShake);
        }

        private void RotateRightAndShake()
        {
            ConfigureRotate();
            transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_easeType).SetDelay(_interval).OnComplete(ShakerRight);
        }

        private void ShakerRight()
        {
            ConfigureShake();
            transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_shakeEaseType).SetDelay(_interval).OnComplete(RotateRightAndShake);
        }

        private void RotateLeftAndShake()
        {
            ConfigureRotate();
            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_easeType).SetDelay(_interval).OnComplete(ShakerLeft);
        }

        private void ShakerLeft()
        {
            ConfigureShake();
            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_shakeEaseType).SetDelay(_interval).OnComplete(RotateLeftAndShake);
        }

        private void ConfigureShake()
        {
            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
                return;
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

            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop,
                RotateMode.FastBeyond360).SetEase(_easeType).SetDelay(_interval).OnComplete(RotateLeftRight);
        }

        private void RotateToRight()
        {
            ConfigureRotate();
            transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop,
                RotateMode.FastBeyond360).SetEase(_easeType).SetDelay(_interval).OnComplete(RotateToRight);
        }

        private void RotateToLeft()
        {
            ConfigureRotate();
            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360).SetEase(_easeType)
                .SetDelay(_interval).SetLoops(1, _loopType).OnComplete(RotateToLeft);
        }

        private void ConfigureRotate()
        {
            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
                return;
            _timeToStop = Random.Range(_stopTimeMin, _stopTimeMax);
            _angle = 360f * _velocity * _timeToStop;
            _angle += Random.Range(0f, 30f);
            _interval = Random.Range(0, _delayTime);
        }

        public void PlayHitImpact()
        {
            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
                return;
            transform.DOPunchScale(new Vector3(1, 1, 1), 0.2f, 5).SetEase(Ease.InElastic);
        }

        public void PlayKnifeHitTarget()
        {
            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
                return;
            transform.DOShakeScale(0.1f, 0.1f, 10, 1, true).SetEase(Ease.InSine);
        }

        private void Update()
        {
            if (_moveType != MoveType.NONE)
                CenterOfCircle = _collider2D.bounds.center;
        }

        private void OnDestroy()
        {
            DOTween.Kill(transform);
        }
    }
}