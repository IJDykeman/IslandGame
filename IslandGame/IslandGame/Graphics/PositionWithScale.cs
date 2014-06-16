using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    struct MatrixAndOpacity
    {
        public Matrix matrix;
        public float opacity;

        public MatrixAndOpacity(Matrix nMatrix, float nOpacity)
        {
            matrix = nMatrix;
            opacity = nOpacity;
        }

        public MatrixAndOpacity(Vector3 nLoc, float nScale)
        {
            matrix = Matrix.CreateTranslation(nLoc) * Matrix.CreateScale(nScale);
            opacity = 1;
        }

        public MatrixAndOpacity(Vector3 nLoc, float nScale, float nOpacity)
        {
            matrix = Matrix.CreateTranslation(nLoc) * Matrix.CreateScale(nScale);
            opacity = nOpacity;
        }

        public override int GetHashCode()
        {
            return matrix.GetHashCode() + opacity.GetHashCode();
        }
    }
}
