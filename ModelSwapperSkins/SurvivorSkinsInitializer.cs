using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelParts;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModelSwapperSkins
{
    public class SurvivorSkinsInitializer
    {
        readonly SurvivorDef _survivor;

        readonly ModelPartsProvider _survivorPartsProvider;
        readonly BonesProvider _survivorBonesProvider;

        readonly HashSet<Transform> _usedModelTransforms = [];

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

            List<CharacterBody> bodies = BodyCatalog.allBodyPrefabBodyBodyComponents.ToList();
            bodies.Sort((a, b) =>
            {
                static string sanitizeName(string input)
                {
                    const char MAX_ASCII_CHAR = (char)0x7F;

                    StringBuilder stringBuilder = HG.StringBuilderPool.RentStringBuilder();

                    foreach (char c in input)
                    {
                        if (c <= MAX_ASCII_CHAR)
                        {
                            stringBuilder.Append(c);
                        }
                    }

                    if (stringBuilder.Length > 0 && stringBuilder.Length < input.Length)
                    {
                        input = stringBuilder.ToString();
                    }

                    HG.StringBuilderPool.ReturnStringBuilder(stringBuilder);

                    return input;
                }

                static string getSortName(CharacterBody body)
                {
                    switch (BodyCatalog.GetBodyName(body.bodyIndex))
                    {
                        case "ScavLunar1Body":
                            return "ScavengerLunar1";
                        case "ScavLunar2Body":
                            return "ScavengerLunar2";
                        case "ScavLunar3Body":
                            return "ScavengerLunar3";
                        case "ScavLunar4Body":
                            return "ScavengerLunar4";
                        default:
                            return sanitizeName(Language.GetString(body.baseNameToken, "en"));
                    }
                }

                string nameA = getSortName(a);
                string nameB = getSortName(b);

                return nameA.CompareTo(nameB);
            });

            foreach (CharacterBody body in bodies)
            {
                if (shouldCreateSkin(body))
                {
                    dest.AddRange(createSkinsForBody(body));
                }
            }
        }

        bool hasEnoughBonesToMatchWith(BonesProvider otherBonesProvider)
        {
            bool boneTest(BoneType bone)
            {
                return _survivorBonesProvider.HasMatchForBone(bone) && otherBonesProvider.CanMatchToBone(bone);
            }

            bool anyBonesValidMatch(params BoneType[] bones)
            {
                return Array.Exists(bones, boneTest);
            }

            return anyBonesValidMatch(BoneType.Head, BoneType.Chest, BoneType.Stomach, BoneType.Pelvis) &&
                   (anyBonesValidMatch(BoneType.ArmUpperL, BoneType.ArmLowerL, BoneType.ArmUpperR, BoneType.ArmLowerR) ||
                    anyBonesValidMatch(BoneType.LegUpperL, BoneType.LegLowerL, BoneType.LegUpperR, BoneType.LegLowerR));
        }

        bool shouldCreateSkin(CharacterBody body)
        {
            if (!body)
                return false;

            if (body.gameObject == _survivor.bodyPrefab)
                return false;

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

            if (!hasEnoughBonesToMatchWith(bodyBonesProvider))
            {
#if DEBUG
                Log.Debug($"Not creating {body.name} skin for {_survivor.cachedName}: Not enough common bones");
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

            ModelSwappedSkinDef createSkinDef(string nameSuffix, SkinDef baseSkin, int baseSkinIndex)
            {
                ModelSwappedSkinDef skinDef = ScriptableObject.CreateInstance<ModelSwappedSkinDef>();

                skinDef.name = $"skin{_survivor.cachedName}_{body.name}{nameSuffix}";

                string skinNameToken = $"SKIN_{_survivor.cachedName.ToUpper()}_{body.name.ToUpper()}{nameSuffix.ToUpper()}";
                skinDef.nameToken = skinNameToken;

                Dictionary<string, Dictionary<string, string>> skinTokenAdditions = [];
                foreach (Language language in Language.GetAllLanguages())
                {
                    if (!skinTokenAdditions.TryGetValue(language.name, out Dictionary<string, string> tokenDictionaryForLanguage))
                    {
                        tokenDictionaryForLanguage = [];
                        skinTokenAdditions.Add(language.name, tokenDictionaryForLanguage);
                    }

                    string skinName = Language.IsTokenInvalid(body.baseNameToken) ? "???" : Language.GetString(body.baseNameToken, language.name);
                    if (baseSkin)
                    {
                        string baseSkinName;
                        if (Language.IsTokenInvalid(baseSkin.nameToken))
                        {
                            if (baseSkinIndex < 0)
                            {
                                baseSkinName = string.Empty;
                            }
                            else
                            {
                                baseSkinName = $"Variant {baseSkinIndex + 1}";
                            }
                        }
                        else
                        {
                            baseSkinName = Language.GetString(baseSkin.nameToken, language.name);
                        }

                        if (!string.IsNullOrWhiteSpace(baseSkinName))
                        {
                            skinName += $" ({baseSkinName})";
                        }
                    }

                    tokenDictionaryForLanguage.Add(skinNameToken, skinName);
                }

                LanguageAPI.Add(skinTokenAdditions);

                skinDef.icon = SkinIconGenerator.GetOrCreateSkinIcon(body.portraitIcon as Texture2D, baseSkin);

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
