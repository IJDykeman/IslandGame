using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    public class WorldPathingProfile : IslandPathingProfile
    {
        IslandManager islandManager;

        public WorldPathingProfile(IslandManager nIslandManager)
        {
            islandManager = nIslandManager;
        }

        protected override bool isInProfileScope(BlockLoc loc)
        {
            return true;
        }

        public override bool isProfileSolidAtWithWithinCheck(BlockLoc loc)
        {
            return islandManager.getClosestIslandToLocation(loc.toWorldSpaceVector3()).getPathingProfile().isProfileSolidAtWithWithinCheck(loc);
        }



    }
}
