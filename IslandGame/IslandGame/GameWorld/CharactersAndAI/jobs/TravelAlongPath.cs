using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace IslandGame.GameWorld
{
    [Serializable]
    class TravelAlongPath : Job
    {
        Path path;
        IslandGame.GameWorld.CharacterTask.StepToBlock currentStep;



        public TravelAlongPath(Path nPath, Job nToReturnTo)
        {
            Debug.Assert(nPath != null);
            toReturnTo = nToReturnTo;
            path = nPath;
            if (willResultInTravel())
            {
                currentStep = new CharacterTask.StepToBlock(path.getAt(0));
            }
            setJobType(JobType.none);
        }

        public TravelAlongPath(Path nPath)
        {
            Debug.Assert(nPath != null);
            toReturnTo = null;
            path = nPath;
            if (willResultInTravel())
            {
                currentStep = new CharacterTask.StepToBlock(path.getAt(0));
            }
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {


            if (!path.isUseable())
            {
                return new CharacterTask.SwitchJob(new UnemployedJob());
            }

            else if ( path.length() == 0)
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

                path.removeAt(0);
                if (path != null && path.length() > 0)
                {

                    currentStep = new CharacterTask.StepToBlock(path.getFirst());
                }


                return currentStep;
            }
            
        }

        public override bool isUseable()
        {
            return true;
        }

        public bool willResultInTravel()
        {
            return path != null && path.length() > 0;
        }


        public override bool isComplete()
        {
            return false;
        }

        public override JobType getJobType()
        {
            if (toReturnTo != null)
            {
                return toReturnTo.getJobType();
            }
            return base.getJobType();
        }

        public bool foundPath()
        {
            return path != null;
        }

    }
}
