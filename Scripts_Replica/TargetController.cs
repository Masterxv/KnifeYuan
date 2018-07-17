using UnityEngine;

[RequireComponent(typeof(Animation))]
public class TargetController : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;

    [Range(0, 1)] [SerializeField] private float _changeDirectionChance;

    [SerializeField] private float _minTimeToStop;

    [SerializeField] private float _maxTimeToStop;

    [SerializeField] private float _stopTime;

    [SerializeField] private float _slowdownSpeed;

    [SerializeField] private float _speedUpSpeed;

    private Animation _animation;

    private float _timeToStop;
    private float _stopTimeVar;
    private float _speed;

    // Roation _direction 1 is CW, -1 is CCW
    private int _direction = 1;
    private bool _IsSpeedUp;

    void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    void Start()
    {
        _timeToStop = Random.Range(_minTimeToStop, _maxTimeToStop);
        _stopTimeVar = _stopTime;
        _speed = _rotationSpeed;
    }

    void Update()
    {
        RoatateTarget();
    }

    /// <summary>
    /// Вращает цель
    /// </summary>
    private void RoatateTarget()
    {
        if (_IsSpeedUp)
        {
            _speed += _speedUpSpeed * Time.deltaTime;
            transform.Rotate(Vector3.forward, _speed * _direction * Time.deltaTime);
            if (_speed >= _rotationSpeed)
            {
                //_speedUpSpeedVar = _speedUpSpeed * Time.deltaTime;
                _speed = _rotationSpeed;
                _IsSpeedUp = false;
            }

            return;
        }

        if (_timeToStop <= 0f)
        {
            if (_speed <= 0.1f)
            {
                if (_stopTimeVar <= 0f)
                {
                    _timeToStop = Random.Range(_minTimeToStop, _maxTimeToStop);
                    _stopTimeVar = _stopTime;
                    //_speed = _speedUpSpeed * Time.deltaTime;
                    _IsSpeedUp = true;
                    if (Random.value <= _changeDirectionChance) _direction = -_direction;
                    transform.Rotate(Vector3.forward, _speed * _direction * Time.deltaTime);
                }
                else
                {
                    _stopTimeVar -= Time.deltaTime;
                }
            }
            else
            {
                _speed -= _slowdownSpeed * Time.deltaTime;
                transform.Rotate(Vector3.forward, _speed * _direction * Time.deltaTime);
            }
        }
        else
        {
            _timeToStop -= Time.deltaTime;
            transform.Rotate(Vector3.forward, _rotationSpeed * _direction * Time.deltaTime);
        }
    }

    /// <summary>
    /// Проигрывает анимацию попадания ножа в цель
    /// </summary>
    public void TargetHitImpact()
    {
        _animation.Play("TargetImpact");
    }

    /// <summary>
    /// Проигрывает анимацию попадания ножа в нож
    /// </summary>
    public void KnifeHitImpact()
    {
        _animation.Play("TargetImpactMove");
    }

    /// <summary>
    /// Проигрывает анимацию появления цели
    /// </summary>
    public void TargetAppearance()
    {
        _animation.Play("TargetAppearance");
    }
}