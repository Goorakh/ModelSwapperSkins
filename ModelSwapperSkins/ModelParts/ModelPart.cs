using ModelSwapperSkins.Utils;
using System;
using UnityEngine;

namespace ModelSwapperSkins.ModelParts
{
    [Serializable]
    public class ModelPart
    {
        public ModelPartFlags Flags;
        public Transform Transform;
        public string Path;

        public ModelPart(Transform transform, Transform root, ModelPartFlags type) : this(transform, type, TransformUtils.GetObjectPath(transform, root))
        {
        }

        public ModelPart(Transform transform, ModelPartFlags type, string path)
        {
            Flags = type;
            Transform = transform;
            Path = path;
        }

        public bool ShouldShow(bool isMainModel)
        {
            if (isMainModel)
            {
                return (Flags & ModelPartFlags.ShowForMain) != 0;
            }
            else
            {
                return (Flags & ModelPartFlags.ShowForSkin) != 0;
            }
        }
    }
}
