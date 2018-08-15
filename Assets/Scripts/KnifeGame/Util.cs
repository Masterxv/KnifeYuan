using UnityEngine;
using UnityEngine.SceneManagement;

namespace KnifeGame
{
    public static class Util
    {
        private static string LEVEL_UNLOCKED = "LEVEL";
        private static string COUNT_GAMEOVER = "COUNTGAMEOVER";
        private static string LAST_LEVEL_PLAYED = "LEVEL_PLAYED";
        private static string NUMBER_LEVEL_PLAYED = "NUMBEROFLEVELPLAYED";

        public static void SetMaxLevelUnlock(int num)
        {
            PlayerPrefs.SetInt(LEVEL_UNLOCKED, num);
            PlayerPrefs.Save();
        }

        public static int GetMaxLevelUnlock()
        {
            return PlayerPrefs.GetInt(LEVEL_UNLOCKED, 1);
        }

        public static void SetLastLevelPlayed(int num)
        {
            PlayerPrefs.SetInt(LAST_LEVEL_PLAYED, num);
            PlayerPrefs.Save();
        }

        public static int GetLastLevelPlayed()
        {
            return PlayerPrefs.GetInt(LAST_LEVEL_PLAYED);
        }

        public static void ReloadCurrentLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void SetNumberOfLevelPlayed(int num)
        {
            PlayerPrefs.SetInt(NUMBER_LEVEL_PLAYED, num);
            PlayerPrefs.Save();
        }

        public static int GetNumberOfLevelPlayed()
        {
            return PlayerPrefs.GetInt(NUMBER_LEVEL_PLAYED, 0);
        }

        public static string GetCurrentLevelName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public static bool ActiveButtonNext()
        {
            int currentLevel = GetLastLevelPlayed();

            int max = GetMaxLevelUnlock();
            bool canUnlock = false;

            if (currentLevel < max)
            {
                canUnlock = true;
            }

            return canUnlock;
        }

        public static bool ActiveButtonPrevious()
        {
            int currentLevel = GetLastLevelPlayed();
            bool canUnlock = false;

            if (currentLevel > 1)
            {
                canUnlock = true;
            }

            return canUnlock;
        }

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