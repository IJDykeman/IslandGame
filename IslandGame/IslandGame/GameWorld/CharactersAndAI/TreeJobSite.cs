using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class TreesJobSite : MultiblockJobSite
    {
        List<Tree> trees;

        public TreesJobSite(IslandPathingProfile nProfile)
        {
            trees = new List<Tree>();
            profile = nProfile;
        }

        public override void draw(GraphicsDevice device, Effect effect)
        {

            lock (trees)
            {
                foreach (Tree toDraw in trees)
                {
                    
                    //toDraw.addToWorldMarkup();
                    toDraw.draw(device, effect);
                }
            }
        }

        public override void runPreDrawCalculations()
        {
          /*  lock (trees)
            {
                foreach (Tree toDraw in trees)
                {
                    toDraw.update();
                    toDraw.addToWorldMarkup();
                    //toDraw.draw(device, effect);
                }
            }*/
        }

        public override void update()
        {
            if (trees.Count > 0)
            {
                for (int i = trees.Count - 1; i >= 0; i--)
                {
                    trees[i].update();
                    if (trees[i].needsToBeDeleted())
                    {
                        trees.RemoveAt(i);
                    }
                }
            }
        }

        public override void blockWasBuilt(BlockLoc toDestroy)
        {
            
        }

        public override void blockWasDestroyed(BlockLoc toDestroy)
        {

        }

        public override float? intersects(Microsoft.Xna.Framework.Ray ray)
        {
            float? intersection = null;
            Tree closest = null; //currently found but not used for anything
            foreach(Tree test in trees){
                float? thisIntersecton = Intersection.intersects(ray, test.getTrunkBlocks());
                if (thisIntersecton.HasValue)
                {
                    if (intersection.HasValue == false || (float)thisIntersecton < (float)intersection)
                    {
                        intersection = thisIntersecton;
                        closest = test;
                    }
                }
            }
            return intersection;
        }

        public override Job getJob(Character newWorker, Ray ray, IslandWorkingProfile workingProfile)
        {
            return new LoggingJob(newWorker, this, workingProfile);
        }

        public override void updateMesh(int mipLevel)
        {
            foreach (Tree toUpdate in trees)
            {
                toUpdate.setMip(mipLevel);
            }
        }

        public void placeTree(BlockLoc loc, Tree.treeTypes type)
        {
            lock (trees)
            {
                trees.Add(new Tree(loc, type));
            }
        }

        public List<Tree> getTrees()
        {
            return trees;
        }

        internal List<BlockLoc> getTreeTrunkBlocks()
        {
            List<BlockLoc> result = new List<BlockLoc>();
            foreach (Tree tree in trees)
            {
                result.AddRange(tree.getTrunkBlocks());
            }
            return result;
        }

        private Tree getTreeWithTrunkBlock(BlockLoc trunkBlock)
        {
            foreach (Tree tree in trees)
            {
                if (tree.getTrunkBlocks().Contains(trunkBlock))
                {
                    return tree;
                }
            }
            return null;
        }

        public override void chopBlock(BlockLoc blockLoc)
        {
            Tree toChop = getTreeWithTrunkBlock(blockLoc);
            if (toChop != null)
            {
                toChop.getChopped();
                if (toChop.needsToBeDeleted())
                {
                    trees.Remove(toChop);

                }
            }

        }

        public bool hasAtLeastOneTree()
        {
            return trees.Count != 0;
        }


        public override HashSet<BlockLoc> getAllBlocksInSite()
        {
            HashSet<BlockLoc> result = new HashSet<BlockLoc>();
            foreach (Tree tree in trees)
            {

                foreach (BlockLoc test in tree.getTrunkBlocks())
                {
                    result.Add(test);
                }
            }
            return result;
        }
    }
}
