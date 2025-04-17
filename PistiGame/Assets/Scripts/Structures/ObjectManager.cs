using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

public static class ObjectManager
{
    private static readonly Dictionary<string, LinkedList<GameObject>> ObjectPool = new();
    private static readonly Dictionary<string, Transform> ObjectParent = new();
    public static async UniTask CreatePool(object objectReference, int poolSize)
    {
        var key = GetKey(objectReference);
        var loadedList = await AssetManager<GameObject>.LoadObjects(objectReference);
        if (!ObjectPool.ContainsKey(key))
            ObjectPool[key] = new LinkedList<GameObject>();
        if (!ObjectParent.ContainsKey(key))
        {
            var parent = new GameObject(objectReference.ToString()).transform;
            ObjectParent[key] = parent;
        }

        foreach (var loadedObject in loadedList)
        {
            for (var i = 0; i < poolSize; i++)
            {
                var createdObject = Object.Instantiate(loadedObject, Vector3.one * -100, Quaternion.identity, ObjectParent[key]);
                createdObject.SetActive(false);
                ObjectPool[key].AddLast(createdObject);
            }
        }
    }
    public static void ClearAllPools()
    {
        foreach (var objectPool in ObjectPool)
        {
            foreach (var poolObject in objectPool.Value)
            {
                if (poolObject)
                    Object.Destroy(poolObject.gameObject);
            }
        }

        foreach (var parentPool in ObjectParent)
        {
            if (parentPool.Value)
                Object.Destroy(parentPool.Value.gameObject);
        }

        ObjectPool.Clear();
        ObjectParent.Clear();
    }
    public static void ClearPool(object objectReference)
    {
        string key = GetKey(objectReference);
        if (ObjectPool.TryGetValue(key, out var pool))
        {
            while (pool.Count > 0)
            {
                var poolObject = pool.First.Value;
                if (poolObject != null)
                    Object.Destroy(poolObject);
            }

            ObjectPool.Remove(key);
            if (ObjectParent.TryGetValue(key, out var parent))
                Object.Destroy(parent.gameObject);
        }
    }
    private static async UniTask<GameObject> AddPool(object objectReference, CancellationToken token = default)
    {
        var key = GetKey(objectReference);
        if (!ObjectPool.ContainsKey(key))
            ObjectPool[key] = new LinkedList<GameObject>();
        if (!ObjectParent.ContainsKey(key))
        {
            var parent = new GameObject(objectReference.ToString()).transform;
            ObjectParent[key] = parent;
        }
        GameObject loadedObject;

        if (token != CancellationToken.None)
        {
            loadedObject = await AssetManager<GameObject>.LoadObject(objectReference);
        }
        else
        {
            loadedObject = await AssetManager<GameObject>.LoadObject(objectReference);
            if (loadedObject == null)
            {
                Debug.LogError($"Failed to load object for reference: {objectReference} in AddPool method.");
                return null;
            }
        }

        var createdObject = Object.Instantiate(loadedObject, Vector3.one * -100, Quaternion.identity, ObjectParent[key]);
        createdObject.SetActive(false);
        ObjectPool[key].AddLast(createdObject);
        return createdObject;
    }
    public static async UniTask<GameObject> GetObject(object objectReference, Vector3? spawnPosition = null, Quaternion? spawnRotation = null, CancellationToken token = default)
    {
        var key = GetKey(objectReference);
        if (ObjectPool.TryGetValue(key, out var pool))
        {
            foreach (var poolObject in pool)
            {
                if (poolObject == null) continue;
                if (poolObject.activeSelf) continue;
                if (spawnPosition.HasValue)
                    poolObject.transform.position = spawnPosition.Value;
                if (spawnRotation.HasValue)
                    poolObject.transform.rotation = spawnRotation.Value;
                poolObject.SetActive(true);
                PoolSortQueue(objectReference, poolObject);
                return poolObject;
            }
        }
        var newPoolObject = await AddPool(objectReference, token);
        newPoolObject.transform.position = spawnPosition ?? Vector3.zero;
        newPoolObject.transform.rotation = spawnRotation ?? Quaternion.identity;
        newPoolObject.SetActive(true);
        return newPoolObject;
    }
    private static string GetKey(object objectReference)
    {
        return objectReference switch
        {
            string str => str,
            AssetReference assetReference => assetReference.AssetGUID,
            _ => objectReference.ToString()
        };
    }
    private static void PoolSortQueue(object objectReference, GameObject gameObject)
    {
        var key = GetKey(objectReference);
        if (!ObjectPool.TryGetValue(key, out var value))
            return;
        ObjectPool[key].RemoveFirst();
        ObjectPool[key].AddLast(gameObject);
    }
}