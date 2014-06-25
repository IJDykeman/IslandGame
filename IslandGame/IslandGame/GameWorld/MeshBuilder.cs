using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    static class MeshBuilder
    {


        static readonly Vector3 frontNormal = new Vector3(0, 0, -1f);
        static readonly Vector3 backNormal = new Vector3(3f, -.5f, 1f);
        static readonly Vector3 rightNormal = new Vector3(-.6f, -.5f, 1f);
        static readonly Vector3 leftNormal = new Vector3(1, 0, 0);
        static readonly Vector3 bottomNormal = new Vector3(0, 1, 0);
        static readonly Vector3 topNormal = new Vector3(0, 1, 0);


        static readonly byte lowAOvalue = 60;
        static readonly byte highAOvalue = 60;


        static readonly byte[] frontIndices = { 0, 1, 2, 2, 3, 0 };
        static readonly byte[] backIndices = { 2, 1, 0, 0, 3, 2 };

        static readonly byte[] topIndices = { 1, 2, 0, 1, 0, 3 };//{ 0, 3, 1, 1, 2, 0 };
        static readonly byte[] bottomIndices = { 1, 3, 0, 0, 2, 1 };

        static readonly byte[] leftIndices = { 1, 3, 0, 0, 2, 1 };
        static readonly byte[] rightIndices = { 0, 3, 1, 1, 2, 0 };


        static readonly cornr2AOArrLc[] frontCorners = { cornr2AOArrLc.XlYhZl, cornr2AOArrLc.XlYlZl, cornr2AOArrLc.XhYlZl, cornr2AOArrLc.XhYhZl };
        static readonly cornr2AOArrLc[] backCorners = { cornr2AOArrLc.XlYhZh, cornr2AOArrLc.XlYlZh, cornr2AOArrLc.XhYlZh, cornr2AOArrLc.XhYhZh };

        static readonly cornr2AOArrLc[] topCorners = { cornr2AOArrLc.XlYhZl, cornr2AOArrLc.XhYhZh, cornr2AOArrLc.XlYhZh, cornr2AOArrLc.XhYhZl };
        static readonly cornr2AOArrLc[] bottomCorners = { cornr2AOArrLc.XlYlZl, cornr2AOArrLc.XhYlZh, cornr2AOArrLc.XlYlZh, cornr2AOArrLc.XhYlZl };

        static readonly cornr2AOArrLc[] leftCorners = { cornr2AOArrLc.XhYlZh, cornr2AOArrLc.XhYhZl, cornr2AOArrLc.XhYhZh, cornr2AOArrLc.XhYlZl };
        static readonly cornr2AOArrLc[] rightCorners = { cornr2AOArrLc.XlYlZh, cornr2AOArrLc.XlYhZl, cornr2AOArrLc.XlYhZh, cornr2AOArrLc.XlYlZl };
        /*


                    drawFront(maybeSpace, indexList, vertexList, loc, AOarray[2]);

                    drawBack(maybeSpace, indexList, vertexList, loc, AOarray[2]);

                    drawRight(maybeSpace, indexList, vertexList, loc, AOarray[0]);

                    drawLeft(maybeSpace, indexList, vertexList, loc, AOarray[0]);

                    drawBottom(maybeSpace, indexList, vertexList, loc, AOarray[1]);

                    drawTop(maybeSpace, indexList, vertexList, loc, AOarray[1]);
        */


        public enum cornr2AOArrLc : byte
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


        static readonly byte[] xOffSetsAtCorners = { 0, 0, 0, 0, 1, 1, 1, 1 };
        static readonly byte[] yOffSetsAtCorners = { 1, 1, 0, 0, 1, 1, 0, 0 };
        static readonly byte[] zOffSetsAtCorners = { 1, 0, 1, 0, 1, 0, 1, 0 };

        static Random rand = new Random();

        public static int getNumFaces(byte[, ,] array, int spaceWidth, int spaceHeight)
        {

            int numFaces = 0;




            for (int x = 0; x < spaceWidth - 1; x++)
            {
                for (int y = 0; y < spaceHeight - 1; y++)
                {
                    for (int z = 0; z < spaceWidth - 1; z++)
                    {

                        if (array[x, y, z] != (byte)PaintedCubeSpace.AIR)
                        {
                            if (array[x, y + 1, z] == (byte)PaintedCubeSpace.AIR)
                            {
                                numFaces++;
                            }
                            if (array[x + 1, y, z] == (byte)PaintedCubeSpace.AIR)
                            {
                                numFaces++;
                            }
                            if (array[x, y, z + 1] == (byte)PaintedCubeSpace.AIR)
                            {
                                numFaces++;
                            }
                        }
                        else //xyz is air
                        {
                            if (array[x, y + 1, z] != (byte)PaintedCubeSpace.AIR)
                            {
                                numFaces++;
                            }
                            if (array[x + 1, y, z] != (byte)PaintedCubeSpace.AIR)
                            {
                                numFaces++;

                            }
                            if (array[x, y, z + 1] != (byte)PaintedCubeSpace.AIR)
                            {
                                numFaces++;
                            }
                        }
                    }
                }
            }



            for (int x = 0; x < spaceWidth; x++)
            {
                for (int z = 0; z < spaceWidth; z++)
                {
                    int y = spaceHeight - 1;

                    countFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, x, y, z, ref numFaces);
                }
            }

            for (int y = 0; y < spaceHeight; y++)
            {

                for (int z = 0; z < spaceWidth; z++)
                {
                    int x = spaceWidth - 1;
                    countFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, x, y, z, ref numFaces);
                }
            }

            for (int x = 0; x < spaceWidth; x++)
            {
                for (int y = 0; y < spaceHeight; y++)
                {
                    int z = spaceWidth - 1;
                    countFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, x, y, z, ref numFaces);
                }
            }


            for (int x = 0; x < spaceWidth; x++)
            {
                for (int z = 0; z < spaceWidth; z++)
                {
                    int y = -1;
                    countFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, x, y, z, ref numFaces);
                }
            }

            for (int y = 0; y < spaceHeight; y++)
            {

                for (int z = 0; z < spaceWidth; z++)
                {
                    int x = -1;
                    countFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, x, y, z, ref numFaces);
                }
            }

            for (int x = 0; x < spaceWidth; x++)
            {
                for (int y = 0; y < spaceHeight; y++)
                {
                    int z = -1;
                    int before = numFaces;
                    countFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, x, y, z, ref numFaces);

                }
            }




            return numFaces;
        }

        public static VerticesAndIndices buildMesh(byte[, ,] array, int spaceWidth, int spaceHeight, int modelOffsetX, int modelOffsetY, int modelOffsetZ)
        {
            int numFaces = getNumFaces(array, spaceWidth, spaceHeight);

            //List<VertexPostitionColorPaintNormal> vertices = new List<VertexPostitionColorPaintNormal>(numFaces * 4);
            //List<short> indices = new List<short>(numFaces * 6);
            VertexPostitionColorPaintNormal[] vertices = new VertexPostitionColorPaintNormal[numFaces * 4];
            short[] indices = new short[numFaces * 6];

            int USETOStoreVertexCountSoFar = 0;
            int verticesSoFar = 0;
            int indicesSoFar = 0;

            for (int x = 0; x < spaceWidth - 1; x++)
            {
                for (int y = 0; y < spaceHeight - 1; y++)
                {
                    for (int z = 0; z < spaceWidth - 1; z++)
                    {

                        if (array[x, y, z] != (byte)PaintedCubeSpace.AIR)
                        {
                            if (array[x, y + 1, z] == (byte)PaintedCubeSpace.AIR)
                            {
                                drawFace(x, y, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y, z, array, spaceWidth, spaceHeight)[1],
                                    topIndices, topNormal, topCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                            }
                            if (array[x + 1, y, z] == (byte)PaintedCubeSpace.AIR)
                            {
                                drawFace(x, y, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y, z, array, spaceWidth, spaceHeight)[0],
                                    leftIndices, leftNormal, leftCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                            }
                            if (array[x, y, z + 1] == (byte)PaintedCubeSpace.AIR)
                            {
                                drawFace(x, y, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y, z, array, spaceWidth, spaceHeight)[2],
                                    backIndices, backNormal, backCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                            }
                        }
                        else //xyz is air
                        {
                            if (array[x, y + 1, z] != (byte)PaintedCubeSpace.AIR)
                            {
                                drawFace(x, y + 1, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y + 1, z, array, spaceWidth, spaceHeight)[1],
                                    bottomIndices, bottomNormal, bottomCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                            }
                            if (array[x + 1, y, z] != (byte)PaintedCubeSpace.AIR)
                            {
                                drawFace(x + 1, y, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x + 1, y, z, array, spaceWidth, spaceHeight)[0],
                                    rightIndices, rightNormal, rightCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);

                            }
                            if (array[x, y, z + 1] != (byte)PaintedCubeSpace.AIR)
                            {
                                drawFace(x, y, z + 1, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y, z + 1, array, spaceWidth, spaceHeight)[2],
                                    frontIndices, frontNormal, frontCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < spaceWidth; x++)
            {
                for (int z = 0; z < spaceWidth; z++)
                {
                    int y = spaceHeight - 1;
                    createFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, vertices, ref verticesSoFar,
                        indices, ref indicesSoFar, USETOStoreVertexCountSoFar, x, y, z, modelOffsetX, modelOffsetY, modelOffsetZ);
                }
            }


            for (int y = 0; y < spaceHeight; y++)
            {

                for (int z = 0; z < spaceWidth; z++)
                {
                    int x = spaceWidth - 1;
                    createFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, vertices, ref verticesSoFar,
                        indices, ref indicesSoFar, USETOStoreVertexCountSoFar, x, y, z, modelOffsetX, modelOffsetY, modelOffsetZ);
                }
            }

            for (int x = 0; x < spaceWidth; x++)
            {
                for (int y = 0; y < spaceHeight; y++)
                {
                    int z = spaceWidth - 1;
                    createFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, vertices, ref verticesSoFar,
                        indices, ref indicesSoFar, USETOStoreVertexCountSoFar, x, y, z, modelOffsetX, modelOffsetY, modelOffsetZ);
                }
            }


            for (int x = 0; x < spaceWidth; x++)
            {
                for (int z = 0; z < spaceWidth; z++)
                {
                    int y = -1;
                    createFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, vertices, ref verticesSoFar,
                        indices, ref indicesSoFar, USETOStoreVertexCountSoFar, x, y, z, modelOffsetX, modelOffsetY, modelOffsetZ);
                }
            }

            for (int y = 0; y < spaceHeight; y++)
            {

                for (int z = 0; z < spaceWidth; z++)
                {
                    int x = -1;
                    createFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, vertices, ref verticesSoFar,
                        indices, ref indicesSoFar, USETOStoreVertexCountSoFar, x, y, z, modelOffsetX, modelOffsetY, modelOffsetZ);
                }
            }

            for (int x = 0; x < spaceWidth; x++)
            {
                for (int y = 0; y < spaceHeight; y++)
                {
                    int z = -1;
                    createFacesOnBlockWithWithinChecks(array, spaceWidth, spaceHeight, vertices, ref verticesSoFar,
                        indices, ref indicesSoFar, USETOStoreVertexCountSoFar, x, y, z, modelOffsetX, modelOffsetY, modelOffsetZ);
                }
            }

            int test = verticesSoFar;
            if (vertices.Length != verticesSoFar || indices.Length != indicesSoFar)
            {
                throw new Exception("number of elements in vertex array does not match number of vertices");
            }
            return new VerticesAndIndices(vertices, indices);
        }

        private static void countFacesOnBlockWithWithinChecks(byte[, ,] array, int spaceWidth, int spaceHeight, int x, int y, int z, ref int numFaces)
        {

            if (withinSpace(x, y, z, spaceWidth, spaceHeight) && array[x, y, z] != (byte)PaintedCubeSpace.AIR)
            {

                if (!withinSpace(x, y + 1, z, spaceWidth, spaceHeight) || array[x, y + 1, z] == (byte)PaintedCubeSpace.AIR)
                {
                    numFaces++;
                }

                if (!withinSpace(x + 1, y, z, spaceWidth, spaceHeight) || array[x + 1, y, z] == (byte)PaintedCubeSpace.AIR)
                {
                    numFaces++;
                }

                //if (withinSpace(i, y, z+1, spaceWidth, spaceHeight))
                if (!withinSpace(x, y, z + 1, spaceWidth, spaceHeight) || array[x, y, z + 1] == (byte)PaintedCubeSpace.AIR)
                {
                    numFaces++;
                }
            }
            else //xyz is air or outside maybeSpace
            {
                if (withinSpace(x, y + 1, z, spaceWidth, spaceHeight))
                    if (array[x, y + 1, z] != (byte)PaintedCubeSpace.AIR)
                    {
                        numFaces++;
                    }

                if (withinSpace(x + 1, y, z, spaceWidth, spaceHeight))
                    if (array[x + 1, y, z] != (byte)PaintedCubeSpace.AIR)
                    {
                        numFaces++;

                    }

                if (withinSpace(x, y, z + 1, spaceWidth, spaceHeight))
                    if (array[x, y, z + 1] != (byte)PaintedCubeSpace.AIR)
                    {
                        numFaces++;
                    }
            }


        }

        private static void createFacesOnBlockWithWithinChecks(byte[, ,] array, int spaceWidth, int spaceHeight, VertexPostitionColorPaintNormal[] vertices, ref int verticesSoFar,
            short[] indices, ref int indicesSoFar, int USETOStoreVertexCountSoFar, int x, int y, int z, int modelOffsetX, int modelOffsetY, int modelOffsetZ)
        {
            if (withinSpace(x, y, z, spaceWidth, spaceHeight) && array[x, y, z] != (byte)PaintedCubeSpace.AIR)
            {

                //if (!withinSpace(i, y + 1, z, spaceWidth, spaceHeight) ||)
                if (!withinSpace(x, y + 1, z, spaceWidth, spaceHeight) || array[x, y + 1, z] == (byte)PaintedCubeSpace.AIR)
                {
                    drawFace(x, y, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y, z, array, spaceWidth, spaceHeight)[1],
                        topIndices, topNormal, topCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                }

                //if (withinSpace(i+1, y, z, spaceWidth, spaceHeight))
                if (!withinSpace(x + 1, y, z, spaceWidth, spaceHeight) || array[x + 1, y, z] == (byte)PaintedCubeSpace.AIR)
                {
                    drawFace(x, y, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y, z, array, spaceWidth, spaceHeight)[0],
                        leftIndices, leftNormal, leftCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                }

                //if (withinSpace(i, y, z+1, spaceWidth, spaceHeight))
                if (!withinSpace(x, y, z + 1, spaceWidth, spaceHeight) || array[x, y, z + 1] == (byte)PaintedCubeSpace.AIR)
                {
                    drawFace(x, y, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y, z, array, spaceWidth, spaceHeight)[2],
                        backIndices, backNormal, backCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                }
            }
            else //xyz is air or outside maybeSpace
            {
                if (withinSpace(x, y + 1, z, spaceWidth, spaceHeight))
                    if (array[x, y + 1, z] != (byte)PaintedCubeSpace.AIR)
                    {
                        drawFace(x, y + 1, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y + 1, z, array, spaceWidth, spaceHeight)[1],
                            bottomIndices, bottomNormal, bottomCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                    }

                if (withinSpace(x + 1, y, z, spaceWidth, spaceHeight))
                    if (array[x + 1, y, z] != (byte)PaintedCubeSpace.AIR)
                    {
                        drawFace(x + 1, y, z, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x + 1, y, z, array, spaceWidth, spaceHeight)[0],
                            rightIndices, rightNormal, rightCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);

                    }

                if (withinSpace(x, y, z + 1, spaceWidth, spaceHeight))
                    if (array[x, y, z + 1] != (byte)PaintedCubeSpace.AIR)
                    {
                        drawFace(x, y, z + 1, ref array, vertices, ref verticesSoFar, indices, ref indicesSoFar, cornersCoveredAlongforAOArray(x, y, z + 1, array, spaceWidth, spaceHeight)[2],
                            frontIndices, frontNormal, frontCorners, ref USETOStoreVertexCountSoFar, modelOffsetX, modelOffsetY, modelOffsetZ);
                    }
            }

        }

        static void drawFace(int x, int y, int z, ref byte[, ,] array, VertexPostitionColorPaintNormal[] vertices, ref int verticesSoFar, short[] indices, ref int indicesSoFar, byte[] AOarray,
            byte[] indicesForThisFace, Vector3 normal, cornr2AOArrLc[] corners, ref int USETOStoreVertexCountSoFar, int modelOffsetX, int modelOffsetY, int modelOffsetZ)
        {
            USETOStoreVertexCountSoFar = verticesSoFar;

            for (int i = 0; i < indicesForThisFace.Length; i++)
            {
                indices[indicesSoFar] = ((short)(USETOStoreVertexCountSoFar + indicesForThisFace[i]));
                indicesSoFar++;
                //indices.Add((short)(USETOStoreVertexCountSoFar + i));
            }

            AddVertexToVertices(x, y, z, ref vertices, ref verticesSoFar, corners[0], ref normal, ref AOarray, array[x, y, z],modelOffsetX, modelOffsetY, modelOffsetZ);
            AddVertexToVertices(x, y, z, ref vertices, ref verticesSoFar, corners[1], ref normal, ref AOarray, array[x, y, z], modelOffsetX, modelOffsetY, modelOffsetZ);
            AddVertexToVertices(x, y, z, ref vertices, ref verticesSoFar, corners[2], ref normal, ref AOarray, array[x, y, z], modelOffsetX, modelOffsetY, modelOffsetZ);
            AddVertexToVertices(x, y, z, ref vertices, ref verticesSoFar, corners[3], ref normal, ref AOarray, array[x, y, z], modelOffsetX, modelOffsetY, modelOffsetZ);

        }


        private static void AddVertexToVertices(int x, int y, int z, ref VertexPostitionColorPaintNormal[] vertices, ref int verticesSoFar,
            cornr2AOArrLc corner, ref Vector3 normal, ref byte[] AOarray, byte type, int modelOffsetX, int modelOffsetY, int modelOffsetZ)
        {
            VertexPostitionColorPaintNormal Zero = new VertexPostitionColorPaintNormal();
            Zero.Position = new Vector3(x + xOffSetsAtCorners[(int)corner], y + yOffSetsAtCorners[(int)corner], z + zOffSetsAtCorners[(int)corner]);
            Zero.Normal = normal;
           // NoiseGenerator.Amplitude=2;
            //float simplex = (float)NoiseGenerator.Noise(x, z)+1;
            //Zero.PaintColor = new Color((int)(simplex * 255), (int)(simplex * 255), (int)(simplex * 255));
            Random rand = new Random(1+(x + xOffSetsAtCorners[(int)corner]+modelOffsetX) 
                * (y + yOffSetsAtCorners[(int)corner]+modelOffsetY) 
                * ( z + zOffSetsAtCorners[(int)corner]+modelOffsetZ));
            Zero.PaintColor = new Color( ColorPallete.colorArray[type].R + rand.Next(-3,3),ColorPallete.colorArray[type].G + rand.Next(-3,3),ColorPallete.colorArray[type].B + rand.Next(-3,3));
            Zero.Color.R = AOarray[(int)corner];
            vertices[verticesSoFar] = Zero;
            verticesSoFar++;
        }

        static byte[][] cornersCoveredAlongforAOArray(int x, int y, int z, byte[, ,] array, int spaceWidth, int spaceHeight)
        {
            //these are in unmippedArray order
            byte[] resultX = new byte[8];
            byte[] resultY = new byte[8];
            byte[] resultZ = new byte[8];

            //return new byte[][] { resultX, resultY, resultZ };

            //byte XlYhZh = 0;//0
            //byte XlYhZl = 0;//1
            //byte XlYlZh = 0;//2
            //byte XlYlZl = 0;//3
            //byte XhYhZh = 0;//4
            //byte XhYhZl = 0;//5
            //byte XhYlZh = 0;//6
            //byte XhYlZl = 0;//7


            //low Y
            if (isOpaqueAtFromWithinChunkContextForAO(x, y - 1, z - 1, array, spaceWidth, spaceHeight))
            {
                //XlYlZl++;
                //XhYlZl++;
                resultZ[(int)cornr2AOArrLc.XlYlZl] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYlZl] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XlYlZl] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XhYlZl] += highAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x - 1, y - 1, z, array, spaceWidth, spaceHeight))
            {
                //XlYlZl++;
                //XlYlZh++;
                resultX[(int)cornr2AOArrLc.XlYlZl] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XlYlZh] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XlYlZl] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XlYlZh] += highAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x, y - 1, z + 1, array, spaceWidth, spaceHeight))
            {
                //XlYlZh++;
                //XhYlZh++;
                resultY[(int)cornr2AOArrLc.XlYlZh] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XhYlZh] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYlZh] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYlZh] += highAOvalue;

            }
            if (isOpaqueAtFromWithinChunkContextForAO(x + 1, y - 1, z, array, spaceWidth, spaceHeight))
            {
                //XhYlZh++;
                //XhYlZl++;
                resultY[(int)cornr2AOArrLc.XhYlZh] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XhYlZl] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XhYlZh] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XhYlZl] += highAOvalue;
            }
            //mid Y
            if (isOpaqueAtFromWithinChunkContextForAO(x - 1, y, z - 1, array, spaceWidth, spaceHeight))
            {
                //XlYlZl++;
                //XlYhZl++;
                resultX[(int)cornr2AOArrLc.XlYlZl] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XlYhZl] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYlZl] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYhZl] += highAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x - 1, y, z + 1, array, spaceWidth, spaceHeight))
            {
                //XlYlZh++;
                //XlYhZh++;
                resultX[(int)cornr2AOArrLc.XlYlZh] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XlYhZh] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYlZh] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYhZh] += highAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x + 1, y, z + 1, array, spaceWidth, spaceHeight))
            {
                //XhYhZh++;
                //XhYlZh++;
                resultX[(int)cornr2AOArrLc.XhYhZh] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XhYlZh] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYhZh] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYlZh] += highAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x + 1, y, z - 1, array, spaceWidth, spaceHeight))
            {
                //XhYhZl++;
                //XhYlZl++;
                resultX[(int)cornr2AOArrLc.XhYhZl] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XhYlZl] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYhZl] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYlZl] += highAOvalue;
            }
            //high Y
            if (isOpaqueAtFromWithinChunkContextForAO(x, y + 1, z - 1, array, spaceWidth, spaceHeight))
            {
                //XlYhZl++;
                //XhYhZl++;
                resultY[(int)cornr2AOArrLc.XlYhZl] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XhYhZl] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYhZl] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYhZl] += highAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x - 1, y + 1, z, array, spaceWidth, spaceHeight))
            {
                //XlYhZl++;
                //XlYhZh++;
                resultX[(int)cornr2AOArrLc.XlYhZl] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XlYhZh] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XlYhZl] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XlYhZh] += highAOvalue;
            }


            if (isOpaqueAtFromWithinChunkContextForAO(x, y + 1, z + 1, array, spaceWidth, spaceHeight))
            {
                //XlYhZh++;
                //XhYhZh++;
                resultY[(int)cornr2AOArrLc.XlYhZh] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XhYhZh] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYhZh] += highAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYhZh] += highAOvalue;

            }
            if (isOpaqueAtFromWithinChunkContextForAO(x + 1, y + 1, z, array, spaceWidth, spaceHeight))
            {
                //XhYhZh++;
                //XhYhZl++;
                resultY[(int)cornr2AOArrLc.XhYhZh] += highAOvalue;
                resultY[(int)cornr2AOArrLc.XhYhZl] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XhYhZh] += highAOvalue;
                resultX[(int)cornr2AOArrLc.XhYhZl] += highAOvalue;
            }

            //===== corners!

            //high Y corners

            if (isOpaqueAtFromWithinChunkContextForAO(x + 1, y + 1, z + 1, array, spaceWidth, spaceHeight))
            {
                resultX[(int)cornr2AOArrLc.XhYhZh] += lowAOvalue;
                resultY[(int)cornr2AOArrLc.XhYhZh] += lowAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYhZh] += lowAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x - 1, y + 1, z + 1, array, spaceWidth, spaceHeight))
            {
                resultX[(int)cornr2AOArrLc.XlYhZh] += lowAOvalue;
                resultY[(int)cornr2AOArrLc.XlYhZh] += lowAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYhZh] += lowAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x + 1, y + 1, z - 1, array, spaceWidth, spaceHeight))
            {
                resultX[(int)cornr2AOArrLc.XhYhZl] += lowAOvalue;
                resultY[(int)cornr2AOArrLc.XhYhZl] += lowAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYhZl] += lowAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x - 1, y + 1, z - 1, array, spaceWidth, spaceHeight))
            {
                resultX[(int)cornr2AOArrLc.XlYhZl] += lowAOvalue;
                resultY[(int)cornr2AOArrLc.XlYhZl] += lowAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYhZl] += lowAOvalue;
            }

            // low Y corners
            if (isOpaqueAtFromWithinChunkContextForAO(x + 1, y - 1, z + 1, array, spaceWidth, spaceHeight))
            {
                resultX[(int)cornr2AOArrLc.XhYlZh] += lowAOvalue;
                resultY[(int)cornr2AOArrLc.XhYlZh] += lowAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYlZh] += lowAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x - 1, y - 1, z + 1, array, spaceWidth, spaceHeight))
            {
                resultX[(int)cornr2AOArrLc.XlYlZh] += lowAOvalue;
                resultY[(int)cornr2AOArrLc.XlYlZh] += lowAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYlZh] += lowAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x + 1, y - 1, z - 1, array, spaceWidth, spaceHeight))
            {
                resultX[(int)cornr2AOArrLc.XhYlZl] += lowAOvalue;
                resultY[(int)cornr2AOArrLc.XhYlZl] += lowAOvalue;
                resultZ[(int)cornr2AOArrLc.XhYlZl] += lowAOvalue;
            }
            if (isOpaqueAtFromWithinChunkContextForAO(x - 1, y - 1, z - 1, array, spaceWidth, spaceHeight))
            {
                resultX[(int)cornr2AOArrLc.XlYlZl] += lowAOvalue;
                resultY[(int)cornr2AOArrLc.XlYlZl] += lowAOvalue;
                resultZ[(int)cornr2AOArrLc.XlYlZl] += lowAOvalue;
            }

            return new byte[][] { resultX, resultY, resultZ };
        }


        static bool isOpaqueAtFromWithinChunkContextForAO(int x, int y, int z, byte[, ,] array, int spaceWidth, int spaceHeight)
        {

            if (withinSpace(x, y, z, spaceWidth, spaceHeight))
            {

                return !isTransparentAtWithoutWithinCheck(x, y, z, array);
            }
            else if (y >= spaceHeight || y < 0)
            {
                return false;
            }
            return false;

        }


        static bool withinSpace(int x, int y, int z, int spaceWidth, int spaceHeight)  // does not account for maybeSpace locInPath
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


        static bool isTransparentAtWithoutWithinCheck(int x, int y, int z, byte[, ,] array)
        {
            return array[x, y, z] == PaintedCubeSpace.AIR;
        }



    }
}
