using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IslandGame.GameWorld.CharactersAndAI;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld
{
    [Serializable]
    class Farm : MultiblockJobSite
    {
        //HashSet<BlockLoc> BlocksToBeFarmedIn;
        Dictionary<BlockLoc, FarmPlantBlock> PlantBlocks;
        

        public Farm(IslandPathingProfile nProfile, IEnumerable<BlockLoc> nBlocksToBeFarmedOnTopOf)
        {
            PlantBlocks = new Dictionary<BlockLoc, FarmPlantBlock>();
            foreach (BlockLoc test in nBlocksToBeFarmedOnTopOf)
            {
                PlantBlocks.Add(test.getVector3WithAddedIntvec(new IntVector3(0,1,0)), new FarmPlantBlock());
            }
            profile = nProfile;
        }

        public override Job getJob(Character newWorker) { 
            return new FarmJob(this, newWorker); 
        }

        public override float? intersects(Microsoft.Xna.Framework.Ray ray)
        {
            return intersects(ray, PlantBlocks.Keys.ToList());
        }

        public override void blockWasDestroyed(BlockLoc toDestroy)
        {
            PlantBlocks.Remove(toDestroy);
            PlantBlocks.Remove(BlockLoc.AddIntVec3(toDestroy, new IntVector3(0, 1, 0)));
        }

        public override void blockWasBuilt(BlockLoc toDestroy)
        {
            PlantBlocks.Remove(toDestroy);
        }

        public override void draw(GraphicsDevice device, Effect effect)
        {
            foreach (BlockLoc test in PlantBlocks.Keys)
            {
                string number = PlantBlocks[test].getGrowthLevel() + "";
                WorldMarkupHandler.addFlagPathWithPosition(@"C:\Users\Public\CubeStudio\worldMarkup\wheatGrowthStage"+number+".chr",
                                           test.toWorldSpaceVector3() + new Vector3(.5f, -.5f, .5f));
            }
        }

        public int getNumFarmBlocks()
        {
            return PlantBlocks.Count();
        }

        public IEnumerable<BlockLoc> getBlocksToBeFarmedIn()
        {
            return PlantBlocks.Keys;
        }

        internal IEnumerable<BlockLoc> getBlocksNeedingTending()
        {
            byte leastGrownBlockLevel = byte.MaxValue;
            List<BlockLoc> leastGrownBlocks = new List<BlockLoc>();
            List<BlockLoc> toRemove = new List<BlockLoc>();
            foreach (BlockLoc test in PlantBlocks.Keys)
            {
                if(PlantBlocks[test].getGrowthLevel()<leastGrownBlockLevel)
                {
                    leastGrownBlockLevel = PlantBlocks[test].getGrowthLevel();
                    leastGrownBlocks.Clear();
                    leastGrownBlocks.Add(test);
                }
                else if (PlantBlocks[test].getGrowthLevel() == leastGrownBlockLevel)
                {
                    leastGrownBlocks.Add(test);
                }



            }

            return leastGrownBlocks;
        }

        public override void makeFarmBlockGrow(BlockLoc toFarm)
        {
            if (PlantBlocks.ContainsKey(toFarm))
            {
                PlantBlocks[toFarm].growOneStage();
            }
        }

        internal bool allBlocksAreGrown()
        {
            foreach (BlockLoc key in PlantBlocks.Keys)
            {
                if (!PlantBlocks[key].isFullyGrown())
                {
                    return false;
                }
            }
            return true; 
        }

        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc test in getBlocksToBeFarmedIn())
            {
                result.Add(test);
            }
            return result;
        }
    }
}
