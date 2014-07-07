using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    class WaitJob : Job
    {
        int waitDuration;
        int waitSoFar;

        public WaitJob(int duration, Job ntoReturnTo, JobType nType)
        {
            waitDuration = duration;
            waitSoFar = 0;
            toReturnTo = ntoReturnTo;
            setJobType(nType);
        }

        public void update()
        {
            waitSoFar++;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            waitSoFar++;
            if (waitSoFar >= waitDuration)
            {
                return new CharacterTask.SwitchJob(toReturnTo);
            }
            else
            {
                return new CharacterTask.NoTask();
            }
            
        }
        public override bool isComplete()
        {
            return false;
        }
        public override bool isUseable()
        {
            return true;
        }
    }
}
