﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IslandGame.GameWorld
{
    [Serializable]
    class PlaceResourceJob : MultiBlockOngoingJob
    {

        Character character;
        ResourceBlock.ResourceType carriedType;
        bool hasDroppedLoad = false;
        IslandWorkingProfile workingProfile;



        public PlaceResourceJob(ResourceBlock.ResourceType nCarriedType,
            Character nCharacter, Job njobToReturnTo, IslandWorkingProfile nworkingProfile, BlockLoc nLocToPlace)
        {
            targetBlock = nLocToPlace;
            toReturnTo = njobToReturnTo;
            carriedType = nCarriedType;
            character = nCharacter;
            setJobType(JobType.CarryingSomething);
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
