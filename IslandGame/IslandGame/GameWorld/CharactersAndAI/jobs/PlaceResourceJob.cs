using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IslandGame.GameWorld
{
    [Serializable]
    class PlaceResourceJob : MultiBlockOngoingJob
    {

        Character character;
        bool hasFailedToFindBlock = false;
        ResourceBlock.ResourceType carriedType;
        bool hasDroppedLoad = false;
        Job jobToReturnTo;
        IslandWorkingProfile workingProfile;



        public PlaceResourceJob(ResourceBlock.ResourceType nCarriedType,
            Character nCharacter, Job njobToReturnTo, IslandWorkingProfile nworkingProfile, BlockLoc nLocToPlace)
        {
            targetBlock = nLocToPlace;
            jobToReturnTo = njobToReturnTo;
            carriedType = nCarriedType;
            character = nCharacter;
            setJobType(JobType.CarryingWood);
            workingProfile = nworkingProfile;

        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (!hasDroppedLoad)
            {
                hasDroppedLoad = true;
                return new CharacterTask.PlaceResource(targetBlock, carriedType);
            }
            else 
            {
                return new CharacterTask.SwitchJob(jobToReturnTo);
            }


        }

        public override bool isComplete()
        {
            return false;
        }

        public override bool isUseable()
        {
            return true;
        }

        public override List<BlockLoc> getGoalBlock()
        {
            List<BlockLoc> result = new List<BlockLoc>();
            result.Add(targetBlock);
            if (jobToReturnTo != null)
            {
                result.AddRange(jobToReturnTo.getGoalBlock());
            }
            return result;

        }




    }
}
