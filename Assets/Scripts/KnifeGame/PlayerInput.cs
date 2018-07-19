using System;
using UnityEngine;

namespace KnifeGame
{
    public class PlayerInput : MonoBehaviour
    {
        public static Action PausePress;
        public static bool Touched;

        void Start()
        {
        }

        void Update()
        {
            switch (GameState.GetGameState())
            {
                case State.Playing:
                    HandleInputPlaying();
                    HandleInputPause();
                    break;
                case State.Paused:
                    HandleInputPause();
                    break;
//                case State.GameOver:
//                    HandleInputGameOver();
//                    break;
                default: break;
            }
        }

        private void HandleInputPlaying()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
                Touched = Input.GetMouseButtonDown(0);

            if (Application.platform == RuntimePlatform.Android)
                Touched = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        }

        private void HandleInputPause()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                PausePress();
        }
    }
}