using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_ImpBoss : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_ImpBoss Instance { get; } = new BoneInitializerRules_ImpBoss();

        BoneInitializerRules_ImpBoss() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ImpBossBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Chest:
                    bone.PositionOffset += new Vector3(0f, -1f, 0f);
                    break;
                case BoneType.Stomach:
                    bone.PositionOffset += new Vector3(0f, -0.5f, 0f);
                    break;
                case BoneType.Pelvis:
                    bone.PositionOffset += new Vector3(0f, 0.5f, 0f);
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

            Bone chestBone = existingBones.Find(b => b.Info.Type == BoneType.Chest);
            if (chestBone != null)
            {
                yield return new Bone(chestBone)
                {
                    Info = new BoneInfo(BoneType.Head)
                    {
                        PositionOffset = new Vector3(0f, 2.25f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Failed to find chest bone");
            }
        }
    }
}
