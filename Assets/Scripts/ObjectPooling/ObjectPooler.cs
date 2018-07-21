using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    [System.Serializable]
    public class ObjectPoolItem
    {
        public int amountToPool;
        public GameObject objectToPool;
        public bool shouldExpand;
    }

    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler SharedInstance;
        [SerializeField] private List<GameObject> pooledObjects;
        public List<ObjectPoolItem> itemsToPool;

        private void Awake()
        {
            SharedInstance = this;
        }

        private void Start()
        {
            pooledObjects = new List<GameObject>();

            foreach (var item in itemsToPool)
            {
                for (var i = 0; i < item.amountToPool; i++)
                {
                    var obj = Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                }
            }
        }

        public GameObject GetPooledObject(string tag)
        {
            // if object is not active -> get it, otherwise return null
            foreach (var obj in pooledObjects)
            {
                if (!obj.activeInHierarchy && obj.CompareTag(tag))
                {
                    return obj;
                }
            }
            foreach (var item in itemsToPool)
            {
                if (item.objectToPool.CompareTag(tag))
                {
                    if (item.shouldExpand)
                    {
                        var obj = Instantiate(item.objectToPool);
                        obj.SetActive(false);
                        pooledObjects.Add(obj);
                    }
                }
            }
            return null;
        }
    }
}