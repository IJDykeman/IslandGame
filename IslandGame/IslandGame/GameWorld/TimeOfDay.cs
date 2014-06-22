using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    class TimeOfDay
    {
        Vector4 skyHorizonColor;
        Vector4 skyZenithColor;
        float ambientBrightness;
        float lengthInSeconds = 100;
        bool spawnsMonsters = false;

        public TimeOfDay(Vector4 nHorizonColor, Vector4 nZenithColor, float nAmbientBrightness, float numMinutes, bool nSpawnsMonsters)
        {
            skyHorizonColor = nHorizonColor;
            skyZenithColor = nZenithColor;
            ambientBrightness = nAmbientBrightness;
            lengthInSeconds = numMinutes*60.0f;
            spawnsMonsters = nSpawnsMonsters;
        }

        public Vector4 getSkyHorizonColor()
        {
            return skyHorizonColor;
        }

        public Vector4 getSkyZenithColor()
        {
            return skyZenithColor;
        }

        public float getAmbientBrightness()
        {
            return ambientBrightness;
        }


        internal float getLengthInSeconds()
        {
            return lengthInSeconds;
        }

        public bool monstersSpawnAtThisTime()
        {
            return spawnsMonsters;
        }
    }
}
