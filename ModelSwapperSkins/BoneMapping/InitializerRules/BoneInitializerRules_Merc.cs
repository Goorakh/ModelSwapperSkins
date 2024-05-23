using HG;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Merc : BoneInitializerRules_AutoName
    {
        static readonly SkinDef _prisonerSkin = Addressables.LoadAssetAsync<SkinDef>("RoR2/Base/Merc/skinMercAltPrisoner.asset").WaitForCompletion();

        public static new BoneInitializerRules_Merc Instance { get; } = new BoneInitializerRules_Merc();

        BoneInitializerRules_Merc() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("MercBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            if (bone.Type == BoneType.Root)
            {
                // Prisoner skin cloths have their own armature with a ROOT bone for some reason, exclude those
                Transform boneParent = potentialBoneTransform.parent;
                if (boneParent && boneParent.name != "MercArmature")
                {
                    return BoneInfo.None;
                }
            }

            switch (bone.Type)
            {
                case BoneType.Toe1L:
                    bone.RotationOffset *= Quaternion.Euler(0f, 90f, 0f);
                    break;
                case BoneType.Toe1R:
                    bone.RotationOffset *= Quaternion.Euler(0f, 270f, 0f);
                    break;
                case BoneType.Head:
                case BoneType.Neck1:
                case BoneType.Neck2:
                case BoneType.Neck3:
                case BoneType.Neck4:
                case BoneType.Neck5:
                case BoneType.Neck6:
                case BoneType.Neck7:
                case BoneType.Neck8:
                case BoneType.Neck9:
                case BoneType.Neck10:
                case BoneType.Neck11:
                case BoneType.Neck12:
                case BoneType.Neck13:
                case BoneType.Neck14:
                case BoneType.Neck15:
                case BoneType.Neck16:
                    ArrayUtils.ArrayAppend(ref bone.ExclusionRules, new BoneExclusionRule(_prisonerSkin, ModelSkinExclusionRuleType.ExcludeIfApplied));
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

            IEnumerable<Bone> tryCreatePrisonerBones(BoneType boneType)
            {
                Bone baseBone = existingBones.Find(b => b.Info.Type == boneType);
                if (baseBone == null)
                    yield break;

                yield return new Bone(baseBone)
                {
                    Info = new BoneInfo(boneType)
                    {
                        PositionOffset = baseBone.Info.PositionOffset,
                        RotationOffset = baseBone.Info.RotationOffset,
                        Scale = 0.01f,
                        MatchFlags = BoneMatchFlags.AllowMatchTo,
                        ExclusionRules = [
                            new BoneExclusionRule(_prisonerSkin, ModelSkinExclusionRuleType.ExcludeIfNotApplied)
                        ]
                    }
                };

                yield return new Bone(baseBone)
                {
                    Info = new BoneInfo(boneType)
                    {
                        PositionOffset = baseBone.Info.PositionOffset,
                        RotationOffset = baseBone.Info.RotationOffset,
                        MatchFlags = BoneMatchFlags.MatchToOther,
                        ExclusionRules = [
                            new BoneExclusionRule(_prisonerSkin, ModelSkinExclusionRuleType.ExcludeIfNotApplied)
                        ]
                    }
                };
            }

            foreach (Bone prisonerHeadBone in tryCreatePrisonerBones(BoneType.Head))
            {
                yield return prisonerHeadBone;
            }

            for (BoneType boneType = BoneType.Neck1; boneType <= BoneType.Neck16; boneType++)
            {
                foreach (Bone prisonerNeckBone in tryCreatePrisonerBones(boneType))
                {
                    yield return prisonerNeckBone;
                }
            }
        }
    }
}
