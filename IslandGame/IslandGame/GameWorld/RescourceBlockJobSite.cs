using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    class ResourceBlockjobSite : JobSite
    {
        Dictionary<BlockLoc, ResourceBlock> resourceBlocks;

        public ResourceBlockjobSite()
        {
            resourceBlocks = new Dictionary<BlockLoc, ResourceBlock>();
        }

        public override float? intersects(Ray ray)
        {
            return 99000900900;
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
            
        }



        public override void draw(GraphicsDevice device, Effect effect)
        {
            foreach (BlockLoc key in resourceBlocks.Keys)
            {

                WorldMarkupHandler.addFlagPathWithPosition(@"C:\Users\Public\CubeStudio\resources\log.chr",
                                           key.getMiddleInWorldSpace());
            }

        }

        public void placeRescourceBlock(BlockLoc loc, ResourceBlock.ResourceType type)
        {
            resourceBlocks.Add(loc, new ResourceBlock(type));
        }

        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc test in resourceBlocks.Keys)
            {
                result.Add(test);
            }
            return result;
        }
    }
}
