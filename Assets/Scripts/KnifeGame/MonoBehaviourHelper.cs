using UnityEngine;

namespace KnifeGame
{
    public class MonoBehaviourHelper : MonoBehaviour
    {
        private PoolSystem _poolSystem;

        public PoolSystem poolSystem
        {
            get
            {
                if (_poolSystem == null)
                {
                    _poolSystem = FindObjectOfType<PoolSystem>();
                }

                return _poolSystem;
            }
        }

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

        public CanvasManager canvasManager
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

        private LevelManager _levelManager;

        protected LevelManager levelManager
        {
            get
            {
                if (_levelManager == null)
                {
                    _levelManager = FindObjectOfType<LevelManager>();
                }

                return _levelManager;
            }
        }

        private Transform _targetCenter;

        protected Transform targetCenter
        {
            get
            {
                if (_targetCenter == null)
                {
                    _targetCenter = GameObject.FindGameObjectWithTag(TagAndString.CENTER_OF_TARGET).transform;
                }

                return _targetCenter;
            }
        }
    }
}