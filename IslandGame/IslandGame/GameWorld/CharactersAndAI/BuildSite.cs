using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CubeAnimator;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    class WoodBuildSite : MultiblockJobSite
    {
        protected Dictionary<BlockLoc,byte> blocksToBuild;
        protected string markerName = "stoneMarkerBlock";


        public WoodBuildSite(IslandPathingProfile nProfile)
        {
            blocksToBuild = new Dictionary<BlockLoc,byte>();
            profile = nProfile;
        }

        public override float? intersects(Ray ray)
        {
            return Intersection.intersects(ray, blocksToBuild.Keys);
        }

        public override Job getJob(Character newWorker, Ray ray, IslandWorkingProfile workingProfile)
        {
            return new BuildKickoffJob(this, newWorker, workingProfile);
        }

        public int numBlocksLeftToBuild()
        {
            return blocksToBuild.Count;
        }

        public bool siteIsComplete()
        {
            return numBlocksLeftToBuild() == 0;
        }



        public List<BlockLoc> getAllBlocksToBuild()
        {
            return blocksToBuild.Keys.ToList();
        }


        public override void blockWasDestroyed(BlockLoc toDestroy)
        {
            return;
        }

        public override void blockWasBuilt(BlockLoc toDestroy)
        {
            blocksToBuild.Remove(toDestroy);
        }

        public override void draw(GraphicsDevice device, Effect effect, DisplayParameters parameters)
        {
            AnimatedBodyPartGroup block = new AnimatedBodyPartGroup(ContentDistributor.getRootPath() + @"worldMarkup\" + markerName + ".chr", 1.0f / 7.0f);
            block.setScale(1f / 12f);
            foreach (BlockLoc test in blocksToBuild.Keys)
            {
                //WorldMarkupHandler.addCharacter(ContentDistributor.getRootPath()+@"worldMarkup\"+markerName+".chr",
                //                           test.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f), 1.0f/12.0f,.6f);
                block.setRootPartLocation(test.toWorldSpaceVector3() + new Vector3(.5f, .5f, .5f));
                effect.Parameters["xOpacity"].SetValue(.7f);
                effect.Parameters["xTint"].SetValue(ColorPallete.getColorFromByte(blocksToBuild[test]).ToVector4()*1.2f);
                block.draw(device, effect);
                effect.Parameters["xTint"].SetValue(Color.White.ToVector4());
                effect.Parameters["xOpacity"].SetValue(1f);
            }
        }


        public void addBlock(BlockLoc newBlockToPlace, byte type)
        {
            blocksToBuild.Add(newBlockToPlace, type);
        }

        public bool containsBlockToBuild(BlockLoc currentGoalBlock)
        {
            return blocksToBuild.Keys.Contains(currentGoalBlock);
        }

        public void removeBlock(BlockLoc newBlockToPlace)
        {
            blocksToBuild.Remove(newBlockToPlace);
        }

        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (BlockLoc test in blocksToBuild.Keys)
            {
                result.Add(test);
            }
            return result;
        }

        public byte getTypeAt(BlockLoc location)
        {
            if (blocksToBuild.ContainsKey(location))
            {
                return blocksToBuild[location];
            }
            else
            {
                return 0;
            }
            
        }
    }
}
