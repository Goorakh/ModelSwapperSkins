﻿using System;
using UnityEngine;

namespace ModelSwapperSkins.Utils
{
    public class TemporaryTexture(Texture2D texture, bool isTemporary) : IDisposable
    {
        public readonly Texture2D Texture = texture;

        public void Dispose()
        {
            if (isTemporary)
            {
                GameObject.Destroy(Texture);
            }
        }
    }
}
