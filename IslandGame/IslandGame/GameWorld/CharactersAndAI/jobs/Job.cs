using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{

    public enum JobType
    {
        none,
        agriculture,
        combat,
        mining,
        building,
        logging,
        CarryingWood
    }

    public abstract class Job
    {
        private JobType jobType = JobType.none;

        public abstract CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker);
        public abstract bool isComplete();
        public abstract bool isUseable();

        public JobType getJobType()
        {
            return jobType;
        }

        public void setJobType(JobType newJobType)
        {
            jobType = newJobType;
        }



        public virtual void jobWasCancelled()
        {
            
        }

        public virtual BlockLoc? getGoalBlock()
        {
            return null;
        }

        public virtual CharacterTask.Task checkForWorkConflictsNullIfNoResponse(CharacterTaskTracker taskTracker)
        {
            return null;
        }
    }

    [Serializable]
    class UnemployedJob : Job
    {
        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            return new CharacterTask.NoTask();
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
