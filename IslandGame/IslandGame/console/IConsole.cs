using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    static class IConsole
    {
        public static bool active;
        static string currentText = "";
        static KeyboardState oldKeyboardState;
        static List<string> history = new List<string>();

        public static bool boolFlag1;
        public static float floatFlag1;
        public static Vector3 locationFlag1;
        public static Vector3 locationFlag2;

        static Keys consoleOpenkey = Keys.Tab;

        static Keys[] letters = {Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, 
            Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
            Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Space, Keys.OemPeriod};
        static char[] chars = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
             '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ', '.'};

        public static void loadContent()
        {
            oldKeyboardState = Keyboard.GetState();
        }

        public static List<ConsoleAction> update(Vector3 playerLocation)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();
            if (!active)
            {
                if (PlayerInputHandler.justHit(consoleOpenkey, newKeyboardState, oldKeyboardState))
                {
                    active = true;
                }

                if (!currentText.Equals(""))
                {
                    currentText = "";
                }
                oldKeyboardState = newKeyboardState;
                return new List<ConsoleAction>();

            }
            else if (PlayerInputHandler.justHit(consoleOpenkey, newKeyboardState, oldKeyboardState))
            {
                active = false;
                oldKeyboardState = newKeyboardState;
                return new List<ConsoleAction>();
            }


            for (int i = 0; i < letters.Length; i++)
            {
                if (PlayerInputHandler.justHit(letters[i], newKeyboardState, oldKeyboardState))
                {
                    currentText += chars[i];
                }
            }

            if (PlayerInputHandler.justHit(Keys.Back, newKeyboardState, oldKeyboardState))
            {
                if(currentText.Length>0)
                currentText = currentText.Substring(0, currentText.Length - 1);
            }

            if (PlayerInputHandler.justHit(Keys.Enter, newKeyboardState, oldKeyboardState))
            {
                oldKeyboardState = newKeyboardState;
                history.Add(currentText);

                List<ConsoleAction> result = getActionsFromCurrentString(playerLocation);
                currentText = "";
                closeConsole();
                return result;
            }

            oldKeyboardState = newKeyboardState;

            return new List<ConsoleAction>();
        }

        static List<ConsoleAction> getActionsFromCurrentString(Vector3 playerLocation)
        {
            string[] commandArray = currentText.Split(' ');
            string command = commandArray[0];

            List<ConsoleAction> result = new List<ConsoleAction>();
            if (command.Equals("REG"))
            {
                result.Add(new RegenIslandConsoleAction());
            }
            else if (command.Equals("PLACEISLAND"))
            {
                result.Add(new PlaceIslandConsoleAction());
            }
            else if (command.Equals("PLACECHAR") || command.Equals("PC"))
            {
                result.Add(new AddNewCharacterConsoleAction() );
            }
            else if (command.Equals("PLACEENEMY") || command.Equals("PE"))
            {
                result.Add(new AddNewEnemyConsoleAction());
            }
            else if (command.Equals("PLACEBOAT"))
            {
                result.Add(new AddNewBoatConsoleAction());
            }
            else if (command.Equals("SETSPEED"))
            {
                try
                {
                    float speed = (float)Convert.ToDouble(commandArray[1]);
                    result.Add(new SetPlayerSpeedConsoleAction(speed));
                }
                catch { }
            }
            else if (command.Equals("SETFLOATFLAG1"))
            {
                try
                {
                    floatFlag1 = (float)Convert.ToDouble(commandArray[1]);
                }
                catch { }
            }
            else if (command.Equals("MIP"))
            {
                try
                {
                    int level = (int)Convert.ToDouble(commandArray[1]);
                    result.Add(new mipIslandConsoleAction(level));
                }
                catch { }
            }

            else if (command.Equals("FLAG1"))
            {
                try
                {
                    string argument = commandArray[1];
                    boolFlag1 = argument.Equals("TRUE");
                    
                }
                catch { }
            }
            else if (command.Equals("SETLOCATION1"))
            {
                locationFlag1 = playerLocation;
            }
            else if (command.Equals("SETLOCATION2"))
            {
                locationFlag2 = playerLocation;
            }



            return result;
        }



        public static void display(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            if (active)
            {
                spriteBatch.Draw(ContentDistributor.consoleBackground, new Rectangle(20, screenHeight - 60, screenWidth - 40, 30), Color.White);
                spriteBatch.DrawString(ContentDistributor.techFont, currentText, new Vector2(26, screenHeight - 53), Color.White);
            }
        }

        static void closeConsole()
        {
            active = false;
        }

        public static bool isOpen()
        {
            return active;
        }
    }
}
