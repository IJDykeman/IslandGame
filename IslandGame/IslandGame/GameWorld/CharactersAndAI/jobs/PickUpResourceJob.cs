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
        ResourceBlock.ResourceType carriedType;
        bool hasPickedUpLoad = false;
        IslandWorkingProfile workingProfile;



        public PickUpResourceJob(ResourceBlock.ResourceType nCarriedType,
            Character nCharacter, Job njobToReturnTo, IslandWorkingProfile nworkingProfile, BlockLoc nLocToPlace)
        {

            targetBlock = nLocToPlace;
            toReturnTo = njobToReturnTo;
            carriedType = nCarriedType;
            character = nCharacter;
            setJobType(JobType.CarryingSomething);
            workingProfile = nworkingProfile;
            if (toReturnTo == null)
            {
                toReturnTo = new UnemployedJob();
            }

        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (!hasPickedUpLoad)
            {
                hasPickedUpLoad = true;
                return new CharacterTask.PickUpResource(targetBlock, carriedType);
            }
            else 
            {
                return new CharacterTask.SwitchJob(toReturnTo);
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



    }
}
