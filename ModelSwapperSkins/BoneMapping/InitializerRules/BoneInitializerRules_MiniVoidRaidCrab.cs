using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_MiniVoidRaidCrab : BoneInitializerRules
    {
        public static BoneInitializerRules_MiniVoidRaidCrab Instance { get; } = new BoneInitializerRules_MiniVoidRaidCrab();

        BoneInitializerRules_MiniVoidRaidCrab() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyBase");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "ROOT":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, -0.25f, -0.2f),
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f),
                        Scale = 0.8f,
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "HeadBase":
                    return new BoneInfo(BoneType.Head);
                case "backLeg.thigh.l":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "backLeg.foot.l":
                    return new BoneInfo(BoneType.LegLowerL);
                case "backLeg.toe.l":
                    return new BoneInfo(BoneType.FootL);
                case "backLeg.thigh.r":
                    return new BoneInfo(BoneType.LegUpperR);
                case "backLeg.foot.r":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "backLeg.toe.r":
                    return new BoneInfo(BoneType.FootR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "frontLeg.thigh.l":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "frontLeg.foot.l":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "frontLeg.toe.l":
                    return new BoneInfo(BoneType.HandL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "frontLeg.thigh.r":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "frontLeg.foot.r":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "frontLeg.toe.r":
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

                yield return new Bone(stomachBone)
                {
                    Info = new BoneInfo(BoneType.Base)
                    {
                        Scale = 0.01f,
                        MatchFlags = BoneMatchFlags.AllowMatchTo
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
