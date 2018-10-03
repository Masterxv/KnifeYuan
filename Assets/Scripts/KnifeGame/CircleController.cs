using System.IO;
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
        CUSTOM_BOTH,
        LEFT_SHAKE,
        RIGHT_SHAKE,
        LEFT_RIGHT_SHAKE,
        CUSTOM,
        DOLL
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
        [SerializeField] private Ease _rotateEaseType = Ease.Linear;
        private Ease _shakeEaseType; // only 2 types: InBounce and OutBounce


        [Tooltip("The circle appears with this type")] [SerializeField]
        private Ease _circleEaseType = Ease.OutBounce;

        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private MoveType _moveType = MoveType.NONE;
        [SerializeField] private float _moveXValue;
        [SerializeField] private float _moveYValue;
        [SerializeField] private float _moveVel;
        public Ease HorizontalEase = Ease.Linear;
        public Ease VerticalEase = Ease.Linear;
        [SerializeField] private int _dollLoopRate = 2; // how many time to rotate the doll style
        [SerializeField] private float _dollAngle = 60f;
        [SerializeField] private float _dollRotateTime = 0.5f; // time for 1 rotation with  dollAngle

        public Vector3 CenterOfCircle { get; private set; }

        #endregion

        #region PRIVATE

        private float _interval; // time to add between tween loop
        private float _angle;
        private float _timeToStop; // this is MAIN parameter to rotate the target

        private Vector3 _rotateVector = new Vector3(0, 0, 1);
        private float _targetPos;
        private Vector3 _beginPosition;
        private LoopType _loopType = LoopType.Incremental;
        private Sequence _animCircleSeq;

        #endregion


        private void Awake()
        {
            _beginPosition = transform.position;
            transform.localScale *= 0.5f;
            transform.DOScale(Vector3.one, 1.5f).SetEase(_circleEaseType).OnComplete(LaunchTheRotation);

            if (_moveType == MoveType.NONE)
                CenterOfCircle = _collider2D.bounds.center;
            else
            {
                Invoke(nameof(MoveTheCircle), 3f);
            }
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
            transform.DOMoveY(transform.position.y - _moveYValue, _moveYValue * _moveVel).SetLoops(-1, LoopType.Yoyo)
                .SetEase(VerticalEase);
        }

        private void MoveHorizontalStart()
        {
            _targetPos = _beginPosition.x - _moveXValue;
            transform.DOMoveX(_targetPos, _moveXValue * _moveVel).SetEase(HorizontalEase).OnComplete(MoveHorizontalEnd);
        }

        private void MoveHorizontalEnd()
        {
            _targetPos = _beginPosition.x + _moveXValue;
            transform.DOMoveX(_targetPos, _moveXValue * _moveVel * 2f).SetEase(HorizontalEase)
                .OnComplete(MoveHorizontalStart);
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
                case RotateType.CUSTOM:
                    RotateCustom();
                    break;
                case RotateType.CUSTOM_BOTH:
                    CustomBoth(); // random to left or right
                    break;
                case RotateType.DOLL:
                    DollRotateBegin();
                    break;
                default:
                    break;
            }
        }

        private void DollRotateBegin()
        {
            var startAngle = transform.localEulerAngles;
            var ran = Random.value;
            if (ran > 0.5f)
                startAngle.z -= _dollAngle;
            else
                startAngle.z += _dollAngle;
            var nextAngle = startAngle;

            transform.DOLocalRotate(nextAngle, _dollRotateTime, RotateMode.FastBeyond360)
                .SetLoops(_dollLoopRate, LoopType.Yoyo)
                .SetEase(_rotateEaseType).OnComplete(DollRotateEnd);
        }

        private void DollRotateEnd()
        {
            var startAngle = transform.localEulerAngles;
            var ran = Random.value;
            if (ran > 0.5f)
                startAngle.z += 180f;
            else
                startAngle.z -= 180f;

            startAngle.z += Random.Range(-12f, 12f);
            var nextAngle = startAngle;

            transform.DOLocalRotate(nextAngle, 0.6f, RotateMode.FastBeyond360).SetEase(_rotateEaseType)
                .OnComplete(DollRotateBegin);
        }

        private void CustomBoth()
        {
            var ran = Random.value;
            if (ran > 0.5f)
            {
                // left
                ConfigureRotate();
                transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                    .SetEase(_rotateEaseType).SetDelay(_interval).OnComplete(CustomBoth);
            }
            else
            {
                // right
                ConfigureRotate();
                transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop,
                    RotateMode.FastBeyond360).SetEase(_rotateEaseType).SetDelay(_interval).OnComplete(CustomBoth);
            }
        }

        private void RotateCustom()
        {
            if (Random.value > 0.5f)
            {
                ShakerLeft();
            }
            else
            {
                ShakerRight();
            }
        }

        private void RotateLRShake()
        {
            ConfigureRotate();
            _rotateVector *= -1;

            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_rotateEaseType).SetDelay(_interval).OnComplete(ShakeLR);
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
                .SetEase(_rotateEaseType).SetDelay(_interval).OnComplete(ShakerRight);
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
                .SetEase(_rotateEaseType).SetDelay(_interval).OnComplete(ShakerLeft);
        }

        private void ShakerLeft()
        {
            ConfigureShake();
            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_shakeEaseType).SetDelay(_interval).OnComplete(RotateLeftAndShake);
        }

        private void ConfigureShake()
        {
//            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
//                return;
            _timeToStop = Random.Range(_stopTimeMin, _stopTimeMax * 0.6f);
            _angle = 360f * _velocity * _timeToStop;
            _angle += Random.Range(0f, 7f);

            _interval = Random.Range(0f, _delayTime);
            var ran = Random.value;
            _shakeEaseType = ran > 0.5f ? Ease.InBounce : Ease.OutBounce;
        }

        private void RotateLeftRight()
        {
            ConfigureRotate();
            _rotateVector *= -1;

            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop,
                RotateMode.FastBeyond360).SetEase(_rotateEaseType).SetDelay(_interval).OnComplete(RotateLeftRight);
        }

        private void RotateToRight()
        {
            ConfigureRotate();
            transform.DOLocalRotate(-_rotateVector * _angle, _timeToStop,
                RotateMode.FastBeyond360).SetEase(_rotateEaseType).SetDelay(_interval).OnComplete(RotateToRight);
        }

        private void RotateToLeft()
        {
            ConfigureRotate();
            transform.DOLocalRotate(_rotateVector * _angle, _timeToStop, RotateMode.FastBeyond360)
                .SetEase(_rotateEaseType)
                .SetDelay(_interval).SetLoops(1, _loopType).OnComplete(RotateToLeft);
        }

        private void ConfigureRotate()
        {
//            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
//                return;
            _timeToStop = Random.Range(_stopTimeMin, _stopTimeMax);
            _angle = 360f * _velocity * _timeToStop;
            _angle += Random.Range(0f, 30f);
            _interval = Random.Range(0, _delayTime);
        }

        public void PlayHitImpact()
        {
//            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
//                return;
            _animCircleSeq = DOTween.Sequence();
            _animCircleSeq.Append(transform.DOPunchScale(new Vector3(1, 1, 1), 0.2f, 5).SetEase(Ease.InElastic))
                .OnComplete((() => _animCircleSeq.Kill()));
        }

        public void PlayKnifeHitCircle()
        {
//            if (GameState.GetGameState() == State.Paused || GameState.GetGameState() == State.GameOver)
//                return;

            _animCircleSeq = DOTween.Sequence();
            _animCircleSeq.Append(transform.DOShakeScale(0.1f, 0.1f, 5, 1).SetEase(Ease.InSine))
                .OnComplete((() => _animCircleSeq.Kill()));
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