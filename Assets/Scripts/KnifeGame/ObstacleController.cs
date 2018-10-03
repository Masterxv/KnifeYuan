using UnityEngine;

namespace KnifeGame
{
    public class ObstacleController : MonoBehaviour
    {
        private bool _left;
        [SerializeField] private SpriteRenderer _ren;
        [SerializeField] private float _speed = 10f;
        [SerializeField] private Animator _anim;
        [SerializeField] private bool _isPlane;
        private readonly int _dieTrigger = Animator.StringToHash("Die");
        private bool _alive = true;

        private void Start()
        {
            _ren.sortingOrder = 3;
            _left = transform.position.x < 0;
            if (_left)
            {
                _ren.flipX = true;
                if (_isPlane)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            else if (_isPlane)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(false);
            }
        }


        private void Update()
        {
            if (!_alive) return;
            if (_left)
                transform.position += Vector3.right * _speed * Time.deltaTime;
            else
                transform.position -= Vector3.right * _speed * Time.deltaTime;
        }

        public void ObstacleHitKnife()
        {
            _alive = false;
            gameObject.AddComponent<Rigidbody2D>();
            GetComponent<Collider2D>().enabled = false;
            if (transform.childCount > 0)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
            }

            if (_anim != null)
                _anim.SetTrigger(_dieTrigger);
        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}