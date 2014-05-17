using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    class PathHandlerPreferringLowerBlocks : PathHandler
    {
        private List<BlockLoc> PathToMakeGoalsAvailable(IslandPathingProfile startProfile, ref BlockLoc startLoc, out BlockLoc lowestAvailableBlockFound, 
            HashSet<BlockLoc> blocksToMakeAvailable, int heightOfEntity)
        {
            lowestAvailableBlockFound = new BlockLoc();
            /*foreach (BlockLoc goal in blocksToMakeAvailable)
            {
                if (startLoc.Equals(goal))
                {
                    List<BlockLoc> noPathResult = new List<BlockLoc>();
                    noPathResult.Add(startLoc);
                    return noPathResult;
                }
            }*/




            PathNodePriorityQueue openNodes = new PathNodePriorityQueue();
            HashSet<BlockLoc> visitedLocations = new HashSet<BlockLoc>();

            PathNodeForFindingLowGoals lowestPathNodeFoundSoFar = null;
            

            openNodes.insertNode(new PathNodeForFindingLowGoals(null, startLoc, 0, blocksToMakeAvailable.First(), int.MaxValue));

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
                

                if (((PathNodeForFindingLowGoals)from).hasExaustedPostGoalSteps() || ( ((PathNodeForFindingLowGoals)from).isDescendedFromNodeAtAGoal() && nextSteps.Count==0) )
                {
                    List<BlockLoc> finalPath = getPathListFromEnd(lowestPathNodeFoundSoFar);
                    finalPath.RemoveAt(0);
                    finalPath.Add(lowestPathNodeFoundSoFar.loc);


                    Console.WriteLine(finalPath.Count);
                    return finalPath;
                }

                //adding new nodes to the openNodes array
                foreach (BlockLoc next in nextSteps)
                {




                        PathNodeForFindingLowGoals toAdd = new PathNodeForFindingLowGoals
                            (from, next, from.costToGetHere + 1, blocksToMakeAvailable.First(),
                            ((PathNodeForFindingLowGoals)from).getStepsUntilGiveUpOnFindingBetterBlock() - 1);

                        HashSet<BlockLoc> blocksAvailableFromToAdd = profile.getBlocksAvailableForWorkFromFootLoc(toAdd.loc);



                        foreach (BlockLoc available in blocksAvailableFromToAdd)
                        {
                            if (blocksToMakeAvailable.Contains(available))
                            {
                                if (lowestPathNodeFoundSoFar == null || available.WSY() < lowestAvailableBlockFound.WSY())
                                {
                                    lowestAvailableBlockFound = available;
                                    toAdd.setStepCounterWhenNodeIsOnGoal();
                                    lowestPathNodeFoundSoFar = toAdd;
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

            if (lowestPathNodeFoundSoFar != null)
            {
                List<BlockLoc> finalPath = getPathListFromEnd(lowestPathNodeFoundSoFar);
                finalPath.RemoveAt(0);
                finalPath.Add(lowestPathNodeFoundSoFar.loc);


                Console.WriteLine(finalPath.Count);
                return finalPath;
            }

            return null;//no path found
        }


        public override List<BlockLoc> getPathToMakeTheseBlocksAvaiable(
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


            return path;
        }
   
    }
}
