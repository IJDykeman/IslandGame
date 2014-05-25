using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    class ResourceBlock : JobSite
    {
        BlockLoc location;

        public override float? intersects(Ray ray)
        {
            return location.intersects(ray);
        }

        public override Job getJob(Character newWorker)
        {
            //return new BuildJob(this, newWorker);
            return new UnemployedJob();
        }





        public override void blockWasBuilt(BlockLoc toDestroy)
        {

        }


        public override void blockWasDestroyed(BlockLoc toDestroy)
        {
            return;
        }



        public override void draw(GraphicsDevice device, Effect effect)
        {

                WorldMarkupHandler.addFlagPathWithPosition(@"C:\Users\Public\CubeStudio\resources\log.chr",
                                           location.getMiddleInWorldSpace());

        }



        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            result.Add(location);
            return result;
        }
    }
}
