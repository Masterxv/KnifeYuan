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
        [SerializeField] private float _nextStageDelay;

        private List<Transform> _knifes = new List<Transform>(16);
        private List<KnifeController> _knifeControllers = new List<KnifeController>(16);

        private int _levelIndex; // start from 0

        private int _knifeRemain;

//        private GameObject _target;
        private TargetController _targetController;
        private int _currentKnifeIndex = 0;

        private bool _isReady = false;

        [HideInInspector] public bool winLevel = true; // use this to change the color of fill-level image
        [HideInInspector] public int Score = 0;
        [HideInInspector] public int Apple = 0;

        private ParticleSystem _particle;
        private GameObject _targetAtLevel;

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
            // garbage collect
            GarbageCollect();
            canvasManager.ShowLevel();

            _levelIndex = Util.GetLastLevelPlayed(); // _leveIndex should start from 0
            _knifeRemain = KnifeNumEveryLevel[_levelIndex];
            _isReady = false;

            Camera.main.orthographicSize = 7f;
            GameState.SetGameState(State.Playing);

            var rot = Quaternion.Euler(0, 0, Random.Range(0, 180f));
            _targetAtLevel = Instantiate(AllLevels[_levelIndex], _targetSpawnPos.position, rot);

            SpawnKnifeAndGetComponent();

            _knifeRemainController.KnifeNumber = _knifeRemain;
            _knifeRemainController.CreateKnifeRemain();

            _knifeControllers[_currentKnifeIndex].OnHit += OnKnifeHit;
            _targetController = _targetAtLevel.GetComponent<TargetController>();
            var hit = Instantiate(_hitTargetPrefab);
            _particle = hit.GetComponent<ParticleSystem>();
        }

        public void SetGameReady()
        {
            _isReady = true;
        }

        private void SpawnKnifeAndGetComponent()
        {
            ResetKnife();
            for (var i = 0; i < KnifeNumEveryLevel[_levelIndex]; i++)
            {
                var knife = Instantiate(_allKnifePrefab[0], _knifeSpawnPos.position, _knifeSpawnPos.rotation);
                knife.SetActive(false);
                var kc = knife.GetComponent<KnifeController>();
                knife.transform.SetParent(transform);

                _knifes.Add(knife.transform);
                _knifeControllers.Add(kc);
//                _knifeSpriteRenderers.Add(knife.GetComponent<SpriteRenderer>());
            }

            _knifes[_currentKnifeIndex].gameObject.SetActive(true);
        }

        private void ResetKnife()
        {
            _currentKnifeIndex = 0;
            _knifes.Clear();
            _knifeControllers.Clear();
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

            winLevel = false;
            // set game start to GAME OVER
            GameState.SetGameState(State.GameOver);
        }

        private void KnifeHitBlackApple(Component hitTransform)
        {
            _targetController.PlayHitImpact();
            hitTransform.gameObject.SetActive(false);
            _knifeControllers[_currentKnifeIndex].KnifeBound();

            winLevel = false;
            // set game start to GAME OVER
            GameState.SetGameState(State.GameOver);
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

                _knifes[_currentKnifeIndex].SetParent(_targetAtLevel.transform);
                UpdateScore();
                _hitTargetPrefab.position = new Vector3(0, 1.0f, 0);
                _particle.Play();
                // go to next level
                StartCoroutine(LoadNextStageRoutine());
                winLevel = true;
                return;
            }

            _targetController.KnifeHitTarget(); // play shake animation
            _knifes[_currentKnifeIndex].SetParent(_targetAtLevel.transform);
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
            var tempPos = _targetAtLevel.transform.position;
            Instantiate(_seperatedTargetPrefab, tempPos, Quaternion.identity);
            foreach (var knife in _knifeControllers)
            {
                knife.KnifeFlyAppart();
            }

            _targetAtLevel.SetActive(false);
            yield return new WaitForSeconds(_nextStageDelay);
            _targetAtLevel.transform.position = new Vector3(1000, 1000, 0);
            _targetAtLevel.SetActive(true);

            _levelIndex++;

            Util.SetLastLevelPlayed(_levelIndex);
            canvasManager.ZoomImageIn();
        }


        private void AddTouchListener()
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
            canvasManager.ResetBlink();
            canvasManager.BlinkBackground();
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

        public void GarbageCollect() // call from Canvas Manager, when blinking background is completed
        {
            var targetController = FindObjectOfType<TargetController>();
            if (targetController != null)
            {
                Destroy(targetController.gameObject);
            }

            var knifeImpact = FindObjectOfType<KnifeImpactTarget>();
            if (knifeImpact != null)
            {
                Destroy(knifeImpact.gameObject);
            }

            var appleImpact = FindObjectOfType<AppleImpact>();
            if (appleImpact != null)
            {
                Destroy(appleImpact.gameObject);
            }


            // if game over and there are some knives still remain, knives can throw, they are child of game manager
            var children = transform.childCount;
            if (children > 0)
            {
                for (var i = 0; i < children; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }

            var knifeControllers = FindObjectsOfType<KnifeController>();
            if (knifeControllers != null)
            {
                foreach (var kc in knifeControllers)
                    Destroy(kc.gameObject);
            }

            var targetSeperated = FindObjectsOfType<TargetSeperated>();
            if (targetSeperated != null)
            {
                foreach (var ts in targetSeperated)
                    Destroy(ts.gameObject);
            }

            var targetFlyAparts = FindObjectsOfType<TargetFlyAppart>();
            if (targetFlyAparts != null)
            {
                foreach (var tfa in targetFlyAparts)
                {
                    Destroy(tfa.gameObject);
                }
            }

            var halfAppleGroups = FindObjectsOfType<HalfAppleGroup>();
            if (halfAppleGroups != null)
            {
                foreach (var hag in halfAppleGroups)
                    Destroy(hag.gameObject);
            }
        }
    }
}