using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace IslandGame
{
    static class ContentDistributor
    {
        public static Texture2D newGameButton, quitButton, consoleBackground, saveButton, loadButton, continueButton, menuBackground;
        public static Texture2D colorPallete, colorSwatchHighlightBox, colorSwatchBox;
        public static Texture2D excavationIcon, farmIcon, woodBlockIcon, boatIcon, 
            playerBuildIcon, storageIcon, swordIcon, axeIcon, characterIcon, hammerIcon, woodStorageIcon, brickStorageIcon;
        public static Texture2D crossReticle;
        public static Texture2D random;
        public static SpriteFont techFont, toolTipFont;
        static string cubeStudioRootPath = @"C:\Users\Public\CubeStudioCreations\";

        public static void loadContent(ContentManager content)
        {
            newGameButton = content.Load<Texture2D>("UI/NewGameButton");
            quitButton = content.Load<Texture2D>("UI/QuitButton");
            consoleBackground = content.Load<Texture2D>("UI/consoleBGPixel");
            saveButton = content.Load<Texture2D>("UI/SaveGameButton");
            loadButton = content.Load<Texture2D>("UI/LoadGameButton");
            continueButton = content.Load<Texture2D>("UI/continueButton");
            menuBackground = content.Load<Texture2D>("UI/MainMenuBackground");

            excavationIcon = content.Load<Texture2D>("UI/excavationIcon");
            farmIcon = content.Load<Texture2D>("UI/farmIcon");
            woodBlockIcon = content.Load<Texture2D>("UI/woodBlockIcon");
            playerBuildIcon = content.Load<Texture2D>("UI/playerBuildIcon");
            boatIcon = content.Load<Texture2D>("UI/boatIcon");
            storageIcon = content.Load<Texture2D>("UI/lockedChest");
            swordIcon = content.Load<Texture2D>("UI/swordIcon");
            axeIcon = content.Load<Texture2D>("UI/axeIcon");
            characterIcon = content.Load<Texture2D>("UI/characterIcon");
            hammerIcon = content.Load<Texture2D>("UI/hammerIcon");
            woodStorageIcon = content.Load<Texture2D>("UI/woodPileIcon");
            brickStorageIcon = content.Load<Texture2D>("UI/brickPileIcon");

            

            colorPallete = content.Load<Texture2D>("UI/colorsImage");
            colorSwatchHighlightBox = content.Load<Texture2D>("UI/colorSwatchHighlightBox");
            colorSwatchBox = content.Load<Texture2D>("UI/colorSwatchBox");

            random = content.Load<Texture2D>("random");

            crossReticle = content.Load<Texture2D>("UI/crossReticle");

            techFont = content.Load<SpriteFont>("SpriteFont1");
            toolTipFont = content.Load<SpriteFont>("toolTipFont");
        }

        public static string getEmptyString()
        {
            return "";
        }

        public static string getRealRootPath()
        {
            return cubeStudioRootPath;
        }

        internal static string addNecesaryPathing(string path)
        {
            if (path.Contains(getRealRootPath()))
            {
                return path;
            }
            else
            {
                return getRealRootPath() + path;
            }
        }
    }
}
