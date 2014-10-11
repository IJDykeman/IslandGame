using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using IslandGame.GameWorld;
using IslandGame.menus;

namespace IslandGame
{
    class FirstPersonInputHandler : PlayerInputHandler
    {

        Character embodiedCharacter;
        int timeOfLeftClickHold = 0;
        int timeOfRightClickHold = 0;
        int timeOfSwing = 25;

        public FirstPersonInputHandler(Character nSelectedCharacter)
        {
            
            currentMenu = MenuScreen.getFirstPersonHud(Compositer.getScreenWidth(), Compositer.getScreenHeight());
            embodiedCharacter = nSelectedCharacter;
        }

        public override List<PlayerAction.Action> updateAndGetPlayerActions()
        {
            KeyboardState keyState = Player.currentKeyboardState;
            KeyboardState oldKeyboardState = Player.oldKeyboardState;


            List<PlayerAction.Action> result = new List<PlayerAction.Action>();

            if (justHit(Keys.Escape, keyState, oldKeyboardState))
            {
                result.Add(new PlayerAction.DisembodyCharacter());
            }
            else
            {

                result.Add(new PlayerAction.SetCameraLocation(getIdealPlayerCameraLocation(), .2f));
            }

            //adds scroll wheel actions to list
            MouseState currentMouseState = Player.currentMouseState;
            MouseState oldMouseState = Player.oldMouseState;
            List<MenuAction> menuActionsFromScrollWheel = new List<MenuAction>();
            if (currentMouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue)
            {
                menuActionsFromScrollWheel.AddRange(currentMenu.incrementSelection());
            }
            else if (currentMouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue)
            {
                menuActionsFromScrollWheel.AddRange(currentMenu.decrementSelection());
            }
            handleMenuActionList(result, menuActionsFromScrollWheel);


            return result;
        }

        private Vector3 getIdealPlayerCameraLocation()
        {
            return embodiedCharacter.getFootLocation() + new Vector3(0, 1.6f, 0);
        }

        public override List<ActorAction> updateAndGetActorActions()
        {
            KeyboardState oldKeyboardState = Player.oldKeyboardState;
            KeyboardState currentKeyboardstate = Player.currentKeyboardState;
            MouseState currentMouseState = Player.currentMouseState;
            MouseState oldMouseState = Player.oldMouseState;
            List<ActorAction> result = new List<ActorAction>();
            


            if (justHit(Keys.Space, currentKeyboardstate, oldKeyboardState))
            {
                embodiedCharacter.jump();
            }

            handleMouse(ref currentMouseState, ref oldMouseState, result);



            Vector3 moveVector = getMoveVector(currentKeyboardstate);
            moveVector.Normalize();
            moveVector *= embodiedCharacter.getWalkForceUnderDirectControl();
            embodiedCharacter.setRotationWithGivenDeltaVec(Player.getPlayerAimingAtPointAtDistance(1, currentMouseState) - Player.getPlayerAimingAtPointAtDistance(0, currentMouseState));

            if (moveVector.Length() > 0.0f)
            {
                ActorAction moveAction = embodiedCharacter.getAddVelocityAction(moveVector,true);
                embodiedCharacter.setIsWalkingOverride(true);
                result.Add(moveAction);
            }
            else
            {
                embodiedCharacter.setIsWalkingOverride(false);
            }



            return result;
        }

        private void handleMouse(ref MouseState currentMouseState, ref MouseState oldMouseState, List<ActorAction> result)
        {
            if (currentMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton != ButtonState.Pressed)
            {
                result.Add(embodiedCharacter.getRightClickAction(Player.getPlayerAimingAtPointAtDistance(0, currentMouseState),
                    Player.getPlayerAimingAtPointAtDistance(1, currentMouseState)));
            }

            if (currentMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton != ButtonState.Pressed)
            {

                    embodiedCharacter.startHammerAnimation();
                    leftClicked();
                
            }
            if (currentMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Pressed)
            {
                leftClickHeld();
                if (timeToStrikeLeft())
                {
                    result.Add(embodiedCharacter.getLeftClickAction(Player.getPlayerAimingAtPointAtDistance(0, currentMouseState),
                                Player.getPlayerAimingAtPointAtDistance(1, currentMouseState), selectedBlockType));
                }
                if (timeToSwingLeft())
                {
                    embodiedCharacter.startHammerAnimation();
                }
            }
            if (currentMouseState.LeftButton != ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Pressed)
            {
                leftReleased();
            }


        }

        public override Vector2 getDeltaTiltFromMouselook()
        {
            MouseState oldMouseState = Player.oldMouseState;
            MouseState currentMouseState = Player.currentMouseState;
            Mouse.SetPosition(Compositer.device.Viewport.Width / 2, Compositer.device.Viewport.Height / 2);
            float xDifference = 0;
            float yDifference = 0;

            xDifference = currentMouseState.X - Compositer.device.Viewport.Width / 2;
            yDifference = currentMouseState.Y - Compositer.device.Viewport.Height / 2;



            Vector2 result = new Vector2();
            result.X = -xDifference * mouseSensitivity;
            result.Y  =  -yDifference * mouseSensitivity;
            return result;
        }

        void leftClicked()
        {
            timeOfLeftClickHold = 1;
        }
        void leftReleased()
        {
            timeOfLeftClickHold = 0;
        }
        void leftClickHeld()
        {
            timeOfLeftClickHold++;
            timeOfLeftClickHold %= timeOfSwing+1;
        }
        bool timeToStrikeLeft()
        {
            return timeOfLeftClickHold == timeOfSwing ;
        }
        bool timeToSwingLeft()
        {
            return timeOfLeftClickHold == 0;
        }

        public override bool wantsGamePaused()
        {
            return false;
        }
        

         /*           if (currentMouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue)
            {
                result.AddRange(currentMenu.incrementSelection());
            }
            else if (currentMouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue)
            {
                currentMenu.decrementSelection();
            }*/



    }
}
