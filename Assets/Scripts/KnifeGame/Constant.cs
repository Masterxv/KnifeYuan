using System.Collections.Generic;
using UnityEngine;

namespace KnifeGame
{
    public class Constant : MonoBehaviourHelper
    {
        public List<Color> BackgroundColors = new List<Color>();
        public Color SquareColor;
        public Color DotColor;

        void Start()
        {
        }

        void Update()
        {
        }

        public Color RandomBrightColor()
        {
            if (BackgroundColors == null || BackgroundColors.Count == 0)
            {
                return Color.white;
            }

            return BackgroundColors[Random.Range(0, BackgroundColors.Count)];
        }
    }
}