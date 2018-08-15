using System;
using DG.Tweening;
using UnityEngine;

namespace KnifeGame
{
    public class LevelManager : MonoBehaviourHelper
    {
        private void Awake()
        {
            int level = Util.GetLastLevelPlayed();

            if (level > 100)
            {
                Util.SetLastLevelPlayed(100);
            }
        }

        public Level GetLevel(int level)
        {
            Level l = new Level(level);
            return l;
        }
    }

    [Serializable]
    public class Level
    {
        private static int maxLevel = 100;
        private int levelNumber = 0;
        public float rotateDelay = 8f;
        public int numberDotsOnCircle = 0; // number of obstacles
        public Ease rotateEaseType = Ease.Linear;
        public LoopType rotateLoopType = LoopType.Incremental;
        public float sizeRayonRation = 0.25f; // position of obstacles

        public Level(int level)
        {
            levelNumber = level;
            rotateEaseType = Ease.Linear;
            rotateLoopType = LoopType.Incremental;
            sizeRayonRation = .25f;
            rotateDelay = 20 - (level % 10);
            if (level % 2 > 1)
            {
                rotateLoopType = LoopType.Incremental;
            }
            else
            {
                rotateLoopType = LoopType.Yoyo;
            }

            int numOfEnum = (Enum.GetValues(typeof(Ease)).Length);

            int enumNumber = level % numOfEnum;
            rotateEaseType = (Ease) (enumNumber);

            while (rotateEaseType.ToString().Contains("Elastic") ||
                   rotateEaseType.ToString().Contains("INTERNAL_Zero") ||
                   rotateEaseType.ToString().Contains("INTERNAL_Custom"))
            {
                enumNumber++;
                if (enumNumber >= numOfEnum)
                    enumNumber = 0;

                rotateEaseType = (Ease) (enumNumber);
            }

            numberDotsOnCircle = 10 + level % 35;

            if (level < 12)
                numberDotsOnCircle = 5;

            if (level == 1)
                numberDotsOnCircle = 1;

            if (level == 2)
                numberDotsOnCircle = 2;

            if (level > maxLevel)
            {
                Util.SetLastLevelPlayed(100);

                level = 100;

                Util.SetMaxLevelUnlock(100);
            }
        }
    }
}