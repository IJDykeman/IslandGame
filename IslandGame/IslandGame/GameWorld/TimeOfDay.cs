using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    class TimeOfDay
    {
        Vector4 skyHorizonColor;
        Vector4 skyZenithColor;
        float ambientBrightness;

        public TimeOfDay(Vector4 nHorizonColor, Vector4 nZenithColor, float nAmbientBrightness)
        {
            skyHorizonColor = nHorizonColor;
            skyZenithColor = nZenithColor;
            ambientBrightness = nAmbientBrightness;
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

    }
}
