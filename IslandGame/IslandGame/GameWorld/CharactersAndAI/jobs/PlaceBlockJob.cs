using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    class PlaceBlockJob : Job
    {
        WoodBuildSite buildSite;
        Character character;
        bool hasFailedToFindOneBlockToBuild = false;
        bool hasPlacedBlock = false;
        private IslandWorkingProfile workingProfile;


        public PlaceBlockJob(WoodBuildSite nBuildSite, Character nCharacter, BlockLoc placeToPlaceBlock, IslandWorkingProfile nworkingProfile)
        {
            workingProfile = nworkingProfile;
            buildSite = nBuildSite;
            character = nCharacter;
            setJobType(JobType.building);
            targetBlock = placeToPlaceBlock;
        }

        public PlaceBlockJob(WoodBuildSite nBuildSite, Character nCharacter, BlockLoc placeToPlaceBlock, Job nToReturnTo, IslandWorkingProfile nworkingProfile)
        {
            workingProfile = nworkingProfile;
            buildSite = nBuildSite;
            character = nCharacter;
            setJobType(JobType.building);
            targetBlock = placeToPlaceBlock;
            toReturnTo = nToReturnTo;
        }


        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {

            if ( !hasPlacedBlock && buildSite.numBlocksLeftToBuild() > 0 && buildSite.containsBlockToBuild(targetBlock))
            {
                hasPlacedBlock = true;
                return new CharacterTask.BuildBlock(targetBlock, (byte)5);

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
