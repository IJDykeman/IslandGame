using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class IslandManager
    {
        public List<Island> islands;

        [NonSerialized] Thread generationThread;
        [NonSerialized] Thread meshingThread;
        List<Island> generationQueue;




        public IslandManager()
        {
            islands = new List<Island>();
            generationQueue = new List<Island>();


            Random rand = new Random();


            if (!Player.galleryMode)
            {
                float islandLocationScalar = 5.0f;

                generateAndAddIslandToList(new Island(new Vector3(370, 0, 267) * islandLocationScalar, Island.TerrainType.PoplarForest));

                Vector3[] islandLocations = new Vector3[] {
                    
                    new Vector3(370, 0, 90),
                    new Vector3(520, 0, 140),
                    //new Vector3(600, 0, 270),
                    //new Vector3(520, 0, 450),
                    new Vector3(380, 0, 490),
                    //new Vector3(230, 0, 440),
                    new Vector3(150, 0, 300),
                    //new Vector3(192, 0, 170)
                                           };


                
                for (int i=0; i<islandLocations.Length; i++)
                {
                    Array values = Enum.GetValues(typeof(Island.TerrainType));
                    Random random = new Random();
                    Island.TerrainType terrainDifficultyToUse = Island.TerrainType.medium;
                    //(Island.TerrainType)values.GetValue(random.Next(values.Length));

                    Vector3 location = islandLocations[i];

                    switch (i)
                    {
                        case 0:
                            terrainDifficultyToUse = Island.TerrainType.hard;
                            break;
                        case 1:
                            terrainDifficultyToUse = Island.TerrainType.PineForest;
                            break;
                        case 2:
                            terrainDifficultyToUse = Island.TerrainType.SmoothWithBluffs;
                            break;
                        case 3:
                            terrainDifficultyToUse = Island.TerrainType.Plains;
                            break;
                        case 4:
                            terrainDifficultyToUse = Island.TerrainType.hard;
                            break;
                        case 5:
                            break;
                    }
                        
                    


                    generationQueue.Add(new Island(location*islandLocationScalar, terrainDifficultyToUse));
                }



            }
            else
            {
                generationQueue.Add(new Island(new Vector3(0, 0, 0), Island.TerrainType.easy));
            }

            setUpThreads();
        }

        public void setUpAfterGameLoad()
        {
            setUpThreads();
        }

        private void setUpThreads()
        {
            generationThread = new Thread(new ThreadStart(() => generationThreadTask()));
            generationThread.IsBackground = false;
            generationThread.Priority = ThreadPriority.Highest;
            generationThread.Start();

            meshingThread = new Thread(new ThreadStart(() => meshingThreadTask()));
            meshingThread.IsBackground = false;
            meshingThread.Priority = ThreadPriority.Highest;
            meshingThread.Start();
        }


        public Island getClosestIslandToLocation(ref Vector3 location)
        {
            if (islands.Count > 0)
            {
                int shortestDistance = Int32.MaxValue;
                Island result = islands[0];
                foreach (Island island in islands)
                {
                    int distance = (int)Vector3.Distance(location, island.getCenterAtYZero());
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        result = island;
                    }
                }
                return result;
            }
            return null;

        }

        public Island getClosestIslandToLocation(Vector3 location)
        {
            return getClosestIslandToLocation(ref location);
        }

        public void generationThreadTask()
        {
            while (true)
            {
                lock (generationQueue)
                {

                    if (Player.galleryMode)
                    {
                        generationQueue.Clear();
                        bool timeToRegen = IslandGeneratorLoader.updateAndReturnIfNewCodeIsReady();
                        if (timeToRegen)
                        {
                            lock (islands)
                            {
                                islands.Clear();
                            }
                            Island island = new Island(new Vector3(), Island.TerrainType.easy);
                            island.generateWithGenerator(IslandGeneratorLoader.getGenerator());
                            lock (islands)
                            {
                                addIsland(island);
                            }
                        }
                    }


                    foreach (Island toUpdate in generationQueue)
                    {
                        if (!toUpdate.hasBeenGenerated)
                        {
                            generateAndAddIslandToList(toUpdate);
                        }



                    }
                    generationQueue.Clear();


                }
                Thread.Sleep(1);
            }
        }

        private void generateAndAddIslandToList(Island toUpdate)
        {
            toUpdate.generateIsland();
            lock (islands)
            {
                addIsland(toUpdate);
            }
            toUpdate.hasBeenGenerated = true;
        }

        private void addIsland(Island toUpdate)
        {
            islands.Add(toUpdate);

        }


        private void meshingThreadTask()
        {
            while (true)
            {
                List<Island> tempIslands = new List<Island>();
                lock (islands)
                {

                    tempIslands.AddRange(islands);
                }


                    foreach (Island test in tempIslands)
                    {

                        test.updateMeshes();
                    }
                }
            
        }

        public void makeNewIsland(Vector2 location)
        {

            //TODO parametrize island aspects
            generationQueue.Add(new Island(new Vector3(location.X, 0, location.Y), Island.TerrainType.easy));

        }

        public void display(GraphicsDevice device, Effect effect, BoundingFrustum frustrum)
        {
            lock (islands)
            {
                
                foreach (Island island in islands)
                {
                    if (frustrum.Intersects(island.getBoundingBox()))
                    {
                        island.display(device, effect);
                    }
                }
            }
        }

        public void update()
        {
            lock (islands)
            {
                foreach (Island island in islands)
                {
                    island.update();
                }
            }


        }


        public void mipIslandToLevel(Island island, int level)
        {
            island.setMipLevel(level);
        }

        public void setIslandMipsWithCameraLocation(Vector3 cameraLoc)
        {


                lock (islands)
                {
                    foreach (Island test in islands)
                    {
                        int mipLevel = getMipLevelAtDistance((int)Vector3.Distance(cameraLoc, test.getCenterAtYZero()));
                        mipLevel = (int)MathHelper.Clamp(mipLevel, 0, 4);
                        test.setMipLevel(mipLevel);

                    }
                
            }
            
        }

        int getMipLevelAtDistance(int distance)
        {
            return distance / 400;
        }

        public Vector3? getNearestBlockHit(Ray ray)
        {
            Island nearest = getIslandIntersectedByRay(ray);
            if (nearest == null)
            {
                return null;
            }

            return nearest.getNearestBlockAlongRayInAndFromWorldSpace(ray);
        }

        Island getIslandIntersectedByRay(Ray ray)
        {
            Island nearest = null;
            float minDist = float.MaxValue;

            foreach (Island test in islands)
            {
                float? distToStrike = test.boundingBoxIntersectsRay(ray);
                if (distToStrike.HasValue)
                {
                    if (test.boundingBoxContaintPoint(ray.Position))
                    {
                        nearest = test;
                        break;
                    }

                    if ((float)distToStrike < minDist)
                    {
                        nearest = test;
                    }
                }
            }
            return nearest;
        }

        internal Vector3? getOceanIntersectionAtY1(Ray ray)
        {
            ray.Direction.Normalize();
            Vector3? SolidBlockHit = getNearestBlockHit(ray);

            Vector3 rayPosOnYOceanLevel = ray.Position;
            rayPosOnYOceanLevel.Y=.7f;

            BoundingBox oceanBoundingBox = new BoundingBox(rayPosOnYOceanLevel + new Vector3(-10000, -1, -1000), rayPosOnYOceanLevel + new Vector3(10000, 0, 1000));
            float? oceanIntersectionDist = oceanBoundingBox.Intersects(ray);
            if (oceanIntersectionDist.HasValue)
            {
                Vector3 oceanIntersetion = ray.Position + ray.Direction * (float)oceanIntersectionDist;
                oceanIntersetion.Y = 1;
                if (SolidBlockHit.HasValue)
                {
                    if (Vector3.Distance((Vector3)SolidBlockHit, ray.Position) < (float)oceanIntersectionDist)
                    {
                        return null;//land was hit before ocean

                    }
                    else
                    {
                        return oceanIntersetion;
                    }
                }
                else
                {
                    return oceanIntersetion;
                }
            }
            else
            {
                return null;
            }

        }


        internal JobSite getJobSiteAlongRay(Ray ray)
        {
            
            Island toTest = getIslandIntersectedByRay(ray);
            if (toTest == null)
            {
                return null;
            }
            return toTest.getJobSiteAlongRay(ray);
        }

        public byte? getBlockAt(ref BlockLoc loc)
        {
            Vector3 worldSpaceVec3 = loc.toWorldSpaceVector3();
            Island toTest = getClosestIslandToLocation(ref worldSpaceVec3);
            return toTest.getBlockAt(ref loc);
        }

        public byte? getBlockAtOnGivenIsland(ref BlockLoc loc, Island toTest)
        {
            return toTest.getBlockAt(ref loc);
        }

        public void buildBlockAt(BlockLoc blockLoc, byte typeToBuild)
        {
            Island relevantIsland = getClosestIslandToLocation(blockLoc.toWorldSpaceVector3());

            relevantIsland.buildBlock(blockLoc,typeToBuild);
        }


        public bool vehiclePlacedHereCouldBeBoarded(BlockLoc vehicleLoc)
        {
            return getClosestIslandToLocation(vehicleLoc.toWorldSpaceVector3()).vehiclePlacedHereCouldBeBoarded(vehicleLoc);
        }

        public void acceptWorkStrike(ActorStrikeAction actorStrikeAction)
        {
            Island relevantIsland = getClosestIslandToLocation(actorStrikeAction.getStriker().getLocation());
            relevantIsland.acceptWorkStrike(actorStrikeAction);

        }
    }
}
