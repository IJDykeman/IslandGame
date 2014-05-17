using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CubeAnimator;


namespace IslandGame.GameWorld
{
    [Serializable]
    public class SetPiece : CubeAnimator.AnimatedBodyPartGroup
    {

        AxisAlignedBoundingBox AABB;
        

        public SetPiece()
        {
        }

        public SetPiece(AxisAlignedBoundingBox nAABB, string path, float scale)
        {
            setupSetPiece(nAABB, path, scale);
        }

        protected void setupSetPiece(AxisAlignedBoundingBox nAABB, string path, float scale)
        {
            setupSetPiece( nAABB,  path);
            setScale(scale);

        }

        protected void setupSetPiece(AxisAlignedBoundingBox nAABB, string path)
        {
            positionQueue = new List<PositionForTime>();
            loadFromFile(path);
            main.setAnimations();
            AABB = nAABB;
            setRootPartLocation(AABB.loc + new Vector3(AABB.Xwidth / 2f, 0, AABB.Zwidth / 2f));
        }


        public virtual void update()
        {

            //main.orderAnimation(new List<AnimationType>(), new noAnimation());
        }

        public virtual bool shouldDissapearWhenThisBlockIsDestroyed(BlockLoc loc)
        {
            return false;
        }

        



    }
}
