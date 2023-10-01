using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    public class BonesProvider : MonoBehaviour
    {
        [SerializeField]
        BoneType[] _providedBoneTypes;

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
                _providedBoneTypes = value.Select(b => b.Info.Type).Distinct().OrderBy(b => b).ToArray();
            }
        }

        public bool HasBone(BoneType boneType)
        {
            return Array.BinarySearch(_providedBoneTypes, boneType) >= 0;
        }

        public int GetNumMatchingBones(BonesProvider other)
        {
            return _providedBoneTypes.Count(other.HasBone);
        }

        public void MapBonesTo(BonesProvider other)
        {
            List<MatchBoneTransform> boneMatches = new List<MatchBoneTransform>();

            foreach (Bone bone in Bones)
            {
                if ((bone.Info.MatchFlags & BoneMatchFlags.MatchToOther) == 0)
                    continue;

                Bone matchingBone = other.Bones.FirstOrDefault(b => (b.Info.MatchFlags & BoneMatchFlags.AllowMatchTo) != 0 && b.Info.Type == bone.Info.Type);
                if (matchingBone == null)
                    continue;

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
