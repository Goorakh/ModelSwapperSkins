﻿using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_BeetleQueen2 : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_BeetleQueen2 Instance { get; } = new BoneInitializerRules_BeetleQueen2();

        BoneInitializerRules_BeetleQueen2() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("BeetleQueen2Body");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type == BoneType.None)
            {
                switch (potentialBoneTransform.name)
                {
                    case "backThigh.l":
                        return new BoneInfo(BoneType.LegUpperL);
                    case "backCalf.l":
                        return new BoneInfo(BoneType.LegLowerL);
                    case "backThigh.r":
                        return new BoneInfo(BoneType.LegUpperR);
                    case "backCalf.r":
                        return new BoneInfo(BoneType.LegLowerR);
                    default:
                        return bone;
                }
            }

            switch (bone.Type)
            {
                case BoneType.Neck1:
                    bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                    break;
                case BoneType.Stomach:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 180f);
                    break;
                case BoneType.Chest:
                case BoneType.Head:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.ArmUpperL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.ArmLowerL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.ArmUpperR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.ArmLowerR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
            }

            return bone;
        }
    }
}
