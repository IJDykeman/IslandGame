using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    class FarmJob : MultiBlockOngoingJob
    {
        Farm farm;
        TravelAlongPath currentWalkJob;
        WaitJob currentWait = null;
        Character character;
        BlockLoc currentGoalBlock;
        bool hasFailedToFindBlock = false;


        public FarmJob(Farm nFarm, Character nCharacter)
        {
            farm = nFarm;
            character = nCharacter;
            setJobType(JobType.agriculture);
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (farm.getNumFarmBlocks() > 0)
            {
                CharacterTask.Task maybeFarm =  updateCurrentTask(taskTracker);
                if (maybeFarm.taskType != CharacterTask.Type.NoTask)
                {
                    return maybeFarm;
                }
                if (currentWalkJob.isUseable() && (currentWait==null || currentWait.isComplete()))
                {
                    return currentWalkJob.getCurrentTask(taskTracker);
                }
                else
                {
                    return new CharacterTask.LookTowardPoint(currentGoalBlock.toWorldSpaceVector3() + new Microsoft.Xna.Framework.Vector3(1, 1, 1) / 2f);
                }

            }
            else
            {
                return new CharacterTask.NoTask();
            }
        }

        private CharacterTask.Task updateCurrentTask(CharacterTaskTracker taskTracker)
        {

                if (farm.getNumFarmBlocks() > 0)
                {
                    if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.isComplete() == false )
                    {
                        return new CharacterTask.NoTask();
                    }
                    else
                    {
                        if (currentWait != null && !currentWait.isComplete())//if the wait needs to be updated
                        {
                            currentWait.update();
                            if (currentWait.isComplete())
                            {
                                setWalkTaskToNextBlockInFarm(taskTracker);
                                return new CharacterTask.MakeFarmBlockGrow(currentGoalBlock);
                                
                            }
                        }
                        else
                        {
                            if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.isComplete() == true)
                            {

                                    
                                    currentWait = new WaitJob(30);
                                    return new CharacterTask.NoTask();
                                
                            }

                            setWalkTaskToNextBlockInFarm(taskTracker);     
                        }
                    }
                }
                return new CharacterTask.NoTask();

            

        }

        private void setWalkTaskToNextBlockInFarm(CharacterTaskTracker taskTracker)
        {
            List<BlockLoc> nextBlocksToTend = farm.getBlocksNeedingTending().ToList();

            foreach (BlockLoc claimed in taskTracker.blocksCurrentlyClaimed())
            {
                nextBlocksToTend.Remove(claimed);
               
            }
            PathHandler pathHandler = new PathHandler();
            List<BlockLoc> path = pathHandler.getPathToBlockEnumerable(farm.getProfile(),
                new BlockLoc(character.getFootLocation()), farm.getProfile(),
                nextBlocksToTend, 2);
            currentWalkJob = new TravelAlongPath(path);
            if (currentWalkJob.isUseable())
            {
                currentGoalBlock = currentWalkJob.getGoalBlock();
            }
            else
            {
                hasFailedToFindBlock = true;
            }
        }

        public override bool isComplete()
        {
            return hasFailedToFindBlock;
        }

        public override bool isUseable()
        {
            return !hasFailedToFindBlock;
        }

        public override BlockLoc? getCurrentGoalBlock()
        {
            if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.willResultInTravel())
            {
                return currentWalkJob.getGoalBlock();
            }
            return null;
        }

    }
}
