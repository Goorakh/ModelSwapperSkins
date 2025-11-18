using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ModelSwapperSkins.Patches
{
    // Fixes nullrefs if non-existing paths are put into gameObjectActivationTemplates or meshReplacementTemplates.
    // rendererInfoTemplates is not included in this patch since they are already null-checked properly
    static class FixMissingSkinPathsPatch
    {
        [SystemInitializer]
        static void Init()
        {
            On.RoR2.SkinDef.RuntimeSkin.ApplyAsync_GameObject_List1_List1_List1_AsyncReferenceHandleUnloadType += RuntimeSkin_ApplyAsync_GameObject_List1_List1_List1_AsyncReferenceHandleUnloadType;
        }

        static IEnumerator RuntimeSkin_ApplyAsync_GameObject_List1_List1_List1_AsyncReferenceHandleUnloadType(On.RoR2.SkinDef.RuntimeSkin.orig_ApplyAsync_GameObject_List1_List1_List1_AsyncReferenceHandleUnloadType orig, SkinDef.RuntimeSkin self, GameObject modelObject, List<AssetReferenceT<Material>> loadedMaterials, List<AssetReferenceT<Mesh>> loadedMeshes, List<AssetReferenceT<GameObject>> loadedGameObjects, AsyncReferenceHandleUnloadType unloadType)
        {
            if (self is SkinDef.RuntimeSkin skin)
            {
                Transform modelTransform = modelObject.transform;

                if (skin.gameObjectActivationTemplates.src != null && skin.gameObjectActivationTemplates.Length > 0)
                {
                    List<SkinDef.GameObjectActivationTemplate> gameObjectActivationTemplates = [.. skin.gameObjectActivationTemplates];
                    bool activationTemplatesModified = false;
                    for (int i = gameObjectActivationTemplates.Count - 1; i >= 0; i--)
                    {
                        if (!modelTransform.Find(gameObjectActivationTemplates[i].transformPath))
                        {
                            Log.Debug($"Removing invalid gameObjectActivationTemplates path \"{skin.gameObjectActivationTemplates[i].transformPath}\"");
                            gameObjectActivationTemplates.RemoveAt(i);
                            activationTemplatesModified = true;
                        }
                    }

                    if (activationTemplatesModified)
                    {
                        skin.gameObjectActivationTemplates = gameObjectActivationTemplates.ToArray();
                    }
                }

                if (skin.meshReplacementTemplates.src != null && skin.meshReplacementTemplates.Length > 0)
                {
                    List<SkinDef.MeshReplacementTemplate> meshReplacementTemplates = [.. skin.meshReplacementTemplates];
                    bool meshReplacementsModified = false;

                    for (int i = meshReplacementTemplates.Count - 1; i >= 0; i--)
                    {
                        if (!modelTransform.Find(meshReplacementTemplates[i].transformPath))
                        {
                            Log.Debug($"Removing invalid meshReplacementTemplates path \"{skin.meshReplacementTemplates[i].transformPath}\"");
                            meshReplacementTemplates.RemoveAt(i);
                            meshReplacementsModified = true;
                        }
                    }

                    if (meshReplacementsModified)
                    {
                        skin.meshReplacementTemplates = meshReplacementTemplates.ToArray();
                    }
                }
            }

            return orig(self, modelObject, loadedMaterials, loadedMeshes, loadedGameObjects, unloadType);
        }
    }
}
