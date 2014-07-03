using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IslandGame.GameWorld
{
    [Serializable]
    class CarryResourceToStockpileKickoffJob : MultiBlockOngoingJob
    {

        Character character;

        bool hasFailedToFindBlock = false;
        ResourceBlock.ResourceType carriedType;
        IslandWorkingProfile workingProfile;



        public CarryResourceToStockpileKickoffJob(ResourceBlock.ResourceType nCarriedType,
            Character nCharacter, Job njobToReturnTo, IslandWorkingProfile nworkingProfile)
        {
            toReturnTo = njobToReturnTo;
            carriedType = nCarriedType;
            character = nCharacter;
            setJobType(JobType.CarryingSomething);
            workingProfile = nworkingProfile;

        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            //return new CharacterTask.PlaceResource(targetBlock, carriedType);
            List<BlockLoc> goalsForBlockPlacement = workingProfile.getResourcesJobSite().getBlocksToStoreThisTypeIn(carriedType).ToList();
           
            foreach(BlockLoc test in taskTracker.blocksCurrentlyClaimed())
            {
                goalsForBlockPlacement.Remove(test);
            }

            PathHandler pathHandler = new PathHandlerPreferringLowerBlocks();

            if (goalsForBlockPlacement.Count > 0)
            {
                TravelAlongPath walkJob;
                 
                Path path = pathHandler.
                    getPathToMakeTheseBlocksAvaiable( workingProfile.getPathingProfile(), new BlockLoc(character.getFootLocation()),
                    workingProfile.getPathingProfile(), goalsForBlockPlacement, 2, out targetBlock);

                Job toSwichToAfterWalk = new PlaceResourceJob(carriedType, character, 
                    toReturnTo, workingProfile, targetBlock);

                walkJob = new TravelAlongPath(path,getWaitJobWithReturn(5, toSwichToAfterWalk));
                if(path.length()==0){
                    return new CharacterTask.SwitchJob(new UnemployedJob());
                }
                
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




    }
}
