using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    public enum ResourceType
    {
        Wheat,
        Wood
    }

    public struct ResourceAmount
    {
        ResourceType type;
        int amount;

        public ResourceAmount(int nAmount, ResourceType nType)
        {
            type = nType;
            amount = nAmount;
        }

        public int getAmount()
        {
            return amount;
        }

        public ResourceType getType()
        {
            return type;
        }

    }
}
