using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class ActorManager
    {
        List<Actor> actors;

        public ActorManager()
        {
            actors = new List<Actor>();
            //addCharacterAt(new Vector3(20, 20, 20));
        }

        public void display(GraphicsDevice device, Effect effect, Character playerIsInhabiting)
        {

            foreach (Actor test in actors)
            {
                if (test != playerIsInhabiting)
                {
                    test.draw(device, effect);
                }
                else
                {
                    test.drawWithPartTypeExcluded(device, effect, CubeAnimator.BodyPartType.head);
                }
            }
        }

        public List<ActorAction> update()
        {
            List<ActorAction> result = new List<ActorAction>();

            CharacterTaskTracker taskTracker = new CharacterTaskTracker(actors);

            foreach (Actor test in actors)
            {
                result.AddRange(test.update(taskTracker));
            }
            return result;
        }

        public void addCharacterAt(Vector3 location, Actor.Faction faction)
        {

            actors.Add(new Character(new AxisAlignedBoundingBox(location + new Vector3(), .6f, .6f, 1.8f), faction));
        }

        public void addCharacterAt(Vector3 location, Actor.Faction faction, IslandPathingProfile islandPathingProfile, ActorStateProfile actorStateProfile)
        {
            Character character = new Character(new AxisAlignedBoundingBox(location + new Vector3(), .6f, .6f, 1.8f), faction);
            if (faction == Actor.Faction.enemy)
            {
                character.setJobAndCheckUseability(new AggressiveStanceJob(islandPathingProfile, actorStateProfile, character));
            }
            actors.Add(character);
        }

        public T getNearestActorOfTypeAlongRay<T>(Ray ray) where T : Actor
        {
            return getNearestActorOfTypeAlongRay<T>(ray, null);
        }

        public T getNearestActorOfTypeAlongRay<T>(Ray ray, Actor toIgnore) where T:Actor
        {
            Actor result = null;
            float minDist = float.MaxValue;

            foreach (Actor test in actors)
            {
                if (test != toIgnore)
                {
                    float? distToStrike = ray.Intersects(test.getBoundingBox());
                    if (distToStrike.HasValue)
                    {
                        if ((float)distToStrike < minDist)
                        {
                            if (test is T)
                            {
                                result = test;
                            }
                        }
                    }
                }
            }
            if (result is T)
            {
                return (T)result;
            }
            else
            {
                return null;
            }
        }

        public void deleteActor(Actor toDelete)
        {
            actors.Remove(toDelete);
        }

        public void addBoatAt(Microsoft.Xna.Framework.Vector3 nLoc)
        {
            actors.Add(new Boat(nLoc));
        }

        public List<Actor> getAllActorsWithFaction(Actor.Faction faction)
        {
            List<Actor> result = new List<Actor>();
            foreach (Actor test in actors)
            {
                if (test.getFaction() == faction)
                {
                    result.Add(test);
                }
            }
            return result;
        }

        public ActorStateProfile getActorProfile()
        {
            return new ActorStateProfile(this);
        }

        public void handleStrike(Actor striker, Vector3 origen, Vector3 direction, float range, float damage)
        {
            Ray strikeRay = new Ray(origen, direction);
            Actor struck = getNearestActorOfTypeAlongRay<Actor>(strikeRay, striker);
            if (struck != null)
            {
                if (Actor.areHostile(striker.getFaction(), struck.getFaction()))
                {
                    float distToStrike = (float)strikeRay.Intersects(struck.getBoundingBox());
                    if (distToStrike <= range)
                    {
                        struck.damage(damage);
                    }
                }
            }
        }

        public void deleteAllEnemiesOfFactionInAABB(BoundingBox playerIslandBox, Actor.Faction faction)
        {
            for (int i = actors.Count - 1; i >= 0; i--)
            {
                if (actors[i].getFaction() == faction)
                {
                    if (actors[i].getBoundingBox().Intersects(playerIslandBox))
                    {
                        actors.RemoveAt(i);
                    }
                }
            }
        }
    }



}
