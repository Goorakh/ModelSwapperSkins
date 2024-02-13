using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelParts;
using ModelSwapperSkins.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins
{
    public static class DynamicSkinAdder
    {
        public delegate void AddSkinDelegate(SurvivorDef survivor, List<SkinDef> skins);
        public static event AddSkinDelegate AddSkins;

        static bool _skinBakeDisabled = false;

        [SystemInitializer(typeof(SurvivorCatalog), typeof(ModelPartsInitializer), typeof(BoneInitializer))]
        static void Init()
        {
            // Bake is called from Awake, before we've had a chance to set all the fields, it will be called manually later instead
            void SkinDef_Bake(On.RoR2.SkinDef.orig_Bake orig, SkinDef self)
            {
                if (_skinBakeDisabled)
                    return;

                orig(self);
            }

            On.RoR2.SkinDef.Bake += SkinDef_Bake;

            foreach (SurvivorDef survivor in SurvivorCatalog.allSurvivorDefs)
            {
                addSkinsTo(survivor);
            }

            On.RoR2.SkinDef.Bake -= SkinDef_Bake;
        }

        static void addSkinsTo(SurvivorDef survivor)
        {
            if (!survivor)
                return;

            GameObject bodyPrefab = survivor.bodyPrefab;
            if (!bodyPrefab)
                return;

            ModelLocator modelLocator = bodyPrefab.GetComponent<ModelLocator>();
            if (!modelLocator)
                return;

            Transform modelTransform = modelLocator.modelTransform;
            if (!modelTransform)
                return;

            ModelSkinController modelSkinController = modelTransform.GetComponent<ModelSkinController>();
            if (!modelSkinController)
            {
                Log.Warning($"Survivor {survivor.cachedName} body prefab ({bodyPrefab}) model is missing ModelSkinController");
                return;
            }

            List<SkinDef> newSkins = [];

            _skinBakeDisabled = true;
            AddSkins?.Invoke(survivor, newSkins);
            _skinBakeDisabled = false;

            if (newSkins.Count > 0)
            {
                foreach (SkinDef skin in newSkins)
                {
                    skin.baseSkins ??= [];

                    if (!skin.rootObject)
                        skin.rootObject = modelTransform.gameObject;

#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                    skin.Bake();
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
                }

                ArrayUtil.Append(ref modelSkinController.skins, newSkins);

                BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(bodyPrefab);
                if (bodyIndex != BodyIndex.None)
                {
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                    SkinDef[][] skins = BodyCatalog.skins;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
                    if ((int)bodyIndex < skins.Length)
                    {
                        ArrayUtil.Append(ref skins[(int)bodyIndex], newSkins);
                    }
                }
            }

            Log.Info_NoCallerPrefix($"Created {newSkins.Count} skin(s) for {survivor.cachedName}");
        }
    }
}
