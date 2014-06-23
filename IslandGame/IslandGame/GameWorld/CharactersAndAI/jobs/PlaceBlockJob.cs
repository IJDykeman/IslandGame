using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    class PlaceBlockJob : Job
    {
        WoodBuildSite buildSite;
        WaitJob currentWait = null;
        Character character;
        BlockLoc placementloc;
        bool hasFailedToFindOneBlockToBuild = false;
        bool hasPlacedBlock = false;
        private IslandWorkingProfile workingProfile;


        public PlaceBlockJob(WoodBuildSite nBuildSite, Character nCharacter, BlockLoc placeToPlaceBlock, IslandWorkingProfile nworkingProfile)
        {
            workingProfile = nworkingProfile;
            buildSite = nBuildSite;
            character = nCharacter;
            setJobType(JobType.building);
            placementloc = placeToPlaceBlock;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {

            if ( !hasPlacedBlock && buildSite.numBlocksLeftToBuild() > 0 && buildSite.containsBlockToBuild(placementloc))
            {
                hasPlacedBlock = true;
                return new CharacterTask.BuildBlock(placementloc, (byte)5);

            }

            else
            {
                                return new CharacterTask.SwitchJob(new BuildKickoffJob(buildSite, character,workingProfile));

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
            return placementloc;
        }

    }
}
