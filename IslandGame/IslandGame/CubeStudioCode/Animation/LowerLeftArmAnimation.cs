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
    public class LowerLeftArmAnimation : AnimationSystem
    {

        Quaternion extended;
        Quaternion folded;

        float walkArmSwingAmount;
        float walkArmSwingSpeed;
        Quaternion swungForward;
        Quaternion rest;

        int animationAge = 0;

        bool inverted;

        public LowerLeftArmAnimation(bool ninverted)
        {
            inverted = ninverted;
            int invert = 1;
            if (inverted)
            {
                invert = -1;
            }
            currentTarget = new MovementTarget(Quaternion.Identity, 1);
            rotation = Quaternion.Identity;
            currentAnimation = AnimationType.none;
            


            walkArmSwingAmount = MathHelper.ToRadians(25);
            walkArmSwingSpeed = .02f;
            swungForward = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-90), 0, MathHelper.ToRadians(10));
            rest = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(0), 0, MathHelper.ToRadians(10));

            extended = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(0), 0,.001f);
            folded = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-90), 0, .001f);

            if (inverted)
            {

                swungForward = AnimationFunctions.mirror(swungForward);
                rest = AnimationFunctions.mirror(rest);
            }
        }

        public override void handleOrder(List<AnimationType> type, AnimationSystem parent)
        {

            leftArmAnimation parentArm = parent as leftArmAnimation;


            if (type.Contains(AnimationType.armsOut))
            {
                currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0,0,0), .2f);

            }
            else if (type.Contains(AnimationType.stabLeftArm) && !inverted)
            {

                    currentTarget = new MovementTarget(extended, .6f);
                
            }
            else if (type.Contains(AnimationType.stabRightArm) && inverted)
            {

                    currentTarget = new MovementTarget(extended, .6f);
                
            }
            else if (type.Contains(AnimationType.hammerHitLoweredLeftArm) && !inverted)
            {
                currentTarget = new MovementTarget(folded, .2f);
            }
            else if (type.Contains(AnimationType.hammerHitRaisedLeftArm) && !inverted)
            {
                currentTarget = new MovementTarget(extended, .2f);
            }
            else if (type.Contains(AnimationType.running) || type.Contains(AnimationType.walking))
            {
                if (parentArm != null)
                {
                    currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll((-parentArm.parentSwingTime/2 + (inverted ? 1 : -1) * (2-MathHelper.ToRadians(90))), (inverted ? 1 : -1) * walkArmSwingAmount * (inverted ? 1 : 1) *
                        0, 0), .2f);
                }

            }
            else if (type.Contains(AnimationType.standing))
            {
                currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll((0
                ), 0, MathHelper.ToRadians(1)), .2f);
            }


            lerpRotation();

        }
    }

}
