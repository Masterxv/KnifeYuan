using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KnifeGame
{
    public class KnifeRemainController : MonoBehaviour
    {
        [SerializeField] private Transform _knifeIcon;
        public int KnifeNumber { get; set; }
        [SerializeField] [Range(0, 1)] private float _colorShift;
        private readonly List<Transform> _knifes = new List<Transform>(16);

        private int _currentKnifeIndex;

        void Start()
        {
            for (int i = 0; i < KnifeNumber; i++)
            {
                var instance = Instantiate(_knifeIcon);
                instance.SetParent(transform);

                _knifes.Add(instance);
            }
        }

        public void ChangeKnifeIconColor()
        {
            var image = _knifes[_currentKnifeIndex].GetComponent<Image>();
            var color = image.color;

            image.color = new Color(color.r - _colorShift, color.g - _colorShift, color.b - _colorShift, color.a);
            _currentKnifeIndex++;
        }
    }
}