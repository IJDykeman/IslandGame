using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class CharacterTaskTracker
    {
        HashSet<BlockLoc> currentlyClaimedForWork;

        public CharacterTaskTracker(IEnumerable<Actor> characters)
        {
            currentlyClaimedForWork = getBlocksClaimedForWork(characters);
        }

        public HashSet<BlockLoc> blocksCurrentlyClaimed()
        {
            return currentlyClaimedForWork;
        }

        HashSet<BlockLoc> getBlocksClaimedForWork(IEnumerable<Actor> characters)
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (Actor test in characters)
            {
                BlockLoc? claimed = test.blockClaimedToWorkOn();
                if (claimed.HasValue)
                {
                    result.Add((BlockLoc)claimed);
                }
            }
            return result;
        }
    }
}
