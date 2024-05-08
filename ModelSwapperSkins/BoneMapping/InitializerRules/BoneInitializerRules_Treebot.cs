using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Treebot : BoneInitializerRules
    {
        public static BoneInitializerRules_Treebot Instance { get; } = new BoneInitializerRules_Treebot();

        BoneInitializerRules_Treebot() : base()
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
                case "ROOT":
                    return new BoneInfo(BoneType.Root);
                case "Base":
                    return new BoneInfo(BoneType.Base);
                case "PlatformBase":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, -0.15f, 0f),
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f)
                    };
                case "HeadBase":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        PositionOffset = new Vector3(0f, -0.15f, 0f),
                        RotationOffset = Quaternion.Euler(270f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.MatchToOther
                    };
                case "Calf.Back.l":
                    return new BoneInfo(BoneType.LegUpperL);
                case "Foot.Back.l":
                    return new BoneInfo(BoneType.LegLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
                case "Calf.Back.r":
                    return new BoneInfo(BoneType.LegUpperR);
                case "Foot.Back.r":
                    return new BoneInfo(BoneType.LegLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f)
                    };
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

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;
            }

            Bone stomachBone = existingBones.Find(b => b.Info.Type == BoneType.Stomach && (b.Info.MatchFlags & BoneMatchFlags.MatchToOther) != 0);
            if (stomachBone != null)
            {
                Bone stomachBoneTemplate = stomachBone.Clone();

                {
                    Bone fakePelvisBone = stomachBoneTemplate.Clone();
                    fakePelvisBone.Info = new BoneInfo(BoneType.Pelvis)
                    {
                        PositionOffset = new Vector3(0f, 0.2f, 0f),
                        RotationOffset = Quaternion.Euler(90f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    };

                    yield return fakePelvisBone;
                }

                {
                    Bone fakeChestBone = stomachBoneTemplate.Clone();
                    fakeChestBone.Info = new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, 0.2f, -0.3f),
                        RotationOffset = Quaternion.Euler(270f, 0f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    };

                    yield return fakeChestBone;
                }

                {
                    Bone fakeHeadBone = stomachBoneTemplate.Clone();
                    fakeHeadBone.Info = new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, 0.3f, -1.3f),
                        RotationOffset = Quaternion.Euler(320f, 180f, 180f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    };

                    yield return fakeHeadBone;
                }
            }
            else
            {
                Log.Error("Missing stomach bone");
            }
        }
    }
}
