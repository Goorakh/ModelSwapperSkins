using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Engi : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Engi Instance { get; } = new BoneInitializerRules_Engi();

        BoneInitializerRules_Engi() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("EngiBody");
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
                Bone headBone = chestBone.Clone();
                headBone.Info = new BoneInfo(BoneType.Head)
                {
                    PositionOffset = new Vector3(0f, 0.4f, 0f),
                    MatchFlags = BoneMatchFlags.AllowMatchTo
                };

                yield return headBone;
            }
            else
            {
                Log.Warning("Failed to find fake head target bone");
            }
        }
    }
}
