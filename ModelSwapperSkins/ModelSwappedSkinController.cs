using UnityEngine;

namespace ModelSwapperSkins
{
    public class ModelSwappedSkinController : MonoBehaviour
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