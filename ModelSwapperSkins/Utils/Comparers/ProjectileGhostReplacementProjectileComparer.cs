using RoR2;
using System.Collections.Generic;

namespace ModelSwapperSkins.Utils.Comparers
{
    public sealed class ProjectileGhostReplacementProjectileComparer : IEqualityComparer<SkinDefParams.ProjectileGhostReplacement>
    {
        public static ProjectileGhostReplacementProjectileComparer Instance { get; } = new ProjectileGhostReplacementProjectileComparer();

        ProjectileGhostReplacementProjectileComparer()
        {
        }

        public bool Equals(SkinDefParams.ProjectileGhostReplacement x, SkinDefParams.ProjectileGhostReplacement y)
        {
            return x.projectilePrefab == y.projectilePrefab;
        }

        public int GetHashCode(SkinDefParams.ProjectileGhostReplacement obj)
        {
            return obj.projectilePrefab.GetHashCode();
        }
    }
}
