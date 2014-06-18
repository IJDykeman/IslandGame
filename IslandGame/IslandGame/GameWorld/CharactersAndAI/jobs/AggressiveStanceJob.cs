using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    class AggressiveStanceJob : Job
    {
        IslandPathingProfile pathingProfile;
        ActorStateProfile actorProfile;
        Character character;
        TravelAlongPath currentWalkJob;
        WaitJob currentWait = null;
        float agroRange = 20;

        public AggressiveStanceJob(IslandPathingProfile nPathingProfile, ActorStateProfile nActorStateProfile, Character nCharacter)
        {
            pathingProfile = nPathingProfile;
            actorProfile = nActorStateProfile;
            character = nCharacter;
            setJobType(JobType.combat);
        }



        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            Actor toAttack = getNearestEnemyInAgroRange();
            if (toAttack != null)
            {
                return new CharacterTask.SwitchJob(new AttackActorJob(toAttack,pathingProfile,actorProfile,character));
            }
            if (character.getFaction() != Actor.Faction.friendly)
            {
                return updateWandering(taskTracker);
            }
            else
            {
                return new CharacterTask.NoTask();
            }
        }

        private Actor getNearestEnemyInAgroRange()
        {
            Actor nearestInRange = null;
            float nearestDist = float.MaxValue;
            List<Actor> playerAgents = actorProfile.getAllActorsWithFaction(Actor.getFactionHostileTo(character.getFaction()));

            foreach (Actor test in playerAgents)
            {
                float distToTest = Vector3.Distance(test.getFootLocation(), character.getFootLocation());
                if (distToTest < nearestDist && distToTest < agroRange)
                {
                    nearestInRange = test;
                    nearestDist = distToTest;
                }
            }
            return nearestInRange;
        }

        private CharacterTask.Task updateWandering(CharacterTaskTracker taskTracker)
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
                    currentWait = new WaitJob(new Random().Next(30, 120));

                }
                currentWait.update();
                if (currentWait.isComplete())
                {
                    setWalkJobToRandomStep();

                }

                return currentWait.getCurrentTask(taskTracker);
            }
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
