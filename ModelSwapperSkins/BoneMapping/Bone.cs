using System;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    [Serializable]
    public class Bone
    {
        public BoneInfo Info;
        public Transform BoneTransform;
        public string ModelPath;

        public Bone MakeCopyFor(BonesProvider displayBonesProvider)
        {
            return new Bone
            {
                Info = Info,
                ModelPath = ModelPath,
                BoneTransform = displayBonesProvider.transform.Find(ModelPath)
            };
        }
    }
}
