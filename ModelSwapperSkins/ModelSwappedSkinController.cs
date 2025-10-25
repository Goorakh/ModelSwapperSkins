using UnityEngine;

namespace ModelSwapperSkins
{
    public sealed class ModelSwappedSkinController : MonoBehaviour
    {
        public ModelSwappedSkinDef AppliedSkin;
        public GameObject SkinModelObject;

        void OnDestroy()
        {
            if (SkinModelObject)
            {
                Destroy(SkinModelObject);
            }
        }
    }
}