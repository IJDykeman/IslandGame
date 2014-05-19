using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace IslandGame.GameWorld
{
    [Serializable]
    class Chunk
    {
        PaintedCubeSpace space;


        public Chunk(int nChunkWidth, int nChunkHeight, int xOffset, int zOffset)
        {

            space = new PaintedCubeSpace(nChunkWidth, nChunkHeight, new Vector3(xOffset, 0 ,zOffset));
            

        }

        public void setBlockAt(byte type, int x, int y, int z)
        {
            space.array[x, y, z] = type;
        }
        
        public void updateMesh(int mipLevel)
        {

            if (!space.canBeDrawn())
            {
                space.createModel(Compositer.device);
            }
            space.setMipLevel(mipLevel);

        }
        
        public void forceUpdateMesh()
        {
            space.createModel(Compositer.device);
        }

        public bool needsMeshUpdate()
        {
            return space.canBeDrawn();
        }



        byte getMostAbundantBlockType(byte[, ,] originalArray, IntVector3 locationOfTestArea, int widthOfTestArea)
        {
            List<byte> types = new List<byte>();
            List<int> numberOfType = new List<int>();

            for (int x = locationOfTestArea.X; x < locationOfTestArea.X + widthOfTestArea; x += 1)
            {
                for (int y = locationOfTestArea.Y; y < locationOfTestArea.Y + widthOfTestArea; y += 1)
                {
                    for (int z = locationOfTestArea.Z; z < locationOfTestArea.Z + widthOfTestArea; z += 1)
                    {
                        if (originalArray[x, y, z] != PaintedCubeSpace.AIR)
                        {
                            if (types.Contains(originalArray[x, y, z]))
                            {
                                numberOfType[types.IndexOf(originalArray[x, y, z])]++;
                            }
                            else
                            {
                                types.Add(originalArray[x, y, z]);
                                numberOfType.Add(1);
                            }
                        }
                    }
                }
            }
            int numberOfMostAbundant = 0;
            byte result = 0;
            for (int i = 0; i < types.Count; i++ )
            {
                if (numberOfType[i] > numberOfMostAbundant)
                {
                    result = types[i];
                }

            }
            return result;
        }




        public void saveChunk(string folderPath)
        {
            space.serializeChunk(folderPath+space.loc.X+","+space.loc.Z);
        }

        public byte getChunkBlockAt(int x, int y, int z)
        {

            return space.array[x, y, z];
        }

        public void display(GraphicsDevice device, Effect effect, Vector3 chunkSpaceLocation)
        {

            if (space.canBeDrawn())
            {
                space.drawForChunk(device, effect, Matrix.CreateTranslation(chunkSpaceLocation));
            }
            

        }

        /*Matrix getOffsetMatrixForMipLevel()
        {

            if (mippedSpace == null)
            {
                return Matrix.Identity;
            }
            return Matrix.CreateTranslation(new Vector3(0, -(int)Math.Pow(2,mippedSpace.mipLevel) + 1, 0));
        }*/

        public Vector3? getNearestBlockAlongRayFromInsideChunkSpaceContext(Ray ray)
        {
            if (ray.Intersects(getBoundingBoxInChunkSpaceConext()).HasValue)
            {

                Vector3? hit = space.getNearestBlockAlongRayFromChunkSpaceContext(ray);

                if(hit.HasValue)
                {
                    hit+=space.loc;
                }
                return hit;
            }
            return null;
        }

        public BoundingBox getBoundingBoxInChunkSpaceConext()
        {
            return new BoundingBox(space.loc, space.loc + new Vector3(space.spaceWidth, space.spaceHeight, space.spaceWidth));
        }


    }
}
