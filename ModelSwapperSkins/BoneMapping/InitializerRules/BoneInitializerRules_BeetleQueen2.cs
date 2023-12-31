﻿using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_BeetleQueen2 : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_BeetleQueen2 Instance = new BoneInitializerRules_BeetleQueen2();

        protected BoneInitializerRules_BeetleQueen2()
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
                case BoneType.Stomach:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 180f);
                    break;
                case BoneType.Chest:
                case BoneType.Head:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    break;
            }

            return bone;
        }
    }
}
