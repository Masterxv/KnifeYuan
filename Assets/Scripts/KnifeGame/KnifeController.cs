using System;
using System.Collections;
using UnityEngine;

namespace KnifeGame
{
    public class KnifeController : MonoBehaviourHelper
    {
        [Tooltip("Speed of throwing of knife")] [SerializeField]
        private float _speed;

        [SerializeField] private float _mass;
        [SerializeField] private float _torque;

        [Tooltip("Force to add to knife when it's bounding or flying apart")] [SerializeField]
        private float _forceMultiplier;

        public event Action<Transform> OnHit;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private float _rotationSpeed;

        private Transform _centerOfCircle;

//        private bool _becameChildAlready;
        private Vector3 _localForward;
        private Vector3 _axis = new Vector3(0, 0, 1);
        private float _zE;
        private bool _standBy = true;
        private KnifeDirection _knifeDir;
        private float _rotDirection;
        private Transform _rotatePoint;
        private bool _reverse = true;

        private float _angle;
//        private ContactPoint2D[] _contacts = new ContactPoint2D[2];

        private void Awake()
        {
            _centerOfCircle = GameObject.FindGameObjectWithTag(TagAndString.CENTER_OF_CIRCLE).transform;
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
//            if (other.gameObject.CompareTag(TagAndString.CIRCLE_TAG))
//            {
//                _rigidbody.GetContacts(_contacts);
//                var p = _contacts[0].point;
//                print(p);
//            }

//            if (!other.gameObject.CompareTag(TagAndString.APPLE_TAG))
//            {
//                _rigidbody.bodyType = RigidbodyType2D.Static;
//                _collider.isTrigger = true;
//                if (!_becameChildAlready)
//                {
//                    var localForward = transform.TransformDirection(Vector3.up);
//                    transform.position += localForward * 0.8f;
//                    _becameChildAlready = true;
//                }
//            }
//            if (other.gameObject.CompareTag(TagAndString.CIRCLE_TAG))
//            {
//                _rigidbody.bodyType = RigidbodyType2D.Static;
//                _collider.isTrigger = true;
//                var localForward = transform.TransformDirection(Vector3.up);
//                transform.position += localForward * 1f;
//                transform.rotation = Quaternion.identity;
//            }
//            if (other.gameObject.CompareTag(TagAndString.APPLE_TAG))
//                other.gameObject.SetActive(false);
//            OnHit?.Invoke(other.transform);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(TagAndString.KNIFE_TAG))
            {
//                print("hit knife");
            }

            if (other.gameObject.CompareTag(TagAndString.APPLE_TAG))
            {
                other.gameObject.SetActive(false);
//                print("hit apple");
            }

            if (other.gameObject.CompareTag(TagAndString.PLATFORM))
            {
                _angle = transform.position.x < 0 ? -45f : 45f;
                var rot = Quaternion.Euler(0, 0, transform.rotation.z + _angle);
                transform.rotation = rot;
                _localForward = transform.TransformDirection(Vector3.up);
                _rigidbody.velocity = _localForward * _speed * 0.8f;
            }

            if (other.gameObject.CompareTag(TagAndString.CIRCLE_TAG))
            {
                var point = other.contacts[0].point;
                print(point);

                transform.parent = other.transform;
                _rigidbody.bodyType = RigidbodyType2D.Static;
//                _collider.isTrigger = true;
//                print("center of circle: " + _centerOfCircle.position);
                var dirFromPointToCenter = _centerOfCircle.position - (Vector3) point;
                transform.up = dirFromPointToCenter.normalized;
//                _localForward = transform.TransformDirection(Vector3.up);
//                transform.position += _localForward * 1.2f;
                Time.timeScale = 0;
            }

            OnHit?.Invoke(other.transform);
        }

        public void KnifeBound()
        {
            _collider.enabled = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            var direction = transform.position - _centerOfCircle.position;
            _rigidbody.mass = _mass;
            _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
            _rigidbody.AddForce(direction.normalized * _forceMultiplier);
        }

        public void KnifeFlyAppart()
        {
            _collider.enabled = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;

            transform.SetParent(null);
            gameObject.SetActive(true);
            var direction = transform.position - _centerOfCircle.position;
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
            if (zE < 330f && _reverse)
                _rotDirection = -_rotationSpeed; // chieu kim dong ho

            if (zE < 230f)
            {
                _reverse = false;
                _rotDirection = _rotationSpeed;
            }

            if (zE > 330.1f)
            {
                _reverse = true;
                _rotDirection = -_rotationSpeed;
            }
        }

        private void KnifeRotateLeftSide(float zE)
        {
            if (zE > 30f && _reverse)
                _rotDirection = _rotationSpeed; // chieu nguoc kim dong ho
            if (zE > 115f)
            {
                _reverse = false;
                _rotDirection = -_rotationSpeed;
            }

            if (zE < 29.9f)
            {
                _reverse = true;
                _rotDirection = _rotationSpeed;
            }
        }

        private void OnBecameInvisible()
        {
//            _rigidbody.bodyType = RigidbodyType2D.Static;
//            gameObject.transform.position = new Vector3(1000, 1000, 0);
        }
    }
}