using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{

    interface Intersectable
    {
        float? intersects(Ray ray);


    }


    static class Intersection
    {
        public static Intersectable getNearestIntersectableAlongRay(Ray ray, IEnumerable<Intersectable> intersectables)
        {
            float? minDist = float.MaxValue;
            Intersectable result = null;
            foreach (Intersectable site in intersectables)
            {
                float? thisDist = site.intersects(ray);
                if (thisDist.HasValue)
                {
                    if (minDist > thisDist)
                    {
                        minDist = thisDist;
                        result = site;
                    }
                }
            }
            return result;
        }

        public static float? intersects(Ray ray, IEnumerable<Intersectable> intersectables)
        {
            float? minDist = float.MaxValue;
            Intersectable result = null;
            foreach (Intersectable site in intersectables)
            {
                float? thisDist = site.intersects(ray);
                if (thisDist.HasValue)
                {
                    if (minDist > thisDist)
                    {
                        minDist = thisDist;
                        result = site;
                    }
                }
            }
            return minDist;
        }

        public static float? intersects(Microsoft.Xna.Framework.Ray ray, IEnumerable<BlockLoc> blockList)
        {
            float? minDist = null;
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
