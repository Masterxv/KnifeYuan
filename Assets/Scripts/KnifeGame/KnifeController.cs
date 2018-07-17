using UnityEngine;

namespace KnifeGame
{
    public class KnifeController : MonoBehaviour
    {
        [SerializeField] private float _speed;
        private Rigidbody2D _rigidbody2D;


        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void Throw()
        {
            _rigidbody2D.velocity = Vector2.up * _speed;
        }
    }
}