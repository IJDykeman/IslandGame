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
        ResourceManager rescourcesOnThisIsland;
        


        public JobSiteManager(IslandPathingProfile profile)
        {
            rescourcesOnThisIsland = new ResourceManager();
            jobSites = new List<JobSite>();
            jobSites.Add(new TreesJobSite(profile));
        }

        public void display(GraphicsDevice device, Effect effect, IslandLocationProfile locationProfile)
        {
            foreach (JobSite site in jobSites)
            {
                site.draw(device, effect);
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
            return getJobSiteAlongRay(ray, jobSites);
        }

        public static JobSite getJobSiteAlongRay(Ray ray, IEnumerable<JobSite> jobSites)
        {
            float? minDist = float.MaxValue;
            JobSite result = null;
            foreach (JobSite site in jobSites)
            {
                float? thisDist = site.intersects(ray);
                if (thisDist.HasValue)
                {
                    if (minDist > thisDist)
                    {
                        minDist = thisDist;
                        result = site;
                    }
                }
            }
            return result;
        }

        internal void addFarmWithGivenBlocksToFarmOn(IEnumerable<BlockLoc> blocksToAdd, IslandPathingProfile profile)
        {
            HashSet<BlockLoc> locs = new HashSet<BlockLoc>();
            HashSet<BlockLoc> blocksToNotBeFarmed = allBlocksWithJobSites();
            foreach (BlockLoc test in blocksToAdd)
            {
                locs.Add(test);
            }
            foreach (BlockLoc alreadyOccupiedBlock in getAllFarmBlocksfarmedOn())
            {
                locs.Remove(alreadyOccupiedBlock);
            }
            foreach (BlockLoc alreadyOccupiedBlock in blocksToNotBeFarmed)
            {
                locs.Remove(BlockLoc.AddIntVec3(alreadyOccupiedBlock, new IntVector3(0,-1,0)));
            }

            if (locs.Count > 0)
            {
                jobSites.Add(new Farm(profile, locs));
            }
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

        internal void placeWoodBlockPlanAlongRay(Ray placeWoodBlockClickRay, Vector3? exactSpaceHitLocOnIsland, IslandPathingProfile profile)
        {


            IEnumerable<WoodBuildSite> woodBuildSites = getWoodBuildSiteEnumerable();
            placeWoodBlockClickRay.Direction.Normalize();

            if (woodBuildSites.Count() == 0)
            {
                jobSites.Add(new WoodBuildSite(profile));
            }
            woodBuildSites = getWoodBuildSiteEnumerable();

            WoodBuildSite buildSite= null;

            foreach (WoodBuildSite toAddToPotentially in woodBuildSites)
            {
                buildSite = toAddToPotentially;
            }
            
            foreach (WoodBuildSite toAddToPotentially in woodBuildSites)
            {
                float? intersectsJobSite = toAddToPotentially.intersects(placeWoodBlockClickRay);
                if (intersectsJobSite.HasValue)
                {
                    
                    Vector3 locationOfSelectedSpaceOnJobSite = placeWoodBlockClickRay.Position + placeWoodBlockClickRay.Direction * ((float)intersectsJobSite-.01f);

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

                        BlockLoc newBlockToPlace = new BlockLoc(placeWoodBlockClickRay.Position + placeWoodBlockClickRay.Direction * ((float)intersectsJobSite-.01f));
                        
                       // toAddToPotentially.addBlock(newBlockToPlace);
                        return;
                    }
                }
            }
            if (exactSpaceHitLocOnIsland.HasValue)
            {

                BlockLoc newBlockToPlace = new BlockLoc((Vector3)exactSpaceHitLocOnIsland - placeWoodBlockClickRay.Direction*.01f);
                if (!profile.isProfileSolidAt(newBlockToPlace))
                {
                    addBlockToBuildSite(buildSite, newBlockToPlace);
                }
                return;
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
            //jobSites.Add(nSite);

        }

        private TreesJobSite getLoggingSiteInJobList()
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
            getLoggingSiteInJobList().placeTree(loc, type);
        }


        public void chopBlock(BlockLoc blockLoc)
        {
            foreach (JobSite test in jobSites)
            {
                rescourcesOnThisIsland.addRescources(test.chopBlockAndGetRescources(blockLoc));
            }
        }


        public void makeFarmBlockGrow(BlockLoc blockLoc)
        {
            foreach (JobSite test in jobSites)
            {
                rescourcesOnThisIsland.addRescources(test.makeFarmBlockGrowAndGetRescources(blockLoc));
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
    }
}

