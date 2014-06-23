using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class World
    {
        IslandManager islandManager;
        ActorManager actorManager;
        GameDirector gameDirector;


        public World()
        {
            setupIslandManager();
            actorManager = new ActorManager();
            gameDirector = new GameDirector();

        }


        public void setUpAfterGameLoad()
        {
            islandManager.setUpAfterGameLoad();
        }

        public void update()
        {
            gameDirector.update();
            Compositer.setSkyColors(gameDirector.getSkyHorizonColor(), gameDirector.getSkyZenithColor(), gameDirector.getAmbientBrighness());
            handleActorActions(actorManager.update());
            islandManager.update();
        }

        public void handleActorActions(List<ActorAction> actions)
        {
            foreach (ActorAction action in actions)
            {
                switch (action.type)
                {

                    case ActorActions.placeBoat:
                        addBoatAt(((ActorPlaceBoatAction)action).getBoatLocToPlace().toWorldSpaceVector3());
                        break;

                    case ActorActions.moveTo:
                        handleMoveToAction(action);

                        break;
                    case ActorActions.rightClickAction:
                        Ray ray = new Ray( ((ActorRightClickAction)action).nearClickPoint,
                            ((ActorRightClickAction)action).farClickPoint-((ActorRightClickAction)action).nearClickPoint);
                        Boat boatClicked = actorManager.getNearestActorOfTypeAlongRay<Boat>(ray);
                        if (boatClicked != null)
                        {
                            ((ActorRightClickAction)action).character.rightClickedBoat(boatClicked);
                        }
                        break;
                    case ActorActions.setShipVelocity:
                        ((ActorSetShipVelocity)action).getBoat().setVelocity(((ActorSetShipVelocity)action).getNewVelocity());
                        break;
                    case ActorActions.PlaceResource:

                        islandManager.addResourceBlock(((ActorPlaceResourceAction)action).getLocToPlace()
                            , ((ActorPlaceResourceAction)action).getRescourceTypeToPlace());
                        break;
                    case ActorActions.PickUpResource:

                        islandManager.removeResourceBlock(((ActorPickUpResourceAction)action).getLocToPlace()
                            , ((ActorPickUpResourceAction)action).getRescourceTypeToPlace());
                        break;
                    case ActorActions.strike:
                        handleStrikeAction(action);
                        break;
                    case ActorActions.die:
                        actorManager.deleteActor(((ActorDieAction)action).getActorToBeKilled());
                        break;

                }
            }
        }

        private void handleMoveToAction(ActorAction action)
        {
            ActorMoveToAction moveAction = (ActorMoveToAction)action;

            Island closestIsland = islandManager.getClosestIslandToLocation(moveAction.character.getLocation());
            if (closestIsland == null)// if no islands are loaded
            {
                moveAction.character.setVelocity(new Vector3());
            }
            else
            {

                List<BlockLoc> intersectedByActor = moveAction.character.getBlocksIntersectedByAABB();
                IslandPathingProfile profile = closestIsland.getPathingProfile();
                foreach (BlockLoc test in intersectedByActor)
                {
                    if (profile.isProfileSolidAt(test))
                    {
                        moveAction.character.setFootLocation(moveAction.character.getFootLocation() + new Vector3(0, .3f, 0));
                        return;
                    }
                }
                runPhysicsWithMoveActionAndIsland(moveAction, closestIsland);
            }
        }

        private void handleStrikeAction(ActorAction action)
        {
            ActorStrikeAction strikeAction = (ActorStrikeAction)action;
            if (strikeAction.getStrikeType() == ActorStrikeAction.StrikeType.OnBlock)
            {
                switch (strikeAction.getJobType())
                {

                    case JobType.combat:
                        break;

                    case JobType.agriculture:
                        islandManager.acceptWorkStrike((ActorStrikeAction)action);
                        break;
                    case JobType.building:
                        buildBlockAt(((ActorStrikeBlockAction)action).getStrikeTarget(),
                            5);
                        break;
                    case JobType.mining:
                        strikeAction.getStriker().setRotationWithGivenDeltaVec(strikeAction.getLookDeltaVec());
                        islandManager.acceptWorkStrike((ActorStrikeAction)action);
                        break;
                    case JobType.logging:
                        strikeAction.getStriker().setRotationWithGivenDeltaVec(strikeAction.getLookDeltaVec());
                        islandManager.acceptWorkStrike((ActorStrikeAction)action);
                        //chopBlockAt(((ActorStrikeBlockAction)action).getStrikeTarget());
                        break;
                }

            }
            else // is strike along ray
            {
                ActorStrikeAlongRayAction rayStrike = (ActorStrikeAlongRayAction)strikeAction;
                switch (strikeAction.getJobType())
                {

                    case JobType.combat:

                        actorManager.handleStrike(rayStrike.getStriker(), rayStrike.getStrikeOrigen(),
                            rayStrike.getStrikeDirectionNormal(), rayStrike.getStrikeDistance(), 1);
                        rayStrike.getStriker().StartStrikeAnimation();
                        break;

                    case JobType.agriculture:
                        islandManager.acceptWorkStrike(rayStrike);
                        break;
                    case JobType.building:
                        //buildBlockAt(((ActorStrikeBlockAction)action).getStrikeTarget(),
                        //    5);
                        islandManager.acceptWorkStrike(rayStrike);
                        break;
                    case JobType.mining:
                        islandManager.acceptWorkStrike(rayStrike);
                        break;
                    case JobType.logging:
                        islandManager.acceptWorkStrike(rayStrike);
                        break;
                }

            }
        }

        private static void runPhysicsWithMoveActionAndIsland(ActorMoveToAction moveAction, Island closestIsland)
        {
            Vector3 originalDesiredCharacterAABBMiddle = moveAction.desiredAABB.middle();


            AxisAlignedBoundingBox newAABBforCharacter = closestIsland.AABBPhysics(moveAction.currentAABB, moveAction.desiredAABB);
            Vector3 newVelocity = moveAction.character.getVelocity();
            if (newAABBforCharacter.middle().X != originalDesiredCharacterAABBMiddle.X)
            {
                newVelocity.X = 0;
            }
            if (newAABBforCharacter.middle().Y != originalDesiredCharacterAABBMiddle.Y)
            {
                newVelocity.Y = 0;
            }
            if (newAABBforCharacter.middle().Z != originalDesiredCharacterAABBMiddle.Z)
            {
                newVelocity.Z = 0;
            }
            moveAction.character.setVelocity(newVelocity);
            moveAction.character.setAABB(newAABBforCharacter);
        }

        private void buildBlockAt(BlockLoc blockLoc, byte typeToBuild)
        {
            islandManager.buildBlockAt(blockLoc, typeToBuild);
        }

        public void handleRightClickWhenCharacterIsSelected(PlayerAction.RightClick rightClick, Character character)
        {

            Ray rightClickRay = rightClick.getRay();

            Job job = giveCharacterJobSiteJobReturnWhetherJobFound(character, ref rightClickRay);
            if (!(job is UnemployedJob))
            {
                character.setJobAndCheckUseability(job);
                return;
            }

            Actor clickedActor = actorManager.getNearestActorOfTypeAlongRay<Actor>(rightClickRay);
            if (clickedActor != null)
            {
                character.setJobAndCheckUseability(clickedActor.getJobWhenClicked(character, getIslandManager().getClosestIslandToLocation(clickedActor.getLocation()).getPathingProfile(), actorManager.getActorProfile()));

            }
            else
            {
                giveCharacterTravelJob(character, rightClickRay);
            }

        }

        private void giveCharacterTravelJob(Character character, Ray rightClickRay)
        {
            Vector3? clicked = getIslandManager().getClosestIslandToLocation(character.getLocation()).getLastSpaceAlongRayConsideringResourceBlocks(rightClickRay);
            if (clicked.HasValue)
            {
                IntVector3 clickedBlock = new IntVector3((Vector3)clicked);
                IslandPathingProfile profile = getIslandManager().getClosestIslandToLocation(character.getLocation()).getPathingProfile();
                
                PathHandler pathHandler = new PathHandler();

                Path path = pathHandler.getPathToSingleBlock(profile,
                    new BlockLoc(character.getFootLocation()), profile, new BlockLoc(clickedBlock.toVector3()), 2);

                TravelAlongPath walkTask = new TravelAlongPath(path);
                if (walkTask.isUseable())
                {
                    character.setJobAndCheckUseability(walkTask);
                }
                return;
            }

            Vector3? oceanClick = islandManager.getOceanIntersectionAtY1(rightClickRay);

            if (oceanClick.HasValue)
            {
                //List<BlockLoc> path = islandManager.getOceanPath((Vector3)oceanClick, character.getLocation());
                PathHandler pathHandler = new PathHandler();
                BlockLoc goal = new BlockLoc((Vector3)oceanClick);
                goal.setValuesInWorldSpace(goal.WSX(),0,goal.WSZ());
                BlockLoc start = new BlockLoc(character.getLocation());
                start.setValuesInWorldSpace(start.WSX(), 0, start.WSZ());
                Path path = pathHandler.getPathToSingleBlock(new WorldPathingProfile(islandManager),
                    start, new WorldPathingProfile(islandManager), goal, 2);
                
                character.pathAlongOceanWithOceanPath(path);
            }
            
        }

        private Job giveCharacterJobSiteJobReturnWhetherJobFound(Character character, ref Ray rightClickRay)
        {
            Vector3? righClickedBlock = getLastSpaceAlongRay(rightClickRay);
            JobSite clickedJobSite = getIslandManager().getJobSiteAlongRay(rightClickRay);
            if (clickedJobSite != null)
            {
                if (righClickedBlock.HasValue)
                {
                    if (Vector3.Distance((Vector3)righClickedBlock, rightClickRay.Position) < (float)clickedJobSite.intersects(rightClickRay)-.01f)
                    {
                        return new UnemployedJob();
                    }
                }

                Job job = clickedJobSite.getJob(character, rightClickRay, new IslandWorkingProfile(
                    islandManager.getClosestIslandToLocation(character.getFootLocation()).getJobSiteManager(), 
                    islandManager.getClosestIslandToLocation(character.getFootLocation()).getPathingProfile()));
                character.setJobAndCheckUseability(job);
                return job;
            }
            return new UnemployedJob();
        }


        public AxisAlignedBoundingBox AABBPhysicsCollisionOnly(AxisAlignedBoundingBox currentAABB, AxisAlignedBoundingBox desiredAABB)
        {
            Island closestIsland = islandManager.getClosestIslandToLocation(desiredAABB.middle());
            if (closestIsland == null)// if no islands are loaded
            {
                return desiredAABB;
            }
            AxisAlignedBoundingBox newAABB = closestIsland.AABBPhysics(currentAABB, desiredAABB);


            return newAABB;
        }

        public void setupIslandManager()
        {

            islandManager = new IslandManager();

            
        }

        public void placeIsland(Vector3 location)
        {
            islandManager.makeNewIsland(new Vector2(location.X, location.Z));
        }

        public void displayIslands(GraphicsDevice device, Effect effect, BoundingFrustum frustrum, DisplayParameters displayParameters)
        {
            islandManager.display(device, effect, frustrum, displayParameters);
        }

        public void runPreDrawCalculations()
        {
            islandManager.runPreDrawCalculations();
        }

        public void displayActors(GraphicsDevice device, Effect effect, Character doNotDisplay)
        {
            actorManager.display(device, effect, doNotDisplay);

        }

        public void addCharacterAt(Vector3 location, Actor.Faction faction)
        {
            actorManager.addCharacterAt(location, faction,
                islandManager.getClosestIslandToLocation(location).getPathingProfile(),
                actorManager.getActorProfile());
        }

        public IslandManager getIslandManager()
        {
            return islandManager;
        }

        public Character getCharacterAlongRay(Ray ray)
        {
            Vector3? blockhit = getBlockAlongRay(ray);
            //gets nearest character along ray unless land feature obstructs ray
            ray.Direction.Normalize();

            Character nearestAlongRay = actorManager.getNearestActorOfTypeAlongRay<Character>(ray);



            if (blockhit.HasValue)
            {
                if (nearestAlongRay != null)
                {
                    float? characterHit = nearestAlongRay.getBoundingBox().Intersects(ray);
                    if (characterHit.HasValue)
                    {
                        if (Vector3.Distance(ray.Position, (Vector3)blockhit) < characterHit)
                        {
                            nearestAlongRay = null;
                        }
                    }
                }



                return nearestAlongRay;
            }
            return nearestAlongRay;
        }

        public Vector3? getBlockAlongRay(Ray ray)
        {
            return islandManager.getNearestBlockHit(ray);
        }

        public Vector3? getLastSpaceAlongRay(Ray ray)
        {

            Vector3? blockLocMaybe = islandManager.getNearestBlockHit(ray);
            if (!(blockLocMaybe.HasValue))
            {
                return null;
            }
            else
            {
                Vector3 blockLoc = (Vector3)blockLocMaybe;
                BoundingBox box = new BoundingBox(new IntVector3(blockLoc).toVector3(), new IntVector3(blockLoc).toVector3() + new Vector3(1, 1, 1));
                ray.Direction.Normalize();
                float distToHit = (float)ray.Intersects(box);
                //float distToHit = Vector3.Distance(ray.Position, blockLocToAccess);
                return ray.Direction * (distToHit - .001f) + ray.Position;
            }
        }

        public void deleteCharacter(Character toDelete)
        {
            actorManager.deleteActor(toDelete);
        }

        internal void placeExcavationBlockAlongRay(Ray ray)
        {
            Island island = islandManager.getClosestIslandToLocation(ray.Position);
            island.placeExcavationMark(ray);
        }

        public void HandleExcavationMouseover(Ray hoverRay)
        {


            Vector3? space = getLastSpaceAlongRay(hoverRay);
            Vector3? block = getBlockAlongRay(hoverRay);

            if (space.HasValue && block.HasValue)
            {
                IntVector3 spaceLoc = new IntVector3((Vector3)space);
                IntVector3 blockLoc = new IntVector3((Vector3)block);

                Vector3 pointingTowardBlock = blockLoc.toVector3() - spaceLoc.toVector3();

                CubeAnimator.AnimatedBodyPartGroup flag = new CubeAnimator.AnimatedBodyPartGroup(@"worldMarkup\shovel.chr", 1f / 26f);
                flag.setRootPartLocation(spaceLoc.toVector3() + new Vector3(.5f, .5f, .5f));
                flag.setRootPartRotationOffset(Quaternion.CreateFromRotationMatrix(GeometryFunctions.getRotationMatrixFromNormal(pointingTowardBlock)));
                Compositer.addAnimatedBodyPartGroupForThisFrame(flag);
            }
        }

        public void HandleBoatPlacementMouseover(Ray ray)
        {
            Vector3? OceanBlockIntesectionAtY1 = islandManager.getOceanIntersectionAtY1(ray);
            if (OceanBlockIntesectionAtY1.HasValue && islandManager.vehiclePlacedHereCouldBeBoarded(new BlockLoc((Vector3)OceanBlockIntesectionAtY1)))
            {
                WorldMarkupHandler.addCharacter(@"boats\greenOnePersonBoat.chr",
                    (new BlockLoc((Vector3)OceanBlockIntesectionAtY1)).toWorldSpaceVector3() + new Vector3(.5f, 0, .5f),
                    IslandGame.GameWorld.Boat.SCALE,.5f);
            }
        }

        public void HandleBlockPlanPlacementMouseover(Ray ray)
        {
            Island island = islandManager.getClosestIslandToLocation(ray.Position);
            Vector3? maybeSpace = island.getLastSpaceAlongRayConsideringBuildSite(ray);
            if (maybeSpace.HasValue)
            {
                Vector3 space = (Vector3)maybeSpace;
                space.X = (int)space.X;
                space.Y = (int)space.Y;
                space.Z = (int)space.Z;
                WorldMarkupHandler.addCharacter(ContentDistributor.getRootPath() + @"worldMarkup\" + "stoneMarkerOutline" + ".chr",
                                           ((Vector3)space) + new Vector3(.5f, .5f, .5f), 1.0f / 12.0f, .6f);
            }

            Vector3? maybeBlock = island.getNearestBlockAlongRayInAndFromWorldSpace(ray);
            if (maybeBlock.HasValue)
            {
                Vector3 space = (Vector3)maybeBlock;
                space.X = (int)space.X;
                space.Y = (int)space.Y;
                space.Z = (int)space.Z;
                WorldMarkupHandler.addCharacter(ContentDistributor.getRootPath() + @"worldMarkup\" + "stoneMarkerOutline" + ".chr",
                                           ((Vector3)space) + new Vector3(.5f, .5f, .5f), 1.0f / 12.0f, .6f);
            }

        }

        internal void HandleBoatJobsitePlacement(Ray ray)
        {
            Vector3? oceanBlockIntesectionAtY1 = islandManager.getOceanIntersectionAtY1(ray);
            if (oceanBlockIntesectionAtY1.HasValue && islandManager.vehiclePlacedHereCouldBeBoarded(new BlockLoc((Vector3)oceanBlockIntesectionAtY1)))
            {
                BlockLoc boatSiteLoc = new BlockLoc((Vector3)oceanBlockIntesectionAtY1);
                Island island = islandManager.getClosestIslandToLocation((Vector3)oceanBlockIntesectionAtY1);
                island.addJobSite(new IslandGame.GameWorld.CharactersAndAI.BoatBuildSite(boatSiteLoc, island.getPathingProfile()));
            }
        }

        internal void handleCharacterPlacement(Ray ray)
        {
            Island relevant = islandManager.getClosestIslandToLocation(ray.Position);
            Vector3? toPlace = getLastSpaceAlongRay(ray);
            if (toPlace.HasValue)
            {
                if (relevant.couldAffordResourceExpendeture(12, ResourceBlock.ResourceType.Wheat))
                {
                    relevant.debitResource(12, ResourceBlock.ResourceType.Wheat);
                    addCharacterAt(new BlockLoc((Vector3)toPlace).getMiddleInWorldSpace(), Actor.Faction.friendly);
                }
            }
        }

        public LinkedList<BlockLoc> GetBlocksBoundBy(BlockLoc loc1, BlockLoc loc2)
        {
            BlockLoc min = new BlockLoc((int)Math.Min(loc1.WSX(), loc2.WSX()), (int)Math.Min(loc1.WSY(), loc2.WSY()), (int)Math.Min(loc1.WSZ(), loc2.WSZ()));
            BlockLoc max = new BlockLoc((int)Math.Max(loc1.WSX(), loc2.WSX()), (int)Math.Max(loc1.WSY(), loc2.WSY()), (int)Math.Max(loc1.WSZ(), loc2.WSZ()));

            LinkedList<BlockLoc> result = new LinkedList<BlockLoc>();

            Island relevant = getIslandManager().getClosestIslandToLocation(loc2.toWorldSpaceVector3());
            int spaceSize = (max.WSY() - min.WSY());
            Console.WriteLine(spaceSize + " space size");
            for (int x = min.WSX(); x <= max.WSX(); x++)
            {
                for (int y = min.WSY(); y <= max.WSY(); y++)
                {
                    for (int z = min.WSZ(); z <= max.WSZ(); z++)
                    {
                        BlockLoc toTest = new BlockLoc(x, y, z);
                        byte? block = islandManager.getBlockAtOnGivenIsland(ref toTest, relevant);
                        if (block.HasValue)
                        {
                            if (PaintedCubeSpace.isSolidType((byte)block))
                            {
                               result.AddLast(new BlockLoc(x, y, z));
                            }
                        }
                    }
                }
            }
            return result;
        }

        public LinkedList<BlockLoc> getSurfaceBlocksBoundBy(BlockLoc loc1, BlockLoc loc2)
        {
            BlockLoc min = new BlockLoc((int)Math.Min(loc1.WSX(), loc2.WSX()), (int)Math.Min(loc1.WSY(), loc2.WSY()), (int)Math.Min(loc1.WSZ(), loc2.WSZ()));
            BlockLoc max = new BlockLoc((int)Math.Max(loc1.WSX(), loc2.WSX()), (int)Math.Max(loc1.WSY(), loc2.WSY()), (int)Math.Max(loc1.WSZ(), loc2.WSZ()));

            LinkedList<BlockLoc> result = new LinkedList<BlockLoc>();

            Island relevant = getIslandManager().getClosestIslandToLocation(loc2.toWorldSpaceVector3());

            BlockLoc aboveBlockLoc = new BlockLoc();
            BlockLoc blockAtLoc = new BlockLoc();

            for (int x = min.WSX(); x <= max.WSX(); x++)
            {
                for (int y = min.WSY(); y <= max.WSY(); y++)
                {
                    for (int z = min.WSZ(); z <= max.WSZ(); z++)
                    {
                        blockAtLoc.setValuesInWorldSpace(x, y, z);
                        byte? block = islandManager.getBlockAtOnGivenIsland(ref blockAtLoc, relevant);
                        if (block.HasValue)
                        {
                            if (PaintedCubeSpace.isSolidType((byte)block))
                            {
                                aboveBlockLoc.setValuesInWorldSpace(x, y+1, z);
                                byte? aboveBlock = islandManager.getBlockAtOnGivenIsland(ref aboveBlockLoc, relevant);
                                if (aboveBlock.HasValue)
                                {
                                    if (!PaintedCubeSpace.isOpaqueType((byte)aboveBlock))
                                    {

                                        result.AddLast(new BlockLoc(x, y, z));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
           
            
        }

        public bool isSurfaceBlock(ref BlockLoc loc)
        {

            byte? block = islandManager.getBlockAt(ref loc);
            BlockLoc blockLocAbove = BlockLoc.AddIntVec3(loc, new IntVector3(0, 1, 0));
            byte? aboveBlock = islandManager.getBlockAt(ref blockLocAbove);
            if (block.HasValue && aboveBlock.HasValue)
            {
                if (PaintedCubeSpace.isOpaqueType((byte)block) && !PaintedCubeSpace.isOpaqueType((byte)aboveBlock))
                {
                    return true;
                }
            }
            return false;

        }
        
        public void handlePlayerFinishDrag(Vector3 addedFrom, IEnumerable<BlockLoc> blocksToAdd, PlayerAction.Dragging.DragType dragType)
        {
            islandManager.getClosestIslandToLocation(addedFrom).addPlayerDraggedJobsiteWithBlocks(blocksToAdd, dragType);
        }

        public void placeWoodBlockPlanAlongRay(Ray placeWoodBlockClickRay)
        {
            Island island = islandManager.getClosestIslandToLocation(placeWoodBlockClickRay.Position);
            island.placeWoodBlockPlan(placeWoodBlockClickRay);
        }

        public void removeWoodBlockPlanAlongRay(Ray removeWoodBlockClickRay)
        {
            Island island = islandManager.getClosestIslandToLocation(removeWoodBlockClickRay.Position);
            island.removeWoodBlockPlan(removeWoodBlockClickRay);
        }

        public void placeBlockAlongRay(Ray placementRay, byte typeToPlace)
        {
            Island island = islandManager.getClosestIslandToLocation(placementRay.Position);
            island.placeBlockAlongRay(placementRay, typeToPlace);
        }

        public void destroyBlockAlongRay(Ray destructionRay)
        {
            Island island = islandManager.getClosestIslandToLocation(destructionRay.Position);
            island.destroyBlockAlongRay(destructionRay);
        }

        public void addBoatAt(Vector3 nLoc)
        {
            actorManager.addBoatAt(nLoc);
        }

    }
}
