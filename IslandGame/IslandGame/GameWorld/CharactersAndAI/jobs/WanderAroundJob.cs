using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    class WanderAroundJob : Job
    {
        IslandPathingProfile pathingProfile;
        ActorStateProfile actorProfile;
        Character character;
        TravelAlongPath currentWalkJob;
        WaitJob currentWait = null;

        public WanderAroundJob(IslandPathingProfile nPathingProfile, ActorStateProfile nActorStateProfile, Character nCharacter)
        {
            pathingProfile = nPathingProfile;
            actorProfile = nActorStateProfile;
            character = nCharacter;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (currentWalkJob == null)
            {
                setWalkJobToRandomStep();
            }
            if (currentWait == null)
            {
                currentWait = new WaitJob(0);
                currentWait.update();
            }


            if (currentWalkJob.isUseable() && !currentWalkJob.isComplete() && (currentWait.isComplete()))
            {
                return currentWalkJob.getCurrentTask(taskTracker);
            }
            else
            {
                if (currentWait.isComplete())
                {
                    currentWait = new WaitJob(new Random().Next(30,120));
                    
                }
                currentWait.update();
                if(currentWait.isComplete())
                {
                    setWalkJobToRandomStep();

                }

                return currentWait.getCurrentTask(taskTracker);
            }


        }

        private void setWalkJobToRandomStep()
        {
            PathHandler pathHandler = new PathHandler();
            List<BlockLoc> path = pathHandler.getPathToSingleBlock(pathingProfile,
                new BlockLoc(character.getFootLocation()), pathingProfile,
                BlockLoc.AddIntVec3(new BlockLoc(character.getFootLocation()), pathingProfile.getRandomMove()), 2);
            currentWalkJob = new TravelAlongPath(path);
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
