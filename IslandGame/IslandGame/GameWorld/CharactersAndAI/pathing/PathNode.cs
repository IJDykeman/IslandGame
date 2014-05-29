using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    class PathNode
    {
        public PathNode previous;
        public BlockLoc loc;
        public int costToGetHere;
        public int approximateDistanceToTarget;

        public PathNode() { }

        public PathNode(PathNode nPrevious, BlockLoc nLoc, int nCostToGetHere, BlockLoc endLoc)
        {
            previous = nPrevious;
            loc = nLoc;
            costToGetHere = nCostToGetHere;
            approximateDistanceToTarget = approximateDistanceTo(endLoc);
        }

        protected virtual int approximateDistanceTo(BlockLoc endLoc)
        {
            return Math.Abs(loc.WSX() - endLoc.WSX()) + Math.Abs(loc.WSY() - endLoc.WSY()) + Math.Abs(loc.WSZ() - endLoc.WSZ());
        }

    }
}
