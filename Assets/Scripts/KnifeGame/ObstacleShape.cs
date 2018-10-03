using DG.Tweening;
using UnityEngine;

namespace KnifeGame
{
    public class ObstacleShape : MonoBehaviour
    {
        public Ease EaseType = Ease.Linear;
        private Vector3 _scale;
        [SerializeField] private float _time;

        private void Start()
        {
            _scale = transform.localScale * 0.5f;
            Scale();
        }

        private void Scale()
        {
            transform.DOScale(_scale, _time).SetLoops(-1, LoopType.Yoyo).SetEase(EaseType);
        }

        private void OnDestroy()
        {
            transform.DOKill(transform);
        }
    }
}