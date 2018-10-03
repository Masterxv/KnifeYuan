using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KnifeGame
{
    public class ChooseLevelController : MonoBehaviourHelper
    {
        public Transform GridView;
        public Sprite Unlocked;
        public Sprite Locked;
        public Button HomeButton;
        public Button ShopButton;

        private readonly List<LevelControl> _levelControls = new List<LevelControl>(Util.MaxLevel);
        private int _gridChild;
        private string _levelText;

        private void Awake()
        {
            _gridChild = GridView.childCount;
            AddToLevelList();
        }

        void Start()
        {
//          Lock all the buttons by LevelControl function
            SetUnlock();
            AddButtonListener();
        }

        private void AddToLevelList()
        {
            for (var i = 0; i < _gridChild; i++)
            {
                var l = GridView.GetChild(i).GetComponent<LevelControl>();
                _levelControls.Add(l);
            }
        }

        private void SetUnlock()
        {
            // so sánh với GetMaxLevelUnlock()
            // set background to Unlocked và hiển thị Lable, Star
            var maxLevel = Util.GetMaxLevelUnlock();
            for (var i = 0; i <= maxLevel; i++)
            {
                _levelControls[i].Main.sprite = Unlocked;
                _levelControls[i].Main.raycastTarget = true;
                _levelControls[i].Label.gameObject.SetActive(true);
                _levelText = (i + 1).ToString();
                _levelControls[i].Label.text = _levelText;
                _levelControls[i].LButton.interactable = true;
                _levelControls[i].LevelByUser = i;
            }
        }

        private void AddButtonListener()
        {
            HomeButton.onClick.AddListener(HomePressed);
            ShopButton.onClick.AddListener(ShopPress);
        }

        private void HomePressed()
        {
            audioManager.PlayButtonClick();
            mainMenuController.HomePressed();
        }

        private void ShopPress()
        {
            audioManager.PlayButtonClick();
            print("press shop");
        }

        void Update()
        {
        }
    }
}