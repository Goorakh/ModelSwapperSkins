using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelInfos;
using ModelSwapperSkins.ModelParts;
using ModelSwapperSkins.Utils;
using ModelSwapperSkins.Utils.Comparers;
using ModelSwapperSkins.Utils.Extensions;
using RoR2;
using RoR2.ContentManagement;
using RoR2.EntityLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ModelSwapperSkins
{
    public sealed class ModelSwappedSkinDef : SkinDef
    {
        public CharacterBody NewModelBodyPrefab;
        public Transform NewModelTransformPrefab;
        public SkinDef ModelSkin;

        public bool KeepSkinModelAnimatorActive;

        public void Initialize(ModelPartsProvider modelPartsProvider, ModelPartsProvider skinModelPartsProvider)
        {
            HashSet<SkinDef> encounteredSkinInstances = [];
            List<SkinDef> baseSkinsApplyOrder = [];

            void addBaseSkinToApplyOrder(SkinDef skin)
            {
                if (!encounteredSkinInstances.Add(skin))
                    return;

                if (skin.baseSkins != null)
                {
                    foreach (SkinDef baseSkin in skin.baseSkins)
                    {
                        addBaseSkinToApplyOrder(baseSkin);
                    }
                }

                baseSkinsApplyOrder.Add(skin);
            }

            if (baseSkins != null)
            {
                foreach (SkinDef baseSkin in baseSkins)
                {
                    addBaseSkinToApplyOrder(baseSkin);
                }
            }

            AssetOrDirectReference<SkinDefParams> skinParamsRef = GetSkinParams().ReferenceOrDirect();
            SkinDefParams skinParams = skinParamsRef.WaitForCompletion();

            skinParams.gameObjectActivations = new SkinDefParams.GameObjectActivation[modelPartsProvider.Parts.Length];

            for (int i = 0; i < modelPartsProvider.Parts.Length; i++)
            {
                ModelPart modelPart = modelPartsProvider.Parts[i];

                GameObject partObject = modelPart.Transform.gameObject;
                bool shouldActivate = modelPart.ShouldShow(true);

                bool partFoundInBaseSkin = false;
                for (int j = baseSkinsApplyOrder.Count - 1; j >= 0; j--)
                {
                    SkinDef baseSkin = baseSkinsApplyOrder[j];

                    SkinDefParams.GameObjectActivation[] baseGameObjectActivations = [];

                    AssetOrDirectReference<SkinDefParams> baseSkinParamsRef = baseSkin.GetSkinParams().ReferenceOrDirect();
                    if (baseSkinParamsRef.directRef || (baseSkinParamsRef.address != null && baseSkinParamsRef.address.RuntimeKeyIsValid()))
                    {
                        SkinDefParams baseSkinParams = baseSkinParamsRef.WaitForCompletion();
                        baseGameObjectActivations = baseSkinParams.gameObjectActivations;
                    }
                    else
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        baseGameObjectActivations = [.. baseSkin.gameObjectActivations];
#pragma warning restore CS0618 // Type or member is obsolete
                    }

                    baseSkinParamsRef.Reset();

                    int baseObjectActivationIndex = Array.FindIndex(baseGameObjectActivations, a => a.gameObject == partObject);
                    if (baseObjectActivationIndex >= 0)
                    {
                        bool basePartActive = baseGameObjectActivations[baseObjectActivationIndex].shouldActivate;

                        shouldActivate &= basePartActive;
                        partFoundInBaseSkin = true;

                        break;
                    }
                }

                if (!partFoundInBaseSkin)
                {
                    if (shouldActivate && !partObject.activeSelf)
                    {
                        Log.Debug($"{name}: Disabling model part object activation {modelPart.Path}: unreferenced object disabled in prefab");
                        shouldActivate = false;
                    }
                }

                skinParams.gameObjectActivations[i] = new SkinDefParams.GameObjectActivation
                {
                    gameObject = partObject,
                    shouldActivate = shouldActivate
                };
            }

            if (baseSkins == null || baseSkins.Length == 0)
            {
                skinParams.rendererInfos = modelPartsProvider.GetComponent<CharacterModel>().baseRendererInfos;
            }
            else
            {
                HashSet<SkinDefParams.MinionSkinReplacement> combinedMinionSkinReplacements = new HashSet<SkinDefParams.MinionSkinReplacement>(MinionSkinReplacementBodyComparer.Instance);
                HashSet<SkinDefParams.ProjectileGhostReplacement> combinedProjectileGhostReplacements = new HashSet<SkinDefParams.ProjectileGhostReplacement>(ProjectileGhostReplacementProjectileComparer.Instance);

                foreach (SkinDef baseSkin in baseSkins)
                {
                    if (!baseSkin)
                        continue;

                    SkinDefParams.MinionSkinReplacement[] minionSkinReplacements = [];
                    SkinDefParams.ProjectileGhostReplacement[] projectileGhostReplacements = [];

                    AssetOrDirectReference<SkinDefParams> baseSkinParamsRef = baseSkin.GetSkinParams().ReferenceOrDirect();
                    SkinDefParams baseSkinParams = baseSkinParamsRef.WaitForCompletion();
                    if (baseSkinParams)
                    {
                        minionSkinReplacements = baseSkinParams.minionSkinReplacements;
                        projectileGhostReplacements = baseSkinParams.projectileGhostReplacements;
                    }
                    else
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        minionSkinReplacements = [.. baseSkin.minionSkinReplacements];
                        projectileGhostReplacements = [.. baseSkin.projectileGhostReplacements];
#pragma warning restore CS0618 // Type or member is obsolete
                    }

                    if (minionSkinReplacements.Length > 0)
                    {
                        combinedMinionSkinReplacements.UnionWith(minionSkinReplacements);
                    }

                    if (projectileGhostReplacements.Length > 0)
                    {
                        combinedProjectileGhostReplacements.UnionWith(projectileGhostReplacements);
                    }

                    baseSkinParamsRef.Reset();
                }

                skinParams.minionSkinReplacements = [.. combinedMinionSkinReplacements];
                skinParams.projectileGhostReplacements = [.. combinedProjectileGhostReplacements];
            }

            skinParamsRef.Reset();

            switch (BodyCatalog.GetBodyName(NewModelBodyPrefab.bodyIndex))
            {
                case "AcidLarvaBody":
                case "CaptainBody":
                case "EngiBody":
                case "GeepBody":
                case "GipBody":
                case "GupBody":
                case "HermitCrabBody":
                case "ImpBody":
                case "ImpBossBody":
                case "MiniMushroomBody":
                case "MiniVoidRaidCrabBodyBase":
                case "NullifierAllyBody":
                case "NullifierBody":
                case "ScavBody":
                case "ScavLunar1Body":
                case "ScavLunar2Body":
                case "ScavLunar3Body":
                case "ScavLunar4Body":
                case "TreebotBody":
                case "VoidInfestorBody":
                    KeepSkinModelAnimatorActive = true;
                    break;
            }
        }

        public void RemoveFrom(Transform modelTransform, GameObject skinModelObject)
        {
            if (modelTransform.TryGetComponent(out CharacterModel characterModel))
            {
                List<CharacterModel.LightInfo> lights = [.. characterModel.baseLightInfos];

                bool anyChange = false;
                for (int i = lights.Count - 1; i >= 0; i--)
                {
                    if (!lights[i].light || lights[i].light.transform.IsChildOf(skinModelObject.transform))
                    {
                        lights.RemoveAt(i);
                        anyChange = true;
                    }
                }

                if (anyChange)
                {
                    characterModel.baseLightInfos = [.. lights];
                }
            }
        }

        public IEnumerator InstantiateModelAsync(Transform modelTransform, List<AssetReferenceT<Material>> loadedMaterials, List<AssetReferenceT<Mesh>> loadedMeshes, AsyncReferenceHandleUnloadType unloadType)
        {
            Transform skinModelTransfom = GameObject.Instantiate(NewModelTransformPrefab, modelTransform);

            if (ModelSkin)
            {
                yield return ModelSkin.ApplyAsync(skinModelTransfom.gameObject, loadedMaterials, loadedMeshes, unloadType);
            }

            bool isInfestor = NewModelBodyPrefab.bodyIndex == BodyCatalog.FindBodyIndex("VoidInfestorBody");

            foreach (HurtBox hurtBox in skinModelTransfom.GetComponentsInChildren<HurtBox>(true))
            {
                if (isInfestor)
                {
                    foreach (StartEvent startEvent in hurtBox.GetComponents<StartEvent>())
                    {
                        startEvent.enabled = false;
                    }

                    foreach (DelayedEvent delayedEvent in hurtBox.GetComponents<DelayedEvent>())
                    {
                        delayedEvent.enabled = false;
                    }
                }

                hurtBox.enabled = false;
            }

            foreach (HitBox hitBox in skinModelTransfom.GetComponentsInChildren<HitBox>(true))
            {
                hitBox.enabled = false;
            }

            foreach (Collider collider in skinModelTransfom.GetComponentsInChildren<Collider>(true))
            {
                collider.enabled = false;
            }

            bool isToolbot = NewModelBodyPrefab.bodyIndex == BodyCatalog.FindBodyIndex("ToolbotBody");

            foreach (Animator animator in skinModelTransfom.GetComponentsInChildren<Animator>(true))
            {
                if (isToolbot && animator.TryGetComponent(out CharacterModel characterModel))
                {
                    IEnumerator waitUntilInitializedThenFixToolbotAnimator(Animator animator)
                    {
                        Transform toolbotMeshObj = animator.transform.Find("ToolbotMesh");
                        if (toolbotMeshObj && toolbotMeshObj.TryGetComponent(out Renderer toolbotMeshRenderer))
                        {
                            yield return new WaitForEndOfFrame();
                            yield return new WaitWhile(() => toolbotMeshRenderer && !toolbotMeshRenderer.enabled);

                            if (!animator)
                                yield break;
                        }

                        animator.speed = 0f;
                        animator.Update(0f);
                        animator.SetInteger("weaponStance", 0);
                        animator.Update(0f);
                        animator.PlayInFixedTime("NailgunOut", animator.GetLayerIndex("Stance, Additive"), 0f);
                        animator.Update(0f);

                        yield return new WaitForEndOfFrame();

                        if (animator)
                            animator.enabled = false;
                    }

                    characterModel.StartCoroutine(waitUntilInitializedThenFixToolbotAnimator(animator));
                }
                else if (!KeepSkinModelAnimatorActive)
                {
                    animator.enabled = false;
                }
            }

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinModelTransfom.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                // If this is false, some model meshes just disappear when looked at from certain angles
                // There's probably a better solution, but this also seems to work
                skinnedMeshRenderer.updateWhenOffscreen = true;
            }

            foreach (Behaviour skinModelComponent in skinModelTransfom.GetComponents<Behaviour>())
            {
                if (!skinModelComponent)
                    continue;

                if (skinModelComponent.GetType().FullName == "Generics.Dynamics.InverseKinematics")
                {
                    skinModelComponent.enabled = false;
                }
            }

            foreach (PrintController printController in skinModelTransfom.GetComponentsInChildren<PrintController>(true))
            {
                printController.enabled = false;
            }

            foreach (AkEvent ak in skinModelTransfom.GetComponentsInChildren<AkEvent>(true))
            {
                Destroy(ak);
            }

            foreach (StriderLegController striderLegController in skinModelTransfom.GetComponentsInChildren<StriderLegController>())
            {
                striderLegController.enabled = false;
            }

            foreach (Animator mainModelAnimator in modelTransform.GetComponents<Animator>())
            {
                // Not overriding this makes *some* characters not animate properly for some reason,
                // super inconsistent which ones are affected by this, but this seems to fix it
                mainModelAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }

            ModelInfo mainModelInfo = ModelInfoProvider.GetModelInfo(modelTransform.gameObject);
            ModelInfo skinModelInfo = ModelInfoProvider.GetModelInfo(skinModelTransfom.gameObject);

            float skinScale = mainModelInfo.HeightScale / skinModelInfo.HeightScale;
            skinModelTransfom.localScale = new Vector3(skinScale, skinScale, skinScale);

            Log.Debug($"Skin model {skinModelTransfom.name} scale for {modelTransform.name}: {skinScale}");

            if (modelTransform.TryGetComponent(out BonesProvider modelBonesProvider) && skinModelTransfom.TryGetComponent(out BonesProvider skinBonesProvider))
            {
                skinBonesProvider.MapBonesTo(modelBonesProvider);
            }

            if (skinModelTransfom.TryGetComponent(out ModelPartsProvider skinModelPartsProvider))
            {
                foreach (ModelPart part in skinModelPartsProvider.Parts)
                {
                    if (!part.ShouldShow(false))
                    {
                        part.Transform.gameObject.SetActive(false);
                    }
                }
            }

            if (modelTransform.TryGetComponent(out CharacterModel mainModel))
            {
                bool isVulture = NewModelBodyPrefab.bodyIndex == BodyCatalog.FindBodyIndex("VultureBody");
                bool isGraveKeeper = NewModelBodyPrefab.bodyIndex == BodyCatalog.FindBodyIndex("GravekeeperBody");
                bool isRoboBallBoss = NewModelBodyPrefab.bodyIndex == BodyCatalog.FindBodyIndex("RoboBallBossBody") ||
                                      NewModelBodyPrefab.bodyIndex == BodyCatalog.FindBodyIndex("SuperRoboBallBossBody");

                foreach (DynamicBone dynamicBone in skinModelTransfom.GetComponentsInChildren<DynamicBone>())
                {
                    dynamicBone.enabled = false;

                    bool shouldDisableBone = isVulture || isRoboBallBoss ||
                                            (isGraveKeeper && dynamicBone.m_Root && dynamicBone.m_Root.name == "head");

                    if (!shouldDisableBone)
                    {
                        static IEnumerator waitThenSetEnabled(Behaviour behaviour)
                        {
                            yield return new WaitForEndOfFrame();

                            if (behaviour)
                            {
                                behaviour.enabled = true;
                            }
                        }

                        mainModel.StartCoroutine(waitThenSetEnabled(dynamicBone));
                    }
                }

                List<CharacterModel.RendererInfo> skinModelRendererInfos = [];
                List<CharacterModel.LightInfo> skinModelLightInfos = [];

                CharacterModel skinModel = skinModelTransfom.GetComponent<CharacterModel>();

                if (skinModel && (skinModel.baseRendererInfos.Length > 0 || skinModel.baseLightInfos.Length > 0))
                {
                    CharacterModel.RendererInfo[] skinRendererInfos = skinModel.baseRendererInfos;

                    if (NewModelBodyPrefab.bodyIndex == BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyBase"))
                    {
                        for (int i = 0; i < skinRendererInfos.Length; i++)
                        {
                            ref CharacterModel.RendererInfo rendererInfo = ref skinRendererInfos[i];
                            if (!rendererInfo.renderer)
                                continue;

                            switch (rendererInfo.renderer.name)
                            {
                                case "EyePupilMesh":
                                case "VoidRaidCrabEye":
                                    rendererInfo.ignoreOverlays = true;
                                    break;
                                case "VoidRaidCrabHead":
                                case "VoidRaidCrabMetalLegRingsMesh":
                                case "VoidRaidCrabMetalMesh":
                                case "VoidRaidCrabBrain":
                                    rendererInfo.ignoreOverlays = false;
                                    break;
                            }
                        }
                    }

                    skinModelRendererInfos.AddRange(skinRendererInfos);

                    skinModelLightInfos.AddRange(skinModel.baseLightInfos);

                    mainModel.mainSkinnedMeshRenderer = skinModel.mainSkinnedMeshRenderer;
                }
                else
                {
                    skinModelRendererInfos.AddRange(skinModelTransfom.GetComponentsInChildren<Renderer>().Select(r =>
                    {
                        return new CharacterModel.RendererInfo
                        {
                            renderer = r,
                            defaultMaterial = r.sharedMaterial,
                            defaultShadowCastingMode = r.shadowCastingMode,
                            hideOnDeath = false,
                            ignoreOverlays = r is ParticleSystemRenderer
                        };
                    }));

                    skinModelLightInfos.AddRange(skinModelTransfom.GetComponentsInChildren<Light>().Select(l => new CharacterModel.LightInfo(l)));
                }

                List<UnityEngine.Object> objectsToCleanupOnModelDestroy = [];

                for (int i = 0; i < skinModelRendererInfos.Count; i++)
                {
                    CharacterModel.RendererInfo rendererInfo = skinModelRendererInfos[i];

                    bool changedEntry = false;
                    if (rendererInfo.defaultMaterial.IsKeywordEnabled(ShaderKeywords.PRINT_CUTOFF))
                    {
                        Material materialInstance = GameObject.Instantiate(rendererInfo.defaultMaterial);

                        materialInstance.DisableKeyword(ShaderKeywords.PRINT_CUTOFF);
                        materialInstance.SetInt(ShaderIDs._PrintOn, 0);

                        rendererInfo.defaultMaterial = materialInstance;

                        objectsToCleanupOnModelDestroy.Add(materialInstance);

                        changedEntry = true;
                    }

                    if (changedEntry)
                    {
                        skinModelRendererInfos[i] = rendererInfo;
                    }
                }

                if (objectsToCleanupOnModelDestroy.Count > 0)
                {
                    OnDestroyCallback.AddCallback(modelTransform.gameObject, _ =>
                    {
                        foreach (UnityEngine.Object obj in objectsToCleanupOnModelDestroy)
                        {
                            if (obj)
                            {
                                GameObject.Destroy(obj);
                            }
                        }
                    });
                }

                mainModel.baseRendererInfos = [..mainModel.baseRendererInfos, ..skinModelRendererInfos];
                mainModel.baseLightInfos = [..mainModel.baseLightInfos, ..skinModelLightInfos];

                if (skinModel)
                {
                    skinModel.enabled = false;
                }
            }

            if (modelTransform.TryGetComponent(out ModelSwappedSkinController modelSwappedSkinReference))
            {
                modelSwappedSkinReference.SkinModelObject = skinModelTransfom.gameObject;
            }
        }
    }
}
