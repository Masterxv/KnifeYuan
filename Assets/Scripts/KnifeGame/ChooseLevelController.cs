using UnityEngine;
using UnityEngine.SceneManagement;

namespace KnifeGame
{
    public class ChooseLevelController : MonoBehaviourHelper
    {
        void Start()
        {
        }

        void Update()
        {
        }

        public void GoToLevel(int level) // start from 0
        {
            Util.ChooseLevelBool = true;
            Util.SetLevelByChoosing(level);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}