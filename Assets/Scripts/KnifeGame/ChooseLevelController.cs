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
            Util.SetLastLevelPlayed(level);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}