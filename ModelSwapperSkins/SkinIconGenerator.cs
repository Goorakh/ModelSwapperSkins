using ModelSwapperSkins.Utils;
using ModelSwapperSkins.Utils.Extensions;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins
{
    public static class SkinIconGenerator
    {
        record SkinIconKey(Texture2D bodyPortrait, Sprite background);

        static readonly Dictionary<SkinIconKey, Sprite> _cachedIcons = [];

        public static Sprite GetOrCreateSkinIcon(Texture2D bodyPortrait, SkinDef backgroundSkin)
        {
            return GetOrCreateSkinIcon(bodyPortrait, backgroundSkin ? backgroundSkin.icon : null);
        }

        public static Sprite GetOrCreateSkinIcon(Texture2D bodyPortrait, Sprite background)
        {
            SkinIconKey key = new SkinIconKey(bodyPortrait, background);
            if (_cachedIcons.TryGetValue(key, out Sprite cachedIcon))
                return cachedIcon;

            Sprite icon = generateSkinIcon(bodyPortrait, background);
            _cachedIcons.Add(key, icon);
            return icon;
        }

        static Sprite generateSkinIcon(Texture2D bodyPortrait, Sprite background)
        {
            if (!bodyPortrait || !background)
            {
                if (bodyPortrait)
                {
                    return bodyPortrait.CreateSprite();
                }
                else if (background)
                {
                    return background;
                }
                else
                {
                    return null;
                }
            }

            Rect bodyPortraitRect = new Rect(0f, 0f, bodyPortrait.width, bodyPortrait.height);
            Rect backgroundRect = background.rect;

            using TemporaryTexture readableBodyPortrait = bodyPortrait.AsReadable();
            using TemporaryTexture readableBackground = background.texture.AsReadable();

            int width = bodyPortrait.width;
            int height = bodyPortrait.height;

            Texture2D iconTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color getCurrentPixelColor(Texture2D texture, Rect baseRect)
                    {
                        int sampleX = Mathf.FloorToInt(Util.Remap(x, 0, width, baseRect.x, baseRect.x + baseRect.width));
                        int sampleY = Mathf.FloorToInt(Util.Remap(y, 0, height, baseRect.y, baseRect.y + baseRect.height));

                        return texture.GetPixel(sampleX, sampleY);
                    }

                    Color bodyPixelColor = getCurrentPixelColor(readableBodyPortrait.Texture, bodyPortraitRect);
                    Color backgroundPixelColor = getCurrentPixelColor(readableBackground.Texture, backgroundRect);

                    iconTexture.SetPixel(x, y, Color.Lerp(bodyPixelColor, backgroundPixelColor, 1f - bodyPixelColor.a));
                }
            }

            iconTexture.Apply();

            return iconTexture.CreateSprite();
        }
    }
}
