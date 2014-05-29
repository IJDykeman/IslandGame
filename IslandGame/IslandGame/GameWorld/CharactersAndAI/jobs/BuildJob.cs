using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    class BuildJob : Job
    {
        WoodBuildSite buildSite;
        TravelAlongPath currentWalkJob;
        WaitJob currentWait = null;
        Character character;
        BlockLoc currentGoalBlock;
        bool hasFailedToFindOneBlockToBuild = false;


        public BuildJob( WoodBuildSite nBuildSite, Character nCharacter)
        {
            buildSite = nBuildSite;
            character = nCharacter;
            setJobType(JobType.building);
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (buildSite.numBlocksLeftToBuild() > 0)
            {

                updateCurrentTask(taskTracker);
                if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.isComplete())
                {
                    if (buildSite.containsBlockToBuild(currentGoalBlock))
                    {
                        return new CharacterTask.BuildBlock(currentGoalBlock, (byte)5);
                    }
                }
                if (currentWalkJob.isUseable() && (currentWait==null || currentWait.isComplete()))
                {

                     return currentWalkJob.getCurrentTask(taskTracker);
                }
                else
                {
                    if (currentGoalBlock != null)
                    {
                        return new CharacterTask.LookTowardPoint(currentGoalBlock.toWorldSpaceVector3() 
                            + new Microsoft.Xna.Framework.Vector3(1, 1, 1) / 2f);

                    }
                    return new CharacterTask.NoTask();
                }



            }
            else
            {
                return new CharacterTask.NoTask();
            }
        }

        private void updateCurrentTask(CharacterTaskTracker taskTracker)
        {

            if (buildSite.numBlocksLeftToBuild() > 0)
                {
                    if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.isComplete() == false )
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
                                hasFailedToFindOneBlockToBuild = true;
                            }
                            
                        }
                        
                    


                    }
                }


            return;

        }

        private void setWalkTaskToNextBlockInBuildSite(CharacterTaskTracker taskTracker)
        {
            List<BlockLoc> nextBlocksToBuild = buildSite.getNextBlocksToBuild().ToList();

            foreach (BlockLoc claimed in taskTracker.blocksCurrentlyClaimed())
            {
                nextBlocksToBuild.Remove(claimed);
               
            }
            BlockLoc blockFoundToBuild;

            PathHandlerPreferringLowerBlocks pathHadler = new PathHandlerPreferringLowerBlocks();

            List<BlockLoc> path = pathHadler.getPathToMakeTheseBlocksAvaiable(
                buildSite.getProfile(),
                new BlockLoc(character.getFootLocation()), 
                buildSite.getProfile(),
                nextBlocksToBuild, 
                2, out blockFoundToBuild);
            currentGoalBlock = blockFoundToBuild;
            currentWalkJob = new TravelAlongPath(path);
        }

        public override bool isComplete()
        {
            return buildSite.siteIsComplete() || hasFailedToFindOneBlockToBuild;
        }

        public override bool isUseable()
        {
            return currentWalkJob == null || currentWalkJob.isUseable();
        }

        public BlockLoc? getCurrentGoalBlock()
        {
            if (currentWalkJob != null && currentWalkJob.isUseable())
            {
                return currentWalkJob.getGoalBlock();
            }
            
            return null;
        }

    }
}
