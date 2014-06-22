using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using IslandGame.GameWorld.CharactersAndAI;

namespace IslandGame.GameWorld
{
    [Serializable] 
    public class Island
    {

        ChunkSpace chunkSpace;
        SetPieceManager setPieceManager;
        JobSiteManager jobSiteManager;
        IslandGenerator gen;
        int mipLevel = 0;

        IslandGenerator[][] generators;
        TerrainType terrainDifficulty;


        public bool hasBeenGenerated = false;



        public enum TerrainType
        {
            easy,
            medium,
            hard,
            any,
            PineForest,
            PoplarForest,
            Plains,
            SmoothWithBluffs
        }
        

        public Island(Vector3 loc, TerrainType nTerrainDifficulty)
        {
            generators = new IslandGenerator[Enum.GetValues(typeof(Island.TerrainType)).Length][];
            generators[(int)TerrainType.easy] = new IslandGenerator[]{ 
                new SmoothWithBluffsGenerator(),new PoplarForestGenerator()};

            generators[(int)TerrainType.medium] = new IslandGenerator[]{ 
                new HillyGenerator(), new PlainsGenerator(), new PineForestGenerator()};

            generators[(int)TerrainType.hard] = new IslandGenerator[]{ 
                new Volcanic()};
            generators[(int)TerrainType.any] = new IslandGenerator[]{ 
                new SmoothWithBluffsGenerator(),new PoplarForestGenerator(), 
                new HillyGenerator(), new PlainsGenerator(), new PineForestGenerator(),
                new Volcanic()};
            generators[(int)TerrainType.PineForest] = new IslandGenerator[]{  new PineForestGenerator()};
            generators[(int)TerrainType.PoplarForest] = new IslandGenerator[] { new PoplarForestGenerator() };
            generators[(int)TerrainType.Plains] = new IslandGenerator[] { new PlainsGenerator() };
            generators[(int)TerrainType.SmoothWithBluffs] = new IslandGenerator[] { new SmoothWithBluffsGenerator() };


            terrainDifficulty = nTerrainDifficulty;

            Random rand = new Random();
            chunkSpace = new ChunkSpace(loc);
            setPieceManager = new SetPieceManager();
            jobSiteManager = new JobSiteManager(getPathingProfile());
        }

        public void generateIsland()
        {




            gen = generators[(int)terrainDifficulty][new Random().Next(generators[(int)terrainDifficulty].Length)];
                //gen = new SmoothWithBluffsGenerator();////!
                if (Player.galleryMode)
                {
                    bool timeToRegen = IslandGeneratorLoader.updateAndReturnIfNewCodeIsReady();
                    if (timeToRegen)
                    {
                        gen = IslandGeneratorLoader.getGenerator();
                    }
                }

            gen.generateIsland(chunkSpace, setPieceManager, jobSiteManager, getLocationProfile());
        }

        public void generateWithGenerator(IslandGenerator generator)
        {
            setPieceManager = new SetPieceManager();
            generator.generateIsland(chunkSpace, setPieceManager, jobSiteManager, getLocationProfile());
            chunkSpace.forceUpdateAllMeshes();
        }


        public void updateMeshes()
        {
            chunkSpace.updateAllMeshes(mipLevel);
            jobSiteManager.updateAllMeshes(mipLevel);
        }

        public Vector3 getCenterAtYZero()
        {
            Vector3 result = chunkSpace.getCenter();
            result.Y = 0;
            return result;
        }

        public void setMipLevel(int level)
        {
            mipLevel = level;
        }

        public void display(GraphicsDevice device, Effect effect)
        {
            

            chunkSpace.display(device, effect);
            setPieceManager.display(device, effect);
            jobSiteManager.display(device, effect, getLocationProfile());
        }

        public void runPreDrawCalculations()
        {
            jobSiteManager.runPreDrawCalculations();
        }

        public void update()
        {
            jobSiteManager.update();
        }

        //TODO split this into different class
        public AxisAlignedBoundingBox AABBPhysics(AxisAlignedBoundingBox currentAABB, AxisAlignedBoundingBox newAABB)
        {
            float fudge = .001f;
            currentAABB.loc = worldSpaceToIslandSpaceForPhysics(currentAABB.loc);
            newAABB.loc = worldSpaceToIslandSpaceForPhysics(newAABB.loc);
            //checking y
            for (int x = (int)(currentAABB.loc.X); x <= (int)(currentAABB.loc.X + currentAABB.Xwidth); x++)
            {
                for (int z = (int)(currentAABB.loc.Z); z <= (int)(currentAABB.loc.Z + currentAABB.Zwidth); z++)
                {
                    for (int y = (int)(newAABB.loc.Y); y <= (int)(newAABB.loc.Y + newAABB.height); y++)
                    {
                        if (isIslandSolidInIslandSpace(x, y, z))
                        {
                            if (newAABB.loc.Y < currentAABB.loc.Y)
                            { // trying to move down
                                newAABB.loc.Y = y + 1 + fudge;
                            }
                            else if (newAABB.loc.Y > currentAABB.loc.Y)
                            { // trying to move up
                                newAABB.loc.Y = (int)y - fudge - newAABB.height;

                            }
                        }
                    }
                }
            }

            //now testing z
            for (int x = (int)(currentAABB.loc.X); x <= (int)(currentAABB.loc.X + newAABB.Xwidth); x++)
            {
                for (int y = (int)(currentAABB.loc.Y); y <= (int)(currentAABB.loc.Y + currentAABB.height); y++)
                {
                    //int z = (int)(newLoc.Z - AABBHalfWidth);
                    for (int z = (int)(newAABB.loc.Z); z <= (int)(newAABB.loc.Z + newAABB.Zwidth); z++)
                    {



                        if (isIslandSolidInIslandSpace(x, y, z))
                        {


                                if (newAABB.loc.Z < currentAABB.loc.Z)
                                { // trying to move down
                                    newAABB.loc.Z = (int)z + 1 + fudge;
                                    //finalLoc.Y = locInPath.Y+5;
                                }


                                else if (newAABB.loc.Z > currentAABB.loc.Z)
                                { // trying to move up
                                    newAABB.loc.Z = (int)z - newAABB.Zwidth - fudge;

                                }

                            

                            newAABB.loc.Z = currentAABB.loc.Z;
                        }


                    }
                }
            }

            //now testing X



            for (int z = (int)(currentAABB.loc.Z); z <= (int)(currentAABB.loc.Z + currentAABB.Zwidth); z++)
            {
                for (int y = (int)(currentAABB.loc.Y); y <= (int)(currentAABB.loc.Y + currentAABB.height); y++)
                {
                    for (int x = (int)(newAABB.loc.X); x <= (int)(newAABB.loc.X + newAABB.Xwidth); x++)
                    {



                        if (isIslandSolidInIslandSpace(x, y, z))
                        {
                            if (newAABB.loc.X < currentAABB.loc.X)
                            { // trying to move down in X
                                newAABB.loc.X = (int)x + 1 + fudge;
                            }

                        }

                        /////////////////////////////////////////////////////////////////////
                        x = (int)(newAABB.loc.X + newAABB.Xwidth);



                        if (isIslandSolidInIslandSpace(x, y, z))
                        {
                            if (newAABB.loc.X > currentAABB.loc.X)
                            { // trying to move up in X
                                newAABB.loc.X = (int)x - newAABB.Xwidth - fudge;

                            }

                        }
                    }
                }

            }

            newAABB.loc = islandSpaceToWorldSpaceForPhysics(newAABB.loc);
            return newAABB;
        }

        public BoundingBox getBoundingBox()
        {
            return chunkSpace.getBoundingBox();
        }

        public bool boundingBoxContaintPoint(Vector3 test)
        {
            ContainmentType pointInTestIsland;
            getBoundingBox().Contains(ref test, out pointInTestIsland);
            return pointInTestIsland == ContainmentType.Contains;

        }

        public float? boundingBoxIntersectsRay(Ray ray)
        {
            return ray.Intersects(getBoundingBox());
        }

        public Vector3? getNearestBlockAlongRayInAndFromWorldSpace(Ray ray)
        {
            Ray rayInChunkSpaceSpace = new Ray(ray.Position, ray.Direction);
            

            rayInChunkSpaceSpace.Position = chunkSpace.worldSpaceToChunkSpaceSpace(ray.Position);

            Vector3? result = chunkSpace.getNearestBlockAlongRayInChunkSpaceSpace(rayInChunkSpaceSpace);

            if (result.HasValue)
            {
                result = chunkSpace.chunkSpaceToWorldSpace((Vector3)result);
            }


            return result;
        }

        public Vector3? getLastSpaceAlongRayInAndFromWorldSpace(Ray ray)
        {
            Ray rayInChunkSpaceSpace = new Ray(ray.Position, ray.Direction);
            ray.Direction.Normalize();

            rayInChunkSpaceSpace.Position = chunkSpace.worldSpaceToChunkSpaceSpace(ray.Position);

            Vector3? result = chunkSpace.getNearestBlockAlongRayInChunkSpaceSpace(rayInChunkSpaceSpace);

            if (result.HasValue)
            {
                result = chunkSpace.chunkSpaceToWorldSpace((Vector3)result);
                Vector3 intersectedBlockLoc = new Vector3((int)((Vector3)result).X, (int)((Vector3)result).Y, (int)((Vector3)result).Z);
                float? intersection = ray.Intersects(new BoundingBox(intersectedBlockLoc,intersectedBlockLoc+ new Vector3(1,1,1)));
                ray.Direction.Normalize();
                if (intersection.HasValue)
                {
                    return ray.Direction * ((float)intersection - .001f) + ray.Position;
                }
            }


            return result;
        }

        public Vector3? getLastSpaceAlongRayConsideringBuildSite(Ray ray){
            return jobSiteManager.getLastSpaceAlongRayConsideringBuildSite(ray, getLastSpaceAlongRayInAndFromWorldSpace(ray));
        }


        public byte? getBlockAt(ref BlockLoc loc)
        {
            return chunkSpace.getBlockAt(ref loc, new IslandPathingProfile(this));
        }

        public void placeExcavationMark(Ray ray)
        {
            Vector3? blockVec3 = getNearestBlockAlongRayInAndFromWorldSpace(ray);
            if (!blockVec3.HasValue)
            {
                return;
            }

            BlockLoc blockLoc = new BlockLoc((Vector3)blockVec3);
            if (chunkSpace.isChunkSpaceSolidAt(blockLoc, new IslandPathingProfile(this)))
            {
                jobSiteManager.addExcavationMark(blockLoc, getPathingProfile());
            }
        }

        public IslandPathingProfile getPathingProfile()
        {
            return new IslandPathingProfile(this);
        }

        public IslandLocationProfile getLocationProfile()
        {
            return new IslandLocationProfile(this);
        }

        private Vector3 worldSpaceToIslandSpaceForPhysics(Vector3 loc)
        {
            return chunkSpace.worldSpaceToChunkSpaceSpace(loc);
        }

        private Vector3 islandSpaceToWorldSpaceForPhysics(Vector3 loc)
        {
            return chunkSpace.chunkSpaceToWorldSpace(loc);
        }

        public void destroyBlock(BlockLoc toDestroy)
        {
            chunkSpace.setBlockAtWithMeshUpdate(PaintedCubeSpace.AIR, toDestroy.toISIntVec3(getPathingProfile()));
            jobSiteManager.blockWasDestroyed(toDestroy);
            setPieceManager.blockWasDestroyed(toDestroy);
        }

        public JobSite getJobSiteAlongRay(Ray ray)
        {
            return jobSiteManager.getJobSiteAlongRay(ray);
        }

        public void addPlayerDraggedJobsiteWithBlocks(IEnumerable<BlockLoc> blocksToAdd, PlayerAction.Dragging.DragType dragType)
        {
            jobSiteManager.addPlayerDraggedJobsiteWithBlocks(blocksToAdd, getPathingProfile(), dragType);
        }

        public void placeWoodBlockPlan(Ray placeWoodBlockClickRay)
        {
            jobSiteManager.placeWoodBlockPlanAlongRay(placeWoodBlockClickRay, getLastSpaceAlongRayInAndFromWorldSpace(placeWoodBlockClickRay), getPathingProfile());
        }

        public void buildBlock(BlockLoc blockLoc, byte typeToBuild)
        {
            chunkSpace.setBlockAtWithMeshUpdate(typeToBuild, blockLoc.toISIntVec3(getPathingProfile()));
            jobSiteManager.blockWasBuilt(blockLoc);
        }

        public void removeWoodBlockPlan(Ray removeWoodBlockClickRay)
        {
            jobSiteManager.removeWoodBlockPlanAlongRay(removeWoodBlockClickRay, getNearestBlockAlongRayInAndFromWorldSpace(removeWoodBlockClickRay), getPathingProfile());

        }

        public void placeBlockAlongRay(Ray placementRay, byte typeToPlace)
        {
            Vector3? hit = getLastSpaceAlongRayInAndFromWorldSpace(placementRay);
            if (hit.HasValue)
            {
                buildBlock(new BlockLoc((Vector3)hit), typeToPlace);
            }
        }

        public void destroyBlockAlongRay(Ray destructionRay)
        {
            Vector3? hit = getNearestBlockAlongRayInAndFromWorldSpace(destructionRay);
            if (hit.HasValue)
            {
                destroyBlock(new BlockLoc((Vector3)hit));
            }
        }

        public bool vehiclePlacedHereCouldBeBoarded(BlockLoc vehicleLoc)
        {
            IslandPathingProfile profile = getPathingProfile();
            if (profile.getFootLocsThatHaveAccessToBlock(vehicleLoc).Count != 0)
            {
                if (profile.getStandableFootLocsThatHaveAccessToBlock(vehicleLoc, 2).Count != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void addJobSite(CharactersAndAI.BoatBuildSite boatBuildSite)
        {
            jobSiteManager.addJobSite(boatBuildSite);
        }

        public void acceptWorkStrike(ActorStrikeAction actorStrikeAction)
        {
            if (actorStrikeAction.getStrikeType() == ActorStrikeAction.StrikeType.OnBlock)
            {
                ActorStrikeBlockAction strikeBlock = (ActorStrikeBlockAction)actorStrikeAction;
                handleStrikeEffectOnIsland(strikeBlock);
                jobSiteManager.acceptStrikeAt(strikeBlock.getStrikeTarget(), strikeBlock.getJobType());
            }
            else 

                //NEEDS DISTANCE LIMITTING
            {
                ActorStrikeAlongRayAction rayStrike = (ActorStrikeAlongRayAction)actorStrikeAction;
                Ray ray = new Ray(rayStrike.getStrikeOrigen(), rayStrike.getStrikeDirectionNormal());
                JobSite intersectedSite = getJobSiteAlongRay(ray);
                Vector3? blockClicked = getNearestBlockAlongRayInAndFromWorldSpace(ray);
                if (blockClicked != null && intersectedSite != null)
                {
                    if (Math.Abs(Vector3.Distance((Vector3)blockClicked, rayStrike.getStrikeOrigen()) - (float)intersectedSite.intersects(ray))<.01)
                    {
                        //block has been clicked
                        handleRayStrikeOnIslandBlocks(rayStrike, ray);
                    }
                    else if (Vector3.Distance((Vector3)blockClicked, rayStrike.getStrikeOrigen()) < (float)intersectedSite.intersects(ray))
                    {
                        //block has been clicked
                        handleRayStrikeOnIslandBlocks(rayStrike, ray);
                    }
                    else
                    {
                        //siteHasBeenClicked
                        ray = JobSiteWasHitWithJobRay(rayStrike, ray, intersectedSite);
                    }
                }
                else if (blockClicked != null)
                {
                    //block has been clicked
                    handleRayStrikeOnIslandBlocks(rayStrike, ray);
                }
                else if (intersectedSite != null)
                {
                    //site has been clicked
                    ray = JobSiteWasHitWithJobRay(rayStrike, ray, intersectedSite);
                }
            }
        }

        private void handleRayStrikeOnIslandBlocks(ActorStrikeAlongRayAction rayStrike, Ray ray)
        {
            switch (rayStrike.getJobType())
            {
                case JobType.mining:
                    destroyBlockAlongRay(ray);
                    break;
                case JobType.building:
                    buildBlock(new BlockLoc((Vector3)getLastSpaceAlongRayInAndFromWorldSpace(ray)), 5);
                    break;
            }
        }

        private Ray JobSiteWasHitWithJobRay(ActorStrikeAlongRayAction rayStrike, Ray ray, JobSite intersectedSite)
        {
            BlockLoc locOfWork = new BlockLoc(rayStrike.getStrikeOrigen()
                + rayStrike.getStrikeDirectionNormal() * ((float)intersectedSite.intersects(ray) + .001f));
            jobSiteManager.acceptStrikeAt(locOfWork, rayStrike.getJobType());
            return ray;
        }

        private void handleStrikeEffectOnIsland(ActorStrikeBlockAction strikeBlock)
        {
            switch (strikeBlock.getJobType())
            {
                case JobType.mining:
                    destroyBlock(strikeBlock.getStrikeTarget());
                    break;
            }
        }

        public void addResourceBlock(BlockLoc loc, ResourceBlock.ResourceType type)
        {
            jobSiteManager.addResourceBlock(loc, type);
        }

        public JobSiteManager getJobSiteManager()
        {
            return jobSiteManager;
        }

        internal Vector3 chunkSpaceToWorldSpace(Vector3 loc)
        {
            return chunkSpace.chunkSpaceToWorldSpace(loc);
        }

        internal Vector3 worldSpaceToChunkSpaceSpace(Vector3 loc)
        {
            return chunkSpace.worldSpaceToChunkSpaceSpace(loc);
        }

        internal bool isChunkSpaceSolidAt(BlockLoc loc)
        {
            return chunkSpace.isChunkSpaceSolidAt(loc, new IslandPathingProfile(this)) || jobSiteManager.isSolidAt(loc);
        }

        internal bool isIslandSolidInIslandSpace(int x, int y, int z)
        {
            return chunkSpace.isChunkSpaceSolidAt(x,y,z) || jobSiteManager.isSolidAt(new BlockLoc(new Vector3(x,y,z)+getLocation()));
        }

        internal Vector3 getLocation()
        {
            return chunkSpace.getLocation();
        }

        internal bool withinChunkSpaceInChunkSpace(int x, int y, int z)
        {
            return chunkSpace.withinChunkSpaceInChunkSpace(x, y, z);
        }

        public bool couldAffordResourceExpendeture(int cost, ResourceBlock.ResourceType resourceType)
        {
            return jobSiteManager.couldAffordResourceExpendeture(cost, resourceType);
        }

        public void debitResource(int cost, ResourceBlock.ResourceType resourceType)
        {
            jobSiteManager.debitResource(cost, resourceType);
        }
    }
}
