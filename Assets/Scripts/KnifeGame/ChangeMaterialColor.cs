using DG.Tweening;
using UnityEngine;

namespace KnifeGame
{
    public class ChangeMaterialColor : MonoBehaviourHelper
    {
        private Renderer r;
        public bool isAngle;

        private const string _UVOffsetX = "_UVXOffset";
        private const string _UVOffsetY = "_UVYOffset";
        private const string _Offset = "_Offset";
        private const string _Angle = "_Angle";
        private const string _Color = "_Color";
        private const string _Color2 = "_Color2";

        void Start()
        {
            r = GetComponent<Renderer>();
            AnimColor1();
            AnimColor2();
            AnimOffset();
            AnimUVOfffsetX();
            AnimUVOfffsetY();
        }


        private void AnimUVOfffsetX()
        {
            r.material.DOFloat(Random.Range(-0.5f, 0.5f), _UVOffsetX, Random.Range(5, 20)).OnComplete(AnimUVOfffsetX);
        }

        private void AnimUVOfffsetY()
        {
            r.material.DOFloat(Random.Range(-0.5f, 0.5f), _UVOffsetY, Random.Range(5, 20)).OnComplete(AnimUVOfffsetY);
        }

        private void AnimOffset()
        {
            if (isAngle)
            {
                r.material.DOFloat(Random.Range(-360f, 360f), _Angle, Random.Range(5, 20)).OnComplete(AnimOffset);
            }
            else
            {
                r.material.DOFloat(Random.Range(-360f, 360f), _Offset, Random.Range(5, 20)).OnComplete(AnimOffset);
            }
        }

        private void AnimColor2()
        {
            Color c = constant.RandomBrightColor();
            if (Random.Range(0, 2) == 0)
            {
                c = new Color(c.r, c.g, c.b, Random.Range(0.4f, 0.6f));
            }

            r.material.DOColor(c, _Color2, Random.Range(3, 10)).OnComplete(AnimColor1);
        }

        private void AnimColor1()
        {
            Color c = constant.RandomBrightColor();
            if (Random.Range(0, 2) == 0)
            {
                c = new Color(c.r, c.g, c.b, Random.Range(0.4f, 0.6f));
            }

            r.material.DOColor(c, _Color, Random.Range(3, 10)).OnComplete(AnimColor1);
        }
    }
}