using System;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins.ModelInfo
{
    public class ModelPartsProvider : MonoBehaviour
    {
        public ModelPart[] Parts = Array.Empty<ModelPart>();

        public void CopyTo(ModelPartsProvider other)
        {
            other.Parts = Parts.Select(p => new ModelPart(other.transform.Find(p.Path), p.Flags, p.Path))
                               .Where(p => p.Transform)
                               .ToArray();
        }
    }
}
