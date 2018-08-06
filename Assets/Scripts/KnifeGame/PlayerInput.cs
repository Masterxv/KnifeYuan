using System;
using UnityEngine;

namespace KnifeGame
{
    public class PlayerInput : MonoBehaviour
    {
        public static Action PausePress;
        public static bool Touched;
        public static bool Swiped;
        private Vector2 _position0;
        private Vector2 _position1;

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
                default: break;
            }
        }

        private void HandleInputPlaying()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Touched = Input.GetMouseButtonDown(0);
                // swip in Window editor
//                if (Input.GetMouseButtonDown(0))
//                    _position0 = Input.mousePosition;
//                if (Input.GetMouseButtonUp(0))
//                {
//                    _position1 = Input.mousePosition;
//                    if (Vector3.Distance(_position0, _position1) > 0.01f)
//                        Swiped = true;
//                }
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                Touched = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
//                Swiped = Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(1).phase == TouchPhase.Moved; // not finish android's swipe
            }
        }

        private void HandleInputPause()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                PausePress();
        }
    }
}