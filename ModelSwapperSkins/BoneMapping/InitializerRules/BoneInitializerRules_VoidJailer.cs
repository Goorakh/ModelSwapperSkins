using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_VoidJailer : BoneInitializerRules
    {
        public static BoneInitializerRules_VoidJailer Instance { get; } = new BoneInitializerRules_VoidJailer();

        BoneInitializerRules_VoidJailer() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("VoidJailerBody") || bodyIndex == BodyCatalog.FindBodyIndex("VoidJailerAllyBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "root_jnt":
                    return new BoneInfo(BoneType.Root);
                case "spine_01_jnt":
                    return new BoneInfo(BoneType.Pelvis)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 90f)
                    };
                case "spine_02_jnt":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        RotationOffset = Quaternion.Euler(270f, 90f, 0f)
                    };
                case "spine_04_jnt":
                    return new BoneInfo(BoneType.Chest)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
                case "head_jnt":
                    return new BoneInfo(BoneType.Head)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
                case "arm_left_jnt":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 90f),
                        Scale = 2f
                    };
                case "elbow_left_jnt":
                    return new BoneInfo(BoneType.ArmLowerL)
                    {
                        RotationOffset = Quaternion.Euler(90f, 90f, 0f),
                        Scale = 2f
                    };
                case "clavicle_right_jnt":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 45f),
                        Scale = 1.5f
                    };
                case "elbow_right_jnt":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(270f, 180f, 0f),
                        Scale = 1.5f
                    };
                case "hips_left_jnt":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        RotationOffset = Quaternion.Euler(45f, 295f, 0f)
                    };
                case "twist_knee_left_jnt":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 90f)
                    };
                case "twist_foot_left_jnt":
                    return new BoneInfo(BoneType.FootL)
                    {
                        PositionOffset = new Vector3(0f, 0f, 0.25f),
                        RotationOffset = Quaternion.Euler(0f, 270f, 90f)
                    };
                case "hips_right_jnt":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        RotationOffset = Quaternion.Euler(315f, 115f, 180f)
                    };
                case "twist_knee_right_jnt":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 90f)
                    };
                case "twist_foot_right_jnt":
                    return new BoneInfo(BoneType.FootR)
                    {
                        PositionOffset = new Vector3(0f, 0f, 0.25f),
                        RotationOffset = Quaternion.Euler(0f, 90f, 90f)
                    };
                case "tail_01_jnt":
                    return new BoneInfo(BoneType.Tail1)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
                case "tail_02_jnt":
                    return new BoneInfo(BoneType.Tail2)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
                case "tail_03_jnt":
                    return new BoneInfo(BoneType.Tail3)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
                case "tail_04_jnt":
                    return new BoneInfo(BoneType.Tail4)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
                default:
                    return BoneInfo.None;
            }
        }

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;
            }

            Bone pelvisBone = existingBones.Find(b => b.Info.Type == BoneType.Pelvis);
            if (pelvisBone != null)
            {
                yield return new Bone(pelvisBone)
                {
                    Info = new BoneInfo(BoneType.Base)
                    {
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [
                            new BoneExclusionRule([BoneType.Pelvis], OtherBoneMatchExclusionRuleType.ExcludeIfAnyMatch)
                        ]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find pelvis bone");
            }
        }
    }
}
