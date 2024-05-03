﻿using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Vermin : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Vermin Instance { get; } = new BoneInitializerRules_Vermin();

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
                            RotationOffset = Quaternion.Euler(0f, 180f, 180f)
                        };
                    case "Spine2":
                        return new BoneInfo(BoneType.Stomach);
                    case "Spine3":
                        return new BoneInfo(BoneType.Chest);
                    case "Leg1.l":
                        return new BoneInfo(BoneType.LegLowerL)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                    case "Leg1.r":
                        return new BoneInfo(BoneType.LegLowerR)
                        {
                            RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                        };
                }
            }

            switch (bone.Type)
            {
                case BoneType.Base:
                    bone.RotationOffset *= Quaternion.Euler(330f, 180f, 0f);
                    break;
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, -0.3f, 0f);
                    bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                    break;
                case BoneType.Jaw:
                    bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                    break;
            }

            return bone;
        }
    }
}