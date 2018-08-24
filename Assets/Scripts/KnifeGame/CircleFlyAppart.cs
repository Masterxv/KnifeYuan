using UnityEngine;

namespace KnifeGame
{
    public class CircleFlyAppart : MonoBehaviour
    {
        [SerializeField] private Transform _centerOfCircle;
        [SerializeField] private float _mass;
        [SerializeField] private float _torque;
        [SerializeField] private float _forceMultiplier;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            if (_centerOfCircle == null)
                _centerOfCircle = GameObject.FindGameObjectWithTag(TagAndString.CENTER_OF_CIRCLE).transform;
        }

        private void Start()
        {
            FlyAppart();
        }


        private void FlyAppart()
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            transform.SetParent(null);
            var direction = transform.position - _centerOfCircle.position;
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