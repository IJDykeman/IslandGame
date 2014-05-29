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
using CubeAnimator;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;


namespace IslandGame
{
    class Game
    {
        
        public static readonly int screenWidth = 1280;
        public static readonly int screenHeight = 720;



        public GameWorld.World world;
        Player player;
        Main main;



        public Game(GraphicsDevice device)
        {
            GameWorld.ColorPallete.loadContent();
            Compositer.device = device;
            player = new Player();
            newGame();

        }

        public void Initialize()
        {

        }

        public void LoadContent(ContentManager content)
        {
            Compositer.LoadContent(content);
            player.loadContent();  
        }

        public void saveGame()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("IsaacsCoolSerial.bin",
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
           // try
          //  {
                formatter.Serialize(stream, world);
          //  }
          //  catch (Exception e)
          //  {
          //      string message = e.Message;
           // }
            stream.Close();
        }

        public void newGame()
        {
            world = new World();
        }

        public void loadGame()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("IsaacsCoolSerial.bin",
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);

            world = (GameWorld.World)formatter.Deserialize(stream);
            stream.Close();
            world.setUpAfterGameLoad();
        }

        public void UnloadContent()
        {

        }

        public void Update(Main main)
        {
            this.main = main;
            handleConsoleActions(IConsole.update(player.getCameraLoc()));



            world.getIslandManager().setIslandMipsWithCameraLocation(player.getCameraLoc());
            

            player.updateFirstStep();
            handlePlayerActions(player.getPlayerActions());
            world.update();
            world.handleActorActions(player.getActorActions());

            player.updateLastStep();


        }

        void handleConsoleActions(List<ConsoleAction> actions)
        {
            foreach(ConsoleAction action in actions)
            {
                switch (action.type)
                {
                    case ConsoleActions.regenIsland:
                        world.setupIslandManager();
                        break;

                    case ConsoleActions.setPlayerSpeed:
                        Player.floatingCameraSpeed = ((SetPlayerSpeedConsoleAction)action).speed;
                        break;

                    case ConsoleActions.placeIsland:
                        world.placeIsland(new IntVector3(player.getCameraLoc()).toVector3());
                        break;

                    case ConsoleActions.mipIsland:
                        world.getIslandManager().mipIslandToLevel(world.getIslandManager().getClosestIslandToLocation(player.getCameraLoc()),
                            ((mipIslandConsoleAction)action).mipLevel);
                        break;
                    case ConsoleActions.addNewCharacter:
                        world.addCharacterAt(player.getCameraLoc(), Actor.Faction.friendly);
                        break;
                    case ConsoleActions.addNewEnemy:
                        world.addCharacterAt(player.getCameraLoc(), Actor.Faction.enemy);
                        break;
                    case ConsoleActions.addNewBoat:
                        world.addBoatAt(player.getCameraLoc());
                        break;
                    default:
                        throw new Exception("unhandled console action");
                }
            }
           
        }

        void handlePlayerActions(List<PlayerAction.Action> actions)
        {
            foreach (PlayerAction.Action action in actions)
            {
                switch (action.type)
                {
                    case PlayerAction.Type.LeftClick:
                        handleLeftClickAsCharacterSelection(action);
                        break;
                    case PlayerAction.Type.ReleaseLeftMouseButton:
                        
                        break;
                    case PlayerAction.Type.DoubleClick:
                        handleDoubleClick(action);
                        break;

                    case PlayerAction.Type.RightClick:
                        if (player.hasCharacterSelected())
                        {
                            world.handleRightClickWhenCharacterIsSelected((PlayerAction.RightClick)action, player.getSelectedCharacter());
                        }
                        break;


                    case PlayerAction.Type.PlaceExcavationMarker:
                        PlayerAction.PlaceExcavationMarker click = (PlayerAction.PlaceExcavationMarker)action;
                        Ray ray = click.getRay();
                        world.placeExcavationBlockAlongRay(ray);
                        break;

                    case PlayerAction.Type.PlayerBuildBlock:
                        PlayerAction.PlayerBuildBlock placementClick = (PlayerAction.PlayerBuildBlock)action;
                        Ray placementRay = placementClick.getRay();
                        world.placeBlockAlongRay(placementRay, player.getSelectedBlockType());
                        break;
                    case PlayerAction.Type.PlayerDestoryBlock:
                        PlayerAction.PlayerDestroyBlock destructionClick = (PlayerAction.PlayerDestroyBlock)action;
                        Ray destructionTay = destructionClick.getRay();
                        world.destroyBlockAlongRay(destructionTay);
                        break;


                    case PlayerAction.Type.ExcavationMouseover:
                        world.HandleExcavationMouseover(((PlayerAction.ExcavationMouseHover)action).getRay());
                        break;
                    case PlayerAction.Type.BoatPlacementMouseover:
                        world.HandleBoatPlacementMouseover(((PlayerAction.BoatPlacementHover)action).getRay());
                        break;
                    case PlayerAction.Type.BoatPlacement:
                        world.HandleBoatJobsitePlacement(((PlayerAction.BoatPlacement)action).getRay());
                        break;


                    case PlayerAction.Type.StartDragging:
                        PlayerAction.StartDragging starDragClick = (PlayerAction.StartDragging)action;
                        Ray startFarmRay = starDragClick.getRay();
                        Vector3? firstDragBlock = world.getBlockAlongRay(startFarmRay);
                        if (firstDragBlock.HasValue)
                        {
                            player.setFirstBlockInDrag(new BlockLoc((Vector3)firstDragBlock));
                        }
                        break;


                    case PlayerAction.Type.Dragging:
                        PlayerAction.Dragging dragClick = (PlayerAction.Dragging)action;
                        Ray dragRay = dragClick.getRay();
                        Vector3? dragBlock = world.getBlockAlongRay(dragRay);

                        if (dragBlock.HasValue)
                        {
                           BlockLoc currentDragBlock= new BlockLoc((int)((Vector3)dragBlock).X,(int)((Vector3)dragBlock).Y,(int)((Vector3)dragBlock).Z);
                           IEnumerable<BlockLoc> draggedBlocks = world.getSurfaceBlocksBoundBy(player.getFirstBlockInDrag(), currentDragBlock);

                           foreach (BlockLoc test in draggedBlocks)
                           {
                               switch (dragClick.getType())
                               {
                                   case PlayerAction.Dragging.DragType.farm:
                                       WorldMarkupHandler.addFlagPathWithPosition(ContentDistributor.getRootPath() + @"worldMarkup\farmMarker.chr",
                                           test.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f));
                                       break;
                                   case PlayerAction.Dragging.DragType.storage:
                                       WorldMarkupHandler.addFlagPathWithPosition(ContentDistributor.getRootPath() + @"worldMarkup\storageMarker.chr",
                                            test.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f));
                                       break;
                               }
                           }
                        }
                        break;


                    case PlayerAction.Type.FinishDragging:
                        PlayerAction.FinishDragging finishDragClick = (PlayerAction.FinishDragging)action;
                        Ray finishDragRay = finishDragClick.getRay();
                        Vector3? finishDragBlock = world.getBlockAlongRay(finishDragRay);
                        if (finishDragBlock.HasValue)
                        {
                            BlockLoc currentFinishBlock = new BlockLoc((int)((Vector3)finishDragBlock).X, (int)((Vector3)finishDragBlock).Y, (int)((Vector3)finishDragBlock).Z);
                            IEnumerable<BlockLoc> draggedBlocks = world.getSurfaceBlocksBoundBy(player.getFirstBlockInDrag(), currentFinishBlock);
                            world.handlePlayerFinishDrag(player.getCameraLoc(), draggedBlocks, finishDragClick.getType());
                        }
                        break;


                    case PlayerAction.Type.PlaceWoodBlockPlan:
                        Ray placeWoodBlockClickRay = ((PlayerAction.MouseAction)action).getRay();
                        world.placeWoodBlockPlanAlongRay(placeWoodBlockClickRay);
                        break;


                    case PlayerAction.Type.RemoveWoodBlockPlan:
                        Ray removeWoodBlockClickRay = ((PlayerAction.MouseAction)action).getRay();
                        world.removeWoodBlockPlanAlongRay(removeWoodBlockClickRay);
                        break;


                    case PlayerAction.Type.MoveTo:
                        PlayerAction.MoveTo move = (PlayerAction.MoveTo)action;
                        AxisAlignedBoundingBox newAABB = world.AABBPhysicsCollisionOnly(move.currentAABB, move.desiredAABB);

                        player.setCameraLoc(move.desiredAABB.middle());
                        
                        break;
                    case PlayerAction.Type.setCameraLocation:
                        player.setCameraLoc(((PlayerAction.SetCameraLocation)action).newCameraLocation, (((PlayerAction.SetCameraLocation)action).getNeckAdjustment()));
                        break;
                    case PlayerAction.Type.MoveBy:
                        PlayerAction.MoveBy moveBy = (PlayerAction.MoveBy)action;
                        AxisAlignedBoundingBox oldMoveAABB = player.getThirdPersonAABB();
                        AxisAlignedBoundingBox goalMoveAABB = player.getThirdPersonAABB();
                        goalMoveAABB.loc += moveBy.moveBy;

                        AxisAlignedBoundingBox newMoveAABB = world.AABBPhysicsCollisionOnly(oldMoveAABB, goalMoveAABB);
                        
                        //AxisAlignedBoundingBox newAABB = world.AABBPhysicsCollisionOnly(move.currentAABB, move.desiredAABB);

                        player.setCameraLoc(newMoveAABB.middle());
                        break;


                    case PlayerAction.Type.Quit:
                        main.Exit();
                        break;
                    case PlayerAction.Type.Save:
                        saveGame();
                        break;
                    case PlayerAction.Type.NewGame:
                        newGame();
                        break;
                    case PlayerAction.Type.LoadGame:
                        loadGame();
                        break;
                    
                    case PlayerAction.Type.DisembodyCharacter:
                        player.disembodyCharacter();
                        break;
                    case PlayerAction.Type.DeselectCharacter:
                        player.deselectCharacter();
                        break;
                    
                    default:
                        throw new Exception("unhandled player action " + action.type);
                }
            }
        }

        

        private void handleLeftClickAsCharacterSelection(PlayerAction.Action action)
        {
            PlayerAction.LeftClick click = (PlayerAction.LeftClick)action;
            Ray ray = click.getRay();
            ray.Direction.Normalize();
            Vector3? blockVec = world.getBlockAlongRay(ray);
            if (blockVec.HasValue)
            {
                //world.addCharacterAt((Vector3)blockVec+new Vector3(0,1,0));
            }

            Character character = world.getCharacterAlongRay(ray);
            bool shouldJump = character != player.getSelectedCharacter();
            if (character == null)
            {
                player.deselectCharacter();
            }
            else
            {
                player.characterWasLeftClicked(character);
            }
            if (character != null && shouldJump)
            {
                //character.jump();
            }
        }

        private void handleDoubleClick(PlayerAction.Action action)
        {
            PlayerAction.DoubleClick click = (PlayerAction.DoubleClick)action;
            Ray ray = click.getRay();

            Vector3? blockVec = world.getBlockAlongRay(ray);
            if (blockVec.HasValue)
            {
                //world.addCharacterAt((Vector3)blockVec+new Vector3(0,1,0));
            }

            Character character = world.getCharacterAlongRay(ray);

            //player.characterWasLeftClicked(character);
            if (character != null)
            {
                player.embodyCharacter(character);
            }
        }




        public void Draw(SpriteBatch spriteBatch)
        {
            Compositer.UpdateViewMatrix(player.getCameraLoc(), player.getCameraRotation());
            Compositer.drawFinalImageFirst(player, false);
            Compositer.display(world, player, player.getControlledCharacter());

            //Compositer.effectToUse.CurrentTechnique = Compositer.effectToUse.Techniques["GreyscaleToBrownsale"];


            spriteBatch.Begin();//0, BlendState.Opaque, null, null, null, Compositer.effectToUse);
                IConsole.display(spriteBatch, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight);
                player.display2D(spriteBatch);
            spriteBatch.End();




        }
    }
}
