using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using IslandGame.Graphics;

namespace IslandGame
{
    class Sky
    {
        Effect skyEffect;
        Model sphere;
        CloudManager cloudManager;

        public void loadContent(ContentManager content)
        {
            cloudManager = new CloudManager();
            skyEffect = content.Load<Effect>("sky");
            sphere = content.Load<Model>("sphere");
            //sphere.Meshes[0].Effects = new ModelEffectCollection();
        }

        public void draw(GraphicsDevice device, Effect effect, Matrix view, Matrix projection, Vector3 centerLocation)
        {
            cloudManager.display(device, effect);

            foreach (ModelMesh mesh in sphere.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = skyEffect;
                    skyEffect.Parameters["World"].SetValue(Matrix.CreateScale(2000, 600, 2000) * Matrix.CreateTranslation(new Vector3(centerLocation.X,0,centerLocation.Z)) * mesh.ParentBone.Transform);
                    skyEffect.Parameters["View"].SetValue(view);
                    skyEffect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
            //sphere.Draw(Matrix.CreateTranslation(0,0,0)* Matrix.CreateScale(30),view, projection);
        }
    }
}
