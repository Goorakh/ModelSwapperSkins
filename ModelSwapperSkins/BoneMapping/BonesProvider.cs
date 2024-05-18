using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    public class BonesProvider : MonoBehaviour
    {
        [SerializeField]
        BoneType[] _matchableBoneTypes;

        [SerializeField]
        BoneType[] _canMatchToBoneTypes;

        [SerializeField]
        Bone[] _bones;

        public Bone[] Bones
        {
            get
            {
                return _bones;
            }
            set
            {
                _bones = value;

                _matchableBoneTypes = value.Where(b => (b.Info.MatchFlags & BoneMatchFlags.AllowMatchTo) != 0).Select(b => b.Info.Type).Distinct().OrderBy(b => b).ToArray();
                _canMatchToBoneTypes = value.Where(b => (b.Info.MatchFlags & BoneMatchFlags.MatchToOther) != 0).Select(b => b.Info.Type).Distinct().OrderBy(b => b).ToArray();
            }
        }

        public bool HasMatchForBone(BoneType boneType)
        {
            return Array.BinarySearch(_matchableBoneTypes, boneType) >= 0;
        }

        public bool CanMatchToBone(BoneType boneType)
        {
            return Array.BinarySearch(_canMatchToBoneTypes, boneType) >= 0;
        }

        public void MapBonesTo(BonesProvider other)
        {
            List<MatchBoneTransform> boneMatches = [];

            foreach (Bone bone in Bones)
            {
                if ((bone.Info.MatchFlags & BoneMatchFlags.MatchToOther) == 0)
                    continue;

                if (bone.Info.ShouldExcludeMatch(this, other))
                    continue;

                Bone matchingBone = other.Bones.FirstOrDefault(b => (b.Info.MatchFlags & BoneMatchFlags.AllowMatchTo) != 0 && b.Info.Type == bone.Info.Type && !b.Info.ShouldExcludeMatch(other, this));
                if (matchingBone == null)
                    continue;

                if (bone.BoneTransform.TryGetComponent(out MatchBoneTransform existingBoneMatcher))
                {
                    Log.Warning($"Duplicate bone match for {bone.ModelPath}, attempting to match {matchingBone.ModelPath} ({matchingBone.Info.Type}), but {existingBoneMatcher.TargetBone.ModelPath} ({existingBoneMatcher.TargetBone.Info.Type})");
                    continue;
                }

                MatchBoneTransform matchBoneTransform = bone.BoneTransform.gameObject.AddComponent<MatchBoneTransform>();
                matchBoneTransform.Bone = bone;
                matchBoneTransform.TargetBone = matchingBone;

                boneMatches.Add(matchBoneTransform);
            }

            foreach (MatchBoneTransform matchBone in boneMatches)
            {
                Transform boneTransform = matchBone.Bone?.BoneTransform;
                if (!boneTransform)
                    continue;

                Transform boneParent = boneTransform.parent;
                if (!boneParent)
                    continue;

                MatchBoneTransform parentBoneMatch = boneParent.GetComponentInParent<MatchBoneTransform>();
                if (parentBoneMatch)
                {
                    matchBone.ParentBone = parentBoneMatch;
                }
            }
        }

        public void CopyTo(BonesProvider other)
        {
            other.Bones = Bones.Select(b => b.MakeCopyFor(other)).Where(b => b.BoneTransform).ToArray();
        }
    }
}
