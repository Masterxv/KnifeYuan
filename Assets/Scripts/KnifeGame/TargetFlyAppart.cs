using UnityEngine;

namespace KnifeGame
{
    public class TargetFlyAppart : MonoBehaviour
    {
        [SerializeField] private Transform _centerOfTarget;
        [SerializeField] private float _mass;
        [SerializeField] private float _torque;
        [SerializeField] private float _forceMultiplier;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            FlyAppart();
        }


        private void FlyAppart()
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            transform.SetParent(null);
            var direction = transform.position - _centerOfTarget.position;
            _rigidbody.mass = _mass;
            _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
            _rigidbody.AddForce(direction.normalized * _forceMultiplier);
        }
    }
}