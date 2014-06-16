using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    public class CaptainingBoatJob : Job
    {
        private Boat boat;
        TravelAlongPath travelAlongPath;

        public CaptainingBoatJob(Boat nBoat)
        {
            boat = nBoat;
        }

        public override CharacterTask.Task getCurrentTask(CharacterTaskTracker taskTracker)
        {
            if (travelAlongPath != null)
            {
                if (travelAlongPath.isUseable() && !travelAlongPath.isComplete())
                {
                    CharacterTask.Task task = travelAlongPath.getCurrentTask(taskTracker);
                    if (task.taskType == CharacterTask.Type.StepToBlock)
                    {
                        CharacterTask.StepToBlock stepTask = (CharacterTask.StepToBlock)task;
                        Vector3 newFootLoc = LinearLocationInterpolator.interpolate(boat.getFootLocation(), stepTask.getGoalLoc().toWorldSpaceVector3() + new Vector3(.5f, 0, .5f), boat.getSpeed());

                        boat.setFootLocation(newFootLoc);
                        boat.setRotationWithGivenDeltaVec(stepTask.getGoalLoc().toWorldSpaceVector3() + new Vector3(1, 1, 1) / 2f - boat.getFootLocation());
                        boat.setVelocity(new Vector3());
                        //setRootPartRotationOffset(getYRotationFromDeltaVector(stepTask.getGoalLoc().toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f) - getFootLocation()));
                        if (LinearLocationInterpolator.isLinearInterpolationComplete(boat.getFootLocation(), stepTask.getGoalLoc().toWorldSpaceVector3() + new Vector3(.5f, 0, .5f), boat.getSpeed()))
                        {
                            stepTask.taskWasCompleted();
                        }
                    }

                }
                else if (travelAlongPath.isComplete())
                {
                    travelAlongPath = null;
                }
            }

            return new CharacterTask.CaptainBoat(boat);
        }

        public override bool isComplete()
        {
            return false;
        }

        public override bool isUseable()
        {
            return true;
        }

        public override void jobWasCancelled()
        {
            boat.setVelocity(new Vector3());
        }



        public Boat getBoat()
        {
            return boat;
        }

        public void setTravelPath(List<BlockLoc> nPath)
        {
            travelAlongPath = new TravelAlongPath(nPath);
        }

        public void cancelPathing()
        {
            travelAlongPath = null;
        }
    }
}
