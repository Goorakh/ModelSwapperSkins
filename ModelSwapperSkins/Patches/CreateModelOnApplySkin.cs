using HG;
using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ModelSwapperSkins.Patches
{
    static class CreateModelOnApplySkin
    {
        [SystemInitializer]
        static void Init()
        {
            On.RoR2.SkinDef.ApplyAsync += SkinDef_ApplyAsync;
        }

        static IEnumerator SkinDef_ApplyAsync(On.RoR2.SkinDef.orig_ApplyAsync orig, SkinDef self, GameObject modelObject, List<AssetReferenceT<Material>> loadedMaterials, List<AssetReferenceT<Mesh>> loadedMeshes, AsyncReferenceHandleUnloadType unloadType)
        {
            if (modelObject.TryGetComponent(out ModelSwappedSkinReference existingModelObjectTracker))
            {
                if (existingModelObjectTracker.AppliedSkin && existingModelObjectTracker.SkinModelObject)
                {
                    existingModelObjectTracker.AppliedSkin.RemoveFrom(modelObject.transform, existingModelObjectTracker.SkinModelObject);
                }

                Object.Destroy(existingModelObjectTracker);
            }

            yield return orig(self, modelObject, loadedMaterials, loadedMeshes, unloadType);

            if (self is ModelSwappedSkinDef modelSwappedSkin)
            {
                ModelSwappedSkinReference modelObjectTracker = modelObject.EnsureComponent<ModelSwappedSkinReference>();
                modelObjectTracker.AppliedSkin = modelSwappedSkin;

                yield return modelSwappedSkin.InstantiateModelAsync(modelObject.transform, loadedMaterials, loadedMeshes, unloadType);
            }
        }
    }
}