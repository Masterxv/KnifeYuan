using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnifesRemainController : MonoBehaviour
{
    [SerializeField] private Transform _knifeIcon;

    // Затемнение иконки после броска ножа (можно было бы использовать другой спрайт, но его нет)
    [Range(0, 1)] [SerializeField] private float _colorShift;

    private int _currentKnifeIndex;
    private readonly List<Transform> _knifesList = new List<Transform>(16);

    public int KnifesNumber { get; set; }

    /// <summary>
    /// Инициализация списка иконок ножей
    /// </summary>
    void Start()
    {
        for (var i = 0; i < KnifesNumber; i++)
        {
            var instance = Instantiate(_knifeIcon);
            instance.SetParent(transform);
            _knifesList.Add(instance);
        }
    }

    /// <summary>
    /// Обновление количество оставшихся ножей после броска
    /// </summary>
    public void KnifeThrown()
    {
        var image = _knifesList[_currentKnifeIndex].GetComponent<Image>();
        var color = image.color;
        image.color = new Color(color.r - _colorShift, color.g - _colorShift, color.b - _colorShift, color.a);
        _currentKnifeIndex++;
    }
}