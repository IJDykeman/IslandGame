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

        static Effect effect;
        static Effect shadowEffect;
        static Sky sky;
        static Ocean ocean;
        static float ambientBrightness = .9f;

        static RenderTarget2D renderTarget;
        //static Texture2D r;
        static Texture2D mainRenderImage;

        static RenderTarget2D shadowRendertarget;
        static Texture2D shadowMap;

        static List<AnimatedBodyPartGroup> CharactersForThisFrame = new List<AnimatedBodyPartGroup>();

        static float shadowBufferScale = 3.2f;



        public static void LoadContent(ContentManager content)
        {
            // Create a new SpriteBatch, which can be used to drawForChunk textures.
            device = Main.graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);
            effect = content.Load<Effect>("effects");
            shadowEffect = content.Load<Effect>("shadowEffect");
            
            sky = new Sky();
            sky.loadContent(content);

            ocean = new Ocean();
            ocean.loadContent(content);

            renderTarget = new RenderTarget2D(device, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight,
                false, device.DisplayMode.Format, DepthFormat.Depth24Stencil8, 4, RenderTargetUsage.DiscardContents);

            shadowRendertarget = new RenderTarget2D(device, getShadowBufferWidth(), getShadowBufferHeight(),
                 false,
                                                    SurfaceFormat.Single,
                                                    DepthFormat.Depth24);



        }

        private static int getShadowBufferHeight()
        {
            return (int)(device.PresentationParameters.BackBufferHeight * shadowBufferScale);
        }

        private static int getShadowBufferWidth()
        {
            return (int)(device.PresentationParameters.BackBufferWidth * shadowBufferScale);
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

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            device.RasterizerState = rs;
            device.BlendState = BlendState.Opaque;

            drawShadows(player, world);

            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(getPerspectiveMatrix(1000));

            effect.Parameters["xShadowWorld"].SetValue(getShadowWorldMatrix());
            effect.Parameters["xShadowView"].SetValue(getShadowViewMatrix(player));
            effect.Parameters["xShadowProjection"].SetValue(getShadowProjectionMatrix());
            effect.Parameters["xTexture"].SetValue(shadowMap);
            effect.Parameters["xLightPos"].SetValue(getLightLoc(player));
            Vector2 shadowMapPixelSize = new Vector2(0.5f / shadowRendertarget.Width, 0.5f / shadowRendertarget.Height);
            effect.Parameters["ShadowMapPixelSize"].SetValue(shadowMapPixelSize);
            effect.Parameters["ShadowMapSize"].SetValue(new Vector2(shadowRendertarget.Width, shadowRendertarget.Height));
            effect.Parameters["xEnableLighting"].SetValue(true);
            Vector3 lightDirection = new Vector3(-.3f, .5f, -1f);
            lightDirection.Normalize();
            lightDirection *= (float).3f;
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(ambientBrightness);
            effect.Parameters["xOpacity"].SetValue(1f);
            effect.Parameters["xCamPos"].SetValue(player.getCameraLoc());

            // Matrix sunRotation = Matrix.CreateRotationX(MathHelper.ToRadians(updateCount)) * Matrix.CreateRotationZ(MathHelper.ToRadians(updateCount));


            
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.Solid;
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            device.SetRenderTarget(renderTarget);
            device.DepthStencilState = new DepthStencilState()
            {
                DepthBufferEnable = true
            };


            display3DObjects(world, player, doNotDisplay, effect);

            effect.Parameters["xWorld"].SetValue(Matrix.Identity);

            //device.Clear(Color.Black);

            mainRenderImage = (Texture2D)renderTarget;

            device.SetRenderTarget(null);


           // renderTarget.Dispose();
           // renderTarget = null;
            
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
            SamplerState.PointClamp, DepthStencilState.Default,
            RasterizerState.CullNone);
            spriteBatch.Draw(mainRenderImage, new Vector2(0, 0), Color.White);
            //device.SamplerStates[1] = SamplerState.PointClamp;
            //spriteBatch.Draw(shadowMap, new Rectangle(0, 0, 400, 240), Color.Green);
            spriteBatch.End();

        }

        private static void display3DObjects(GameWorld.World world, Player player, Character doNotDisplay, Effect effectToUse)
        {

            effect.Parameters["xProjection"].SetValue(getPerspectiveMatrix(2000));
            sky.draw(device, effectToUse, viewMatrix, getPerspectiveMatrix(2000), player.getCameraLoc());
            effect.Parameters["xProjection"].SetValue(getPerspectiveMatrix(1000));


            world.displayIslands(device, effectToUse, new BoundingFrustum(viewMatrix * getPerspectiveMatrix(1000)));
            world.displayActors(device, effectToUse, doNotDisplay);
            player.display3D();
            foreach (AnimatedBodyPartGroup group in CharactersForThisFrame)
            {
                group.draw(device, effectToUse);
            }
            CharactersForThisFrame.Clear();
            WorldMarkupHandler.draw(device, effectToUse);
            ocean.draw(device, viewMatrix, getPerspectiveMatrix(1000), player.getCameraLoc(), ambientBrightness);

        }

        private static void displayShadowCasters(GameWorld.World world, Player player, Character doNotDisplay, Effect effectToUse)
        {
            world.displayIslands(device, effectToUse, new BoundingFrustum(viewMatrix * getPerspectiveMatrix(1000)));
            world.displayActors(device, effectToUse, doNotDisplay);
        }

        public static void drawShadows(Player player, World world)
        {


            device.SetRenderTarget(shadowRendertarget);
            device.DepthStencilState = new DepthStencilState()
            {
                DepthBufferEnable = true
            };

            shadowEffect.Parameters["xWorld"].SetValue(getShadowWorldMatrix());
            shadowEffect.Parameters["xView"].SetValue(getShadowViewMatrix(player));
            shadowEffect.Parameters["xProjection"].SetValue(getShadowProjectionMatrix());
            shadowEffect.Parameters["xEnableLighting"].SetValue(true);
            Vector3 lightDirection = new Vector3(-.3f, .5f, -1f);
            lightDirection.Normalize();
            lightDirection *= (float).3f;
            shadowEffect.Parameters["xLightDirection"].SetValue(lightDirection);
            shadowEffect.Parameters["xAmbient"].SetValue(ambientBrightness);
            shadowEffect.Parameters["xOpacity"].SetValue(1f);
            shadowEffect.Parameters["xCamPos"].SetValue(getLightLoc(player));

            

            displayShadowCasters(world, player, null, shadowEffect);

            shadowMap = (Texture2D)shadowRendertarget;
            device.SetRenderTarget(null);

        }

        private static Matrix getShadowProjectionMatrix()
        {

            BoundingSphere sphere = getViewFrustumBoundingSphereForShadows();

            const float NearClip = 1f;


            float farClip = 81;
            Matrix shadowProjMatrix = Matrix.CreateOrthographic(sphere.Radius * 2, sphere.Radius*2, NearClip, farClip);

            return shadowProjMatrix;

        }
        
        private static float getShadowViewWidthInWorldSpace()
        {
            return getViewFrustumBoundingSphereForShadows().Radius * 2.0f;
        }

        private static Matrix getShadowViewMatrix(Player player)
        {
            Vector3 shadowCamPos = getLightLoc(player);
            Matrix shadowViewMatrix = Matrix.CreateLookAt(shadowCamPos, shadowCamPos+Vector3.Down, Vector3.Left);

            return shadowViewMatrix;
        }

        private static Vector3 getLightLoc(Player player)
        {
            BoundingSphere sphere = getViewFrustumBoundingSphereForShadows();

            Vector3 shadowCamPos = sphere.Center;
            shadowCamPos.Y = 80;

            shadowCamPos.X /= getShadowViewWidthInWorldSpace() / getShadowBufferHeight();
            shadowCamPos.Z /= getShadowViewWidthInWorldSpace() / getShadowBufferWidth();
            shadowCamPos.X = (int)shadowCamPos.X;
            shadowCamPos.Z = (int)shadowCamPos.Z;
            shadowCamPos.X *= getShadowViewWidthInWorldSpace() / getShadowBufferHeight();
            shadowCamPos.Z *= getShadowViewWidthInWorldSpace() / getShadowBufferWidth();

            return shadowCamPos;
        }
        


        private static BoundingSphere getViewFrustumBoundingSphereForShadows()
        {
            BoundingSphere sphere = BoundingSphere.CreateFromFrustum(new BoundingFrustum(viewMatrix * getPerspectiveMatrix(10)));
            return sphere;
        }

        private static Matrix getShadowWorldMatrix()
        {
            return Matrix.Identity;
        }

        public static Matrix getPerspectiveMatrix(int viewDistance)
        {


            return Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 2.3f, device.Viewport.AspectRatio, 0.16f, viewDistance); //used to be 1 and 300 for the last two arguments
        }

        public static void UpdateViewMatrix(Vector3 loc, Quaternion cameraRotationQuaternion)
        {




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
            effect.Parameters["xProjection"].SetValue(getPerspectiveMatrix(2000));

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

        public static void setSkyColors(Vector4 horizonColor, Vector4 zenithColor, float nambientBrightness)
        {
            sky.setColors(horizonColor, zenithColor);
            ambientBrightness = nambientBrightness;
        }


    }
}
