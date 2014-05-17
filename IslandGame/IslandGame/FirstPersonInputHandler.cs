using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using IslandGame.GameWorld;

namespace IslandGame
{
    class FirstPersonInputHandler : PlayerInputHandler
    {

        Character embodiedCharacter;

        public FirstPersonInputHandler(Character nSelectedCharacter)
        {
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
            moveVector *= embodiedCharacter.getSpeed();
            embodiedCharacter.setRotationWithGivenDeltaVec(Player.getPlayerAimingAtPointAtDistance(1, currentMouseState) - Player.getPlayerAimingAtPointAtDistance(0, currentMouseState));

            if (moveVector.Length() > 0.0f)
            {
                ActorAction moveAction = embodiedCharacter.getMoveToActionWithMoveByVector(moveVector);
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
                if (embodiedCharacter.canSwing())
                {
                    embodiedCharacter.StartHammerAnimationIfPossible();
                    result.Add(embodiedCharacter.getLeftClickAction(Player.getPlayerAimingAtPointAtDistance(0, currentMouseState),
                    Player.getPlayerAimingAtPointAtDistance(1, currentMouseState)));
                }
                
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



    }
}
