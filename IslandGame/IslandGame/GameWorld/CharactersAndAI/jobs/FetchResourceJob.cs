using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IslandGame.GameWorld
{
    [Serializable]
    class FetchResourceJob : MultiBlockOngoingJob
    {
        
        IslandWorkingProfile workingProfile;
        TravelAlongPath currentWalkJob;
        Character character;
        BlockLoc currentGoalBlock;
        bool hasFailedToFindBlock = false;
        ResourceBlock.ResourceType typeToFetch;
        bool hasTriedToPickUpResource = false;
        Job jobToReturnTo;



        public FetchResourceJob(IslandWorkingProfile nworkingProfile, ResourceBlock.ResourceType nTypeToFetch,
            Character nCharacter, Job njobToReturnTo)
        {
            jobToReturnTo = njobToReturnTo;
            workingProfile = nworkingProfile;
            typeToFetch = nTypeToFetch;
            character = nCharacter;
            setJobType(JobType.none);
            setWalkTaskToPickUpResourceInStockpile();

        }

        private void setWalkTaskToPickUpResourceInStockpile()
        {
            List<BlockLoc> goalsForBlockPickup = workingProfile.getBlocksToGetThisTypeFrom(typeToFetch).ToList();
            BlockLoc blockToPlaceResourceIn;
            PathHandler pathHandler = new PathHandler();

            if (goalsForBlockPickup.Count > 0)
            {
                currentWalkJob = new TravelAlongPath(pathHandler.
                    getPathToMakeTheseBlocksAvaiable(workingProfile.getPathingProfile(), new BlockLoc(character.getFootLocation()),
                    workingProfile.getPathingProfile(), goalsForBlockPickup, 2, out blockToPlaceResourceIn));

                currentGoalBlock = blockToPlaceResourceIn;
            }
            else
            {
                List<BlockLoc> noPath = new List<BlockLoc>();
                noPath.Add(new BlockLoc(character.getFootLocation()));
                //TODO: make it find the nearest good maybeSpace to place a resource block

               currentWalkJob = new TravelAlongPath(noPath);

                currentGoalBlock = new BlockLoc(character.getFootLocation());
            }
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            //TODO make path object so that there is no edge case where a 
            //stockpile that is full and adjacent to the character does not cause a crash

            if (hasTriedToPickUpResource)
            {
                return new CharacterTask.SwitchJob(jobToReturnTo);
            }
            else
            {
                if (currentWalkJob.isUseable() && !currentWalkJob.isComplete())
                {

                    return currentWalkJob.getCurrentTask(taskTracker);
                }
                else if (!hasTriedToPickUpResource && currentWalkJob.isUseable())
                {
                    hasTriedToPickUpResource = true;
                    return new CharacterTask.PickUpResourceBlock(currentGoalBlock, typeToFetch);
                }



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
            if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.willResultInTravel())
            {
                return currentWalkJob.getGoalBlock();
            }
            return null;
        }


    }
}
