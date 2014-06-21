using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IslandGame.menus;
using IslandGame.GameWorld;

namespace IslandGame
{
    class ThirdPersonInputHandler : PlayerInputHandler
    {

        public ThirdPersonInputHandler()
        {
            currentMenu = MenuScreen.getGameplayHud(Compositer.getScreenWidth(), Compositer.getScreenHeight());
            currentInterfaceState = InterfaceStates.playing;
        }

        public override List<PlayerAction.Action> updateAndGetPlayerActions()
        {
            List<PlayerAction.Action> result = new List<PlayerAction.Action>();



            if (Player.oldMouseState.LeftButton == ButtonState.Released && Player.currentMouseState.LeftButton == ButtonState.Pressed)
            {
                addMenuActionsToList(result);
            }

            if (currentMenu.disablesPlayerInteraction())
            {
                return result; 
            }

            result.AddRange(processThirdPersonKeyboard());
            if (!currentMenu.clickLocHitsMenu(new Vector2(Player.currentMouseState.X, Player.currentMouseState.Y)))
            {
                result.AddRange(processThirdPersonMouse());
            }

            return result;
        }

        private List<PlayerAction.Action> processThirdPersonKeyboard()
        {
            List<PlayerAction.Action> result = new List<PlayerAction.Action>();

            KeyboardState keyState = Player.currentKeyboardState;
            KeyboardState oldKeyboardState = Player.oldKeyboardState;

            if (justHit(Keys.Escape, keyState, oldKeyboardState))
            {
                if (currentInterfaceState != InterfaceStates.playing && currentInterfaceState != InterfaceStates.inMainMenu)
                {
                    currentInterfaceState = InterfaceStates.playing;
                }
                else
                {
                    openMainMenu();
                }
            }

            if (( currentInterfaceState == InterfaceStates.buildingDirectly) && IConsole.active == false)
            {
                if (justHit(Keys.Space, keyState, oldKeyboardState))
                {
                    switchToColorPallete();
                }

            }

            if (justReleased(Keys.Space, keyState, oldKeyboardState))
            {
                closeColorPallete();
            }




            Vector3 moveVector = getMoveVector(keyState);

            if (moveVector.Length() != 0)
            {
                result.Add(new PlayerAction.MoveBy(moveVector));
            }
            //result.Add(new PlayerAction.MoveTo(currentAABB, desiredAABB));

            oldKeyboardState = keyState;
            return result;
        }

        private MouseState AddThirdPersonHoverActionsToResult(List<PlayerAction.Action> result)
        {
            MouseState currentMouseState = Player.currentMouseState;
            switch (currentInterfaceState)
            {
                case InterfaceStates.playing:
                    break;
                case InterfaceStates.inMainMenu:
                    break;
                case InterfaceStates.placingExcavation:
                    result.Add(new PlayerAction.ExcavationMouseHover(Player.getPlayerAimingAtPointAtDistance(0, currentMouseState), Player.getPlayerAimingAtPointAtDistance(1, currentMouseState)));
                    break;
                case InterfaceStates.placingBoat:
                    result.Add(new PlayerAction.BoatPlacementHover(Player.getPlayerAimingAtPointAtDistance(0, currentMouseState), Player.getPlayerAimingAtPointAtDistance(1, currentMouseState)));
                    break;
                case InterfaceStates.placingWoodPlanBlocks:
                    result.Add(new PlayerAction.BlockPlanPlacementHover(Player.getPlayerAimingAtPointAtDistance(0, currentMouseState), Player.getPlayerAimingAtPointAtDistance(1, currentMouseState)));
                    break;
            }
            return currentMouseState;
        }


        //
        List<PlayerAction.Action> processThirdPersonMouse()
        {
            List<PlayerAction.Action> result = new List<PlayerAction.Action>();

            MouseState currentMouseState = Player.currentMouseState;
            KeyboardState currentKeyboardState = Player.currentKeyboardState;

            MouseState oldMouseState = Player.oldMouseState ;

            currentMouseState = AddThirdPersonHoverActionsToResult(result);

            Vector3 nearPoint = Player.getPlayerAimingAtPointAtDistance(0, currentMouseState);
            Vector3 farPoint = Player.getPlayerAimingAtPointAtDistance(1, currentMouseState);




            if (currentMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton != ButtonState.Pressed)
            {
                AddThirdPersonLeftMouseClickActionsToResult(result, currentMouseState);
            }

            //dealing with block placement clicks with leftctrl auto click
            if (currentInterfaceState == InterfaceStates.buildingDirectly)
            {

                if (currentMouseState.LeftButton == ButtonState.Pressed && (oldMouseState.LeftButton != ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.LeftControl)))
                    {
                        result.Add(new PlayerAction.PlayerDestroyBlock(nearPoint, farPoint));
                    }

                    if (currentMouseState.RightButton == ButtonState.Pressed && (oldMouseState.RightButton != ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.LeftControl)))
                    {
                        result.Add(new PlayerAction.PlayerBuildBlock(nearPoint, Player.getPlayerAimingAtPointAtDistance(1, currentMouseState)));
                    }
                
            }


            if (currentMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton != ButtonState.Pressed)
            {
                if (currentInterfaceState == InterfaceStates.placingWoodPlanBlocks)
                {
                    result.Add(new PlayerAction.PlaceWoodBlockPlan(nearPoint, farPoint));
                }
                result.Add(new PlayerAction.RightClick(nearPoint, farPoint));
            }


            if (leftMouseButtonReleased(ref currentMouseState, ref oldMouseState) )
            {
                   
                switch (currentInterfaceState)
                {
                    case InterfaceStates.placingFarm:
                        result.Add(new PlayerAction.FinishDragging(nearPoint, farPoint, PlayerAction.Dragging.DragType.farm));
                        currentInterfaceState = InterfaceStates.playing; 
                        break;
                    case InterfaceStates.placingWoodStorage:
                       
                        result.Add(new PlayerAction.FinishDragging(nearPoint, farPoint, PlayerAction.Dragging.DragType.storeWood));
                        currentInterfaceState = InterfaceStates.playing; 
                        break;
                    case InterfaceStates.placingWheatStorage:
                        result.Add(new PlayerAction.FinishDragging(nearPoint, farPoint, PlayerAction.Dragging.DragType.storeWheat));
                        currentInterfaceState = InterfaceStates.playing;
                        break;

                }
                
                
            }

            //left mouse button held
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                    
                    switch (currentInterfaceState)
                    {
                        case InterfaceStates.placingFarm:

                            result.Add(new PlayerAction.Dragging(nearPoint,
                                farPoint,PlayerAction.Dragging.DragType.farm));
                            break;
                        case InterfaceStates.placingWheatStorage:
                            result.Add(new PlayerAction.Dragging(nearPoint,
                                farPoint, PlayerAction.Dragging.DragType.storeWheat));
                            break;
                        case InterfaceStates.placingExcavation:
                            result.Add(new PlayerAction.ExcavationMouseHover(nearPoint,
                                farPoint));
                            break;
                    }
                
            }

            timeSinceLastLeftClick++;
            return result;

        }

        private static bool leftMouseButtonReleased(ref MouseState currentMouseState, ref MouseState oldMouseState)
        {
            return currentMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton != ButtonState.Released;
        }


        public override List<ActorAction> updateAndGetActorActions()
        {
            return new List<ActorAction>();
        }

        public override Vector2 getDeltaTiltFromMouselook()
        {
            if (currentMenu.disablesPlayerInteraction())
            {
                return new Vector2();
            }

            MouseState currentMouseState = Player.currentMouseState;
            MouseState oldMouseState = Player.oldMouseState;


            float xDifference = 0;
            float yDifference = 0;
            if (currentMouseState.MiddleButton == ButtonState.Pressed)
            {
                xDifference = currentMouseState.X - oldMouseState.X;
                yDifference = currentMouseState.Y - oldMouseState.Y;
            }



            Vector2 result = new Vector2();
            result.X = -xDifference * mouseSensitivity;
            result.Y = -yDifference * mouseSensitivity;

            return result;
        }

        private void AddThirdPersonLeftMouseClickActionsToResult(List<PlayerAction.Action> result, MouseState currentMouseState)
        {

                Vector3 nearPoint = Player.getPlayerAimingAtPointAtDistance(0, currentMouseState);
                Vector3 farPoint =  Player.getPlayerAimingAtPointAtDistance(1, currentMouseState);

                switch (currentInterfaceState)
                {
                    case InterfaceStates.playing:
                        if (timeSinceLastLeftClick > doubleLeftClickInterval)
                        {
                            timeSinceLastLeftClick = 0;
                            result.Add(new PlayerAction.LeftClick(nearPoint,
                               farPoint));
                        }
                        else
                        {
                            result.Add(new PlayerAction.DoubleClick(nearPoint,
                               farPoint));
                        }
                        break;
                    case InterfaceStates.inMainMenu:
                        break;
                    case InterfaceStates.placingExcavation:
                        result.Add(new PlayerAction.PlaceExcavationMarker(nearPoint,farPoint));
                        break;
                    case InterfaceStates.placingBoat:
                        result.Add(new PlayerAction.BoatPlacement(nearPoint,farPoint));
                        break;
                    case InterfaceStates.placingFarm:
                        result.Add(new PlayerAction.StartDragging(nearPoint, farPoint, PlayerAction.Dragging.DragType.farm));
                        break;
                    case InterfaceStates.placingWoodPlanBlocks:
                        result.Add(new PlayerAction.RemoveWoodBlockPlan(nearPoint,farPoint));
                        break;
                    case InterfaceStates.placingWoodStorage:
                        PlayerAction.Dragging.DragType dragType = PlayerAction.Dragging.DragType.storeWood;
                        result.Add(new PlayerAction.StartDragging(nearPoint, farPoint, dragType));
                        break;
                    case InterfaceStates.placingWheatStorage:
                        PlayerAction.Dragging.DragType dragWheat = PlayerAction.Dragging.DragType.storeWheat;
                        result.Add(new PlayerAction.StartDragging(nearPoint, farPoint, dragWheat));
                        break;

                }

        }




    }
}
