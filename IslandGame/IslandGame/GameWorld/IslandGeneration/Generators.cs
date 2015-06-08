using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class SmoothWithBluffsGenerator : IslandGenerator
    {

        public override void generateIsland(ChunkSpace chunkSpace, SetPieceManager setPieceManager, JobSiteManager jobSiteManager, IslandLocationProfile locationProfile)
        {
            float magnitude = 1.0f;
            float frequency = .02f;
            float persistance = .25f;

            NoiseGenerator.setValuesForPass(1, 6, magnitude, frequency, persistance);
            NoiseGenerator.randomizeSeed();

            Random rand = new Random();

            float radius = ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2;
            for (int x = 0; x < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; x++)
            {
                for (int z = 0; z < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; z++)
                {

                    float distFromCenter = (float)Math.Sqrt(Math.Pow(radius - x, 2) + Math.Pow(ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2 - z, 2));
                    float ratioFromCenter = (radius - distFromCenter) / (radius); //increases farther out
                    float centerCone = 1.0f - ratioFromCenter;
                    float heightNormal = (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;

                    heightNormal += (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;



                    //heightNormal -= .3f*(1f-ratioFromCenter);

                    float smoothedCone = 1 - ((float)Math.Pow(centerCone, 4f));
                    smoothedCone = (float)MathHelper.Clamp(smoothedCone, .1f, 1) + .03f;
                    float smoothConePurturbation = (float)((float)NoiseGenerator.Noise(x + 903, z + 455) + .5f) / 2.0f;
                    smoothConePurturbation += .5f;
                    heightNormal *= smoothedCone * smoothConePurturbation;


                    float beachHeight = .02f;
                    float lowBeachLimit = .1f;
                    float highBeachLimit = .2f;

                    if (heightNormal < highBeachLimit && heightNormal > lowBeachLimit)
                    {
                        heightNormal = beachHeight;
                    }
                    else
                    {
                        heightNormal -= highBeachLimit - beachHeight;

                    }




                    if (heightNormal >= highBeachLimit)
                    {
                        heightNormal *= 4.0f;
                        heightNormal = (float)Math.Round(heightNormal);
                        heightNormal /= 4.0f;
                    }

                    float erosion = ((float)NoiseGenerator.Noise(z + 644, x + 455) + .5f) * .5f * centerCone;
                    heightNormal -= erosion / (heightNormal + 2.0f);
                    //heightNormal = (float)Math.Pow(heightNormal, 1.3);

                    int heightHere = (int)(heightNormal * ChunkSpace.chunkHeight);

                    for (int y = 0; y < ChunkSpace.chunkHeight; y++)
                    {

                        if (heightHere == y && y > 3)
                        {
                            if (rand.NextDouble() > .999999)
                            {
                                jobSiteManager.placeTree(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())), Tree.treeTypes.maple);
                            }
                        }

                        if (heightHere > y)
                        {
                            if (y <= 1)
                            {
                                chunkSpace.setBlockAt(regularSandColor, x, y, z);//sand
                            }
                            else
                            {
                                if (heightHere - 1 > y)
                                {
                                    chunkSpace.setBlockAt(7, x, y, z);
                                }
                                else
                                {
                                    if (rand.NextDouble() > .999)
                                    {

                                        jobSiteManager.placeTree(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())), Tree.treeTypes.maple);
                                    }
                                    else if (rand.NextDouble() > .995)
                                    {
                                        setPieceManager.placeDecorativePlant(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())));
                                    }
                                    float grassRand = (float)rand.NextDouble();
                                    if (!(heightNormal + .1f < highBeachLimit && heightNormal + .1f > lowBeachLimit))
                                    {

                                        chunkSpace.setBlockAt(184, x, y, z);

                                    }
                                    else
                                    {
                                        chunkSpace.setBlockAt(187, x, y, z);
                                    }
                                }

                            }

                        }
                        else if (y == 0)
                        {
                            chunkSpace.setBlockAt(PaintedCubeSpace.AIR, x, y, z);

                        }
                    }



                }

            }
        }

    }

    [Serializable]
    public class Volcanic : IslandGenerator
    {
        public override void generateIsland(ChunkSpace chunkSpace, SetPieceManager setPieceManager, JobSiteManager jobSiteManager, IslandLocationProfile locationProfile)
        {
            float magnitude = 1.0f;
            float frequency = .02f;
            float persistance = .25f;

            NoiseGenerator.setValuesForPass(1, 6, magnitude, frequency, persistance);
            NoiseGenerator.randomizeSeed();

            Random rand = new Random();

            float radius = ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2;
            for (int x = 0; x < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; x++)
            {
                for (int z = 0; z < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; z++)
                {

                    float distFromCenter = (float)Math.Sqrt(Math.Pow(radius - x, 2)
                        + Math.Pow(ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2 - z, 2));
                    float ratioFromCenter = (radius - distFromCenter) / (radius); //increases farther out
                    ratioFromCenter = MathHelper.Clamp(ratioFromCenter, 0, 1);
                    float smoothDome = (float)Math.Pow(ratioFromCenter, 1.0f / 7.0f);
                    smoothDome = (float)MathHelper.Clamp(smoothDome, 0, 1);

                    float heightNormal = 0;

                    generateCentralMountain(ref heightNormal, ratioFromCenter, x, z);


                    heightNormal = normalClamp(heightNormal);
                    float baseDome = (float)Math.Pow(ratioFromCenter, 1.0f / 2.0f);
                    baseDome = (float)MathHelper.Clamp(baseDome, 0, 1);
                    float baseLand = (float)(NoiseGenerator.Noise(x + 234, z + 3432) + .2f) / 2.0f;
                    baseLand *= baseDome;
                    baseDome *= baseLand;
                    float erosion = (float)(NoiseGenerator.Noise(x * 5 + 456, z * 5 + 445) + .5f) / 8f;


                    heightNormal += baseLand;
                    heightNormal -= erosion * (1.0f - ratioFromCenter * ratioFromCenter + .1f);
                    //heightNormal -= 5*erosion*((float)Math.Pow(ratioFromCenter,4));
                    heightNormal = normalClamp(heightNormal);

                    //heightNormal += (float)(NoiseGenerator.Noise(i*5+456 , z*5+445)  +.5f)/8f;

                    float volcanoRimHeight = .9f;
                    if (heightNormal > volcanoRimHeight)
                    {
                        heightNormal -= volcanoRimHeight;
                        heightNormal *= -.6f;
                        heightNormal += volcanoRimHeight;
                    }




                    int heightHere = (int)(heightNormal * ChunkSpace.chunkHeight);

                    for (int y = 0; y < ChunkSpace.chunkHeight; y++)
                    {

                        if (heightHere == y && y > 3)
                        {
                            if (rand.NextDouble() > .999999)
                            {
                                jobSiteManager.placeTree(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())), Tree.treeTypes.maple);
                            }
                        }

                        if (heightHere > y)
                        {
                            if (y <= 1)
                            {
                                chunkSpace.setBlockAt(41, x, y, z);
                            }
                            else
                            {
                                if (heightHere - 1 > y)
                                {
                                    chunkSpace.setBlockAt(3, x, y, z);
                                }
                                else
                                {
                                    if (rand.NextDouble() > .999)
                                    {
                                        //treesJobSite.placeTree(new IntVector3(i, y, z));
                                    }
                                    chunkSpace.setBlockAt(4, x, y, z);
                                }

                            }

                        }
                        else if (y == 0)
                        {
                            chunkSpace.setBlockAt(PaintedCubeSpace.AIR, x, y, z);

                        }
                    }



                }

            }
        }
    }

    [Serializable]
    public class PlainsGenerator : IslandGenerator
    {

        public override void generateIsland(ChunkSpace chunkSpace, SetPieceManager setPieceManager, JobSiteManager jobSiteManager, IslandLocationProfile locationProfile)
        {
            float magnitude = 1.0f;
            float frequency = .02f;
            float persistance = .25f;

            NoiseGenerator.setValuesForPass(1, 6, magnitude, frequency, persistance);
            NoiseGenerator.randomizeSeed();

            Random rand = new Random();

            float radius = ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2;
            for (int x = 0; x < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; x++)
            {
                for (int z = 0; z < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; z++)
                {

                    float distFromCenter = (float)Math.Sqrt(Math.Pow(radius - x, 2)
                        + Math.Pow(ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2 - z, 2));
                    float ratioFromCenter = (radius - distFromCenter) / (radius); //increases farther out
                    ratioFromCenter = MathHelper.Clamp(ratioFromCenter, 0, 1);
                    float smoothDome = (float)Math.Pow(ratioFromCenter, 1.0f / 10.0f);
                    smoothDome = (float)MathHelper.Clamp(smoothDome, 0, 1);

                    float heightNormal = 0;

                    heightNormal = .1f;//smoothDome/10.0f;
                    float erosionMag = (float)(NoiseGenerator.Noise(x + 4456, z + 445) + .5f) / 2f + (1.0f - ratioFromCenter);
                    float erosion = (float)Math.Pow((1.0f - ratioFromCenter), 3.0) * ((float)(NoiseGenerator.Noise(x * 3 + 34, z * 3 + 554)) / 2f + .1f);
                    erosion += (1.0f - smoothDome) / 4.0f - .01f;
                    erosion = normalClamp(erosion);
                    heightNormal -= erosion;
                    heightNormal *= smoothDome;


                    int heightHere = (int)(heightNormal * ChunkSpace.chunkHeight);

                    for (int y = 0; y < ChunkSpace.chunkHeight; y++)
                    {

                        if (heightHere == y && y > 3)
                        {
                            if (rand.NextDouble() > .999999)
                            {
                                jobSiteManager.placeTree(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())), Tree.treeTypes.maple);
                            }
                        }

                        if (heightHere > y)
                        {
                            if (y <= 1)
                            {
                                chunkSpace.setBlockAt(regularSandColor, x, y, z);
                            }
                            else
                            {
                                if (heightHere - 1 > y)
                                {
                                    chunkSpace.setBlockAt(5, x, y, z);
                                }

                                else
                                {
                                    if (rand.NextDouble() > .9995)
                                    {

                                        jobSiteManager.placeTree(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())), Tree.treeTypes.maple);
                                    }
                                    else if (rand.NextDouble() > .98)
                                    {
                                        setPieceManager.placeDecorativePlant(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())));
                                    }

                                    chunkSpace.setBlockAt((y > 2) ? (byte)184 : (byte)194, x, y, z);
                                }

                            }

                        }
                        else if (y == 0)
                        {
                            chunkSpace.setBlockAt(PaintedCubeSpace.AIR, x, y, z);

                        }
                    }



                }

            }
        }

    }


    [Serializable]
    public class HillyGenerator : IslandGenerator
    {

        public override void generateIsland(ChunkSpace chunkSpace, SetPieceManager setPieceManager, JobSiteManager jobSiteManager, IslandLocationProfile locationProfile)
        {
            NoiseGenerator.setValuesForPass(1, 6, 1, .02f, .3f);
            NoiseGenerator.randomizeSeed();

            Random rand = new Random();

            float radius = ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2;
            for (int x = 0; x < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; x++)
            {
                for (int z = 0; z < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; z++)
                {

                    float distFromCenter = (float)Math.Sqrt(Math.Pow(radius - x, 2) + Math.Pow(ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2 - z, 2));
                    float ratioFromCenter = (radius - distFromCenter) / (radius);
                    float heightNormal = (float)Math.Pow((.5f / MathHelper.Clamp((float)NoiseGenerator.Noise(x, z) + 1f, .1f, 1)), 2);

                    //// heightNormal *= -((float)Math.Pow( (1-ratioFromCenter+.5f), .7)) + 1;


                    //heightNormal += (float)Math.Pow((MathHelper.Clamp((float)NoiseGenerator.Noise(i + 345, z + 4395), 0, 1)), ratioFromCenter)*.2f;
                    heightNormal -= (float)Math.Pow((MathHelper.Clamp((float)NoiseGenerator.Noise(x * 2 + 100, z * 2 + 200) + .5f, 0, 1)), 1) * ratioFromCenter * .1f;

                    // heightNormal += ((float)Math.Pow((radius - distFromCenter) / (radius),1))*(float)Math.Pow((MathHelper.Clamp(.5f*(float)NoiseGenerator.Noise(i*2+654, z*2+7395)+.5f, 0, .6f)), 1)*.4f;

                    //float heightMultiplier = (float)Math.Pow((float)NoiseGenerator.Noise(i + 400, z + 4595)+.5f,1);


                    //heightNormal *= heightMultiplier;


                    // heightNormal += .1f;
                    // heightNormal *= -(1-(float)Math.Pow((radius - distFromCenter) / (radius),1))+1;
                    if (distFromCenter > radius)
                    {
                        heightNormal = 0;
                    }

                    heightNormal -= 1.3f * (1 - (float)Math.Pow((radius - distFromCenter) / (radius), 1)) * MathHelper.Clamp(.5f * (float)NoiseGenerator.Noise(x + 200, z + 300) + .5f, 0f, 1f);

                    heightNormal = (float)MathHelper.Clamp(heightNormal, 0, 1);

                    float erosion = ((float)NoiseGenerator.Noise(z + 200, x + 455) + .5f) * heightNormal;
                    // erosion += ((float)NoiseGenerator.Noise(z + 50, i) + 1) * .1f;
                    // heightNormal -= erosion;

                    heightNormal = (float)Math.Pow(heightNormal, 1.3);

                    for (int y = 0; y < ChunkSpace.chunkHeight; y++)
                    {

                        if ((int)(heightNormal * ChunkSpace.chunkHeight) == y && y > 3)
                        {
                            if (rand.NextDouble() > .999)
                            {
                                jobSiteManager.placeTree(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())), Tree.treeTypes.maple);
                            }
                        }

                        if (heightNormal > (float)y / (float)ChunkSpace.chunkHeight)
                        {
                            chunkSpace.setBlockAt((byte)(y <= 1 ? regularSandColor : 184), x, y, z);

                        }
                        else if (y == 0)
                        {
                            chunkSpace.setBlockAt(PaintedCubeSpace.AIR, x, y, z);

                        }
                    }



                }

            }
            //treesJobSite.placeTree(new IntVector3((int)getBoundingBox().Max.X, 1, (int)getBoundingBox().Max.Z));
        }

    }

    [Serializable]
    public class SmoothWithShorterBluffsGenerator : IslandGenerator
    {




        public override void generateIsland(ChunkSpace chunkSpace, SetPieceManager setPieceManager, JobSiteManager jobSiteManager, IslandLocationProfile locationProfile)
        {
            float magnitude = 1.0f;
            float frequency = .02f;
            float persistance = .25f;

            NoiseGenerator.setValuesForPass(1, 6, magnitude, frequency, persistance);
            NoiseGenerator.randomizeSeed();

            Random rand = new Random();

            float radius = ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2;
            for (int x = 0; x < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; x++)
            {
                for (int z = 0; z < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; z++)
                {

                    float distFromCenter = (float)Math.Sqrt(Math.Pow(radius - x, 2) + Math.Pow(ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2 - z, 2));
                    float ratioFromCenter = (radius - distFromCenter) / (radius); //increases farther out
                    float centerCone = 1.0f - ratioFromCenter;
                    float heightNormal = (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;

                    heightNormal += (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;



                    //heightNormal -= .3f*(1f-ratioFromCenter);

                    float smoothedCone = 1 - ((float)Math.Pow(centerCone, 4f));
                    smoothedCone = (float)MathHelper.Clamp(smoothedCone, .1f, 1) + .03f;
                    float smoothConePurturbation = (float)((float)NoiseGenerator.Noise(x + 903, z + 455) + .5f);
                    //smoothConePurturbation+=.5f;
                    heightNormal = smoothConePurturbation;
                    heightNormal += 1;
                    heightNormal *= 4;

                    // heightNormal *=3;
                    //  heightNormal =(float) Math.Pow(heightNormal,.1f);
                    //  heightNormal /=3;

                    heightNormal = 1f / heightNormal;

                    heightNormal += 1;
                    heightNormal = (float)Math.Pow(heightNormal, 2f);
                    heightNormal -= 1;

                    heightNormal *= smoothedCone;


                    float beachHeight = .02f;
                    float lowBeachLimit = .1f;
                    float highBeachLimit = .2f;//
                    if (heightNormal < highBeachLimit && heightNormal > lowBeachLimit)
                    {
                        heightNormal = beachHeight;
                    }
                    else
                    {
                        heightNormal -= highBeachLimit - beachHeight;
                        //heightNormal = (float)MathHelper.Clamp(heightNormal,.001f,1);
                    }





                    if (heightNormal >= highBeachLimit)
                    {
                        heightNormal *= 4.0f;
                        heightNormal = (float)Math.Round(heightNormal);
                        heightNormal /= 4.0f;
                    }
                    else if (heightNormal > lowBeachLimit)
                    {
                        //heightNormal/=10.0f;
                        //highBeachLimit+=beachHeight;
                    }


                    if (distFromCenter > radius)
                    {
                        //heightNormal = 0;
                    }

                    float erosion = ((float)NoiseGenerator.Noise(z + 644, x + 455) + .5f) * .5f * centerCone;
                    heightNormal -= erosion / (heightNormal + 2.0f);
                    //heightNormal = (float)Math.Pow(heightNormal, 1.3);

                    int heightHere = (int)(heightNormal * ChunkSpace.chunkHeight);

                    for (int y = 0; y < ChunkSpace.chunkHeight; y++)
                    {

                        if (heightHere == y && y > 3)
                        {
                            if (rand.NextDouble() > .997)
                            {
                                //jobSiteManager.placeTree(new BlockLoc( locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())));
                            }
                        }

                        if (heightHere > y)
                        {
                            if (y <= 1)
                            {
                                chunkSpace.setBlockAt(56, x, y, z);//sand
                            }
                            else
                            {
                                if (heightHere - 1 > y)
                                {
                                    chunkSpace.setBlockAt(7, x, y, z);
                                }
                                else
                                {
                                    if (rand.NextDouble() > .9999)
                                    {
                                        //jobSiteManager.placeTree(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())));
                                    }
                                    chunkSpace.setBlockAt(184, x, y, z);
                                }

                            }

                        }
                        else if (y == 0)
                        {
                            chunkSpace.setBlockAt(PaintedCubeSpace.AIR, x, y, z);

                        }
                    }



                }

            }
        }


    }



    [Serializable]
    public class PoplarForestGenerator : IslandGenerator
    {


        public override void generateIsland(ChunkSpace chunkSpace, SetPieceManager setPieceManager, JobSiteManager jobSiteManager, IslandLocationProfile locationProfile)
        {
            float magnitude = 1.0f;
            float frequency = .02f;
            float persistance = .25f;

            NoiseGenerator.setValuesForPass(1, 6, magnitude, frequency, persistance);
            NoiseGenerator.randomizeSeed();

            Random rand = new Random();

            float radius = ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2;
            for (int x = 0; x < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; x++)
            {
                for (int z = 0; z < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; z++)
                {

                    float distFromCenter = (float)Math.Sqrt(Math.Pow(radius - x, 2) + Math.Pow(ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2 - z, 2));
                    float ratioFromCenter = (radius - distFromCenter) / (radius); //increases farther out
                    float centerCone = 1.0f - ratioFromCenter;
                    float heightNormal = (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;

                    heightNormal += (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;



                    //heightNormal -= .3f*(1f-ratioFromCenter);

                    float smoothedCone = 1 - ((float)Math.Pow(centerCone, 4f));
                    smoothedCone = (float)MathHelper.Clamp(smoothedCone, .1f, 1) + .03f;
                    float smoothConePurturbation = (float)((float)NoiseGenerator.Noise(x + 903, z + 455) + .5f);
                    //smoothConePurturbation+=.5f;
                    heightNormal = smoothConePurturbation;
                    heightNormal += 1;
                    heightNormal *= 4;

                    // heightNormal *=3;
                    //  heightNormal =(float) Math.Pow(heightNormal,.1f);
                    //  heightNormal /=3;

                    heightNormal = 1f / heightNormal;

                    heightNormal += 1;
                    heightNormal = (float)Math.Pow(heightNormal, 2f);
                    heightNormal -= 1;

                    heightNormal *= smoothedCone;


                    float beachHeight = .02f;
                    float lowBeachLimit = .1f;
                    float highBeachLimit = .2f;//
                    if (heightNormal < highBeachLimit && heightNormal > lowBeachLimit)
                    {
                        heightNormal = beachHeight;
                    }
                    else
                    {
                        heightNormal -= highBeachLimit - beachHeight;
                        //heightNormal = (float)MathHelper.Clamp(heightNormal,.001f,1);
                    }





                    if (heightNormal >= highBeachLimit)
                    {
                        heightNormal *= 4.0f;
                        heightNormal = (float)Math.Round(heightNormal);
                        heightNormal /= 4.0f;
                    }
                    else if (heightNormal > lowBeachLimit)
                    {
                        //heightNormal/=10.0f;
                        //highBeachLimit+=beachHeight;
                    }


                    if (distFromCenter > radius)
                    {
                        //heightNormal = 0;
                    }

                    float erosion = ((float)NoiseGenerator.Noise(z + 644, x + 455) + .5f) * .5f * centerCone;
                    heightNormal -= erosion / (heightNormal + 2.0f);
                    //heightNormal = (float)Math.Pow(heightNormal, 1.3);

                    int heightHere = (int)(heightNormal * ChunkSpace.chunkHeight);

                    for (int y = 0; y < ChunkSpace.chunkHeight; y++)
                    {

                        if (heightHere == y && y > 3)
                        {
                            if (rand.NextDouble() > .997)
                            {
                                jobSiteManager.placeTree(
                                 new BlockLoc(locationProfile.profileSpaceToWorldSpace(
                                        new IntVector3(x, y, z).toVector3())),
                                         Tree.treeTypes.poplar);//
                            }
                        }

                        if (heightHere > y)
                        {
                            if (y <= 1)
                            {
                                chunkSpace.setBlockAt(53, x, y, z);//sand
                            }
                            else
                            {
                                if (heightHere - 1 > y)
                                {
                                    chunkSpace.setBlockAt(7, x, y, z);
                                }
                                else
                                {
                                    if (rand.NextDouble() < .03)
                                    {
                                        float groundColorPick = (float)rand.NextDouble();
                                        if (groundColorPick < .5)
                                        {
                                            chunkSpace.setBlockAt(59, x, y, z);
                                        }

                                        else
                                        {
                                            chunkSpace.setBlockAt(60, x, y, z);
                                        }
                                    }
                                    else
                                    {
                                        if (rand.NextDouble() > .98)
                                        {
                                            setPieceManager.placeDecorativePlant(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())));
                                        }
                                        byte grassPick = ((float)NoiseGenerator.Noise(z * 3 + 443, x * 3 + 72) + rand.NextDouble() * .1 > 0) ? (byte)184 : (byte)200;
                                        chunkSpace.setBlockAt((y > 2) ? (byte)grassPick : (byte)199, x, y, z);
                                    }

                                }

                            }

                        }
                        else if (y == 0)
                        {
                            chunkSpace.setBlockAt(PaintedCubeSpace.AIR, x, y, z);

                        }
                    }



                }

            }
        }


    }

    [Serializable]
    public class PineForestGenerator : IslandGenerator
    {

        public override void generateIsland(ChunkSpace chunkSpace, SetPieceManager setPieceManager, JobSiteManager jobSiteManager, IslandLocationProfile locationProfile)
        {
            float magnitude = 1.0f;
            float frequency = .02f;
            float persistance = .25f;

            NoiseGenerator.setValuesForPass(1, 6, magnitude, frequency, persistance);
            NoiseGenerator.randomizeSeed();

            Random rand = new Random();

            float radius = ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2;
            for (int x = 0; x < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; x++)
            {
                for (int z = 0; z < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; z++)
                {

                    float distFromCenter = (float)Math.Sqrt(Math.Pow(radius - x, 2) + Math.Pow(ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2 - z, 2));
                    float ratioFromCenter = (radius - distFromCenter) / (radius); //increases farther out
                    float centerCone = 1.0f - ratioFromCenter;
                    float heightNormal = (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;

                    heightNormal += (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;



                    //heightNormal -= .3f*(1f-ratioFromCenter);

                    float smoothedCone = 1 - ((float)Math.Pow(centerCone, 4f));
                    smoothedCone = (float)MathHelper.Clamp(smoothedCone, .1f, 1) + .03f;
                    float smoothConePurturbation = (float)((float)NoiseGenerator.Noise(x + 903, z + 455) + .5f);
                    //smoothConePurturbation+=.5f;
                    heightNormal = smoothConePurturbation;
                    heightNormal += 1;
                    heightNormal *= 4;

                    // heightNormal *=3;
                    //  heightNormal =(float) Math.Pow(heightNormal,.1f);
                    //  heightNormal /=3;

                    heightNormal = 1f / heightNormal;

                    heightNormal += 1;
                    heightNormal = (float)Math.Pow(heightNormal, 2f);
                    heightNormal -= 1;

                    heightNormal *= smoothedCone;


                    float beachHeight = .02f;
                    float lowBeachLimit = .1f;
                    float highBeachLimit = .2f;//
                    if (heightNormal < highBeachLimit && heightNormal > lowBeachLimit)
                    {
                        heightNormal = beachHeight;
                    }
                    else
                    {
                        heightNormal -= highBeachLimit - beachHeight;
                        //heightNormal = (float)MathHelper.Clamp(heightNormal,.001f,1);
                    }





                    if (heightNormal >= highBeachLimit)
                    {
                        heightNormal *= 4.0f;
                        heightNormal = (float)Math.Round(heightNormal);
                        heightNormal /= 4.0f;
                    }
                    else if (heightNormal > lowBeachLimit)
                    {
                        //heightNormal/=10.0f;
                        //highBeachLimit+=beachHeight;
                    }


                    if (distFromCenter > radius)
                    {
                        //heightNormal = 0;
                    }

                    float erosion = ((float)NoiseGenerator.Noise(z * 2 + 644, x * 2 + 455) + .5f) * .5f * centerCone;
                    heightNormal -= erosion / (heightNormal + 2.0f);
                    //heightNormal = (f`loat)Math.Pow(heightNormal, 1.3);
                    heightNormal = (float)Math.Pow(heightNormal * ChunkSpace.chunkHeight, 1.1f) / ChunkSpace.chunkHeight;
                    heightNormal *= 1.6f;

                    heightNormal -= ((float)NoiseGenerator.Noise(z * 2 + 454, x * 2 + 4445) + .5f) * .2f;
                    int heightHere = (int)(heightNormal * ChunkSpace.chunkHeight);

                    for (int y = 0; y < ChunkSpace.chunkHeight; y++)
                    {

                        if (heightHere == y && y > 3)
                        {
                            if (rand.NextDouble() > .985)
                            {
                                jobSiteManager.placeTree(
                                 new BlockLoc(locationProfile.profileSpaceToWorldSpace(
                                        new IntVector3(x, y, z).toVector3())),
                                         Tree.treeTypes.pine);//
                            }
                        }

                        if (heightHere > y)
                        {
                            if (y <= 1)
                            {
                                chunkSpace.setBlockAt(20, x, y, z);//beach area
                            }
                            else
                            {
                                if (heightHere - 1 > y)
                                {
                                    chunkSpace.setBlockAt(40, x, y, z);
                                }
                                else
                                {


                                    if (rand.NextDouble() > .98)
                                    {
                                        setPieceManager.placeDecorativePlant(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())));
                                    }

                                    chunkSpace.setBlockAt((y > 2) ? (byte)210 : (byte)211, x, y, z);


                                }

                            }

                        }//
                        else if (y == 0)
                        {
                            chunkSpace.setBlockAt(PaintedCubeSpace.AIR, x, y, z);

                        }
                    }



                }

            }
        }
    }

    [Serializable]
    public class SnowyIslandGenerator : IslandGenerator
    {


        public override void generateIsland(ChunkSpace chunkSpace, SetPieceManager setPieceManager, JobSiteManager jobSiteManager, IslandLocationProfile locationProfile)
        {
            float magnitude = 2.0f;
            float frequency = .02f;
            float persistance = .25f;

            NoiseGenerator.setValuesForPass(1, 6, magnitude, frequency, persistance);
            NoiseGenerator.randomizeSeed();

            Random rand = new Random();

            float radius = ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2;
            for (int x = 0; x < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; x++)
            {
                for (int z = 0; z < ChunkSpace.chunkWidth * chunkSpace.widthInChunks; z++)
                {

                    float distFromCenter = (float)Math.Sqrt(Math.Pow(radius - x, 2) + Math.Pow(ChunkSpace.chunkWidth * chunkSpace.widthInChunks / 2 - z, 2));
                    float ratioFromCenter = (radius - distFromCenter) / (radius); //increases farther out
                    float centerCone = 1.0f - ratioFromCenter;
                    float heightNormal = (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;

                    heightNormal += (float)((float)NoiseGenerator.Noise(x, z) + .5f) / 2.0f;

                    float smoothedCone = 1 - ((float)Math.Pow(centerCone, 4f));
                    smoothedCone = (float)MathHelper.Clamp(smoothedCone, .1f, 1) + .03f;
                    float smoothConePurturbation = (float)((float)NoiseGenerator.Noise(x + 903, z + 455) + .5f);

                    heightNormal = smoothConePurturbation;
                    heightNormal += 1;
                    heightNormal *= 3;


                    heightNormal = 1f / heightNormal;

                    heightNormal += 1;
                    heightNormal = (float)Math.Pow(heightNormal, 1.5f);
                    heightNormal -= 1;

                    heightNormal *= smoothedCone;


                    float beachHeight = .02f;
                    float lowBeachLimit = .1f;
                    float highBeachLimit = .2f;//
                    if (heightNormal < highBeachLimit && heightNormal > lowBeachLimit)
                    {
                        heightNormal = beachHeight;
                    }
                    else
                    {
                        heightNormal -= highBeachLimit - beachHeight; ;
                    }





                    if (heightNormal >= highBeachLimit)
                    {
                        heightNormal *= 4.0f;
                        heightNormal = (float)Math.Round(heightNormal);
                        heightNormal /= 4.0f;
                    }

                    float erosion = ((float)NoiseGenerator.Noise(z * 2 + 644, x * 2 + 455) + .5f) * .5f * centerCone;
                    heightNormal *= 9;
                    heightNormal = (int)heightNormal;
                    heightNormal /= 10;

                    erosion = ((float)Noise.Generate((float)z * .05f, (float)x * .05f) + 1) / 2.0f * .06f * (centerCone);
                    erosion = ((float)Noise.Generate((float)z * .03f, (float)x * .03f) + 1) / 2.0f * .05f * (centerCone);

                    heightNormal = MathHelper.Clamp(heightNormal, 0, 1);

                    heightNormal -= erosion / heightNormal;

                    heightNormal -= .05f;

                    heightNormal += .1f;

                    int heightHere = (int)(heightNormal * ChunkSpace.chunkHeight);
                    for (int y = 0; y < ChunkSpace.chunkHeight; y++)
                    {
                        if (Noise.Generate((float)x * .05f, (float)y * .05f, (float)z * .05f) < .6f)
                        {
                            if (heightHere == y && y > 3)
                            {
                                if (rand.NextDouble() > .97 + .03 * (1.0f - heightNormal))
                                {
                                    if (rand.NextDouble() > .8f)
                                    //deciding which type of tree to place
                                    {
                                        jobSiteManager.placeTree(
                                         new BlockLoc(locationProfile.profileSpaceToWorldSpace(
                                                new IntVector3(x, y, z).toVector3())),
                                                 Tree.treeTypes.snowyLargePine);
                                    }
                                    else
                                    {
                                        jobSiteManager.placeTree(
                                         new BlockLoc(locationProfile.profileSpaceToWorldSpace(
                                                new IntVector3(x, y, z).toVector3())),
                                                 Tree.treeTypes.snowyPine);
                                    }
                                }
                            }
                            if (heightHere > y)
                            {
                                if (y <= 1)
                                {
                                    chunkSpace.setBlockAt(37, x, y, z);//beach area
                                }
                                else
                                {
                                    if (heightHere - 1 > y)
                                    {
                                        chunkSpace.setBlockAt(34, x, y, z);
                                    }
                                    else
                                    {


                                        if (rand.NextDouble() > .999)
                                        {
                                            setPieceManager.placeDecorativePlant(new BlockLoc(locationProfile.profileSpaceToWorldSpace(new IntVector3(x, y, z).toVector3())));
                                        }

                                        chunkSpace.setBlockAt((y > 2) ? (byte)15 : (byte)14, x, y, z);


                                    }

                                }

                            }//
                            else if (y == 0)
                            {
                                chunkSpace.setBlockAt(PaintedCubeSpace.AIR, x, y, z);

                            }
                        }
                    }
                }

            }
        }
    }
        
    
}

