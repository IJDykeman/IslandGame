using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CubeAnimator
{
    class LoadedCubeSpaceData
    {
        public byte[, ,] array;
        public int spaceWidth;
        public int spaceHeight;
        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;
    }
}
