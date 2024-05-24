using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_MiniMushroom : BoneInitializerRules
    {
        public static BoneInitializerRules_MiniMushroom Instance { get; } = new BoneInitializerRules_MiniMushroom();

        BoneInitializerRules_MiniMushroom() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("MiniMushroomBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "miniMush_TrajectorySHJnt":
                    return new BoneInfo(BoneType.Root);
                case "miniMush_ROOTSHJnt":
                    return new BoneInfo(BoneType.Base)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 180f)
                    };
                case "miniMush_Spine_TopSHJnt":
                    return new BoneInfo(BoneType.Head)
                    {
                        RotationOffset = Quaternion.Euler(90f, 270f, 0f),
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "miniMush_Spine_02SHJnt":
                    return new BoneInfo(BoneType.Chest)
                    {
                        RotationOffset = Quaternion.Euler(90f, 270f, 0f)
                    };
                case "miniMush_Spine_01SHJnt":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        RotationOffset = Quaternion.Euler(90f, 270f, 0f)
                    };
                case "miniMush_L_Leg_HipSHJnt":
                    return new BoneInfo(BoneType.LegUpperL);
                case "miniMush_L_Leg_KneeSHJnt":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 215f, 180f),
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "miniMush_L_Leg_AnkleSHJnt":
                    return new BoneInfo(BoneType.FootL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 270f)
                    };
                case "miniMush_R_Leg_HipSHJnt":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        RotationOffset = Quaternion.Euler(180f, 0f, 0f)
                    };
                case "miniMush_R_Leg_KneeSHJnt":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 45f, 0f),
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "miniMush_R_Leg_AnkleSHJnt":
                    return new BoneInfo(BoneType.FootR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 270f)
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

            Bone headBone = existingBones.Find(b => b.Info.Type == BoneType.Head);
            if (headBone != null)
            {
                yield return new Bone(headBone)
                {
                    Info = new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(-0.75f, 0f, 0f),
                        RotationOffset = headBone.Info.RotationOffset,
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Missing head bone");
            }

            Bone legLowerLBone = existingBones.Find(b => b.Info.Type == BoneType.LegLowerL);
            if (legLowerLBone != null)
            {
                yield return new Bone(legLowerLBone)
                {
                    Info = new BoneInfo(BoneType.LegLowerL)
                    {
                        PositionOffset = new Vector3(0f, 1f, 0f),
                        RotationOffset = legLowerLBone.Info.RotationOffset,
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Missing LegUpperL bone");
            }

            Bone legLowerRBone = existingBones.Find(b => b.Info.Type == BoneType.LegLowerR);
            if (legLowerRBone != null)
            {
                yield return new Bone(legLowerRBone)
                {
                    Info = new BoneInfo(BoneType.LegLowerR)
                    {
                        PositionOffset = new Vector3(0f, -1f, 0f),
                        RotationOffset = legLowerRBone.Info.RotationOffset,
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Missing LegUpperL bone");
            }

            Bone stomachBone = existingBones.Find(b => b.Info.Type == BoneType.Stomach);
            if (stomachBone != null)
            {
                yield return new Bone(stomachBone)
                {
                    Info = new BoneInfo(BoneType.Pelvis)
                    {
                        RotationOffset = Quaternion.Euler(270f, 270f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Missing Stomach bone");
            }

            Bone chestBone = existingBones.Find(b => b.Info.Type == BoneType.Chest) ?? stomachBone;
            if (chestBone != null)
            {
                yield return new Bone(chestBone)
                {
                    Info = new BoneInfo(BoneType.ArmUpperL)
                    {
                        Scale = 0.01f,
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };

                yield return new Bone(chestBone)
                {
                    Info = new BoneInfo(BoneType.ArmUpperR)
                    {
                        Scale = 0.01f,
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Missing Chest bone");
            }
        }
    }
}
