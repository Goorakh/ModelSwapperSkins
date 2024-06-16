using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Gup : BoneInitializerRules
    {
        public static BoneInitializerRules_Gup Instance { get; } = new BoneInitializerRules_Gup();

        BoneInitializerRules_Gup() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("GupBody") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("GeepBody") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("GipBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "ROOT":
                    return new BoneInfo(BoneType.Root);
                case "mainBody.1":
                    return new BoneInfo(BoneType.Head)
                    {
                        Scale = 3f
                    };
                case "LegBack.1.L":
                case "LegBack.1.R":
                case "LegFront.1.L":
                    return new BoneInfo(BoneType.Head)
                    {
                        RotationOffset = Quaternion.Euler(270f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "LegFront.1.R":
                    return new BoneInfo(BoneType.Head)
                    {
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "LegMid.1.L":
                case "LegMid.1.R":
                    return new BoneInfo(BoneType.Head)
                    {
                        Scale = 100f,
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "LegBack.2.L":
                    return new BoneInfo(BoneType.LegUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 270f, 0f)
                    };
                case "LegBack.3.L":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(270f, 0f, 0f)
                    };
                case "LegBack.4.L":
                    return new BoneInfo(BoneType.FootL)
                    {
                        PositionOffset = new Vector3(0f, -0.1f, 0.2f),
                        RotationOffset = Quaternion.Euler(270f, 0f, 0f)
                    };
                case "LegBack.2.R":
                    return new BoneInfo(BoneType.LegUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 90f, 0f)
                    };
                case "LegBack.3.R":
                    return new BoneInfo(BoneType.LegLowerR);
                case "LegBack.4.R":
                    return new BoneInfo(BoneType.FootR)
                    {
                        PositionOffset = new Vector3(0f, -0.1f, 0.2f),
                        RotationOffset = Quaternion.Euler(270f, 0f, 0f)
                    };
                case "LegFront.2.L":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "LegFront.3.L":
                    return new BoneInfo(BoneType.ArmLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "LegFront.4.L":
                    return new BoneInfo(BoneType.HandL);
                case "LegFront.2.R":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "LegFront.3.R":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "LegFront.4.R":
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

            Bone headBone = existingBones.Find(b => b.Info.Type == BoneType.Head);
            if (headBone != null)
            {
                yield return new Bone(headBone)
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
                Log.Error("Failed to find Head bone");
            }
        }
    }
}
