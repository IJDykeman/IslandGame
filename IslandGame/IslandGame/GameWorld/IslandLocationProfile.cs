using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class IslandLocationProfile
    {
        protected Island island;

        public IslandLocationProfile() { }

        public IslandLocationProfile(Island nIsland)
        {
            island = nIsland;
        }

        public Vector3 profileSpaceToWorldSpace(Vector3 loc)
        {
            return island.chunkSpaceToWorldSpace(loc);
        }

        public Vector3 worldSpaceToProfileSpace(Vector3 loc)
        {
            return island.worldSpaceToChunkSpaceSpace(loc);
        }

        public IntVector3 profileSpaceToWorldSpace(IntVector3 blockToFlag)
        {
            return new IntVector3(island.chunkSpaceToWorldSpace(blockToFlag.toVector3()));
        }
    }
}
