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
        private Transform _centerOfTarget;
        private bool _gotChild = true;
        private void Awake()
        {
            _centerOfTarget = GameObject.FindGameObjectWithTag(TagAndString.CENTER_OF_CIRCLE).transform;
        }

        public void Throw()
        {
            _rigidbody.velocity = transform.up * _speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag(TagAndString.APPLE_TAG))
            {
                if (other.gameObject.CompareTag(TagAndString.CIRCLE_TAG))
                    transform.parent = other.transform;

                _rigidbody.bodyType = RigidbodyType2D.Static;
                _collider.isTrigger = true;
                if (_gotChild)
                {
                    var localForward = transform.TransformDirection(Vector3.up);
                    transform.position += localForward * 0.8f;
                    _gotChild = false;
                }
            }

            if (other.gameObject.CompareTag(TagAndString.APPLE_TAG))
                other.gameObject.SetActive(false);
            OnHit?.Invoke(other.transform);
        }

        public void KnifeBound()
        {
            _collider.enabled = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            var direction = transform.position - _centerOfTarget.position;
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
            var direction = transform.position - _centerOfTarget.position;
            _rigidbody.mass = _mass;
            _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
            _rigidbody.AddForce(direction.normalized * _forceMultiplier);
        }

        private void OnBecameInvisible()
        {
            _rigidbody.bodyType = RigidbodyType2D.Static;
            gameObject.transform.position = new Vector3(1000, 1000, 0);
        }
    }
}