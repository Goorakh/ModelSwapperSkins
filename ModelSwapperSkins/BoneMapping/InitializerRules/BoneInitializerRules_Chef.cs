using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public sealed class BoneInitializerRules_Chef : BoneInitializerRules_AutoName
    {
        public static new BoneInitializerRules_Chef Instance { get; } = new BoneInitializerRules_Chef();

        BoneInitializerRules_Chef() : base()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return bodyIndex == BodyCatalog.FindBodyIndex("ChefBody");
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name)
            {
                case "Root_M":
                    return new BoneInfo(BoneType.Pelvis)
                    {
                        PositionOffset = new Vector3(0.3f, 0f, 0f),
                        RotationOffset = Quaternion.Euler(90f, 90f, 0f),
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    };
                case "RootPart1_M":
                    return new BoneInfo(BoneType.Stomach)
                    {
                        RotationOffset = Quaternion.Euler(270f, 90f, 0f)
                    };
                case "Shoulder_L":
                    return new BoneInfo(BoneType.ArmUpperL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 90f)
                    };
                case "Elbow_L":
                    return new BoneInfo(BoneType.ArmLowerL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 200f, 90f)
                    };
                case "Wrist_L":
                    return new BoneInfo(BoneType.HandL)
                    {
                        RotationOffset = Quaternion.Euler(0f, 180f, 90f)
                    };
                case "Shoulder_R":
                    return new BoneInfo(BoneType.ArmUpperR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
                case "Elbow_R":
                    return new BoneInfo(BoneType.ArmLowerR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 45f, 90f)
                    };
                case "Wrist_R":
                    return new BoneInfo(BoneType.HandR)
                    {
                        RotationOffset = Quaternion.Euler(0f, 0f, 90f)
                    };
            }

            BoneInfo bone = base.getBoneInfo(modelTransform, potentialBoneTransform);

            switch (bone.Type)
            {
                case BoneType.Head:
                    bone.PositionOffset += new Vector3(0.15f, 0f, 0f);
                    bone.RotationOffset *= Quaternion.Euler(270f, 90f, 0f);
                    break;
                case BoneType.Chest:
                    bone.PositionOffset += new Vector3(0.3f, 0f, 0f);
                    bone.RotationOffset *= Quaternion.Euler(270f, 90f, 0f);
                    break;
                case BoneType.ArmLowerL:
                    bone.RotationOffset *= Quaternion.Euler(0f, 180f, 90f);
                    break;
            }

            return bone;
        }

        public override IEnumerable<Bone> GetAdditionalBones(Transform modelTransform, List<Bone> existingBones)
        {
            foreach (Bone bone in base.GetAdditionalBones(modelTransform, existingBones))
            {
                yield return bone;
            }

            Bone pelvisBone = existingBones.Find(b => b.Info.Type == BoneType.Pelvis);
            if (pelvisBone != null)
            {
                yield return new Bone(pelvisBone)
                {
                    Info = new BoneInfo(BoneType.Pelvis)
                    {
                        PositionOffset = new Vector3(-0.8f, 0f, 0f),
                        RotationOffset = pelvisBone.Info.RotationOffset,
                        MatchFlags = BoneMatchFlags.MatchToOther
                    }
                };

                yield return new Bone(pelvisBone)
                {
                    Info = new BoneInfo(BoneType.Base)
                    {
                        MatchFlags = BoneMatchFlags.AllowMatchTo
                    }
                };
            }
            else
            {
                Log.Error("Failed to find pelvis bone");
            }

            Transform wheelTransform = modelTransform.Find("root/Root_M/Wheel_M");
            if (wheelTransform)
            {
                yield return new Bone(new BoneInfo(BoneType.LegUpperL)
                {
                    MatchFlags = BoneMatchFlags.AllowMatchTo,
                    RotationOffset = Quaternion.Euler(90f, 270f, 0f),
                    Scale = 1.3f
                }, modelTransform, wheelTransform);

                yield return new Bone(new BoneInfo(BoneType.LegLowerL)
                {
                    MatchFlags = BoneMatchFlags.AllowMatchTo,
                    RotationOffset = Quaternion.Euler(90f, 270f, 0f),
                    Scale = 1.3f
                }, modelTransform, wheelTransform);

                yield return new Bone(new BoneInfo(BoneType.LegUpperR)
                {
                    MatchFlags = BoneMatchFlags.AllowMatchTo,
                    RotationOffset = Quaternion.Euler(90f, 90f, 0f),
                    Scale = 1.3f
                }, modelTransform, wheelTransform);

                yield return new Bone(new BoneInfo(BoneType.LegLowerR)
                {
                    MatchFlags = BoneMatchFlags.AllowMatchTo,
                    RotationOffset = Quaternion.Euler(90f, 90f, 0f),
                    Scale = 1.3f
                }, modelTransform, wheelTransform);
            }
        }
    }
}
