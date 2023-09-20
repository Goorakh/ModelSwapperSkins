using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_AutoName : BoneInitializerRules
    {
        public static readonly BoneInitializerRules_AutoName Instance = new BoneInitializerRules_AutoName();

        protected BoneInitializerRules_AutoName()
        {
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name.ToLower())
            {
                case "root":
                    return new BoneInfo(BoneType.Root);
                case "base":
                    return new BoneInfo(BoneType.Base);
                case "pelvis":
                case "hip":
                    return new BoneInfo(BoneType.Pelvis);
                case "thigh.l":
                    return new BoneInfo(BoneType.LegUpperL);
                case "calf.l":
                    return new BoneInfo(BoneType.LegLowerL);
                case "foot.l":
                case "footl":
                    return new BoneInfo(BoneType.FootL);
                case "toe.l":
                    return new BoneInfo(BoneType.ToeL);
                case "toe.l_end":
                    return new BoneInfo(BoneType.ToeL_end);
                case "thigh.r":
                    return new BoneInfo(BoneType.LegUpperR);
                case "calf.r":
                    return new BoneInfo(BoneType.LegLowerR);
                case "foot.r":
                case "footr":
                    return new BoneInfo(BoneType.FootR);
                case "toe.r":
                    return new BoneInfo(BoneType.ToeR);
                case "toe.r_end":
                    return new BoneInfo(BoneType.ToeR_end);
                case "stomach":
                    return new BoneInfo(BoneType.Stomach);
                case "chest":
                    return new BoneInfo(BoneType.Chest);
                case "head":
                    return new BoneInfo(BoneType.Head);
                case "neck":
                    return new BoneInfo(BoneType.Neck);
                case "clavicle.l":
                case "shoulder.l":
                    return new BoneInfo(BoneType.ShoulderL);
                case "upper_arm.l":
                case "upperarm.l":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "lower_arm.l":
                case "forearm.l":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "hand.l":
                case "handl":
                    return new BoneInfo(BoneType.HandL);
                case "hand.l_end":
                    return new BoneInfo(BoneType.HandL_end);
                case "palml":
                    return new BoneInfo(BoneType.HandPalmL);
                case "finger1.1.l":
                case "finger1.l":
                    return new BoneInfo(BoneType.IndexFinger1L);
                case "finger1.2.l":
                    return new BoneInfo(BoneType.IndexFinger2L);
                case "finger1.3.l":
                    return new BoneInfo(BoneType.IndexFinger3L);
                case "finger1.3.l_end":
                case "finger1.l_end":
                    return new BoneInfo(BoneType.IndexFinger3L_end);
                case "finger2.1.l":
                case "finger2.l":
                    return new BoneInfo(BoneType.MiddleFinger1L);
                case "finger2.2.l":
                    return new BoneInfo(BoneType.MiddleFinger2L);
                case "finger2.3.l":
                    return new BoneInfo(BoneType.MiddleFinger3L);
                case "finger2.3.l_end":
                case "finger2.l_end":
                    return new BoneInfo(BoneType.MiddleFinger3L_end);
                case "finger3.1.l":
                    return new BoneInfo(BoneType.RingFinger1L);
                case "finger3.2.l":
                    return new BoneInfo(BoneType.RingFinger2L);
                case "finger3.3.l":
                    return new BoneInfo(BoneType.RingFinger3L);
                case "finger3.3.l_end":
                    return new BoneInfo(BoneType.RingFinger3L_end);
                case "finger4.1.l":
                    return new BoneInfo(BoneType.PinkyFinger1L);
                case "finger4.2.l":
                    return new BoneInfo(BoneType.PinkyFinger2L);
                case "finger4.3.l":
                    return new BoneInfo(BoneType.PinkyFinger3L);
                case "finger4.3.l_end":
                    return new BoneInfo(BoneType.PinkyFinger3L_end);
                case "thumb.1.l":
                case "thumb.l":
                    return new BoneInfo(BoneType.Thumb1L);
                case "thumb.2.l":
                    return new BoneInfo(BoneType.Thumb2L);
                case "thumb.2.l_end":
                case "thumb.l_end":
                    return new BoneInfo(BoneType.Thumb2L_end);
                case "clavicle.r":
                case "shoulder.r":
                    return new BoneInfo(BoneType.ShoulderR);
                case "upper_arm.r":
                case "upperarm.r":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "lower_arm.r":
                case "forearm.r":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "hand.r":
                case "handr":
                    return new BoneInfo(BoneType.HandR);
                case "finger1.1.r":
                case "finger1.r":
                    return new BoneInfo(BoneType.IndexFinger1R);
                case "finger1.2.r":
                    return new BoneInfo(BoneType.IndexFinger2R);
                case "finger1.3.r":
                    return new BoneInfo(BoneType.IndexFinger3R);
                case "finger1.3.r_end":
                case "finger1.r_end":
                    return new BoneInfo(BoneType.IndexFinger3R_end);
                case "finger2.1.r":
                case "finger2.r":
                    return new BoneInfo(BoneType.MiddleFinger1R);
                case "finger2.2.r":
                    return new BoneInfo(BoneType.MiddleFinger2R);
                case "finger2.3.r":
                    return new BoneInfo(BoneType.MiddleFinger3R);
                case "finger2.3.r_end":
                case "finger2.r_end":
                    return new BoneInfo(BoneType.MiddleFinger3R_end);
                case "finger3.1.r":
                    return new BoneInfo(BoneType.RingFinger1R);
                case "finger3.2.r":
                    return new BoneInfo(BoneType.RingFinger2R);
                case "finger3.3.r":
                    return new BoneInfo(BoneType.RingFinger3R);
                case "finger3.3.r_end":
                    return new BoneInfo(BoneType.RingFinger3R_end);
                case "finger4.1.r":
                    return new BoneInfo(BoneType.PinkyFinger1R);
                case "finger4.2.r":
                    return new BoneInfo(BoneType.PinkyFinger2R);
                case "finger4.3.r":
                    return new BoneInfo(BoneType.PinkyFinger3R);
                case "finger4.3.r_end":
                    return new BoneInfo(BoneType.PinkyFinger3R_end);
                case "thumb.1.r":
                case "thumb.r":
                    return new BoneInfo(BoneType.Thumb1R);
                case "thumb.2.r":
                    return new BoneInfo(BoneType.Thumb2R);
                case "thumb.2.r_end":
                case "thumb.r_end":
                    return new BoneInfo(BoneType.Thumb2R_end);
                case "tail1":
                case "tail.1":
                    return new BoneInfo(BoneType.Tail1);
                case "tail2":
                case "tail.2":
                    return new BoneInfo(BoneType.Tail2);
                case "tail3":
                case "tail.3":
                    return new BoneInfo(BoneType.Tail3);
                case "tail4":
                case "tail.4":
                    return new BoneInfo(BoneType.Tail4);
                case "tail5":
                    return new BoneInfo(BoneType.Tail5);
                case "tail6":
                    return new BoneInfo(BoneType.Tail6);
                case "tail.7":
                    return new BoneInfo(BoneType.Tail7);
                case "tail3_end":
                case "tail.4_end":
                case "tail4_end":
                case "tail5_end":
                case "tail6_end":
                case "tail.7_end":
                    return new BoneInfo(BoneType.Tail_end);
                default:
                    return BoneInfo.None;
            }
        }
    }
}
