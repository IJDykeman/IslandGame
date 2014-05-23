using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    class PathHandler
    {
        //!! all path locations are at foot level NOT head level


        protected List<BlockLoc> getPathWithStartAndEndGroupValidation(IslandPathingProfile startProfile, BlockLoc startLoc, IslandPathingProfile endProfile, HashSet<BlockLoc> goals, int heightOfEntity)
        {

            if (!startProfile.isStandableAtWithHeight(startLoc, heightOfEntity) && !startProfile.isSwimableAtWithHeight(startLoc, heightOfEntity))
            {
                return null;
            }

            goals = removeUnstandableAndUnswimmableItemsInSet(endProfile, goals, heightOfEntity);
            if (goals.Count == 0)
            {
                return null;
            }



                return Path(startProfile, ref startLoc, goals, heightOfEntity);



        }



        protected virtual List<BlockLoc> Path(IslandPathingProfile startProfile, ref BlockLoc startLoc, HashSet<BlockLoc> goals, int heightOfEntity)
        {
            foreach (BlockLoc goal in goals)
            {
                if (startLoc.Equals(goal))
                {
                    List<BlockLoc> noPathResult = new List<BlockLoc>();
                    noPathResult.Add(startLoc);
                    return noPathResult;
                }
            }




            PathNodePriorityQueue openNodes = new PathNodePriorityQueue();
            HashSet<BlockLoc> visitedLocations = new HashSet<BlockLoc>();

            openNodes.insertNode(new PathNode(null, startLoc, 0, goals.First()));

            IslandPathingProfile profile = startProfile;
            while (openNodes.size() > 0)
            {
                PathNode from = openNodes.pop();

                List<BlockLoc> nextSteps = profile.getSpacesThatCanBeMovedToFrom(from.loc, heightOfEntity);

                //adding new nodes to the openNodes unmippedArray
                foreach (BlockLoc next in nextSteps)
                {
                    if (!visitedLocations.Contains(next))
                    {

                        PathNode toAdd = new PathNode(from, next, from.costToGetHere + 1, goals.First());

                        if (goals.Contains(toAdd.loc))
                        {
                            List<BlockLoc> finalPath = getPathListFromEnd(toAdd);
                            finalPath.RemoveAt(0);
                            finalPath.Add(toAdd.loc);


                            Console.WriteLine(finalPath.Count);
                            return finalPath;
                        }

                        openNodes.insertNode(toAdd);
                        visitedLocations.Add(next);

                    }
                }



            }
            return null;//no path found
        }

        protected virtual List<BlockLoc> PathNEW(IslandPathingProfile startProfile, ref BlockLoc startLoc, HashSet<BlockLoc> goals, int heightOfEntity)
        {
            foreach (BlockLoc goal in goals)
            {
                if (startLoc.Equals(goal))
                {
                    List<BlockLoc> noPathResult = new List<BlockLoc>();
                    noPathResult.Add(startLoc);
                    return noPathResult;
                }
            }




            PathNodePriorityQueue openNodes = new PathNodePriorityQueue();
            HashSet<BlockLoc> visitedLocations = new HashSet<BlockLoc>();


            openNodes.insertNode(new PathNode(null, startLoc, 0, goals.First()));

            IslandPathingProfile profile = startProfile;
            while (openNodes.size() > 0)
            {
                //return null;
                PathNode from = openNodes.pop();

                List<BlockLoc> nextSteps = profile.getSpacesThatCanBeMovedToFrom(from.loc, heightOfEntity);

                //adding new nodes to the openNodes array

                foreach (BlockLoc next in nextSteps)
                {
                    if (visitedLocations.Contains(next))
                    {

                        continue;
                    }

                    PathNode toAdd = new PathNode(from, next, from.costToGetHere + 1, goals.First());

                    if (goals.Contains(toAdd.loc))
                    {
                        List<BlockLoc> finalPath = getPathListFromEnd(toAdd);
                        finalPath.RemoveAt(0);
                        finalPath.Add(toAdd.loc);


                        Console.WriteLine(finalPath.Count);
                        return finalPath;
                    }

                    if (openNodes.hasNodeWithLoc(next))
                    {
                         continue;
                    }


                    if (visitedLocations.Contains(toAdd.loc))
                    {

                        continue;
                    }


                    openNodes.insertNode(toAdd);






                }
                visitedLocations.Add(from.loc);

                //return null;

            }
            return null;//no path found
        }

        public List<BlockLoc> getPathToSingleBlock(IslandPathingProfile startProfile, BlockLoc startLoc, IslandPathingProfile endProfile, BlockLoc endLoc, int heightOfEntity)
        {
            HashSet<BlockLoc> goals = new HashSet<BlockLoc>();
            goals.Add(endLoc);
            return getPathWithStartAndEndGroupValidation(startProfile, startLoc, endProfile, goals, heightOfEntity);
        }

        public List<BlockLoc> getPathToBlockEnumerable(IslandPathingProfile startProfile, BlockLoc startLoc, IslandPathingProfile endProfile, IEnumerable<BlockLoc> goalsEnum, int heightOfEntity)
        {
            HashSet<BlockLoc> goals = new HashSet<BlockLoc>();
            foreach (BlockLoc toAdd in goalsEnum)
            {
                goals.Add(toAdd);
            }
            if (goals.Count == 0)
            {
                return null;
            }
            return getPathWithStartAndEndGroupValidation(startProfile, startLoc, endProfile, goals, heightOfEntity);
        }




        public virtual List<BlockLoc> getPathToMakeTheseBlocksAvaiable(
            IslandPathingProfile startProfile,
            BlockLoc startLoc, 
            IslandPathingProfile endProfile,
            List<BlockLoc> blockLocs, 
            int heightOfEntity, 
            out BlockLoc blockMadeAvailable)
        {
            blockMadeAvailable = new BlockLoc();
            HashSet<BlockLoc> goals = new HashSet<BlockLoc>();
            foreach (BlockLoc blockLocToAccess in blockLocs)
            {
                HashSet<BlockLoc> toAdd= endProfile.getFootLocsThatHaveAccessToBlock(blockLocToAccess);
                foreach (BlockLoc addToGoals in toAdd)
                {
                    goals.Add(addToGoals);
                }
            }

            List<BlockLoc> path = getPathWithStartAndEndGroupValidation(startProfile, startLoc, endProfile, goals, heightOfEntity);

            foreach (BlockLoc blockToAcces in blockLocs)
            {
                if (path != null)
                {
                    if (endProfile.getFootLocsThatHaveAccessToBlock(blockToAcces).Contains(path.Last()))
                    {
                        blockMadeAvailable = blockToAcces;
                    }
                }

            }

            return path;
        }



        protected PathNode getNodeWithLocation(IntVector3 loc, List<PathNode> list)
        {
            foreach (PathNode test in list)
            {
                if (test.loc.Equals(loc))
                {
                    return test;
                }
            }
            return null;
        }

        protected List<BlockLoc> getPathListFromEnd(PathNode end)
        {
            List<BlockLoc> result = new List<BlockLoc>();
            PathNode current = end;

            while (true)
            {
                result.Add(current.loc);
                if (current.previous == null)
                {
                    break;
                }
                else
                {
                    current = current.previous;
                }
            }
            result.Reverse();
            return result;
        }

        protected HashSet<BlockLoc> removeUnstandableAndUnswimmableItemsInSet(IslandPathingProfile profile, HashSet<BlockLoc> locs, int entityHeight)
        {
            HashSet<BlockLoc> toRemove = new HashSet<BlockLoc>();
            foreach (BlockLoc test in locs)
            {
                if (!profile.isStandableAtWithHeight(test, 2) && !profile.isSwimableAtWithHeight(test,2))
                {
                    toRemove.Add(test);
                }
            }

            foreach (BlockLoc itemToRemove in toRemove)
            {
                locs.Remove(itemToRemove);
            }

            return locs;
        }
    }


}
