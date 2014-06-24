using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld.CharactersAndAI
{
    class BuildKickoffJob : Job
    {
        WoodBuildSite buildSite;
        WaitJob currentWait = null;
        Character character;
        IslandWorkingProfile workingProfile;


        public BuildKickoffJob( WoodBuildSite nBuildSite, Character nCharacter, IslandWorkingProfile nworkingProfile)
        {
            workingProfile = nworkingProfile;
            buildSite = nBuildSite;
            character = nCharacter;
            setJobType(JobType.building);
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (character.isCarryingItem() && character.getLoad() == ResourceBlock.ResourceType.Stone)
            {
                if (buildSite.numBlocksLeftToBuild() > 0)
                {
                    List<BlockLoc> nextBlocksToBuild = buildSite.getNextBlocksToBuild().ToList();

                    foreach (BlockLoc claimed in taskTracker.blocksCurrentlyClaimed())
                    {
                        if (nextBlocksToBuild.Contains(claimed))
                        {
                            nextBlocksToBuild.Remove(claimed);
                        }

                    }
                    BlockLoc blockFoundToBuild;

                    PathHandlerPreferringLowerBlocks pathhandler = new PathHandlerPreferringLowerBlocks();
                    Path path = pathhandler.getPathToMakeTheseBlocksAvaiable(
                buildSite.getProfile(),
                new BlockLoc(character.getFootLocation()),
                buildSite.getProfile(),
                nextBlocksToBuild,
                2, out blockFoundToBuild);



                    PlaceBlockJob placeBlockJob = new PlaceBlockJob(buildSite, character, blockFoundToBuild, 
                        new BuildKickoffJob(buildSite,character,workingProfile), workingProfile);
                    TravelAlongPath walkJob = new TravelAlongPath(path, placeBlockJob);;
                    return new CharacterTask.SwitchJob(walkJob);

                }

                else
                {
                    return new CharacterTask.SwitchJob(new UnemployedJob());
                }
            }
            else
            {
                if (buildSite.numBlocksLeftToBuild() > 0)
                {
                    List<BlockLoc> nextBlocksToBuild = buildSite.getNextBlocksToBuild().ToList();

                    foreach (BlockLoc claimed in taskTracker.blocksCurrentlyClaimed())
                    {
                        if (nextBlocksToBuild.Contains(claimed))
                        {
                            nextBlocksToBuild.Remove(claimed);
                        }

                    }
                    BlockLoc blockFoundToBuild;

                    PathHandlerPreferringLowerBlocks pathhandler = new PathHandlerPreferringLowerBlocks();
                    Path path = pathhandler.getPathToMakeTheseBlocksAvaiable(
                buildSite.getProfile(),
                new BlockLoc(character.getFootLocation()),
                buildSite.getProfile(),
                nextBlocksToBuild,
                2, out blockFoundToBuild);



                    //PlaceBlockJob placeBlockJob = new PlaceBlockJob(buildSite, character, blockFoundToBuild, workingProfile);
                    BuildKickoffJob build =  new BuildKickoffJob(buildSite, character, workingProfile);
                    FetchResourceJob fetch = new FetchResourceJob(workingProfile, ResourceBlock.ResourceType.Stone, character, build);
                    return new CharacterTask.SwitchJob(fetch);

                }

                else
                {
                    return new CharacterTask.SwitchJob(new UnemployedJob());
                }
            }



        }


        public override bool isComplete()
        {
            return buildSite.siteIsComplete();
        }

        public override bool isUseable()
        {
            return true;
        }

        public BlockLoc? getCurrentGoalBlock()
        {
            return null;
        }

    }
}
