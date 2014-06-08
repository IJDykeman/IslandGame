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
        Job toReturnTo;


        public TravelAlongPath(List<BlockLoc> nPath, Job nToReturnTo)
        {
            toReturnTo = nToReturnTo;
            path = nPath;
            if (willResultInTravel())
            {
                currentStep = new CharacterTask.StepToBlock(path[0]);
            }
        }

        public TravelAlongPath(List<BlockLoc> nPath)
        {
            toReturnTo = null;
            path = nPath;
            if (willResultInTravel())
            {
                currentStep = new CharacterTask.StepToBlock(path[0]);
            }
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (( path == null || path.Count == 0))
            {
                if (toReturnTo != null)
                {
                    return new CharacterTask.SwitchJob(toReturnTo);
                }
                else
                {
                    return new CharacterTask.SwitchJob(new UnemployedJob());
                }
            }
            else if (!currentStep.isComplete())
            {
                return currentStep;
            }
            else
            {

                path.RemoveAt(0);
                if (path != null && path.Count > 0)
                {

                    currentStep = new CharacterTask.StepToBlock(path[0]);
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
            return false;/*
            if (path == null || path.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }*/
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
