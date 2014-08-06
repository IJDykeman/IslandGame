using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    public enum ActorActions
    {
        addToVelocity,
        placeBoat,
        rightClickAction,
        addToShipVelocity,
        die,
        strike,
        PlaceResource,
        PickUpResource,
        placeBlock


    }

    [Serializable]
    public class ActorAction
    {
        public ActorActions type;
    }

    [Serializable]
    public class ActorAddToVelocityAction : ActorAction
    {
        public AxisAlignedBoundingBox currentAABB;
        public Vector3 velocityAddition;
        public Actor character;
        private bool footPropelled;

        public ActorAddToVelocityAction(AxisAlignedBoundingBox nCurrentAABB, Vector3 nMoveBy, Actor ncharacter, bool nIsFootPropelled)
        {
            type = ActorActions.addToVelocity;
            currentAABB = nCurrentAABB;
            velocityAddition = nMoveBy;
            character = ncharacter;
            footPropelled = nIsFootPropelled;
        }

        public bool isFootPropelled() 
        {
            return footPropelled;
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
    public class ActorPlaceBlockAction : ActorAction
    {
        BlockLoc whereToPlaceBlock;
        byte blockType;

        public ActorPlaceBlockAction(BlockLoc nwhereToPlaceBlock, byte nBlockType)
        {
            type = ActorActions.placeBlock;
            whereToPlaceBlock = nwhereToPlaceBlock;
            blockType = nBlockType;
        }

        public BlockLoc getlocToPlaceBlock()
        {
            return whereToPlaceBlock;
        }

        public byte getTypeToPlace()
        {
            return blockType;
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
    public class ActorAddToShipVelocity : ActorAction
    {
        Boat boat;
        Vector3 toAddToVelocity;

        public ActorAddToShipVelocity(Boat nBoat, Vector3 nToAdd)
        {
            type = ActorActions.addToShipVelocity;
            boat = nBoat;
            toAddToVelocity = nToAdd;
        }

        public Boat getBoat()
        {
            return boat;
        }

        public Vector3 getVelocityAddition()
        {
            return toAddToVelocity;
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




    [Serializable]
    public class ActorPlaceResourceAction : ActorAction
    {
        ResourceBlock.ResourceType typeToPlace;
        BlockLoc locToPlace;

        public ActorPlaceResourceAction(BlockLoc nlocToPlace, ResourceBlock.ResourceType nTypeToPlace)
        {
            type = ActorActions.PlaceResource;
            typeToPlace = nTypeToPlace;
            locToPlace = nlocToPlace;
        }

        public ResourceBlock.ResourceType getRescourceTypeToPlace()
        {
            return typeToPlace;
        }

        public BlockLoc getLocToPlace()
        {
            return locToPlace;
        }
    }

    [Serializable]
    public class ActorPickUpResourceAction : ActorAction
    {
        ResourceBlock.ResourceType typeToPlace;
        BlockLoc locToPlace;

        public ActorPickUpResourceAction(BlockLoc nlocToPlace, ResourceBlock.ResourceType nTypeToPlace)
        {
            type = ActorActions.PickUpResource;
            typeToPlace = nTypeToPlace;
            locToPlace = nlocToPlace;
        }

        public ResourceBlock.ResourceType getRescourceTypeToPlace()
        {
            return typeToPlace;
        }

        public BlockLoc getLocToPlace()
        {
            return locToPlace;
        }
    }

    [Serializable]
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
        //this class stores strike information and allows the striker to be told what the results
        //of its action are

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

        public void notifyThatActionHasDestroyedBlock()
        {
            if (striker is Character)
            {
                Character toNotify = (Character)striker;
                toNotify.pickUpItem(ResourceBlock.ResourceType.Stone);
            }
        }

        public void notifyThatActionHasPlacedBlock()
        {
            if (striker is Character)
            {
                Character toNotify = (Character)striker;
                toNotify.dropLoad();
            }
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
