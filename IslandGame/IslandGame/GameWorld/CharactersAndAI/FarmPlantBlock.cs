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

        public void getTendedAndReturnWheatHarvested()
        {
            if (!isFullyGrown())
            {
                growth++;

            }
            else 
            {
                growth = 0;

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
