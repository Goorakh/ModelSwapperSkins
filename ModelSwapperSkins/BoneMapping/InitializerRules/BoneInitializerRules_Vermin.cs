using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Vermin : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Vermin Instance { get; } = new BoneInitializerRules_Vermin();

        BoneInitializerRules_Vermin() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("VerminBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type == BoneType.None)
            {
                switch (potentialBoneTransform.name)
                {
                    case "Spine1":
                        return new BoneInfo(BoneType.Pelvis)
                        {
                            PositionOffset = new Vector3(0f, 0f, 0.5f),
                            RotationOffset = Quaternion.Euler(0f, 180f, 180f)
                        };
                    case "Spine2":
                        return new BoneInfo(BoneType.Stomach)
                        {
                            PositionOffset = new Vector3(0f, 0f, 0.5f)
                        };
                    case "Spine3":
                        return new BoneInfo(BoneType.Chest)
                        {
                            PositionOffset = new Vector3(0f, -0.5f, 0.5f)
                        };
                    case "Leg1.l":
                        return new BoneInfo(BoneType.LegLowerL);
                    case "Leg1.r":
                        return new BoneInfo(BoneType.LegLowerR);
                }
            }

            switch (bone.Type)
            {
                case BoneType.Base:
                    bone.RotationOffset *= Quaternion.Euler(330f, 180f, 0f);
                    break;
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, 0.5f, 0.5f);
                    bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                    break;
                case BoneType.Jaw:
                    bone.PositionOffset += new Vector3(0f, 0f, 0.4f);
                    bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                    break;
                case BoneType.Tongue3:
                case BoneType.Tongue4:
                case BoneType.LegUpperL:
                case BoneType.LegUpperR:
                case BoneType.Toe1L:
                case BoneType.Toe1R:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
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
                yield return new Bone(tail1Bone)
                {
                    Info = new BoneInfo(BoneType.Pelvis)
                    {
                        PositionOffset = new Vector3(0f, -1.3228f, -0.5592f),
                        RotationOffset = Quaternion.Euler(330f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [
                            new BoneExclusionRule([BoneType.Tail1], OtherBoneMatchExclusionRuleType.ExcludeIfAnyMatch)
                        ]
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
