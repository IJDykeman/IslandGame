using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    class DestroyBlockJob : MultiBlockOngoingJob
    {
        Character character;
        IslandWorkingProfile workingProfile;



        public DestroyBlockJob(Character nCharacter, IslandWorkingProfile nWorkingProfile, BlockLoc blockToDestroy)
        {
            targetBlock = blockToDestroy;

            character = nCharacter;
            setJobType(JobType.mining);
            workingProfile =nWorkingProfile;
        }

        public override List<BlockLoc> getGoalBlock()
        {
            List<BlockLoc> result = new List<BlockLoc>();
            result.Add(targetBlock);
            return result;
        }

        public override bool isComplete()
        {
            return false;
        }

        public override bool isUseable()
        {
            return true;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (workingProfile.getPathingProfile().isProfileSolidAt(targetBlock))
            {
                    return new CharacterTask.DestroyBlock(targetBlock);
            }
            else
            {
                return new CharacterTask.SwitchJob(new CarryResourceToStockpileJob(ResourceBlock.ResourceType.Stone,character,
                    new ExcavateKickoffJob(workingProfile,character), workingProfile));
            }

        }
    }
}
