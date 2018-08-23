using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KnifeGame
{
    public class ChooseLevelController : MonoBehaviourHelper
    {
        public Transform GridView;
        private List<LevelControl> _levelControls = new List<LevelControl>(100);

        public Sprite Unlocked;
        public Sprite Locked;

        void Start()
        {
            GetAndLockAll();
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

        private void GetAndLockAll()
        {
            var gridChild = GridView.childCount; // gridChild =100 if there are 100 levels
            for (var i = 0; i < gridChild; i++)
            {
                var l = GridView.GetChild(i).GetComponent<LevelControl>();
                _levelControls.Add(l);
                l.Main.sprite = Locked;
                l.Label.gameObject.SetActive(false);
                l.Star.gameObject.SetActive(false);
            }
        }

    }
}