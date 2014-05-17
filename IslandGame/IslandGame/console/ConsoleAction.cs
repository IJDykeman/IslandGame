using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    public enum ConsoleActions
    {
        regenIsland,
        setPlayerSpeed,
        placeIsland,
        mipIsland,
        addNewCharacter,
        addNewBoat,
        addNewEnemy
    }

    class ConsoleAction
    {
        public ConsoleActions type;
        public ConsoleAction()
        {
            
        }
    }

    class RegenIslandConsoleAction : ConsoleAction
    {
        public RegenIslandConsoleAction()
        {
            type = ConsoleActions.regenIsland;
        }
    }

    class SetPlayerSpeedConsoleAction : ConsoleAction
    {
        public float speed;
        public SetPlayerSpeedConsoleAction(float nSpeed)
        {
            type = ConsoleActions.setPlayerSpeed;
            speed = nSpeed;
        }
    }

    class PlaceIslandConsoleAction : ConsoleAction
    {

        public PlaceIslandConsoleAction()
        {
            type = ConsoleActions.placeIsland;

        }
    }


    class mipIslandConsoleAction : ConsoleAction
    {
        public int mipLevel;
        public mipIslandConsoleAction(int nMipLevel)
        {
            mipLevel = nMipLevel;
            type = ConsoleActions.mipIsland;

        }
    }

    class AddNewCharacterConsoleAction : ConsoleAction
    {

        public AddNewCharacterConsoleAction()
        {

            type = ConsoleActions.addNewCharacter;

        }
    }

    class AddNewEnemyConsoleAction : ConsoleAction
    {

        public AddNewEnemyConsoleAction()
        {

            type = ConsoleActions.addNewEnemy;

        }
    }

    class AddNewBoatConsoleAction : ConsoleAction
    {

        public AddNewBoatConsoleAction()
        {

            type = ConsoleActions.addNewBoat;

        }
    }

}
