using System;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
    class LowerLeftLegAnimation : AnimationSystem
    {

        Quaternion raised;
        Quaternion lowered;
        Quaternion forward;

        float walkArmSwingAmount;
        float walkArmSwingSpeed;
        Quaternion swungForward;
        Quaternion swungBackward;

        int animationAge = 0;

        bool inverted;

        public float lowerArmSwing;

        public LowerLeftLegAnimation(bool ninverted)
        {
            inverted = ninverted;
            int invert = 1;
            if (inverted)
            {
                invert = -1;
            }
            currentTarget = new MovementTarget(Quaternion.Identity, 1);
            rotation = Quaternion.Identity;
   
            



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
            
            leftLegAnimation leftLeg = parent as leftLegAnimation;
            

            if(type.Contains( AnimationType.walking)){
                if (leftLeg != null)
                {
                    currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, (inverted ? 1 : -1) * walkArmSwingAmount * (inverted ? 1 : 1) *
                            (leftLeg.swingTime + (inverted ? -1 : 1) * 1), 0), .2f);
                }
            }
            else if (type.Contains(AnimationType.running))
            {
                if (leftLeg != null)
                {
                    currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, (inverted ? 1 : -1) * walkArmSwingAmount * (inverted ? 1 : 1) *
                        (leftLeg.swingTime + (inverted ? -1 : 1) * 2), 0), .2f);
                }

            }
            else if (type.Contains(AnimationType.standing))
            {
                currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0,0,0), .2f);
            }
            lerpRotation();
            
           

        }
    }
    
}
