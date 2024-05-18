using System;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    [Serializable]
    public record struct BoneInfo(BoneType Type, Vector3 PositionOffset, Quaternion RotationOffset, float Scale, BoneExclusionRule[] ExclusionRules)
    {
        public static readonly BoneInfo None = new BoneInfo(BoneType.None);

        public BoneType Type = Type;
        public Vector3 PositionOffset = PositionOffset;
        public Quaternion RotationOffset = RotationOffset;
        public float Scale = Scale;

        public BoneExclusionRule[] ExclusionRules = ExclusionRules;

        public BoneMatchFlags MatchFlags = BoneMatchFlags.AllowCompleteMatch;

        public BoneInfo(BoneType type) : this(type, Vector3.zero, Quaternion.identity, 1f, [])
        {
        }

        public readonly bool ShouldExcludeMatch(BonesProvider currentBones, BonesProvider targetBones)
        {
            for (int i = 0; i < ExclusionRules.Length; i++)
            {
                if (ExclusionRules[i].ShouldExclude(currentBones, targetBones))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
