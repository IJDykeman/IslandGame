using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CubeAnimator
{
    public enum AnimationType
    {
        none,
        standing,
        running,
        falling,
        toolInLeftHand,
        toolInRightHand,
        walking,
        armsOut,
        stabLeftArm,
        stabRightArm,
        hammerHitRaisedLeftArm,
        hammerHitLoweredLeftArm,
        torsoLeftShoulderForward


    }

    [Serializable] 
    public abstract class AnimationSystem
    {
        public Quaternion rotation;
        public AnimationType currentAnimation;

        public MovementTarget currentTarget;


        public abstract void handleOrder(List<AnimationType> typem, AnimationSystem parent);
        public Quaternion getRotation()
        {
            return rotation;
        }

        public void lerpRotation()
        {


            //if (float.IsNaN())
            //{
             //   rotation = currentTarget.goal;

            //}
            float angleBetween = AnimationFunctions.angleBetweenQuaternions(rotation, currentTarget.goal);

            if (MathHelper.ToDegrees(angleBetween) < 10)
            {
                rotation = Quaternion.Lerp(rotation, currentTarget.goal, .2f);
                return;
            }

            float amountToUse = currentTarget.speed / angleBetween;

            float speedLimit = .4f;
            if(amountToUse>speedLimit)
            {
                amountToUse=speedLimit;
            }

            rotation = Quaternion.Lerp(rotation, currentTarget.goal, amountToUse);

            if (float.IsNaN(rotation.W))
            {
                rotation = currentTarget.goal;
            }
        }


        public void returnToDefaultPosition()
        {
            rotation = Quaternion.Identity;
        }

    }

    [Serializable]
    public class noAnimation : AnimationSystem
    {
        public noAnimation()
        {
            rotation = Quaternion.Identity;
        }
        public override void handleOrder(List<AnimationType> type, AnimationSystem parent)
        {

        }

    }


 


  

    static class AnimationSystemFactory
    {
        public static AnimationSystem getSystemFromType(BodyPartType type)
        {
            

            switch (type)
            {
                case BodyPartType.leftArm:
                    return new leftArmAnimation(false);
                case BodyPartType.lowerLeftArm:
                   return new LowerLeftArmAnimation(false);
                case BodyPartType.rightArm:
                    return new leftArmAnimation(true);
                case BodyPartType.lowerRightArm:
                    return new LowerLeftArmAnimation(true);
                case BodyPartType.rightLeg:
                    return new leftLegAnimation(true);
                case BodyPartType.lowerRightLeg:
                    return new LowerLeftLegAnimation(true);
                case BodyPartType.leftLeg:
                    return new leftLegAnimation(false);
                case BodyPartType.lowerLeftLeg:
                    return new LowerLeftLegAnimation(false);
                case BodyPartType.torso:
                    return new TorsoAnimation();
                case BodyPartType.head:
                    return new headAnimation();
                case BodyPartType.wheel:
                    return new WheelAnimation();
                case BodyPartType.rigid:
                    return new noAnimation();
                default:
                    throw new Exception("unhandled body type");
            }
        }
    }

    
}




