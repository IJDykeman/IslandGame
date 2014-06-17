using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld.CharactersAndAI
{
    [Serializable]
    public class BoatBuildSite : ObjectBuildJobSite
    {

        public BoatBuildSite(BlockLoc newObjectLoc, IslandPathingProfile nProfile)
        {
            objectLoc = newObjectLoc;
            profile = nProfile;
        }

        public override void blockWasBuilt(BlockLoc loc) { }

        public override void blockWasDestroyed(BlockLoc loc) { }

        public override Job getJob(Character newWorker, Ray ray, IslandWorkingProfile workingProfile)
        {
            return new ObjectBuildingJob(newWorker, this);
        }

        public override void draw(GraphicsDevice device, Effect effect)
        {
            WorldMarkupHandler.addCharacter(ContentDistributor.getRootPath()+@"boats\greenOnePersonBoat.chr",
            objectLoc.toWorldSpaceVector3(),
            IslandGame.GameWorld.Boat.SCALE, .5f);
        }
    }
}
