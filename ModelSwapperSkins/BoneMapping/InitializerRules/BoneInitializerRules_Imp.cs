using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Imp : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Imp Instance { get; } = new BoneInitializerRules_Imp();

        BoneInitializerRules_Imp() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ImpBody");
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
                        PositionOffset = new Vector3(0f, 0.35f, 0f),
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
