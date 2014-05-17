using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    abstract class MultiblockJobSite : JobSite
    {

        protected static float? intersects(Microsoft.Xna.Framework.Ray ray, IEnumerable<BlockLoc> blockList)
        {
            float? minDist =  null;
            foreach (BlockLoc test in blockList)
            {
                float? thisIntersection = new BoundingBox(test.toWorldSpaceVector3(), test.toWorldSpaceVector3() + new Vector3(1, 1, 1)).Intersects(ray);
                if (thisIntersection != null)
                {
                    if (minDist == null || (float)minDist > (float)thisIntersection)
                    {
                        minDist = thisIntersection;
                    }
                }
            }
            return minDist;
        }
    }
}
