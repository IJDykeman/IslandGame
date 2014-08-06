using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using CubeAnimator;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class ResourceBlockJobSite : JobSite
    {
        Dictionary<BlockLoc, ResourceBlock> resourceBlocks;
        List<Stockpile> stockpiles;


        public ResourceBlockJobSite(IslandPathingProfile nprofile)
        {
            resourceBlocks = new Dictionary<BlockLoc, ResourceBlock>();
            stockpiles = new List<Stockpile>();
            profile = nprofile;

        }

        public override float? intersects(Ray ray)
        {
            //return Intersection.intersects(ray, resourceBlocks.Keys);
            Intersectable intersected = Intersection.getNearestIntersectableAlongRay(ray, stockpiles);
            if (intersected != null)
            {
                return intersected.intersects(ray);
            }
            return null;
        }

        public override Job getJob(Character newWorker, Ray ray, IslandWorkingProfile workingProfile)
        {
            //return new CarryResourceToStockpileKickoffJob(getStockpileAlongRay(ray).getStoredType(),
            //    newWorker, new UnemployedJob(), workingProfile);
            return new UnemployedJob();
        }

        Stockpile getStockpileAlongRay(Ray ray)
        {
            return (Stockpile)Intersection.getNearestIntersectableAlongRay(ray, stockpiles);
        }



        public override void blockWasBuilt(BlockLoc toDestroy)
        {

        }


        public override void blockWasDestroyed(BlockLoc toDestroy)
        {

        }



        public override void draw(GraphicsDevice device, Effect effect, DisplayParameters parameters)
        {
            AnimatedBodyPartGroup wheatBale = new AnimatedBodyPartGroup(ContentDistributor.getEmptyString() + @"resources\wheatBale.chr", 1.0f / 7.0f);
            AnimatedBodyPartGroup log = new AnimatedBodyPartGroup(ContentDistributor.getEmptyString() + @"resources\log.chr", 1.0f / 7.0f);
            AnimatedBodyPartGroup standardBlock = new AnimatedBodyPartGroup(ContentDistributor.getEmptyString() + @"resources\standardBlock.chr", 1.0f / 7.0f);
            wheatBale.setScale(1f / 7f);
            log.setScale(1f / 7f);
            standardBlock.setScale(1f / 7f);

            foreach (BlockLoc key in resourceBlocks.Keys)
            {
                switch (resourceBlocks[key].getResourceType())
                {
                    case ResourceBlock.ResourceType.Wood:
                        log.setRootPartLocation(key.getMiddleInWorldSpace());
                        log.draw(device, effect);
                        break;
                    case ResourceBlock.ResourceType.Wheat:
                        wheatBale.setRootPartLocation(key.getMiddleInWorldSpace());
                        wheatBale.draw(device, effect);
                        break;

                    case ResourceBlock.ResourceType.Stone:
                        standardBlock.setRootPartLocation(key.getMiddleInWorldSpace());
                        standardBlock.draw(device, effect);
                        break;
                }

            }
            foreach (Stockpile stockpile in stockpiles)
            {
                stockpile.draw(device, effect, parameters);
            }




            
        }

        public void placeRescourceBlock(BlockLoc loc, ResourceBlock.ResourceType type)
        {
            if (!resourceBlocks.ContainsKey(loc))
            {
                resourceBlocks.Add(loc, new ResourceBlock(type));
            }
        }

        internal void removeRescourceBlock(BlockLoc blockLoc, ResourceBlock.ResourceType resourceType)
        {
            Debug.Assert(resourceBlocks[blockLoc].getResourceType() == resourceType);
            resourceBlocks.Remove(blockLoc);
        }

        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc test in resourceBlocks.Keys)
            {
                result.Add(test);
            }
            foreach (Stockpile test in stockpiles)
            {
                foreach (BlockLoc loc in test.getAllBlocksInStockPile())
                {
                    //if (!result.Contains(loc))
                    //{

                        result.Add(loc);
                    //}
                }
            }
            return result;
        }

        public void addStockpile(Stockpile stockpileJobSite)
        {
            stockpiles.Add(stockpileJobSite);
        }

        public IEnumerable<BlockLoc> getBlocksToStoreThisTypeIn(ResourceBlock.ResourceType carriedType)
        {
            List<BlockLoc> result = new List<BlockLoc>();
            foreach (Stockpile stockpile in stockpiles)
            {
                if (stockpile.getStoredType() == carriedType)
                {
                    foreach (BlockLoc loc in stockpile.getAllBlocksInStockPile())
                    {
                        if (thereAreNoResourcesAt(loc))
                        {
                            result.Add(loc);
                        }
                    }

                }
            }

            return result;
        }

        private bool thereAreNoResourcesAt(BlockLoc loc)
        {
            foreach (Stockpile test in stockpiles)
            {
                if (resourceBlocks.Keys.Contains(loc))
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<BlockLoc> getBlocksToGetThisTypeFrom(ResourceBlock.ResourceType typeToFetch)
        {

            List<BlockLoc> result = new List<BlockLoc>();

            foreach (BlockLoc storageLocation in resourceBlocks.Keys)
            {
                if (resourceBlocks[storageLocation].getResourceType() == typeToFetch)
                {
                    result.Add(storageLocation);
                }
            }

            return result;

        }

        public bool isSolidAt(BlockLoc loc)
        {
            return resourceBlocks.ContainsKey(loc);
        }


        internal void debitResource(int cost, ResourceBlock.ResourceType resourceType)
        {
            HashSet<BlockLoc> toRemove = getThisManyResourceBlocksOfType(cost, resourceType);
            foreach (BlockLoc test in toRemove)
            {
                resourceBlocks.Remove(test);
            }
        }

        internal HashSet<BlockLoc> getThisManyResourceBlocksOfType(int cost, ResourceBlock.ResourceType resourceType)
        {
            HashSet<BlockLoc> resourceBlocksToRemove = new HashSet<BlockLoc>();
            int blocksRemovedSoFar = 0;
            for (int i = 0; i < 3; i++)//repeats so it can remove stacked blocks
            {
                foreach (BlockLoc test in resourceBlocks.Keys)
                {
                    if (resourceBlocks[test].getResourceType() == resourceType)
                    {
                        BlockLoc blockAbove = (BlockLoc.AddIntVec3(test, new IntVector3(0, 1, 0)));
                        if (!resourceBlocks.ContainsKey(blockAbove))
                        {
                            if (!resourceBlocksToRemove.Contains(test))
                            {
                                blocksRemovedSoFar++;
                                resourceBlocksToRemove.Add(test);
                                if (blocksRemovedSoFar == cost)
                                {
                                    return resourceBlocksToRemove;
                                }
                            }

                        }
                    }
                }
            }
            return resourceBlocksToRemove;
        }

        public override bool respondToDeleteClickAndReturnIfShouldBeDeleted(Ray ray)
        {
            Stockpile stockPileAlongRay = (Stockpile)Intersection.getNearestIntersectableAlongRay(ray, stockpiles);
            stockpiles.Remove(stockPileAlongRay);
            return false;
        }


    }
}
