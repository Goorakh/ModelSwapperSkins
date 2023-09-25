using System;
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
            foreach (Bone bone in Bones)
            {
                if ((bone.Info.MatchFlags & BoneMatchFlags.MatchToOther) == 0 || !other.HasBone(bone.Info.Type))
                    continue;

                Bone matchingBone = other.Bones.First(b => (b.Info.MatchFlags & BoneMatchFlags.AllowMatchTo) != 0 && b.Info.Type == bone.Info.Type);

                MatchBoneTransform matchBoneTransform = bone.BoneTransform.gameObject.AddComponent<MatchBoneTransform>();
                matchBoneTransform.Bone = bone;
                matchBoneTransform.TargetBone = matchingBone;
            }
        }

        public void CopyTo(BonesProvider other)
        {
            other.Bones = Bones.Select(b => b.MakeCopyFor(other)).Where(b => b.BoneTransform).ToArray();
        }
    }
}
