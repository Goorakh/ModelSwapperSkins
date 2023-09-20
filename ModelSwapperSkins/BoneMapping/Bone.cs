using System;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    [Serializable]
    public class Bone
    {
        public BoneType Type;
        public Transform BoneTransform;
        public string ModelPath;

        public Bone MakeCopyFor(BonesProvider displayBonesProvider)
        {
            return new Bone
            {
                Type = Type,
                ModelPath = ModelPath,
                BoneTransform = displayBonesProvider.transform.Find(ModelPath)
            };
        }
    }
}
