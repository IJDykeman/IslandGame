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

            protected List<UIElement> UIElements;
            bool willDisablePlayerInteraction = false;

            public MenuScreen(List<UIElement> nList)
            {
                UIElements = nList;
            }

            public MenuScreen(List<UIElement> nList, bool nWillDisablePlayerInteraction)
            {
                willDisablePlayerInteraction = nWillDisablePlayerInteraction;
                UIElements = nList;
            }
            

            public List<MenuAction> click(Vector2 click)
            {
                List<MenuAction> result = new List<MenuAction>();
                foreach (UIElement button in UIElements)
                {
                    result.AddRange(button.click(click));
                }
                return result;
            }

            public bool clickLocHitsMenu(Vector2 click)
            {
                foreach (UIElement button in UIElements)
                {
                    if (button.locIsWithinElement(click))
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

            public static MenuScreen getThirdPersonHud(int width, int height)
            {
                List<UIElement> buttonList = new List<UIElement>();
                int horizontalPadding = 50;
                int verticlePadding = 50;
                float scale = 1f;
                Color hudTint = Color.Wheat;
                buttonList.Add(new UIElement(new ExcavationHudButtonClick(), ContentDistributor.excavationIcon, new Vector2(width - horizontalPadding, height - verticlePadding),
                    scale, hudTint, "designate blocks for excavation"));
                buttonList.Add(new UIElement(new FarmHudButtonClick(), ContentDistributor.farmIcon, new Vector2(width - horizontalPadding * 2, height - verticlePadding),
                    scale, hudTint, "place a farm"));
                buttonList.Add(new UIElement(new WoodBuildHudClick(), ContentDistributor.woodBlockIcon, new Vector2(width - horizontalPadding * 3, height - verticlePadding),
                    scale, hudTint, "design structures to be built"));
                buttonList.Add(new UIElement(new PlayerBuildHudClick(), ContentDistributor.playerBuildIcon, new Vector2(width - horizontalPadding * 4, height - verticlePadding),
                    scale, hudTint, "build structures by hand"));
                buttonList.Add(new UIElement(new PlayerBuildBoatHudClick(), ContentDistributor.boatIcon, new Vector2(width - horizontalPadding * 5, height - verticlePadding),
                    scale, hudTint, "designate a boat build site"));
                buttonList.Add(new UIElement(new PlayerPlaceWoodStorageHudClick(), ContentDistributor.storageIcon, new Vector2(width - horizontalPadding * 6, height - verticlePadding),
                    scale, hudTint, "place wood stockpile"));
                buttonList.Add(new UIElement(new PlayerPlaceWheatStorageHudClick(), ContentDistributor.storageIcon, new Vector2(width - horizontalPadding * 7, height - verticlePadding),
                    scale, hudTint, "place wheat stockpile"));
                buttonList.Add(new UIElement(new PlayerPlaceNewCharacterHudClick(), ContentDistributor.characterIcon, new Vector2(width - horizontalPadding * 8, height - verticlePadding),
                    scale, hudTint, "create new character (-12 wheat)"));
                MenuScreen newInterface = new MenuScreen(buttonList);
                return newInterface;
            }

            public static MenuScreen getFirstPersonHud(int width, int height)
            {
                List<UIElement> buttonList = new List<UIElement>();
                int horizontalPadding = 50;
                int verticlePadding = 50;
                float scale = 1f;
                Color hudTint = Color.Wheat;
                buttonList.Add(new UIElement(new JobTypeSwitch(IslandGame.GameWorld.JobType.mining), ContentDistributor.excavationIcon, new Vector2(horizontalPadding, height - verticlePadding),
                    scale, hudTint, "pickaxe"));
                buttonList.Add(new UIElement(new JobTypeSwitch(IslandGame.GameWorld.JobType.agriculture), ContentDistributor.farmIcon, new Vector2(horizontalPadding * 2, height - verticlePadding),
                    scale, hudTint, "hoe"));
                buttonList.Add(new UIElement(new JobTypeSwitch(IslandGame.GameWorld.JobType.building), ContentDistributor.woodBlockIcon, new Vector2(horizontalPadding * 3, height - verticlePadding),
                    scale, hudTint, "hammer"));
                buttonList.Add(new UIElement(new JobTypeSwitch(IslandGame.GameWorld.JobType.combat), ContentDistributor.swordIcon, new Vector2(horizontalPadding * 4, height - verticlePadding),
                    scale, hudTint, "sword"));
                buttonList.Add(new UIElement(new JobTypeSwitch(IslandGame.GameWorld.JobType.logging), ContentDistributor.axeIcon, new Vector2(horizontalPadding * 5, height - verticlePadding),
                    scale, hudTint, "axe"));
                UIRadioGroup group = new UIRadioGroup(buttonList);
                List<UIElement> interfaceList = new List<UIElement>();
                interfaceList.Add(group);

                MenuScreen newInterface = new MenuScreen(interfaceList);
                return newInterface;
            }

            public static MenuScreen getColorPalleteInterface(int height, int width)
            {
                List<UIElement> UIElementList = new List<UIElement>();
                UIElementList.Add(new ColorPallete(ContentDistributor.colorPallete,ContentDistributor.colorSwatchHighlightBox, new Vector2(200,200)));
                MenuScreen newInterface = new MenuScreen(UIElementList);
                return newInterface;
            }

            public void display(SpriteBatch spriteBatch, Vector2 mouseLocation, int screenWidth, int screenHeight)
            {
                foreach (UIElement button in UIElements)
                {
                    button.draw(spriteBatch, mouseLocation);

                }

                foreach (UIElement button in UIElements)
                {
                    if (button.hasToolTip() && button.locIsWithinElement(mouseLocation))
                    {
                        string toDraw = button.getToolTip();
                        int toolTipWidthInPixels = (int)(toDraw.Length * 8.1429) + 3;
                        Vector2 toolTipLoc = new Vector2(button.getRectangle().Center.X - toolTipWidthInPixels/2,
                            button.getRectangle().Center.Y-button.getRectangle().Height / 2-23);//mouseLocation + new Vector2(20, 0);
                        if (toolTipLoc.X + toolTipWidthInPixels > screenWidth)
                        {
                            toolTipLoc.X = screenWidth - toolTipWidthInPixels;
                        }

                        spriteBatch.Draw(ContentDistributor.consoleBackground, new Rectangle((int)toolTipLoc.X - 3, (int)(toolTipLoc.Y - 3), toolTipWidthInPixels, 20), Color.White);
                        spriteBatch.DrawString(ContentDistributor.toolTipFont, toDraw, toolTipLoc, Color.White);
                    }
                }

                
            }

            public bool disablesPlayerInteraction()
            {
                return willDisablePlayerInteraction;
            }

            public virtual List<MenuAction> incrementSelection()
            {
                List<MenuAction> result = new List<MenuAction>();
                foreach (UIElement test in UIElements)
                {
                    result.AddRange(test.incrementSelection());
                }
                return result;
            }

            public virtual List<MenuAction> decrementSelection()
            {
                List<MenuAction> result = new List<MenuAction>();
                foreach (UIElement test in UIElements)
                {
                    result.AddRange(test.decrementSelection());
                }
                return result;
            }
        
    }
}
