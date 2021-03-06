﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class IslandWorkingProfile
    {
        private JobSiteManager jobSiteManager;
        private IslandPathingProfile profile;

        public IslandWorkingProfile(JobSiteManager nJobsiteManager, IslandPathingProfile nProfile)
        {
            jobSiteManager = nJobsiteManager;
            profile = nProfile;
        }

        public TreesJobSite getTreeJobSite()
        {
            return jobSiteManager.getTreeJobSite();
        }

        public IEnumerable<BlockLoc> getBlocksToGetThisTypeFrom(ResourceBlock.ResourceType typeToFetch)
        {
            return jobSiteManager.getResourceJobSite().getBlocksToGetThisTypeFrom(typeToFetch).ToList();
        }

        public ResourceBlockJobSite getResourcesJobSite()
        {
            return jobSiteManager.getResourceJobSite();
        }

        public ExcavationSite getExcavationSite()
        {
            return jobSiteManager.getExcavationSite();
        }

        public IslandPathingProfile getPathingProfile()
        {
            return profile;
        }


    }
}
