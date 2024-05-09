using ModelSwapperSkins.Utils;
using System;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    [Serializable]
    public class Bone : ICloneable
    {
        public BoneInfo Info;
        public Transform BoneTransform;
        public string ModelPath;

        public Bone(BoneInfo info, Transform boneTransform, string modelPath)
        {
            Info = info;
            BoneTransform = boneTransform;
            ModelPath = modelPath;
        }

        public Bone(Bone other) : this(other.Info, other.BoneTransform, other.ModelPath)
        {
        }

        public Bone(BoneInfo info, string modelPath, Transform rootTransform) : this(info, rootTransform.Find(modelPath), modelPath)
        {
        }

        public Bone(BoneInfo info, Transform rootTransform, Transform boneTransform) : this(info, boneTransform, TransformUtils.GetObjectPath(boneTransform, rootTransform))
        {
        }

        public Bone MakeCopyFor(BonesProvider displayBonesProvider)
        {
            return new Bone(Info, ModelPath, displayBonesProvider.transform);
        }

        public Bone Clone()
        {
            return new Bone(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public override string ToString()
        {
            return $"{Info.Type} ({Info.MatchFlags}): {ModelPath}";
        }
    }
}
