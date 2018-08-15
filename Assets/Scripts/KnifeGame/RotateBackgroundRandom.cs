using DG.Tweening;
using UnityEngine;

namespace KnifeGame
{
    public class RotateBackgroundRandom : MonoBehaviour
    {
        private Sequence _sequence;

        void Start()
        {
            SequenLogic();
        }

        private void SequenLogic()
        {
            _sequence?.Kill(); // if _sequence != null, => _sequence.Kill()
            _sequence = DOTween.Sequence();

            var rotateVector = new Vector3(0, 0, 1);
            if (Random.Range(0, 2) == 0)
            {
                rotateVector = new Vector3(0, 0, -1);
            }

            _sequence.Append(transform.DORotate(rotateVector * Random.Range(360, 520), Random.Range(3f, 10f),
                RotateMode.FastBeyond360));
            _sequence.SetLoops(1, LoopType.Incremental);
            _sequence.OnStepComplete(SequenLogic);
            _sequence.Play();
        }
    }
}