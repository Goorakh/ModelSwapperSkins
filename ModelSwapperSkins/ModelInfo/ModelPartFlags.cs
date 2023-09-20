using System;

namespace ModelSwapperSkins.ModelInfo
{
    [Flags]
    public enum ModelPartFlags : byte
    {
        None = 0,

        ShowForSkin = 1 << 0,
        ShowForMain = 1 << 1,
        AlwaysShow = ShowForSkin | ShowForMain,

        /// <summary>
        /// Shown for skin, not for main model
        /// </summary>
        Body = ShowForSkin,

        /// <summary>
        /// Shown for main model, not for skin
        /// </summary>
        Weapon = ShowForMain,

        /// <summary>
        /// Shown for main model, not for skin
        /// </summary>
        Decoration = ShowForMain,

        /// <summary>
        /// Shown for both main and skin models
        /// </summary>
        BodyFeature = AlwaysShow
    }
}
