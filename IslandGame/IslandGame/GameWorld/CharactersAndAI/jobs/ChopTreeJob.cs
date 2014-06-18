using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    class ChopTreeJob : MultiBlockOngoingJob
    {
        BlockLoc currentBlockToChop;
        WaitJob currentWait = null;
        Character character;

        bool failedToFindATreeToChop = false;
        BlockLoc currentGoalBlock;
        IslandWorkingProfile workingProfile;



        public ChopTreeJob(Character nCharacter, IslandWorkingProfile nWorkingProfile, BlockLoc blockToChop)
        {
            currentGoalBlock = blockToChop;

            character = nCharacter;
            setJobType(JobType.logging);
            workingProfile =nWorkingProfile;
        }

        public override BlockLoc? getCurrentGoalBlock()
        {
            return currentBlockToChop;
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

                if (workingProfile.getTreeJobSite().getTreeTrunkBlocks().Contains(currentGoalBlock))
                {
                    return new CharacterTask.ChopBlockForFrame(currentGoalBlock);
                }
                else
                {

                    return new CharacterTask.SwitchJob(new CarryResourceToStockpileJob(
                        ResourceBlock.ResourceType.Wood,
                        character,
                        new LoggingJob(character, workingProfile), workingProfile));

                }

            }
            else
            {
                return new CharacterTask.SwitchJob(new UnemployedJob());
            }

        }


    }
}
