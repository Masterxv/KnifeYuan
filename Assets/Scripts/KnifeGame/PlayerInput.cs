using System;
using UnityEngine;

namespace KnifeGame
{
    public class PlayerInput : MonoBehaviour
    {
        public static Action PausePress;
//        public static Action AnyKeyPress;
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
#if DEBUG
            Touched = Input.GetMouseButtonDown(0);
#else
            Touched = Input.touchCount > 0;
#endif
        }

        private void HandleInputPause()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                PausePress();
        }

//        private void HandleInputGameOver()
//        {
//            if (Input.anyKeyDown)
//            {
//                AnyKeyPress();
//            }
//        }
    }
}