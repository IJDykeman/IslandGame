using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    class WalkToAndDestroyBlock : Job
    {
        TravelAlongPath walkJob;
        CharacterTask.DestroyBlock destroyTask;

        public WalkToAndDestroyBlock(TravelAlongPath nWalkJob, CharacterTask.DestroyBlock nDestoryTask)
        {
            walkJob = nWalkJob;
            destroyTask = nDestoryTask;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (walkJob.isComplete() == false)
            {

                return walkJob.getCurrentTask(taskTracker);
            }
            else if (destroyTask.isComplete() == false)
            {
                
                return destroyTask;
            }
            else
            {
                return new CharacterTask.NoTask();

            }
        }

        public override bool isComplete()
        {
            if (walkJob.isComplete() == false)
            {
                return false;
            }
            else if (destroyTask.isComplete() == false)
            {
                return false;
            }
            else
            {
                return true;

            }
        }

        public override bool isUseable()
        {
            return walkJob.foundPath();
        }

        public TravelAlongPath getWalkJob()
        {
            return walkJob;
        }

        public CharacterTask.DestroyBlock getDestroyTask()
        {
            return destroyTask;
        }

        internal BlockLoc getGoalBlock()
        {

            return destroyTask.getBlockToDestroy();

        }
    }
}
