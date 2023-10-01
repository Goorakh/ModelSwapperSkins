using RoR2;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping.InitializerRules
{
    public class BoneInitializerRules_AutoName : BoneInitializerRules
    {
        public static readonly BoneInitializerRules_AutoName Instance = new BoneInitializerRules_AutoName();

        protected BoneInitializerRules_AutoName()
        {
        }

        public override bool AppliesTo(BodyIndex bodyIndex)
        {
            return true;
        }

        protected override BoneInfo getBoneInfo(Transform modelTransform, Transform potentialBoneTransform)
        {
            switch (potentialBoneTransform.name.ToLower())
            {
                case "root":
                case "root_jnt":
                    return new BoneInfo(BoneType.Root);
                case "base":
                    return new BoneInfo(BoneType.Base);
                case "pelvis":
                case "pelvis_jnt":
                case "hip":
                case "hips":
                    return new BoneInfo(BoneType.Pelvis);
                case "thigh.l":
                case "thigh1.l":
                case "l_thigh_jnt":
                    return new BoneInfo(BoneType.LegUpperL);
                case "calf.l":
                case "shin1.l":
                case "l_calf_jnt":
                    return new BoneInfo(BoneType.LegLowerL);
                case "foot.l":
                case "footl":
                case "l_foot_jnt":
                    return new BoneInfo(BoneType.FootL);
                case "toe.l":
                case "toe1.l":
                case "l_toet_jnt":
                    return new BoneInfo(BoneType.Toe1L);
                case "toe2.l":
                    return new BoneInfo(BoneType.Toe2L);
                case "toe3.l":
                    return new BoneInfo(BoneType.Toe3L);
                case "thigh.r":
                case "thigh1.r":
                case "r_thigh_jnt":
                    return new BoneInfo(BoneType.LegUpperR);
                case "calf.r":
                case "shin1.r":
                case "r_calf_jnt":
                    return new BoneInfo(BoneType.LegLowerR);
                case "foot.r":
                case "footr":
                case "r_foot_jnt":
                    return new BoneInfo(BoneType.FootR);
                case "toe.r":
                case "toe1.r":
                case "r_toet_jnt":
                    return new BoneInfo(BoneType.Toe1R);
                case "toe2.r":
                    return new BoneInfo(BoneType.Toe2R);
                case "toe3.r":
                    return new BoneInfo(BoneType.Toe3R);
                case "stomach":
                    return new BoneInfo(BoneType.Stomach);
                case "chest":
                case "chest_jnt":
                    return new BoneInfo(BoneType.Chest);
                case "neck":
                case "neck.1":
                case "neck1":
                case "neck_jnt":
                    return new BoneInfo(BoneType.Neck1);
                case "neck.2":
                case "neck2":
                    return new BoneInfo(BoneType.Neck2);
                case "neck.3":
                case "neck3":
                    return new BoneInfo(BoneType.Neck3);
                case "neck.4":
                case "neck4":
                    return new BoneInfo(BoneType.Neck4);
                case "neck.5":
                case "neck5":
                    return new BoneInfo(BoneType.Neck5);
                case "neck.6":
                case "neck6":
                    return new BoneInfo(BoneType.Neck6);
                case "neck.7":
                case "neck7":
                    return new BoneInfo(BoneType.Neck7);
                case "neck.8":
                case "neck8":
                    return new BoneInfo(BoneType.Neck8);
                case "neck.9":
                case "neck9":
                    return new BoneInfo(BoneType.Neck9);
                case "neck.10":
                case "neck10":
                    return new BoneInfo(BoneType.Neck10);
                case "neck.11":
                case "neck11":
                    return new BoneInfo(BoneType.Neck11);
                case "neck.12":
                case "neck12":
                    return new BoneInfo(BoneType.Neck12);
                case "neck.13":
                case "neck13":
                    return new BoneInfo(BoneType.Neck13);
                case "neck.14":
                case "neck14":
                    return new BoneInfo(BoneType.Neck14);
                case "neck.15":
                case "neck15":
                    return new BoneInfo(BoneType.Neck15);
                case "neck.16":
                case "neck16":
                    return new BoneInfo(BoneType.Neck16);
                case "head":
                case "head_jnt":
                    return new BoneInfo(BoneType.Head);
                case "jaw":
                    return new BoneInfo(BoneType.Jaw);
                case "tongue.1":
                case "tongue1":
                    return new BoneInfo(BoneType.Tongue1);
                case "tongue.2":
                case "tongue2":
                    return new BoneInfo(BoneType.Tongue2);
                case "tongue.3":
                case "tongue3":
                    return new BoneInfo(BoneType.Tongue3);
                case "tongue.4":
                case "tongue4":
                    return new BoneInfo(BoneType.Tongue4);
                case "clavicle.l":
                case "shoulder.l":
                case "l_shoulder_jnt":
                    return new BoneInfo(BoneType.ShoulderL);
                case "upper_arm.l":
                case "upper_arm1.l":
                case "upperarm.l":
                case "l_arm_jnt":
                    return new BoneInfo(BoneType.ArmUpperL);
                case "lower_arm.l":
                case "lowerarm.l":
                case "forearm.l":
                case "forearm1.l":
                case "l_forearm_jnt":
                    return new BoneInfo(BoneType.ArmLowerL);
                case "hand.l":
                case "handl":
                case "l_hand_jnt":
                    return new BoneInfo(BoneType.HandL);
                case "palml":
                    return new BoneInfo(BoneType.HandPalmL);
                case "finger1.1.l":
                case "finger1.l":
                case "index1.l":
                case "l_index_1_jnt":
                    return new BoneInfo(BoneType.IndexFinger1L);
                case "finger1.2.l":
                case "finger1.l.001":
                case "index2.l":
                case "l_index_2_jnt":
                    return new BoneInfo(BoneType.IndexFinger2L);
                case "finger1.3.l":
                case "finger1.l.002":
                case "index3.l":
                case "l_index_3_jnt":
                    return new BoneInfo(BoneType.IndexFinger3L);
                case "finger2.1.l":
                case "finger2.l":
                case "middle1.l":
                case "l_middle_1_jnt":
                    return new BoneInfo(BoneType.MiddleFinger1L);
                case "finger2.2.l":
                case "finger2.l.001":
                case "middle2.l":
                case "l_middle_2_jnt":
                    return new BoneInfo(BoneType.MiddleFinger2L);
                case "finger2.3.l":
                case "finger2.l.002":
                case "middle3.l":
                case "l_middle_3_jnt":
                    return new BoneInfo(BoneType.MiddleFinger3L);
                case "finger3.1.l":
                case "finger3.l":
                case "ring1.l":
                case "l_ring_1_jnt":
                    return new BoneInfo(BoneType.RingFinger1L);
                case "finger3.2.l":
                case "finger3.l.001":
                case "ring2.l":
                case "l_ring_2_jnt":
                    return new BoneInfo(BoneType.RingFinger2L);
                case "finger3.3.l":
                case "finger3.l.002":
                case "ring3.l":
                case "l_ring_3_jnt":
                    return new BoneInfo(BoneType.RingFinger3L);
                case "finger4.1.l":
                case "pinky1.l":
                case "l_pinky_1_jnt":
                    return new BoneInfo(BoneType.PinkyFinger1L);
                case "finger4.2.l":
                case "pinky2.l":
                case "l_pinky_2_jnt":
                    return new BoneInfo(BoneType.PinkyFinger2L);
                case "finger4.3.l":
                case "pinky3.l":
                case "l_pinky_3_jnt":
                    return new BoneInfo(BoneType.PinkyFinger3L);
                case "thumb.1.l":
                case "thumb1.l":
                case "thumb.l":
                case "l_thumb_1_jnt":
                    return new BoneInfo(BoneType.Thumb1L);
                case "thumb.2.l":
                case "thumb.l.001":
                case "thumb2.l":
                case "l_thumb_2_jnt":
                    return new BoneInfo(BoneType.Thumb2L);
                case "thumb.2.l_end":
                case "thumb.l_end":
                    return new BoneInfo(BoneType.Thumb2L_end);
                case "clavicle.r":
                case "shoulder.r":
                case "r_shoulder_jnt":
                    return new BoneInfo(BoneType.ShoulderR);
                case "upper_arm.r":
                case "upper_arm1.r":
                case "upperarm.r":
                case "r_arm_jnt":
                    return new BoneInfo(BoneType.ArmUpperR);
                case "lower_arm.r":
                case "forearm1.r":
                case "lowerarm.r":
                case "forearm.r":
                case "r_forearm_jnt":
                    return new BoneInfo(BoneType.ArmLowerR);
                case "hand.r":
                case "handr":
                case "r_hand_jnt":
                    return new BoneInfo(BoneType.HandR);
                case "finger1.1.r":
                case "finger.1.1.r":
                case "finger1.r":
                case "index1.r":
                    return new BoneInfo(BoneType.IndexFinger1R);
                case "finger1.2.r":
                case "finger.1.2.r":
                case "finger1.r.001":
                case "index2.r":
                    return new BoneInfo(BoneType.IndexFinger2R);
                case "finger1.3.r":
                case "finger1.r.002":
                case "index3.r":
                    return new BoneInfo(BoneType.IndexFinger3R);
                case "finger2.1.r":
                case "finger.2.1.r":
                case "finger2.r":
                case "middle1.r":
                    return new BoneInfo(BoneType.MiddleFinger1R);
                case "finger2.2.r":
                case "finger.2.2.r":
                case "finger2.r.001":
                case "middle2.r":
                    return new BoneInfo(BoneType.MiddleFinger2R);
                case "finger2.3.r":
                case "finger2.r.002":
                case "middle3.r":
                    return new BoneInfo(BoneType.MiddleFinger3R);
                case "finger3.1.r":
                case "finger3.r":
                case "ring1.r":
                    return new BoneInfo(BoneType.RingFinger1R);
                case "finger3.2.r":
                case "finger3.r.001":
                case "ring2.r":
                    return new BoneInfo(BoneType.RingFinger2R);
                case "finger3.3.r":
                case "finger3.r.002":
                case "ring3.r":
                    return new BoneInfo(BoneType.RingFinger3R);
                case "finger4.1.r":
                case "pinky1.r":
                    return new BoneInfo(BoneType.PinkyFinger1R);
                case "finger4.2.r":
                case "pinky2.r":
                    return new BoneInfo(BoneType.PinkyFinger2R);
                case "finger4.3.r":
                case "pinky3.r":
                    return new BoneInfo(BoneType.PinkyFinger3R);
                case "thumb.1.r":
                case "thumb1.r":
                case "thumb.r":
                    return new BoneInfo(BoneType.Thumb1R);
                case "thumb.2.r":
                case "thumb.r.001":
                case "thumb2.r":
                    return new BoneInfo(BoneType.Thumb2R);
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
                case "tail.5":
                    return new BoneInfo(BoneType.Tail5);
                case "tail6":
                case "tail.6":
                    return new BoneInfo(BoneType.Tail6);
                case "tail.7":
                    return new BoneInfo(BoneType.Tail7);
                default:
                    return BoneInfo.None;
            }
        }
    }
}
