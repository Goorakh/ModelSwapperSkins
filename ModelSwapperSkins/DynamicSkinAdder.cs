using HG;
using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelParts;
using ModelSwapperSkins.Utils;
using ModelSwapperSkins.Utils.Extensions;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins
{
    public static class DynamicSkinAdder
    {
#if DEBUG
        const bool AllowNonSurvivorSkinAdditions = false;
#else
        const bool AllowNonSurvivorSkinAdditions = false;
#endif

        public delegate void AddSkinDelegate(CharacterBody bodyPrefab, List<SkinDef> skins);
        public static event AddSkinDelegate AddSkins;

        [SystemInitializer(typeof(SurvivorCatalog), typeof(BodyCatalog), typeof(ModelPartsInitializer), typeof(BoneInitializer))]
        static void Init()
        {
            if (SkinCatalog.skinCount > 0)
            {
                Log.Error("SkinCatalog already initialized");
            }

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

                if (!AllowNonSurvivorSkinAdditions && SurvivorCatalog.GetSurvivorIndexFromBodyIndex(body.bodyIndex) == SurvivorIndex.None)
                    continue;

                addSkinsTo(body);
            }
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
                if (AllowNonSurvivorSkinAdditions)
                {
                    modelSkinController = modelTransform.gameObject.AddComponent<ModelSkinController>();
                    modelSkinController.skins = [];
                }
                else
                {
                    Log.Warning($"{body.name} model is missing ModelSkinController");
                    return;
                }
            }

            try
            {
                AddSkins?.Invoke(body, newSkins);
            }
            catch (Exception e)
            {
                Log.Error_NoCallerPrefix($"Failed to generate skins for {body.name}: {e}");
                return;
            }

            if (newSkins.Count > 0)
            {
                for (int i = 0; i < newSkins.Count; i++)
                {
                    if (!newSkins[i].rootObject)
                    {
                        newSkins[i].rootObject = modelTransform.gameObject;
                    }
                }

                ArrayUtil.Append(ref modelSkinController.skins, newSkins);

                SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(body.gameObject);
                if (survivorDef && survivorDef.displayPrefab)
                {
                    if (survivorDef.displayPrefab.TryGetComponent(out ModelSkinController displayPrefabSkinController))
                    {
                        ArrayUtil.Append(ref displayPrefabSkinController.skins, newSkins);
                    }
                }

                BodyIndex bodyIndex = body.bodyIndex;
                if (bodyIndex != BodyIndex.None)
                {
                    SkinDef[][] skins = SkinCatalog.skinsByBody;
                    if ((uint)bodyIndex < skins.Length)
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

                    AssetOrDirectReference<SkinDefParams> skinParamsReference = skin.GetSkinParams().ReferenceOrDirect();
                    SkinDefParams skinDefParams = skinParamsReference.WaitForCompletion();

                    List<SkinDefParams.GameObjectActivation> gameObjectActivations;
                    if (skinDefParams)
                    {
                        gameObjectActivations = [.. skinDefParams.gameObjectActivations];
                    }
                    else
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        gameObjectActivations = [.. skin.gameObjectActivations];
#pragma warning restore CS0618 // Type or member is obsolete
                    }

                    bool gameObjectActivationsChanged = false;

                    foreach (ModelPart modelPart in modelParts)
                    {
                        bool partAlreadyExists = false;
                        foreach (SkinDefParams.GameObjectActivation gameObjectActivation in gameObjectActivations)
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
                                Log.Debug($"Appending model part {modelPart.Path} object activation to regular skin {skin.name}, default enabled: {partTransform.gameObject.activeSelf}");

                                gameObjectActivations.Add(new SkinDefParams.GameObjectActivation
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
                        if (skinDefParams)
                        {
                            skinDefParams.gameObjectActivations = [.. gameObjectActivations];
                        }
                        else
                        {
#pragma warning disable CS0618 // Type or member is obsolete
                            skin.gameObjectActivations = [.. gameObjectActivations.Select(g => new SkinDef.GameObjectActivation { gameObject = g.gameObject, shouldActivate = g.shouldActivate })];
#pragma warning restore CS0618 // Type or member is obsolete
                        }

                        // Force re-baking
                        skin._runtimeSkin = null;
                    }
                }
            }
            else
            {
                modelSkinController = modelTransform.gameObject.AddComponent<ModelSkinController>();

                SkinDef defaultSkin = ScriptableObject.CreateInstance<SkinDef>();

                string name = $"skin{bodyPrefab.name}Default";
                defaultSkin.name = name;
                defaultSkin.nameToken = name.ToUpper();

                defaultSkin.rootObject = modelTransform.gameObject;

                defaultSkin.baseSkins = [];

                SkinDefParams defaultSkinParams = ScriptableObject.CreateInstance<SkinDefParams>();
                defaultSkinParams.name = $"{defaultSkin.name}_params";

                defaultSkin.skinDefParams = defaultSkinParams;
                defaultSkin.optimizedSkinDefParams = defaultSkinParams;

                List<CharacterModel.RendererInfo> rendererInfos = [.. characterModel.baseRendererInfos];

                for (int i = rendererInfos.Count - 1; i >= 0; i--)
                {
                    Renderer renderer = rendererInfos[i].renderer;
                    if (!renderer || !renderer.transform.IsChildOf(defaultSkin.rootObject.transform))
                    {
                        rendererInfos.RemoveAt(i);
                    }
                }

                List<SkinDefParams.MeshReplacement> meshReplacements = [];
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in modelTransform.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    if (skinnedMeshRenderer.sharedMesh)
                    {
                        meshReplacements.Add(new SkinDefParams.MeshReplacement
                        {
                            renderer = skinnedMeshRenderer,
                            mesh = skinnedMeshRenderer.sharedMesh
                        });
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

                defaultSkinParams.rendererInfos = [.. rendererInfos];

                defaultSkinParams.meshReplacements = [.. meshReplacements];

                defaultSkinParams.gameObjectActivations = Array.ConvertAll(modelParts, p => new SkinDefParams.GameObjectActivation
                {
                    gameObject = p.Transform.gameObject,
                    shouldActivate = p.Transform.gameObject.activeSelf
                });

                modelSkinController.skins = [defaultSkin];

                BodyIndex bodyIndex = bodyPrefab.bodyIndex;
                if (bodyIndex != BodyIndex.None)
                {
                    SkinDef[][] skins = SkinCatalog.skinsByBody;
                    if ((uint)bodyIndex < skins.Length)
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
                    ModelSkinController survivorDisplaySkinController = survivorDisplayCharacterModel.gameObject.EnsureComponent<ModelSkinController>();
                    survivorDisplaySkinController.skins = modelSkinController.skins;
                }
            }
        }
    }
}
