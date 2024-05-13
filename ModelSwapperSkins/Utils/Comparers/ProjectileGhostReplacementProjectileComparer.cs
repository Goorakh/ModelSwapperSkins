using RoR2;
using System.Collections.Generic;

namespace ModelSwapperSkins.Utils.Comparers
{
    public class ProjectileGhostReplacementProjectileComparer : IEqualityComparer<SkinDef.ProjectileGhostReplacement>
    {
        public static ProjectileGhostReplacementProjectileComparer Instance { get; } = new ProjectileGhostReplacementProjectileComparer();

        ProjectileGhostReplacementProjectileComparer()
        {
        }

        public bool Equals(SkinDef.ProjectileGhostReplacement x, SkinDef.ProjectileGhostReplacement y)
        {
            return x.projectilePrefab == y.projectilePrefab;
        }

        public int GetHashCode(SkinDef.ProjectileGhostReplacement obj)
        {
            return obj.projectilePrefab.GetHashCode();
        }
    }
}
