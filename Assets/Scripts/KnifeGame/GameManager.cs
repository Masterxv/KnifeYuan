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
        [SerializeField] private float _nextStageDelay;
        public GameObject[] SeperatedFruits;

        public Transform UpSideSetup;
        public Transform LeftSideSetup;
        public Transform RightSideSetup;
        [SerializeField] private GameObject _platformRight;
        [SerializeField] private GameObject _platformLeft;
        [SerializeField] private GameObject _borderObject;
        public Transform[] Obstacles;

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
        private ParticleSystem _hitFruitParticle;
        private ParticleSystem _hitBlackFruitParticle;
        private GameObject _circle;
        private Quaternion _knifeRot;
        private int _fruitIndex = 0;
        private Vector2 _obstaclePos;

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
            if (_hitFruitParticle == null)
            {
                var hitAppleFx = Instantiate(_hitApplePrefab, new Vector3(100f, 100f, 0), Quaternion.identity);
                _hitFruitParticle = hitAppleFx.GetComponent<ParticleSystem>();
            }

            if (_hitCircleParticle == null)
            {
                var hitCircleFx = Instantiate(_hitCirclePrefab, new Vector3(100, 100f, 0), Quaternion.identity);
                _hitCircleParticle = hitCircleFx.GetComponent<ParticleSystem>();
            }

            if (_hitBlackFruitParticle == null)
            {
                var hitBlackAppleFx = Instantiate(_hitBlackApplePrefab, new Vector3(100, 100, 0), Quaternion.identity);
                _hitBlackFruitParticle = hitBlackAppleFx.GetComponent<ParticleSystem>();
            }
        }

        public void CreateGame()
        {
            GarbageCollect();

            _levelIndex = Util.ChooseLevelBool ? Util.GetLevelByChoosing() : Util.GetLastLevelPlayed();
//            _levelIndex = 99;
            Util.SetLastLevelPlayed(_levelIndex);

            SetupPlatform();

            SetupForKnifeDirection();

            _knifeRemain = LevelManager[_levelIndex].NumberOfKnife;
            _isReady = false;
            _apple = Util.GetApplePlayerGet();

//            Camera.main.orthographicSize = 8f;
            GameState.SetGameState(State.Playing);

            var circleRot = Quaternion.Euler(0, 0, Random.Range(0, 180f));
            _circle = Instantiate(LevelManager[_levelIndex].CirclePrefab, _circleSpawnPos.position, circleRot);

            SpawnKnifeAndGetComponent();

            _knifeRemainController.KnifeNumber = _knifeRemain;
            _knifeRemainController.CreateKnifeRemain();

            _circleController = _circle.GetComponent<CircleController>();
            _borderObject.SetActive(true);
            _knifeRemainController.gameObject.SetActive(true);
        }

        private void SetupPlatform()
        {
//            https://answers.unity.com/questions/62189/detect-edge-of-screen.html
            var dist = (transform.position - Camera.main.transform.position).z;
            var rightX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
            var leftX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
            var middleY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.418f, dist)).y;

            _platformRight.transform.position = new Vector2(rightX, middleY);
            _platformRight.SetActive(false);

            _platformLeft.transform.position = new Vector2(leftX, middleY);
            _platformLeft.SetActive(false);
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
            _platformRight.SetActive(true);
            _platformLeft.SetActive(true);
        }

        private void RightSetup()
        {
            _knifeSpawnPos = RightSideSetup;
            _platformRight.SetActive(true);
            _platformLeft.SetActive(true);
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

            if (LevelManager[_levelIndex].HasObstacle)
            {
                var rate = LevelManager[_levelIndex].RepeatRate;
                InvokeRepeating(nameof(CreateObstacle), 1.5f, rate); // wait A second, create obstacle every B second
            }

            if (LevelManager[_levelIndex].HasTimer)
            {
                canvasManager.StartClock(LevelManager[_levelIndex].TimeInMin);
            }
        }

        private void CreateObstacle()
        {
            if (!_isReady) return;
            // get the position
            var dist = (transform.position - Camera.main.transform.position).z;
            var rightX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
            var leftX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
            var middleY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, dist)).y;
            rightX += 1.2f;
            leftX -= 1.2f;
            middleY += Random.Range(-2.4f, 0.8f);

            _obstaclePos = Random.value > 0.5f ? new Vector2(rightX, middleY) : new Vector2(leftX, middleY);

            var ran = Random.Range(0, 7);
            Instantiate(Obstacles[ran], _obstaclePos, Quaternion.identity);
        }

        private void SpawnKnifeAndGetComponent()
        {
            ResetKnife();
            var numKnife = LevelManager[_levelIndex].NumberOfKnife;
            for (var i = 0; i < numKnife; i++)
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

        public void KnifeHitBorder()
        {
            _circleController.enabled = false;
            winLevel = false;
            GameState.SetGameState(State.GameOver);
        }

        public void KnifeHitOtherKnife()
        {
            _circleController.PlayHitImpact(); // play hit effect
            _circleController.enabled = false;
            winLevel = false;
            GameState.SetGameState(State.GameOver);
        }

        public void KnifeHitObstacle(Vector3 hitPoint)
        {
            _hitBlackFruitParticle.transform.position = hitPoint;
            _hitBlackFruitParticle.Play();
            CancelInvoke();
            winLevel = false;
            GameState.SetGameState(State.GameOver);
        }

        public void KnifeHitDeathFruit(Transform hitTransform)
        {
            _hitBlackFruitParticle.transform.position = hitTransform.position;
            _hitBlackFruitParticle.Play();
            _circleController.PlayHitImpact();

            winLevel = false;
            GameState.SetGameState(State.GameOver);
        }

        public void KnifeHitFruit(Transform hitTransform)
        {
            _hitFruitParticle.transform.position = hitTransform.position;
            _hitFruitParticle.Play();
            var hitGameobjectName = hitTransform.gameObject.name;
            _fruitIndex = GetFruitToInstantiate(hitGameobjectName);
            Instantiate(SeperatedFruits[_fruitIndex], hitTransform.position, Quaternion.identity);
            UpdateApple(); // update for apple text
        }

        private int GetFruitToInstantiate(string fruitName)
        {
            var index = 0;
            switch (fruitName)
            {
                case "Apple":
                    index = 0;
                    break;
                case "Avocado":
                    index = 1;
                    break;
                case "Banana":
                    index = 2;
                    break;
                case "Coconut":
                    index = 3;
                    break;
                case "Grape":
                    index = 4;
                    break;
                case "GreenApple2":
                    index = 5;
                    break;
                case "GreenApple":
                    index = 6;
                    break;
                case "Hong":
                    index = 7;
                    break;
                case "Kiwi":
                    index = 8;
                    break;
                case "Lekima":
                    index = 9;
                    break;
                case "Lemon":
                    index = 10;
                    break;
                case "Luu2":
                    index = 11;
                    break;
                case "Luu":
                    index = 12;
                    break;
                case "Lychee":
                    index = 13;
                    break;
                case "Mango2":
                    index = 14;
                    break;
                case "Mango":
                    index = 15;
                    break;
                case "Melon":
                    index = 16;
                    break;
                case "Orange":
                    index = 17;
                    break;
                case "Papaya":
                    index = 18;
                    break;
                case "Pear":
                    index = 19;
                    break;
                case "PeperBell1":
                    index = 20;
                    break;
                case "PeperBell2":
                    index = 21;
                    break;
                case "Pineapple":
                    index = 22;
                    break;
                case "Plum":
                    index = 23;
                    break;
                case "Potato":
                    index = 24;
                    break;
                case "Pumpkin":
                    index = 25;
                    break;
                case "Quyt":
                    index = 26;
                    break;
                case "Raspberry":
                    index = 27;
                    break;
                case "RedApple":
                    index = 28;
                    break;
                case "Strawberry":
                    index = 29;
                    break;
                case "Sung":
                    index = 30;
                    break;
                case "SweetPotato":
                    index = 31;
                    break;
                case "Tomato":
                    index = 32;
                    break;
                case "WaterMelon":
                    index = 33;
                    break;
                case "YellowApple":
                    index = 34;
                    break;

                default: break;
            }

            return index;
        }

        public void KnifeHitCircle()
        {
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

            _circleController.PlayKnifeHitCircle();
            _currentKnifeIndex++;
            if (GameState.GetGameState() != State.GameOver)
                _knifes[_currentKnifeIndex].gameObject.SetActive(true);
            UpdateScore();
            _isReady = true;
        }

        private IEnumerator LoadNextStageRoutine()
        {
            CancelInvoke();
            canvasManager.StopClock();

            _platformRight.SetActive(false);
            _platformLeft.SetActive(false);
            _borderObject.SetActive(false);
            _knifeRemainController.gameObject.SetActive(false);
            var tempPos = _circle.transform.position;
            Instantiate(LevelManager[_levelIndex].SeperatedCirclePref, tempPos, Quaternion.identity);
            foreach (var knife in _knifeControllers)
            {
                knife.KnifeFlyAppart();
                knife.GetComponent<Collider2D>().enabled = false;
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
            GameState.SetGameState(State.WinLevel);
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
            // ReSharper disable once DelegateSubtraction
            GameState.OnPaused -= Paused;
            // ReSharper disable once DelegateSubtraction
            PlayerInput.PausePress -= PausePressed;
            // ReSharper disable once DelegateSubtraction
            GameState.OnPlaying -= Playing;
        }

        private void Playing()
        {
        }

        public void GameOver()
        {
            if (GameState.GetGameState() == State.WinLevel) return;
            CancelInvoke();
            canvasManager.StopClock();
            _platformRight.SetActive(false);
            _platformLeft.SetActive(false);
            _borderObject.SetActive(false);

            _knifeRemainController.gameObject.SetActive(false);
            // try to set best score
            Util.SetBestScore(_score);

            Util.SetLastLevelPlayed(_levelIndex);
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
                .SetEase(Ease.OutExpo).OnComplete(() => DOTween.Kill(Camera.main));
        }

        public void GarbageCollect() // call from Canvas Manager, when blinking background is completed
        {
            var targetController = FindObjectOfType<CircleController>();
            if (targetController != null)
            {
                Destroy(targetController.gameObject);
            }

//            var knifeImpact = FindObjectOfType<KnifeImpactCircle>();
//            if (knifeImpact != null)
//            {
//            }
//
//            var appleImpact = FindObjectOfType<AppleImpact>();
//            if (appleImpact != null)
//            {
//            }


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
                    Destroy(tfa.gameObject);
            }

            var halfAppleGroups = FindObjectsOfType<HalfFruitGroup>();
            if (halfAppleGroups != null)
            {
                foreach (var hag in halfAppleGroups)
                    Destroy(hag.gameObject);
            }
        }
    }
}