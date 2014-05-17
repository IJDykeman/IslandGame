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
        protected ChunkSpace chunkSpace;

        public IslandLocationProfile() { }

        public IslandLocationProfile(ChunkSpace nChunkSpace)
        {
            chunkSpace = nChunkSpace;
        }

        public Vector3 profileSpaceToWorldSpace(Vector3 loc)
        {
            return chunkSpace.chunkSpaceToWorldSpace(loc);
        }

        public Vector3 worldSpaceToProfileSpace(Vector3 loc)
        {
            return chunkSpace.worldSpaceToChunkSpaceSpace(loc);
        }

        public IntVector3 profileSpaceToWorldSpace(IntVector3 blockToFlag)
        {
            return new IntVector3(chunkSpace.chunkSpaceToWorldSpace(blockToFlag.toVector3()));
        }
    }
}
