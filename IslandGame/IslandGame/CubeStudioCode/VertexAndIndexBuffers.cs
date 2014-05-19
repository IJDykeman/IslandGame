using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame
{
    class VertexAndIndexBuffers
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        public VertexAndIndexBuffers(VertexBuffer nVertexBuffer, IndexBuffer nIndexBuffer)
        {
            vertexBuffer = nVertexBuffer;
            indexBuffer = nIndexBuffer;
        }

        public VertexBuffer getVertexBuffer()
        {
            return vertexBuffer;
        }

        public IndexBuffer getIndexBuffer()
        {
            return indexBuffer;
        }
    }
}
