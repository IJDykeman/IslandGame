using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public abstract class Actor : CubeAnimator.AnimatedBodyPartGroup
    {
        protected PhysicsHandler physics;
        protected Faction faction = Faction.neutral;
        float health = 10;

        public enum Faction
        {
            friendly,
            enemy,
            neutral
        }

        public abstract List<ActorAction> update(CharacterTaskTracker taskTracker);

        public BoundingBox getBoundingBox()
        {
            return new BoundingBox(physics.AABB.loc, physics.AABB.max());
        }

        public void setVelocity(Vector3 nVelocity)
        {
            physics.velocity = nVelocity;
        }

        public Vector3 getVelocity()
        {
            return physics.velocity;
        }

        public void setAABB(AxisAlignedBoundingBox nAABB)
        {
            physics.AABB = nAABB;
        }

        public Vector3 getLocation()
        {
            return physics.AABB.middle();
        }

        public Vector3 getFootLocation()
        {
            return physics.AABB.loc + new Vector3(physics.AABB.Xwidth / 2f, 0, physics.AABB.Zwidth / 2f);
        }

        public void setFootLocation(Vector3 nFootLoc)
        {
            setAABBCenterLocation(nFootLoc + new Vector3(0, physics.AABB.height / 2.0f, 0));
        }

        void setAABBCenterLocation(Vector3 nLoc)
        {
            physics.AABB.loc += (nLoc - physics.AABB.middle());
        }

        public virtual ActorAction getMoveToActionWithMoveByVector(Vector3 moveBy)
        {
            return getActorMoveToActionFromDeltaVec(ref moveBy);
        }

        protected ActorAction getActorMoveToActionFromDeltaVec(ref Vector3 moveBy)
        {
            Vector3 halfAABBWidthVec = new Vector3(physics.AABB.Xwidth / 2f, physics.AABB.height / 2f, physics.AABB.Zwidth / 2f);

            AxisAlignedBoundingBox currentAABB = new AxisAlignedBoundingBox(physics.AABB.middle()
                - halfAABBWidthVec, physics.AABB.middle() + halfAABBWidthVec);

            AxisAlignedBoundingBox desiredAABB = new AxisAlignedBoundingBox(physics.AABB.middle()
                - halfAABBWidthVec + moveBy, physics.AABB.middle() + halfAABBWidthVec + moveBy);

            return new ActorMoveToAction(currentAABB,
                desiredAABB, this);
        }

        protected Quaternion getYRotationFromDeltaVector(Vector3 delta)
        {
            //delta = new Vector3((float)Math.Cos(age / 20f), 0, (float)Math.Sin(age / 20f));

            delta.Y = 0;
            delta.Normalize();
            Vector2 floorDelta = new Vector2(delta.X, delta.Z);
            if (floorDelta.Length() > 0)
            {
                //floorDelta.Normalize();
            }
            else
            {
                return getWholeBodyRotation();
            }
            floorDelta.Normalize();

            float angle = -(float)Math.Atan2(floorDelta.Y, floorDelta.X) - (float)Math.PI / 2.0f;

            Quaternion current = getWholeBodyRotation();
            Quaternion goal = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY(angle));



            return Quaternion.Slerp(current, goal, .25f);



            /*Matrix matrix = Matrix.Identity;
            matrix.Forward = delta;
            matrix.Right = Vector3.Normalize(Vector3.Cross(matrix.Forward, Vector3.Up));
            matrix.Up = Vector3.Cross(matrix.Right, matrix.Forward);

            return Quaternion.CreateFromRotationMatrix(matrix);*/
        }

        public void setRotationWithGivenDeltaVec(Vector3 delta)
        {
            setRootPartRotationOffset(getYRotationFromDeltaVector(delta));
        }

        public void setRotationToLookAt(Vector3 lookTarget)
        {
            setRotationWithGivenDeltaVec(lookTarget - getLocation());
        }

        protected void runPhysics(List<ActorAction> actions)
        {
            physics.gravitate();
            actions.Add(getMoveToActionWithMoveByVector(physics.velocity));
        }

        public abstract BlockLoc? blockClaimedToWorkOn();

        public virtual Job getJobWhenClicked(Character nWorker, IslandPathingProfile profile, ActorStateProfile nActorProfile)
        {
            return new UnemployedJob();
        }


        public Faction getFaction()
        {
            return faction;
        }

        public void damage(float damage)
        {
            health -= damage;
        }

        public bool isDead()
        {
            return health <= 0;
        }

        public static bool areHostile(Actor.Faction faction1, Actor.Faction faction2)
        {
            if (faction1 == Faction.friendly && faction2 == Faction.enemy)
            {
                return true;
            }
            else if (faction1 == Faction.enemy && faction2 == Faction.friendly)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Faction getFactionHostileTo(Faction test)
        {
            if (test == Faction.friendly || test == Faction.neutral)
            {
                return Faction.enemy;
            }
            else
            {
                return Faction.friendly;
            }
              
        }

    }
}
