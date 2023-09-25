using System;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    [Serializable]
    public record struct BoneInfo(BoneType Type, Vector3 PositionOffset, Quaternion RotationOffset, Vector3 Scale)
    {
        public static readonly BoneInfo None = new BoneInfo(BoneType.None);

        public BoneType Type = Type;
        public Vector3 PositionOffset = PositionOffset;
        public Quaternion RotationOffset = RotationOffset;
        public Vector3 Scale = Scale;

        public BoneMatchFlags MatchFlags = BoneMatchFlags.AllowCompleteMatch;

        public readonly Matrix4x4 OffsetMatrix => Matrix4x4.TRS(PositionOffset, RotationOffset, Scale);

        public BoneInfo(BoneType type) : this(type, Vector3.zero, Quaternion.identity, Vector3.one)
        {
        }
    }
}
