using RoR2;
using System.Collections.Generic;

namespace ModelSwapperSkins.Utils.Comparers
{
    public class MinionSkinReplacementBodyComparer : IEqualityComparer<SkinDef.MinionSkinReplacement>
    {
        public static MinionSkinReplacementBodyComparer Instance { get; } = new MinionSkinReplacementBodyComparer();

        MinionSkinReplacementBodyComparer()
        {
        }

        public bool Equals(SkinDef.MinionSkinReplacement x, SkinDef.MinionSkinReplacement y)
        {
            return x.minionBodyPrefab == y.minionBodyPrefab;
        }

        public int GetHashCode(SkinDef.MinionSkinReplacement obj)
        {
            return obj.minionBodyPrefab.GetHashCode();
        }
    }
}
