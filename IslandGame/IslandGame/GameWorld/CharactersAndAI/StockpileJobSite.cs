using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    class StockpileJobSite : JobSite
    {
        HashSet<BlockLoc> storageSpace;
        ResourceBlock.ResourceType typeToStore;

        public StockpileJobSite(IEnumerable<BlockLoc> blockLocsForStorage)
        {
            storageSpace = new HashSet<BlockLoc>();
            foreach (BlockLoc toAdd in blockLocsForStorage)
            {
                storageSpace.Add(toAdd);
            }
            typeToStore = ResourceBlock.ResourceType.Wood;
        }

        public ResourceBlock.ResourceType typeStored()
        {
            return typeToStore;
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
            foreach (BlockLoc test in storageSpace)
            {
                WorldMarkupHandler.addFlagPathWithPosition(ContentDistributor.getRootPath() + @"worldMarkup\farmMarker.chr",
                    test.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f));
            }

        }



        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc test in storageSpace)
            {
                result.Add(test);
            }
            return result;
        }
    }
}
