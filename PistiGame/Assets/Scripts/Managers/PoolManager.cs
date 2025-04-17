using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Structures;
using UnityEngine;

namespace Managers
{
    public class PoolManager : Singleton<PoolManager>
    {
        public Transform defaultParent;
        private readonly List<PoolData> _pools = new List<PoolData>();

        public Task CreatePool(GameObject objectPrefabs, PoolType poolType, int poolLenght,string poolParentName = null,[CanBeNull] string objectName=null)
        {
            if (_pools.Any(pool => pool.poolType == poolType))
            {
                PoolData existingPool = _pools.Find(pool => pool.poolType == poolType);
                existingPool.AddNewObjects(objectPrefabs, poolLenght,objectName);
            }
            else
            {
                PoolData newPoolData = new PoolData(objectPrefabs, poolType, poolLenght, FindPoolParent(poolParentName),objectName);
                _pools.Add(newPoolData);
            }

            return Task.CompletedTask;
        }
        private Transform FindPoolParent(string poolParentName)
        {
            if (poolParentName == null)
                return defaultParent;
            GameObject poolParentObject = GameObject.Find(poolParentName);
            return poolParentObject != null ? poolParentObject.transform : defaultParent;
        }
        public GameObject GetObjectFromPool(PoolType targetPoolType,[CanBeNull] string objectName=null)
        {
            PoolData existingPool = _pools.Find(pool => pool.poolType == targetPoolType);
            return existingPool.GetObjectFromPool(objectName);
        }
    }
}
