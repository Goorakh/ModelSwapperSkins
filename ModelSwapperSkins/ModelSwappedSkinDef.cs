using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelInfos;
using ModelSwapperSkins.ModelParts;
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
            gameObjectActivations = modelPartsProvider.Parts.Where(m => !m.ShouldShow(true)).Select(m =>
            {
                return new GameObjectActivation
                {
                    gameObject = m.Transform.gameObject,
                    shouldActivate = false
                };
            }).ToArray();

            rendererInfos = modelPartsProvider.GetComponent<CharacterModel>().baseRendererInfos;
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

        public Transform OnAppliedTo(Transform modelTransform)
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

                    mainModel.StartCoroutine(waitThenSetEnabled(dynamicBone));
                }

                ModelPartsProvider mainModelPartsProvider = modelTransform.GetComponent<ModelPartsProvider>();
                bool shouldShow(Component component, bool isMainModel)
                {
                    if (!mainModelPartsProvider)
                        return true;

                    if (!component)
                        return false;

                    return mainModelPartsProvider.Parts.Where(p => component.transform == p.Transform || component.transform.IsChildOf(p.Transform))
                                                       .All(p => p.ShouldShow(isMainModel));
                }

                IEnumerable<CharacterModel.RendererInfo> rendererInfos = mainModel.baseRendererInfos.Where(r => shouldShow(r.renderer, true));

                IEnumerable<CharacterModel.LightInfo> lightInfos = mainModel.baseLightInfos.Where(l => shouldShow(l.light, true));

                if (skinModelTransfom.TryGetComponent(out CharacterModel skinModel) && (skinModel.baseRendererInfos.Length > 0 || skinModel.baseLightInfos.Length > 0))
                {
                    IEnumerable<CharacterModel.RendererInfo> skinRendererInfos = skinModel.baseRendererInfos;

                    if (NewModelBodyPrefab.bodyIndex == BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyBase"))
                    {
                        skinRendererInfos = skinRendererInfos.Select(r =>
                        {
                            if (r.renderer)
                            {
                                switch (r.renderer.name)
                                {
                                    case "EyePupilMesh":
                                    case "VoidRaidCrabEye":
                                        r.ignoreOverlays = true;
                                        break;
                                    case "VoidRaidCrabHead":
                                    case "VoidRaidCrabMetalLegRingsMesh":
                                    case "VoidRaidCrabMetalMesh":
                                    case "VoidRaidCrabBrain":
                                        r.ignoreOverlays = false;
                                        break;
                                }
                            }

                            return r;
                        });
                    }

                    rendererInfos = rendererInfos.Concat(skinRendererInfos);

                    lightInfos = lightInfos.Concat(skinModel.baseLightInfos);

#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                    mainModel.mainSkinnedMeshRenderer = skinModel.mainSkinnedMeshRenderer;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

                    skinModel.enabled = false;
                }
                else
                {
                    rendererInfos = rendererInfos.Concat(from renderer in skinModelTransfom.GetComponentsInChildren<Renderer>()
                                                         select new CharacterModel.RendererInfo
                                                         {
                                                             renderer = renderer,
                                                             defaultMaterial = renderer.sharedMaterial,
                                                             defaultShadowCastingMode = renderer.shadowCastingMode,
                                                             hideOnDeath = false,
                                                             ignoreOverlays = renderer is ParticleSystemRenderer
                                                         });

                    lightInfos = lightInfos.Concat(from light in skinModelTransfom.GetComponentsInChildren<Light>()
                                                   select new CharacterModel.LightInfo(light));
                }

                mainModel.baseRendererInfos = rendererInfos.ToArray();
                mainModel.baseLightInfos = lightInfos.ToArray();
            }

            return skinModelTransfom;
        }
    }
}
