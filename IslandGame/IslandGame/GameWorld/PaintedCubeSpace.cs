using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;


namespace IslandGame.GameWorld
{
    [Serializable]
    public class PaintedCubeSpace
    {
        

        public int spaceWidth;
        public int spaceHeight;

        public int numFaces = 0;

        string pathThatThisSpaceWasLoadedFromCANBENULL = null;

        PaintedCubeSpaceDisplayComponant unMippedDisplayer, mippedDisplayer;
        public byte[, ,] array;
        public static readonly byte AIR = 0;
        public static readonly byte WATER = (byte)165;
        public float scale = 1f;
        public bool serializedAfterLastChange = false; //must be set to false when the chunk is generated or changed
        public Vector3 loc;


        public enum AxisDirection : byte
        {
            pX, nX, pY, nY, pZ, nZ
        }


        public enum cornerToAOArrayLoc
        {
            XlYhZh = 0,
            XlYhZl = 1,
            XlYlZh = 2,
            XlYlZl = 3,
            XhYhZh = 4,
            XhYhZl = 5,
            XhYlZh = 6,
            XhYlZl = 7

        }

        public PaintedCubeSpace(int width, int height, Vector3 nLoc)
        {
            unMippedDisplayer = new PaintedCubeSpaceDisplayComponant(0);
            mippedDisplayer = null;
            array = new byte[width, height, width];


            loc = nLoc;

            spaceWidth = width;
            spaceHeight = height;

            
            


        }


        public void setLoadedFrompath(string nPath)
        {
            pathThatThisSpaceWasLoadedFromCANBENULL = nPath;
        }

        public void setMipLevel(int level)
        {
            if (level == 0)
            {
                mippedDisplayer = null;




            }

            else if (mippedDisplayer == null || mippedDisplayer.getMipLevel() != level)
            {
                mippedDisplayer = new PaintedCubeSpaceDisplayComponant(level);
                if (pathThatThisSpaceWasLoadedFromCANBENULL != null)
                {
                    if (CubeAnimator.ModelLoader.hasMipAtPathAndLevel(pathThatThisSpaceWasLoadedFromCANBENULL, level))
                    {
                        mippedDisplayer.setBuffers(CubeAnimator.ModelLoader.getBuffersAtPathAtMipLevel(pathThatThisSpaceWasLoadedFromCANBENULL, level));

                    }
                    else
                    {
                        mippedDisplayer.createModel(Compositer.device, array, spaceWidth, spaceHeight);

                        CubeAnimator.ModelLoader.addPathAndMipForMemoization(pathThatThisSpaceWasLoadedFromCANBENULL, level,
new VertexAndIndexBuffers(mippedDisplayer.getVertexBuffer(), mippedDisplayer.getIndexBuffer()));
                    }
                }
                else
                {

                    mippedDisplayer.createModel(Compositer.device, array, spaceWidth, spaceHeight);

                }


            }

        }

        public void drawForBodyPartWithPresetBuffers(Effect effect, Matrix superMatrix, Quaternion rotation, bool highLighted)
        {
            getCurrentDisplayer().drawForBodyPartWithPresetBuffers(effect,Matrix.CreateScale((float)Math.Pow(2, getCurrentDisplayer().getMipLevel()))* getMatrix(superMatrix, rotation), highLighted);
        }

        public void drawForBodyPart(GraphicsDevice device, Effect effect, Matrix superMatrix, Quaternion rotation, bool highLighted)
        {
            getCurrentDisplayer().drawForBodyPart(device, effect, 
                Matrix.CreateScale((float)Math.Pow(2, getCurrentDisplayer().getMipLevel()))*getMatrix(superMatrix, rotation), highLighted);
        }


        public void addToWorldMarkup(Matrix matrix)
        {
            if (pathThatThisSpaceWasLoadedFromCANBENULL != null)
    
            {


                WorldMarkupHandler.addFlagWithMatrix(pathThatThisSpaceWasLoadedFromCANBENULL,
     Matrix.CreateScale
     ((float)Math.Pow(2, getCurrentDisplayer().getMipLevel())) * matrix, unMippedDisplayer);

                //WorldMarkupHandler.addFlagWithMatrix(pathThatThisSpaceWasLoadedFromCANBENULL, 
               //      Matrix.CreateScale
               //      ((float)Math.Pow(2, getCurrentDisplayer().getMipLevel()))
              //  * getMatrix(superMatrix, rotation), unMippedDisplayer);

                //WorldMarkupHandler.addFlagWithMatrix(pathThatThisSpaceWasLoadedFromCANBENULL, 
                //     Matrix.Identity, getCurrentDisplayer());
            }
        }


        public void drawForChunk(GraphicsDevice device, Effect effect, Matrix superMatrix)
        {
            getCurrentDisplayer().drawForChunk(device, effect, Matrix.CreateScale((float)Math.Pow(2, getCurrentDisplayer().getMipLevel()))* getMatrix() * superMatrix);
        }

        public void createModel(GraphicsDevice device)
        {
            getCurrentDisplayer().createModel(device, array, spaceWidth, spaceHeight);
        }

        public IndexBuffer getIndexBuffer()
        {
            return getCurrentDisplayer().getIndexBuffer();
        }

        public VertexBuffer getVertexBuffer()
        {
            return getCurrentDisplayer().getVertexBuffer();
        }

        private PaintedCubeSpaceDisplayComponant getCurrentDisplayer()
        {
            if (mippedDisplayer != null&& mippedDisplayer.canBeDrawn())
            {
                return mippedDisplayer;
            }
            else
            {
                return unMippedDisplayer;
            }
        }

        public bool isTransparentAt(int x, int y, int z)
        {
            if (withinSpace(new Vector3(x, y, z)))
            {
                return array[x, y, z] == AIR;
            }
            return false;
        }

        public bool isTransparentAtWithoutWithinCheck(int x, int y, int z)
        {
           return array[x, y, z] == AIR;
        }

        public bool canBeDrawn()
        {
            return getCurrentDisplayer().canBeDrawn();
        }



        public void serializeChunk(string savePath)
        {


            //Opens a file and serializes the object into it in binary format.

            if (new FileInfo(savePath).Extension != ".vox")
            {
                savePath += ".vox";
            }

            Stream stream = File.Open(savePath, FileMode.OpenOrCreate);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, array);
            stream.Close();
            serializedAfterLastChange = true;


            //MainWindow.singleton.updateFileTreeView();

        }

        public byte[] compressedArray()
        {

            List<byte> numberList = new List<byte>();   // numBlocks, blockType, numBLocks, blockType

            for (int z = 0; z < spaceWidth; z++)
            {
                for (int x = 0; x < spaceWidth; x++)
                {
                    byte numCubesOfSameTypeSoFar = 0;

                    byte currentType = array[x, 0, z];

                    for (int y = 0; y < spaceHeight; y++)
                    {
                        if (array[x, y, z] == (byte)currentType)
                        {

                            numCubesOfSameTypeSoFar++;
                        }
                        if (y + 1 >= spaceHeight || array[x, y, z] != (byte)currentType || numCubesOfSameTypeSoFar > 250) //put the current numbers into the unmippedArray  
                        {
                            numberList.Add((byte)(numCubesOfSameTypeSoFar));
                            numberList.Add((byte)currentType);
                            numCubesOfSameTypeSoFar = 1; //0?
                            currentType = array[x, y, z];
                        }
                    }

                }

            }
            byte[] result = numberList.ToArray();



            return result;
        }

        public void decompressArrayAndSetArray(byte[] input)
        {
            int numBlocksDone = 0;
            int currentSpotInArray = 0;
            for (int z = 0; z < spaceWidth; z++)
            {
                for (int x = 0; x < spaceWidth; x++)
                {
                    for (int y = 0; y < spaceHeight; y++)
                    {
                        numBlocksDone++;
                        array[x, y, z] = input[currentSpotInArray + 1];
                        input[currentSpotInArray]--;
                        if (input[currentSpotInArray] == 0)
                        {
                            currentSpotInArray += 2;
                        }
                    }
                }
            }
        }

        public void comeIntoView(GraphicsDevice device)
        {
            getCurrentDisplayer().comeIntoView(device,array,spaceWidth,spaceHeight);

        }

        public void flipSpace()
        {
            byte[, ,] newArray = new byte[spaceWidth, spaceHeight, spaceWidth];
            for (int x = 0; x < spaceWidth; x++)
            {
                for (int y = 0; y < spaceHeight; y++)
                {
                    for (int z = 0; z < spaceWidth; z++)
                    {
                        Vector3 flipped = flippedLoc(x, y, z);

                        newArray[(int)flipped.X, (int)flipped.Y, (int)flipped.Z] = array[x, y, z];

                    }
                }
            }

            array = newArray;
        }

        public void moveWork(Vector3 move)
        {
            //expands whole unmippedArray
            byte[, ,] newArray = new byte[spaceWidth + 2, spaceHeight + 2, spaceWidth + 2];
            for (int x = 0; x < spaceWidth; x++)
            {
                for (int y = 0; y < spaceHeight; y++)
                {
                    for (int z = 0; z < spaceWidth; z++)
                    {
                        if (withinSpace(move + new Vector3(x, y, z)))
                        {
                            newArray[x + 1 + (int)move.X, y + 1 + (int)move.Y, z + 1 + (int)move.Z] = array[x, y, z];
                        }
                    }
                }
            }
            spaceWidth += 2;
            spaceHeight += 2;
            array = newArray;


        }





        public void buildRect(Vector3 loc1, Vector3 loc2, byte type)
        {
            float temp;
            if (loc1.X > loc2.X)
            {
                temp = loc1.X;
                loc1.X = loc2.X;
                loc2.X = temp;
            }
            if (loc1.Y > loc2.Y)
            {
                temp = loc1.Y;
                loc1.Y = loc2.Y;
                loc2.Y = temp;
            }
            if (loc1.Z > loc2.Z)
            {
                temp = loc1.Z;
                loc1.Z = loc2.Z;
                loc2.Z = temp;
            }
            for (int x = (int)loc1.X; x < (int)loc2.X; x++)
            {
                for (int y = (int)loc1.Y; y < (int)loc2.Y; y++)
                {
                    for (int z = (int)loc1.Z; z < (int)loc2.Z; z++)
                    {
                        Vector3 locToPut = Vector3.Transform(new Vector3(x, y, z), putPointIntoSpaceContext());
                        while (!withinSpace(locToPut))
                        {
                            expandArray();
                        }
                        if (withinSpace(locToPut))
                        {
                            array[(int)locToPut.X, (int)locToPut.Y, (int)locToPut.Z] = type;
                        }
                        else
                        {
                        }
                    }
                }

            }
        }

        public bool destroyBlockFromWorldSpace(Vector3 firstRef, Vector3 secondRef, bool mirror)
        {
            Vector3? blockLocMaybe = rayTileHitsViaModernaInSpaceContext(true, float.NaN, array, Vector3.Transform(firstRef,
                putPointIntoSpaceContext()), Vector3.Transform(secondRef, putPointIntoSpaceContext()));

            if (!blockLocMaybe.HasValue)
            {
                return false;
            }
            Vector3 blockLoc = (Vector3)blockLocMaybe;



            array[(int)blockLoc.X, (int)blockLoc.Y, (int)blockLoc.Z] = 0;
            if (mirror)
            {
                mirrorBlockPlace((int)blockLoc.X, (int)blockLoc.Y, (int)blockLoc.Z, 0);
            }
            return true;

        }

        public byte? getBlockFromWorldSpace(Vector3 firstRef, Vector3 secondRef)
        {
            Vector3? blockLocMaybe = rayTileHitsViaModernaInSpaceContext(true, float.NaN, array, Vector3.Transform(firstRef,
                putPointIntoSpaceContext()), Vector3.Transform(secondRef, putPointIntoSpaceContext()));

            if (!blockLocMaybe.HasValue)
            {
                return null;
            }
            Vector3 blockLoc = (Vector3)blockLocMaybe;



            return array[(int)blockLoc.X, (int)blockLoc.Y, (int)blockLoc.Z];

        }

        public bool placeBlockFromWorldSpace(Vector3 firstRef, Vector3 secondRef, byte toPlace, bool mirror)
        {
            Vector3? blockLocMaybe = rayTileHitsViaModernaInSpaceContext(false, float.NaN, array, Vector3.Transform(firstRef,
                putPointIntoSpaceContext()), Vector3.Transform(secondRef, putPointIntoSpaceContext()));

            if (!blockLocMaybe.HasValue)
            {
                return false;
            }
            Vector3 blockLoc = (Vector3)blockLocMaybe;
            if (!withinSpace(blockLoc))
            {
                expandArray();
                placeBlockFromWorldSpace(firstRef, secondRef, toPlace, mirror);
                return true;
            }
            array[(int)blockLoc.X, (int)blockLoc.Y, (int)blockLoc.Z] = toPlace;
            if (mirror)
            {
                mirrorBlockPlace((int)blockLoc.X, (int)blockLoc.Y, (int)blockLoc.Z, toPlace);
            }
            return true;
        }

        void mirrorBlockPlace(int x, int y, int z, byte toPlace)
        {
            float middle = ((float)spaceWidth / 2.0f);
            if (x < middle)
            {
                array[spaceWidth - x - 1, y, z] = toPlace;
            }
            if (x >= middle)
            {
                array[(int)(middle - (x - middle + 1)), y, z] = toPlace;
            }
        }

        Vector3 flippedLoc(int x, int y, int z)
        {
            float middle = ((float)spaceWidth / 2.0f);
            if (x < middle)
            {
                return new Vector3(spaceWidth - x - 1, y, z);
            }
            else
            {
                return new Vector3((int)(middle - (x - middle + 1)), y, z);
            }
        }

        Vector3 mirrorVec(Vector3 test)
        {
            Vector3 result = new Vector3(test.X, test.Y, test.Z);
            float middle = ((float)spaceWidth / 2.0f);
            if (test.X < middle)
            {
                result.X = spaceWidth - test.X - 1;
            }
            if (test.X >= middle)
            {
                result.X = (int)(middle - (test.X - middle + 1));
            }
            return result;
        }

        public void setUnmippedBuffers(VertexBuffer nVertexBuffer,IndexBuffer nIndexBuffer)
        {
            getUnmippedDisplayer().setBuffers(nVertexBuffer, nIndexBuffer);
        }

        private PaintedCubeSpaceDisplayComponant getUnmippedDisplayer()
        {
            return unMippedDisplayer;
        }

        public Matrix getMatrix()
        {
            Vector3 cubeSpaceOffset = getOffset();

            return Matrix.CreateScale(scale ) * Matrix.CreateTranslation(-cubeSpaceOffset) * Matrix.CreateTranslation(loc)
                * Matrix.CreateTranslation(cubeSpaceOffset)  ;
        }

        public Matrix getMatrix(Matrix superMatrix, Quaternion rotation)
        {
            Vector3 cubeSpaceOffset = getOffset();
            /*Quaternion superRotation;
            Vector3 superScale;
            Vector3 superTranslation;
            worldMatrix.Decompose(out superScale,out superRotation, out superTranslation);*/

            return (Matrix.CreateTranslation(cubeSpaceOffset) * Matrix.CreateScale(scale ) * Matrix.CreateFromQuaternion(rotation)* Matrix.CreateTranslation(loc)) * superMatrix;
        }

        Vector3 getOffset()
        {
            return new Vector3(-(float)spaceWidth / 2.0f,
               -(float)spaceHeight / 2.0f, -(float)spaceWidth / 2.0f);
        }

        Matrix putPointIntoSpaceContext(Matrix superMatrix, Quaternion rotation)
        {
            return Matrix.Invert(getMatrix(superMatrix, rotation));
        }

        Matrix putPointIntoSpaceContext()
        {
            return Matrix.Invert(getMatrix());
        }

        public bool withinSpace(Vector3 test)  // does not account for space locInPath
        {
            if (
             test.X < spaceWidth && test.X >= 0
                && test.Z < spaceWidth && test.Z >= 0
                && test.Y < spaceHeight && test.Y >= 0)
            {
                return true;
            }
            return false;
        }

        public bool withinSpace(int x, int y, int z)  // does not account for space locInPath
        {
            if (
             x < spaceWidth && x >= 0
                && z < spaceWidth && z >= 0
                && y < spaceHeight && y >= 0)
            {
                return true;
            }
            return false;
        }

        Vector3? rayTileHitsViaModernaInSpaceContext(bool wantNearestHit, float range, byte[, ,] array, Vector3 firstRef, Vector3 secondRef)
        {

            List<Vector3> intersected;
            intersected = new List<Vector3>();

            int xMin, xMax, yMin, yMax, zMin, zMax;
            if (!System.Single.IsNaN(range))
            {
                xMin = (int)(firstRef.X - range);
                xMax = (int)(firstRef.X + range);
                yMin = (int)(firstRef.Y - range);
                yMax = (int)(firstRef.Y + range);
                zMin = (int)(firstRef.Z - range);
                zMax = (int)(firstRef.Z + range);

            }
            else //it's Nan
            {
                xMin = 0;
                xMax = spaceWidth;
                yMin = 0;
                yMax = spaceHeight;
                zMin = 0;
                zMax = spaceWidth;
            }

            for (int a = xMin; a < xMax; a += 10)
            {
                for (int b = yMin; b < yMax; b += 10)
                {
                    for (int c = zMin; c < zMax; c += 10)
                    {
                        if (withinSpace(new Vector3(a, b, c)))
                        {


                            Vector3 wat = new Vector3(0, 0, 0);
                            if (GeometryFunctions.CheckLineBox(new Vector3(a, b, c), new Vector3(a + 10, b + 10, c + 10), secondRef, firstRef, ref wat))
                            {
                                for (int x = a; x < a + 10; x++)
                                {
                                    for (int y = b; y < b + 10; y++)
                                    {
                                        for (int z = c; z < c + 10; z++)
                                        {
                                            if (withinSpace(new Vector3(x, y, z)))
                                            {
                                                Vector3 lul = new Vector3(0, 0, 0);
                                                if (GeometryFunctions.CheckLineBox(new Vector3(x, y, z), new Vector3(x + 1, y + 1, z + 1), secondRef, firstRef, ref lul))
                                                {
                                                    intersected.Add(new Vector3(x, y, z));

                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            float minDist = 99999999999;
            List<float> distances = new List<float>();
            Vector3 closest = new Vector3();
            Vector3 secondClosest = new Vector3();
            bool foundOne = false;
            for (int i = 0; i < intersected.Count; i++)
            {

                Vector3 test = intersected[i];
                if (array[(int)test.X, (int)test.Y, (int)test.Z] != 0)//if (getTypeAt((int)alreadyOccupiedBlock.X, (int)alreadyOccupiedBlock.Y, (int)alreadyOccupiedBlock.Z).transLoS == false)
                {
                    Vector3 dist = firstRef - test;
                    float length = (float)Math.Sqrt((firstRef.X - test.X) * (firstRef.X - test.X) + (firstRef.Y - test.Y) * (firstRef.Y - test.Y) + (firstRef.Z - test.Z) * (firstRef.Z - test.Z));
                    distances.Add(length);

                    if ((float)length < (float)minDist)
                    {
                        minDist = length;

                        foundOne = true;
                        closest = test;
                    }


                }
            }


            BoundingBox box = new BoundingBox(new Vector3(closest.X, closest.Y, closest.Z), new Vector3(closest.X + 1, closest.Y + 1, closest.Z + 1));
            Ray ray = new Ray(firstRef, Vector3.Normalize(secondRef - firstRef));

            float? distanceToHitMaybe = box.Intersects(ray);
            if (!distanceToHitMaybe.HasValue)
            {
                return null;
            }
            float distanceToHit = (float)distanceToHitMaybe;


            float backup = .0001f;
            Vector3 slightlyCloser = ray.Direction * (distanceToHit - backup) + firstRef;



            secondClosest = slightlyCloser;
            if (foundOne)
            {
                if (withinSpace(closest))
                {

                }

            }

            if (wantNearestHit)
            {
                return closest;
            }
            else
            {
                return secondClosest;
            }

        }

        public Vector3? getNearestBlockAlongRayFromChunkSpaceContext(Ray nray)
        {
            Ray ray = new Ray(Vector3.Transform(nray.Position, putPointIntoSpaceContext()), nray.Direction);
            ray.Direction.Normalize();

            List<Vector3> hits = new List<Vector3>();
            int roughStep = 10;
            int x, y, z;
            for (int roughZ = 0; roughZ < spaceWidth; roughZ += roughStep)
            {
                for (int roughX = 0; roughX < spaceWidth; roughX += roughStep)
                {
                    for (int roughY = 0; roughY < spaceHeight; roughY += roughStep)
                    {
                        if (ray.Intersects(new BoundingBox(new Vector3(roughX, roughY, roughZ), new Vector3(roughX, roughY, roughZ) + new Vector3(roughStep, roughStep, roughStep))).HasValue)
                        {
                            for (int fineStepZ = 0; fineStepZ < roughStep; fineStepZ++)
                            {
                                for (int fineStepX = 0; fineStepX < roughStep; fineStepX++)
                                {
                                    for (int fineStepY = 0; fineStepY < roughStep; fineStepY++)
                                    {
                                        x = roughX + fineStepX;
                                        y = roughY + fineStepY;
                                        z = roughZ + fineStepZ;
                                        if (withinSpace(x, y, z))
                                        {
                                            if (!isTransparentAtWithoutWithinCheck(x, y, z))
                                            {
                                                float? intersectDist = ray.Intersects(new BoundingBox(new Vector3(x, y, z), new Vector3(x, y, z) + new Vector3(1, 1, 1)));

                                                if (intersectDist.HasValue)
                                                {
                                                    Vector3 hitLoc = ray.Position + ray.Direction * ((float)intersectDist + .0001f);
                                                    if (withinSpace(hitLoc))
                                                    {
                                                        if (array[(int)hitLoc.X, (int)hitLoc.Y, (int)hitLoc.Z] != AIR)
                                                        {
                                                            hits.Add(hitLoc);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Vector3? nearest = null;
            float minDist = float.MaxValue;
            foreach (Vector3 test in hits)
            {
                float dist = Vector3.Distance(test, ray.Position);
                if (dist < minDist)
                {
                    nearest = test;
                    minDist = dist;
                }
            }
            /*if (nearest != null)  //delete found block
            {
                unmippedArray[(int)((Vector3)nearest).X, (int)((Vector3)nearest).Y, (int)((Vector3)nearest).Z]=0;
                createModel(Compositer.device);
            }*/
            return nearest;
        }

        public Vector3? getNearestHitInSpaceContext(Vector3 firstRef, Vector3 secondRef, Matrix superMatrix, Quaternion rotation)
        {
            return rayTileHitsViaModernaInSpaceContext(true, float.NaN, array, Vector3.Transform(firstRef,
                 putPointIntoSpaceContext(superMatrix, rotation)), Vector3.Transform(secondRef, putPointIntoSpaceContext(superMatrix, rotation)));
        }

        public Vector3? getNearestHitInSpaceContext(Vector3 firstRef, Vector3 secondRef)
        {
            return rayTileHitsViaModernaInSpaceContext(true, float.NaN, array, Vector3.Transform(firstRef,
                 putPointIntoSpaceContext()), Vector3.Transform(secondRef, putPointIntoSpaceContext()));
        }

        void expandArray()
        {
            byte[, ,] newArray = new byte[spaceWidth + 2, spaceHeight + 2, spaceWidth + 2];
            for (int x = 0; x < spaceWidth; x++)
            {
                for (int y = 0; y < spaceWidth; y++)
                {
                    for (int z = 0; z < spaceWidth; z++)
                    {
                        newArray[x + 1, y + 1, z + 1] = array[x, y, z];
                    }
                }
            }
            spaceWidth += 2;
            spaceHeight += 2;
            array = newArray;
        }

        public void replaceArrayWith(byte[, ,] newArray)
        {
            array = newArray;
            spaceWidth = newArray.GetLength(0);
            spaceHeight = newArray.GetLength(1);
        }

        void fill(Vector3 startLoc, byte colorFrom, byte colorTo)
        {
            if (array[(int)startLoc.X, (int)startLoc.Y, (int)startLoc.Z] == colorTo)
            {
                return;
            }
            List<Vector3> openNodes = getAdjacentBlocks((int)startLoc.X, (int)startLoc.Y, (int)startLoc.Z);
            array[(int)startLoc.X, (int)startLoc.Y, (int)startLoc.Z] = colorTo;
            removeAllNotOfColor(colorFrom, openNodes);
            while (openNodes.Count > 0)
            {

                List<Vector3> newOpenNodes = new List<Vector3>();
                foreach (Vector3 from in openNodes)
                {
                    array[(int)from.X, (int)from.Y, (int)from.Z] = colorTo;
                    List<Vector3> potentialNextSteps = getAdjacentBlocks((int)from.X, (int)from.Y, (int)from.Z);
                    removeAllNotOfColor(colorFrom, potentialNextSteps);
                    newOpenNodes.AddRange(potentialNextSteps);
                }
                openNodes = newOpenNodes;
            }
        }

        public byte fillFromWorldContext(Vector3 firstRef, Vector3 secondRef, byte color, bool mirror)
        {

            Vector3? blockLocMaybe = rayTileHitsViaModernaInSpaceContext(true, float.NaN, array, Vector3.Transform(firstRef,
                putPointIntoSpaceContext()), Vector3.Transform(secondRef, putPointIntoSpaceContext()));

            if (!blockLocMaybe.HasValue)
            {
                return 0;
            }
            Vector3 blockLoc = (Vector3)blockLocMaybe;
            //if (!withinSpace(blockLocMaybe))
            //{
            //expandArray();
            fill(blockLoc, array[(int)blockLoc.X, (int)blockLoc.Y, (int)blockLoc.Z], color);
            if (mirror)
            {
                fill(mirrorVec(blockLoc), array[(int)blockLoc.X, (int)blockLoc.Y, (int)blockLoc.Z], color);
            }

            //return;
            //}
            return array[(int)blockLoc.X, (int)blockLoc.Y, (int)blockLoc.Z];
        }

        void removeAllNotOfColor(byte color, List<Vector3> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {

                if (array[(int)list[i].X, (int)list[i].Y, (int)list[i].Z] != color)
                {
                    list.RemoveAt(i);

                }
            }
        }

        List<Vector3> getAdjacentBlocks(int x, int y, int z)
        {
            List<Vector3> result = new List<Vector3>();
            List<Vector3> directions = new List<Vector3>();
            directions.Add(new Vector3(1, 0, 0));
            directions.Add(new Vector3(-1, 0, 0));
            directions.Add(new Vector3(0, 1, 0));
            directions.Add(new Vector3(0, -1, 0));
            directions.Add(new Vector3(0, 0, 1));
            directions.Add(new Vector3(0, 0, -1));

            foreach (Vector3 test in directions)
            {
                if (withinSpace(new Vector3(x, y, z) + test))
                {

                    result.Add(new Vector3(x, y, z) + test);
                }
            }

            return result;
        }

        public static bool isSolidType(byte type)
        {
            return type != AIR && type != WATER;
        }

        public static bool isOpaqueType(byte type)
        {
            return type != AIR && type != WATER;
        }
    }
}
