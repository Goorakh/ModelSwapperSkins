using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Acrid : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Acrid Instance = new BoneInitializerRules_Acrid();

        protected BoneInitializerRules_Acrid() : base()
        {
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo boneInfo = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (boneInfo.Type)
            {
                case BoneType.Stomach:
                case BoneType.Chest:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case BoneType.Head:
                    boneInfo.RotationOffset = Quaternion.Euler(270f, 180f, 0f);
                    break;
                case BoneType.FootL:
                case BoneType.FootR:
                    boneInfo.RotationOffset = Quaternion.Euler(290f, 0f, 0f);
                    break;
            }

            return boneInfo;
        }
    }
}
