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
            standardBlock
        }

        ResourceType type;

        public static string getPathForResourceType(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.standardBlock:
                    return ContentDistributor.getEmptyString() + @"resources\standardBlock.chr";
                case ResourceType.Wheat:
                    return ContentDistributor.getEmptyString() + @"resources\wheatBale.chr";
                case ResourceType.Wood:
                    return ContentDistributor.getEmptyString() + @"resources\log.chr";
                default:
                    throw new Exception("unhandled resource type");
            }

        }

        public ResourceBlock(ResourceType nType)
        {
            type = nType;
        }

        public ResourceType getResourceType()
        {
            return type;
        }
    }
}
