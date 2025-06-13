using HG;
using ModelSwapperSkins.Utils;
using UnityEngine;

namespace ModelSwapperSkins
{
    public class AdditionalRendererInfoProvider : MonoBehaviour
    {
        public Material[] AdditionalMaterials = [];

        public static void AddMaterials(Renderer renderer, Material[] additionalMaterials)
        {
            AdditionalRendererInfoProvider rendererInfoProvider = renderer.gameObject.EnsureComponent<AdditionalRendererInfoProvider>();
            ArrayUtil.Append(ref rendererInfoProvider.AdditionalMaterials, additionalMaterials);
        }
    }
}
