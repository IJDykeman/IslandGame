using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    class FarmPlantBlock
    {
        private static readonly byte maxGrowthLevel = 10;
        byte growth;

        public FarmPlantBlock()
        {
            growth = 0;
        }

        public void growOneStage()
        {
            if(!isFullyGrown())
            {
                growth++;
            }
        }

        public byte getGrowthLevel()
        {
            return growth;
        }

        public bool isFullyGrown()
        {
            return growth >= maxGrowthLevel;
        }
    }
}
