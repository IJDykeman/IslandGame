using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class SetPieceManager
    {
        List<SetPiece> setPieces;


        public SetPieceManager()
        {

            setPieces = new List<SetPiece>();
        }

        public void display(GraphicsDevice device, Effect effect)
        {
            try//because it might get modified by the meshing thread
            {
                foreach (SetPiece test in setPieces)
                {
                    test.update();
                    test.draw(device, effect);

                    if (test is Tree)
                    {
                        List<BlockLoc> trunkBlocks = ((Tree)test).getTrunkBlocks();
                        foreach (BlockLoc trunkBlock in trunkBlocks)
                        {
                            WorldMarkupHandler.addFlagPathWithPosition(@"C:\Users\Public\CubeStudio\worldMarkup\redCubeOutline.chr",
                                           trunkBlock.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f));
                        }
                    }

                }
            }
            catch (Exception e) 
            {
                Console.WriteLine("setPiece disp error caught");
                Console.WriteLine(e.Message);
            }
        }

        public void blockWasDestroyed(BlockLoc toDestroy)
        {
            for (int i = setPieces.Count-1; i >=0; i--)
            {
                if (setPieces[i].shouldDissapearWhenThisBlockIsDestroyed(toDestroy))
                {
                    setPieces.RemoveAt(i);
                }
            }
        }

        public void placeDecorativePlant(BlockLoc nLoc)
        {
            setPieces.Add(new DecorativePlant(nLoc));
        }
    }
}
