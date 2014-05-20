using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using IslandGame.GameWorld;
using CubeAnimator;

namespace IslandGame
{
    public static class Compositer
    {

        public static GraphicsDevice device;

        static SpriteBatch spriteBatch;
        public static Matrix viewMatrix;
        public static Matrix projectionMatrix;
        public static Effect effect;
        static Sky sky;

        static List<AnimatedBodyPartGroup> CharactersForThisFrame = new List<AnimatedBodyPartGroup>();

        public static void construct(GraphicsDevice ndevice)
        {

            device = ndevice;

        }

        public static void LoadContent(ContentManager content)
        {
            // Create a new SpriteBatch, which can be used to drawForChunk textures.
            device = Main.graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);
            effect = content.Load<Effect>("effects");
            effect.Parameters["xTexture"].SetValue(ContentDistributor.random);
            sky = new Sky();
            sky.loadContent(content);

        }

        public static void drawFinalImageFirst(Player player, bool isAnimating)
        {
            effect.CurrentTechnique = effect.Techniques["Colored"];
            UpdateViewMatrix(player.getCameraLoc(), player.getCameraRotation());
            //  blurer = new GaussianBlurHandler(main.Content.LoadGame<Effect>("GaussianBlur"), device,
            //graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            //device.Clear((isAnimating ? Color.CornflowerBlue : new Color(160, 160, 170)));
        }

        public static void display(GameWorld.World world, Player player, Character doNotDisplay)
        {
            device.DepthStencilState = new DepthStencilState()
            {
                DepthBufferEnable = true



            };
            RasterizerState rs = new RasterizerState();

            rs.CullMode = CullMode.None;
            device.RasterizerState = rs;

            effect.Parameters["xWorld"].SetValue(Matrix.Identity);

            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);

            effect.Parameters["xEnableLighting"].SetValue(true);

            Vector3 lightDirection = new Vector3(-.3f, .5f, -1f);

            lightDirection.Normalize();
            lightDirection *= (float).3f;
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(.8f);
            effect.Parameters["xOpacity"].SetValue(1f);
            effect.Parameters["xCamPos"].SetValue(player.getCameraLoc());

            // Matrix sunRotation = Matrix.CreateRotationX(MathHelper.ToRadians(updateCount)) * Matrix.CreateRotationZ(MathHelper.ToRadians(updateCount));


            
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.Solid;
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            world.displayIslands(device, effect, new BoundingFrustum(viewMatrix * projectionMatrix));
            world.displayActors(device, effect, doNotDisplay);

            player.display3D();

            displayOcean(player.getCameraLoc());

            //effect.Parameters["xOpacity"].SetValue(.4f);
            foreach (AnimatedBodyPartGroup group in CharactersForThisFrame)
            {
                group.draw(device, effect);
            }
           // effect.Parameters["xOpacity"].SetValue(1f);

            CharactersForThisFrame.Clear();
            WorldMarkupHandler.draw(device,effect);

            

            
            Random rand = new Random();



            sky.draw(device, effect, viewMatrix, projectionMatrix, player.getCameraLoc());

            effect.Parameters["xWorld"].SetValue(Matrix.Identity);


        }

        private static void SetUpCamera()
        {
            //temp += 0.11f;
            //viewMatrix = Matrix.CreateLookAt(player.locInPath, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            float viewDistance = (float)2000;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 2.3f, device.Viewport.AspectRatio, 0.16f, viewDistance); //used to be 1 and 300 for the last two arguments
        }

        public static void UpdateViewMatrix(Vector3 loc, Quaternion cameraRotationQuaternion)
        {


            SetUpCamera();
            /*if (updownRot > MathHelper.ToRadians(89))
            {
                updownRot = MathHelper.ToRadians(89);
            }
            else if (updownRot < MathHelper.ToRadians(-87))
            {
                updownRot = MathHelper.ToRadians(-87);
            }*/


            Matrix cameraRotation = Matrix.CreateFromQuaternion(cameraRotationQuaternion);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            //if (inBody)
            // {
           // Vector3 neckAdjustment = new Vector3((float)Math.Cos(-leftrightRot + MathHelper.ToRadians(90)), 0, (float)Math.Sin(-leftrightRot + MathHelper.ToRadians(90)));
            //neckAdjustment.Normalize();
            //neckAdjustment = neckAdjustment * -.1f;
            Vector3 cameraFinalTarget = loc + cameraRotatedTarget;
            //}

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);



            viewMatrix = Matrix.CreateLookAt(loc, cameraFinalTarget, cameraRotatedUpVector);
            //GALLERY
            if (Player.galleryMode)
            {
                viewMatrix = Matrix.CreateLookAt(loc, new Vector3(60, 10, 60), Vector3.Up);
            }

        }

        public static void drawLine(Vector3 loc1, Vector3 loc2)
        {

            effect.CurrentTechnique = effect.Techniques["ColoredNoShading"];
            List<Vector3> locations = new List<Vector3>(2);
            locations.Add(loc1);
            locations.Add(loc2);
            Color color = Color.Blue;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                pass.Apply();
                var data = new List<VertexPositionColor>(locations.Count * 2);
                for (int i = 1; i < locations.Count; i++)
                {





                    data.Add(new VertexPositionColor(locations[i], color));
                    data.Add(new VertexPositionColor(locations[i - 1], color));
                }
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, data.ToArray(), 0, data.Count / 2);
            }
        }

        public static void addFlagForThisFrame(Vector3 loc, string Color)
        {
            AnimatedBodyPartGroup flag = new AnimatedBodyPartGroup(@"C:\Users\Public\CubeStudio\worldMarkup\short" + Color + "Flag.chr", 1f / 12f);
            flag.setRootPartLocation((Vector3)loc);
            addAnimatedBodyPartGroupForThisFrame(flag);
        }

        public static void addAnimatedBodyPartGroupForThisFrame(AnimatedBodyPartGroup toAdd)
        {
            CharactersForThisFrame.Add(toAdd);
        }

        public static void drawPart(BodyPart bodypart)
        {

            device.DepthStencilState = new DepthStencilState()
            {
                DepthBufferEnable = true
            };
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);

            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);

            effect.Parameters["xEnableLighting"].SetValue(true);

            Vector3 lightDirection = new Vector3(-.3f, .5f, -1f);

            lightDirection.Normalize();
            lightDirection *= (float).3f;
            effect.Parameters["xLightDirection"].SetValue(lightDirection);


            // Matrix sunRotation = Matrix.CreateRotationX(MathHelper.ToRadians(updateCount)) * Matrix.CreateRotationZ(MathHelper.ToRadians(updateCount));


            effect.Parameters["xAmbient"].SetValue(.6f);
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.Solid;
            device.RasterizerState = rasterizerState;
            bodypart.draw(device, effect, Matrix.Identity, Quaternion.Identity);


            effect.Parameters["xWorld"].SetValue(Matrix.Identity);



        }

        internal static int getScreenWidth()
        {
            return device.Viewport.Width;
        }

        internal static int getScreenHeight()
        {
            return device.Viewport.Height;
        }

        static void displayOcean(Vector3 center)
        {
            float oceanWidth = 100;
            AnimatedBodyPartGroup ocean = new AnimatedBodyPartGroup(@"C:\Users\Public\CubeStudio\world_decoration\blueCubeAtYNeg1.chr", oceanWidth);
            ocean.setRootPartLocation(new Vector3(center.X, -oceanWidth/2.0f+.6f, center.Z));
            addAnimatedBodyPartGroupForThisFrame(ocean);

        }
    }
}
