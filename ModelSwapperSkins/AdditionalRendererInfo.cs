using ModelSwapperSkins.Utils;
using System;
using UnityEngine;

namespace ModelSwapperSkins
{
    public class AdditionalRendererInfoProvider : MonoBehaviour
    {
        public AdditionalRendererInfo AdditionalRendererInfo;

        public static void AddMaterials(Renderer renderer, Material[] additionalMaterials)
        {
            if (renderer.TryGetComponent(out AdditionalRendererInfoProvider rendererInfoProvider))
            {
                ArrayUtil.Append(ref rendererInfoProvider.AdditionalRendererInfo.Materials, additionalMaterials);
            }
            else
            {
                rendererInfoProvider = renderer.gameObject.AddComponent<AdditionalRendererInfoProvider>();
                rendererInfoProvider.AdditionalRendererInfo.Materials = additionalMaterials;
            }
        }
    }

    [Serializable]
    public struct AdditionalRendererInfo
    {
        public Material[] Materials;
    }
}
