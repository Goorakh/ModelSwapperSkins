using RoR2;
using System.Collections.Generic;

namespace ModelSwapperSkins.Utils.Comparers
{
    public sealed class GameObjectActivationObjectComparer : IEqualityComparer<SkinDefParams.GameObjectActivation>
    {
        public static GameObjectActivationObjectComparer Instance { get; } = new GameObjectActivationObjectComparer();

        GameObjectActivationObjectComparer()
        {
        }

        public bool Equals(SkinDefParams.GameObjectActivation x, SkinDefParams.GameObjectActivation y)
        {
            return x.gameObject == y.gameObject;
        }

        public int GetHashCode(SkinDefParams.GameObjectActivation obj)
        {
            return obj.gameObject.GetHashCode();
        }
    }
}
