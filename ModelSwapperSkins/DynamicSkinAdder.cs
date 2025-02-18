﻿using HG;
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
            if (SkinCatalog.skinCount > 0)
            {
                Log.Error("SkinCatalog already initialized");
            }

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

                // Prevent non-survivors from getting skins
                // This check is removed in debug builds to help with alignment
                // Maybe enable in release builds if it's verified to be safe/compatible with other mods?
#if !DEBUG
                if (SurvivorCatalog.GetSurvivorIndexFromBodyIndex(body.bodyIndex) == SurvivorIndex.None)
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
                        skin.Bake();
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
                    SkinDef[][] skins = BodyCatalog.skins;
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

            Log.Debug($"{bodyPrefab.name} model parts: [{string.Join(", ", modelParts.Select(p => p.Path))}]");

            if (modelTransform.TryGetComponent(out ModelSkinController modelSkinController))
            {
                foreach (SkinDef skin in modelSkinController.skins)
                {
                    if (skin.rootObject != modelTransform.gameObject)
                    {
                        Log.Warning($"Incorrect skin root object for {skin.name} on {bodyPrefab.name}");
                        continue;
                    }

                    List<SkinDef.GameObjectActivation> gameObjectActivations = new List<SkinDef.GameObjectActivation>(skin.gameObjectActivations ?? []);

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
                                if (partTransform.gameObject.activeSelf)
                                {
                                    Log.Debug($"Appending model part {modelPart.Path} object activation to regular skin {skin.name}");
                                }
                                else
                                {
                                    Log.Debug($"Appending default disabled model part {modelPart.Path} object activation to regular skin {skin.name}");
                                }

                                gameObjectActivations.Add(new SkinDef.GameObjectActivation
                                {
                                    gameObject = partTransform.gameObject,
                                    shouldActivate = partTransform.gameObject.activeSelf
                                });

                                gameObjectActivationsChanged = true;
                            }
                        }
                    }

                    if (gameObjectActivationsChanged)
                    {
                        skin.gameObjectActivations = gameObjectActivations.ToArray();

                        // Force re-baking
                        skin.runtimeSkin = null;
                    }
                }
            }
            else
            {
                _skinBakeDisabled = true;
                SkinDef defaultSkin = ScriptableObject.CreateInstance<SkinDef>();
                _skinBakeDisabled = false;

                defaultSkin.name = $"skin{bodyPrefab.name}Default";

                defaultSkin.rootObject = modelTransform.gameObject;

                defaultSkin.baseSkins = [];

                List<CharacterModel.RendererInfo> rendererInfos = new List<CharacterModel.RendererInfo>(characterModel.baseRendererInfos ?? []);

                for (int i = rendererInfos.Count - 1; i >= 0; i--)
                {
                    Renderer renderer = rendererInfos[i].renderer;
                    if (!renderer || !renderer.transform.IsChildOf(defaultSkin.rootObject.transform))
                    {
                        rendererInfos.RemoveAt(i);
                    }
                }

                foreach (ModelPart part in modelParts)
                {
                    if (part.RendererInfo.HasValue && part.Transform.TryGetComponent(out Renderer renderer))
                    {
                        Log.Debug($"Adding model part {part.Path} renderer ({renderer}) to generated default skin {defaultSkin.name}");

                        Material[] materials = renderer.sharedMaterials;
                        if (materials.Length <= 0)
                            continue;

                        ModelPartRendererInfo partRendererInfo = part.RendererInfo.Value;
                        CharacterModel.RendererInfo newRendererInfo = new CharacterModel.RendererInfo
                        {
                            renderer = renderer,
                            defaultMaterial = materials[0],
                            defaultShadowCastingMode = renderer.shadowCastingMode,
                            ignoreOverlays = partRendererInfo.IgnoreOverlays,
                            hideOnDeath = partRendererInfo.HideOnDeath
                        };

                        int existingRendererIndex = rendererInfos.FindIndex(r => r.renderer == renderer);
                        if (existingRendererIndex < 0)
                        {
                            rendererInfos.Add(newRendererInfo);
                        }
                        else
                        {
                            rendererInfos[existingRendererIndex] = newRendererInfo;
                        }

                        if (materials.Length > 1)
                        {
                            Material[] additionalMaterials = new Material[materials.Length - 1];
                            Array.Copy(materials, 1, additionalMaterials, 0, additionalMaterials.Length);

                            AdditionalRendererInfoProvider.AddMaterials(renderer, additionalMaterials);
                        }
                    }
                }

                defaultSkin.rendererInfos = rendererInfos.ToArray();

                defaultSkin.gameObjectActivations = Array.ConvertAll(modelParts, p => new SkinDef.GameObjectActivation
                {
                    gameObject = p.Transform.gameObject,
                    shouldActivate = p.Transform.gameObject.activeSelf
                });

                defaultSkin.meshReplacements = [];
                defaultSkin.projectileGhostReplacements = [];
                defaultSkin.minionSkinReplacements = [];

                modelSkinController = modelTransform.gameObject.AddComponent<ModelSkinController>();
                modelSkinController.skins = [defaultSkin];

                BodyIndex bodyIndex = bodyPrefab.bodyIndex;
                if (bodyIndex != BodyIndex.None)
                {
                    SkinDef[][] skins = BodyCatalog.skins;
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
