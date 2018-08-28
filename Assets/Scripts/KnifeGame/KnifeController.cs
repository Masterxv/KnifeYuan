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
        [SerializeField] float _distance = 2.2f; // make the distance between knife and centerOfCircle is fixed
        [HideInInspector] public Vector3 HitPosition; 
            

        private Transform _centerOfCircle;
        private Vector3 _localForward;
        private readonly Vector3 _axis = new Vector3(0, 0, 1);
        private float _zE;
        private bool _standBy = true;
        private KnifeDirection _knifeDir;
        private float _rotDirection;
        private Transform _rotatePoint;
        private bool _reverse = true;
        private float _angle;

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
            if (other.gameObject.CompareTag(TagAndString.CIRCLE_TAG))
            {
                transform.parent = other.transform;
                _rigidbody.bodyType = RigidbodyType2D.Static;
                _collider.isTrigger = true;
                var dirFromPointToCenter = _centerOfCircle.position - transform.position;
                transform.up = dirFromPointToCenter.normalized;

                transform.position = (transform.position - _centerOfCircle.position).normalized * _distance + _centerOfCircle.position;
                HitPosition = transform.position;
            }

            if (other.gameObject.CompareTag(TagAndString.PLATFORM))
            {
                _angle = transform.position.x < 0 ? -45f : 45f;
                var rot = Quaternion.Euler(0, 0, transform.rotation.z + _angle);
                transform.rotation = rot;
                _localForward = transform.TransformDirection(Vector3.up);
                _rigidbody.velocity = _localForward * _speed * 0.8f;
            }

            if (other.gameObject.CompareTag(TagAndString.APPLE_TAG))
                other.gameObject.SetActive(false);
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