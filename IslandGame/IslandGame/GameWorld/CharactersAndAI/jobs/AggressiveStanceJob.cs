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
                PathHandler pathHandler = new PathHandler();
                Path path = pathHandler.getPathToSingleBlock(pathingProfile,
                    new BlockLoc(character.getFootLocation()), pathingProfile,
                    BlockLoc.AddIntVec3(new BlockLoc(character.getFootLocation()), pathingProfile.getRandomMove()), 2);
                TravelAlongPath currentWalkJob = new TravelAlongPath(path, new AggressiveStanceJob(pathingProfile, actorProfile, character));
                if (currentWalkJob.isUseable())
                {
                    return new CharacterTask.SwitchJob(currentWalkJob);
                }
            }

                return new CharacterTask.NoTask();
            
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
