using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    class WoodBuildSite : MultiblockJobSite
    {
        protected HashSet<BlockLoc> blocksToBuild;
        protected string markerName = "stoneMarkerBlock";


        public WoodBuildSite(IslandPathingProfile nProfile)
        {
            blocksToBuild = new HashSet<BlockLoc>();
            profile = nProfile;
        }

        public override float? intersects(Ray ray)
        {
            return Intersection.intersects(ray, blocksToBuild);
        }

        public override Job getJob(Character newWorker, Ray ray, IslandWorkingProfile workingProfile)
        {
            return new BuildKickoffJob(this, newWorker);
        }

        public int numBlocksLeftToBuild()
        {
            return blocksToBuild.Count;
        }

        public bool siteIsComplete()
        {
            return numBlocksLeftToBuild() == 0;
        }



        public List<BlockLoc> getNextBlocksToBuild()
        {
            return blocksToBuild.ToList();
        }


        public override void blockWasDestroyed(BlockLoc toDestroy)
        {
            return;
        }

        public override void blockWasBuilt(BlockLoc toDestroy)
        {
            blocksToBuild.Remove(toDestroy);
        }

        public override void draw(GraphicsDevice device, Effect effect)
        {
            foreach (BlockLoc test in blocksToBuild)
            {
                WorldMarkupHandler.addCharacter(ContentDistributor.getRootPath()+@"worldMarkup\"+markerName+".chr",
                                           test.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f), 1.0f/12.0f,.6f);
            }
        }


        public void addBlock(BlockLoc newBlockToPlace)
        {
            blocksToBuild.Add(newBlockToPlace);
        }

        public bool containsBlockToBuild(BlockLoc currentGoalBlock)
        {
            return blocksToBuild.Contains(currentGoalBlock);
        }

        public void removeBlock(BlockLoc newBlockToPlace)
        {
            blocksToBuild.Remove(newBlockToPlace);
        }

        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc test in blocksToBuild)
            {
                result.Add(test);
            }
            return result;
        }
    }
}
