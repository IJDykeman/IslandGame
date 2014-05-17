using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{

    static class LinearLocationInterpolator
    {
        public static Vector3 interpolate(Vector3 start, Vector3 end, float speed)
        {
            Vector3 movementNormal = Vector3.Normalize(end - start);
            if (Vector3.Distance(start, end) < speed)
            {
                return end + new Vector3();
            }
            else
            {
                return start + (movementNormal * speed);
            }
        }

        public static bool isLinearInterpolationComplete(Vector3 start, Vector3 end, float speed)
        {
            return Vector3.Distance(start, end) < speed;
        }
    }
}
