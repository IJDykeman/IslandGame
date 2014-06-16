using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IslandGame.GameWorld.CharactersAndAI.jobs;

namespace IslandGame.GameWorld
{
    [Serializable]
    class FarmingKickoffJob : MultiBlockOngoingJob
    {
        Farm farm;
        WaitJob currentWait = null;
        Character character;
        BlockLoc currentGoalBlock;
        bool hasFailedToFindBlock = false;
        IslandWorkingProfile workingProfile;


        public FarmingKickoffJob(Farm nFarm, Character nCharacter, IslandWorkingProfile nWorkingProfile)
        {
            farm = nFarm;
            character = nCharacter;
            setJobType(JobType.agriculture);
            workingProfile = nWorkingProfile;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (farm.getNumFarmBlocks() > 0)
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
                
                TravelAlongPath travelJob = new TravelAlongPath(path);

                if (travelJob.isUseable())
                {
                    BlockLoc toTend = path[path.Count - 1];

                    TravelAlongPath travelToSwitchTo = new TravelAlongPath(path, new TendFarmBlockJob(toTend, workingProfile, farm, character));
                    return new CharacterTask.SwitchJob(travelToSwitchTo);
                }
                else
                {
                    hasFailedToFindBlock = true;
                    return new CharacterTask.NoTask();
                }
            }
            else
            {
                return new CharacterTask.NoTask();
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
            return null;
        }

    }
}
