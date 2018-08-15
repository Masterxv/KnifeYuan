using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KnifeGame
{
    public class GameManager : MonoBehaviour
    {
        private Sequence _sequence;
        private bool isGameOver;
        private int _level;
        public Transform CircleBorder;
        private LoopType loopType = LoopType.Incremental;
        private Vector3 rotateVector = Vector3.one;
        private float rotateCircleDelay = 5f;
        private Ease easeType = Ease.Linear;

        private void Awake()
        {
            AddTouchListener();
            Util.SetCountGameOver(0);
        }

        private void AddTouchListener()
        {
            InputTouch.onTouchDown += delegate(Vector3 pos)
            {
                if (pos.x < Screen.width && pos.y < Screen.height /** 0.9f*/)
                {
                    print("DoJump"); // throw the knife
                }
            };
        }

        void Start()
        {
            CreateGame(_level);
        }

        void Update()
        {
        }

        void CreateGame(int level)
        {
            _sequence?.Kill();

            // if sequenceDOT = null -> reset
            isGameOver = false;
            _level = Util.GetLastLevelPlayed();
            Time.timeScale = 1;
            GC.Collect();
            Application.targetFrameRate = 60;

            // for the circleBorder
            DOTween.Kill(CircleBorder);
            CircleBorder.localScale = Vector3.one * 0.5f;
            CircleBorder.DOScale(Vector3.one, 0.3f).SetDelay(0.2f).SetEase(Ease.InBack).OnComplete(LaunchRotateCircle);

            rotateVector = new Vector3(0, 0, 1);
            if (_level % 2 == 0)
            {
                rotateVector = new Vector3(0, 0, -1);
            }
        }

        private void LaunchRotateCircle()
        {
            SequenceLogic();
        }

        private void SequenceLogic()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            if (loopType == LoopType.Incremental)
            {
                _sequence.Append(CircleBorder
                    .DORotate(-rotateVector * Random.Range(360, 520), rotateCircleDelay, RotateMode.FastBeyond360)
                    .SetEase(easeType));
                _sequence.SetLoops(1, loopType);
            }
            else
            {
                _sequence.Append(CircleBorder
                    .DORotate(-rotateVector * Random.Range(360, 520), rotateCircleDelay, RotateMode.FastBeyond360)
                    .SetEase(easeType));
                _sequence.SetLoops(2, loopType);
            }

            _sequence.OnStepComplete(SequenceLogic);

            _sequence.Play();
        }
    }
}