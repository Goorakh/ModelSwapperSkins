using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Scav : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Scav Instance { get; } = new BoneInitializerRules_Scav();

        BoneInitializerRules_Scav() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ScavBody") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("ScavLunar1Body") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("ScavLunar2Body") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("ScavLunar3Body") ||
                   bodyIndex == BodyCatalog.FindBodyIndex("ScavLunar4Body");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);
            if (bone.Type != BoneType.None)
            {
                switch (bone.Type)
                {
                    case BoneType.Stomach:
                    case BoneType.Chest:
                    case BoneType.ShoulderL:
                    case BoneType.ArmUpperL:
                    case BoneType.ArmLowerL:
                    case BoneType.ShoulderR:
                    case BoneType.ArmUpperR:
                    case BoneType.ArmLowerR:
                        bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                        break;
                }

                return bone;
            }

            return bone;
        }

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;
            }

            Bone chestBone = existingBones.Find(b => b.Info.Type == BoneType.Chest);
            if (chestBone != null)
            {
                yield return new Bone(chestBone)
                {
                    Info = new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, 7.5f, -2f),
                        RotationOffset = Quaternion.Euler(0f, 180f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Missing Chest bone");
            }
        }
    }
}
