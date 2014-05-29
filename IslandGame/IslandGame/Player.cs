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

using IslandGame.menus;
using IslandGame.GameWorld;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;


namespace IslandGame
{
    public class Player
    {
        

        public static bool galleryMode = false;

        public static MouseState oldMouseState;
        public static MouseState currentMouseState;
        public static KeyboardState oldKeyboardState;
        public static KeyboardState currentKeyboardState;
        

        private BlockLoc firstBlockInDrag;

        bool embodying=false;
        Character selectedCharacter;
        LocationInterpolator smoothCam;

        

        public static float floatingCameraSpeed = .4f;

        public static float leftRightRot = -5.6f;
        public static float upDownRot = 0;

        int age = 0;
        
        
        PlayerInputHandler inputHandler;

        private float mouseSensitivity = .004f;


        public Player()
        {
            smoothCam = new LocationInterpolator(new Vector3(2079, 50, 1590));

        }

        public void loadContent()
        {
            inputHandler = new ThirdPersonInputHandler();
            oldMouseState = Mouse.GetState();

            inputHandler.openMainMenu();
        }


        public void updateFirstStep()
        {
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
        }

        public List<ActorAction> getActorActions()
        {
            return inputHandler.updateAndGetActorActions();
        }

        public List<PlayerAction.Action> getPlayerActions()
        {
            List<PlayerAction.Action> result = new List<PlayerAction.Action>();

            MouseState currentMouseState = Mouse.GetState();

            
            result = inputHandler.updateAndGetPlayerActions();

            
            smoothCam.getMoveByVecWithMinSpeed(floatingCameraSpeed);


            return result;
        }

        public void updateLastStep()
        {

            Vector2 deltaTilt = inputHandler.getDeltaTiltFromMouselook();
            leftRightRot += deltaTilt.X;
            upDownRot += deltaTilt.Y;
            //upDownRot = -(float)Math.PI / 2.0f;


            oldKeyboardState = currentKeyboardState;
            oldMouseState = currentMouseState;

            if (galleryMode)
            {
                int islandWidth = ChunkSpace.chunkWidth * ChunkSpace.widthInChunksStaticForGallery;
                smoothCam = new LocationInterpolator(
                    new Vector3((float)Math.Cos(age / 40.0), ((float)Math.Cos(age / 100.0 + 10.0) + 1.0f) / 4f
                        + .1f, (float)Math.Sin(age / 40.0)) * islandWidth 
                        + new Vector3(islandWidth / 2, 0, islandWidth / 2));
            }



            age++;
        }







        public void display2D(SpriteBatch spriteBatch)
        {

            if (inputHandler.currentMenu != null)
            {
                inputHandler.currentMenu.display(spriteBatch);
            }
            if (isEmbodyingCharacter())
            {
                spriteBatch.Draw(ContentDistributor.crossReticle, new Rectangle(Compositer.device.Viewport.Width/2 - 
                    ContentDistributor.crossReticle.Width / 2, Compositer.device.Viewport.Height/2 - ContentDistributor.crossReticle.Height / 2,
                    ContentDistributor.crossReticle.Width, ContentDistributor.crossReticle.Height), Color.White);
            }
        }

        public void display3D()
        {
            if (selectedCharacter != null && !isEmbodyingCharacter())
            {
                Compositer.addFlagForThisFrame(selectedCharacter.getLocation() + new Vector3(0, 1.1f, 0), "white");
            }
        }




        public static Vector3 getPlayerAimingAtPointAtDistance(float distance, MouseState mouseState)
        {

            Vector3 far = new Vector3(mouseState.X, mouseState.Y, distance);

            far = Compositer.device.Viewport.Unproject(far, Compositer.getPerspectiveMatrix(2000), Compositer.viewMatrix, Matrix.Identity);
            return far;
        }

        public void setCameraLoc(Vector3 nCamLoc)
        {
            smoothCam.setIdealCameraLocation(nCamLoc);

        }

        public void setCameraLoc(Vector3 nCamLoc, float neckAdjustmentDistance)
        {
            Vector3 neckAdjustment = new Vector3(-(float)Math.Sin(leftRightRot), 0, -(float)Math.Cos(leftRightRot)) * neckAdjustmentDistance;
            setCameraLoc(nCamLoc + neckAdjustment);
        }

        public Vector3 getCameraLoc()
        {
            return smoothCam.getCameraLocation();

        }


        public void characterWasLeftClicked(Character nSelected)
        {
            if (hasCommandOverCharacter(nSelected))
            {
                selectedCharacter = nSelected;
            }
        }

        public void deselectCharacter()
        {
            selectedCharacter = null;
        }

        public void embodyCharacter(Character nEmbodied)
        {
            if (hasCommandOverCharacter(nEmbodied))
            {
                embodying = true;
                selectedCharacter = nEmbodied;
                inputHandler = new FirstPersonInputHandler(selectedCharacter);
                selectedCharacter.wasJustEmbodiedByPlayer();
                selectedCharacter.quitCurrentJob();
            }
        }

        private bool hasCommandOverCharacter(Character character)
        {
            return character.getFaction() == Actor.Faction.friendly;
        }

        public void disembodyCharacter()
        {
            embodying = false;
            selectedCharacter.setIsWalkingOverride(false);
            setCameraLoc(selectedCharacter.getFootLocation() + new Vector3(0,10,0));
            inputHandler = new ThirdPersonInputHandler();
        }

        public Character getSelectedCharacter()
        {
            if (hasCharacterSelected())
            {
                return selectedCharacter;
            }
            else
            {
                return null;
            }
        }

        public bool hasCharacterSelected()
        {
            return selectedCharacter != null;
        }

        public bool isEmbodyingCharacter() {
            return embodying;
        }

        public void setFirstBlockInDrag(BlockLoc toSet)
        {
            firstBlockInDrag = toSet;
        }

        public BlockLoc getFirstBlockInDrag()
        {
            return firstBlockInDrag;
        }

        public bool controllingCharacter()
        {
            return embodying;
        }

        public Character getControlledCharacter()
        {
            if (embodying)
            {
                return selectedCharacter;
            }
            else
            {
                return null;
            }
        }

        public Quaternion getCameraRotation()
        {
            return Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftRightRot));
        }

        Vector3 getEmbodiedCameraLoc()
        {
            return (selectedCharacter.getBoundingBox().Min + selectedCharacter.getBoundingBox().Max) / 2f + new Vector3(0, .6f, 0);
        }

        public AxisAlignedBoundingBox getThirdPersonAABB()
        {
            return new GameWorld.AxisAlignedBoundingBox(getCameraLoc() - new Vector3(.3f, .3f, .3f), getCameraLoc() + new Vector3(.3f, .3f, .3f));
        }





        public byte getSelectedBlockType()
        {
            return inputHandler.selectedBlockType;
        }

    }
}
