using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Railgunner : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Railgunner Instance { get; } = new BoneInitializerRules_Railgunner();

        BoneInitializerRules_Railgunner() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("RailgunnerBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.LegLowerL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.Toe1L:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.LegLowerR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1R:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.Stomach:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    bone.Scale *= 0.7f;
                    break;
                case BoneType.Chest:
                    bone.PositionOffset += new Vector3(0f, -0.15f, 0f);
                    bone.Scale *= 0.7f;
                    break;
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, -0.125f, 0f);
                    break;
            }

            return bone;
        }
    }
}
