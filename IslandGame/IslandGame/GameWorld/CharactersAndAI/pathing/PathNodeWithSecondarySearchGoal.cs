using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    class PathNodeForFindingLowGoals : PathNode
    {
        protected int stepsUntilGiveUpOnFindingBetterBlock = int.MaxValue;
        protected int numStepsStillToSearchAfterFindingABlock = 10;
        protected int yMovementWeight = 10;

        public PathNodeForFindingLowGoals() { }

        public PathNodeForFindingLowGoals(PathNode nPrevious, BlockLoc nLoc, int nCostToGetHere, BlockLoc endLoc, int nStepsUntilGiveUpOnFindingBetterBlock)
        {
            stepsUntilGiveUpOnFindingBetterBlock = nStepsUntilGiveUpOnFindingBetterBlock;
            previous = nPrevious;
            loc = nLoc;
            costToGetHere = nCostToGetHere;
            approximateDistanceToTarget = approximateDistanceTo(endLoc);
        }



        public void setStepCounterWhenNodeIsOnGoal()
        {
            stepsUntilGiveUpOnFindingBetterBlock = numStepsStillToSearchAfterFindingABlock;
        }

        public int getStepsUntilGiveUpOnFindingBetterBlock()
        {
            return stepsUntilGiveUpOnFindingBetterBlock;
        }

        public bool hasExaustedPostGoalSteps()
        {
            return stepsUntilGiveUpOnFindingBetterBlock <= 1;
        }

        public void incrementPostGoalSteps()
        {
            stepsUntilGiveUpOnFindingBetterBlock--;
        }

        public bool isDescendedFromNodeAtAGoal()
        {
            //bit of a hack.  Just tell you whether the number of steps is huge, 
                //if it's not then it was set small in setStepCounterWhenNodeIsOnGoal()
            return stepsUntilGiveUpOnFindingBetterBlock < int.MaxValue / 2;
        }
    }

    [Serializable]
    class PathNodeForFindingHighGoals : PathNodeForFindingLowGoals
    {
        public PathNodeForFindingHighGoals() 
        {
            yMovementWeight = -10;
        }

        public PathNodeForFindingHighGoals(PathNode nPrevious, BlockLoc nLoc, int nCostToGetHere, BlockLoc endLoc, int nStepsUntilGiveUpOnFindingBetterBlock)
        {
            yMovementWeight = -10;
            stepsUntilGiveUpOnFindingBetterBlock = nStepsUntilGiveUpOnFindingBetterBlock;
            previous = nPrevious;
            loc = nLoc;
            costToGetHere = nCostToGetHere;
            approximateDistanceToTarget = approximateDistanceTo(endLoc);
        }
    }

}
