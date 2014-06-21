using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IslandGame.GameWorld;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    [Serializable]
    public struct BlockLoc
    {
        private int worldSpaceX;
        private int worldSpaceY;
        private int worldSpaceZ;

        public BlockLoc(int nWorldSpaceX, int nWorldSpaceY, int nWorldSpaceZ)
        {
            worldSpaceX = nWorldSpaceX;
            worldSpaceY = nWorldSpaceY;
            worldSpaceZ = nWorldSpaceZ;
        }

        public BlockLoc(Vector3 WorldSpaceVec)
        {
            worldSpaceX = (int)WorldSpaceVec.X;
            worldSpaceY = (int)WorldSpaceVec.Y;
            worldSpaceZ = (int)WorldSpaceVec.Z;

            if (WorldSpaceVec.X < 0)
            {
                worldSpaceX--;
            }
            if (WorldSpaceVec.Y < 0)
            {
                worldSpaceY--;
            }
            if (WorldSpaceVec.Z < 0)
            {
                worldSpaceZ--;
            }
        }

        public void setValuesInWorldSpace(int nWorldSpaceX, int nWorldSpaceY, int nWorldSpaceZ)
        {
            worldSpaceX = nWorldSpaceX;
            worldSpaceY = nWorldSpaceY;
            worldSpaceZ = nWorldSpaceZ;
        }

        public Vector3 toWorldSpaceVector3()
        {
            return new Vector3(worldSpaceX, worldSpaceY, worldSpaceZ);
        }

        public Vector3 getMiddleInWorldSpace()
        {
            return new Vector3(worldSpaceX + .5f, worldSpaceY + .5f, worldSpaceZ + .5f);

        }

        public int WSX()
        {
            return worldSpaceX;
        }

        public int WSY()
        {
            return worldSpaceY;
        }

        public int WSZ()
        {
            return worldSpaceZ;
        }

        public int ISX(IslandPathingProfile profile)
        {
            return profile.getIslandSpaceLocationX(worldSpaceX);
        }

        public int ISY(IslandPathingProfile profile)
        {
            return profile.getIslandSpaceLocationY(worldSpaceY);
        }

        public int ISZ(IslandPathingProfile profile)
        {
            return profile.getIslandSpaceLocationZ(worldSpaceZ);
        }

        public IntVector3 toISIntVec3(IslandPathingProfile profile) 
        {
            return new IntVector3(ISX(profile), ISY(profile), ISZ(profile));
        }

        public BlockLoc getVector3WithAddedIntvec(IntVector3 islandVecSpaceToAdd)
        {
            return new BlockLoc(worldSpaceX + islandVecSpaceToAdd.X, worldSpaceY + islandVecSpaceToAdd.Y, worldSpaceZ + islandVecSpaceToAdd.Z);
        }

        public static BlockLoc AddIntVec3(BlockLoc loc1, IntVector3 toAdd)
        {
            return new BlockLoc(loc1.WSX() + toAdd.X, loc1.WSY() + toAdd.Y, loc1.WSZ() + toAdd.Z);
        }

        public int getHashCode()
        {
            string toHash = "X:" + worldSpaceX + "Y:" + worldSpaceY + "Z:" + worldSpaceZ;
            return toHash.GetHashCode();
        }

        public override bool Equals(System.Object obj)
    
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is BlockLoc)
            {
                BlockLoc value2 = (BlockLoc)obj;
                return (worldSpaceX == value2.worldSpaceX &&
                    worldSpaceY == value2.worldSpaceY &&
                    worldSpaceZ == value2.worldSpaceZ);
            }
            else
            {
                return false;
            }


            
        }

        public static bool operator ==(BlockLoc value1, BlockLoc value2)
        {
            return value1.worldSpaceX == value2.worldSpaceX &&
                value1.worldSpaceY == value2.worldSpaceY &&
                value1.worldSpaceZ == value2.worldSpaceZ;
        }

        public static bool operator !=(BlockLoc value1, BlockLoc value2)
        {
            return value1.worldSpaceX != value2.worldSpaceX ||
                value1.worldSpaceY != value2.worldSpaceY ||
                value1.worldSpaceZ != value2.worldSpaceZ;
        }

        public float? intersects(Ray ray)
        {

            BoundingBox boundingBox = new BoundingBox(
                getMiddleInWorldSpace() - new Vector3(.5f, .5f, .5f), getMiddleInWorldSpace() + new Vector3(.5f, .5f, .5f));
            return ray.Intersects(boundingBox);

        }

        public override int GetHashCode()
        {
            return ("" + worldSpaceX + worldSpaceY + worldSpaceZ).GetHashCode();
        }
    }
}
