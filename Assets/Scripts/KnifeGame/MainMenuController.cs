using UnityEngine;
using UnityEngine.SceneManagement;

namespace KnifeGame
{
    public class MainMenuController : MonoBehaviourHelper
    {
        [SerializeField] private GameObject _chooseLevelPrefab;
        [SerializeField] private GameObject _mainMenuGroup;

        void Start()
        {
        }

        void Update()
        {
        }

        public void PlayGame()
        {
            Util.ChooseLevelBool = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void GoToChooseLevel()
        {
            Instantiate(_chooseLevelPrefab, Vector3.zero, Quaternion.identity);
            _mainMenuGroup.SetActive(false);
        }
    }
}