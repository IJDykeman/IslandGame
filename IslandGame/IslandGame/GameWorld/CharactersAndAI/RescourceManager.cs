using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    class ResourceManager
    {
        Dictionary<ResourceType, int> rescourcesOnThisIsland;

        public ResourceManager()
        {
            rescourcesOnThisIsland = new Dictionary<ResourceType,int>();
            rescourcesOnThisIsland.Add(ResourceType.Wheat, 0);
            rescourcesOnThisIsland.Add(ResourceType.Wood, 0);
        }

        public void addRescources(ResourceAmount toAdd)
        {
            rescourcesOnThisIsland[toAdd.getType()] += toAdd.getAmount();
        }
    }
}
