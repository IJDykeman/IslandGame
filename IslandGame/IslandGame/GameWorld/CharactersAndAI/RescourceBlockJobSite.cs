using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    public class ResourceBlockjobSite : JobSite
    {
        Dictionary<BlockLoc, ResourceBlock> resourceBlocks;
        List<Stockpile> stockpiles;


        public ResourceBlockjobSite(IslandPathingProfile nprofile)
        {
            resourceBlocks = new Dictionary<BlockLoc, ResourceBlock>();
            stockpiles = new List<Stockpile>();
            profile = nprofile;

        }

        public override float? intersects(Ray ray)
        {
            return Intersection.getDistanceToNearestIntersectableOnRay(ray, stockpiles);
        }

        public override Job getJob(Character newWorker, Ray ray, IslandWorkingProfile workingProfile)
        {
            return new CarryResourceToStockpileJob(getStockpileAlongRay(ray).getStoredType(),
                newWorker, new UnemployedJob(), workingProfile);
            //return new UnemployedJob();
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



        public override void draw(GraphicsDevice device, Effect effect)
        {
            foreach (BlockLoc key in resourceBlocks.Keys)
            {
                string resourceBlockPath = @"resources\log.chr";
                switch (resourceBlocks[key].getResourceType())
                {
                    case ResourceBlock.ResourceType.Wood:
                        resourceBlockPath = @"resources\log.chr";
                        break;
                    case ResourceBlock.ResourceType.Wheat:
                        resourceBlockPath = @"resources\wheatBale.chr";
                        break;

                    case ResourceBlock.ResourceType.Stone: 
                        resourceBlockPath = @"resources\log.chr";
                        break;
                }

                WorldMarkupHandler.addFlagPathWithPosition(ContentDistributor.getRootPath() + resourceBlockPath,
                                           key.getMiddleInWorldSpace(), 1.0f/7.0f);
            }

            foreach (Stockpile stockpile in stockpiles)
            {
                stockpile.draw(device, effect);
            }
        }

        public void placeRescourceBlock(BlockLoc loc, ResourceBlock.ResourceType type)
        {
            resourceBlocks.Add(loc, new ResourceBlock(type));
        }

        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc test in resourceBlocks.Keys)
            {
                result.Add(test);
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
    }
}
