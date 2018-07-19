using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace KnifeGame
{
    public class KnifeController : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _mass;
        [SerializeField] private float _torque;
        [SerializeField] private float _forceMultiplier;

        public event Action<Transform> OnHit;
        private Rigidbody2D _rigidbody;
        private BoxCollider2D _collider;
        private Transform centerOfTarget;
        private readonly string APPLE_TAG = "Apple";

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            centerOfTarget = GameObject.FindGameObjectWithTag("CenterOfTarget").transform;
        }

        public void Throw()
        {
            _rigidbody.velocity = Vector2.up * _speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!APPLE_TAG.Equals(other.gameObject.tag, StringComparison.Ordinal))
            {
                _rigidbody.velocity = Vector2.zero;
                _collider.isTrigger = true;
                _collider.size = new Vector2(_collider.size.x, 1.06f);
                _collider.offset = new Vector2(0, -0.7f);
            }

            OnHit?.Invoke(other.transform);
        }

        public void KnifeBound()
        {
            _collider.enabled = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            var direction = transform.position - centerOfTarget.position;
            _rigidbody.mass = _mass;
            _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
            _rigidbody.AddForce(direction.normalized * _forceMultiplier);
        }

        public void KnifeFlyAppart()
        {
            _collider.enabled = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;

            transform.SetParent(null);
            var direction = transform.position - centerOfTarget.position;
            _rigidbody.mass = _mass;
            _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
            _rigidbody.AddForce(direction.normalized * _forceMultiplier);
        }
    }
}