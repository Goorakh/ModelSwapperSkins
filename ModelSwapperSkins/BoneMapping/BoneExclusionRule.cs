using HG;
using ModelSwapperSkins.Patches;
using RoR2;
using System;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    [Serializable]
    public struct BoneExclusionRule
    {
        [SerializeField]
        BoneExclusionRuleType _ruleType;

        [SerializeField]
        int _subRuleType;

        [SerializeField]
        BoneType[] _otherBoneMatches;

        [SerializeField]
        SkinDef _modelSkin;

        public BoneExclusionRule(BoneType[] otherBoneMatches, OtherBoneMatchExclusionRuleType matchExclusionRule)
        {
            _ruleType = BoneExclusionRuleType.OtherBoneMatch;
            _subRuleType = (int)matchExclusionRule;
            _otherBoneMatches = otherBoneMatches ?? throw new ArgumentNullException(nameof(otherBoneMatches));
        }

        public BoneExclusionRule(SkinDef modelSkin, ModelSkinExclusionRuleType skinExclusionRule)
        {
            _ruleType = BoneExclusionRuleType.ModelSkin;
            _subRuleType = (int)skinExclusionRule;
            _modelSkin = modelSkin;
        }

        public readonly bool ShouldExclude(BonesProvider currentBones, BonesProvider targetBones)
        {
            switch (_ruleType)
            {
                case BoneExclusionRuleType.OtherBoneMatch:
                    return (OtherBoneMatchExclusionRuleType)_subRuleType switch
                    {
                        OtherBoneMatchExclusionRuleType.ExcludeIfAllMatch => Array.TrueForAll(_otherBoneMatches, targetBones.HasMatchForBone),
                        OtherBoneMatchExclusionRuleType.ExcludeIfAnyMatch => Array.Exists(_otherBoneMatches, targetBones.HasMatchForBone),
                        OtherBoneMatchExclusionRuleType.ExcludeIfNoMatch => !Array.Exists(_otherBoneMatches, targetBones.HasMatchForBone),
                        OtherBoneMatchExclusionRuleType.ExcludeIfNotAllMatch => !Array.TrueForAll(_otherBoneMatches, targetBones.HasMatchForBone),
                        _ => throw new NotImplementedException($"Bone match rule type {(OtherBoneMatchExclusionRuleType)_subRuleType} is not implemented"),
                    };

                case BoneExclusionRuleType.ModelSkin:
                    SkinDef appliedSkin = currentBones.TryGetComponent(out ModelSkinController skinController) ? ArrayUtils.GetSafe(skinController.skins, skinController.currentSkinIndex) : null;

                    if (appliedSkin is ModelSwappedSkinDef && appliedSkin.baseSkins != null && appliedSkin.baseSkins.Length > 0)
                    {
                        appliedSkin = appliedSkin.baseSkins[0];
                    }
                    
                    return appliedSkin && (ModelSkinExclusionRuleType)_subRuleType switch
                    {
                        ModelSkinExclusionRuleType.ExcludeIfApplied => appliedSkin == _modelSkin,
                        ModelSkinExclusionRuleType.ExcludeIfNotApplied => appliedSkin != _modelSkin,
                        _ => throw new NotImplementedException($"Model skin rule type {(ModelSkinExclusionRuleType)_subRuleType} is not implemented"),
                    };

                default:
                    throw new NotImplementedException($"Rule type {_ruleType} is not implemented");
            }
        }
    }
}
