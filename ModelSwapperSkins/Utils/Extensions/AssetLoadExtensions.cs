using RoR2.ContentManagement;
using System;
using System.Diagnostics;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ModelSwapperSkins.Utils.Extensions
{
    public static class AssetLoadExtensions
    {
        public static AssetOrDirectReference<T> ReferenceOrDirect<T>(this (T directDef, AssetReferenceT<T> assetRef) tuple) where T : UnityEngine.Object
        {
            return new AssetOrDirectReference<T>
            {
                address = tuple.assetRef,
                directRef = tuple.directDef
            };
        }

        public static AssetOrDirectReference<T> ReferenceOrDirect<T>(this (AssetReferenceT<T> assetRef, T directDef) tuple) where T : UnityEngine.Object
        {
            return new AssetOrDirectReference<T>
            {
                address = tuple.assetRef,
                directRef = tuple.directDef
            };
        }

        public static void CallOnSuccess<T>(this AsyncOperationHandle<T> handle, Action<T> onSuccess)
        {
#if DEBUG
            StackTrace stackTrace = new StackTrace();
#endif

            handle.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Failed)
                {
                    Log.Error($"Failed to load asset '{handle.LocationName}'"
#if DEBUG
                        + $". at {stackTrace}"
#endif
                        );

                    return;
                }

                onSuccess(handle.Result);
            };
        }
    }
}
