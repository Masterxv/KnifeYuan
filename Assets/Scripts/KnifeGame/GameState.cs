using System;
using UnityEngine;

namespace KnifeGame
{
    public enum State
    {
        Playing,
        Paused,
        GameOver,
        WinLevel
    }

    public class GameState : MonoBehaviour
    {
        private static State _gameState;
        public static Action OnGameOver;
        public static Action OnPaused;
        public static Action OnPlaying;

        public static State GetGameState()
        {
            return _gameState;
        }

        public static void SetGameState(State state)
        {
            switch (state)
            {
                case State.Playing:
                    SetStatePlaying();
                    break;
                case State.Paused:
                    SetStatePaused();
                    break;
                case State.GameOver:
                    SetStateOver();
                    break;
                case State.WinLevel:
                    SetStateWinLevel();
                    break;
            }
        }

        private static void SetStateWinLevel()
        {
            _gameState = State.WinLevel;
        }

        private static void SetStatePlaying()
        {
            _gameState = State.Playing;
            OnPlaying();
        }

        private static void SetStatePaused()
        {
            _gameState = State.Paused;
            OnPaused();
        }

        private static void SetStateOver()
        {
            _gameState = State.GameOver;
            OnGameOver();
        }
    }
}