using UnityEngine;

namespace KnifeGame
{
    public static class Util
    {
        private static int _maxLevel = 6;
        private static string LEVEL_UNLOCKED = "LEVEL";
        private static string CHOOSE_LEVEL = "CHOOSE_LEVEL";
        private static string APPLE = "APPLE";
        private static string BEST_SCORE = "BEST_SCORE";
        private static string COUNT_GAMEOVER = "COUNTGAMEOVER";
        private static string LAST_LEVEL_PLAYED = "LEVEL_PLAYED";
        private static string NUMBER_LEVEL_PLAYED = "NUMBEROFLEVELPLAYED";
        public static bool ChooseLevelBool = true;

        #region LEVEL UNLOCK

        public static void SetMaxLevelUnlock(int num)
        {
            if (num > _maxLevel) num = _maxLevel;

            var maxLevelUnlock = GetMaxLevelUnlock();
            if (num > maxLevelUnlock)
                PlayerPrefs.SetInt(LEVEL_UNLOCKED, num);

            PlayerPrefs.Save();
        }

        private static int GetMaxLevelUnlock()
        {
            return PlayerPrefs.GetInt(LEVEL_UNLOCKED, 1);
        }

        #endregion

        #region APPLE

        public static void SetApplePlayerGet(int num)
        {
            PlayerPrefs.SetInt(APPLE, num);
            PlayerPrefs.Save();
        }

        public static int GetApplePlayerGet()
        {
            return PlayerPrefs.GetInt(APPLE, 0);
        }

        #endregion

        #region CHOOSE LEVEL FROM CHOOSE SCENE

        public static void SetLevelByChoosing(int num)
        {
            PlayerPrefs.SetInt(CHOOSE_LEVEL, num);
            PlayerPrefs.Save();
        }

        public static int GetLevelByChoosing()
        {
            return PlayerPrefs.GetInt(CHOOSE_LEVEL, 0);
        }

        #endregion

        #region BESTSCORE

        public static void SetBestScore(int newScore)
        {
            var currentBest = GetBestScore();
            if (newScore > currentBest)
            {
                PlayerPrefs.SetInt(BEST_SCORE, newScore);
            }

            PlayerPrefs.Save();
        }

        public static int GetBestScore()
        {
            return PlayerPrefs.GetInt(BEST_SCORE, 0);
        }

        #endregion

        #region LAST LEVEL

        public static void SetLastLevelPlayed(int num)
        {
            var lastLevelPlayed = GetLastLevelPlayed(); // 2
            if (num >= lastLevelPlayed)
                PlayerPrefs.SetInt(LAST_LEVEL_PLAYED, num);

            PlayerPrefs.Save();
        }

        public static int GetLastLevelPlayed()
        {
            return PlayerPrefs.GetInt(LAST_LEVEL_PLAYED);
        }

        #endregion

        #region NUMBER OF LEVEL PLAYED

        public static void SetNumberOfLevelPlayed(int num)
        {
            PlayerPrefs.SetInt(NUMBER_LEVEL_PLAYED, num);
            PlayerPrefs.Save();
        }

        public static int GetNumberOfLevelPlayed()
        {
            return PlayerPrefs.GetInt(NUMBER_LEVEL_PLAYED, 0);
        }

        #endregion

        #region BUTTON NEXT, PREVIOUS

        public static bool ActiveButtonNext()
        {
            var currentLevel = GetLastLevelPlayed();
            var max = GetMaxLevelUnlock();
            var canUnlock = currentLevel < max;

            return canUnlock;
        }

        public static bool ActiveButtonPrevious()
        {
            var currentLevel = GetLastLevelPlayed();
            var canUnlock = currentLevel > 1;

            return canUnlock;
        }

        #endregion


        public static void SetCountGameOver(int count)
        {
            PlayerPrefs.SetInt(COUNT_GAMEOVER, count);
            PlayerPrefs.Save();
        }

        // SetSound()
        // SetSoundOn()
        // SetSoundOff()
    }
}