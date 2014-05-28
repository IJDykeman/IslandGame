using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using IslandGame.menus;

namespace IslandGame
{
    abstract class PlayerInputHandler
    {


        protected float mouseSensitivity = .004f;

        public MenuScreen currentMenu = null;

        protected int timeSinceLastLeftClick = 200;
        protected static readonly int doubleLeftClickInterval = 15;

        public byte selectedBlockType { get; set; }

        public  enum InterfaceStates
        {
            inMainMenu,
            playing,
            placingBlocks,
            placingExcavation,
            placingFarm,
            placingStorage,
            placingWoodPlanBlocks,
            buildingDirectly,
            placingBoat
        }


        protected InterfaceStates currentInterfaceState = InterfaceStates.inMainMenu;

        public abstract List<PlayerAction.Action> updateAndGetPlayerActions();

        public abstract List<IslandGame.GameWorld.ActorAction> updateAndGetActorActions();

        public abstract Vector2 getDeltaTiltFromMouselook();

        public static bool justHit(Keys test, KeyboardState currentKeyboardState, KeyboardState oldKeyboardState)
        {
            bool result = false;

            if (currentKeyboardState.IsKeyDown(test))
            {

                if (!oldKeyboardState.IsKeyDown(test))
                {
                    result = true;
                }
            }
            return result;
        }

        public static bool justReleased(Keys test, KeyboardState currentKeyboardState, KeyboardState oldKeyboardState)
        {

            if (!currentKeyboardState.IsKeyDown(test))
            {

                if (oldKeyboardState.IsKeyDown(test))
                {
                    return true;
                }
            }
            return false;
        }

        protected Vector3 getMoveVector(KeyboardState keyState)
        {
            Vector3 moveVector = new Vector3();

            if (keyState.IsKeyDown(Keys.W))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, -1, 0);

            moveVector = Vector3.Transform(moveVector * Player.floatingCameraSpeed, Matrix.CreateRotationY(Player.leftRightRot));

            if (IConsole.isOpen())
            {
                moveVector = new Vector3();
            }
            return moveVector;
        }

        public void setInterfaceState(InterfaceStates newState)
        {
            currentInterfaceState = newState;
        }

        protected void addMenuActionsToList(List<PlayerAction.Action> result)
        {
            MouseState mouseState = Player.currentMouseState;
            List<MenuAction> menuActions = currentMenu.click(new Vector2(mouseState.X, mouseState.Y));
            foreach (MenuAction action in menuActions)
            {
                switch (action.type)
                {
                    case MenuActionType.Continue:
                        closeMainMenu();
                        break;
                    case MenuActionType.Save:
                        result.Add(new PlayerAction.Save());
                        break;
                    case MenuActionType.NewGame:
                        result.Add(new PlayerAction.NewGame());
                        break;
                    case MenuActionType.Quit:
                        result.Add(new PlayerAction.Quit());
                        break;
                    case MenuActionType.Load:
                        result.Add(new PlayerAction.LoadGame());
                        break;
                    case MenuActionType.ExcavationHudButtonClick:
                        result.Add(new PlayerAction.DeselectCharacter());
                        setInterfaceState(PlayerInputHandler.InterfaceStates.placingExcavation);
                        break;
                    case MenuActionType.FarmHudButtonClick:
                        result.Add(new PlayerAction.DeselectCharacter());
                        setInterfaceState(PlayerInputHandler.InterfaceStates.placingFarm);
                        break;
                    case MenuActionType.StorageHudClick:
                        result.Add(new PlayerAction.DeselectCharacter());
                        setInterfaceState(PlayerInputHandler.InterfaceStates.placingStorage);
                        break;
                    case MenuActionType.WoodBuildHudClick:
                        result.Add(new PlayerAction.DeselectCharacter());
                        setInterfaceState(PlayerInputHandler.InterfaceStates.placingWoodPlanBlocks);
                        break;
                    case MenuActionType.PlayerBuildHudClick:
                        result.Add(new PlayerAction.DeselectCharacter());
                        setInterfaceState(PlayerInputHandler.InterfaceStates.buildingDirectly);
                        break;
                    case MenuActionType.PlayerBuildBoatHudClick:
                        result.Add(new PlayerAction.DeselectCharacter());
                        setInterfaceState(PlayerInputHandler.InterfaceStates.placingBoat);
                        break;
                    case MenuActionType.ColorPalleteColorSelection:
                        selectedBlockType = ((ColorPalleteColorSelection)action).selectedColor;
                        break;
                    default:
                        throw new Exception("unhandled menu action");

                }

            }

        }

        void closeMainMenu()
        {
            currentMenu = MenuScreen.getGameplayHud(Compositer.getScreenWidth(), Compositer.getScreenHeight());
            setInterfaceState(PlayerInputHandler.InterfaceStates.playing);
        }

        public void openMainMenu()
        {
            currentMenu = MenuScreen.getMainMenuInterface(Compositer.getScreenWidth(), Compositer.getScreenHeight());
            setInterfaceState(PlayerInputHandler.InterfaceStates.inMainMenu);
        }

        public void closeColorPallete()
        {
            currentMenu = MenuScreen.getGameplayHud(Compositer.getScreenWidth(), Compositer.getScreenHeight());
        }

        public void switchToRegularPlayMode()
        {
            closeMainMenu();
        }

        public void switchToColorPallete()
        {
            currentMenu = MenuScreen.getColorPalleteInterface(Compositer.getScreenWidth(), Compositer.getScreenHeight());
        }

    }
}
