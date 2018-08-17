using UnityEngine;

namespace KnifeGame
{
    public class DotManager : MonoBehaviourHelper
    {
        public SpriteRenderer lineSprite;
        public SpriteRenderer DotSprite;
        public float position = 0;
        private bool _isBlack;
        public bool isOnCircle = false;
        public bool isEnable = false;    

        public bool isBlack
        {
            get { return _isBlack; }
            set
            {
                _isBlack = value;
                if (value)
                {
                    DotSprite.color = constant.SquareColor;
                    DotSprite.sortingOrder = 10;
                }
                else
                {
                    DotSprite.color = constant.DotColor;
                    DotSprite.sortingOrder = 1;

                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,
                        transform.localPosition.z - 0.01f);
                }

                lineSprite.color = DotSprite.color;
            }
        }

        private void Awake()
        {
            isBlack = false;
            DotSprite.color = constant.DotColor;
            Reset();
        }

        private void Reset()
        {
            transform.rotation = Quaternion.identity;
            if (lineSprite != null)
                lineSprite.color = Color.clear;

            isOnCircle = true;

            transform.localScale = Vector3.one;

            transform.rotation = Quaternion.identity;

            isBlack = false;

            DotSprite.color = new Color(DotSprite.color.r, DotSprite.color.g, DotSprite.color.b, 1f);
        }

        public void ActivateLine(Vector3 target, Transform circleBorder)
        {
            transform.position = target;
            position = Vector2.Distance(target, circleBorder.position);
            transform.parent = circleBorder;
            transform.localScale = Vector3.one;
            if (lineSprite != null)
            {
                lineSprite.transform.localScale = new Vector3(position/2f, lineSprite.transform.localScale.y,
                    lineSprite.transform.localScale.z);
            }
        }
    }
}