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
        IslandWorkingProfile workingProfile;



        public ChopTreeJob(Character nCharacter, IslandWorkingProfile nWorkingProfile, BlockLoc blockToChop)
        {
            targetBlock = blockToChop;

            character = nCharacter;
            setJobType(JobType.logging);
            workingProfile =nWorkingProfile;
        }

        public override List<BlockLoc> getGoalBlock()
        {
            List<BlockLoc> result = new List<BlockLoc>();
            result.Add(currentBlockToChop);
            return result;
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
