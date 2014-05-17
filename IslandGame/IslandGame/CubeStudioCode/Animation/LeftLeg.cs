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
    public class leftLegAnimation : AnimationSystem
    {
        Quaternion raised;
        Quaternion lowered;
        Quaternion forward;

        float walkArmSwingAmount;
        float walkArmSwingSpeed;
        Quaternion swungForward;
        Quaternion swungBackward;

        int animationAge = 0;

        public float swingTime;

        bool inverted;

        public float lowerArmSwing;

        public leftLegAnimation(bool ninverted)
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
            



            walkArmSwingAmount = MathHelper.ToRadians(30);
            walkArmSwingSpeed = .04f;
            swungForward = Quaternion.CreateFromYawPitchRoll(0, walkArmSwingAmount, 0);
            swungBackward = Quaternion.CreateFromYawPitchRoll(0, -walkArmSwingAmount, 0);
            if (inverted)
            {

                swungForward = AnimationFunctions.mirror(swungForward);
                swungBackward = AnimationFunctions.mirror(swungBackward);
            }
        }

        public override void handleOrder(List<AnimationType> type, AnimationSystem parent)
        {

            TorsoAnimation torso = parent as TorsoAnimation;
            if (torso != null)
            {
                swingTime = torso.swingTime;
            }


            
                
            if(type.Contains(AnimationType.running)){
                    currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, (inverted ? 1 : -1) * walkArmSwingAmount * (swingTime), 0)
                        , .400000f);
            }
            else if(type.Contains(AnimationType.walking)){
                    //rotation = Quaternion.CreateFromYawPitchRoll(0, (inverted ? 1 : -1) * walkArmSwingAmount * (swingTime), 0);
                    currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, (inverted ? 1 : -1) * walkArmSwingAmount * (swingTime), 0)
                        , .1200000f);
                    
            }
            else if (type.Contains(AnimationType.standing))
            {
                currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, 0,0)
                        , .1200000f);
            }

            
            lerpRotation();

        }
    }
}
