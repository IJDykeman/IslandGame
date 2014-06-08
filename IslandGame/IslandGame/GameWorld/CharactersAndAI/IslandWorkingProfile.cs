using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
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
            return getBlocksToGetThisTypeFrom(typeToFetch).ToList();
        }

        public ResourceBlockjobSite getResourcesJobSite()
        {
            return jobSiteManager.getResourceJobSite();
        }

        internal IslandPathingProfile getPathingProfile()
        {
            return profile;
        }
    }
}
