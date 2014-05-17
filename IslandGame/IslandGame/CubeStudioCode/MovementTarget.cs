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
    public class MovementTarget
    {
        public Quaternion goal;
        public float speed;

        public MovementTarget(Quaternion ngoal, float nspeed)
        {
            goal = ngoal;// new Quaternion(ngoal.X, ngoal.Y, ngoal.Z, ngoal.W);
            //^preserving pointer

            speed = nspeed;
        }
    }
}
