using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IslandGame.GameWorld
{
    [Serializable]
    class PickUpResourceJob : MultiBlockOngoingJob
    {

        Character character;
        BlockLoc whereToPickUpRescource;
        bool hasFailedToFindBlock = false;
        ResourceBlock.ResourceType carriedType;
        bool hasPickedUpLoad = false;
        Job jobToReturnTo;
        IslandWorkingProfile workingProfile;



        public PickUpResourceJob(ResourceBlock.ResourceType nCarriedType,
            Character nCharacter, Job njobToReturnTo, IslandWorkingProfile nworkingProfile, BlockLoc nLocToPlace)
        {

            whereToPickUpRescource = nLocToPlace;
            jobToReturnTo = njobToReturnTo;
            carriedType = nCarriedType;
            character = nCharacter;
            setJobType(JobType.CarryingWood);
            workingProfile = nworkingProfile;
            if (jobToReturnTo == null)
            {
                jobToReturnTo = new UnemployedJob();
            }

        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (!hasPickedUpLoad)
            {
                hasPickedUpLoad = true;
                return new CharacterTask.PickUpResource(whereToPickUpRescource, carriedType);
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
            result.Add(whereToPickUpRescource);
            if (jobToReturnTo != null)
            {
                result.AddRange(jobToReturnTo.getGoalBlock());
            }
            return result;

        }




    }
}
