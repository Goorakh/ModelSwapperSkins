using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_FalseSon : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_FalseSon Instance { get; } = new BoneInitializerRules_FalseSon();

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("FalseSonBody") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("FalseSonBossBody") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("FalseSonBossBodyLunarShard") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("FalseSonBossBodyBrokenLunarShard");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Neck1:
                        bone.RotationOffset *= Quaternion.Euler(270f, 0f, 0f);
                        break;
                    case BoneType.ShoulderR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        bone.PositionOffset += new Vector3(0f, 0.2f, 0f);
                        break;
                    case BoneType.ShoulderL:
                        bone.PositionOffset += new Vector3(0f, 0.2f, 0f);
                        break;
                    case BoneType.ArmUpperR:
                    case BoneType.ArmUpperL:
                        bone.PositionOffset += new Vector3(0f, -0.25f, 0f);
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.ArmLowerR:
                    case BoneType.HandL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case BoneType.LegUpperL:
                    case BoneType.LegUpperR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.LegLowerL:
                    case BoneType.LegLowerR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.FootR:
                    case BoneType.FootL:
                        bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                        break;
                    case BoneType.Toe1L:
                    case BoneType.Toe1R:
                        bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                        break;
                    case BoneType.Pelvis:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 180f);
                        break;
                }

                return bone;
            }

            switch (potentialBoneTransform.name)
            {
                case "SpineEnd":
                    return new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, -0.7f, 0f)
                    };
                case "Spine2":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, -0.3f, 0f)
                    };
                case "Spine1":
                    return new BoneInfo(BoneType.Base);
            }

            return BoneInfo.None;
        }
    }
}
