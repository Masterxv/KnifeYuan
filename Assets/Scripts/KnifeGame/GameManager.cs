using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace KnifeGame
{
    public class GameManager : MonoBehaviourHelper
    {
        public GameObject[] AllLevels;

        [Tooltip("Manage number of every level, we start from 0, so becareful, element 0 = LEVEL 1")]
        public int[] KnifeNumEveryLevel;

        [SerializeField] private GameObject[] _allKnifePrefab;
        [SerializeField] private Transform _knifeSpawnPos;
        [SerializeField] private Transform _targetSpawnPos;

//        [SerializeField] private Transform _knifeParent;
        [SerializeField] private Transform _hitTargetPrefab;
        [SerializeField] private Transform _hitApplePrefab;
        [SerializeField] private KnifeRemainController _knifeRemainController;
        [SerializeField] private GameObject _seperatedTargetPrefab;

        [SerializeField] private GameObject _seperatedApplePrefab;

//        [SerializeField] [Tooltip("Number of knife in each level")]
//        private int _knifeRemain;
        [SerializeField] private float _nextStageDelay;
        [SerializeField] private float _gameOverDelay;

        private List<Transform> _knifes = new List<Transform>(16);
        private List<KnifeController> _knifeControllers = new List<KnifeController>(16);

        private int _levelIndex; // start from 0
        private int _knifeRemain;
        private GameObject _target;
        private TargetController _targetController;
        private int _currentKnifeIndex = 0;

        private bool _isReady = false;

        /*[HideInInspector] */
        public bool winLevel = true; // use this to change the color of fill-level image
        [HideInInspector] public int Score = 0;
        [HideInInspector] public int Apple = 0;

        private ParticleSystem _particle;

        private void Awake()
        {
            GC.Collect();
            AddTouchListener();
        }

        void Start()
        {
        }

        public void CreateGame()
        {
            // instance the level prefab from Choose Level Controller
//            var level = Util.GetLastLevelPlayed();
            _levelIndex = 1; // level should begin from 0
            _knifeRemain = KnifeNumEveryLevel[_levelIndex];
            _isReady = true;
            GameState.SetGameState(State.Playing);

            var rot = Quaternion.Euler(0, 0, Random.Range(0, 30));
            Instantiate(AllLevels[_levelIndex], _targetSpawnPos.position, rot);
            Camera.main.orthographicSize = 7f;

            SpawnKnifeAndGetComponent();

            _knifeRemainController.KnifeNumber = _knifeRemain;
            _knifeRemainController.CreateKnifeRemain();

            _knifeControllers[_currentKnifeIndex].OnHit += OnKnifeHit;
            _target = GameObject.FindGameObjectWithTag(TagAndString.TARGET_TAG);
            _targetController = _target.GetComponent<TargetController>();
            _hitTargetPrefab = Instantiate(_hitTargetPrefab);
            _particle = _hitTargetPrefab.GetComponent<ParticleSystem>();
        }

        private void SpawnKnifeAndGetComponent()
        {
            for (var i = 0; i < KnifeNumEveryLevel[_levelIndex]; i++)
            {
                var knife = Instantiate(_allKnifePrefab[0], _knifeSpawnPos.position, _knifeSpawnPos.rotation);
                knife.SetActive(false);
                var kc = knife.GetComponent<KnifeController>();
//                knife.transform.SetParent(_knifeParent);

                _knifes.Add(knife.transform);
                _knifeControllers.Add(kc);
                kc.CenterOfTarget = GameObject.FindGameObjectWithTag(TagAndString.CENTER_OF_TARGET).transform;
//                _knifeSpriteRenderers.Add(knife.GetComponent<SpriteRenderer>());
            }

            _knifes[_currentKnifeIndex].gameObject.SetActive(true);
        }

        private void OnKnifeHit(Transform hitTransform)
        {
            switch (hitTransform.tag)
            {
                case TagAndString.TARGET_TAG: // "Target"
                    KnifeHitTarget();
                    break;
                case TagAndString.APPLE_TAG: // "Apple"
                    KnifeHitApple(hitTransform);
                    break;
                case TagAndString.BLACK_APPLE_TAG:
                    KnifeHitBlackApple(hitTransform);
                    break;
                case TagAndString.KNIFE_TAG: // "Knife"
                    KnifeHitOtherKnife();
                    break;
                default: break;
            }
        }

        private void KnifeHitOtherKnife()
        {
            _knifeControllers[_currentKnifeIndex].OnHit -= OnKnifeHit;
            _targetController.PlayHitImpact();
            // play hit effect
            _targetController.enabled = false;
            _knifeControllers[_currentKnifeIndex].KnifeBound();

            // set game start to GAME OVER
            GameState.SetGameState(State.GameOver);
//            StartCoroutine(GameOverRoutine());
        }

        private void KnifeHitBlackApple(Component hitTransform)
        {
            print("hit the black apple");
            _targetController.enabled = false;
            _targetController.PlayHitImpact();
            hitTransform.gameObject.SetActive(false);
            _knifeControllers[_currentKnifeIndex].KnifeBound();

            // set game start to GAME OVER
            GameState.SetGameState(State.GameOver);
//            StartCoroutine(GameOverRoutine());
        }

        private void KnifeHitApple(Transform hitTransform)
        {
            Instantiate(_hitApplePrefab, hitTransform.position, Quaternion.identity);
            Instantiate(_seperatedApplePrefab, hitTransform.position, Quaternion.identity);
            UpdateApple(); // update for apple text
            hitTransform.gameObject.SetActive(false);
        }

        private void KnifeHitTarget()
        {
            _knifeControllers[_currentKnifeIndex].OnHit -= OnKnifeHit;
            if (_knifeRemain == 0)
            {
                ShakerCamera();
                SeperateTarget(); // disable target and instantiate many pieces

                _knifes[_currentKnifeIndex].SetParent(_target.transform);
                UpdateScore();
                _hitTargetPrefab.position = new Vector3(0, 1.0f, 0);
                _particle.Play();
                // go to next level
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
        }

        private IEnumerator LoadNextStageRoutine()
        {
            foreach (var knife in _knifeControllers)
            {
                knife.KnifeFlyAppart();
            }

            yield return new WaitForSeconds(_nextStageDelay);
        }

        private void GameOverRoutine()
        {
//            GameState.SetGameState(State.GameOver);
//            GarbageCollect();
//            yield return new WaitForSeconds(_gameOverDelay);
//            enabled = false;
        }


        void AddTouchListener()
        {
            InputTouch.onTouchDown += delegate(Vector3 pos)
            {
                if (pos.x < Screen.width && pos.y < Screen.height)
                {
                    if (_knifeRemain > 0 && _isReady)
                    {
                        _knifeControllers[_currentKnifeIndex].Throw();
                        _knifeRemainController.ChangeKnifeIconColor();
                        _knifeRemain--;
                        _isReady = false;
                    }
                }
            };
        }

        private void UpdateApple()
        {
            Apple++;
            canvasManager.AppleText.text = Apple.ToString();
        }

        private void UpdateScore()
        {
            Score++;
            canvasManager.ScoreText.text = Score.ToString();
        }

        private void OnEnable()
        {
            GameState.OnPaused += Paused;
            GameState.OnPlaying += Playing;
            GameState.OnGameOver += GameOver;
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

        private void GameOver()
        {
            canvasManager.BlinkBackground();
            GarbageCollect();
        }

        private void Paused()
        {
            StartCoroutine(PauseRoutine());
        }

        private IEnumerator PauseRoutine()
        {
            canvasManager.PauseText.gameObject.SetActive(true);

            while (GameState.GetGameState() == State.Paused)
                yield return new WaitForEndOfFrame();
            canvasManager.PauseText.gameObject.SetActive(false);
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

        private static void ShakerCamera()
        {
            Camera.main.DOShakePosition(duration: 0.2f, strength: 0.1f, vibrato: 1, randomness: 2.0f)
                .SetEase(Ease.OutQuart);
        }

        private void SeperateTarget()
        {
            _target.gameObject.SetActive(false);
            Instantiate(_seperatedTargetPrefab, _target.transform.position,
                _target.transform.rotation); // instantiate seperated target object
        }

        private void GarbageCollect()
        {
            var knifeImpacts = FindObjectsOfType<KnifeImpactTarget>();
            if (knifeImpacts == null) return;
            foreach (var ki in knifeImpacts)
                Destroy(ki.gameObject);

            var knifeAfterFlies = FindObjectsOfType<KnifeAfterFly>();
            if (knifeAfterFlies == null) return;
            foreach (var kaf in knifeAfterFlies)
                Destroy(kaf.gameObject);

            var targetFlyAparts = FindObjectsOfType<TargetFlyAppart>();
            if (targetFlyAparts == null) return;
            foreach (var tfa in targetFlyAparts)
                Destroy(tfa.gameObject);

            var halfAppleGroups = FindObjectsOfType<HalfAppleGroup>();
            if (halfAppleGroups == null) return;
            foreach (var hag in halfAppleGroups)
                Destroy(hag.gameObject);

            // target controller
            var targetControllers = FindObjectsOfType<TargetController>();
            if (targetControllers == null) return;
            foreach (var tc in targetControllers)
                Destroy(tc.gameObject);
        }

        private void NextKnife()
        {
//            if (_selectedKnifeSpriteIndex + 1 >= _knifeSprites.Count)
//            {
//                _selectedKnifeSpriteIndex = 0;
//                PlayerInput.Swiped = false;
//            }
//            else
//            {
//                _selectedKnifeSpriteIndex++;
//                PlayerInput.Swiped = false;
//            }
//
//            UpdateKnifes();
        }

        private void UpdateKnifes()
        {
//            foreach (var knife in _knifeSpriteRenderers)
//            {
//                knife.sprite = _knifeSprites[_selectedKnifeSpriteIndex];
//            }
        }
    }
}