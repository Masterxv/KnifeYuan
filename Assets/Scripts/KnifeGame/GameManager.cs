using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KnifeGame
{
    public class GameManager : MonoBehaviourHelper
    {
        #region PUBLIC

        public LevelManager[] LevelManager;
        public ShopManager[] ShopManager;

        [SerializeField] private Transform _knifeSpawnPos;
        [SerializeField] private Transform _circleSpawnPos;
        [SerializeField] private Transform _hitCirclePrefab;
        [SerializeField] private Transform _hitApplePrefab;
        [SerializeField] private Transform _hitBlackApplePrefab;
        [SerializeField] private KnifeRemainController _knifeRemainController;
        [SerializeField] private GameObject _seperatedApplePrefab;
        [SerializeField] private float _nextStageDelay;

        public Transform UpSideSetup;
        public Transform LeftSideSetup;
        public Transform RightSideSetup;
        public GameObject PlatformPrefab;
        private GameObject _platformObject;
        public bool WaitToMove { get; set; }
        #endregion


        #region PRIVATE

        private List<Transform> _knifes = new List<Transform>(16);
        private List<KnifeController> _knifeControllers = new List<KnifeController>(16);
        private int _levelIndex;
        private int _knifeRemain;
        private CircleController _circleController;
        private int _currentKnifeIndex = 0;
        private bool _isReady;
        [HideInInspector] public bool winLevel = true; // use this to change the color of fill-level image
        private int _score;
        private int _apple;
        private ParticleSystem _hitCircleParticle;
        private ParticleSystem _hitAppleParticle;
        private ParticleSystem _hitBlackAppleParticle;
        private GameObject _circle;
        private Quaternion _knifeRot;

        public int LevelIndex
        {
            get { return _levelIndex; }
        }

        

        #endregion

        private void Awake()
        {
//            PlayerPrefs.DeleteAll();
            GC.Collect();
            Application.targetFrameRate = Application.isMobilePlatform ? 30 : 60;
        }

        private void Start()
        {
            AddTouchListener();

            CreateFx();
        }

        private void CreateFx()
        {
            if (_hitAppleParticle == null)
            {
                var hitAppleFx = Instantiate(_hitApplePrefab, new Vector3(100f, 100f, 0), Quaternion.identity);
                _hitAppleParticle = hitAppleFx.GetComponent<ParticleSystem>();
            }

            if (_hitCircleParticle == null)
            {
                var hitCircleFx = Instantiate(_hitCirclePrefab, new Vector3(100, 100f, 0), Quaternion.identity);
                _hitCircleParticle = hitCircleFx.GetComponent<ParticleSystem>();
            }

            if (_hitBlackAppleParticle == null)
            {
                var hitBlackAppleFx = Instantiate(_hitBlackApplePrefab, new Vector3(100, 100, 0), Quaternion.identity);
                _hitBlackAppleParticle = hitBlackAppleFx.GetComponent<ParticleSystem>();
            }
        }

        public void CreateGame()
        {
            GarbageCollect();

            _levelIndex = Util.ChooseLevelBool ? Util.GetLevelByChoosing() : Util.GetLastLevelPlayed();
//            print(Util.GetMaxLevelUnlock());
//            print("Last level: " + Util.GetLastLevelPlayed());
            Util.SetLastLevelPlayed(_levelIndex);

            if (_platformObject == null)
                _platformObject = Instantiate(PlatformPrefab, Vector3.zero, Quaternion.identity);
            _platformObject.SetActive(false);

            SetupForKnifeDirection();

            _knifeRemain = LevelManager[_levelIndex].NumberOfKnife;
            _isReady = false;
            _apple = Util.GetApplePlayerGet();

            Camera.main.orthographicSize = 7.5f;
            GameState.SetGameState(State.Playing);

            var circleRot = Quaternion.Euler(0, 0, Random.Range(0, 180f));
            _circle = Instantiate(LevelManager[_levelIndex].CirclePrefab, _circleSpawnPos.position, circleRot);

            SpawnKnifeAndGetComponent();

            _knifeRemainController.KnifeNumber = _knifeRemain;
            _knifeRemainController.CreateKnifeRemain();

            _knifeControllers[_currentKnifeIndex].OnHit += OnKnifeHit;
            _circleController = _circle.GetComponent<CircleController>();
        }

        void SetupForKnifeDirection()
        {
            var dir = LevelManager[_levelIndex].KnifeDirection;
            switch (dir)
            {
                case KnifeDirection.Up:
                    UpSetup();
                    break;
                case KnifeDirection.Left:
                    LeftSetup();
                    break;
                case KnifeDirection.Right:
                    RightSetup();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpSetup()
        {
            _knifeSpawnPos = UpSideSetup;
        }

        private void LeftSetup()
        {
            _knifeSpawnPos = LeftSideSetup;
            _platformObject.SetActive(true);
        }

        private void RightSetup()
        {
            _knifeSpawnPos = RightSideSetup;
            _platformObject.SetActive(true);
        }

        private void AddTouchListener()
        {
            InputTouch.onTouchDown += delegate(Vector3 pos)
            {
                if (pos.x < Screen.width && pos.y < Screen.height * 0.9f)
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

        public void SetGameReady()
        {
            _isReady = true;
            canvasManager.ShowGroup(true);
            canvasManager.CheckToShowButton();
        }

        private void SpawnKnifeAndGetComponent()
        {
            ResetKnife();
            for (var i = 0; i < LevelManager[_levelIndex].NumberOfKnife; i++)
            {
                // set to shopKnifeIndex
                var knife = Instantiate(ShopManager[0].KnifePrefab, _knifeSpawnPos.position, _knifeSpawnPos.rotation);
                knife.SetActive(false);
                var kc = knife.GetComponent<KnifeController>();
                knife.transform.SetParent(transform);

                _knifes.Add(knife.transform);
                _knifeControllers.Add(kc);
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
                case TagAndString.CIRCLE_TAG: // "Target"
                    KnifeHitCircle();
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
            }
        }

        private void KnifeHitOtherKnife()
        {
            _knifeControllers[_currentKnifeIndex].OnHit -= OnKnifeHit;
            _circleController.PlayHitImpact(); // play hit effect
            _circleController.enabled = false;
            _knifeControllers[_currentKnifeIndex].KnifeBound();

            winLevel = false;
            // set game start to GAME OVER
            GameState.SetGameState(State.GameOver);
        }

        private void KnifeHitBlackApple(Transform hitTransform)
        {
//            print("hit black apple");
            _hitBlackAppleParticle.transform.position = hitTransform.position;
            _hitBlackAppleParticle.Play();
            _knifeControllers[_currentKnifeIndex].OnHit -= OnKnifeHit;
            _circleController.PlayHitImpact();

            _knifeControllers[_currentKnifeIndex].KnifeBound();

            winLevel = false;
            GameState.SetGameState(State.GameOver);
        }

        private void KnifeHitApple(Transform hitTransform)
        {
            _hitAppleParticle.transform.position = hitTransform.position;
            _hitAppleParticle.Play();
            Instantiate(_seperatedApplePrefab, hitTransform.position, Quaternion.identity);
            UpdateApple(); // update for apple text
        }

        private void KnifeHitCircle()
        {
            _knifeControllers[_currentKnifeIndex].OnHit -= OnKnifeHit;
            if (_knifeRemain == 0)
            {
                ShakerCamera();
                UpdateScore();
                _hitCircleParticle.transform.position = _knifeControllers[_currentKnifeIndex].HitPosition;
                _hitCircleParticle.Play();

                StartCoroutine(LoadNextStageRoutine());
                winLevel = true;
                return;
            }

            _hitCircleParticle.transform.position = _knifeControllers[_currentKnifeIndex].HitPosition;
            _hitCircleParticle.Play();

            _circleController.PlayKnifeHitTarget();
            _currentKnifeIndex++;
            _knifes[_currentKnifeIndex].gameObject.SetActive(true);
            _knifeControllers[_currentKnifeIndex].OnHit += OnKnifeHit;
            UpdateScore();

            _isReady = true;
        }

        private IEnumerator LoadNextStageRoutine()
        {
            _platformObject.gameObject.SetActive(false);
            var tempPos = _circle.transform.position;
            Instantiate(LevelManager[_levelIndex].SeperatedCirclePref, tempPos, Quaternion.identity);
            foreach (var knife in _knifeControllers)
            {
                knife.KnifeFlyAppart();
            }

            _circle.SetActive(false);
            canvasManager.ShowGroup(false);
            yield return new WaitForSeconds(_nextStageDelay);
            _circle.transform.position = new Vector3(1000, 1000, 0);
            _circle.SetActive(true);

            Util.ChooseLevelBool = false;
            _levelIndex++;

            Util.SetLastLevelPlayed(_levelIndex);
            Util.SetMaxLevelUnlock(_levelIndex); // we have compare function inside
            canvasManager.ZoomImageIn();
        }

        private void UpdateApple()
        {
            _apple++;
            canvasManager.AppleText.text = _apple.ToString();
            Util.SetApplePlayerGet(_apple);
        }

        private void UpdateScore()
        {
            _score++;
            canvasManager.ScoreText.text = _score.ToString();
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
            _platformObject.gameObject.SetActive(false);
            // try to set best score
            Util.SetBestScore(_score);

//            Util.SetLastLevelPlayed(_levelIndex);
            // reset score
            _score = 0;
            canvasManager.ResetScoreText();
            // save the apple
            Util.SetApplePlayerGet(_apple);
            canvasManager.BlinkBackGround();
            canvasManager.ShowGroup(false);
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

        public void GarbageCollect() // call from Canvas Manager, when blinking background is completed
        {
            var targetController = FindObjectOfType<CircleController>();
            if (targetController != null)
            {
                Destroy(targetController.gameObject);
            }

            var knifeImpact = FindObjectOfType<KnifeImpactCircle>();
            if (knifeImpact != null)
            {
            }

            var appleImpact = FindObjectOfType<AppleImpact>();
            if (appleImpact != null)
            {
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

            var seperatedCircles = FindObjectsOfType<SeperatedCircle>();
            if (seperatedCircles != null)
            {
                foreach (var ts in seperatedCircles)
                    Destroy(ts.gameObject);
            }

            var circleFlyAparts = FindObjectsOfType<CircleFlyAppart>();
            if (circleFlyAparts != null)
            {
                foreach (var tfa in circleFlyAparts)
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