using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    class PaintedCubeSpaceDisplayComponant
    {
        [NonSerialized]
        public VertexPostitionColorPaintNormal[] vertices;
        [NonSerialized]
        public short[] indices; //having this be a short could be causing the chunk complexity limit issue
        [NonSerialized]
        public IndexBuffer indexBuffer;
        [NonSerialized]
        public VertexBuffer vertexBuffer;

        bool readyToBeDisplayed = false;

        public int mipLevel = 0;
        public float scaleMultiplyerForMip = 1;

        public PaintedCubeSpaceDisplayComponant(int nMipLevel)
        {
            mipLevel = nMipLevel;
        }

        public void setBuffers(VertexAndIndexBuffers buffers)
        {
            vertexBuffer = buffers.getVertexBuffer();
            indexBuffer = buffers.getIndexBuffer();
            readyToBeDisplayed = true;
        }

        static byte[,,] getMippedArray(int mipLevel, int spaceWidth, int spaceHeight, byte[,,] unmippedArray)
        {

            int scaleDivisor = (int)Math.Pow(2, mipLevel);
            int mippedWidth = spaceWidth / scaleDivisor;
            int mippedHeight = spaceHeight / scaleDivisor;
            if (mippedWidth <1)
            {
                mippedWidth = 1;
            }
            if (mippedHeight < 1)
            {
                mippedHeight = 1;
            }

            byte[, ,] mippedArray = new byte[mippedWidth, mippedHeight, mippedWidth];


            for (int x = 0; x < spaceWidth ; x += scaleDivisor)
            {
                for (int y = 0; y < spaceHeight; y += scaleDivisor)
                {
                    for (int z = 0; z < spaceWidth ; z += scaleDivisor)
                    {
                        int mippedX = (int)MathHelper.Clamp(x / scaleDivisor, 0, spaceWidth / scaleDivisor - 1);
                        int mippedY = (int)MathHelper.Clamp(y / scaleDivisor, 0, spaceHeight / scaleDivisor - 1);
                        int mippedZ = (int)MathHelper.Clamp(z / scaleDivisor, 0, spaceWidth / scaleDivisor - 1);

                            mippedArray[mippedX, mippedY, mippedZ] = getMostAbundantBlockType(unmippedArray,
                                        new IntVector3(x, y, z), scaleDivisor, spaceWidth, spaceHeight);//space.unmippedArray[i, y, z];



                    }
                }
            }

            //mippedSpace.createModel(Compositer.device);
           // mippedSpace.scale = scaleDivisor;

            //mippedSpace.mipLevel = mipLevel;
            return mippedArray;

        }

        static byte getMostAbundantBlockType(byte[, ,] originalArray, IntVector3 locationOfTestArea,
            int widthOfTestArea, int widthOfOriginalArray, int heightOfOriginalArray)
        {
            List<byte> types = new List<byte>();
            List<int> numberOfType = new List<int>();

            int xMax = locationOfTestArea.X + widthOfTestArea;
            int yMax = locationOfTestArea.Y + widthOfTestArea;
            int zMax = locationOfTestArea.Z + widthOfTestArea;

            if (xMax >= widthOfOriginalArray)
            {
                xMax = widthOfOriginalArray;
            }

            if (yMax >= heightOfOriginalArray)
            {
                yMax = heightOfOriginalArray;
            }

            if (zMax >= widthOfOriginalArray)
            {
                zMax = widthOfOriginalArray;
            }
            

            for (int x = locationOfTestArea.X; x < xMax; x += 1)
            {
                for (int y = locationOfTestArea.Y; y < yMax; y += 1)
                {
                    for (int z = locationOfTestArea.Z; z < zMax; z += 1)
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
            for (int i = 0; i < types.Count; i++)
            {
                if (numberOfType[i] > numberOfMostAbundant)
                {
                    result = types[i];
                }

            }
            return result;
        }



        public void drawForChunk(GraphicsDevice device, Effect effect, Matrix worldMatrix)
        {

            effect.Parameters["xWorld"].SetValue(worldMatrix);

            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //NO OLD BAD WRONG//world.device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / (3), VertexPositionNormalTexture.VertexDeclaration);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                    vertexBuffer.VertexCount, 0,
                    indexBuffer.IndexCount / 3);

            }
            //effect.Parameters["xWorld"].SetValue(Matrix.Identity);
        }

        public void drawForBodyPart(GraphicsDevice device, Effect effect, Matrix worldMatrix, bool highLighted)
        {
            setBuffersForDraw(device);
            drawWithoutSettingBuffers(effect, worldMatrix);
        }

        public void drawForBodyPartWithPresetBuffers(Effect effect, Matrix worldmatrix, bool highLighted)
        {
            drawWithoutSettingBuffers(effect, worldmatrix);
        }

        private void drawWithoutSettingBuffers(Effect effect, Matrix worldMatrix)
        {
            effect.Parameters["xWorld"].SetValue(worldMatrix);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //NO OLD BAD WRONG//world.device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / (3), VertexPositionNormalTexture.VertexDeclaration);
                Main.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                    vertexBuffer.VertexCount, 0,
                    indexBuffer.IndexCount / 3);
            }
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
        }

        private void setBuffersForDraw(GraphicsDevice device)
        {
            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);
        }



        public void createModel(GraphicsDevice device, byte[,,] unmippedArray, int spaceWidth, int spaceHeight)
        {

            byte[, ,] mippedArray = unmippedArray;
            int mipAmount = (int)Math.Pow(2,mipLevel);
            int spaceWidthMipped = spaceWidth;
            int spaceHeightMipped = spaceHeight;

            if (mippingRequired())
            {
                mippedArray = getMippedArray(mipLevel, spaceWidth, spaceHeight, unmippedArray);
                spaceWidthMipped /= mipAmount;
                spaceHeightMipped /= mipAmount;
            }


            VerticesAndIndices vertsAndInts = MeshBuilder.buildMesh(mippedArray, spaceWidthMipped, spaceHeightMipped);

            vertices = vertsAndInts.vertices;
            indices = vertsAndInts.indices;


            //copyFromTempIntAndVertexListIntoArrays(vertsAndInts.indices,vertsAndInts.vertices);


            if (vertices.Length == 0)
            {
                return;
            }
            copyToBuffers();

            readyToBeDisplayed = true;

            // the code that you want to measure comes here
            //watch.Stop();
            //var elapsedMs = watch.ElapsedMilliseconds;
            //if (spaceHeight == 64)
            // Console.WriteLine(elapsedMs);
        }

        private bool mippingRequired()
        {
            return mipLevel != 0;
        }

        public void copyFromTempIntAndVertexListIntoArrays(List<short> tempIntList, List<VertexPostitionColorPaintNormal> tempVertexList)
        {
            vertices = tempVertexList.ToArray();
            indices = tempIntList.ToArray();
        }

        public void copyFromTempIntAndVertexListIntoArrays(List<int> tempIntList, List<VertexPostitionColorPaintNormal> tempVertexList)
        {
            vertices = tempVertexList.ToArray();
            indices = new short[tempIntList.Count];

            for (int i = 0; i < tempIntList.Count; i++)
            {
                indices[i] = (short)tempIntList[i];
            }
        }

        public bool canBeDrawn()
        {
            if (!readyToBeDisplayed || vertexBuffer == null || indexBuffer == null)
            {
                return false;
            }
            return true;
        }

        private void copyToBuffers()
        {

            vertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, VertexPostitionColorPaintNormal.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPostitionColorPaintNormal>(vertices);
            indexBuffer = new IndexBuffer(Main.graphics.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);


        }

        public VertexBuffer getVertexBuffer()
        {
            return vertexBuffer;
        }

        public IndexBuffer getIndexBuffer()
        {
            return indexBuffer;
        }

        public void setBuffers(VertexBuffer nVertexBuffer, IndexBuffer nIndexBuffer)
        {
            vertexBuffer = nVertexBuffer;
            indexBuffer = nIndexBuffer;
            readyToBeDisplayed = true;
        }

        public void comeIntoView(GraphicsDevice device, byte[, ,] array, int spaceWidth, int spaceHeight)
        {
            createModel(device,  array,  spaceWidth,  spaceHeight);
            readyToBeDisplayed = true;
        }



        public int getMipLevel()
        {
            return mipLevel;
        }
    }
}
