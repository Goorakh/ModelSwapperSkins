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

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;
            }

            Bone chestBone = existingBones.Find(b => b.Info.Type == BoneType.Neck1);
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
        }
    }
}
