using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld
{
    [Serializable]
    class Stockpile : Intersectable
    {
        HashSet<BlockLoc> storageSpace;
        ResourceBlock.ResourceType typeToStore;

        public Stockpile(IEnumerable<BlockLoc> blockLocsForStorage, ResourceBlock.ResourceType ntypeToStore)
        {
            storageSpace = new HashSet<BlockLoc>();
            foreach (BlockLoc toAdd in blockLocsForStorage)
            {
                storageSpace.Add(toAdd);
            }
            typeToStore = ntypeToStore;
        }

        public ResourceBlock.ResourceType typeStored()
        {
            return typeToStore;
        }

        public float? intersects(Ray ray)
        {
            return Intersection.intersects(ray, storageSpace);
        }






        public void blockWasBuilt(BlockLoc toDestroy)
        {
            throw new NotImplementedException();
        }


        public void blockWasDestroyed(BlockLoc toDestroy)
        {
            throw new NotImplementedException();
        }



        public void draw(GraphicsDevice device, Effect effect)
        {
            foreach (BlockLoc test in storageSpace)
            {
                WorldMarkupHandler.addFlagPathWithPosition(ContentDistributor.getRootPath() + @"worldMarkup\farmMarker.chr",
                    test.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f));
            }

        }



        public HashSet<BlockLoc> getAllBlocksInStockPile()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc test in storageSpace)
            {
                result.Add(test);
            }
            return result;
        }

        public ResourceBlock.ResourceType getStoredType()
        {
            return typeToStore;
        }

    }
}
