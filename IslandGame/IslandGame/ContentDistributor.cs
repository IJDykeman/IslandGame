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
        public static Texture2D colorPallete, colorSwatchHighlightBox;
        public static Texture2D excavationIcon, farmIcon, woodBlockIcon, boatIcon, playerBuildIcon;
        public static Texture2D crossReticle;
        public static Texture2D random;
        public static SpriteFont techFont;
        static string cubeStudioRootPath = @"C:\Users\Public\CubeStudio\";

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
            

            colorPallete = content.Load<Texture2D>("UI/colorsImage");
            colorSwatchHighlightBox = content.Load<Texture2D>("UI/colorSwatchHighlightBox");

            random = content.Load<Texture2D>("random");

            crossReticle = content.Load<Texture2D>("UI/crossReticle");

            techFont = content.Load<SpriteFont>("SpriteFont1");
        }

        public static string getRootPath()
        {
            return cubeStudioRootPath;
        }
    }
}
