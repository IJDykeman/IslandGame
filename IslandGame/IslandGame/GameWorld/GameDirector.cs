﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IslandGame.GameWorld;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    [Serializable]
    class GameDirector
    {
        DayNightCycler dayNightCycler;
        UnlockableManager unlockManager;

        public GameDirector()
        {
            dayNightCycler = new DayNightCycler();
        }

        public void update()
        {
            dayNightCycler.update();
        }

        public Vector4 getSkyHorizonColor()
        {
            return dayNightCycler.getSkyHorizonColor();
        }

        public Vector4 getSkyZenithColor()
        {
            return dayNightCycler.getSkyZenithColor();
        }


        public float getAmbientBrighness()
        {
            return dayNightCycler.getCurrentAmbientBrightness();
        }

        public bool monstersAreSpawning()
        {
            return dayNightCycler.monstersSpawning();
        }
    }
}
