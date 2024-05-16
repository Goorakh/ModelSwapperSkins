using RoR2;
using System.Collections.Generic;

namespace ModelSwapperSkins.Utils.Comparers
{
    public sealed class GameObjectActivationObjectComparer : IEqualityComparer<SkinDef.GameObjectActivation>
    {
        public static GameObjectActivationObjectComparer Instance { get; } = new GameObjectActivationObjectComparer();

        GameObjectActivationObjectComparer()
        {
        }

        public bool Equals(SkinDef.GameObjectActivation x, SkinDef.GameObjectActivation y)
        {
            return x.gameObject == y.gameObject;
        }

        public int GetHashCode(SkinDef.GameObjectActivation obj)
        {
            return obj.gameObject.GetHashCode();
        }
    }
}
