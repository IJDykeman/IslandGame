using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    class PlaceBlockJob : Job
    {
        WoodBuildSite buildSite;
        Character character;
        bool hasFailedToFindOneBlockToBuild = false;
        bool hasPlacedBlock = false;
        private IslandWorkingProfile workingProfile;
        byte typeToPlace;

        public PlaceBlockJob(WoodBuildSite nBuildSite, Character nCharacter, BlockLoc placeToPlaceBlock, 
            Job nToReturnTo, IslandWorkingProfile nworkingProfile, byte nTypeToPlace)
        {
            workingProfile = nworkingProfile;
            buildSite = nBuildSite;
            character = nCharacter;
            setJobType(JobType.building);
            targetBlock = placeToPlaceBlock;
            toReturnTo = nToReturnTo;
            typeToPlace = nTypeToPlace;
        }


        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {

            if ( !hasPlacedBlock && buildSite.numBlocksLeftToBuild() > 0 && buildSite.containsBlockToBuild(targetBlock))
            {
                hasPlacedBlock = true;
                return new CharacterTask.BuildBlock(targetBlock, typeToPlace);
            }
            else
            {
                return new CharacterTask.SwitchJob(toReturnTo);
            }
        }


        public override bool isComplete()
        {
            return buildSite.siteIsComplete() || hasFailedToFindOneBlockToBuild;
        }

        public override bool isUseable()
        {
            return true;
        }

        public BlockLoc? getCurrentGoalBlock()
        {
            return targetBlock;
        }

    }
}
