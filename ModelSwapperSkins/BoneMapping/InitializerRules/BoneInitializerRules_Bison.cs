using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Bison : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Bison Instance { get; } = new BoneInitializerRules_Bison();

        BoneInitializerRules_Bison() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("BisonBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type == BoneType.None)
            {
                switch (potentialBoneTransform.name)
                {
                    case "thigh.b.l":
                        return new BoneInfo(BoneType.LegUpperL)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "calf.b.l":
                        return new BoneInfo(BoneType.LegLowerL);
                    case "foot.b.l":
                        return new BoneInfo(BoneType.FootL)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "toe.b.l":
                        return new BoneInfo(BoneType.Toe1L);
                    case "thigh.b.r":
                        return new BoneInfo(BoneType.LegUpperR)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "calf.b.r":
                        return new BoneInfo(BoneType.LegLowerR);
                    case "foot.b.r":
                        return new BoneInfo(BoneType.FootR)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "toe.b.r":
                        return new BoneInfo(BoneType.Toe1R);
                    case "thigh.f.l":
                        return new BoneInfo(BoneType.ArmUpperL)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "calf.f.l":
                        return new BoneInfo(BoneType.ArmLowerL)
                        {
                            RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                        };
                    case "foot.f.l":
                        return new BoneInfo(BoneType.HandL);
                    case "thigh.f.r":
                        return new BoneInfo(BoneType.ArmUpperR)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "calf.f.r":
                        return new BoneInfo(BoneType.ArmLowerR)
                        {
                            RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                        };
                    case "foot.f.r":
                        return new BoneInfo(BoneType.HandR);
                    default:
                        return bone;
                }
            }

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.RotationOffset *= Quaternion.Euler(315f, 180f, 0f);
                    break;
                case BoneType.Stomach:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.Chest:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    bone.Scale *= 2f;
                    bone.MatchFlags = BoneMatchFlags.MatchToOther;
                    break;
            }

            return bone;
        }

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;
            }

            Bone chestBone = existingBones.Find(b => b.Info.Type == BoneType.Chest && b.Info.MatchFlags == BoneMatchFlags.MatchToOther);
            if (chestBone != null)
            {
                yield return new Bone(chestBone)
                {
                    Info = new BoneInfo(BoneType.Chest)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
        }
    }
}
