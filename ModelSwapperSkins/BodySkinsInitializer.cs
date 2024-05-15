using ModelSwapperSkins.BoneMapping;
using ModelSwapperSkins.ModelParts;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModelSwapperSkins
{
    public class BodySkinsInitializer
    {
        readonly CharacterBody _bodyPrefab;

        readonly ModelPartsProvider _bodyPartsProvider;
        readonly BonesProvider _bodyBonesProvider;
        readonly ModelSkinController _bodySkinController;

        readonly HashSet<Transform> _usedModelTransforms = [];

        public BodySkinsInitializer(CharacterBody bodyPrefab)
        {
            _bodyPrefab = bodyPrefab;

            if (_bodyPrefab && _bodyPrefab.TryGetComponent(out ModelLocator modelLocator))
            {
                Transform modelTransform = modelLocator.modelTransform;
                if (modelTransform)
                {
                    _bodyPartsProvider = modelTransform.GetComponent<ModelPartsProvider>();
                    _bodyBonesProvider = modelTransform.GetComponent<BonesProvider>();
                    _bodySkinController = modelTransform.GetComponent<ModelSkinController>();
                }
            }
        }

        bool isSurvivorValidForSkins()
        {
            return _bodyPrefab && _bodyPartsProvider && _bodyBonesProvider;
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
                return _bodyBonesProvider.HasMatchForBone(bone) && otherBonesProvider.CanMatchToBone(bone);
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

            if (body == _bodyPrefab)
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
                Log.Debug($"Not creating {body.name} skin for {_bodyPrefab.name}: Not enough matching bones");
#endif
                return false;
            }

            if (!_usedModelTransforms.Add(modelTransform))
                return false;

            return true;
        }

        IEnumerable<ModelSwappedSkinDef> createSkinsForBody(CharacterBody body)
        {
            ModelLocator modelLocator = body.GetComponent<ModelLocator>();
            Transform modelTransform = modelLocator.modelTransform;

            ModelPartsProvider bodyModelPartsProvider = modelTransform.GetComponent<ModelPartsProvider>();

            ModelSwappedSkinDef createSkinDef(SkinDef baseSkin, SkinDef modelSkin)
            {
                ModelSwappedSkinDef skinDef = ScriptableObject.CreateInstance<ModelSwappedSkinDef>();

                string skinName;
                if (baseSkin)
                {
                    if (modelSkin)
                    {
                        skinName = $"skin{_bodyPrefab.name}_{baseSkin.name}_{body.name}_{modelSkin.name}";
                    }
                    else
                    {
                        skinName = $"skin{_bodyPrefab.name}_{baseSkin.name}_{body.name}";
                    }
                }
                else if (modelSkin)
                {
                    skinName = $"skin{_bodyPrefab.name}_{body.name}_{modelSkin.name}";
                }
                else
                {
                    skinName = $"skin{_bodyPrefab.name}_{body.name}";
                }

                skinDef.name = skinName;

                skinDef.icon = BodyIconCache.GetOrCreatePortraitIcon(body.portraitIcon as Texture2D);

                if (baseSkin)
                {
                    skinDef.baseSkins = [baseSkin];
                }

                skinDef.NewModelBodyPrefab = body;
                skinDef.NewModelTransformPrefab = modelTransform;

                skinDef.ModelSkin = modelSkin;

                skinDef.Initialize(_bodyPartsProvider, bodyModelPartsProvider);

                return skinDef;
            }

            SkinDef[] bodySkins = _bodySkinController ? _bodySkinController.skins : null;
            if (bodySkins == null || bodySkins.Length == 0)
                bodySkins = [null];

            ModelSkinController modelSkinController = modelTransform.GetComponent<ModelSkinController>();

            foreach (SkinDef bodySkin in bodySkins)
            {
                if (modelSkinController)
                {
                    IEnumerable<SkinDef> modelSkins = modelSkinController.skins.Where(s => s is not ModelSwappedSkinDef);
                    if (modelSkins.Any())
                    {
                        foreach (SkinDef modelSkin in modelSkins)
                        {
                            yield return createSkinDef(bodySkin, modelSkin);
                        }

                        continue;
                    }
                }

                yield return createSkinDef(bodySkin, null);
            }
        }
    }
}
