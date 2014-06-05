using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    class LoggingJob : MultiBlockOngoingJob
    {
        BlockLoc currentBlockToChop;
        TravelAlongPath currentWalkJob;
        WaitJob currentWait = null;
        Character character;
        TreesJobSite treeJobSite;
        bool failedToFindATreeToChop = false;
        BlockLoc currentGoalBlock;

        public LoggingJob(Character nCharacter, TreesJobSite nSite)
        {
            treeJobSite = nSite;
            character = nCharacter;
            setJobType(JobType.logging);
        }

        public override BlockLoc? getCurrentGoalBlock()
        {
            return currentBlockToChop;
        }

        public override bool isComplete()
        {
            return !treeJobSite.hasAtLeastOneTree() || failedToFindATreeToChop;
        }

        public override bool isUseable()
        {
            return true;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (treeJobSite.getTreeTrunkBlocks().Count > 0)
            {

                updateCurrentWalkAndChop(taskTracker);
                if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.isComplete())
                {
                    if (treeJobSite.getTreeTrunkBlocks().Contains(currentGoalBlock))
                    {
                        return new CharacterTask.ChopBlockForFrame(currentGoalBlock);
                    }
                    else
                    {
                        return new CharacterTask.SwitchJobType(JobType.CarryingWood);
                    }
                }
                if (currentWalkJob.isUseable() && (currentWait == null || currentWait.isComplete()))
                {

                    return currentWalkJob.getCurrentTask(taskTracker);
                }
                else
                {
                    return new CharacterTask.NoTask();
                }



            }
            else
            {
                return new CharacterTask.SwitchJob(new UnemployedJob());
            }
        }

        private void updateCurrentWalkAndChop(CharacterTaskTracker taskTracker)
        {
            List<BlockLoc> blocksToRemove = treeJobSite.getTreeTrunkBlocks();
            if (blocksToRemove.Count > 0)
            {
                if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.isComplete() == false)
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

                            setWalkTaskToNextBlockInBuildSite(taskTracker);

                        }
                    }
                    else
                    {
                        if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.isComplete() == true)
                        {

                            currentWait = new WaitJob(30);
                            return;

                        }

                        setWalkTaskToNextBlockInBuildSite(taskTracker);
                        if (!currentWalkJob.isUseable())
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

        private void setWalkTaskToNextBlockInBuildSite(CharacterTaskTracker taskTracker)
        {
            List<BlockLoc> nextBlocksToBuild = treeJobSite.getTreeTrunkBlocks();

            foreach (BlockLoc claimed in taskTracker.blocksCurrentlyClaimed())
            {
                nextBlocksToBuild.Remove(claimed);

            }
            BlockLoc blockFoundToBuild;

            PathHandler pathHadler = new PathHandler();

            List<BlockLoc> path = pathHadler.getPathToMakeTheseBlocksAvaiable(
                treeJobSite.getProfile(),
                new BlockLoc(character.getFootLocation()),
                treeJobSite.getProfile(),
                nextBlocksToBuild,
                2, out blockFoundToBuild);
            currentGoalBlock = blockFoundToBuild;
            currentWalkJob = new TravelAlongPath(path);

            if (!currentWalkJob.isUseable())
            {
                failedToFindATreeToChop = true;
            }
        }

    }
}
