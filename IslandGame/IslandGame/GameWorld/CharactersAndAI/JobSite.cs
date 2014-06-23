using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld
{
    [Serializable]
    public abstract class JobSite : Intersectable, Drawable
    {
        protected IslandPathingProfile profile;

        public abstract float? intersects(Microsoft.Xna.Framework.Ray ray);
        public abstract Job getJob(Character newWorker, Ray ray, IslandWorkingProfile workingProfile);
        public abstract void draw(GraphicsDevice device, Effect effect, DisplayParameters parameters);
        public abstract void blockWasDestroyed(BlockLoc toDestroy);
        public abstract void blockWasBuilt(BlockLoc toDestroy);

        public virtual void update()
        {

        }

        

        public virtual bool isComplete()
        {
            return false;
        }

        public IslandPathingProfile getProfile()
        {
            return profile;
        }


        public virtual void makeFarmBlockGrow(BlockLoc blockLoc)
        {
            
        }


        public virtual void chopBlock(BlockLoc blockLoc)
        {
        }

        public virtual HashSet<BlockLoc> getAllBlocksInSite()
        {
            return new HashSet<BlockLoc>();
        }

        public virtual void updateMesh(int mipLevel)
        {
            
        }

        public virtual void runPreDrawCalculations()
        {
            
        }
    }
}
