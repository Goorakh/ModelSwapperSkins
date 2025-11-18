using HG;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Drifter : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Drifter Instance { get; } = new BoneInitializerRules_Drifter();

        BoneInitializerRules_Drifter()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("DrifterBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "shoulder_l":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        PositionOffset = new Vector3(-0.3f, 0f, 0f),
                        RotationOffset = Quaternion.Euler(90f, 90f, 0f),
                    };
                case "shoulder_r":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        PositionOffset = new Vector3(0.3f, 0f, 0f),
                        RotationOffset = Quaternion.Euler(270f, 90f, 0f),
                    };
                default:
                    BoneInfo boneInfo = base.getBoneInfo(modelTransform, potentialBoneTransform);

                    switch (boneInfo.Type)
                    {
                        case BoneType.Neck1:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 180f);
                            break;
                        case BoneType.Head:
                        case BoneType.Chest:
                            boneInfo.RotationOffset *= Quaternion.Euler(270f, 90f, 0f);
                            break;
                        case BoneType.ArmLowerL:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 0f, 270f);
                            break;
                        case BoneType.HandL:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                            break;
                        case BoneType.ArmLowerR:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 270f);
                            break;
                        case BoneType.HandR:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 270f, 180f);
                            break;
                        case BoneType.Stomach:
                            boneInfo.RotationOffset *= Quaternion.Euler(270f, 90f, 0f);
                            break;
                        case BoneType.Pelvis:
                            boneInfo.RotationOffset *= Quaternion.Euler(90f, 90f, 0f);
                            break;
                        case BoneType.LegUpperL:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                            boneInfo.Scale *= 0.6f;
                            boneInfo.MatchFlags = BoneMatchFlags.AllowMatchTo;
                            break;
                        case BoneType.LegLowerL:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 0f, 270f);
                            boneInfo.Scale *= 0.8f;
                            boneInfo.MatchFlags = BoneMatchFlags.AllowMatchTo;
                            break;
                        case BoneType.FootL:
                            boneInfo.RotationOffset *= Quaternion.Euler(270f, 270f, 0f);
                            break;
                        case BoneType.Toe1L:
                            boneInfo.RotationOffset *= Quaternion.Euler(90f, 90f, 0f);
                            break;
                        case BoneType.LegUpperR:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 0f, 180f);
                            boneInfo.Scale *= 0.6f;
                            boneInfo.MatchFlags = BoneMatchFlags.AllowMatchTo;
                            break;
                        case BoneType.LegLowerR:
                            boneInfo.RotationOffset *= Quaternion.Euler(0f, 180f, 270f);
                            boneInfo.Scale *= 0.8f;
                            boneInfo.MatchFlags = BoneMatchFlags.AllowMatchTo;
                            break;
                        case BoneType.FootR:
                            boneInfo.RotationOffset *= Quaternion.Euler(90f, 270f, 0f);
                            break;
                        case BoneType.Toe1R:
                            boneInfo.RotationOffset *= Quaternion.Euler(270f, 90f, 0f);
                            break;
                    }

                    return boneInfo;
            }
        }

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;   
            }

            BoneType[] legBoneTypes = [BoneType.LegUpperR, BoneType.LegLowerR, BoneType.LegUpperL, BoneType.LegLowerL];
            foreach (BoneType boneType in legBoneTypes)
            {
                Bone bone = existingBones.Find(b => b.Info.Type == boneType);
                if (bone != null)
                {
                    yield return new Bone(bone)
                    {
                        Info = new BoneInfo(boneType)
                        {
                            PositionOffset = bone.Info.PositionOffset,
                            RotationOffset = bone.Info.RotationOffset,
                            Scale = 1f,
                            MatchFlags = BoneMatchFlags.MatchToOther,
                            ExclusionRules = ArrayUtils.Clone(bone.Info.ExclusionRules),
                        }
                    };
                }
                else
                {
                    Log.Warning($"Failed to find leg bone {boneType}");
                }
            }
        }
    }
}
