using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI.jobs
{
    class TendFarmBlockJob : Job
    {
        Farm farm;
        WaitJob currentWait = null;
        Character character;
        bool hasTendedBlock = false;
        IslandWorkingProfile workingProfile;
        // new TendFarmBlockJob(path.Last,workingProfile,farm, character)

        public TendFarmBlockJob(BlockLoc nBlockToTend, IslandWorkingProfile nWorkingProfile,
            Farm nfarm,Character ncharacter)
        {
            targetBlock = nBlockToTend;
            workingProfile = nWorkingProfile;
            farm = nfarm;
            character = ncharacter;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (!hasTendedBlock)
            {

                hasTendedBlock = true;
                if (farm.blockIsFullGrown(targetBlock))
                {
                    return new CharacterTask.HarvestFarmBlock(targetBlock, farm, workingProfile);
                }
                else
                {
                    return new CharacterTask.MakeFarmBlockGrow(targetBlock);
                }
            }
            else
            {
                
                return new CharacterTask.SwitchJob(new FarmingKickoffJob(farm,character,
                    workingProfile));
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
