using DG.Tweening;
using UnityEngine;

namespace KnifeGame
{
    public class BigObstacleShape : MonoBehaviour
    {
        public Ease EaseType = Ease.Linear;
        private Vector3 _smallScale;
        private Vector3 _bigScale;
        [SerializeField] private float _bigDuration = 1f;
        [SerializeField] private float _delayToSmall = 0.5f;
        [SerializeField] private float _delayToBig = 2f;

        private void Start()
        {
            _bigScale = transform.localScale;
            _smallScale = transform.localScale * 0.5f;
            ScaleToSmall();
        }

        private void ScaleToSmall()
        {
            _delayToSmall += Random.Range(-0.5f, 1f);
            transform.DOScale(_smallScale, 0.5f).SetDelay(_delayToSmall).SetEase(Ease.InExpo).OnComplete(ScaleToBig);
        }

        private void ScaleToBig()
        {
            _delayToBig += Random.Range(-0.5f, 1f);
            transform.DOScale(_bigScale, _bigDuration).SetDelay(_delayToBig).SetEase(EaseType).OnComplete(ScaleToSmall);
        }

        private void OnDestroy()
        {
            transform.DOKill(transform);
        }
    }
}