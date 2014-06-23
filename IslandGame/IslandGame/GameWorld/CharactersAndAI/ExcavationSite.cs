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
    public class ExcavationSite : MultiblockJobSite
    {          
        HashSet<BlockLoc> blocksToBeRemoved;


        public ExcavationSite(IslandPathingProfile nProfile)
        {
            blocksToBeRemoved = new HashSet<BlockLoc>();
            profile = nProfile;
        }

        public void addBlockToDestroy(BlockLoc blockLoc)
        {
            if (!blocksToBeRemoved.Contains(blockLoc))
            {
                blocksToBeRemoved.Add(blockLoc);
            }

        }

        public List<BlockLoc> getBlocksToRemove()
        {
            return blocksToBeRemoved.ToList();
        }



        public override void blockWasDestroyed(BlockLoc toDestroy)
        {
            blocksToBeRemoved.Remove(toDestroy);
        }

        public override void blockWasBuilt(BlockLoc toDestroy)
        {
            return;
        }

        public override float? intersects(Microsoft.Xna.Framework.Ray ray)
        {
            return Intersection.intersects(ray, blocksToBeRemoved);
        }

        public override Job getJob(Character newWorker, Ray ray, IslandWorkingProfile workingProfile)
        {
            return new ExcavateKickoffJob(workingProfile, newWorker);
        }

        public override void draw(GraphicsDevice device, Effect effect, DisplayParameters parameters)
        {
            foreach (BlockLoc test in blocksToBeRemoved)
            {
                WorldMarkupHandler.addCharacter(ContentDistributor.getRootPath() + @"worldMarkup\redCubeOutline.chr",
                                           test.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f), 1.1f/7f,.6f);
            }
        }


        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc test in blocksToBeRemoved)
            {
                result.Add(test);
            }
            return result;
        }

    }
}
