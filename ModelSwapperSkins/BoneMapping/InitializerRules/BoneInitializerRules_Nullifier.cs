using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Nullifier : BoneInitializerRules
    {
        public static BoneInitializerRules_Nullifier Instance { get; } = new BoneInitializerRules_Nullifier();

        BoneInitializerRules_Nullifier() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("NullifierBody") || bodyIndex == BodyCatalog.FindBodyIndex("NullifierAllyBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "ROOT":
                    return new BoneInfo(BoneType.Root);
                case "Base":
                    return new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, -0.15f, 0f)
                    };
                case "Head":
                    return new BoneInfo(BoneType.Head);
                case "backThigh.l":
                    return new BoneInfo(BoneType.Pelvis)
                    {
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "backCalf.l":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "backFoot.l":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "backToe.l":
                    return new BoneInfo(BoneType.FootL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "backThigh.r":
                    return new BoneInfo(BoneType.Pelvis)
                    {
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "backCalf.r":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "backFoot.r":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "backToe.r":
                    return new BoneInfo(BoneType.FootR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "frontThigh.l":
                    return new BoneInfo(BoneType.ShoulderL);
                case "frontCalf.l":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "frontFoot.l":
                    return new BoneInfo(BoneType.ArmLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "frontToe.l":
                    return new BoneInfo(BoneType.HandL);
                case "frontThigh.r":
                    return new BoneInfo(BoneType.ShoulderR);
                case "frontCalf.r":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "frontFoot.r":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270, 0f)
                    };
                case "frontToe.r":
                    return new BoneInfo(BoneType.HandR);
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

            Bone chestBone = existingBones.Find(b => b.Info.Type == BoneType.Chest);
            if (chestBone != null)
            {
                yield return new Bone(chestBone)
                {
                    Info = new BoneInfo(BoneType.Base)
                    {
                        ExclusionRules = [
                            new BoneExclusionRule([BoneType.Chest], OtherBoneMatchExclusionRuleType.ExcludeIfAnyMatch)
                        ]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find chest bone");
            }
        }
    }
}
