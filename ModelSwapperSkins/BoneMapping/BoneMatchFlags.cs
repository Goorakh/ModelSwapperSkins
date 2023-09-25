using System;

namespace ModelSwapperSkins.BoneMapping
{
    [Flags]
    public enum BoneMatchFlags : byte
    {
        None,
        MatchToOther = 1 << 0,
        AllowMatchTo = 1 << 1,
        AllowCompleteMatch = MatchToOther | AllowMatchTo
    }
}
