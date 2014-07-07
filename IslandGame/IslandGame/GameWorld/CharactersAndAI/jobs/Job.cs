using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    [Serializable]
    public enum JobType
    {
        none,
        agriculture,
        combat,
        mining,
        building,
        logging,
        CarryingSomething
    }

    [Serializable]
    public abstract class Job
    {
        private JobType jobType = JobType.none;

        public abstract CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker);
        public abstract bool isComplete();
        public abstract bool isUseable();
        //PUT TARGETBLOCK FIELD HERE
        protected BlockLoc targetBlock;
        protected Job toReturnTo;

        public virtual JobType getJobType()
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

        public List<BlockLoc> getGoalBlock()
        {
            List<BlockLoc> result = new List<BlockLoc>(1);
            if (targetBlock != null)
            {
                result.Add(targetBlock);
            }
            if (toReturnTo != null)
            {
                result.AddRange(toReturnTo.getGoalBlock());
            }
            return result;

        }

        public CharacterTask.Task waitBeforeDoingReturnTo(int timeToWait, JobType type)
        {
            return new CharacterTask.SwitchJob(new WaitJob(timeToWait, toReturnTo, type));
        }

        public Job getWaitJobWithReturn(int timeToWait, Job ntoReturnTo)
        {
            return new WaitJob(timeToWait, ntoReturnTo, ntoReturnTo.getJobType());
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
