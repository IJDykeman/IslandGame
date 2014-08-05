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
                        Vector3 stepGoal = stepTask.getGoalLoc().toWorldSpaceVector3() + new Vector3(.5f, 0, .5f);
                        stepGoal.Y = boat.getFootLocation().Y;
                        Vector3 newFootLoc = LinearLocationInterpolator.interpolate(boat.getFootLocation(), stepGoal, boat.getSpeed());
                        newFootLoc.Y = boat.getFootLocation().Y;
                        boat.setFootLocation(newFootLoc);
                        boat.setRotationWithGivenDeltaVec(stepGoal - boat.getFootLocation());
                        boat.setVelocity(new Vector3());

                        
                        if (LinearLocationInterpolator.isLinearInterpolationComplete(boat.getFootLocation(),
                            stepGoal, boat.getSpeed()))
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

        public void setTravelPath(Path nPath)
        {
            travelAlongPath = new TravelAlongPath(nPath);
        }

        public void cancelPathing()
        {
            travelAlongPath = null;
        }
    }
}
