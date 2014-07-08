using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class ActorStateProfile
    {
        private ActorManager actorManager;

        public ActorStateProfile(ActorManager nActorManager)
        {
            actorManager = nActorManager;
        }

        public List<Actor> getAllActorsWithFaction(Actor.Faction faction)
        {
            return actorManager.getAllActorsWithFaction(faction);
        }
    }
}
