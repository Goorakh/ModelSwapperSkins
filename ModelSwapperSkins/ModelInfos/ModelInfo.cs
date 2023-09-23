using System;

namespace ModelSwapperSkins.ModelInfos
{
    [Serializable]
    public record struct ModelInfo(float HeightScale)
    {
        public static readonly ModelInfo Default = new ModelInfo(1f);

        public float HeightScale = HeightScale;
    }
}
