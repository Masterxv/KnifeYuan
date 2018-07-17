using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private Text _scoreText;

    [SerializeField] private Text _applesText;

    [SerializeField] private Transform _knifePrefab;

    [SerializeField] private List<Sprite> _knifeSprites = new List<Sprite>(16);

    [SerializeField] private Transform _target;

    [SerializeField] private Animation _backgroundImpact;

    [SerializeField] private Transform _hitEffect;

    [SerializeField] private Transform _impactEffect;

    [SerializeField] private KnifesRemainController _knifesRemainController;

    [SerializeField] private Animation _gameOverScreenTint;

    [SerializeField] private GameObject _restartButton;

    [SerializeField] private int _knifesRemain;

    [SerializeField] private Transform _stageClearEffect;

    [SerializeField] private bool _playTargetAppearanceAnimation;

    private static int _score;

    // Количество яблок и выбранный нож по идее должны из бд (файла) загружаться, для простоты сделал так
    private static int _apples;
    private static int _selectedKnifeSpriteIndex;

    private float _gameOverDelay = 0.8f;
    private float _nextStageDelay = 0.6f;
    private int _currentKnifeIndex;
    private bool _isReady = true;

    private TargetController _targetController;
    private ParticleSystem _particles;
    private Camera _mainCamera;

    private List<KnifeController> _knifesControllers = new List<KnifeController>(16);
    private List<Transform> _knifes = new List<Transform>(16);
    private List<SpriteRenderer> _knifesSpriteRenderers = new List<SpriteRenderer>(16);

    /// <summary>
    /// Создание системы частиц, задание количества ножей для контейнера с иконками
    /// </summary>
    void Awake()
    {
        _impactEffect = Instantiate(_impactEffect);
        _particles = _impactEffect.GetComponent<ParticleSystem>();
        _knifesRemainController.KnifesNumber = _knifesRemain;
        _targetController = _target.GetComponent<TargetController>();
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// Задание значений для очков, яблок, создание первого ножа, проигрывание анимации появления цели
    /// </summary>
    void Start()
    {
        _scoreText.text = _score.ToString();
        _applesText.text = _apples.ToString();
        for (int i = 0; i < _knifesRemain; i++)
        {
            var knife = Instantiate(_knifePrefab);
            _knifes.Add(knife);
            _knifesControllers.Add(knife.GetComponent<KnifeController>());
            _knifesSpriteRenderers.Add(knife.GetComponent<SpriteRenderer>());
            //Я решил просто префаб выключить вместо этой строчки
            //knife.gameObject.SetActive(false);
        }

        _knifesControllers[_currentKnifeIndex].OnHit += OnKnifeHit;
        _knifes[_currentKnifeIndex].gameObject.SetActive(true);
        if (_playTargetAppearanceAnimation) _targetController.TargetAppearance();
    }

    /// <summary>
    /// Обработка клика и бросок ножа по возможности
    /// </summary>
    void Update()
    {
#if DEBUG
        if (Input.GetMouseButtonDown(0))
        {
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
#endif
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -_mainCamera.transform.position.z;
            var point = _mainCamera.ScreenToWorldPoint(mousePos);
            var hit = Physics2D.Raycast(point, Vector2.zero, 0f);
            if (hit.collider?.transform == _knifes[_currentKnifeIndex])
            {
                NextKnife();
            }
            else
            {
                if (_knifesRemain > 0 && _isReady)
                {
                    _isReady = false;
                    _knifesControllers[_currentKnifeIndex].Throw();
                    _knifesRemainController.KnifeThrown();
                    _knifesRemain--;
                }
            }
        }
    }

    /// <summary>
    /// Обработка события попадания ножа в другой объект
    /// </summary>
    /// <param name="hitTransform">объект столкновения</param>
    private void OnKnifeHit(Transform hitTransform)
    {
        switch (hitTransform.tag)
        {
            case "Apple":
                UpdateApples();
                hitTransform.gameObject.SetActive(false);
                break;
            case "Knife":
                _knifesControllers[_currentKnifeIndex].OnHit -= OnKnifeHit;
                _targetController.KnifeHitImpact();
                _hitEffect.position = hitTransform.position;
                Instantiate(_hitEffect);
                _backgroundImpact.Play();
                _targetController.enabled = false;
                _knifesControllers[_currentKnifeIndex].KnifeBounce();
                StartCoroutine(GameOver());
                break;
            case "Target":
                _knifesControllers[_currentKnifeIndex].OnHit -= OnKnifeHit;
                if (_knifesRemain == 0)
                {
                    StartCoroutine(nameof(LoadNextStage));
                    return;
                }

                _knifes[_currentKnifeIndex].SetParent(_target);
                _currentKnifeIndex++;
                _knifes[_currentKnifeIndex].gameObject.SetActive(true);
                _knifesControllers[_currentKnifeIndex].OnHit += OnKnifeHit;
                _targetController.TargetHitImpact();
                UpdateScore();
                _impactEffect.position = hitTransform.position;
                _particles.Play();
                _isReady = true;
                break;
        }
    }

    /// <summary>
    /// С задержкой затемняет экран и показывает кнопку рестарта
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(_gameOverDelay);
        _gameOverScreenTint.Play();
        _restartButton.SetActive(true);
        this.enabled = false;
    }

    /// <summary>
    /// Обновление счета
    /// </summary>
    private void UpdateScore()
    {
        _score++;
        _scoreText.text = _score.ToString();
    }

    /// <summary>
    /// Обновление количества яблок
    /// </summary>
    public void UpdateApples()
    {
        _apples += 1;
        _applesText.text = _apples.ToString();
    }

    /// <summary>
    /// Загрузить следующий уровень с задержкой после разлета ножей
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadNextStage()
    {
        Instantiate(_stageClearEffect);
        foreach (var knife in _knifesControllers)
        {
            knife.enabled = true;
            knife.KnifeFlyAppart();
        }

        yield return new WaitForSeconds(_nextStageDelay);
        if (SceneManager.GetActiveScene().buildIndex + 1 <= SceneManager.sceneCount)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else Restart();
    }

    /// <summary>
    /// Начать с начала (метод для кнопки)
    /// </summary>
    public void Restart()
    {
        _score = 0;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Поменять нож
    /// </summary>
    /// <param name="index">индекс выбранного ножа</param>
    public void ChangeKnife(int index)
    {
        if (index >= _knifeSprites.Count)
        {
            Debug.LogError($"Parameter {nameof(index)} is out of range {nameof(_knifeSprites)} list.");
            return;
        }

        _selectedKnifeSpriteIndex = index;
    }

    /// <summary>
    /// Поменять нож на следующий из списка доступных
    /// </summary>
    public void NextKnife()
    {
        if (_selectedKnifeSpriteIndex + 1 >= _knifeSprites.Count) _selectedKnifeSpriteIndex = 0;
        else _selectedKnifeSpriteIndex++;
        UpdateKnifes();
    }

    /// <summary>
    /// Обновляет спрайты всех ножей
    /// </summary>
    private void UpdateKnifes()
    {
        foreach (var knife in _knifesSpriteRenderers)
        {
            knife.sprite = _knifeSprites[_selectedKnifeSpriteIndex];
        }
    }
}