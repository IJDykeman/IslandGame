using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld
{
    [Serializable]
    public abstract class JobSite
    {
        protected IslandPathingProfile profile;

        public abstract float? intersects(Microsoft.Xna.Framework.Ray ray);
        public abstract Job getJob(Character newWorker);
        public abstract void draw(GraphicsDevice device, Effect effect);
        public abstract void blockWasDestroyed(BlockLoc toDestroy);
        public abstract void blockWasBuilt(BlockLoc toDestroy);

        public virtual bool isComplete()
        {
            return false;
        }

        public IslandPathingProfile getProfile()
        {
            return profile;
        }


        public virtual ResourceAmount makeFarmBlockGrowAndGetRescources(BlockLoc blockLoc)
        {
            return new ResourceAmount(0, ResourceType.Wheat);
        }


        public virtual void chopBlock(BlockLoc blockLoc)
        {
            return;
        }

        public virtual HashSet<BlockLoc> getAllBlocksInSite()
        {
            return new HashSet<BlockLoc>();
        }
    }
}
