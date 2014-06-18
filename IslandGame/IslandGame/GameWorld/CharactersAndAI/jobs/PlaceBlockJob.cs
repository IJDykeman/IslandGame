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


        public PlaceBlockJob(WoodBuildSite nBuildSite, Character nCharacter, BlockLoc placeToPlaceBlock)
        {
            buildSite = nBuildSite;
            character = nCharacter;
            setJobType(JobType.building);
            placementloc = placeToPlaceBlock;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (hasPlacedBlock)
            {
                return new CharacterTask.SwitchJob(new BuildKickoffJob(buildSite, character));
            }

            if (buildSite.numBlocksLeftToBuild() > 0 && buildSite.containsBlockToBuild(placementloc))
            {
                hasPlacedBlock = true;
                return new CharacterTask.BuildBlock(placementloc, (byte)5);

            }

            else
            {
                if (placementloc != null)
                {
                    return new CharacterTask.LookTowardPoint(placementloc.toWorldSpaceVector3()
                        + new Microsoft.Xna.Framework.Vector3(1, 1, 1) / 2f);

                }
                return new CharacterTask.NoTask();
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
