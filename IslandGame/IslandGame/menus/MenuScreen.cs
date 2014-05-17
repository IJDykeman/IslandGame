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

namespace IslandGame.menus
{
    public class MenuScreen
    {

            public List<UIElement> buttons;
            bool willDisablePlayerInteraction = false;

            public MenuScreen(List<UIElement> nList)
            {
                buttons = nList;
            }

            public MenuScreen(List<UIElement> nList, bool nWillDisablePlayerInteraction)
            {
                willDisablePlayerInteraction = nWillDisablePlayerInteraction;
                buttons = nList;
            }


            public List<MenuAction> click(Vector2 click)
            {
                List<MenuAction> result = new List<MenuAction>();
                foreach (UIElement button in buttons)
                {
                    result.AddRange(button.click(click));
                }
                return result;
            }

            public bool clickLocHitsMenu(Vector2 click)
            {
                foreach (UIElement button in buttons)
                {
                    if (button.clickIsWithinElement(click))
                    {
                        return true;
                    }
                }
                return false;
            }

            public static MenuScreen getMainMenuInterface(int width, int height)
            {
                Vector2 menuLocation = new Vector2(width / 2, height / 2.5f);
                List<UIElement> buttonList = new List<UIElement>();
                buttonList.Add(new Image(ContentDistributor.menuBackground, new Vector2(0, 0), width, height));
                buttonList.Add(new UIElement(new ContinueMenuAction(), ContentDistributor.continueButton, new Vector2(menuLocation.X - ContentDistributor.newGameButton.Width / 2, menuLocation.Y - ContentDistributor.newGameButton.Height / 2)));
                buttonList.Add(new UIElement(new NewGameMenuAction(), ContentDistributor.newGameButton, new Vector2(menuLocation.X - ContentDistributor.newGameButton.Width / 2, menuLocation.Y - ContentDistributor.newGameButton.Height / 2 + 75 * 1)));
                buttonList.Add(new UIElement(new LoadMenuAction(), ContentDistributor.loadButton, new Vector2(menuLocation.X - ContentDistributor.newGameButton.Width / 2, menuLocation.Y - ContentDistributor.newGameButton.Height / 2 + 75*2)));
                buttonList.Add(new UIElement(new SaveMenuAction(), ContentDistributor.saveButton, new Vector2(menuLocation.X - ContentDistributor.newGameButton.Width / 2, menuLocation.Y - ContentDistributor.newGameButton.Height / 2 + 75*3)));
                buttonList.Add(new UIElement(new QuitMenuAction(), ContentDistributor.quitButton, new Vector2(menuLocation.X - ContentDistributor.newGameButton.Width / 2, menuLocation.Y - ContentDistributor.newGameButton.Height / 2 + 75*4)));

                MenuScreen newInterface = new MenuScreen(buttonList, true);
                return newInterface;
            }

            public static MenuScreen getGameplayHud(int width, int height)
            {
                List<UIElement> buttonList = new List<UIElement>();
                int horizontalPadding = 50;
                int verticlePadding = 50;
                float scale = 1f;
                Color hudTint = Color.Wheat;
                buttonList.Add(new UIElement(new ExcavationHudButtonClick(), ContentDistributor.excavationIcon, new Vector2(width - horizontalPadding, height - verticlePadding), scale, hudTint));
                buttonList.Add(new UIElement(new FarmHudButtonClick(), ContentDistributor.farmIcon, new Vector2(width - horizontalPadding * 2, height - verticlePadding), scale, hudTint));
                buttonList.Add(new UIElement(new WoodBuildHudClick(), ContentDistributor.woodBlockIcon, new Vector2(width - horizontalPadding * 3, height - verticlePadding), scale, hudTint));
                buttonList.Add(new UIElement(new PlayerBuildHudClick(), ContentDistributor.playerBuildIcon, new Vector2(width - horizontalPadding * 4, height - verticlePadding), scale, hudTint));
                buttonList.Add(new UIElement(new PlayerBuildBoatHudClick(), ContentDistributor.boatIcon, new Vector2(width - horizontalPadding * 5, height - verticlePadding), scale, hudTint));
                MenuScreen newInterface = new MenuScreen(buttonList);
                return newInterface;
            }

            public static MenuScreen getColorPalleteInterface(int height, int width)
            {
                List<UIElement> UIElementList = new List<UIElement>();
                UIElementList.Add(new ColorPallete(ContentDistributor.colorPallete,ContentDistributor.colorSwatchHighlightBox, new Vector2(200,200)));
                MenuScreen newInterface = new MenuScreen(UIElementList);
                return newInterface;
            }


            public void display(SpriteBatch spriteBatch)
            {
                foreach (UIElement button in buttons)
                {
                    spriteBatch.Draw(button.getTexture(), button.getRectangle(), button.getColor());
                }
            }

            public bool disablesPlayerInteraction()
            {
                return willDisablePlayerInteraction;
            }
        
    }
}
