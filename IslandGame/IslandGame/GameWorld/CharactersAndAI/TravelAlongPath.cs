using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    class TravelAlongPath : Job
    {
        List<BlockLoc> path;
        IslandGame.GameWorld.CharacterTask.StepToBlock currentStep;


        public TravelAlongPath(List<BlockLoc> nPath)
        {

            path = nPath;
            if (willResultInTravel())
            {
                currentStep = new CharacterTask.StepToBlock(path[0]);
            }


        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (!currentStep.isComplete())
            {
                return currentStep;
            }
            else
            {
                if (path.Count > 0)
                {
                    path.RemoveAt(0);
                    if (!isComplete())
                    {
                        currentStep = new CharacterTask.StepToBlock(path[0]);
                    }
                }
                return currentStep;
            }
            
        }

        public override bool isUseable()
        {
            return path != null;
        }

        public bool willResultInTravel()
        {
            return path != null && path.Count > 0;
        }


        public override bool isComplete()
        {
            if (path == null || path.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool foundPath()
        {
            return path != null;
        }

        public BlockLoc getGoalBlock()
        {

            return path[path.Count - 1];

        }
    }
}
