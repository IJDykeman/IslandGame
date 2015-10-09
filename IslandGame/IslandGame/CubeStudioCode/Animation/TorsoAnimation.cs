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
    [Serializable] 
    class TorsoAnimation : AnimationSystem
    {


        int animationAge = 0;




        public float swingTime = 0;
        public ExponentialInterpolator swingTimeInter;
        public ExponentialInterpolator periodInter;
        public ExponentialInterpolator magInter;

        Quaternion leftShoulderForward;

        double age = 0;
        float bodySwing = MathHelper.ToRadians(7);

        AnimationType currentSinMotion = AnimationType.walking;

        public TorsoAnimation()
        {
            leftShoulderForward = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-20), 0, 0);
            swingTimeInter = new ExponentialInterpolator(0, 0, -24, 24, 1f);
            periodInter = new ExponentialInterpolator(12, 12, 0, 12, 1f);
            magInter = new ExponentialInterpolator(1, 1, -11000, 11000, .001f);
            


            
        }

        public override void handleOrder(List<AnimationType> type, AnimationSystem parent)
        {
            
            age++;
            //swingTime = (float)Math.Sin(MathHelper.ToRadians((float)age*5));
            


            if (type.Contains(AnimationType.torsoLeftShoulderForward))
            {
                //swingTime = (float)Math.Sin(MathHelper.ToRadians((float)age * 5));
                currentTarget = new MovementTarget(leftShoulderForward, .02f);


                //swingTimeInter.floatingCameraSpeed = .1f;
            }
            else if (type.Contains(AnimationType.walking))
            {
                //swingTime = (float)Math.Sin(MathHelper.ToRadians((float)age * 5));
                periodInter.idealValue = 5;
                magInter.idealValue = 1;
                swingTimeInter.idealValue = (float)Math.Sin(MathHelper.ToRadians((float)age * 7)) * 1;
                currentTarget = new MovementTarget( Quaternion.CreateFromYawPitchRoll(getYaw(), 0, 0),2f);


                //swingTimeInter.floatingCameraSpeed = .1f;
            }
            else if (type.Contains(AnimationType.running))
            {
                //swingTime = (float)Math.Sin(MathHelper.ToRadians((float)age * 12)) * 2;
                //periodInter.idealValue = 10;
                magInter.idealValue = 2;
                swingTimeInter.idealValue = (float)Math.Sin(MathHelper.ToRadians((float)age * 10)) * 2;
                //rotation = Quaternion.CreateFromYawPitchRoll(getYaw(), 0, 0);
                currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(getYaw(), 0, 0), .2f);


                //swingTimeInter.floatingCameraSpeed = .5f;

            }
            else
            {
                currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, 0, 0), .2f);
            }
            swingTimeInter.update();
            periodInter.update();
            magInter.update();
            swingTime = swingTimeInter.value;
            lerpRotation();
            //currentAnimation = taskType;

            //rotation = Quaternion.Slerp(rotation, currentTarget.goal, currentTarget.floatingCameraSpeed / AnimationFunctions.angleBetweenQuaternions(rotation, currentTarget.goal));


        }

        public float getYaw()
        {
            return (float)-swingTime* (float)bodySwing;
        }
    }
}
