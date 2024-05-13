using RoR2;
using System.Collections.Generic;

namespace ModelSwapperSkins.Utils.Comparers
{
    public class RendererInfoRendererComparer : IEqualityComparer<CharacterModel.RendererInfo>
    {
        public static RendererInfoRendererComparer Instance { get; } = new RendererInfoRendererComparer();

        RendererInfoRendererComparer()
        {
        }

        public bool Equals(CharacterModel.RendererInfo x, CharacterModel.RendererInfo y)
        {
            return x.renderer == y.renderer;
        }

        public int GetHashCode(CharacterModel.RendererInfo obj)
        {
            return obj.renderer.GetHashCode();
        }
    }
}
