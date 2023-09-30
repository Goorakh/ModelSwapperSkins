using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelParts;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins
{
    public class SurvivorSkinsInitializer
    {
        readonly SurvivorDef _survivor;

        readonly ModelPartsProvider _survivorPartsProvider;
        readonly BonesProvider _survivorBonesProvider;

        readonly HashSet<Transform> _usedModelTransforms = new HashSet<Transform>();

        public SurvivorSkinsInitializer(SurvivorDef survivor)
        {
            _survivor = survivor;

            if (survivor && survivor.bodyPrefab && survivor.bodyPrefab.TryGetComponent(out ModelLocator survivorModelLocator))
            {
                Transform survivorModelTransform = survivorModelLocator.modelTransform;
                if (survivorModelTransform)
                {
                    _survivorPartsProvider = survivorModelTransform.GetComponent<ModelPartsProvider>();
                    _survivorBonesProvider = survivorModelTransform.GetComponent<BonesProvider>();
                }
            }
        }

        bool isSurvivorValidForSkins()
        {
            return _survivor && _survivor.bodyPrefab && _survivorPartsProvider && _survivorBonesProvider;
        }

        public void TryCreateSkins(List<SkinDef> dest)
        {
            if (!isSurvivorValidForSkins())
                return;

            _usedModelTransforms.Clear();

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                if (shouldCreateSkin(body))
                {
                    dest.AddRange(createSkinsForBody(body));
                }
            }
        }

        bool shouldCreateSkin(CharacterBody body)
        {
            if (!body)
                return false;

            if (body.gameObject == _survivor.bodyPrefab)
                return false;

            if (string.IsNullOrWhiteSpace(body.baseNameToken) || Language.GetString(body.baseNameToken) == body.baseNameToken)
                return false;

            switch (body.name)
            {
                case "BeetleBody": // Bone mapping doesn't work for some reason, blacklist for now
                case "BeetleGuardCrystalBody": // Bad material, logspam
                case "BomberBody": // Just Commando
                case "CommandoPerformanceTestBody": // Just Commando
                case "EnforcerBody": // Literally just a cube
                case "GolemBodyInvincible": // Just Stone Golem
                    return false;
            }

            ModelLocator modelLocator = body.GetComponent<ModelLocator>();
            if (!modelLocator)
                return false;

            Transform modelTransform = modelLocator.modelTransform;
            if (!modelTransform || modelTransform.childCount == 0)
                return false;

            ModelPartsProvider bodyModelPartsProvider = modelTransform.GetComponent<ModelPartsProvider>();
            if (!bodyModelPartsProvider)
                return false;

            BonesProvider bodyBonesProvider = modelTransform.GetComponent<BonesProvider>();
            if (!bodyBonesProvider)
                return false;

            const int MIN_MATCHING_BONES = 10;
            bool canMatchBones = BoneInitializer.HasCustomIntializerRules(body.bodyIndex) || _survivorBonesProvider.GetNumMatchingBones(bodyBonesProvider) >= MIN_MATCHING_BONES;

            if (!canMatchBones)
            {
#if DEBUG
                Log.Debug($"Not creating {body.name} skin for {_survivor.cachedName}: Not enough common bones (only {_survivorBonesProvider.GetNumMatchingBones(bodyBonesProvider)}, {MIN_MATCHING_BONES} required)");
#endif
                return false;
            }

            if (!_usedModelTransforms.Add(modelTransform))
                return false;

            return true;
        }

        IEnumerable<SkinDef> createSkinsForBody(CharacterBody body)
        {
            ModelLocator modelLocator = body.GetComponent<ModelLocator>();
            Transform modelTransform = modelLocator.modelTransform;

            ModelPartsProvider bodyModelPartsProvider = modelTransform.GetComponent<ModelPartsProvider>();

            Sprite skinSprite = null;
            if (body.portraitIcon is Texture2D iconTexture)
            {
                skinSprite = Sprite.Create(iconTexture, new Rect(0f, 0f, iconTexture.width, iconTexture.height), Vector2.one / 2f);
            }

            ModelSwappedSkinDef createSkinDef(string nameSuffix, SkinDef baseSkin, int baseSkinIndex)
            {
                ModelSwappedSkinDef skinDef = ScriptableObject.CreateInstance<ModelSwappedSkinDef>();

                skinDef.name = $"skin{_survivor.cachedName}_{body.name}{nameSuffix}";

                string skinNameToken = $"SKIN_{_survivor.cachedName.ToUpper()}_{body.name.ToUpper()}{nameSuffix.ToUpper()}";
                skinDef.nameToken = skinNameToken;

                Dictionary<string, Dictionary<string, string>> skinTokenAdditions = new Dictionary<string, Dictionary<string, string>>();
                foreach (Language language in Language.GetAllLanguages())
                {
                    if (!skinTokenAdditions.TryGetValue(language.name, out Dictionary<string, string> tokenDictionaryForLanguage))
                    {
                        tokenDictionaryForLanguage = new Dictionary<string, string>();
                        skinTokenAdditions.Add(language.name, tokenDictionaryForLanguage);
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

                LanguageAPI.Add(skinTokenAdditions);

                skinDef.icon = skinSprite;

                skinDef.Initialize(_survivorPartsProvider, bodyModelPartsProvider);

                skinDef.NewModelBodyPrefab = body;
                skinDef.NewModelTransformPrefab = modelTransform;

                skinDef.ModelSkin = baseSkin;

                return skinDef;
            }

            if (modelTransform.TryGetComponent(out ModelSkinController modelSkinController))
            {
                IEnumerable<SkinDef> modelSkins = modelSkinController.skins.Where(s => s is not ModelSwappedSkinDef);
                if (modelSkins.Any())
                {
                    int skinIndex = 0;
                    foreach (SkinDef modelSkin in modelSkins)
                    {
                        yield return createSkinDef($"_{modelSkin.name}", modelSkin, skinIndex++);
                    }

                    yield break;
                }
            }

            yield return createSkinDef(string.Empty, null, -1);
        }
    }
}
