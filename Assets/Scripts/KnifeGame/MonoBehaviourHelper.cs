using UnityEngine;

namespace KnifeGame
{
    public class MonoBehaviourHelper : MonoBehaviour
    {
//        private PoolSystem _poolSystem;
//
//        public PoolSystem poolSystem
//        {
//            get
//            {
//                if (_poolSystem == null)
//                {
//                    _poolSystem = FindObjectOfType<PoolSystem>();
//                }
//
//                return _poolSystem;
//            }
//        }

        private GameManager _gameManager;

        protected GameManager gameManager
        {
            get
            {
                if (_gameManager == null)
                {
                    _gameManager = FindObjectOfType<GameManager>();
                }

                return _gameManager;
            }
        }

        private CanvasManager _canvasManager;

        protected CanvasManager canvasManager
        {
            get
            {
                if (_canvasManager == null)
                {
                    _canvasManager = FindObjectOfType<CanvasManager>();
                }

                return _canvasManager;
            }
        }

        private Constant _constant;

        protected Constant constant
        {
            get
            {
                if (_constant == null)
                {
                    _constant = FindObjectOfType<Constant>();
                }

                return _constant;
            }
        }

        private ChooseLevelController _chooseLevelController;

        protected ChooseLevelController chooseLevelController
        {
            get
            {
                if (_chooseLevelController == null)
                    _chooseLevelController = FindObjectOfType<ChooseLevelController>();
                return _chooseLevelController;
            }
        }

        private AudioManager _audioManager;
        protected AudioManager audioManager
        {
            get {
                if (_audioManager == null)
                    _audioManager = FindObjectOfType<AudioManager>();
                return _audioManager;
            }
        }

        private MainMenuController _mainMenu;

        protected MainMenuController mainMenuController
        {
            get
            {
                if (_mainMenu == null)
                    _mainMenu = FindObjectOfType<MainMenuController>();
                return _mainMenu;
            }
        }
    }
}