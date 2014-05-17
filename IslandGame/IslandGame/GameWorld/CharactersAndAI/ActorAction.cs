using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    public enum ActorActions
    {
        moveTo,
        placeBoat,
        rightClickAction,
        setShipVelocity,
        die,
        strike,

    }

    [Serializable]
    public class ActorAction
    {
        public ActorActions type;
    }

    [Serializable]
    public class ActorMoveToAction : ActorAction
    {
        public AxisAlignedBoundingBox currentAABB;
        public AxisAlignedBoundingBox desiredAABB;
        public Actor character;

        public ActorMoveToAction(AxisAlignedBoundingBox nCurrentAABB, AxisAlignedBoundingBox nDesiredAABB, Actor ncharacter)
        {
            type = ActorActions.moveTo;
            currentAABB = nCurrentAABB;
            desiredAABB = nDesiredAABB;
            character = ncharacter;
        }
    }

    [Serializable]
    public class ActorPlaceBoatAction : ActorAction
    {
        public BlockLoc toPlaceBoatAt;


        public ActorPlaceBoatAction(BlockLoc newBoatLoc)
        {
            type = ActorActions.placeBoat;
            toPlaceBoatAt = newBoatLoc;
        }

        public BlockLoc getBoatLocToPlace()
        {
            return toPlaceBoatAt;
        }
    }

    [Serializable]
    public class ActorRightClickAction : ActorAction
    {
        public Vector3 nearClickPoint;
        public Vector3 farClickPoint;
        public Character character;

        public ActorRightClickAction(Vector3 nNearClickPoint, Vector3 nFarClickPoint, Character nCharacter)
        {
            type = ActorActions.rightClickAction;
            nearClickPoint = nNearClickPoint;
            farClickPoint = nFarClickPoint;
            character = nCharacter;
        }
    }

    [Serializable]
    public class ActorSetShipVelocity : ActorAction
    {
        Boat boat;
        Vector3 newVelocity;

        public ActorSetShipVelocity(Boat nBoat, Vector3 nNewVelocity)
        {
            type = ActorActions.setShipVelocity;
            boat = nBoat;
            newVelocity = nNewVelocity;
        }

        public Boat getBoat()
        {
            return boat;
        }

        public Vector3 getNewVelocity()
        {
            return newVelocity;
        }
    }

    [Serializable]
    public class ActorDieAction : ActorAction
    {
        Actor toDie;

        public ActorDieAction(Actor nToDie)
        {
            type = ActorActions.die;
            toDie = nToDie;
        }

        public Actor getActorToBeKilled()
        {
            return toDie;
        }

    }

    public abstract class ActorStrikeAction : ActorAction
    {
        protected Actor striker;
        public Actor getStriker()
        {
            return striker;
        }

        public enum StrikeType
        {
            AlongRay, OnBlock
        }
        protected JobType jobType;
        public JobType getJobType()
        {
            return jobType;
        }

        protected StrikeType strikeType;
        public StrikeType getStrikeType()
        {
            return strikeType;
        }

        public abstract Vector3 getLookDeltaVec();
    }

    [Serializable]
    public class ActorStrikeAlongRayAction : ActorStrikeAction
    {
        
        Vector3 strikeOrigen;
        float strikeDistance;
        Vector3 strikeDirectionNormal;


        public ActorStrikeAlongRayAction(Actor nstriker, Vector3 nStrikeOrigen, float nStrikeDistance, Vector3 nStrikeDirectionNormal, JobType nJobType)
        {
            type = ActorActions.strike;
            jobType = nJobType;
            strikeType = StrikeType.AlongRay;
            striker = nstriker;
            strikeOrigen = nStrikeOrigen;
            strikeDistance = nStrikeDistance;
            strikeDirectionNormal = Vector3.Normalize( nStrikeDirectionNormal);
        }

        public override Vector3 getLookDeltaVec()
        {
            return strikeDirectionNormal;
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


    }

    [Serializable]
    public class ActorStrikeBlockAction : ActorStrikeAction
    {
        
        BlockLoc toStrike;


        public ActorStrikeBlockAction(Actor nstriker, BlockLoc ntoStrike, JobType nJobType)
        {
            type = ActorActions.strike;
            jobType = nJobType;
            strikeType = StrikeType.OnBlock;
            striker = nstriker;
            toStrike = ntoStrike;
        }

        public override Vector3 getLookDeltaVec()
        {
            return toStrike.getMiddleInWorldSpace() - getStriker().getLocation();
        } 

        

        public BlockLoc getStrikeTarget()
        {
            return toStrike;
        }



    }



}
