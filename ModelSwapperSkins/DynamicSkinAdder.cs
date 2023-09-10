using HG;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins
{
    public static class DynamicSkinAdder
    {
        [SystemInitializer(typeof(BodyCatalog), typeof(SurvivorCatalog))]
        static void Init()
        {
            foreach (SurvivorDef survivor in SurvivorCatalog.allSurvivorDefs)
            {
                if (!survivor)
                    continue;

                GameObject bodyPrefab = survivor.bodyPrefab;
                if (!bodyPrefab)
                    continue;

                ModelLocator modelLocator = bodyPrefab.GetComponent<ModelLocator>();
                if (!modelLocator)
                    continue;

                Transform modelTransform = modelLocator.modelTransform;
                if (!modelTransform)
                    continue;

                ModelSkinController modelSkinController = modelTransform.GetComponent<ModelSkinController>();
                if (!modelSkinController)
                {
                    Log.Warning($"Survivor {survivor.cachedName} body prefab ({bodyPrefab}) model is missing ModelSkinController");
                    continue;
                }

                void SkinDef_Bake(On.RoR2.SkinDef.orig_Bake orig, SkinDef self)
                {
                    if (self.baseSkins == null)
                        self.baseSkins = Array.Empty<SkinDef>();

                    if (!self.rootObject)
                        self.rootObject = modelTransform.gameObject;

                    orig(self);
                }

                On.RoR2.SkinDef.Bake += SkinDef_Bake;

                List<SkinDef> newSkins = new List<SkinDef>();

                foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
                {
                    if ((body.bodyFlags & CharacterBody.BodyFlags.Masterless) != 0)
                        continue;

                    if (body.gameObject == bodyPrefab)
                        continue;

                    SkinDef skinDef = ScriptableObject.CreateInstance<SkinDef>();

                    skinDef.nameToken = body.baseNameToken;

                    newSkins.Add(skinDef);
                }

                On.RoR2.SkinDef.Bake -= SkinDef_Bake;

                if (newSkins.Count > 0)
                {
                    void appendSkins(ref SkinDef[] skins)
                    {
                        int skinsLength = skins.Length;
                        Array.Resize(ref skins, skinsLength + newSkins.Count);
                        newSkins.CopyTo(skins, skinsLength);
                    }

                    appendSkins(ref modelSkinController.skins);

                    BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(bodyPrefab);
                    if (bodyIndex != BodyIndex.None)
                    {
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                        SkinDef[][] skins = BodyCatalog.skins;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
                        if ((int)bodyIndex < skins.Length)
                        {
                            appendSkins(ref skins[(int)bodyIndex]);
                        }
                    }

#if DEBUG
                    Log.Debug($"Added {newSkins.Count} skins to {survivor.cachedName}");
#endif
                }
            }
        }
    }
}
