using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Mithrix : BoneInitializerRules_AutoName
    {
        public static new readonly BoneInitializerRules_Mithrix Instance = new BoneInitializerRules_Mithrix();

        protected BoneInitializerRules_Mithrix() : base()
        {
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo boneInfo = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (boneInfo.Type)
            {
                case BoneType.Toe1L:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1R:
                    boneInfo.RotationOffset = Quaternion.Euler(0f, 270f, 0f);
                    break;
            }

            return boneInfo;
        }
    }
}
