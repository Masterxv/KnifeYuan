using System;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class K : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rigidbody;

    void Start()
    {
    }

    private void OnEnable()
    {
        _rigidbody.velocity = transform.up * _speed;
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            other.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}