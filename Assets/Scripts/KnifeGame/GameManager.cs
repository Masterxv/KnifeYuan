using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KnifeGame
{
    public class GameManager : Singleton<GameManager>
    {
        public GameObject PauseText;
        [SerializeField] private GameObject knifePrefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private int _knifeRemain;
        [SerializeField] private KnifeRemainController _knifeRemainController;

        private List<Transform> _knifes = new List<Transform>(16);
        List<KnifeController> _knifeControllers = new List<KnifeController>(16);
        private int _currentKnifeIndex = 0;
        private bool _isReady = true;

        private void Awake()
        {
            _knifeRemainController.KnifeNumber = _knifeRemain;
            SpawnKnife();
        }

        void Start()
        {
            PauseText.SetActive(false);
            GameState.SetGameState(State.Playing);
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

        private void OnEnable()
        {
            GameState.OnPlaying += Playing;
            GameState.OnPaused += Paused;
            GameState.OnGameOver += GameOver;
            PlayerInput.PausePress += PausePressed;
//            PlayerInput.AnyKeyPress += AnyKeyPressed;
        }

        private void OnDisable()
        {
            GameState.OnPlaying -= Playing;
            GameState.OnPaused -= Paused;
            GameState.OnGameOver -= GameOver;
            PlayerInput.PausePress -= PausePressed;
//            PlayerInput.AnyKeyPress -= AnyKeyPressed;
        }

        private void Playing()
        {
            StartCoroutine(PlayRoutine());
        }

        IEnumerator PlayRoutine()
        {
            while (GameState.GetGameState() == State.Playing)
            {
                // if knife amount =0 Game Over (knife is out of bound)

                // if knife on knife
                // if knife on apple
                // add score
                yield return new WaitForEndOfFrame();
            }
        }

        private void Paused()
        {
            StartCoroutine(PauseRoutine());
        }

        IEnumerator PauseRoutine()
        {
            PauseText.SetActive(true);

            while (GameState.GetGameState() == State.Paused)
            {
                yield return new WaitForEndOfFrame();
            }

            PauseText.SetActive(false);
        }

        private void GameOver()
        {
        }

//        private void AnyKeyPressed()
//        {
//        }

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


        public void ActiveNextKnife()
        {
            _knifes[_currentKnifeIndex].gameObject.SetActive(true);
        }
    }
}