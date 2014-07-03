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
    public class Farm : MultiblockJobSite
    {
        //HashSet<BlockLoc> BlocksToBeFarmedIn;
        Dictionary<BlockLoc, FarmPlantBlock> plantBlocks;
        bool harvestTime;
        

        public Farm(IslandPathingProfile nProfile, IEnumerable<BlockLoc> nBlocksToBeFarmedOnTopOf)
        {
            plantBlocks = new Dictionary<BlockLoc, FarmPlantBlock>();
            foreach (BlockLoc test in nBlocksToBeFarmedOnTopOf)
            {
                plantBlocks.Add(test.getVector3WithAddedIntvec(new IntVector3(0,1,0)), new FarmPlantBlock());
            }
            profile = nProfile;
        }

        public override Job getJob(Character newWorker, Ray ray, IslandWorkingProfile workingProfile)
        { 
            return new FarmingKickoffJob(this, newWorker,workingProfile); 
        }

        public override float? intersects(Microsoft.Xna.Framework.Ray ray)
        {
            return Intersection.intersects(ray, plantBlocks.Keys.ToList());
        }

        public override void blockWasDestroyed(BlockLoc toDestroy)
        {
            plantBlocks.Remove(toDestroy);
            plantBlocks.Remove(BlockLoc.AddIntVec3(toDestroy, new IntVector3(0, 1, 0)));
        }

        public override void blockWasBuilt(BlockLoc toDestroy)
        {
            plantBlocks.Remove(toDestroy);
        }

        public override void draw(GraphicsDevice device, Effect effect, DisplayParameters parameters)
        {
            foreach (BlockLoc test in plantBlocks.Keys)
            {
                string number = plantBlocks[test].getGrowthLevel() + "";
                WorldMarkupHandler.addFlagPathWithPosition(ContentDistributor.getRootPath()+@"worldMarkup\wheatGrowthStage"+number+".chr",
                                           test.toWorldSpaceVector3() + new Vector3(.5f, -.5f, .5f));
            }
        }

        public int getNumFarmBlocks()
        {
            return plantBlocks.Count();
        }

        public IEnumerable<BlockLoc> getBlocksToBeFarmedIn()
        {
            return plantBlocks.Keys;
        }

        private bool allBlocksAreGrown()
        {
            foreach (BlockLoc key in plantBlocks.Keys)
            {
                if (!plantBlocks[key].isFullyGrown())
                {
                    return false;
                }
            }
            return true;
        }

        private bool noBlocksArePlanted()
        {
            foreach (BlockLoc key in plantBlocks.Keys)
            {
                if (plantBlocks[key].getGrowthLevel() != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool noBlocksAreGrown()
        {
            foreach (BlockLoc key in plantBlocks.Keys)
            {
                if (blockIsFullGrown(key))
                {
                    return false;
                }
            }
            return true;
        }


        internal IEnumerable<BlockLoc> getBlocksNeedingTending()
        {
            int leastGrownBlockLevel = int.MaxValue;
            List<BlockLoc> leastGrownBlocks = new List<BlockLoc>();


            if (allBlocksAreGrown())
            {
                harvestTime = true;
            }


            if (harvestTime)
            {
                if (noBlocksArePlanted() || noBlocksAreGrown())
                {
                    harvestTime = false;
                }
                else
                {
                    List<BlockLoc> result = new List<BlockLoc>();
                    foreach (BlockLoc test in plantBlocks.Keys)
                    {
                        if(plantBlocks[test].isFullyGrown())
                        {
                            result.Add(test);
                        }
                    }
                    return result;
                }
            }


            foreach (BlockLoc test in plantBlocks.Keys)
            {

                if (plantBlocks[test].getGrowthLevel() < leastGrownBlockLevel)
                {
                    leastGrownBlockLevel = plantBlocks[test].getGrowthLevel();
                    leastGrownBlocks.Clear();
                    leastGrownBlocks.Add(test);
                }
                else if (plantBlocks[test].getGrowthLevel() == leastGrownBlockLevel)
                {
                    leastGrownBlocks.Add(test);
                }
            }

            return leastGrownBlocks;

        }



        public override void makeFarmBlockGrow(BlockLoc toFarm)
        {
            if (plantBlocks.ContainsKey(toFarm))
            {
                plantBlocks[toFarm].getTendedAndReturnWheatHarvested();
            }
 
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

        public bool blockIsFullGrown(BlockLoc blockToTend)
        {
            if (plantBlocks.ContainsKey(blockToTend))
            {
                return plantBlocks[blockToTend].isFullyGrown();
            }
            return false;
        }
    }
}
