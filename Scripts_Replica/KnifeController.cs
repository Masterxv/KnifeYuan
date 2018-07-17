using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class KnifeController : MonoBehaviour
{
    [SerializeField] private float _speed;

    [SerializeField] private float _mass;

    [SerializeField] private float _torque;

    [SerializeField] private float _forceMultiplier;

    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;

    public event Action<Transform> OnHit;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Бросок ножа в цель
    /// </summary>
    public void Throw()
    {
        _rigidbody.velocity = Vector2.up * _speed;
    }

    /// <summary>
    /// Взаимодействие с тригером
    /// </summary>
    /// <param name="other">коллайдер объекта взаимодействия</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Apple"))
        {
            _rigidbody.velocity = Vector2.zero;
            _collider.isTrigger = true;
            _collider.size = new Vector2(_collider.size.x, 0.075f);
            _collider.offset = new Vector2(0, -0.03f);
        }

        OnHit?.Invoke(other.transform);
    }

    /// <summary>
    /// Отскок одного ножа от другого
    /// </summary>
    public void KnifeBounce()
    {
        _collider.enabled = false;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.mass = _mass;
        _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Отбрасывает нож в сторону от цели
    /// </summary>
    public void KnifeFlyAppart()
    {
        _collider.enabled = false;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        var direction = transform.position;
        transform.SetParent(null);
        _rigidbody.mass = _mass;
        _rigidbody.AddTorque(_torque, ForceMode2D.Impulse);
        _rigidbody.AddForce(direction.normalized * _forceMultiplier);
    }
}