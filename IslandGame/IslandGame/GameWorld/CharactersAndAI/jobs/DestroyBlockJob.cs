using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
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
                    character.pickUpItem(ResourceBlock.ResourceType.Stone);
                    return new CharacterTask.DestroyBlock(targetBlock);
            }
            else
            {
                return new CharacterTask.SwitchJob(new CarryResourceToStockpileKickoffJob(ResourceBlock.ResourceType.Stone,character,
                    new ExcavateKickoffJob(workingProfile,character), workingProfile));
            }

        }
    }
}
