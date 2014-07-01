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
        Character character;
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
                Path path = pathHandler.getPathToBlockEnumerable(farm.getProfile(),
                    new BlockLoc(character.getFootLocation()), farm.getProfile(),
                    nextBlocksToTend, 2);
                
                TravelAlongPath travelJob = new TravelAlongPath(path);

                if (travelJob.isUseable())
                {
                    BlockLoc? toTend = path.getLast();
                    if (toTend != null)
                    {
                        TravelAlongPath travelToSwitchTo = new TravelAlongPath(path, new TendFarmBlockJob((BlockLoc)toTend, workingProfile, farm, character));
                        return new CharacterTask.SwitchJob(travelToSwitchTo);
                    }
                    return new CharacterTask.NoTask();
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

    }
}
