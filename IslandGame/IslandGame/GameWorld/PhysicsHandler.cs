﻿using System;
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




        public void update(float coefficientOfFriction)
        {
            velocity.X -= velocity.X * coefficientOfFriction;
            velocity.Z -= velocity.Z * coefficientOfFriction;

            gravitate();
        }

        private void gravitate()
        {
            velocity += new Vector3(0, -.6f / 60.0f, 0);
        }


    }
}
