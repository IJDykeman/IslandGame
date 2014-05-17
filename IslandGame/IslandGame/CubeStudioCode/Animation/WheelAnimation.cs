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
    public class WheelAnimation : AnimationSystem
    {

        int turn=0;



        public WheelAnimation()
        {

            currentTarget = new MovementTarget(Quaternion.Identity, 1);
            rotation = Quaternion.Identity;
            currentAnimation = AnimationType.none;
            

        }

        public override void handleOrder(List<AnimationType> type, AnimationSystem parent)
        {

            if (type.Contains(AnimationType.running) || type.Contains(AnimationType.walking))
            {
                turn+=5;
                rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX,MathHelper.ToRadians(-turn));

            }
            else if (type.Contains(AnimationType.standing))
            {
                currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll((0
                ), 0, MathHelper.ToRadians(1)), .2f);
            }


            //lerpRotation();

        }
    }

}
