using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame
{
    [Serializable]
    public class UnlockableManager
    {
        static int numColorsUnlocked=0;

        public void unlockNewColor()
        {
            numColorsUnlocked++;
        }

        public static int getNumColorsUnlocked()
        {
            return numColorsUnlocked;
        }
    }
}
