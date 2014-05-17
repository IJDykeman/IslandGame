using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    struct VerticesAndIndices
    {
        public VertexPostitionColorPaintNormal[] vertices;
        public short[] indices;
        public VerticesAndIndices(VertexPostitionColorPaintNormal[] nVertices, short[] nIndices)
        {
            vertices = nVertices;
            indices = nIndices;
        }
    }
}
