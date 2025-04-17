using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Managers
{
    public static class AssetLoader<T> where T : UnityEngine.Object
    {
        public static async Task<T> LoadObject(string assetName)
        {
            T handleResult;
            try
            {
                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetName);
                await handle.Task;
                handleResult = handle.Result;
                Addressables.Release(handle);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return handleResult;
        }

        public static async Task<IList<T>> LoadList(AssetReference assetReference)
        {
            List<T> newAssetList = new List<T>();
            try
            {
                AsyncOperationHandle<T[]> handle = Addressables.LoadAssetAsync<T[]>(assetReference);
                await handle.Task;
                IList<T> objects = handle.Result;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    newAssetList.AddRange(objects);
                    Addressables.Release(handle);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"{assetReference.Asset.name} could not be loaded! Because {ex}");
            }

            return newAssetList;
        }
    }
}
