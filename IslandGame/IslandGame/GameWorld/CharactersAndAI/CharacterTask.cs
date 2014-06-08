using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class CharacterTask
    {
        public enum Type
        {
            StepToBlock,
            WalkTowardPoint,
            NoTask,
            DestoryBlock,
            BuildBlock,
            ChopBlockForFrame,
            ObjectBuildForFrame,
            CaptainBoat,
            GetInBoat,
            SwitchJob,
            LookTowardPoint,
            MakeFarmBlockGrow,
            DoStrikeOfWorkAlongRay,
            DoStrikeOfWorkOnBlock,
            PlaceResource,

        }

        [Serializable]
        public abstract class Task
        {
            public Type taskType;
            public abstract bool isComplete();
        }

        [Serializable]
        public class NoTask : Task
        {
            public NoTask()
            {
                taskType = Type.NoTask;
            }
            public override bool isComplete()
            {
                return true;
            }
        }

        [Serializable]
        public class StepToBlock : Task
        {
            BlockLoc goalLoc;
            bool complete = false;

            public StepToBlock(BlockLoc nGoalLoc)
            {
                goalLoc = nGoalLoc;
                taskType = Type.StepToBlock;
            }

            public BlockLoc getGoalLoc()
            {
                return goalLoc;
            }

            public void taskWasCompleted()
            {
                complete = true;
            }

            public override bool isComplete()
            {
                return complete;
            }


        }

        [Serializable]
        public class WalkTowardPoint : Task
        {
            Vector3 target;

            public WalkTowardPoint(Vector3 ntarget)
            {
                target = ntarget;
                taskType = Type.WalkTowardPoint;
            }

            public Vector3 getTargetLoc()
            {
                return target;
            }

            public override bool isComplete()
            {
                return true;
            }
        }

        [Serializable]
        public class LookTowardPoint : Task
        {
            Vector3 target;

            public LookTowardPoint(Vector3 ntarget)
            {
                target = ntarget;
                taskType = Type.LookTowardPoint;
            }

            public Vector3 getTargetLoc()
            {
                return target;
            }

            public override bool isComplete()
            {
                return true;
            }
        }

        [Serializable]
        public class DestroyBlock : Task
        {
            BlockLoc toDestroy;
            bool completed = false;
            int timer = 0;
            int timeBeforeDestroy = 30;
            int pauseAfterCompletion = 10;

            public DestroyBlock(BlockLoc nToDestory)
            {
                toDestroy = nToDestory;
                taskType = Type.DestoryBlock;
            }

            public BlockLoc getBlockToDestroy()
            {
                return toDestroy;
            }

            public void destructionWasOrdered()
            {
                completed = true;
            }

            public override bool isComplete()
            {
                return completed && timer >= timeBeforeDestroy + pauseAfterCompletion;
            }


            internal bool readyToDestroy()
            {
                return timer == timeBeforeDestroy;
            }

            internal void updateTime()
            {
                timer++;
            }
        }

        [Serializable]
        public class BuildBlock : Task
        {
            BlockLoc toBuild;
            bool completed = false;
            byte blockType;


            public BuildBlock(BlockLoc nToDestory, byte nTypeToBuild)
            {
                toBuild = nToDestory;
                taskType = Type.BuildBlock;
                blockType = nTypeToBuild;
            }

            public BlockLoc getBlockLocToBuild()
            {
                return toBuild;
            }

            public byte getBlockTypeToBuild()
            {
                return blockType;
            }

            public void destructionWasOrdered()
            {
                completed = true;
            }

            public override bool isComplete()
            {
                return completed;
            }

        }

        [Serializable]
        public class ChopBlockForFrame : Task
        {
            BlockLoc toChop;

            public ChopBlockForFrame(BlockLoc nToDestory)
            {
                toChop = nToDestory;
                taskType = Type.ChopBlockForFrame;
            }

            public BlockLoc getBlockToChop()
            {
                return toChop;
            }

            public override bool isComplete()
            {
                return true;
            }
        }

        [Serializable]
        public class MakeFarmBlockGrow : Task
        {
            BlockLoc toFarm;

            public MakeFarmBlockGrow(BlockLoc nToDestory)
            {
                toFarm = nToDestory;
                taskType = Type.MakeFarmBlockGrow;
            }

            public BlockLoc getBlockToFarm()
            {
                return toFarm;
            }

            public override bool isComplete()
            {
                return true;
            }
        }



        [Serializable]

        public class ObjectBuildForFrame : Task
        {
            IslandGame.GameWorld.CharactersAndAI.ObjectBuildJobSite buildSite;

            public ObjectBuildForFrame(IslandGame.GameWorld.CharactersAndAI.ObjectBuildJobSite nToBuild)
            {
                buildSite = nToBuild;
                taskType = Type.ObjectBuildForFrame;
            }

            public IslandGame.GameWorld.CharactersAndAI.ObjectBuildJobSite getSiteToWorkOn()
            {
                return buildSite;
            }

            public override bool isComplete()
            {
                return true;
            }
        }

        [Serializable]
        public class CaptainBoat : Task
        {
            Boat boat;


            public CaptainBoat(Boat nBoat)
            {
                boat = nBoat;
                taskType = Type.CaptainBoat;
            }

            public override bool isComplete()
            {
                return false;
            }

            public Boat getBoat()
            {
                return boat;
            }
        }

        [Serializable]
        public class GetInBoat : Task
        {
            Boat boat;


            public GetInBoat(Boat nBoat)
            {
                boat = nBoat;
                taskType = Type.GetInBoat;
            }

            public override bool isComplete()
            {
                return true;
            }

            public Boat getBoat()
            {
                return boat;
            }
        }

        [Serializable]
        public class SwitchJob : Task
        {
            Job newJob;


            public SwitchJob(Job nJob)
            {
                newJob = nJob;
                taskType = Type.SwitchJob;
            }

            public override bool isComplete()
            {
                return true;
            }

            public Job getNewJob()
            {
                return newJob;
            }
        }


        [Serializable]
        public class DoStrikeOfWorkAlongRay : Task
        {
            Actor striker;
            Vector3 strikeOrigen;
            float strikeDistance;
            Vector3 strikeDirectionNormal;
            JobType jobType;

            public DoStrikeOfWorkAlongRay(Actor nstriker, Vector3 nStrikeOrigen, float nStrikeDistance, Vector3 nStrikeDirectionNormal)
            {
                taskType = Type.DoStrikeOfWorkAlongRay;
                jobType = JobType.combat;
                striker = nstriker;
                strikeOrigen = nStrikeOrigen;
                strikeDistance = nStrikeDistance;
                strikeDirectionNormal = Vector3.Normalize(nStrikeDirectionNormal);
            }

            public Actor getStriker()
            {
                return striker;
            }

            public Vector3 getStrikeOrigen()
            {
                return strikeOrigen;
            }

            public Vector3 getStrikeDirectionNormal()
            {
                return strikeDirectionNormal;
            }

            public float getStrikeDistance()
            {
                return strikeDistance;
            }

            public override bool isComplete()
            {
                return true;
            }
        }


        public class DoStrikeOfWorkOnBlock : Task
        {
            Actor striker;
            BlockLoc blockToStrike;
            JobType jobType;

            public DoStrikeOfWorkOnBlock(Actor nstriker, BlockLoc nBlockToStrike, JobType nJobType)
            {
                taskType = Type.DoStrikeOfWorkOnBlock;
                jobType = nJobType;
                striker = nstriker;
                blockToStrike = nBlockToStrike;
            }

            public Actor getStriker()
            {
                return striker;
            }

            public BlockLoc getBlockToStrike()
            {
                return blockToStrike;
            }
            public override bool isComplete()
            {
                return true;
            }

            public JobType getJobType()
            {
                return jobType;
            }

        }

        public class PlaceResource : Task
        {
            BlockLoc blockToPlaceIn;
            ResourceBlock.ResourceType typeToPlace;

            public PlaceResource(BlockLoc nBlockToPlaceIn, ResourceBlock.ResourceType ntypeToPlace)
            {
                taskType = Type.PlaceResource;
                typeToPlace = ntypeToPlace;
                blockToPlaceIn = nBlockToPlaceIn;
            }



            public BlockLoc getLocToPlaceResource()
            {
                return blockToPlaceIn;
            }
            public override bool isComplete()
            {
                return true;
            }

            public ResourceBlock.ResourceType getTypeToPlace()
            {
                return typeToPlace;
            }

        }

        public class PickUpResourceBlock : Task
        {
            BlockLoc blockToPickUp;
            ResourceBlock.ResourceType typeToPickUp;

            public PickUpResourceBlock(BlockLoc nBlockToPickUpFrom, ResourceBlock.ResourceType ntypeToPickUp)
            {
                taskType = Type.PlaceResource;
                typeToPickUp = ntypeToPickUp;
                blockToPickUp = nBlockToPickUpFrom;
            }



            public BlockLoc getLocToPickUpFrom()
            {
                return blockToPickUp;
            }
            public override bool isComplete()
            {
                return true;
            }

            public ResourceBlock.ResourceType getTypeToPickUp()
            {
                return typeToPickUp;
            }

        }
    }
}
