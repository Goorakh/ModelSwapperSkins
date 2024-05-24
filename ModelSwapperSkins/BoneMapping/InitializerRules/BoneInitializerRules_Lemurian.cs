﻿using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Lemurian : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Lemurian Instance { get; } = new BoneInitializerRules_Lemurian();

        BoneInitializerRules_Lemurian() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("LemurianBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Stomach:
                    case BoneType.Chest:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.Head:
                        bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                        break;
                    case BoneType.Jaw:
                        bone.RotationOffset *= Quaternion.Euler(330f, 0f, 0f);
                        break;
                    case BoneType.ArmUpperL:
                        bone.PositionOffset += new Vector3(0f, -0.15f, 0f);
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.ArmUpperR:
                        bone.PositionOffset += new Vector3(0f, -0.15f, 0f);
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.Toe1L:
                    case BoneType.Toe1R:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                }

                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                case "neck_low":
                    return new BoneInfo(BoneType.Neck1);
                case "neck_high":
                    return new BoneInfo(BoneType.Neck2);
                default:
                    return bone;
            }
        }
    }
}
