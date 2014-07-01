using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    class ChopTreeJob : MultiBlockOngoingJob
    {
        Character character;

        bool failedToFindATreeToChop = false;
        IslandWorkingProfile workingProfile;



        public ChopTreeJob(Character nCharacter, IslandWorkingProfile nWorkingProfile, BlockLoc blockToChop)
        {
            targetBlock = blockToChop;

            character = nCharacter;
            setJobType(JobType.logging);
            workingProfile =nWorkingProfile;
        }

        public override bool isComplete()
        {
            return !workingProfile.getTreeJobSite().hasAtLeastOneTree() || failedToFindATreeToChop;
        }

        public override bool isUseable()
        {
            return true;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (workingProfile.getTreeJobSite().getTreeTrunkBlocks().Count > 0)
            {

                if (workingProfile.getTreeJobSite().getTreeTrunkBlocks().Contains(targetBlock))
                {
                    return new CharacterTask.ChopBlockForFrame(targetBlock);
                }
                else
                {
                    character.pickUpItem(ResourceBlock.ResourceType.Wood);
                    return new CharacterTask.SwitchJob(new CarryResourceToStockpileJob(
                        ResourceBlock.ResourceType.Wood,
                        character,
                        new LoggingKickoffJob(character, workingProfile), workingProfile));
                    

                }

            }
            else
            {
                return new CharacterTask.SwitchJob(new UnemployedJob());
            }

        }


    }
}
