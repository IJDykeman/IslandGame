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
        ResourceBlockJobSite resourceBlockJobsite;
        ExcavationSite excavationSite;
        IslandWorkingProfile workingProfile;
        BuildStie buildSite;

        public JobSiteManager(IslandWorkingProfile nworkingProfile)
        {
            workingProfile = nworkingProfile;
            jobSites = new List<JobSite>();
            jobSites.Add(new TreesJobSite(nworkingProfile.getPathingProfile()));

            excavationSite = new ExcavationSite(nworkingProfile.getPathingProfile());
            jobSites.Add(excavationSite);
            resourceBlockJobsite = new ResourceBlockJobSite(nworkingProfile.getPathingProfile());
            jobSites.Add(resourceBlockJobsite);
            buildSite = new BuildStie(workingProfile.getPathingProfile());
            jobSites.Add(buildSite);
        }

        public void display(GraphicsDevice device, Effect effect, IslandLocationProfile locationProfile, DisplayParameters parameters)
        {
            foreach (JobSite site in jobSites)
            {
                site.draw(device, effect, parameters);
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


            excavationSite.addBlockToDestroy(blockLoc);

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

        internal void addPlayerDraggedJobsiteWithBlocks(List<BlockLoc> blocksToAdd, IslandPathingProfile profile, 
            PlayerAction.Dragging.DragType dragType)
        {


           /// blocksToAdd = alreadyOccupied;

            switch (dragType)
            {
                case PlayerAction.Dragging.DragType.farm:
                    placeFarmWithBlocks(blocksToAdd, profile);
                    break;
                case PlayerAction.Dragging.DragType.storeWheat:
                    placeStorageAreaWithBlocksToPlaceOn(blocksToAdd, profile, ResourceBlock.ResourceType.Wheat);
                    break;
                case PlayerAction.Dragging.DragType.storeWood:
                    placeStorageAreaWithBlocksToPlaceOn(blocksToAdd, profile, ResourceBlock.ResourceType.Wood);
                    break;
                case PlayerAction.Dragging.DragType.storeStone:
                    placeStorageAreaWithBlocksToPlaceOn(blocksToAdd, profile, ResourceBlock.ResourceType.standardBlock);
                    break;
                case PlayerAction.Dragging.DragType.excavate:
                    foreach (BlockLoc test in blocksToAdd)
                    {
                        addExcavationMark(test, workingProfile.getPathingProfile());
                    }
                    break;

            }
        }

        private void placeStorageAreaWithBlocksToPlaceOn(IEnumerable<BlockLoc> blocksToPlaceSiteOn, IslandPathingProfile profile,
            ResourceBlock.ResourceType toStore)
        {

            List<BlockLoc> blocksForSite = new List<BlockLoc>();
            HashSet<BlockLoc> alreadyOccupied = allBlocksWithJobSites();

            foreach (BlockLoc inGround in blocksToPlaceSiteOn)
            {
                if (!alreadyOccupied.Contains(BlockLoc.AddIntVec3(inGround, new IntVector3(0, 1, 0))))
                {
                    blocksForSite.Add(BlockLoc.AddIntVec3(inGround, new IntVector3(0, 1, 0)));
                }

                if (!alreadyOccupied.Contains(BlockLoc.AddIntVec3(inGround, new IntVector3(0, 2, 0))))
                {
                    blocksForSite.Add(BlockLoc.AddIntVec3(inGround, new IntVector3(0, 2, 0)));
                }
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
                resourceBlockJobsite.addStockpile(new Stockpile(locsNotSolid,toStore));
                
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


        private void addBlockToBuildSite(BuildStie toAddToPotentially, BlockLoc newBlockToPlace, byte typeToAdd)
        {
            if (allBlocksWithJobSites().Contains(newBlockToPlace))
            {
                return;
            }
            toAddToPotentially.addBlock(newBlockToPlace, typeToAdd);
        }

        public void blockWasBuilt(BlockLoc blockLoc)
        {
            foreach (JobSite site in jobSites)
            {
                site.blockWasBuilt(blockLoc);
            }
        }

        public void removeWoodBlockPlanAlongRay(Ray removeWoodBlockClickRay, Vector3? exactBlockHitLocOnIsland, IslandPathingProfile profile)
        {


            float? intersectsJobSite = buildSite.intersects(removeWoodBlockClickRay);
            if (intersectsJobSite.HasValue)
            {

                Vector3 locationOfSelectedSpaceOnJobSite = removeWoodBlockClickRay.Position + removeWoodBlockClickRay.Direction * ((float)intersectsJobSite);

                if (exactBlockHitLocOnIsland.HasValue)
                {
                    if (Vector3.Distance((Vector3)exactBlockHitLocOnIsland, removeWoodBlockClickRay.Position) >
                        Vector3.Distance(locationOfSelectedSpaceOnJobSite, removeWoodBlockClickRay.Position))
                    {
                        BlockLoc blockToRemove = new BlockLoc(locationOfSelectedSpaceOnJobSite + removeWoodBlockClickRay.Direction * .01f);
                        buildSite.removeBlock(blockToRemove);
                        return;
                    }
                }
                else
                {

                    BlockLoc blockToRemove = new BlockLoc(removeWoodBlockClickRay.Position + removeWoodBlockClickRay.Direction * ((float)intersectsJobSite));
                    buildSite.removeBlock(blockToRemove);
                    return;
                }
            }

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

        internal void placeWoodBlockPlanAlongRay(Ray placeWoodBlockClickRay, Vector3? exactSpaceHitLocOnIsland, IslandPathingProfile profile, byte typeToAdd)
        {


            
            placeWoodBlockClickRay.Direction.Normalize();

            Vector3? bestBlockToPlaceOnBuildSite = getLastSpaceAlongRayConsideringBuildSite(placeWoodBlockClickRay, exactSpaceHitLocOnIsland);

            if (bestBlockToPlaceOnBuildSite.HasValue)
            {
                addBlockToBuildSite(buildSite,new BlockLoc((Vector3)bestBlockToPlaceOnBuildSite),typeToAdd);
                return;
            }
        }

        public Vector3? getLastSpaceAlongRayConsideringBuildSite(Ray ray, Vector3? exactSpaceHitLocOnIsland)
        {
            
                float? intersectsJobSite = buildSite.intersects(ray);
                if (intersectsJobSite.HasValue)
                {

                    Vector3 locationOfSelectedSpaceOnJobSite = ray.Position + ray.Direction 
                        * ((float)intersectsJobSite - .001f);

                    if (exactSpaceHitLocOnIsland.HasValue)
                    {
                        if (Vector3.Distance((Vector3)exactSpaceHitLocOnIsland, ray.Position) >
                            Vector3.Distance(locationOfSelectedSpaceOnJobSite, ray.Position))
                        {
                            return locationOfSelectedSpaceOnJobSite;
                        }
                        else
                        {
                            return exactSpaceHitLocOnIsland;
                        }
                    }
                    else
                    {
                        return locationOfSelectedSpaceOnJobSite;
                    }
                }
                else
                {

                    return exactSpaceHitLocOnIsland;
                }
            
            return null;
        }

        internal Vector3? getLastSpaceAlongRayConsideringResourceBlocks(Ray ray, Vector3? exactSpaceHitLocOnIsland)
        {



            float? intersectsJobSite = resourceBlockJobsite.intersects(ray);
            if (intersectsJobSite.HasValue)
            {

                Vector3 locationOfSelectedSpaceOnJobSite = ray.Position + ray.Direction
                    * ((float)intersectsJobSite - .001f);

                if (exactSpaceHitLocOnIsland.HasValue)
                {
                    if (Vector3.Distance((Vector3)exactSpaceHitLocOnIsland, ray.Position) >
                        Vector3.Distance(locationOfSelectedSpaceOnJobSite, ray.Position))
                    {
                        return locationOfSelectedSpaceOnJobSite;
                    }
                    else
                    {
                        return exactSpaceHitLocOnIsland;
                    }
                }
                else
                {
                    return locationOfSelectedSpaceOnJobSite;
                }
            }
            else
            {

                return exactSpaceHitLocOnIsland;
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

        internal void removeResourceBlock(BlockLoc blockLoc, ResourceBlock.ResourceType resourceType)
        {
            resourceBlockJobsite.removeRescourceBlock(blockLoc, resourceType);
        }

        public ResourceBlockJobSite getResourceJobSite()
        {
            return resourceBlockJobsite;
        }

        public ExcavationSite getExcavationSite()
        {
            return excavationSite;
        }

        internal bool isSolidAt(BlockLoc loc)
        {
            if (resourceBlockJobsite.isSolidAt(loc))
            {
                return true;
            }
            //else if (getTreeJobSite().getTreeTrunkBlocks().Contains(loc))
            //{
            //    return true;
            //}
            else
            {
                return false;
            }
        }

        internal bool couldAffordResourceExpendeture(int cost, ResourceBlock.ResourceType resourceType)
        {
            return resourceBlockJobsite.getBlocksToGetThisTypeFrom(resourceType).Count() >= cost;
        }

        internal void debitResource(int cost, ResourceBlock.ResourceType resourceType)
        {
            resourceBlockJobsite.debitResource(cost, resourceType);
        }

        internal void deleteJobsiteAlongRay(Ray ray)
        {
            JobSite alongRay = getJobSiteAlongRay(ray);
            if (alongRay != null)
            {
                if (alongRay.respondToDeleteClickAndReturnIfShouldBeDeleted(ray))
                {
                    jobSites.Remove(alongRay);
                }
            }
        }
    }
}

