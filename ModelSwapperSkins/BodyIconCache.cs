using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins
{
    public static class BodyIconCache
    {
        sealed record IconKey(Texture2D bodyPortrait);

        static readonly Dictionary<IconKey, Sprite> _cachedIcons = [];

        public static Sprite GetOrCreatePortraitIcon(Texture2D bodyPortrait)
        {
            IconKey key = new IconKey(bodyPortrait);
            if (_cachedIcons.TryGetValue(key, out Sprite cachedIcon))
                return cachedIcon;

            Sprite icon = generateSkinIcon(bodyPortrait);
            _cachedIcons.Add(key, icon);
            return icon;
        }

        static Sprite generateSkinIcon(Texture2D bodyPortrait)
        {
            Sprite icon = Sprite.Create(bodyPortrait, new Rect(0f, 0f, bodyPortrait.width, bodyPortrait.height), Vector2.zero);
            icon.name = bodyPortrait.name;
            return icon;
        }
    }
}
