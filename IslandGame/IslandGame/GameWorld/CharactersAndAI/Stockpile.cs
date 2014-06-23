using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class Stockpile : Intersectable
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



        public void draw(GraphicsDevice device, Effect effect, DisplayParameters parameters)
        {
            string path = @"worldMarkup\farmMarker.chr";
            switch (typeToStore)
            {
                case ResourceBlock.ResourceType.Wheat:
                    path = @"resources\wheatBale.chr";
                    break;
                case ResourceBlock.ResourceType.Wood:
                    path = @"resources\log.chr";
                    break;
                case ResourceBlock.ResourceType.Stone:
                    path = @"resources\standardBlock.chr";
                    break;

            }

            foreach (BlockLoc test in storageSpace)
            {
                float opacity = .2f;
                if (!parameters.hasParameter(DisplayParameter.drawStockpiles))
                {
                    opacity = .1f;
                }

                WorldMarkupHandler.addCharacter(ContentDistributor.getRootPath() + path,
                    test.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f), 1f / 7f, opacity);
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
