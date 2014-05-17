using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeAnimator
{
    public enum BodyPartType
    {
        torso,
        head,
        leftArm,
        lowerLeftArm,
        rightArm,
        lowerRightArm,
        leftLeg,
        lowerLeftLeg,
        rightLeg,
        lowerRightLeg,
        wheel,
        rigid,
        unknown
    }

    public static class BodyPartTypeManager
    {
        public static string getName(BodyPartType type)
        {
            switch (type)
            {
                case BodyPartType.torso:
                    return "torso";
                case BodyPartType.head:
                    return "head";
                case BodyPartType.leftArm:
                    return "left arm";
                case BodyPartType.lowerLeftArm:
                    return "lower left arm";
                case BodyPartType.rightArm:
                    return "right arm";
                case BodyPartType.lowerRightArm:
                    return "lower right arm";
                case BodyPartType.leftLeg:
                    return "left leg";
                case BodyPartType.lowerLeftLeg:
                    return "lower left leg";
                case BodyPartType.rightLeg:
                    return "right leg";
                case BodyPartType.lowerRightLeg:
                    return "lower right leg";
                case BodyPartType.wheel:
                    return "wheel";
                case BodyPartType.rigid:
                    return "rigid connection";
                default:
                    throw new Exception("unhandled body type");
            }
        }
    }
}
