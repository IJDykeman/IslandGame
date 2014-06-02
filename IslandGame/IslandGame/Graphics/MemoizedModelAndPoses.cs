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
            //effect.CurrentTechnique = effect.Techniques["Instanced"];
            //model.sendModelDataToGPU(device);
            VertexBuffer geometry = model.getVertexBuffer();
            IndexBuffer indexBuffer = model.getIndexBuffer();
            VertexBuffer instanceBuffer;

            InstanceInfo[] instances = new InstanceInfo[poses.Count];



            for (int i = 0; i < poses.Count; i++)
            {
                instances[i].World = poses[i].matrix;
                Vector3 pos;
                Vector3 scale;
                Quaternion rot;
                poses[i].matrix.Decompose(out scale, out rot, out pos);
            }
            instanceBuffer = new VertexBuffer(device, GenerateInstanceVertexDeclaration(), poses.Count, BufferUsage.WriteOnly);
            instanceBuffer.SetData(instances);

            effect.CurrentTechnique.Passes[0].Apply();

            VertexBufferBinding[] bindings;

            bindings = new VertexBufferBinding[2];
            bindings[0] = new VertexBufferBinding(geometry);
            bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);


            device.SetVertexBuffers(bindings);
            device.Indices = indexBuffer;

            device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                    geometry.VertexCount, 0,
                    indexBuffer.IndexCount / 3, poses.Count);

            effect.CurrentTechnique = effect.Techniques["Colored"];

        }

    }

    struct InstanceInfo
    {
        public Matrix World;

    };
}
