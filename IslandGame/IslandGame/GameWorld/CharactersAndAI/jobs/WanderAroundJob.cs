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


        public WanderAroundJob(IslandPathingProfile nPathingProfile, ActorStateProfile nActorStateProfile, Character nCharacter)
        {
            pathingProfile = nPathingProfile;
            actorProfile = nActorStateProfile;
            character = nCharacter;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (currentWalkJob == null || !currentWalkJob.isUseable() || !currentWalkJob.isComplete())
            {
                setWalkJobToRandomStep();
            }


            return currentWalkJob.getCurrentTask(taskTracker);
            


        }

        private void setWalkJobToRandomStep()
        {
            PathHandler pathHandler = new PathHandler();
            Path path = pathHandler.getPathToSingleBlock(pathingProfile,
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
