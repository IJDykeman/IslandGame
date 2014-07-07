using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    class LoggingKickoffJob : MultiBlockOngoingJob
    {
        Character character;
        bool failedToFindATreeToChop = false;
        IslandWorkingProfile workingProfile;



        public LoggingKickoffJob(Character nCharacter, IslandWorkingProfile nWorkingProfile)
        {

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
            List<BlockLoc> nextBlocksToBuild = workingProfile.getTreeJobSite().getTreeTrunkBlocks();

            foreach (BlockLoc claimed in taskTracker.blocksCurrentlyClaimed())
            {
                nextBlocksToBuild.Remove(claimed);

            }
            BlockLoc blockFoundToChop;

            PathHandler pathHadler = new PathHandler();

            Path path = pathHadler.getPathToMakeTheseBlocksAvaiable(
                workingProfile.getTreeJobSite().getProfile(),
                new BlockLoc(character.getFootLocation()),
                workingProfile.getTreeJobSite().getProfile(),
                nextBlocksToBuild,
                2, out blockFoundToChop);

            TravelAlongPath walkJob = new TravelAlongPath(path, getWaitJobWithReturn(45, new ChopTreeJob(character, workingProfile, blockFoundToChop)));

            if (!walkJob.isUseable())
            {
                failedToFindATreeToChop = true;
                return new CharacterTask.SwitchJob(new UnemployedJob());
            }

            return new CharacterTask.SwitchJob(walkJob);
        }

       

    }
}
