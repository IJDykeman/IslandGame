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
    class headAnimation : AnimationSystem
    {
        




        public headAnimation()
        {



            
        }

        public override void handleOrder(List<AnimationType> type, AnimationSystem parent)
        {
            
            if (type.Contains(AnimationType.walking) || type.Contains(AnimationType.running))
            {
                //rotation = Quaternion.CreateFromYawPitchRoll(-((TorsoAnimation)parent).getYaw()/1f, 0, 0);
                TorsoAnimation torso = parent as TorsoAnimation;
                if (torso != null)
                {
                    currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(-(torso).getYaw() * 1f, 0, 0), .09f);
                }

            }
            else
            {
                currentTarget = new MovementTarget(Quaternion.CreateFromYawPitchRoll(0, 0, 0), .09f);

            }

            //currentAnimation = taskType;

            //rotation = Quaternion.Slerp(rotation, currentTarget.goal, currentTarget.floatingCameraSpeed / AnimationFunctions.angleBetweenQuaternions(rotation, currentTarget.goal));
            lerpRotation();

        }


    }
}
