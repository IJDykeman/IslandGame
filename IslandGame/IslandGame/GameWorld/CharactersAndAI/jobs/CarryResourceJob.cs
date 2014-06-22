using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IslandGame.GameWorld
{
    [Serializable]
    class CarryResourceToStockpileJob : MultiBlockOngoingJob
    {

        Character character;

        bool hasFailedToFindBlock = false;
        ResourceBlock.ResourceType carriedType;
        Job jobToReturnTo;
        IslandWorkingProfile workingProfile;
        BlockLoc blockToPlaceResourceIn;



        public CarryResourceToStockpileJob(ResourceBlock.ResourceType nCarriedType,
            Character nCharacter, Job njobToReturnTo, IslandWorkingProfile nworkingProfile)
        {
            jobToReturnTo = njobToReturnTo;
            carriedType = nCarriedType;
            character = nCharacter;
            setJobType(JobType.CarryingWood);
            workingProfile = nworkingProfile;

        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            //return new CharacterTask.PlaceResource(whereToPlaceRescource, carriedType);
            List<BlockLoc> goalsForBlockPlacement = workingProfile.getResourcesJobSite().getBlocksToStoreThisTypeIn(carriedType).ToList();
           
            PathHandler pathHandler = new PathHandlerPreferringLowerBlocks();

            if (goalsForBlockPlacement.Count > 0)
            {
                TravelAlongPath walkJob;
                 
                Path path = pathHandler.
                    getPathToMakeTheseBlocksAvaiable( workingProfile.getPathingProfile(), new BlockLoc(character.getFootLocation()),
                    workingProfile.getPathingProfile(), goalsForBlockPlacement, 2, out blockToPlaceResourceIn);

                Job toSwichToAfterWalk = new PlaceResourceJob(carriedType, character, 
                    jobToReturnTo, workingProfile, blockToPlaceResourceIn);

                walkJob = new TravelAlongPath(path,toSwichToAfterWalk);

                
                return new CharacterTask.SwitchJob(walkJob);
            }
            else
            {
                //TODO: make it find the nearest good maybeSpace to place a resource block
                return new CharacterTask.SwitchJob(new UnemployedJob());
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

        public override BlockLoc? getGoalBlock()
        {
            return blockToPlaceResourceIn;
        }


    }
}
