using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    class LocationInterpolator
    {
        protected Vector3 currentLocation;
        protected Vector3 idealCameraLocation;

        public LocationInterpolator() { }

        public LocationInterpolator(Vector3 nLocation)
        {
            currentLocation = nLocation;
            idealCameraLocation = nLocation;
        }

        public virtual void setIdealCameraLocation(Vector3 nIdealLocation)
        {
            idealCameraLocation = nIdealLocation;
        }

        public virtual void setCurrentLocation(Vector3 nCurrentLocation)
        {
            currentLocation = nCurrentLocation;
        }

        public Vector3 getCameraLocation()
        {
            return currentLocation;
        }

        public void getMoveByVecWithMinSpeed(float minSpeed)
        {
            if(distToIdeal()<minSpeed)
            {
                currentLocation += idealCameraLocation - currentLocation;
            }
            else
            {
                currentLocation += (idealCameraLocation - currentLocation) / 4.0f
                    + Vector3.Normalize(idealCameraLocation - currentLocation) * minSpeed;
            }
        }

        private float distToIdeal()
        {
            return Vector3.Distance(idealCameraLocation, currentLocation);
        }


        public Vector3 getIdealLocation()
        {
            return idealCameraLocation;
        }
    }
}
