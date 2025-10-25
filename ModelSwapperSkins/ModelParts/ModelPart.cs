using ModelSwapperSkins.Utils;
using System;
using UnityEngine;

namespace ModelSwapperSkins.ModelParts
{
    [Serializable]
    public sealed class ModelPart
    {
        public ModelPartFlags Flags;
        public Transform Transform;
        public string Path;

        public ModelPartRendererInfo? RendererInfo;

        public ModelPart(Transform transform, Transform root, ModelPartFlags type, ModelPartRendererInfo? rendererInfo) : this(transform, type, TransformUtils.GetObjectPath(transform, root), rendererInfo)
        {
            RendererInfo = rendererInfo;
        }

        public ModelPart(Transform transform, ModelPartFlags type, string path, ModelPartRendererInfo? rendererInfo)
        {
            Flags = type;
            Transform = transform;
            Path = path;
            RendererInfo = rendererInfo;
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

        public override string ToString()
        {
            return $"{Path} ({Flags})";
        }
    }
}
