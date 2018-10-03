using System;
using UnityEngine;

namespace KnifeGame
{
    public enum KnifeDirection
    {
        Up,
        Left,
        Right
    }

    [Serializable]
    public class LevelManager
    {
        public int LevelId;
        public KnifeDirection KnifeDirection = KnifeDirection.Up;
        public int NumberOfKnife;
        public GameObject CirclePrefab;
        public GameObject SeperatedCirclePref;
        public bool HasObstacle;
        public float RepeatRate;
        public bool HasTimer;
        public float TimeInMin;
    }
}