using RoR2;
using System.Collections.Generic;

namespace ModelSwapperSkins.Utils.Comparers
{
    public sealed class MinionSkinReplacementBodyComparer : IEqualityComparer<SkinDefParams.MinionSkinReplacement>
    {
        public static MinionSkinReplacementBodyComparer Instance { get; } = new MinionSkinReplacementBodyComparer();

        MinionSkinReplacementBodyComparer()
        {
        }

        public bool Equals(SkinDefParams.MinionSkinReplacement x, SkinDefParams.MinionSkinReplacement y)
        {
            return x.minionBodyPrefab == y.minionBodyPrefab;
        }

        public int GetHashCode(SkinDefParams.MinionSkinReplacement obj)
        {
            return obj.minionBodyPrefab.GetHashCode();
        }
    }
}
