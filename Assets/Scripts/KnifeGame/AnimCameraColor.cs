using DG.Tweening;
using UnityEngine;

namespace KnifeGame
{
    public class AnimCameraColor : MonoBehaviourHelper
    {
        void Start()
        {
            AnimColor();
        }

        void AnimColor()
        {
            var c = constant.RandomBrightColor();
            Camera.main.DOColor(c, Random.Range(3, 10)).OnComplete(AnimColor); // time is less than under line
//            Camera.main.DOColor(c, Random.Range(3, 10)).SetLoops(-1, LoopType.Incremental);
        }
    }
}