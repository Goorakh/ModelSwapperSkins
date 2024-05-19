using HG;
using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelParts;
using ModelSwapperSkins.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
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

            HashSet<CharacterBody> failedSetupBodies = [];

            foreach (CharacterBody bodyPrefab in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                try
                {
                    addDefaultSkinIfMissing(bodyPrefab);
                }
                catch (Exception e)
                {
                    Log.Error_NoCallerPrefix($"Failed to generate default skin for {bodyPrefab.name}: {e}");
                    failedSetupBodies.Add(bodyPrefab);
                }
            }

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                if (failedSetupBodies.Contains(body))
                    continue;

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

            try
            {
                AddSkins?.Invoke(body, newSkins);
            }
            catch (Exception e)
            {
                Log.Error_NoCallerPrefix($"Failed to generate skins for {body.name}: {e}");
                return;
            }
            finally
            {
                _skinBakeDisabled = false;
            }

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

        static void addDefaultSkinIfMissing(CharacterBody bodyPrefab)
        {
            SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab.gameObject);

            ModelLocator modelLocator = bodyPrefab.GetComponent<ModelLocator>();
            if (!modelLocator)
                return;

            Transform modelTransform = modelLocator.modelTransform;
            if (!modelTransform)
                return;

            CharacterModel characterModel = modelTransform.GetComponent<CharacterModel>();
            if (!characterModel)
                return;

            ModelPart[] modelParts;
            if (modelTransform.TryGetComponent(out ModelPartsProvider modelPartsProvider))
            {
                modelParts = modelPartsProvider.Parts;
            }
            else
            {
                modelParts = [];
            }

#if DEBUG
            Log.Debug($"{bodyPrefab.name} model parts: [{string.Join(", ", modelParts.Select(p => p.Path))}]");
#endif

            if (modelTransform.TryGetComponent(out ModelSkinController modelSkinController))
            {
                foreach (SkinDef skin in modelSkinController.skins)
                {
                    if (skin.rootObject != modelTransform.gameObject)
                    {
                        Log.Warning($"Incorrect skin root object for {skin.name} on {bodyPrefab.name}");
                        continue;
                    }

                    List<SkinDef.GameObjectActivation> gameObjectActivations = new List<SkinDef.GameObjectActivation>(skin.gameObjectActivations);

                    bool gameObjectActivationsChanged = false;

                    foreach (ModelPart modelPart in modelParts)
                    {
                        bool partAlreadyExists = false;
                        foreach (SkinDef.GameObjectActivation gameObjectActivation in gameObjectActivations)
                        {
                            string activationPath = Util.BuildPrefabTransformPath(skin.rootObject.transform, gameObjectActivation.gameObject.transform);
                            if (activationPath == modelPart.Path)
                            {
                                partAlreadyExists = true;
                                break;
                            }
                        }

                        if (!partAlreadyExists)
                        {
                            Transform partTransform = skin.rootObject.transform.Find(modelPart.Path);
                            if (partTransform)
                            {
#if DEBUG
                                Log.Debug($"Appending model part {modelPart.Path} object activation to regular skin {skin.name}");
#endif

                                gameObjectActivations.Add(new SkinDef.GameObjectActivation
                                {
                                    gameObject = partTransform.gameObject,
                                    shouldActivate = true
                                });

                                gameObjectActivationsChanged = true;
                            }
                        }
                    }

                    if (gameObjectActivationsChanged)
                    {
                        skin.gameObjectActivations = gameObjectActivations.ToArray();

                        // Force re-baking
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                        skin.runtimeSkin = null;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
                    }
                }
            }
            else
            {
                modelSkinController = modelTransform.gameObject.AddComponent<ModelSkinController>();

                _skinBakeDisabled = true;
                SkinDef defaultSkin = ScriptableObject.CreateInstance<SkinDef>();
                _skinBakeDisabled = false;

                defaultSkin.name = $"skin{bodyPrefab.name}Default";

                defaultSkin.rootObject = modelTransform.gameObject;

                defaultSkin.baseSkins = [];

                List<CharacterModel.RendererInfo> rendererInfos = new List<CharacterModel.RendererInfo>(characterModel.baseRendererInfos);

                for (int i = rendererInfos.Count - 1; i >= 0; i--)
                {
                    Renderer renderer = rendererInfos[i].renderer;
                    if (!renderer || !renderer.transform.IsChildOf(defaultSkin.rootObject.transform))
                    {
                        rendererInfos.RemoveAt(i);
                    }
                }

                defaultSkin.rendererInfos = rendererInfos.ToArray();

                defaultSkin.gameObjectActivations = Array.ConvertAll(modelParts, p => new SkinDef.GameObjectActivation
                {
                    gameObject = p.Transform.gameObject,
                    shouldActivate = true
                });

                defaultSkin.meshReplacements = [];
                defaultSkin.projectileGhostReplacements = [];
                defaultSkin.minionSkinReplacements = [];

                modelSkinController.skins = [defaultSkin];

                BodyIndex bodyIndex = bodyPrefab.bodyIndex;
                if (bodyIndex != BodyIndex.None)
                {
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                    SkinDef[][] skins = BodyCatalog.skins;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
                    if ((int)bodyIndex < skins.Length)
                    {
                        ArrayUtils.ArrayAppend(ref skins[(int)bodyIndex], defaultSkin);
                    }
                }
            }

            if (survivorDef && survivorDef.displayPrefab)
            {
                CharacterModel survivorDisplayCharacterModel = survivorDef.displayPrefab.GetComponentInChildren<CharacterModel>();
                if (survivorDisplayCharacterModel)
                {
                    if (!survivorDisplayCharacterModel.TryGetComponent(out ModelSkinController survivorDisplaySkinController))
                        survivorDisplaySkinController = survivorDisplayCharacterModel.gameObject.AddComponent<ModelSkinController>();

                    survivorDisplaySkinController.skins = modelSkinController.skins;
                }
            }
        }
    }
}
