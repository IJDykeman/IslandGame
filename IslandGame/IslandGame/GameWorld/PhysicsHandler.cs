using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class PhysicsHandler
    {
        public AxisAlignedBoundingBox AABB;
        public Vector3 velocity;

        public PhysicsHandler(AxisAlignedBoundingBox nAABB)
        {
            AABB = nAABB;
            velocity = new Vector3();
        }




        public void update(bool isStanding)
        {
            if (isStanding)
            {
                velocity.X -= velocity.X * .4f;
                velocity.Z -= velocity.Z * .4f;
            }
            gravitate();
        }

        private void gravitate()
        {
            velocity += new Vector3(0, -1f / 60.0f, 0);
        }


    }
}
