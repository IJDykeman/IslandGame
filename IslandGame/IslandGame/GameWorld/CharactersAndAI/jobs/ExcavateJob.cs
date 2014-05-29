using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    class ExcavateJob : MultiBlockOngoingJob
    {
        ExcavationSite site;
        WalkToAndDestroyBlockJob currentWalkToAndDestroy;
        Character character;
        

        public ExcavateJob(ExcavationSite nSite, Character nCharacter)
        {
            site = nSite;
            character = nCharacter;
            setJobType(JobType.mining);
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (site.getBlocksToRemove().Count > 0)
            {
                updateCurrentWalkAndDestroy(taskTracker);
                if (currentWalkToAndDestroy.isUseable())
                {
                    return currentWalkToAndDestroy.getCurrentTask(taskTracker);
                }
                else
                {
                    return new CharacterTask.NoTask();
                }

            }
            else
            {
                return new CharacterTask.NoTask();
            }
        }

        private void updateCurrentWalkAndDestroy(CharacterTaskTracker taskTracker)
        {
            List<BlockLoc> blocksToRemove = site.getBlocksToRemove();
            if (blocksToRemove.Count > 0)
            {
                if (currentWalkToAndDestroy != null && currentWalkToAndDestroy.isUseable()&& currentWalkToAndDestroy.isComplete() == false )
                {
                    return;
                }
                else
                {
                    BlockLoc toDestroy;

                    foreach (BlockLoc claimed in taskTracker.blocksCurrentlyClaimed())
                    {
                        blocksToRemove.Remove(claimed);

                    }
                    PathHandler pathHandler = new PathHandler();
                    List<BlockLoc> path = pathHandler.getPathToMakeTheseBlocksAvaiable(site.getProfile(),
                        new BlockLoc(character.getFootLocation()), site.getProfile(),
                        blocksToRemove, 2, out toDestroy);
                    TravelAlongPath walkTask = new TravelAlongPath(path);
                    currentWalkToAndDestroy = new WalkToAndDestroyBlockJob(new TravelAlongPath(path), new CharacterTask.DestroyBlock(toDestroy));
                }
            }
            else
            {

            }
        }

        public override bool isComplete()
        {
            return !currentWalkToAndDestroy.isUseable() || site.getBlocksToRemove().Count==0 ;
        }

        public override bool isUseable()
        {
            return true;
        }

        public override BlockLoc? getCurrentGoalBlock()
        {
            if (currentWalkToAndDestroy != null && currentWalkToAndDestroy.isUseable())
            {
                return currentWalkToAndDestroy.getGoalBlock();
            }
            return null;


        }


    }
}
