using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelInfos;
using ModelSwapperSkins.ModelParts;
using ModelSwapperSkins.Utils.Comparers;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins
{
    public class ModelSwappedSkinDef : SkinDef
    {
        public CharacterBody NewModelBodyPrefab;
        public Transform NewModelTransformPrefab;
        public SkinDef ModelSkin;

        public void Initialize(ModelPartsProvider modelPartsProvider, ModelPartsProvider skinModelPartsProvider)
        {
            gameObjectActivations = modelPartsProvider.Parts.Select(m =>
            {
                return new GameObjectActivation
                {
                    gameObject = m.Transform.gameObject,
                    shouldActivate = m.ShouldShow(true)
                };
            }).ToArray();

            if (baseSkins == null || baseSkins.Length == 0)
            {
                rendererInfos = modelPartsProvider.GetComponent<CharacterModel>().baseRendererInfos;
            }
            else
            {
                HashSet<MinionSkinReplacement> combinedMinionSkinReplacements = new HashSet<MinionSkinReplacement>(MinionSkinReplacementBodyComparer.Instance);
                HashSet<ProjectileGhostReplacement> combinedProjectileGhostReplacements = new HashSet<ProjectileGhostReplacement>(ProjectileGhostReplacementProjectileComparer.Instance);

                foreach (SkinDef baseSkin in baseSkins)
                {
                    if (!baseSkin)
                        continue;

                    if (baseSkin.minionSkinReplacements != null && baseSkin.minionSkinReplacements.Length > 0)
                    {
                        combinedMinionSkinReplacements.UnionWith(baseSkin.minionSkinReplacements);
                    }

                    if (baseSkin.projectileGhostReplacements != null && baseSkin.projectileGhostReplacements.Length > 0)
                    {
                        combinedProjectileGhostReplacements.UnionWith(baseSkin.projectileGhostReplacements);
                    }
                }

                minionSkinReplacements = combinedMinionSkinReplacements.ToArray();
                projectileGhostReplacements = combinedProjectileGhostReplacements.ToArray();
            }
        }

        public void RemoveFrom(Transform modelTransform, GameObject skinModelObject)
        {
            if (modelTransform.TryGetComponent(out CharacterModel characterModel))
            {
                List<CharacterModel.LightInfo> lights = characterModel.baseLightInfos.ToList();

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
                    characterModel.baseLightInfos = lights.ToArray();
                }
            }
        }

        public Transform InstantiateModel(Transform modelTransform)
        {
            Transform skinModelTransfom = GameObject.Instantiate(NewModelTransformPrefab, modelTransform);

            if (ModelSkin)
            {
                ModelSkin.Apply(skinModelTransfom.gameObject);
            }

            foreach (HurtBox hurtBox in skinModelTransfom.GetComponentsInChildren<HurtBox>(true))
            {
                hurtBox.enabled = false;
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
                else
                {
                    animator.enabled = false;
                }
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
                ak.enabled = false;
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

#if DEBUG
            Log.Debug($"Skin model {skinModelTransfom.name} scale for {modelTransform.name}: {skinScale}");
#endif

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

                foreach (DynamicBone dynamicBone in skinModelTransfom.GetComponentsInChildren<DynamicBone>())
                {
                    dynamicBone.enabled = false;

                    static IEnumerator waitThenSetEnabled(Behaviour behaviour)
                    {
                        yield return new WaitForEndOfFrame();

                        if (behaviour)
                        {
                            behaviour.enabled = true;
                        }
                    }

                    if (!isVulture)
                    {
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

#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                    mainModel.mainSkinnedMeshRenderer = skinModel.mainSkinnedMeshRenderer;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
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

                mainModel.baseRendererInfos = [..mainModel.baseRendererInfos, ..skinModelRendererInfos];
                mainModel.baseLightInfos = [..mainModel.baseLightInfos, ..skinModelLightInfos];

                if (skinModel)
                {
                    skinModel.enabled = false;
                }
            }

            return skinModelTransfom;
        }
    }
}
