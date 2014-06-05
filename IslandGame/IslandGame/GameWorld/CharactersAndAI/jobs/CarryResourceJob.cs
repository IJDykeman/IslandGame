using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IslandGame.GameWorld
{
    [Serializable]
    class CarryResourceToStockpileJob : MultiBlockOngoingJob
    {
        ResourceBlockjobSite resourceJobsite;
        IslandPathingProfile pathingProfile;
        TravelAlongPath currentWalkJob;
        Character character;
        BlockLoc currentGoalBlock;
        bool hasFailedToFindBlock = false;
        ResourceBlock.ResourceType carriedType;
        bool hasDroppedLoad = false;
        Job jobToReturnToWhenDone;


        public CarryResourceToStockpileJob(ResourceBlockjobSite nresourceJobsite, ResourceBlock.ResourceType nCarriedType, 
            Character nCharacter, IslandPathingProfile nPathingProfile, Job nJobToReturnTo)
        {
            jobToReturnToWhenDone = nJobToReturnTo;
            pathingProfile = nPathingProfile;
            resourceJobsite = nresourceJobsite;
            carriedType = nCarriedType;
            character = nCharacter;
            setJobType(JobType.CarryingWood);
            setWalkTaskToPlaceResourceInStockpile();

        }

        private void setWalkTaskToPlaceResourceInStockpile()
        {
            List<BlockLoc> goalsForBlockPlacement = resourceJobsite.getBlocksToStoreThisTypeIn(carriedType).ToList();
            BlockLoc blockToPlaceResourceIn;
            PathHandler pathHandler = new PathHandlerPreferringLowerBlocks();

            if (goalsForBlockPlacement.Count > 0)
            {
                currentWalkJob = new TravelAlongPath(pathHandler.
                    getPathToMakeTheseBlocksAvaiable(pathingProfile, new BlockLoc(character.getFootLocation()),
                    pathingProfile, goalsForBlockPlacement, 2, out blockToPlaceResourceIn));

                currentGoalBlock = blockToPlaceResourceIn;
            }
            else
            {
                List<BlockLoc> noPath = new List<BlockLoc>();
                noPath.Add(new BlockLoc(character.getFootLocation()));
                //TODO: make it find the nearest good space to place a resource block

               currentWalkJob = new TravelAlongPath(noPath);

                currentGoalBlock = new BlockLoc(character.getFootLocation());
            }
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            //TODO make path object so that there is no edge case where a 
            //stockpile that is full and adjacent to the character does not cause a crash
            if (currentWalkJob.isUseable() && !currentWalkJob.isComplete())
            {
                
                return currentWalkJob.getCurrentTask(taskTracker);
            }
            else if (!hasDroppedLoad && currentWalkJob.isUseable())
            {
                hasDroppedLoad = true;
                return new CharacterTask.PlaceResource(currentGoalBlock, carriedType);
            }
            else
            {

                return new CharacterTask.SwitchJob(jobToReturnToWhenDone);

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
