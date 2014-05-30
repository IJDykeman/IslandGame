using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld
{
    class MemoizedModelAndPoses
    {
        List<MatrixAndOpacity> poses;
        PaintedCubeSpaceDisplayComponant model;

        public MemoizedModelAndPoses(PaintedCubeSpaceDisplayComponant newModel)
        {
            model = newModel;
            poses = new List<MatrixAndOpacity>();
        }

        public void addPose(MatrixAndOpacity nPose)
        {
            poses.Add(nPose);
        }

        public void draw(GraphicsDevice device, Effect effect)
        {
            model.sendModelDataToGPU(device);

            foreach (MatrixAndOpacity pose in poses)
            {
                //model.drawForBodyPartWithPresetBuffers(effect, pose.matrix, false);
            }
        }


    }
}
