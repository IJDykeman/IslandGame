using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    class WalkToAndCompleteTaskJob : Job
    {
        
        private CharacterTask.Task task;
        private Character workerCharacter;
        private bool hasFailedToPathToSite;
        TravelAlongPath currentWalkJob;
        private bool hasReturnedTask = false;
        private BlockLoc taskLocation;
        private IslandPathingProfile profile;

        public WalkToAndCompleteTaskJob(Character nWorker, CharacterTask.Task nTask, BlockLoc nTaskLocation, IslandPathingProfile nProfile)
        {
            profile = nProfile;
            taskLocation = nTaskLocation;
            task = nTask;
            workerCharacter = nWorker;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
             if (currentWalkJob == null)
            {
                setWalkTaskToPathToObject(taskTracker);
            }

            if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.isComplete() == false)
            {
                return currentWalkJob.getCurrentTask(taskTracker);
            }

            hasReturnedTask = true;
            return task;

            
        }

        public override bool isUseable()
        {
            return true;
        }

        public override bool isComplete()
        {
            return hasFailedToPathToSite || hasReturnedTask;
        }

        void setWalkTaskToPathToObject(CharacterTaskTracker taskTracker)
        {
            BlockLoc found = new BlockLoc();
            List<BlockLoc> containsBlockLocTarget = new List<BlockLoc>();
            containsBlockLocTarget.Add(taskLocation);

            PathHandler pathHandler = new PathHandler();
            List<BlockLoc> path = pathHandler.getPathToMakeTheseBlocksAvaiable(profile,
                new BlockLoc(workerCharacter.getFootLocation()), profile,
                containsBlockLocTarget, 2,
                out found);
            currentWalkJob = new TravelAlongPath(path);

            if (!currentWalkJob.isUseable())
            {
                hasFailedToPathToSite = true;
            }
        }
    }
}
