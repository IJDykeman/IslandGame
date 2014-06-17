﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CubeAnimator;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class Boat : Actor
    {

        public static readonly float SCALE = .2f;
        float speed = .16f;
        float permanentYLocation= .4f;


        public Boat(Vector3 nLoc)
        {
            setupAnimatedBodyPartGroup ( ContentDistributor.getRootPath()+@"boats\onePersonBoat.chr", SCALE);
            nLoc.Y = permanentYLocation;
            physics = new PhysicsHandler(new AxisAlignedBoundingBox(new Vector3(-1, 0,-1)+nLoc, new Vector3(1f, .1f, 1f)+nLoc));
            faction = Faction.neutral;
            
        }

        public override List<ActorAction> update(CharacterTaskTracker taskTracker)
        {
            
            List<ActorAction> actions = new List<ActorAction>();
            actions.Add(getMoveToActionWithMoveByVector(getVelocity()));
            physics.AABB.loc.Y = permanentYLocation;
            setRootPartLocation(physics.AABB.middle());
            setRootPartRotationOffset(getYRotationFromDeltaVector(getVelocity()));
           return actions;
        }

        public override BlockLoc? blockClaimedToWorkOn()
        {
            return null;
        }

        public override Job getJobWhenClicked(Character nWorker, IslandPathingProfile profile, ActorStateProfile actorState)
        {
            IslandGame.GameWorld.CharactersAndAI.CompleteTaskJob getInBoat = new 
                IslandGame.GameWorld.CharactersAndAI.CompleteTaskJob(nWorker, new CharacterTask.GetInBoat(this));

            List<BlockLoc>path = new PathHandler().
                getPathToSingleBlock(profile,new BlockLoc(nWorker.getFootLocation()),profile,new BlockLoc(getFootLocation()),2);

            TravelAlongPath travel = new TravelAlongPath(path,getInBoat);

            return travel;
        }


        internal float getSpeed()
        {
            return speed;
        }
    }
}
