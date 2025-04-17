using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Structures
{
    [Serializable]
    public class PoolData
    {
        private readonly List<GameObject> _objectPrefabs = new List<GameObject>();
        private readonly Transform _objectsParent;
        public PoolType poolType;
        private readonly List<GameObject> _objectPool = new List<GameObject>();

        public PoolData(GameObject objectPrefabs,PoolType newPoolType,int poolLenght,Transform objectParent,[CanBeNull] string poolObjectName=null)
        {
            _objectPrefabs.Add(objectPrefabs);
            _objectsParent = objectParent;
            poolType = newPoolType;
            for (int i = 0; i < poolLenght; i++)
                SpawnPoolObject(poolObjectName);
        }

        public void AddNewObjects(GameObject objectPrefabs, int poolLenght,[CanBeNull] string objectName=null)
        {
            _objectPrefabs.Add(objectPrefabs);
            for (int i = 0; i < poolLenght; i++)
                SpawnPoolObject(objectName);
        }
        public GameObject GetObjectFromPool([CanBeNull] string objectName=null)
        {
            foreach (GameObject obj in _objectPool)
            {
                if (obj.activeInHierarchy) continue;
                if (objectName == null)
                {
                    obj.SetActive(true);
                    return obj;
                }
                if (obj.name != objectName) continue;
                obj.SetActive(true);
                return obj;
            }
            return SpawnNewPoolObject(objectName);
        }
        private void SpawnPoolObject([CanBeNull] string objectName)
        {
            GameObject prefabToSpawn = _objectPrefabs[_objectPool.Count % _objectPrefabs.Count];
            GameObject newObject = Object.Instantiate(prefabToSpawn, new Vector3(0,-1000,0),Quaternion.identity, _objectsParent);
            newObject.name = objectName ?? prefabToSpawn.name;
            newObject.SetActive(false);
            _objectPool.Add(newObject);
        }
        private GameObject SpawnNewPoolObject([CanBeNull] string objectName)
        {
            GameObject prefabToSpawn = _objectPrefabs[_objectPool.Count % _objectPrefabs.Count];
            GameObject newObject = Object.Instantiate(prefabToSpawn, new Vector3(0,-1000,0),Quaternion.identity, _objectsParent);
            newObject.name = objectName ?? prefabToSpawn.name;
            _objectPool.Add(newObject);
            return newObject;
        }
    }
    public enum PoolType
    {
        Card,
    }
}

