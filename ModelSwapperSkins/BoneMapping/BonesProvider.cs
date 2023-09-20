using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                _providedBoneTypes = value.Select(b => b.Type).Distinct().ToArray();
            }
        }

        public int GetNumMatchingBones(BonesProvider other)
        {
            return _providedBoneTypes.Count(b => other._providedBoneTypes.Contains(b));
        }

        public void MapBonesTo(BonesProvider other)
        {
            foreach (Bone bone in Bones)
            {
                Bone matchingBone = other.Bones.FirstOrDefault(b => b.Type == bone.Type);
                if (matchingBone == null)
                    continue;

                SyncTransform syncTransform = bone.BoneTransform.gameObject.AddComponent<SyncTransform>();
                syncTransform.Target = matchingBone.BoneTransform;
            }
        }

        public void CopyTo(BonesProvider other)
        {
            other.Bones = Bones.Select(b => b.MakeCopyFor(other)).Where(b => b.BoneTransform).ToArray();
        }
    }
}
