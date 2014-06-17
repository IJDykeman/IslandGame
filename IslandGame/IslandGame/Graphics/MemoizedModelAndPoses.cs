using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using IslandGame.GameWorld;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    class MemoizedModelAndPoses
    {
        HashSet<MatrixAndOpacity> poses;
        PaintedCubeSpaceDisplayComponant model;

        VertexBuffer geometry;
        IndexBuffer indexBuffer;
        VertexBuffer instanceBuffer;
        InstanceInfo[] instances;
        VertexBufferBinding[] bindings;
        bool needsBufferReset = true;

        public MemoizedModelAndPoses(PaintedCubeSpaceDisplayComponant newModel)
        {
            model = newModel;
            poses = new HashSet<MatrixAndOpacity>();
            geometry = model.getVertexBuffer();
            indexBuffer = model.getIndexBuffer();
            instances = new InstanceInfo[0];
        }

        public void addPose(MatrixAndOpacity nPose)
        {
            int why = nPose.GetHashCode();
            if (!poses.Contains(nPose))
            {
                poses.Add(nPose);
                needsBufferReset = true;
            }
            else
            {

            }
        }

        public void drawOLD(GraphicsDevice device, Effect effect)
        {
            model.sendModelDataToGPU(device);
            

            foreach (MatrixAndOpacity pose in poses)
            {
                model.drawForBodyPartWithPresetBuffers(effect, pose.matrix, false);
            }
        }

        private VertexDeclaration GenerateInstanceVertexDeclaration()
        {
            VertexElement[] instanceStreamElements = new VertexElement[4];
            instanceStreamElements[0] = new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1);
            instanceStreamElements[1] = new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2);
            instanceStreamElements[2] = new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3);
            instanceStreamElements[3] = new VertexElement(sizeof(float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4);
            return new VertexDeclaration(instanceStreamElements);
        }

        public void draw(GraphicsDevice device, Effect effect)
        {

            if (poses.Count == 0)
            {
                return;
            }

            effect.Parameters["xOpacity"].SetValue(poses.ElementAt(0).opacity);
            effect.CurrentTechnique.Passes[0].Apply();


            device.SetVertexBuffers(bindings);
            device.Indices = indexBuffer;
            
            device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                    geometry.VertexCount, 0,
                    indexBuffer.IndexCount / 3, poses.Count);

            
        }

        public void setUpBuffers(GraphicsDevice device)
        {

            if (!needsBufferReset)
            {
                return;
            }
                needsBufferReset = false;





            bindings = new VertexBufferBinding[2];
            bindings[0] = new VertexBufferBinding(geometry);
            resetBuffers(device);
        }


        public void resetBuffers(GraphicsDevice device)
        {
            InstanceInfo[] instances = new InstanceInfo[poses.Count];

            int i = 0;
            foreach (MatrixAndOpacity test in poses)
            {
                instances[i].World = test.matrix;
                i++;
            }

            instanceBuffer = new VertexBuffer(device, GenerateInstanceVertexDeclaration(), poses.Count, BufferUsage.WriteOnly);
            instanceBuffer.SetData(instances);
            bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);

        }


        internal void reset()
        {
            poses.Clear();
        }
    }



    struct InstanceInfo
    {
        public Matrix World;

    };
}
