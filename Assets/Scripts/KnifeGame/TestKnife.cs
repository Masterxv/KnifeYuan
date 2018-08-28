using System.Collections.Generic;
using UnityEngine;

namespace KnifeGame
{
    public class TestKnife : MonoBehaviour
    {
        public Transform Point;
        public bool SetToLeft;
        [SerializeField] private float _rotateSpeed;
        
//        private Transform _rightSideConfigure;
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

            if (SetToLeft)
            {
//                _rightSideConfigure.position = new Vector3(-0.8406528f, -4.042425f, 0f);
//                _rightSideConfigure.rotation = Quaternion.Euler(0, 0, 31.472f);
//                transform.position = _rightSideConfigure.position;
//                transform.rotation = _rightSideConfigure.rotation;
            }
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

            if (SetToLeft)
                KnifeRotateLeftSide(zE);
            else
                KnifeRotateRightSide(zE);

            transform.RotateAround(Point.position, axis, Time.deltaTime * _rotDirection);
        }

        private void KnifeRotateLeftSide(float zE)
        {
            if (zE > 30f)
            {
                if (_reverse)
                    _rotDirection = _rotateSpeed; // chieu nguoc kim dong ho
            }

            if (zE > 130f)
            {
                _reverse = false;
                _rotDirection = -_rotateSpeed;
            }

            if (zE < 29.9f)
            {
                _reverse = true;
                _rotDirection = _rotateSpeed;
            }
        }

        private void KnifeRotateRightSide(float zE)
        {
            if (zE < 340f)
                if (_reverse)
                    _rotDirection = -_rotateSpeed; // chieu kim dong ho

            if (zE < 230f)
            {
                _reverse = false;
                _rotDirection = _rotateSpeed;
            }

            if (zE > 340.1f)
            {
                _reverse = true;
                _rotDirection = -_rotateSpeed;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(TagAndString.PLATFORM))
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