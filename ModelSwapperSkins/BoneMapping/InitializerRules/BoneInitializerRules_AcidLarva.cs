using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_AcidLarva : BoneInitializerRules
    {
        public static BoneInitializerRules_AcidLarva Instance { get; } = new BoneInitializerRules_AcidLarva();

        BoneInitializerRules_AcidLarva() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("AcidLarvaBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "Root":
                    return new BoneInfo(BoneType.Root);
                case "BodyBase":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, 0.33f, -2.5f),
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f)
                    };
                case "Beak.Lower":
                    return new BoneInfo(BoneType.Jaw)
                    {
                        PositionOffset = new Vector3(0f, 0f, -0.5f),
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Beak.Upper":
                    return new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, 0f, -1.5f),
                        RotationOffset = Quaternion.Euler(305f, 180f, 0f)
                    };
                case "FrontLeg_Thigh1.L":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "FrontLeg_Calf.L":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "FrontLeg_Foot.L":
                    return new BoneInfo(BoneType.HandL);
                case "FrontLeg_Thigh1.R":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "FrontLeg_Calf.R":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "FrontLeg_Foot.R":
                    return new BoneInfo(BoneType.HandR);
                case "RearLeg_Thigh1.L":
                    return new BoneInfo(BoneType.LegUpperL);
                case "RearLeg_Calf.L":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "RearLeg_Foot.L":
                    return new BoneInfo(BoneType.FootL);
                case "RearLeg_Thigh1.R":
                    return new BoneInfo(BoneType.LegUpperR);
                case "RearLeg_Calf.R":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "RearLeg_Foot.R":
                    return new BoneInfo(BoneType.FootR);
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
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };

                yield return new Bone(stomachBone)
                {
                    Info = new BoneInfo(BoneType.Pelvis)
                    {
                        RotationOffset = Quaternion.Euler(270f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };

                yield return new Bone(stomachBone)
                {
                    Info = new BoneInfo(BoneType.Chest)
                    {
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Failed to find stomach bone");
            }

            Bone jawBone = existingBones.Find(b => b.Info.Type == BoneType.Jaw);
            if (jawBone != null)
            {
                yield return new Bone(jawBone)
                {
                    Info = new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, 0f, -1.5f),
                        RotationOffset = Quaternion.Euler(300f, 180f, 0f),
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [
                            new BoneExclusionRule([BoneType.Jaw], OtherBoneMatchExclusionRuleType.ExcludeIfAnyMatch)
                        ]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find Jaw bone");
            }
        }
    }
}
