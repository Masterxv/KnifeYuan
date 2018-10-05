using UnityEngine;

namespace KnifeGame
{
    public static class Util
    {
        public static bool ChooseLevelBool = true;
        public static int MaxLevel = 100;
        private static string LEVEL_UNLOCKED = "LEVEL";
        private static string CHOOSE_LEVEL = "CHOOSE_LEVEL";
        private static string APPLE = "APPLE";
        private static string BEST_SCORE = "BEST_SCORE";
        private static string LAST_LEVEL_PLAYED = "LEVEL_PLAYED";

        private static string NUMBER_LEVEL_PLAYED = "NUMBEROFLEVELPLAYED";
//        private static string COUNT_GAMEOVER = "COUNTGAMEOVER";

        private static string MUSIC_VOLUME = "MusicVol";
        private static string SOUND_VOLUME = "SoundVol";

        #region LEVEL UNLOCK

        public static void SetMaxLevelUnlock(int num)
        {
            if (num > MaxLevel) num = MaxLevel;

            var maxLevelUnlock = GetMaxLevelUnlock();
            if (num < maxLevelUnlock) return;

            PlayerPrefs.SetInt(LEVEL_UNLOCKED, num);
            PlayerPrefs.Save();
        }

        public static int GetMaxLevelUnlock()
        {
            return PlayerPrefs.GetInt(LEVEL_UNLOCKED, 0);
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
            if (newScore < currentBest) return;
            PlayerPrefs.SetInt(BEST_SCORE, newScore);
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
//            if (num > MaxLevel) return;
            PlayerPrefs.SetInt(LAST_LEVEL_PLAYED, num);
            PlayerPrefs.Save();
        }

        public static int GetLastLevelPlayed()
        {
            return PlayerPrefs.GetInt(LAST_LEVEL_PLAYED, 0);
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
            var canUnlock = currentLevel > 0;

            return canUnlock;
        }

        #endregion

        #region VOLUME

        public static void SetLastMusicVolume(float num)
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME, num);
            PlayerPrefs.Save();
        }

        public static void SetLastSoundVolume(float num)
        {
            PlayerPrefs.SetFloat(SOUND_VOLUME, num);
            PlayerPrefs.Save();
        }

        public static float GetLastMusicVolume()
        {
            return PlayerPrefs.GetFloat(MUSIC_VOLUME, 0);
        }

        public static float GetLastSoundVolume()
        {
            return PlayerPrefs.GetFloat(SOUND_VOLUME, 0);
        }

        #endregion

//        public static void SetCountGameOver(int count)
//        {
//            PlayerPrefs.SetInt(COUNT_GAMEOVER, count);
//            PlayerPrefs.Save();
//        }

        // SetSound()
        // SetSoundOn()
        // SetSoundOff()
    }
}