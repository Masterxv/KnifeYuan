using System;
using System.Collections;
using UnityEngine;

namespace KnifeGame
{
    public class KnifeController : MonoBehaviourHelper
    {
        #region PUBLIC

        [Tooltip("Speed of throwing of knife")] [SerializeField]
        private float _speed;

        [SerializeField] private float _mass;
        [SerializeField] private float _torque;

        [Tooltip("Force to add to knife when it's bounding or flying apart")] [SerializeField]
        private float _forceMultiplier;

//        public event Action<Transform> OnHit;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] float _distance = 2.2f; // make the distance between knife and centerOfCircle is fixed
        [HideInInspector] public Vector3 HitPosition;

        #endregion

        #region PRIVATE

        private CircleController _circleController;
        private Vector3 _localForward;
        private readonly Vector3 _axis = new Vector3(0, 0, 1);
        private float _zE;
        private bool _standBy = true;
        private KnifeDirection _knifeDir;
        private float _rotDirection;
        private Transform _rotatePoint;
        private bool _reverse = true;

        private float _angle;
        private bool _isInsideCircle;

        #endregion


        private void Awake()
        {
            _circleController = GameObject.FindGameObjectWithTag(TagAndString.CIRCLE_TAG)
                .GetComponent<CircleController>();

            _knifeDir = gameManager.LevelManager[gameManager.LevelIndex].KnifeDirection;
            _rotatePoint = GameObject.FindGameObjectWithTag(TagAndString.ROTATE_POINT).transform;
        }

        public void Throw()
        {
            _standBy = false;
            _localForward = transform.TransformDirection(Vector3.up);
            _rigidbody.velocity = _localForward * _speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var otherTag = other.gameObject.tag;
            switch (otherTag)
            {
                case TagAndString.PLATFORM:
                    TriggerHitPlatform();
                    break;
                case TagAndString.CIRCLE_TAG:
                    TriggerHitCircle(other);
                    audioManager.PlayHitCircleSound();
                    break;
                case TagAndString.KNIFE_TAG:
                    TriggerHitKnife();
                    audioManager.PlayHitKnifeSound();
                    break;
                case TagAndString.BORDER_TAG:
                    gameManager.KnifeHitBorder();
                    break;
                case TagAndString.FRUIT_TAG:
                    other.gameObject.SetActive(false);
                    audioManager.PlayHitFruitSound();
                    gameManager.KnifeHitFruit(other.transform);
                    break;
                case TagAndString.DEATH_FRUIT_TAG:
                    other.gameObject.SetActive(false);
                    KnifeBound();
                    gameManager.KnifeHitDeathFruit(other.transform);

                    audioManager.PlayHitDeathFruitSound();
                    break;
                case TagAndString.OBSTACLE_TAG:
                    other.gameObject.GetComponent<ObstacleController>().ObstacleHitKnife();
                    HitObstacle(other.transform.position);
                    audioManager.PlayObstacleSound();
                    break;
                case TagAndString.OBSTACLE_SHAPE_TAG:
                    other.gameObject.SetActive(false);
                    HitObstacle(other.transform.position);
                    audioManager.PlayObstacleShapeSound();
                    break;
                case TagAndString.BIG_OBSTACLE_SHAPE_TAG:
                    other.gameObject.SetActive(false);
                    other.transform.parent.gameObject.SetActive(false);
                    HitObstacle(transform.position);
                    audioManager.PlayObstacleShapeSound();
                    break;
                default: break;
            }
        }

        private void HitObstacle(Vector3 hitPoint)
        {
            KnifeBound();
            gameManager.KnifeHitObstacle(hitPoint);
        }

        private void TriggerHitKnife()
        {
            KnifeBound();
            gameManager.KnifeHitOtherKnife();
        }

        private void TriggerHitCircle(Component other)
        {
            _isInsideCircle = true;
            transform.parent = other.transform;
            _rigidbody.bodyType = RigidbodyType2D.Static;

            var dirFromPointToCenter = _circleController.CenterOfCircle - transform.position;
            transform.up = dirFromPointToCenter.normalized;
            var dir = transform.position - _circleController.CenterOfCircle;
            dir = dir.normalized;
            transform.position = dir * _distance + _circleController.CenterOfCircle;
            HitPosition = transform.position;
            transform.localScale = Vector3.one;
            SetColliderForKnife();
            gameManager.KnifeHitCircle();
        }

        private void TriggerHitPlatform()
        {
            _angle = transform.position.x < 0 ? -45f : 45f;
            var rot = Quaternion.Euler(0, 0, transform.rotation.z + _angle);
            transform.rotation = rot;
            _localForward = transform.TransformDirection(Vector3.up);
            _rigidbody.velocity = _localForward * _speed * 0.8f;
        }

        private void SetColliderForKnife()
        {
            _collider.offset = new Vector2(0, -0.7f);
            _collider.size = new Vector2(0.4f, 1.1f);
        }

        private void KnifeBound()
        {
            if(_isInsideCircle) return;
            _collider.enabled = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.velocity = Vector2.zero;
            var direction = transform.position - _circleController.CenterOfCircle;
            _rigidbody.mass = _mass;
            _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
            _rigidbody.AddForce(direction.normalized * _forceMultiplier);
            transform.parent = null;
        }

        public void KnifeFlyAppart()
        {
            _collider.isTrigger = true;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;

            transform.SetParent(null);
            gameObject.SetActive(true);
            var direction = transform.position - _circleController.CenterOfCircle;
            _rigidbody.mass = _mass;
            _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
            _rigidbody.AddForce(direction.normalized * _forceMultiplier);
        }


        private void Update()
        {
            _zE = transform.eulerAngles.z;
            if (_knifeDir == KnifeDirection.Left)
                KnifeRotateLeftSide(_zE);

            if (_knifeDir == KnifeDirection.Right)
                KnifeRotateRightSide(_zE);
            if (_standBy)
                transform.RotateAround(_rotatePoint.position, _axis, Time.deltaTime * _rotDirection);
        }

        private void KnifeRotateRightSide(float zE)
        {
            if (zE < 340f && _reverse)
                _rotDirection = -_rotationSpeed; // chieu kim dong ho

            if (zE < 240f)
            {
                _reverse = false;
                _rotDirection = _rotationSpeed;
            }

            if (zE > 340.1f)
            {
                _reverse = true;
                _rotDirection = -_rotationSpeed;
            }
        }

        private void KnifeRotateLeftSide(float zE)
        {
            if (zE > 20f && _reverse)
                _rotDirection = _rotationSpeed; // chieu nguoc kim dong ho
            if (zE > 115f)
            {
                _reverse = false;
                _rotDirection = -_rotationSpeed;
            }

            if (zE < 19.9f)
            {
                _reverse = true;
                _rotDirection = _rotationSpeed;
            }
        }

        private void OnBecameInvisible()
        {
            _rigidbody.bodyType = RigidbodyType2D.Static;
            gameObject.transform.position = new Vector3(1000, 1000, 0);
        }
    }
}