using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{


    public struct ResourceAmount
    {
        ResourceBlock.ResourceType type;
        int amount;

        public ResourceAmount(int nAmount, ResourceBlock.ResourceType nType)
        {
            type = nType;
            amount = nAmount;
        }

        public int getAmount()
        {
            return amount;
        }

        public ResourceBlock.ResourceType getType()
        {
            return type;
        }

    }
}
