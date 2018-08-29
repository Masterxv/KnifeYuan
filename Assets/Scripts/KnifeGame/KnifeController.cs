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
        private bool _hit;

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
                    if (!_hit)
                        TriggerHitCircle(other);
                    break;
                case TagAndString.KNIFE_TAG:
                    if (!_hit)
                        TriggerHitKnife();
                    break;
                case TagAndString.BORDER_TAG:
                    gameManager.KnifeHitBorder();
                    break;
                case TagAndString.APPLE_TAG:
                    other.gameObject.SetActive(false);
                    gameManager.KnifeHitApple(other.transform);
                    break;
                case TagAndString.BLACK_APPLE_TAG:
                    if (!_hit)
                    {
                        other.gameObject.SetActive(false);
                        KnifeBound();
                        gameManager.KnifeHitBlackApple(other.transform);
                        _hit = true;
                    }

                    break;
            }
        }

        private void TriggerHitKnife()
        {
            KnifeBound();
            gameManager.KnifeHitOtherKnife();
            _hit = true;
        }

        private void TriggerHitCircle(Component other)
        {
            transform.parent = other.transform;
            _rigidbody.bodyType = RigidbodyType2D.Static;
            _collider.isTrigger = true;

            var dirFromPointToCenter = _circleController.CenterOfCircle - transform.position;
            transform.up = dirFromPointToCenter.normalized;

            transform.position =
                (transform.position - _circleController.CenterOfCircle).normalized * _distance +
                _circleController.CenterOfCircle;
            HitPosition = transform.position;

            SetColliderForKnife();
            gameManager.KnifeHitCircle();
            _hit = true;
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
            _collider.offset = new Vector2(0, -0.6f);
            _collider.size = new Vector2(0.4f, 1.3f);
        }

        private void KnifeBound()
        {
            _collider.enabled = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.velocity = Vector2.zero;
            var direction = transform.position - _circleController.CenterOfCircle;
            _rigidbody.mass = _mass;
            _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
            _rigidbody.AddForce(direction.normalized * _forceMultiplier);
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