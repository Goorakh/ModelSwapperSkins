using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Croco : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Croco Instance { get; } = new BoneInitializerRules_Croco();

        BoneInitializerRules_Croco() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("CrocoBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo boneInfo = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (boneInfo.Type)
            {
                case BoneType.Stomach:
                case BoneType.Chest:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.Head:
                    boneInfo.PositionOffset += new Vector3(0f, 1f, -2f);
                    boneInfo.RotationOffset *= Quaternion.Euler(270f, 180f, 0f);
                    break;
                case BoneType.Jaw:
                    boneInfo.RotationOffset *= Quaternion.Euler(-30f, 0f, 0f);
                    break;
                case BoneType.ShoulderL:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmUpperL:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmLowerL:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.HandL:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.FootL:
                case BoneType.FootR:
                    boneInfo.PositionOffset += new Vector3(0f, 0.25f, 0f);
                    boneInfo.RotationOffset *= Quaternion.Euler(290f, 0f, 0f);
                    break;
                case BoneType.ShoulderR:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmUpperR:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmLowerR:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.HandR:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1L:
                case BoneType.Toe1R:
                    boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
            }

            return boneInfo;
        }

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;
            }

            Bone neck1Bone = existingBones.Find(b => b.Info.Type == BoneType.Neck1);
            if (neck1Bone != null)
            {
                yield return new Bone(neck1Bone)
                {
                    Info = new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, -1.4142f, 1.4142f),
                        RotationOffset = Quaternion.Euler(45f, 180f, 0f),
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [
                            new BoneExclusionRule([BoneType.Neck1], OtherBoneMatchExclusionRuleType.ExcludeIfAnyMatch)
                        ]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find Neck1 bone");
            }

            Bone shoulderLBone = existingBones.Find(b => b.Info.Type == BoneType.ShoulderL);
            if (shoulderLBone != null)
            {
                yield return new Bone(shoulderLBone)
                {
                    Info = new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, -1.4142f, -1.4142f),
                        RotationOffset = Quaternion.Euler(0f, 270f, 315f),
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [
                            new BoneExclusionRule([BoneType.ShoulderL], OtherBoneMatchExclusionRuleType.ExcludeIfAnyMatch)
                        ]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find ShoulderL bone");
            }

            Bone shoulderRBone = existingBones.Find(b => b.Info.Type == BoneType.ShoulderR);
            if (shoulderRBone != null)
            {
                yield return new Bone(shoulderRBone)
                {
                    Info = new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, -1.4142f, -1.4142f),
                        RotationOffset = Quaternion.Euler(0f, 90f, 45f),
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [
                            new BoneExclusionRule([BoneType.ShoulderR], OtherBoneMatchExclusionRuleType.ExcludeIfAnyMatch)
                        ]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find ShoulderR bone");
            }
        }
    }
}
