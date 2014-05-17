using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    struct PositionScaleOpacity
    {
        public Vector3 loc;
        public float scale;
        public float opacity;

        public PositionScaleOpacity(Vector3 nLoc, float nScale)
        {
            loc = nLoc;
            scale = nScale;
            opacity = 1;
        }

        public PositionScaleOpacity(Vector3 nLoc, float nScale, float nOpacity)
        {
            loc = nLoc;
            scale = nScale;
            opacity = nOpacity;
        }
    }
}
