using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        void Start()
        {
            _centerOfTarget = GameObject.FindGameObjectWithTag(TagAndString.CENTER_OF_TARGET).transform;
        }

        public void Throw()
        {
            _rigidbody.velocity = transform.up * _speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag(TagAndString.APPLE_TAG)) // hit the target
            {
                _rigidbody.velocity = Vector2.zero;
                _collider.isTrigger = true;
                _collider.size = new Vector2(_collider.size.x, 1.3f);
                _collider.offset = new Vector2(0, -0.7f);
            }

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