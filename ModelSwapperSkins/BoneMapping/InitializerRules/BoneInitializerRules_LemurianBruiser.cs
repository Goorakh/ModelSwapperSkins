using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_LemurianBruiser : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_LemurianBruiser Instance { get; } = new BoneInitializerRules_LemurianBruiser();

        BoneInitializerRules_LemurianBruiser() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("LemurianBruiserBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            switch (bone.Type)
            {
                case BoneType.Pelvis:
                    bone.PositionOffset += new Vector3(0f, 1f, 0f);
                    break;
                case BoneType.Stomach:
                case BoneType.Chest:
                    bone.PositionOffset += new Vector3(0f, -2f, 0f);
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.Neck1:
                    bone.RotationOffset *= Quaternion.Euler(45f, 0f, 0f);
                    break;
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, 0f, -2.5f);
                    bone.RotationOffset *= Quaternion.Euler(270f, 180f, 0f);
                    break;
                case BoneType.Jaw:
                    bone.PositionOffset += new Vector3(0f, -0.5f, 0f);
                    bone.RotationOffset *= Quaternion.Euler(330f, 0f, 0f);
                    break;
                case BoneType.FootL:
                case BoneType.FootR:
                    bone.PositionOffset += new Vector3(0f, 1f, 0f);
                    bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                    break;
                case BoneType.Toe1L:
                case BoneType.Toe1R:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ShoulderL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmUpperL:
                    bone.PositionOffset += new Vector3(0f, -0.15f, 0f);
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmLowerL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.HandL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.ShoulderR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmUpperR:
                    bone.PositionOffset += new Vector3(0f, -0.15f, 0f);
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmLowerR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.HandR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
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

            Bone neck1Bone = existingBones.Find(b => b.Info.Type == BoneType.Neck1);
            if (neck1Bone != null)
            {
                yield return new Bone(neck1Bone)
                {
                    Info = new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, -3f, 0f),
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f),
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

            Bone jawBone = existingBones.Find(b => b.Info.Type == BoneType.Jaw);
            if (jawBone != null)
            {
                yield return new Bone(jawBone)
                {
                    Info = new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, -0.75f, 1f),
                        RotationOffset = Quaternion.Euler(270f, 0f, 0f),
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
