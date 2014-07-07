using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    class PathHandlerPreferringHigherBlocks : PathHandler
    {
        private List<BlockLoc> PathToMakeGoalsAvailable(IslandPathingProfile startProfile, ref BlockLoc startLoc, out BlockLoc highestAvailableBlockFound, 
            HashSet<BlockLoc> blocksToMakeAvailable, int heightOfEntity)
        {
            highestAvailableBlockFound = new BlockLoc();

                if (blocksToMakeAvailable.Count == 0)
                {
                    List<BlockLoc> noPathResult = new List<BlockLoc>();
                    noPathResult.Add(startLoc);
                    return noPathResult;
                }
            




            PathNodePriorityQueue openNodes = new PathNodePriorityQueue();
            HashSet<BlockLoc> visitedLocations = new HashSet<BlockLoc>();

            PathNodeForFindingLowGoals highestPathNodeFoundSoFar = null;
            

            openNodes.insertNode(new PathNodeForFindingHighGoals(null, startLoc, 0, blocksToMakeAvailable.First(), int.MaxValue));

            IslandPathingProfile profile = startProfile;
            while (openNodes.size() > 0)
            {
                PathNode from = openNodes.pop();

                List<BlockLoc> nextSteps = profile.getSpacesThatCanBeMovedToFrom(from.loc, heightOfEntity);

                for (int i = nextSteps.Count - 1; i >= 0; i--)
                {
                    if (visitedLocations.Contains(nextSteps[i]))
                    {
                        nextSteps.RemoveAt(i);
                    }
                }


                if (((PathNodeForFindingHighGoals)from).hasExaustedPostGoalSteps() || (((PathNodeForFindingHighGoals)from).isDescendedFromNodeAtAGoal() && nextSteps.Count == 0))
                {
                    List<BlockLoc> finalPath = getPathListFromEnd(highestPathNodeFoundSoFar);
                    finalPath.RemoveAt(0);
                    finalPath.Add(highestPathNodeFoundSoFar.loc);


                    Console.WriteLine(finalPath.Count);
                    return finalPath;
                }

                //adding new nodes to the openNodes unmippedArray
                foreach (BlockLoc next in nextSteps)
                {




                    PathNodeForFindingHighGoals toAdd = new PathNodeForFindingHighGoals
                            (from, next, from.costToGetHere + 1, blocksToMakeAvailable.First(),
                            ((PathNodeForFindingHighGoals)from).getStepsUntilGiveUpOnFindingBetterBlock() - 1);

                        HashSet<BlockLoc> blocksAvailableFromToAdd = profile.getBlocksAvailableForWorkFromFootLoc(toAdd.loc);



                        foreach (BlockLoc available in blocksAvailableFromToAdd)
                        {
                            if (blocksToMakeAvailable.Contains(available))
                            {
                                if (highestPathNodeFoundSoFar == null || available.WSY() > highestAvailableBlockFound.WSY())
                                {
                                    highestAvailableBlockFound = available;
                                    toAdd.setStepCounterWhenNodeIsOnGoal();
                                    highestPathNodeFoundSoFar = toAdd;
                                }

                            }
                        }
                        
                        //toAdd.
                        

                        toAdd.incrementPostGoalSteps();
                       //
                       // Compositer.addFlagForThisFrame(toAdd.xLowZ.toWorldSpaceVector3(), "white");
                        openNodes.insertNode(toAdd);
                        visitedLocations.Add(next);

                    }
                

            }

            if (highestPathNodeFoundSoFar != null)
            {
                List<BlockLoc> finalPath = getPathListFromEnd(highestPathNodeFoundSoFar);
                finalPath.RemoveAt(0);
                finalPath.Add(highestPathNodeFoundSoFar.loc);


                Console.WriteLine(finalPath.Count);
                return finalPath;
            }

            return null;//no path found
        }


        public override Path getPathToMakeTheseBlocksAvaiable(
            IslandPathingProfile startProfile,
            BlockLoc startLoc,
            IslandPathingProfile endProfile,
            List<BlockLoc> blockLocs,
            int heightOfEntity,
            out BlockLoc blockMadeAvailable)
        {
            blockMadeAvailable = new BlockLoc();
            HashSet<BlockLoc> blocksToMakeAvailable = new HashSet<BlockLoc>();
            foreach (BlockLoc blockLocToAccess in blockLocs)
            {
                blocksToMakeAvailable.Add(blockLocToAccess);
            }

            List<BlockLoc> path = PathToMakeGoalsAvailable(startProfile, ref startLoc, out blockMadeAvailable, blocksToMakeAvailable, heightOfEntity);


            return new Path(path);
        }
   
    }
}
