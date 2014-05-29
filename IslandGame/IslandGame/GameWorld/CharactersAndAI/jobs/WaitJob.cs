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

        public WaitJob(int duration)
        {
            waitDuration = duration;
            waitSoFar = 0;
        }

        public void update()
        {
            waitSoFar++;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            return new CharacterTask.NoTask();
        }
        public override bool isComplete()
        {
            return waitSoFar >= waitDuration;
        }
        public override bool isUseable()
        {
            return true;
        }
    }
}
