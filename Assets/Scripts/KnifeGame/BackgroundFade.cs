using DG.Tweening;
using UnityEngine;

namespace KnifeGame
{
    public class BackgroundFade : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRen;
        [SerializeField] private float _time;
        [SerializeField] private Ease _easeType = Ease.Linear;
        private Color _nextColor;

        private void Start()
        {
            FadeIn();
        }

        void FadeIn()
        {
            _nextColor = new Color(1, 1, 1, 0.7f);
            _spriteRen.DOColor(_nextColor, _time).SetDelay(7f).SetEase(_easeType).OnComplete(FadeOut);
        }

        void FadeOut()
        {
            _nextColor = new Color(1, 1, 1, 0f);
            _spriteRen.DOColor(_nextColor, _time).SetDelay(7f).SetEase(_easeType).OnComplete(FadeIn);
        }
    }
}