using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace IslandGame.GameWorld
{

    public struct VertexPostitionColorPaintNormal
    {
        public Vector3 Position;
        public Color Color;
        public Color PaintColor;
        public Vector3 Normal;

        public VertexPostitionColorPaintNormal(Vector3 nPosition, Color nColor, Color nPaintColor, Vector3 nNormal)
        {
            Position = nPosition;
            Color = nColor;
            PaintColor = nPaintColor;
            Normal = nNormal;
        }


        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Color, VertexElementUsage.Color, 1),
            new VertexElement(sizeof(float) * 3 + 8, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );
    }

}
