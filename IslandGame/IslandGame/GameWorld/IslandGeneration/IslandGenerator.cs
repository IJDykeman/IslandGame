using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    public abstract class IslandGenerator
    {
        public abstract void generateIsland(ChunkSpace chunkSpace, SetPieceManager setPieceManager, JobSiteManager jobSiteManager, IslandLocationProfile locationProfile);

        protected float normalClamp(float num)
        {
            return MathHelper.Clamp(num, 0.0f, 1.0f);
        }

        protected void generateCentralMountain(ref float heightNormal, float ratioFromCenter, int x, int z)
        {

            float pinnacle = (float)Math.Pow(ratioFromCenter, 1.5);


            heightNormal = pinnacle;

            //erosion
            heightNormal -= (1.0f - (float)(Math.Pow(heightNormal, 3)))
                / (float)((float)NoiseGenerator.Noise(x * 2, z * 2) + .54f) / 6.0f;


        }

    }
}
