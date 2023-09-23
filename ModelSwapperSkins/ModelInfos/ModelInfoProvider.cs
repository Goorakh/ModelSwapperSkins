using UnityEngine;

namespace ModelSwapperSkins.ModelInfos
{
    public class ModelInfoProvider : MonoBehaviour
    {
        public ModelInfo ModelInfo;

        public void CopyTo(ModelInfoProvider other)
        {
            other.ModelInfo = ModelInfo;
        }

        public static ModelInfo GetModelInfo(GameObject obj)
        {
            return obj.TryGetComponent(out ModelInfoProvider modelInfoProvider) ? modelInfoProvider.ModelInfo : ModelInfo.Default;
        }
    }
}
