using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    class TreesJobSite : MultiblockJobSite
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
                    toDraw.update();
                    toDraw.draw(device, effect);
                }
            }

        }

        public override void update()
        {
            if (trees.Count > 0)
            {
                for (int i = trees.Count - 1; i >= 0; i--)
                {
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
                float? thisIntersecton = intersects(ray, test.getTrunkBlocks());
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

        public override Job getJob(Character newWorker)
        {
            return new LoggingJob(newWorker, this);
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

        public override ResourceAmount chopBlockAndGetRescources(BlockLoc blockLoc)
        {
            Tree toChop = getTreeWithTrunkBlock(blockLoc);
            if (toChop != null)
            {
                toChop.getChopped();
                if (toChop.needsToBeDeleted())
                {
                    trees.Remove(toChop);
                    return new ResourceAmount(25, ResourceType.Wood);
                }
            }
            return new ResourceAmount(0, ResourceType.Wood);
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
