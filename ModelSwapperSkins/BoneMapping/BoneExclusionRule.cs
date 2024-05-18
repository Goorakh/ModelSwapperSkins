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

        public BoneExclusionRule(BoneType[] otherBoneMatches, OtherBoneMatchExclusionRuleType matchExclusionRule)
        {
            _ruleType = BoneExclusionRuleType.OtherBoneMatch;
            _subRuleType = (int)matchExclusionRule;
            _otherBoneMatches = otherBoneMatches ?? throw new ArgumentNullException(nameof(otherBoneMatches));
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
                        _ => throw new NotImplementedException($"Bone match rule type {(OtherBoneMatchExclusionRuleType)_subRuleType} is not implemented"),
                    };

                default:
                    throw new NotImplementedException($"Rule type {_ruleType} is not implemented");
            }
        }
    }
}
