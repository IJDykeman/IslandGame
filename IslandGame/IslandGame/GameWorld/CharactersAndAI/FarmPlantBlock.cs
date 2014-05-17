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
        int wheatPerPlant = 1;

        public FarmPlantBlock()
        {
            growth = 0;
        }

        public int getTendedAndReturnWheatHarvested()
        {
            if (!isFullyGrown())
            {
                growth++;
                return 0;
            }
            else 
            {
                growth = 0;
                return wheatPerPlant;
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
