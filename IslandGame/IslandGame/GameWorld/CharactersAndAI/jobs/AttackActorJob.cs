using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class AttackActorJob : Job
    {
        Actor target;

        Character character;
        float loseInterestDistance = 70f;

        public AttackActorJob(Actor nTarget, IslandPathingProfile nPathingProfile, ActorStateProfile nActorStateProfile, Character nCharacter)
        {
            target = nTarget;
            character = nCharacter;
            setJobType(JobType.combat);
        }


        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            float distToTarget = Vector3.Distance(character.getLocation(), target.getLocation());
            if (distToTarget > loseInterestDistance || target.isDead())
            {
                return new CharacterTask.SwitchJob(new AggressiveStanceJob(character));
            }
            else if (distToTarget > getDesiredDistanceFromTarget())
            {
                return new CharacterTask.WalkTowardPoint(target.getLocation());
            }
            else
            {
                return new CharacterTask.DoStrikeOfWorkAlongRay(character, character.getLocation(), character.getStrikeRange(), target.getLocation() - character.getLocation());
            }


            //return new CharacterTask.LookTowardPoint(target.getLocation());


        }

        float getDesiredDistanceFromTarget()
        {
            return character.getStrikeRange() * .8f;
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