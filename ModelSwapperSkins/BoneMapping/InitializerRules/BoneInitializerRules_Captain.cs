using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Captain : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Captain Instance { get; } = new BoneInitializerRules_Captain();

        BoneInitializerRules_Captain() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("CaptainBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0f, -0.075f, 0f);
                    break;
                case BoneType.Toe1L:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1R:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.HandL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 0f);
                    bone.MatchFlags = BoneMatchFlags.MatchToOther;
                    break;
                case BoneType.HandR:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
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

            Bone handLBone = existingBones.Find(b => b.Info.Type == BoneType.HandL);
            if (handLBone != null)
            {
                yield return new Bone(handLBone)
                {
                    Info = new BoneInfo(BoneType.HandL)
                    {
                        Scale = 0.01f,
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
        }
    }
}
