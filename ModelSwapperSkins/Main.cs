using BepInEx;
using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelParts;
using R2API;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(R2API.LanguageAPI.PluginGUID)]
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
            if (!survivor || !survivor.bodyPrefab)
                return;

            if (!survivor.bodyPrefab.TryGetComponent(out ModelLocator survivorModelLocator))
                return;

            Transform survivorModelTransform = survivorModelLocator.modelTransform;
            if (!survivorModelTransform)
                return;

            if (!survivorModelTransform.TryGetComponent(out ModelPartsProvider survivorPartsProvider) ||
                !survivorModelTransform.TryGetComponent(out BonesProvider survivorBonesProvider))
            {
                return;
            }

            HashSet<Transform> usedModels = new HashSet<Transform>();

            Dictionary<string, Dictionary<string, string>> tokensDictionary = new Dictionary<string, Dictionary<string, string>>();

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

                switch (body.name)
                {
                    case "BeetleBody": // Bone mapping doesn't work for some reason, blacklist for now
                    case "BeetleGuardCrystalBody": // Bad material, logspam
                    case "BomberBody": // Just Commando
                    case "CommandoPerformanceTestBody": // Just Commando
                    case "EnforcerBody": // Literally just a cube
                    case "GolemBodyInvincible": // Just Stone Golem
                        continue;
                }

                ModelPartsProvider bodyModelPartsProvider = modelTransform.GetComponent<ModelPartsProvider>();
                if (!bodyModelPartsProvider)
                    continue;

                BonesProvider bodyBonesProvider = modelTransform.GetComponent<BonesProvider>();
                if (!bodyBonesProvider)
                    continue;

                const int MIN_MATCHING_BONES = 10;
                if (survivorBonesProvider.GetNumMatchingBones(bodyBonesProvider) < MIN_MATCHING_BONES)
                {
#if DEBUG
                    Log.Debug($"Not creating {body.name} skin for {survivor.cachedName}: Not enough common bones");
#endif
                    continue;
                }

                Sprite skinSprite = null;
                if (body.portraitIcon is Texture2D iconTexture)
                {
                    skinSprite = Sprite.Create(iconTexture, new Rect(0f, 0f, iconTexture.width, iconTexture.height), Vector2.one / 2f);
                }

                if (!usedModels.Add(modelTransform))
                    continue;

                ModelSwappedSkinDef createSkinDef(string nameSuffix, SkinDef baseSkin, int baseSkinIndex)
                {
                    ModelSwappedSkinDef skinDef = ScriptableObject.CreateInstance<ModelSwappedSkinDef>();

                    skinDef.name = $"skin{survivor.cachedName}_{body.name}{nameSuffix}";

                    string skinNameToken = $"SKIN_{survivor.cachedName.ToUpper()}_{body.name.ToUpper()}{nameSuffix.ToUpper()}";
                    skinDef.nameToken = skinNameToken;

                    foreach (Language language in Language.GetAllLanguages())
                    {
                        if (!tokensDictionary.TryGetValue(language.name, out Dictionary<string, string> tokenDictionaryForLanguage))
                        {
                            tokenDictionaryForLanguage = new Dictionary<string, string>();
                            tokensDictionary.Add(language.name, tokenDictionaryForLanguage);
                        }

                        string skinName = Language.GetString(body.baseNameToken, language.name);
                        if (baseSkin)
                        {
                            string baseSkinName = Language.GetString(baseSkin.nameToken, language.name);
                            if (string.IsNullOrWhiteSpace(baseSkinName))
                            {
                                baseSkinName = $"Variant {baseSkinIndex + 1}";
                            }

                            skinName += $" ({baseSkinName})";
                        }

                        tokenDictionaryForLanguage.Add(skinNameToken, skinName);
                    }

                    skinDef.icon = skinSprite;

                    skinDef.Initialize(survivorPartsProvider, bodyModelPartsProvider);

                    skinDef.NewModelBodyPrefab = body;
                    skinDef.NewModelTransformPrefab = modelTransform;

                    skinDef.ModelSkin = baseSkin;

                    return skinDef;
                }

                bool tryAddFromModelSkins()
                {
                    if (!modelTransform.TryGetComponent(out ModelSkinController modelSkinController))
                        return false;

                    IEnumerable<SkinDef> modelSkins = modelSkinController.skins.Where(s => s is not ModelSwappedSkinDef);
                    if (!modelSkins.Any())
                        return false;

                    int skinIndex = 0;
                    foreach (SkinDef modelSkin in modelSkins)
                    {
                        skins.Add(createSkinDef($"_{modelSkin.name}", modelSkin, skinIndex++));
                    }

                    return true;
                }

                if (!tryAddFromModelSkins())
                {
                    skins.Add(createSkinDef(string.Empty, null, -1));
                }
            }

            if (tokensDictionary.Count > 0)
            {
                LanguageAPI.Add(tokensDictionary);
            }
        }
    }
}
