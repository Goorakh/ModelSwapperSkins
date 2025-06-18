using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Halcyonite : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Halcyonite Instance { get; } = new BoneInitializerRules_Halcyonite();

        BoneInitializerRules_Halcyonite()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("HalcyoniteBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "L_Clav":
                    return new BoneInfo(BoneType.ShoulderL);

                case "R_Clav":
                    return new BoneInfo(BoneType.ShoulderR)
                    {
                        PositionOffset = new Vector3(-0.6988f, 0.7227f, -0.6836f),
                        RotationOffset = Quaternion.Euler(312.6912f, 192.4575f, 353.5174f)
                    };

                case "Spine1":
                    return new BoneInfo(BoneType.Base);

                case "Spine2":
                    return new BoneInfo(BoneType.Stomach);

                case "Spine3":
                    return new BoneInfo(BoneType.Chest);
            }

            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Pelvis:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 180f);
                    break;
            }

            return bone;
        }

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;
            }

            Bone shoulderLBone = existingBones.Find(b => b.Info.Type == BoneType.ShoulderL);
            if (shoulderLBone != null)
            {
                yield return new Bone(shoulderLBone)
                {
                    Info = new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(-0.0918f, 0.1469f, 1.6698f),
                        RotationOffset = Quaternion.Euler(350.1404f, 62.4493f, 236.1282f),
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [new BoneExclusionRule([BoneType.ShoulderL], OtherBoneMatchExclusionRuleType.ExcludeIfAllMatch)]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find ShoulderL bone");
            }

            Bone shoulderRBone = existingBones.Find(b => b.Info.Type == BoneType.ShoulderR);
            if (shoulderRBone != null)
            {
                yield return new Bone(shoulderRBone)
                {
                    Info = new BoneInfo(BoneType.Chest)
                    {
                        PositionOffset = new Vector3(0f, 1.8988f, -1.6281f),
                        RotationOffset = Quaternion.Euler(3.4896f, 89.7881f, 175.8213f),
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [new BoneExclusionRule([BoneType.ShoulderR], OtherBoneMatchExclusionRuleType.ExcludeIfAllMatch)]
                    }
                };
            }
            else
            {
                Log.Error("Failed to find ShoulderR bone");
            }
        }
    }
}
