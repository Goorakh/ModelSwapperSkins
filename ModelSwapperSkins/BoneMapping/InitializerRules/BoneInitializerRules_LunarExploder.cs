using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_LunarExploder : BoneInitializerRules
    {
        public static BoneInitializerRules_LunarExploder Instance { get; } = new BoneInitializerRules_LunarExploder();

        BoneInitializerRules_LunarExploder() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("LunarExploderBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "ROOT":
                    return new BoneInfo(BoneType.Root);
                case "stoneBase":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, 0f, -0.3f),
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f)
                    };
                case "frontThigh.l":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "frontFoot.l":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "frontThigh.r":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "frontFoot.r":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "backThigh.l":
                    return new BoneInfo(BoneType.LegUpperL);
                case "backFoot.l":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "backThigh.r":
                    return new BoneInfo(BoneType.LegUpperR);
                case "backFoot.r":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
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

            Bone stomachBone = existingBones.Find(b => b.Info.Type == BoneType.Stomach);
            if (stomachBone != null)
            {
                yield return new Bone(stomachBone)
                {
                    Info = new BoneInfo(BoneType.Base)
                    {
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };

                yield return new Bone(stomachBone)
                {
                    Info = new BoneInfo(BoneType.Pelvis)
                    {
                        PositionOffset = new Vector3(0f, 0f, -0.6f),
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

                yield return new Bone(stomachBone)
                {
                    Info = new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, 0f, 1f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Missing Stomach bone");
            }
        }
    }
}
