using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    class DecorativePlant : SetPiece
    {
        BlockLoc blockRootedIn;

        public DecorativePlant(BlockLoc FootLocation)
        {
            blockRootedIn = FootLocation;
            float height = 5;
            Random rand = new Random();
            string[] possiblePlants = { "yellowFlower", "blueFlower", "shortLeafyPlant", "bush" };
            string plantName = possiblePlants[rand.Next(possiblePlants.Length)];
            setupSetPiece(new AxisAlignedBoundingBox(FootLocation.toWorldSpaceVector3() + new Vector3(0, 1, 0), FootLocation.toWorldSpaceVector3() + new Vector3(0, 1, 0) + new Vector3(1f, height, 1f)),
                ContentDistributor.getRootPath()+ @"world_decoration\" + plantName + ".chr");

            setRootPartRotationOffset(Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationY((float)new Random().NextDouble() * 10.0f)));
        }

        public override bool shouldDissapearWhenThisBlockIsDestroyed(BlockLoc loc)
        {
            return loc== blockRootedIn ;
        }

    }
}
