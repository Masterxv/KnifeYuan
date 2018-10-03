using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KnifeGame
{
    public class KnifeRemainController : MonoBehaviour
    {
        [SerializeField] private Transform _knifeIcon;
        public int KnifeNumber { private get; set; }
        [SerializeField] [Range(0, 1)] private float _colorShift;
        private /*readonly*/ List<Transform> _knifes = new List<Transform>(16);

        private int _currentKnifeIndex;

        public void CreateKnifeRemain()
        {
            ResetKnife();
            for (var i = 0; i < KnifeNumber; i++)
            {
                var instance = Instantiate(_knifeIcon);
                instance.SetParent(transform);

                _knifes.Add(instance);
                instance.localScale = Vector3.one;
            }

            ResetColor();
        }

        public void ChangeKnifeIconColor()
        {
            var image = _knifes[_currentKnifeIndex].GetComponent<Image>();
            var color = image.color;

            image.color = new Color(color.r - _colorShift, color.g - _colorShift, color.b - _colorShift, color.a);
            _currentKnifeIndex++;
        }

        private void ResetKnife()
        {
            _knifes.Clear();
            _currentKnifeIndex = 0;
            var childs = transform.childCount;
            for (var i = 0; i < childs; i++)
            {
                Destroy(transform.GetChild(i).gameObject); // remove from UI
            }
        }

        private void ResetColor()
        {
            for (var i = 0; i < KnifeNumber; i++)
            {
                var image = _knifes[i].GetComponent<Image>();
                image.color = Color.white;
            }
        }
    }
}