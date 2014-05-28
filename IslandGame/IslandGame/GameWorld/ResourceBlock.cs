using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class ResourceBlock
    {

        public enum ResourceType
        {
            Wood,
            Wheat,
            Stone
        }

        ResourceType type;

        public ResourceBlock(ResourceType nType)
        {
            type = nType;
        }

        ResourceType getType()
        {
            return type;
        }
    }
}
