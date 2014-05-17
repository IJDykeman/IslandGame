using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    public abstract class ObjectBuildJobSite : JobSite
    {
        int framesOfWork=0;
        int framesOfWorkToComplete = 60 * 4;
        protected BlockLoc objectLoc;
        bool objectHasBeenBuilt;
        



        public override float? intersects(Microsoft.Xna.Framework.Ray ray)
        {
            return new BoundingBox(objectLoc.toWorldSpaceVector3() - new Vector3(1, 1, 1), 
                objectLoc.toWorldSpaceVector3() + new Vector3(2, 2, 2)).Intersects(ray);
        }

        public void buildForAFrame()
        {
            framesOfWork++;
        }

        public bool isReadyToBeBuilt()
        {
            if (framesOfWork >= framesOfWorkToComplete && !objectHasBeenBuilt)
            {
                objectHasBeenBuilt = true;
                return true;
            }
            return false;
        }

        public override bool isComplete()
        {
            return objectHasBeenBuilt;
        }

        public BlockLoc getObjectLoc()
        {
            return objectLoc;
        }

    }
}
