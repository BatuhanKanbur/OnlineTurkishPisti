using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class AssetManager<T> where T : Object
{
    #region Initilzation

    private static bool IsInitialized
    {
        set
        {
            if (value && !Initialized)
                Init();
            Initialized = value;
        }
    }
    private static bool Initialized { get; set; }

    private static void Init()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        IsInitialized = true;
    }

    private static void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        AssetCache.Clear();
    }

    #endregion

    private static readonly Dictionary<object, object> AssetCache = new Dictionary<object, object>();

    public static async UniTask<T> LoadObject(object assetReference)
    {
        await Addressables.InitializeAsync(false);
        if (AssetCache.TryGetValue(assetReference, out var assetObject))
            return (T) assetObject;
        
        var handle = Addressables.LoadAssetAsync<T>(assetReference);
        await handle.ToUniTask();
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AssetCache[assetReference] = handle.Result;
            return handle.Result;
        }
        
        throw new Exception("Failed to load asset.");
    }

    public static async UniTask<List<T>> LoadObjects(object assetReference)
    {
        await Addressables.InitializeAsync(false);
        if (AssetCache.TryGetValue(assetReference, out var assetObject))
            return (List<T>) assetObject;
        
        var handle = Addressables.LoadAssetsAsync<T>(assetReference, null);
        await handle.ToUniTask();
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var newAssetList = new List<T>(handle.Result);
            AssetCache[assetReference] = newAssetList;
            return newAssetList;
        }
        
        Debug.LogError(handle.OperationException);
        return new List<T>();
    }
    public static async UniTask<IList<T>> LoadList(AssetReference assetReference)
    {
        List<T> newAssetList = new List<T>();
        try
        {
            AsyncOperationHandle<T[]> handle = Addressables.LoadAssetAsync<T[]>(assetReference);
            await handle.ToUniTask();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                newAssetList.AddRange(handle.Result);
                Addressables.Release(handle);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"{assetReference.Asset.name} could not be loaded! Because {ex}");
        }

        return newAssetList;
    }

    public static void ReleaseAsset(List<object> assetReference)
    {
        if (assetReference is not {Count: > 0}) return;
        
        foreach (var reference in assetReference)
        {
            if (!AssetCache.TryGetValue(reference, out var assetObject))
            {
                Debug.LogError("Asset not found in cache.");
                return;
            }
            
            Addressables.Release(assetObject);
            AssetCache.Remove(reference);
        }
    }
}
