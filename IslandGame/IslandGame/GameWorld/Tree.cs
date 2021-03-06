﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class Tree : SetPiece
    {
        int health = 10;
        BlockLoc FootLocation;
        float timeSinceDeath=0;
        private float timeToFall = 56;


        public enum treeTypes
        {
            maple,
            poplar,
            pine,
            snowyPine,
            snowyLargePine,
            redwood,
            bayou
        }

        public Tree(BlockLoc nFootLocation, treeTypes type)
        {
            FootLocation =nFootLocation;
            Random rand = new Random();
            float height = 5;
            switch (type)
            {
                case treeTypes.maple:
                    height = 5;
                    string[] possibleTrees = { "tree1", "tree2" };
                    string treeName = possibleTrees[rand.Next(possibleTrees.Length)];//"fallTree1";
                    setupSetPiece(new AxisAlignedBoundingBox(FootLocation.toWorldSpaceVector3(), FootLocation.toWorldSpaceVector3() + new Vector3(1f, height, 1f)),
                        ContentDistributor.getEmptyString()+@"trees\" + treeName + ".chr" , .8f + (float)new Random().NextDouble() * .4f);
                    setRootPartRotationOffset(Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY((float)new Random().NextDouble() * 10.0f)));
                    break;
                case treeTypes.poplar:
                    height = 5;

                    string[] poplarNames = { "tree", "tree2", "tree3" };
                    string poplarName = poplarNames[rand.Next(poplarNames.Length)];//"fallTree1";
                    setupSetPiece(new AxisAlignedBoundingBox(FootLocation.toWorldSpaceVector3(), FootLocation.toWorldSpaceVector3() + new Vector3(1f, height, 1f)),
                        ContentDistributor.getEmptyString()+@"poplarTree\" + poplarName + ".chr", .2f);
                    setRootPartRotationOffset(Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY((float)new Random().NextDouble() * 10.0f)));
                    break;
                case treeTypes.pine:
                    height = 5;

                    string[] pineNames = { "tree" };
                    string pineName = pineNames[rand.Next(pineNames.Length)];//"fallTree1";
                    String path = ContentDistributor.getEmptyString() + @"pineTree\" + pineName + ".chr";
                    setupSetPiece(new AxisAlignedBoundingBox(FootLocation.toWorldSpaceVector3(), FootLocation.toWorldSpaceVector3() + new Vector3(1f, height, 1f)),
                        path, .4f + (float)rand.NextDouble()*.3f);
                    setRootPartRotationOffset(Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY((float)new Random().NextDouble() * 10.0f)));
                    break;
                case treeTypes.redwood:
                    height = 5;

                    string[] redwoodNames = { "tree" };
                    string redwoodName = redwoodNames[rand.Next(redwoodNames.Length)];//"fallTree1";
                    setupSetPiece(new AxisAlignedBoundingBox(FootLocation.toWorldSpaceVector3(), FootLocation.toWorldSpaceVector3() + new Vector3(1f, height, 1f)),
                        ContentDistributor.getEmptyString()+@"redwood\" + redwoodName + ".chr", .2f);
                    setRootPartRotationOffset(Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY((float)new Random().NextDouble() * 10.0f)));
                    break;
                case treeTypes.snowyPine:
                    height = 5;

                    string[] snowPineTreeNames = { "snowyPineTree" };
                    string snowPineTreeName = snowPineTreeNames[rand.Next(snowPineTreeNames.Length)];//"fallTree1";
                    String snowPinePath = ContentDistributor.getEmptyString() + @"pineTree\" + snowPineTreeName + ".chr";
                    setupSetPiece(new AxisAlignedBoundingBox(FootLocation.toWorldSpaceVector3(), FootLocation.toWorldSpaceVector3() + new Vector3(1f, height, 1f)),
                        snowPinePath, .2f + (float)rand.NextDouble() * .2f);
                    setRootPartRotationOffset(Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY((float)new Random().NextDouble() * 10.0f)));
                    break;
                case treeTypes.snowyLargePine:
                    height = 5;

                    string[] snowLargePineTreeNames = { "snowyDiskBasedPineTree" };
                    string snowyLargePineTreeName = snowLargePineTreeNames[rand.Next(snowLargePineTreeNames.Length)];//"fallTree1";
                    String snowyLargePinePath = ContentDistributor.getEmptyString() + @"pineTree\" + snowyLargePineTreeName + ".chr";
                    setupSetPiece(new AxisAlignedBoundingBox(FootLocation.toWorldSpaceVector3(), FootLocation.toWorldSpaceVector3() + new Vector3(1f, height, 1f)),
                        snowyLargePinePath, .5f + (float)rand.NextDouble() * .3f);
                    setRootPartRotationOffset(Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY((float)new Random().NextDouble() * 10.0f)));
                    break;
                default:
                    break;
            }
        }

        public List<BlockLoc> getTrunkBlocks()
        {
            int numBlocksUpFromFoot = 10;
            List<BlockLoc> trunkBlocks = new List<BlockLoc>(numBlocksUpFromFoot);
            for (int i = 0; i < numBlocksUpFromFoot; i++)
            {
                trunkBlocks.Add(BlockLoc.AddIntVec3(FootLocation,new IntVector3(0, i, 0))); 
            }
            return trunkBlocks;
        }

        public void getChopped()
        {
            health--;
        }

        public override void update()
        {
            base.update();
            if (health <= 0)
            {
                timeSinceDeath++;
                setRootPartRotationOffset( Quaternion.CreateFromRotationMatrix(
                    Matrix.CreateRotationX((float)MathHelper.PiOver2*
                    MathHelper.Clamp((float)Math.Pow(timeSinceDeath,1.4)/5.0f, 0, timeToFall)
                    / timeToFall) /*  * Matrix.CreateRotationY((float)new Random(FootLocation.GetHashCode()).NextDouble() * 5.0f)*/
                    
                    ));
                if (timeSinceDeath > timeToFall)
                {
                    health = -100000;
                }
            }
        }

        public bool needsToBeDeleted()
        {
            return health <= -100;
        }


    }
}
