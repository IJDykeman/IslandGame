using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class IslandPathingProfile : IslandLocationProfile
    {


        protected static IntVector3[] possibleMoves = 
        {new IntVector3(1,0,0),new IntVector3(-1,0,0),new IntVector3(0,0,1),new IntVector3(0,0,-1),
            new IntVector3(1,0,1),new IntVector3(-1,0,-1),new IntVector3(-1,0,1),new IntVector3(1,0,-1),
            new IntVector3(1,1,0),new IntVector3(-1,1,0),new IntVector3(0,1,1),new IntVector3(0,1,-1),
            new IntVector3(1,-1,0),new IntVector3(-1,-1,0),new IntVector3(0,-1,1),new IntVector3(0,-1,-1)};

        protected static IntVector3[] availableBlocksFromGivenStadingLoc = 
        {   new IntVector3(1,-1,0),new IntVector3(-1,-1,0),new IntVector3(0,-1,1),new IntVector3(0,-1,-1),
            new IntVector3(1,0,0), new IntVector3(-1,0,0), new IntVector3(0,0,1), new IntVector3(0,0,-1),
            new IntVector3(1,1,0), new IntVector3(-1,1,0), new IntVector3(0,1,1), new IntVector3(0,1,-1),
            new IntVector3(1,2,0), new IntVector3(-1,2,0), new IntVector3(0,2,1), new IntVector3(0,2,-1)
        };


        protected static IntVector3[] standingLocsRelativeToAvailableBlock = 
        {   
            new IntVector3(1,1,0), new IntVector3(-1,1,0), new IntVector3(0,1,1), new IntVector3(0,1,-1),
            new IntVector3(1,0,0), new IntVector3(-1,0,0), new IntVector3(0,0,1), new IntVector3(0,0,-1),
            new IntVector3(1,-1,0),new IntVector3(-1,-1,0),new IntVector3(0,-1,1),new IntVector3(0,-1,-1)
            
            
        };



        public IslandPathingProfile()
        {
        }

        public IslandPathingProfile(Island nIsland)
        {
            island = nIsland;
        }

        public virtual bool isProfileSolidAtWithWithinCheck(BlockLoc loc)
        {
            if (withinIsland(loc.toISIntVec3(this)))
            {
                return island.isChunkSpaceSolidAt(loc);
            }

            return false;
        }

        public List<IntVector3> profileSpaceListToWorldSpaceList(List<IntVector3> path)
        {
            IntVector3 islandLoc = new IntVector3(island.getLocation());
            for(int i=0;i<path.Count;i++)
            {
                path[i] += islandLoc;
            }
            return path;
        }

        public int getIslandSpaceLocationX(int worldSpaceX)
        {

            return worldSpaceX -(int)island.getLocation().X;
        }

        public int getIslandSpaceLocationY(int worldSpaceY)
        {
            return worldSpaceY - (int)island.getLocation().Y;
        }

        public int getIslandSpaceLocationZ(int worldSpaceZ)
        {
            return worldSpaceZ - (int)island.getLocation().Z;
        }

        public bool isStandableAtWithHeight(BlockLoc IslandSpace, int entityHeight)
        {
            BlockLoc underFoot = new BlockLoc(IslandSpace.WSX(), IslandSpace.WSY() - 1, IslandSpace.WSZ());//locInPath + new IntVector3(0, -1, 0);
            
            if (!isProfileSolidAtWithWithinCheck(IslandSpace) && isInProfileScope(IslandSpace))
            {
                if (isProfileSolidAtWithWithinCheck(underFoot))
                {

                    for (int i = 1; i < entityHeight; i++)
                    {
                        if (isProfileSolidAtWithWithinCheck(IslandSpace.getVector3WithAddedIntvec( new IntVector3(0, i, 0))))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool isStandableAtExactLocation(AxisAlignedBoundingBox AABB)
        {
            if (!couldStandExactlyAt(AABB))
            {
                return false;
            }

            for (int x = (int)AABB.loc.X; x < AABB.loc.X + AABB.Xwidth; x++)
            {
                for (int z = (int)AABB.loc.Z; z < AABB.loc.Z + AABB.Zwidth; z++)
                {
                    if (isProfileSolidAtWithWithinCheck(new BlockLoc(x, (int)(AABB.loc.Y-.01f), z)))
                    {
                        return true; //return true now if entity has a block under it.
                    }
                }
            }
            return false;//if no blocks are under entity

        }

        public bool couldStandExactlyAt(AxisAlignedBoundingBox AABB)
        {
            for (int x = (int)AABB.loc.X; x < AABB.loc.X + AABB.Xwidth; x++)
            {
                for (int y = (int)AABB.loc.Y; y < AABB.loc.Y + AABB.height; y++)
                {
                    for (int z = (int)AABB.loc.Z; z < AABB.loc.Z + AABB.Zwidth; z++)
                    {
                        if (isProfileSolidAtWithWithinCheck(new BlockLoc(x, y, z)))
                        {
                            return false; //return false if entity is intersecting a block
                        }
                    }
                }
            }
            return true;
        }

        public bool isSwimableAtWithHeight(BlockLoc IslandSpace, int entityHeight)
        {
            
            if (!isProfileSolidAtWithWithinCheck(IslandSpace))
            {
                if (IslandSpace.WSY()==0)
                {

                    for (int i = 1; i < entityHeight; i++)
                    {
                        if (isProfileSolidAtWithWithinCheck(IslandSpace.getVector3WithAddedIntvec(new IntVector3(0, i, 0))))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool isSwimableAtWithHeightAndWithinIsland(BlockLoc loc, int entityHeight)
        {

            return isSwimableAtWithHeight(loc, entityHeight) && isInProfileScope(loc);
        }

        private bool withinIsland(IntVector3 loc)
        {
            return island.withinChunkSpaceInChunkSpace((int)loc.X, (int)loc.Y, (int)loc.Z);
        }

        protected virtual bool isInProfileScope(BlockLoc loc)
        {
            return island.withinChunkSpaceInChunkSpace((int)loc.toISIntVec3(this).X, (int)loc.toISIntVec3(this).Y, (int)loc.toISIntVec3(this).Z);
        }

        public List<BlockLoc> getSpacesThatCanBeMovedToFrom(BlockLoc from, int entityHeight)
        {
            List<BlockLoc> result = new List<BlockLoc>(8);
            foreach (IntVector3 move in possibleMoves)
            {
                if (isStandableAtWithHeight(BlockLoc.AddIntVec3(from, move), entityHeight) || isSwimableAtWithHeightAndWithinIsland(BlockLoc.AddIntVec3(from, move), entityHeight))
                {
                    result.Add(BlockLoc.AddIntVec3(from, move));
                }
            }
            return result;
        }

        public HashSet<BlockLoc> getFootLocsThatHaveAccessToBlock(BlockLoc goalBlockLoc)
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (IntVector3 relativeFootLoc in standingLocsRelativeToAvailableBlock)
            {
                result.Add(BlockLoc.AddIntVec3( goalBlockLoc,relativeFootLoc));
            }
            return result;
        }

        public HashSet<BlockLoc> getBlocksAvailableForWorkFromFootLoc(BlockLoc footLoc)
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (IntVector3 relativeAvailableLoc in availableBlocksFromGivenStadingLoc)
            {
                result.Add(BlockLoc.AddIntVec3(footLoc, relativeAvailableLoc));
            }
            return result;
        }

        public HashSet<BlockLoc> getStandableFootLocsThatHaveAccessToBlock(BlockLoc goalBlockLoc, int entityHeight)
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            HashSet<BlockLoc> allFootLocs = getFootLocsThatHaveAccessToBlock(goalBlockLoc);
            foreach (BlockLoc potentiallyStandable in allFootLocs)
            {
                if (isStandableAtWithHeight(potentiallyStandable, entityHeight))
                {
                    result.Add(potentiallyStandable);
                }
            }
            return result;
        }

        public IntVector3 getRandomMove() 
        {
            IntVector3 pick = possibleMoves[new Random().Next(possibleMoves.Length)];
            return new IntVector3(pick.X, pick.Y, pick.Z);
        }


        public bool isActorStanding(Actor actor)
        {
            for(int x=(int)(actor.getFootLocation().X-actor.getXWidth()) ; x<actor.getFootLocation().X+actor.getXWidth();x++)
            {
                for (int z = (int)(actor.getFootLocation().Z - actor.getZWidth()); z < actor.getFootLocation().Z + actor.getXWidth(); z++)
                {
                    if (isProfileSolidAtWithWithinCheck(new BlockLoc( x, (int)(actor.getFootLocation().Y - .01f), z)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
