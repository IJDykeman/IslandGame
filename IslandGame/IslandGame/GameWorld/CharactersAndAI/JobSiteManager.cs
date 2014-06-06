using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;
using IslandGame.GameWorld.CharactersAndAI;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class JobSiteManager
    {

        List<JobSite> jobSites;
        ResourceBlockjobSite resourceBlockJobsite;


        public JobSiteManager(IslandPathingProfile profile)
        {
            jobSites = new List<JobSite>();
            jobSites.Add(new TreesJobSite(profile));
            resourceBlockJobsite = new ResourceBlockjobSite(profile);
            jobSites.Add(resourceBlockJobsite);
        }

        public void display(GraphicsDevice device, Effect effect, IslandLocationProfile locationProfile)
        {
            foreach (JobSite site in jobSites)
            {
                site.draw(device, effect);
            }

        }

        public void runPreDrawCalculations()
        {
            foreach (JobSite site in jobSites)
            {
                site.runPreDrawCalculations();
            }
        }

        public void update()
        {
            for (int i = jobSites.Count-1; i >=0; i--)
            {
                jobSites[i].update();
                if (jobSites[i].isComplete())
                {
                    jobSites.RemoveAt(i);
                }
            }

        }

        public void addExcavationMark(BlockLoc blockLoc, IslandPathingProfile profile)
        {
            if (allBlocksWithJobSites().Contains(blockLoc))
            {
                return;
            }

            bool hasAnExcavationSite = false;
            foreach (JobSite site in jobSites)
            {
                if (site is ExcavationSite)
                {
                    hasAnExcavationSite = true;
                    break;
                }
            }
            if (!hasAnExcavationSite)
            {
                jobSites.Add(new ExcavationSite(profile));
            }

            foreach (JobSite site in jobSites)
            {
                if (site is ExcavationSite)
                {
                    ((ExcavationSite)site).addBlockToDestroy(blockLoc);
                }
            }
        }

        internal void blockWasDestroyed(BlockLoc toDestroy)
        {
            foreach (JobSite site in jobSites)
            {
                site.blockWasDestroyed(toDestroy);
            }
        }

        internal JobSite getJobSiteAlongRay(Microsoft.Xna.Framework.Ray ray)
        {
            return (JobSite)Intersection.getNearestIntersectableAlongRay(ray, jobSites);
        }

        internal void addPlayerDraggedJobsiteWithBlocks(IEnumerable<BlockLoc> blocksToAdd, IslandPathingProfile profile, PlayerAction.Dragging.DragType dragType)
        {
            switch (dragType)
            {
                case PlayerAction.Dragging.DragType.farm:
                    placeFarmWithBlocks(blocksToAdd, profile);
                    break;
                case PlayerAction.Dragging.DragType.storage:
                    placeStorageAreaWithBlocksToPlaceOn(blocksToAdd, profile);
                    break;
            }
        }

        private void placeStorageAreaWithBlocksToPlaceOn(IEnumerable<BlockLoc> blocksToPlaceSiteOn, IslandPathingProfile profile)
        {

            List<BlockLoc> blocksForSite = new List<BlockLoc>();
            foreach (BlockLoc inGround in blocksToPlaceSiteOn)
            {
                blocksForSite.Add(BlockLoc.AddIntVec3(inGround, new IntVector3(0, 1, 0)));
                blocksForSite.Add(BlockLoc.AddIntVec3(inGround, new IntVector3(0, 2, 0)));
            }

            HashSet<BlockLoc> locsNotOfWork = new HashSet<BlockLoc>();
            removeAllBlocksAtOrBelowWorkBlock(blocksForSite, locsNotOfWork);
            if (locsNotOfWork.Count > 0)
            {
                HashSet<BlockLoc> locsNotSolid = new HashSet<BlockLoc>();
                foreach (BlockLoc test in locsNotOfWork)
                {
                    if (!profile.isProfileSolidAt(test))
                    {
                        locsNotSolid.Add(test);
                    }
                    
                }
                resourceBlockJobsite.addStockpile(new Stockpile(locsNotSolid,ResourceBlock.ResourceType.Wood));
                
            }

        }

        private void placeFarmWithBlocks(IEnumerable<BlockLoc> blocksToAdd, IslandPathingProfile profile)
        {
            HashSet<BlockLoc> locs = new HashSet<BlockLoc>();
            removeAllBlocksAtOrBelowWorkBlock(blocksToAdd, locs);

            if (locs.Count > 0)
            {
                jobSites.Add(new Farm(profile, locs));
            }
        }

        private void removeAllBlocksAtOrBelowWorkBlock(IEnumerable<BlockLoc> blocksToAdd, HashSet<BlockLoc> locs)
        {
            HashSet<BlockLoc> blocksToNotBeFarmed = removeAllBlocksOfWorkFrom(blocksToAdd, locs);
            foreach (BlockLoc alreadyOccupiedBlock in blocksToNotBeFarmed)
            {
                locs.Remove(BlockLoc.AddIntVec3(alreadyOccupiedBlock, new IntVector3(0, -1, 0)));
            }
        }

        private HashSet<BlockLoc> removeAllBlocksOfWorkFrom(IEnumerable<BlockLoc> blocksToAdd, HashSet<BlockLoc> locs)
        {
            HashSet<BlockLoc> blocksToNotBeFarmed = allBlocksWithJobSites();
            foreach (BlockLoc test in blocksToAdd)
            {
                locs.Add(test);
            }
            foreach (BlockLoc alreadyOccupiedBlock in getAllFarmBlocksfarmedOn())
            {
                locs.Remove(alreadyOccupiedBlock);
            }
            return blocksToNotBeFarmed;
        }

        private IEnumerable<Farm> getFarmEnumerable()
        {
            List<Farm> result = new List<Farm>();
            foreach (JobSite site in jobSites)
            {
                if (site is Farm)
                {
                    result.Add((Farm)site);
                }
            }
            return result;
        }

        private IEnumerable<WoodBuildSite> getWoodBuildSiteEnumerable()
        {
            List<WoodBuildSite> result = new List<WoodBuildSite>();
            foreach (JobSite site in jobSites)
            {
                if (site is WoodBuildSite)
                {
                    result.Add((WoodBuildSite)site);
                }
            }
            return result;
        }

        public HashSet<BlockLoc> getAllFarmBlocksFarmedIn()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            List<Farm> farms = getFarmEnumerable().ToList();
            foreach (Farm farm in farms)
            {
                List<BlockLoc> farmBlocks = farm.getBlocksToBeFarmedIn().ToList();
                foreach (BlockLoc occupiedBlock in farmBlocks)
                {
                    result.Add(occupiedBlock);
                }
            }
            return result;
        }

        public HashSet<BlockLoc> getAllFarmBlocksfarmedOn()
        {
            HashSet<BlockLoc> blocksFarmedIn = getAllFarmBlocksFarmedIn();
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc toLower in blocksFarmedIn)
            {
                result.Add(toLower.getVector3WithAddedIntvec(new IntVector3(0, -1, 0)));
            }
            return result;
        }

        public void makeFarmBlockGrow(BlockLoc blockLoc)
        {
            foreach (JobSite test in jobSites)
            {
                test.makeFarmBlockGrow(blockLoc);
            }
        }


        private void addBlockToBuildSite(WoodBuildSite toAddToPotentially, BlockLoc newBlockToPlace)
        {
            if (allBlocksWithJobSites().Contains(newBlockToPlace))
            {
                return;
            }
            toAddToPotentially.addBlock(newBlockToPlace);
        }

        public void blockWasBuilt(BlockLoc blockLoc)
        {
            foreach (JobSite site in jobSites)
            {
                site.blockWasBuilt(blockLoc);
            }
        }

        internal void removeWoodBlockPlanAlongRay(Ray removeWoodBlockClickRay, Vector3? exactBlockHitLocOnIsland, IslandPathingProfile profile)
        {


            IEnumerable<WoodBuildSite> woodBuildSites = getWoodBuildSiteEnumerable();
            removeWoodBlockClickRay.Direction.Normalize();

            if (woodBuildSites.Count() == 0)
            {
                return;
            }
            woodBuildSites = getWoodBuildSiteEnumerable();

            WoodBuildSite buildSite = null;

            foreach (WoodBuildSite toAddToPotentially in woodBuildSites)
            {
                buildSite = toAddToPotentially;//Assumes there is only one item in collection.  Bit of a hack.
            }

            foreach (WoodBuildSite toAddToPotentially in woodBuildSites)
            {
                float? intersectsJobSite = toAddToPotentially.intersects(removeWoodBlockClickRay);
                if (intersectsJobSite.HasValue)
                {

                    Vector3 locationOfSelectedSpaceOnJobSite = removeWoodBlockClickRay.Position + removeWoodBlockClickRay.Direction * ((float)intersectsJobSite);

                    if (exactBlockHitLocOnIsland.HasValue)
                    {
                        if (Vector3.Distance((Vector3)exactBlockHitLocOnIsland, removeWoodBlockClickRay.Position) >
                            Vector3.Distance(locationOfSelectedSpaceOnJobSite, removeWoodBlockClickRay.Position))
                        {
                            BlockLoc newBlockToPlace = new BlockLoc(locationOfSelectedSpaceOnJobSite + removeWoodBlockClickRay.Direction*.01f);
                            toAddToPotentially.removeBlock(newBlockToPlace);
                            return;
                        }
                    }
                    else
                    {

                        BlockLoc newBlockToPlace = new BlockLoc(removeWoodBlockClickRay.Position + removeWoodBlockClickRay.Direction * ((float)intersectsJobSite));
                        toAddToPotentially.removeBlock(newBlockToPlace);
                        return;
                    }
                }
            }

            //WoodBuildSite nSite = new WoodBuildSite(pathingProfile);
            //intersectables.Add(nSite);

        }


        public TreesJobSite getTreeJobSite()
        {
            foreach (JobSite test in jobSites)
            {
                if (test is TreesJobSite)
                {
                    return (TreesJobSite)test;
                }
            }
            return null;
        }

        public void placeTree(BlockLoc loc, Tree.treeTypes type)
        {
            getTreeJobSite().placeTree(loc, type);
        }

        public void chopBlock(BlockLoc blockLoc)
        {
            foreach (JobSite test in jobSites)
            {
                test.chopBlock(blockLoc);
            }
        }

        internal void placeWoodBlockPlanAlongRay(Ray placeWoodBlockClickRay, Vector3? exactSpaceHitLocOnIsland, IslandPathingProfile profile)
        {


            IEnumerable<WoodBuildSite> woodBuildSites = getWoodBuildSiteEnumerable();
            placeWoodBlockClickRay.Direction.Normalize();

            if (woodBuildSites.Count() == 0)
            {
                jobSites.Add(new WoodBuildSite(profile));
            }
            woodBuildSites = getWoodBuildSiteEnumerable();

            WoodBuildSite buildSite = null;

            foreach (WoodBuildSite toAddToPotentially in woodBuildSites)
            {
                buildSite = toAddToPotentially;
            }

            foreach (WoodBuildSite toAddToPotentially in woodBuildSites)
            {
                float? intersectsJobSite = toAddToPotentially.intersects(placeWoodBlockClickRay);
                if (intersectsJobSite.HasValue)
                {

                    Vector3 locationOfSelectedSpaceOnJobSite = placeWoodBlockClickRay.Position + placeWoodBlockClickRay.Direction * ((float)intersectsJobSite - .01f);

                    if (exactSpaceHitLocOnIsland.HasValue)
                    {
                        if (Vector3.Distance((Vector3)exactSpaceHitLocOnIsland, placeWoodBlockClickRay.Position) >
                            Vector3.Distance(locationOfSelectedSpaceOnJobSite, placeWoodBlockClickRay.Position))
                        {
                            BlockLoc newBlockToPlace = new BlockLoc(locationOfSelectedSpaceOnJobSite);
                            addBlockToBuildSite(toAddToPotentially, newBlockToPlace);
                            return;
                        }
                    }
                    else
                    {

                        BlockLoc newBlockToPlace = new BlockLoc(placeWoodBlockClickRay.Position + placeWoodBlockClickRay.Direction * ((float)intersectsJobSite - .01f));

                        // toAddToPotentially.addBlock(newBlockToPlace);
                        return;
                    }
                }
            }
            if (exactSpaceHitLocOnIsland.HasValue)
            {

                BlockLoc newBlockToPlace = new BlockLoc((Vector3)exactSpaceHitLocOnIsland - placeWoodBlockClickRay.Direction * .01f);
                if (!profile.isProfileSolidAt(newBlockToPlace))
                {
                    addBlockToBuildSite(buildSite, newBlockToPlace);
                }
                return;
            }

        }





        public void addJobSite(JobSite newJobSite)
        {
            jobSites.Add(newJobSite);
        }

        private HashSet<BlockLoc> allBlocksWithJobSites()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (JobSite test in jobSites)
            {
                foreach (BlockLoc loc in test.getAllBlocksInSite())
                {
                    result.Add(loc);
                }
            }
            return result;
        }

        public void acceptStrikeAt(BlockLoc blockLoc, JobType jobType)
        {
            switch (jobType)
            {
                case JobType.agriculture:
                    makeFarmBlockGrow(blockLoc);
                    break;
                case JobType.logging:
                    chopBlock(blockLoc);
                    break;
            }
        }

        public void updateAllMeshes(int mipLevel)
        {
            foreach (JobSite toMeshUpdate in jobSites)
            {
                toMeshUpdate.updateMesh(mipLevel);
            }
        }

        public void addResourceBlock(BlockLoc loc, ResourceBlock.ResourceType type)
        {
            resourceBlockJobsite.placeRescourceBlock(loc, type);
        }


        public ResourceBlockjobSite getResourceJobSite()
        {
            return resourceBlockJobsite;
        }
    }
}

