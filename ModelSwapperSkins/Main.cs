using BepInEx;
using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelInfo;
using ModelSwapperSkins.Utils.Extensions;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Gorakh";
        public const string PluginName = "ModelSwapperSkins";
        public const string PluginVersion = "1.0.0";

        void Awake()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Log.Init(Logger);

            SystemInitializerInjector.InjectDependency(typeof(SkinCatalog), typeof(DynamicSkinAdder));

            DynamicSkinAdder.AddSkins += DynamicSkinAdder_AddSkins;

            stopwatch.Stop();
            Log.Info_NoCallerPrefix($"Initialized in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        }

        static void DynamicSkinAdder_AddSkins(SurvivorDef survivor, List<SkinDef> skins)
        {
            if (!survivor)
                return;

            HashSet<Transform> usedModels = new HashSet<Transform>();

            ModelPartsProvider survivorPartsProvider = null;
            BonesProvider survivorBonesProvider = null;

            if (survivor.bodyPrefab && survivor.bodyPrefab.TryGetComponent(out ModelLocator survivorModelLocator))
            {
                Transform survivorModelTransform = survivorModelLocator.modelTransform;
                if (survivorModelTransform)
                {
                    survivorPartsProvider = survivorModelTransform.GetComponent<ModelPartsProvider>();
                    survivorBonesProvider = survivorModelTransform.GetComponent<BonesProvider>();
                }
            }

            if (!survivorPartsProvider || !survivorBonesProvider)
                return;

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                if (!body)
                    continue;

                if (body.gameObject == survivor.bodyPrefab)
                    continue;

                if (string.IsNullOrWhiteSpace(body.baseNameToken) || Language.GetString(body.baseNameToken) == body.baseNameToken)
                    continue;

                ModelLocator modelLocator = body.GetComponent<ModelLocator>();
                if (!modelLocator)
                    continue;

                Transform modelTransform = modelLocator.modelTransform;
                if (!modelTransform || modelTransform.childCount == 0)
                    continue;

                ModelPartsProvider bodyModelPartsProvider = modelTransform.GetComponent<ModelPartsProvider>();
                if (!bodyModelPartsProvider)
                    continue;

                BonesProvider bodyBonesProvider = modelTransform.GetComponent<BonesProvider>();
                if (!bodyBonesProvider)
                    continue;

                if (survivorBonesProvider.GetNumMatchingBones(bodyBonesProvider) < 5)
                {
#if DEBUG
                    Log.Debug($"Not creating {body.name} skin for {survivor.cachedName}: Not enough common bones");
#endif
                    continue;
                }

                Sprite skinSprite = null;
                if (body.portraitIcon)
                {
                    // if (body.portraitIcon.name == "texMysteryIcon" || body.portraitIcon.name == "texNullIcon")
                    //     continue;
                    
                    if (body.portraitIcon is Texture2D iconTexture)
                    {
                        skinSprite = Sprite.Create(iconTexture, new Rect(0f, 0f, iconTexture.width, iconTexture.height), Vector2.one / 2f);
                    }
                }

                if (!usedModels.Add(modelTransform))
                    continue;

                ModelSwappedSkinDef skinDef = ScriptableObject.CreateInstance<ModelSwappedSkinDef>();

                skinDef.name = $"skin{survivor.cachedName}_{body.name}";
                skinDef.nameToken = body.baseNameToken;

                skinDef.icon = skinSprite;

                skinDef.Initialize(survivorPartsProvider, bodyModelPartsProvider);

                skinDef.NewModelBodyPrefab = body;
                skinDef.NewModelTransformPrefab = modelTransform;

                skins.Add(skinDef);
            }
        }
    }
}
