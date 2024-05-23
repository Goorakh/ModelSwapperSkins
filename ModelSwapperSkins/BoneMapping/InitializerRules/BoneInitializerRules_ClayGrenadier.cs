using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_ClayGrenadier : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_ClayGrenadier Instance { get; } = new BoneInitializerRules_ClayGrenadier();

        BoneInitializerRules_ClayGrenadier() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ClayGrenadierBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Head:
                        bone.PositionOffset += new Vector3(0f, -0.15f, -0.3f);
                        bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                        break;
                    case BoneType.Pelvis:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 180f);
                        break;
                    case BoneType.LegUpperL:
                    case BoneType.LegUpperR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.ShoulderL:
                    case BoneType.ShoulderR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.ArmUpperL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.HandL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.ArmUpperR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.HandR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                }

                switch (bone.Type)
                {
                    case BoneType.LegUpperL:
                    case BoneType.LegLowerL:
                    case BoneType.FootL:
                    case BoneType.Toe1L:
                    case BoneType.LegUpperR:
                    case BoneType.LegLowerR:
                    case BoneType.FootR:
                    case BoneType.Toe1R:
                        bone.Scale *= 0.7f;
                        break;
                }

                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                case "WholeBody":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, -0.3f, 0f)
                    };
                case "torso":
                    return new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, -0.3f, 0f)
                    };
                default:
                    return bone;
            }
        }

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;
            }

            Bone stomachBone = existingBones.Find(b => b.Info.Type == BoneType.Stomach);
            if (stomachBone != null)
            {
                yield return new Bone(stomachBone)
                {
                    Info = new BoneInfo(BoneType.Base)
                    {
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [
                            new BoneExclusionRule([BoneType.Stomach], OtherBoneMatchExclusionRuleType.ExcludeIfAnyMatch)
                        ]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find stomach bone");
            }
        }
    }
}
