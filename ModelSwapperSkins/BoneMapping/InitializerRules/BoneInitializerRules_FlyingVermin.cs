﻿using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_FlyingVermin : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_FlyingVermin Instance { get; } = new BoneInitializerRules_FlyingVermin();

        BoneInitializerRules_FlyingVermin() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("FlyingVerminBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.FootL:
                        bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                        break;
                    case BoneType.FootR:
                        bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                        break;
                }

                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                case "Body":
                    return new BoneInfo(BoneType.Chest);
                case "Wing1.l":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "Wing2.l":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "Wing3.l":
                    return new BoneInfo(BoneType.HandL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Wing1.r":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "Wing2.r":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "Wing3.r":
                    return new BoneInfo(BoneType.HandR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                default:
                    return bone;
            }
        }
    }
}
