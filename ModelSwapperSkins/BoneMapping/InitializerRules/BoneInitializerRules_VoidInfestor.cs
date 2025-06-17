using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_VoidInfestor : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_VoidInfestor Instance { get; } = new BoneInitializerRules_VoidInfestor();

        BoneInitializerRules_VoidInfestor()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("VoidInfestorBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "ThoraxLower1":
                    return new BoneInfo(BoneType.Pelvis)
                    {
                        PositionOffset = new Vector3(0f, 0f, -0.3f)
                    };
                case "ThoraxLower2":
                    return new BoneInfo(BoneType.Tail1);
                case "Tail1":
                    return new BoneInfo(BoneType.Tail2);
                case "Tail2":
                    return new BoneInfo(BoneType.Tail3);
                case "Tail3":
                    return new BoneInfo(BoneType.Tail4);
                case "ThoraxUpper1":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, 0f, -0.3f),
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "ThoraxUpper2":
                    return new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, 0f, -0.3f),
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };

                case "LegBack1.l":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        PositionOffset = new Vector3(0f, -0.1f, 0f),
                        Scale = 0.25f
                    };
                case "LegBack2.l":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f),
                        Scale = 0.275f
                    };

                case "LegBack1.r":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        PositionOffset = new Vector3(0f, -0.1f, 0f),
                        Scale = 0.25f
                    };
                case "LegBack2.r":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f),
                        Scale = 0.275f
                    };

                case "LegFront1.l":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        PositionOffset = new Vector3(0f, -0.1f, 0f),
                        Scale = 0.25f
                    };
                case "LegFront2.l":
                    return new BoneInfo(BoneType.ArmLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f),
                        Scale = 0.25f
                    };

                case "LegFront1.r":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        PositionOffset = new Vector3(0f, -0.1f, 0f),
                        Scale = 0.25f
                    };
                case "LegFront2.r":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f),
                        Scale = 0.25f
                    };
            }

            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, 0.15f, -0.5f);
                    bone.RotationOffset *= Quaternion.Euler(270f, 180f, 0f);
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

            Bone tail1Bone = existingBones.Find(b => b.Info.Type == BoneType.Tail1);
            if (tail1Bone != null)
            {
                // ThoraxLower2
                yield return new Bone(tail1Bone)
                {
                    Info = new BoneInfo(BoneType.Pelvis)
                    {
                        PositionOffset = new Vector3(0f, -0.6763f, 0.2521f),
                        RotationOffset = Quaternion.Euler(315f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [new BoneExclusionRule([BoneType.Tail1], OtherBoneMatchExclusionRuleType.ExcludeIfAllMatch)]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find Tail1 bone");
            }
        }
    }
}
