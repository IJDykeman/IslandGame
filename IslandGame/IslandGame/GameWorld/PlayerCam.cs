using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    class PlayerCam : LocationInterpolator
    {
        float minHeight = 1;

        public PlayerCam(Vector3 nLocation)
        {
            currentLocation = nLocation;
            idealCameraLocation = nLocation;
        }


        public override void setIdealCameraLocation(Vector3 nIdealLocation)
        {
            idealCameraLocation = constrainByHeight(nIdealLocation);
        }

        public override void setCurrentLocation(Vector3 nCurrentLocation)
        {
            currentLocation = constrainByHeight(nCurrentLocation);
        }

        Vector3 constrainByHeight(Vector3 toConstrain)
        {
            if (toConstrain.Y < minHeight)
            {
                toConstrain.Y = minHeight;
            }
            return toConstrain;
        }
        
    }
}
