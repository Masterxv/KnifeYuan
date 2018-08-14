using UnityEngine;
using UnityEngine.SceneManagement;

namespace KnifeGame
{
    public class ChooseLevelController : MonoBehaviour
    {
        void Start()
        {
        }

        void Update()
        {
        }

        public void GoToLevel(int level) // start from 0
        {
            PlayerPrefs.SetInt(TagAndString.Level, level);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}