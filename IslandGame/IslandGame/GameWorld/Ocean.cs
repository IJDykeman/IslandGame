using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using CubeAnimator;

namespace IslandGame.GameWorld
{
    class Ocean
    {
        Effect skyEffect;
        Model sphere;


        public void loadContent(ContentManager content)
        {

            skyEffect = content.Load<Effect>("ocean");
            sphere = content.Load<Model>("plane");
            //plane.Meshes[0].Effects = new ModelEffectCollection();
        }

        public void draw(GraphicsDevice device,  Matrix view, Matrix projection, Vector3 centerLocation, float ambientBrightness)
        {


            foreach (ModelMesh mesh in sphere.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = skyEffect;
                    skyEffect.Parameters["xWorld"].SetValue(Matrix.CreateScale(3000, 1, 3000) * Matrix.CreateTranslation(new Vector3(centerLocation.X, .5f, centerLocation.Z)) * mesh.ParentBone.Transform);
                    skyEffect.Parameters["View"].SetValue(view);
                    skyEffect.Parameters["xAmbient"].SetValue(ambientBrightness);
                    skyEffect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
            //plane.Draw(Matrix.CreateTranslation(0,0,0)* Matrix.CreateScale(30),view, projection);
        }

    }
}
