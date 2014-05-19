using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace IslandGame.GameWorld
{
    [Serializable] 
    public class ChunkSpace
    {
        //holds a group of painted cube spaces as chunks
        //displays chunks

        public static int chunkWidth = 32;
        public static int chunkHeight = 64;

        Chunk[,] chunks;

        public static int widthInChunksStaticForGallery = 8;
        public int widthInChunks = widthInChunksStaticForGallery;
        

        Vector3 location;

        public ChunkSpace(Vector3 nLoc)
        {
            location = nLoc;
            chunks = new Chunk[widthInChunks, widthInChunks];
            for (int x = 0; x < widthInChunks; x++)
            {
                for (int z = 0; z < widthInChunks; z++)
                {
                    chunks[x, z] = new Chunk(chunkWidth,chunkHeight,x*chunkWidth,z*chunkWidth);
                }
            }
        }

        public void display(GraphicsDevice device, Effect effect)
        {
            for (int x = 0; x < widthInChunks; x++)
            {
                for (int z = 0; z < widthInChunks; z++)
                {
                    chunks[x, z].display(device, effect, location);
                }
            }
        }

        public void updateAllMeshes(int mipLevel)
        {
            for (int x = 0; x < widthInChunks; x++)
            {
                for (int z = 0; z < widthInChunks; z++)
                {
                    chunks[x, z].updateMesh(mipLevel);
                }
            }
        }

        public void forceUpdateAllMeshes()
        {
            for (int x = 0; x < widthInChunks; x++)
            {
                for (int z = 0; z < widthInChunks; z++)
                {
                    chunks[x, z].forceUpdateMesh();
                }
            }
        }

        public void saveAllChunks()
        {
            
        }

        public void fillAllChunksWithZeroes()
        {
            for (int x = 0; x < chunkWidth * widthInChunks; x++)
            {
                for (int z = 0; z < chunkWidth * widthInChunks; z++)
                {
                    for (int y = 0; y < chunkHeight; y++)
                    {
                        setBlockAt(PaintedCubeSpace.AIR, x, y, z);
                    }
                }
            }
        }

        public void setBlockAt(byte type, int x, int y, int z)
        {
            
            chunks[(int)(x / chunkWidth), (int)(z / chunkWidth)].setBlockAt(type, x % chunkWidth, y, z % chunkWidth);
        }

        public byte getChunkSpaceBlockAtWithoutWithinCheck(int x, int y, int z)
        {
            return chunks[(int)(x / (float)chunkWidth), (int)(z / (float)chunkWidth)].getChunkBlockAt( x % chunkWidth, y, z % chunkWidth);
        }

        public bool isChunkSpaceSolidAt(int x, int y, int z)
        {

            if (withinChunkSpaceInChunkSpace(x, y, z))
            {
                byte block = getChunkSpaceBlockAtWithoutWithinCheck(x, y, z);
                bool result = PaintedCubeSpace.isSolidType(block);
                return result;
            }
            return false;
        }

        public bool isChunkSpaceSolidAt(IntVector3 intloc)
        {
            return isChunkSpaceSolidAt(intloc.X, intloc.Y, intloc.Z);
        }

        public bool isChunkSpaceSolidAt(BlockLoc loc)
        {
            return isChunkSpaceSolidAt(loc.toISIntVec3(new IslandPathingProfile(this)));
        }

        public int getWidth()
        {
            return chunkWidth * widthInChunks;
        }

        public int getHeight()
        {
            return chunkHeight;
        }

        public bool[, ,] getWaterArray()
        {
            bool[, ,] result = new bool[chunkWidth * widthInChunks, chunkHeight, chunkWidth * widthInChunks];
            for (int x = 0; x < chunkWidth * widthInChunks; x++)
            {
                for (int z = 0; z < chunkWidth * widthInChunks; z++)
                {
                    for (int y = 0; y < chunkHeight; y++)
                    {
                        result[x, y, z] = getChunkSpaceBlockAtWithoutWithinCheck(x, y, z) == PaintedCubeSpace.WATER;
                    }
                }
            }
            return result;
        }

        public Vector3 getCenter()
        {
            return location + new Vector3(widthInChunks * chunkWidth / 2.0f, chunkHeight / 2.0f, widthInChunks * chunkWidth / 2.0f);
        }



        public bool withinChunkSpaceInChunkSpace(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < widthInChunks * chunkWidth && y < chunkHeight && z < widthInChunks * chunkWidth;
        }

        public BoundingBox getBoundingBox()
        {
            return new BoundingBox(location, location + new Vector3(chunkWidth * widthInChunks, chunkHeight, chunkWidth * widthInChunks));
        }

        public Vector3? getNearestBlockAlongRayInChunkSpaceSpace(Ray ray)
        {
            

            Vector3? result = null;
            float minDist = float.MaxValue;
            for (int x = 0; x < widthInChunks; x++)
            {
                for (int z = 0; z < widthInChunks; z++)
                {


                    Vector3? hitLoc = chunks[x,z].getNearestBlockAlongRayFromInsideChunkSpaceContext(ray);

                    if(hitLoc.HasValue)
                    {
                        if (Vector3.Distance(ray.Position, (Vector3)hitLoc) < minDist)
                        {
                            result = hitLoc;
                            minDist = Vector3.Distance(ray.Position, (Vector3)hitLoc);
                        }
                    }

                }
            }

            return result;
        }

        public Vector3 worldSpaceToChunkSpaceSpace(Vector3 input)
        {
            Vector3 output = new Vector3(input.X, input.Y, input.Z);
            output -= location;
            return output;
        }

        public Vector3 chunkSpaceToWorldSpace(Vector3 input)
        {
            Vector3 output = new Vector3(input.X, input.Y, input.Z);
            output += location;
            return output;
        }

        public Vector3 getLocation()
        {
            return location;
        }

        public void setBlockAtWithMeshUpdate(byte type, IntVector3 loc)
        {
            Chunk toModify = chunks[(int)((float)loc.X / (float)chunkWidth), (int)((float)loc.Z / (float)chunkWidth)];
            if (getChunkSpaceBlockAtWithoutWithinCheck(loc.X, loc.Y, loc.Z) != type)
            {
                toModify.setBlockAt(type, loc.X % chunkWidth, loc.Y, loc.Z % chunkWidth);
                toModify.forceUpdateMesh();
            }
        }

        internal byte? getBlockAt(ref BlockLoc loc)
        {
            IslandPathingProfile profile = new IslandPathingProfile(this);
            if (withinChunkSpaceInChunkSpace(loc.ISX(profile), loc.ISY(profile), loc.ISZ(profile)))
            {
                return getChunkSpaceBlockAtWithoutWithinCheck(loc.ISX(profile), loc.ISY(profile), loc.ISZ(profile));
            }
            else
            {
                return null;
            }
        }
    }
}
