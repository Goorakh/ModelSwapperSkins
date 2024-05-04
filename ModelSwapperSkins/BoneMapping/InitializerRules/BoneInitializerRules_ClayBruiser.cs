using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_ClayBruiser : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_ClayBruiser Instance { get; } = new BoneInitializerRules_ClayBruiser();

        BoneInitializerRules_ClayBruiser() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ClayBruiserBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, -0.05f, 0.05f);
                    break;
                case BoneType.Toe1L:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1R:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
            }

            return bone;
        }
    }
}
