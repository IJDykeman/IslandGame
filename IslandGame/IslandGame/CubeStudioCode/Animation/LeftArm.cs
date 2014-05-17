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
    public class leftArmAnimation : AnimationSystem
    {
        Quaternion raised;
        Quaternion lowered;
        Quaternion loweredHammer;
        Quaternion stabbedForward;

        float walkArmSwingAmount;
        float walkArmSwingSpeed;
        Quaternion swungForward;
        Quaternion swungBackward;

        int animationAge = 0;

        bool inverted;

        public float parentSwingTime;

        public float lowerArmSwing;

        public leftArmAnimation(bool ninverted)
        {
            inverted = ninverted;
            int invert = 1;
            if (inverted)
            {
                invert = -1;
            }
            currentTarget = new MovementTarget(Quaternion.Identity, 1);
            rotation = Quaternion.Identity;

            

            raised = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(180), MathHelper.ToRadians(10), MathHelper.ToRadians(-110));
            lowered = Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(80));
            loweredHammer = Quaternion.CreateFromYawPitchRoll(0, MathHelper.ToRadians(20), MathHelper.ToRadians(80));
            //forward = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-90), 0, 0);
            //stabbedForward = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(45), MathHelper.ToRadians(90), MathHelper.ToRadians(90));
            stabbedForward = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(0), MathHelper.ToRadians(90), MathHelper.ToRadians(90));
            stabbedForward *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(-45));



            walkArmSwingAmount = MathHelper.ToRadians(19);
            walkArmSwingSpeed = .02f;
            swungForward = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(0), walkArmSwingAmount, MathHelper.ToRadians(90));
            swungBackward = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(0), -walkArmSwingAmount, MathHelper.ToRadians(90));
            if (inverted)
            {
                raised = AnimationFunctions.mirror(raised);
                lowered = AnimationFunctions.mirror(lowered);
                stabbedForward = AnimationFunctions.mirror(stabbedForward);
                swungForward = AnimationFunctions.mirror(swungForward);
                swungBackward = AnimationFunctions.mirror(swungBackward);
            }
        }

        public override void handleOrder(List<AnimationType> types, AnimationSystem parent)
        {


            TorsoAnimation torso = parent as TorsoAnimation;
            if (torso != null)
            {
                parentSwingTime = torso.swingTime;
            }
            
            

                if(types.Contains(AnimationType.armsOut)){
                    currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, 0,0), .200000f);
                    Console.WriteLine("arms out");
                }
                else if (types.Contains(AnimationType.stabLeftArm) && !inverted)
                {
                        currentTarget = new MovementTarget(stabbedForward, .6f);
                }
                else if (types.Contains(AnimationType.stabRightArm) && inverted)
                {
                        currentTarget = new MovementTarget(stabbedForward, .6f);
                }

                else if (types.Contains(AnimationType.hammerHitLoweredLeftArm) && !inverted)
                {
                    currentTarget = new MovementTarget(loweredHammer, .3f);
                }
                else if (types.Contains(AnimationType.hammerHitRaisedLeftArm) && !inverted)
                {
                    currentTarget = new MovementTarget(raised, .3f);
                }

                else if (types.Contains(AnimationType.walking) || types.Contains(AnimationType.running))
                {

                    float armSwingToUse = walkArmSwingAmount;
                    if (inverted)
                    {
                        if(types.Contains(AnimationType.toolInRightHand))
                        {
                            parentSwingTime /= 3f;
                        }
                    }
                    else
                    {
                        if (types.Contains(AnimationType.toolInLeftHand))
                        {
                            parentSwingTime /= 3f;
                        }
                    }


                    //rotation = Quaternion.CreateFromYawPitchRoll(0, (inverted ? -1 : 1) * walkArmSwingAmount * parentSwingTime, (inverted ? -1 : 1) * MathHelper.ToRadians(90));
                    currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, (inverted ? -1 : 1) * armSwingToUse * parentSwingTime,
                        (inverted ? -1 : 1) * MathHelper.ToRadians(90)), .2f);

                }
                else if (types.Contains(AnimationType.standing))
                {
                    currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, 0,
                        (inverted ? -1 : 1) * MathHelper.ToRadians(85)), .2f);
                }
            
            
            
            lerpRotation();


        }
    }

}
