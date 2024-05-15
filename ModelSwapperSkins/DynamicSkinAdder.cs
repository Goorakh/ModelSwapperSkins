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
        public delegate void AddSkinDelegate(CharacterBody bodyPrefab, List<SkinDef> skins);
        public static event AddSkinDelegate AddSkins;

        static bool _skinBakeDisabled = false;

        [SystemInitializer(typeof(SurvivorCatalog), typeof(BodyCatalog), typeof(ModelPartsInitializer), typeof(BoneInitializer))]
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

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
#if !DEBUG
                if (!SurvivorCatalog.FindSurvivorDefFromBody(body.gameObject))
                    continue;
#endif

                addSkinsTo(body);
            }

            On.RoR2.SkinDef.Bake -= SkinDef_Bake;
        }

        static void addSkinsTo(CharacterBody body)
        {
            if (!body)
                return;

            ModelLocator modelLocator = body.GetComponent<ModelLocator>();
            if (!modelLocator)
                return;

            Transform modelTransform = modelLocator.modelTransform;
            if (!modelTransform)
                return;

            List<SkinDef> newSkins = [];

            ModelSkinController modelSkinController = modelTransform.GetComponent<ModelSkinController>();
            if (!modelSkinController)
            {
#if DEBUG
                modelSkinController = modelTransform.gameObject.AddComponent<ModelSkinController>();
                modelSkinController.skins = [];
#else
                Log.Warning($"{body.name} model is missing ModelSkinController");
                return;
#endif
            }

            _skinBakeDisabled = true;
            AddSkins?.Invoke(body, newSkins);
            _skinBakeDisabled = false;

            if (newSkins.Count > 0)
            {
                for (int i = newSkins.Count - 1; i >= 0; i--)
                {
                    SkinDef skin = newSkins[i];

                    skin.baseSkins ??= [];
                    skin.rendererInfos ??= [];
                    skin.gameObjectActivations ??= [];
                    skin.meshReplacements ??= [];
                    skin.projectileGhostReplacements ??= [];
                    skin.minionSkinReplacements ??= [];

                    if (!skin.rootObject)
                        skin.rootObject = modelTransform.gameObject;

                    try
                    {
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                        skin.Bake();
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
                    }
                    catch (Exception e)
                    {
                        Log.Warning_NoCallerPrefix($"Failed to create skin {skin.name} for {body.name}: {e}");

                        GameObject.Destroy(newSkins[i]);
                        newSkins.RemoveAt(i);
                    }
                }

                ArrayUtil.Append(ref modelSkinController.skins, newSkins);

                BodyIndex bodyIndex = body.bodyIndex;
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

            Log.Info_NoCallerPrefix($"Created {newSkins.Count} skin(s) for {body.name}");
        }
    }
}
