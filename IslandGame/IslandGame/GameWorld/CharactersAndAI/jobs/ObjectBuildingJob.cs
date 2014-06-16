using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    class ObjectBuildingJob : Job
    {
        protected ObjectBuildJobSite objectBuildSite;
        protected Character workerCharacter;
        protected bool hasFailedToPathToSite;
        TravelAlongPath currentWalkJob;

        public ObjectBuildingJob(Character nWorker, ObjectBuildJobSite nObjectBuildSite)
        {
            objectBuildSite = nObjectBuildSite;
            workerCharacter = nWorker;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            return new CharacterTask.ObjectBuildForFrame(objectBuildSite);

             if (currentWalkJob == null)
            {
                setWalkTaskToPathToObject(taskTracker);
            }

            if (currentWalkJob != null && currentWalkJob.isUseable() && currentWalkJob.isComplete() == false)
            {
                return currentWalkJob.getCurrentTask(taskTracker);
            }

            return new CharacterTask.ObjectBuildForFrame(objectBuildSite);

            
        }

        public override bool isUseable()
        {
            return true;
        }

        public override bool isComplete()
        {
            return hasFailedToPathToSite || objectBuildSite.isComplete();
        }

        void setWalkTaskToPathToObject(CharacterTaskTracker taskTracker)
        {
            BlockLoc found = new BlockLoc();
            List<BlockLoc> containsBlockLocTarget = new List<BlockLoc>();
            containsBlockLocTarget.Add(objectBuildSite.getObjectLoc());

            PathHandler pathHandler = new PathHandler();
            List<BlockLoc> path = pathHandler.getPathToMakeTheseBlocksAvaiable(objectBuildSite.getProfile(),
                new BlockLoc(workerCharacter.getFootLocation()), objectBuildSite.getProfile(),
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
