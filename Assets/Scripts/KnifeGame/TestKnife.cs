using UnityEngine;

namespace KnifeGame
{
    public class TestKnife : MonoBehaviour
    {
        public Transform Point;
        private Rigidbody2D _rigid;
        private Vector3 _localForward;
        private float _angle;

        private float _rotDirection;
        bool _reverse = true;

        private void Start()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _rigid.bodyType = RigidbodyType2D.Static;
            _rigid.freezeRotation = true;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _rigid.bodyType = RigidbodyType2D.Dynamic;
                _localForward = transform.TransformDirection(Vector3.up);
                _rigid.velocity = _localForward * 20;
            }

            var axis = new Vector3(0, 0, 1);

            var zE = transform.eulerAngles.z;

            #region Right side

//            if (zE < 330f)
//                if (_reverse)
//                    _rotDirection = -100; // chieu kim dong ho
//
//            if (zE < 230f)
//            {
//                _reverse = false;
//                _rotDirection = 100;
//            }
//
//            if (zE > 330.1f)
//            {
//                _reverse = true;
//                _rotDirection = -100;
//            }

            #endregion

            #region Left Side

            if (zE > 30f)
            {
                if (_reverse)
                    _rotDirection = 100; // chieu nguoc kim dong ho
            }

            if (zE > -245f)
            {
                _reverse = false;
                _rotDirection = -100;
            }

//            if (zE < 29f)
//            {
//                _reverse = true;
//                _rotDirection = 100;
//            }

            #endregion


            transform.RotateAround(Point.position, axis, Time.deltaTime * _rotDirection);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Platform"))
            {
                _angle = transform.position.x < 0 ? -45f : 45f;
                var rot = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + _angle);
                transform.rotation = rot;
                _localForward = transform.TransformDirection(Vector3.up);
                _rigid.velocity = _localForward * 20;
            }
        }
    }
}