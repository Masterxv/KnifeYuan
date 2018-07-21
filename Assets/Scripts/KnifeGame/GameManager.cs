using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KnifeGame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Text PauseText;
        [SerializeField] private Text AppleText;
        [SerializeField] private Text ScoreText;
        [SerializeField] private Image _backgroundFade;
        [SerializeField] private GameObject knifePrefab;
        [SerializeField] private Transform _hitTargetPrefab;
        [SerializeField] private Transform _hitApplePrefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private int _knifeRemain;
        [SerializeField] private KnifeRemainController _knifeRemainController;
        [SerializeField] private GameObject _seperatedTargetPrefab;
        [SerializeField] private GameObject _seperatedApplePrefab;
        [SerializeField] private float _nextStageDelay;
        [SerializeField] private float _gameOverDelay;
        [SerializeField] private Button _restartButton;

        private List<Transform> _knifes = new List<Transform>(16);
        List<KnifeController> _knifeControllers = new List<KnifeController>(16);

        private GameObject _target;
        private TargetController _targetController;
        private int _currentKnifeIndex = 0;
        private bool _isReady = false;
        private static int _score;
        private static int _apple = 0;
        private static GameManager _instance;
        private ParticleSystem _particle;

        public static GameManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _knifeRemainController.KnifeNumber = _knifeRemain;
            SpawnKnife();
            AppleText.text = _apple.ToString();
            ScoreText.text = _score.ToString();
            
            _knifeControllers[_currentKnifeIndex].OnHit += OnKnifeHit;
            
            _target = GameObject.FindGameObjectWithTag("Target");
            _targetController = _target.GetComponent<TargetController>();
            
            _hitTargetPrefab = Instantiate(_hitTargetPrefab);
            _particle = _hitTargetPrefab.GetComponent<ParticleSystem>();
            
        }

        void Start()
        {
            FadeBackground();

            PauseText.gameObject.SetActive(false);
            GameState.SetGameState(State.Playing);
            _restartButton.onClick.AddListener(RestartLevel);
            _restartButton.gameObject.SetActive(false);
            
        }

        private void FadeBackground()
        {
            _backgroundFade.DOFade(0, 0.5f).SetEase(Ease.InQuart).OnComplete(SetGameToReady);
        }

        private void SetGameToReady()
        {
            _isReady = true;
        }
        private void SpawnKnife()
        {
            for (var i = 0; i < _knifeRemain; i++)
            {
                var knife = Instantiate(knifePrefab, spawnPosition.position, spawnPosition.rotation);
                knife.SetActive(false);

                _knifes.Add(knife.transform);
                _knifeControllers.Add(knife.GetComponent<KnifeController>());
            }

            _knifes[_currentKnifeIndex].gameObject.SetActive(true);
        }

        private void OnKnifeHit(Transform hitTransform)
        {
            switch (hitTransform.tag)
            {
                case "Target":
                    _knifeControllers[_currentKnifeIndex].OnHit -= OnKnifeHit;
                    if (_knifeRemain == 0)
                    {
                        ShakerCamera();
                        AnimateTarget();
                        StartCoroutine(LoadNextStageRoutine());
                        return;
                    }
                    _targetController.KnifeHitTarget(); // play shake animation
                    _knifes[_currentKnifeIndex].SetParent(_target.transform);
                    _currentKnifeIndex++;
                    _knifes[_currentKnifeIndex].gameObject.SetActive(true); // active next knife
                    _knifeControllers[_currentKnifeIndex].OnHit += OnKnifeHit;
                    // update score
                    UpdateScore();
                    _hitTargetPrefab.position = new Vector3(0, 0.6f, 0);
                    _particle.Play();
                    _isReady = true;
                    break;
                case "Apple":
                    Instantiate(_hitApplePrefab, hitTransform.position, Quaternion.identity);
                    Instantiate(_seperatedApplePrefab, hitTransform.position, Quaternion.identity);
                    UpdateApple(); // update for apple text
                    hitTransform.gameObject.SetActive(false);
                    break;
                case "Knife":
                    _knifeControllers[_currentKnifeIndex].OnHit -= OnKnifeHit;
                    _targetController.PlayHitImpact();
                    // play hit effect
                    _targetController.enabled = false;
                    _knifeControllers[_currentKnifeIndex].KnifeBound();
                    
                    // start coroutine game over
                    StartCoroutine(GameOverRoutine());
                    break;
                default: break;
            }
        }

        private IEnumerator LoadNextStageRoutine()
        {
            foreach (var knife in _knifeControllers)
            {
                knife.enabled = true;
                knife.KnifeFlyAppart();
            }

            yield return new WaitForSeconds(_nextStageDelay);
            if (SceneManager.GetActiveScene().buildIndex + 1 <= SceneManager.sceneCount)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else
                Restart();
        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(_gameOverDelay);
            _restartButton.gameObject.SetActive(true);
            enabled = false;
        }

        private void Restart()
        {
            SceneManager.LoadScene(0);
        }

        private void RestartLevel()
        {
            SceneManager.LoadScene(0);
            _apple = 0;
            _score = 0;
        }

        private void Update()
        {
            if (PlayerInput.Touched)
            {
                if (_knifeRemain > 0 && _isReady)
                {
                    _knifeControllers[_currentKnifeIndex].Throw();
                    _knifeRemainController.ChangeKnifeIconColor();
                    _knifeRemain--;
                    _isReady = false;
                }
            }
        }

        private void UpdateApple()
        {
            _apple++;
            AppleText.text = _apple.ToString();
        }

        private void UpdateScore()
        {
            _score++;
            ScoreText.text = _score.ToString();
        }

        private void OnEnable()
        {
            GameState.OnPaused += Paused;
            GameState.OnPlaying += Playing;
//            GameState.OnGameOver += GameOver;
            PlayerInput.PausePress += PausePressed;
        }

        private void OnDisable()
        {
            GameState.OnPaused -= Paused;
            PlayerInput.PausePress -= PausePressed;
            GameState.OnPlaying -= Playing;
        }

        private void Playing()
        {
        }

        private void Paused()
        {
            StartCoroutine(PauseRoutine());
        }

        private IEnumerator PauseRoutine()
        {
            PauseText.gameObject.SetActive(true);

            while (GameState.GetGameState() == State.Paused)
                yield return new WaitForEndOfFrame();
            PauseText.gameObject.SetActive(false);
        }


        private void PausePressed()
        {
            if (GameState.GetGameState() == State.Playing)
            {
                GameState.SetGameState(State.Paused);
            }
            else if (GameState.GetGameState() == State.Paused)
            {
                GameState.SetGameState(State.Playing);
            }
        }
        private void ShakerCamera()
        {
            Camera.main.DOShakePosition(duration: 0.2f, strength: 1, vibrato: 30, randomness: 90.0f)
                .SetEase(Ease.OutQuart);
        }

        private void AnimateTarget()
        {
            _target.gameObject.SetActive(false);
            Instantiate(_seperatedTargetPrefab, _target.transform.position, _target.transform.rotation); // instantiate seperated target object
        }
    }
}