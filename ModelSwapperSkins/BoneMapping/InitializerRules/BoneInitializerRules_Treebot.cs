using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_Treebot : BoneInitializerRules
    {
        public static readonly BoneInitializerRules_Treebot Instance = new BoneInitializerRules_Treebot();

        protected BoneInitializerRules_Treebot() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("TreebotBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "Base":
                    return new BoneInfo(BoneType.Stomach);
                case "Calf.Back.l":
                    return new BoneInfo(BoneType.LegUpperL);
                case "Foot.Back.l":
                    return new BoneInfo(BoneType.LegLowerL);
                case "Calf.Back.r":
                    return new BoneInfo(BoneType.LegUpperR);
                case "Foot.Back.r":
                    return new BoneInfo(BoneType.LegLowerR);
                case "Thigh.Front.l":
                    return new BoneInfo(BoneType.ShoulderL);
                case "Calf.Front.l":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "Foot.Front.l":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "Thigh.Front.r":
                    return new BoneInfo(BoneType.ShoulderR);
                case "Calf.Front.r":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "Foot.Front.r":
                    return new BoneInfo(BoneType.ArmLowerR);
                default:
                    return BoneInfo.None;
            }
        }
    }
}
