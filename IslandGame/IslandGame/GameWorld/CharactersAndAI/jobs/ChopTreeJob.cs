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

        /*private void updateCurrentWalkAndChop(CharacterTaskTracker taskTracker)
        {

            List<BlockLoc> blocksToRemove = treeJobSite.getTreeTrunkBlocks();
            if (blocksToRemove.Count > 0)
            {
                if (walkJob != null && walkJob.isUseable() && walkJob.isComplete() == false)
                {
                    return;
                }
                else
                {
                    if (currentWait != null && !currentWait.isComplete())//if the wait needs to be updates
                    {
                        currentWait.update();
                        if (currentWait.isComplete())
                        {

                            setWalkTaskToNextTree(taskTracker);

                        }
                    }
                    else
                    {
                        if (walkJob != null && walkJob.isUseable() && walkJob.isComplete() == true)
                        {

                            currentWait = new WaitJob(30);
                            return;

                        }

                        setWalkTaskToNextTree(taskTracker);
                        if (!walkJob.isUseable())
                        {
                            failedToFindATreeToChop = true;

                        }
                        



                    }




                }
            }
            else
            {

            }
        }

        private void setWalkTaskToNextTree(CharacterTaskTracker taskTracker)
        {
            List<BlockLoc> nextBlocksToBuild = treeJobSite.getTreeTrunkBlocks();

            foreach (BlockLoc claimed in taskTracker.blocksCurrentlyClaimed())
            {
                nextBlocksToBuild.Remove(claimed);

            }
            BlockLoc blockFoundToChop;

            PathHandler pathHadler = new PathHandler();

            List<BlockLoc> path = pathHadler.getPathToMakeTheseBlocksAvaiable(
                treeJobSite.getProfile(),
                new BlockLoc(character.getFootLocation()),
                treeJobSite.getProfile(),
                nextBlocksToBuild,
                2, out blockFoundToChop);
            currentGoalBlock = blockFoundToChop;
            walkJob = new TravelAlongPath(path);

            if (!walkJob.isUseable())
            {
                failedToFindATreeToChop = true;
            }
        }*/

    }
}
