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
        static Sky sky;
        static Ocean ocean;
        static float ambientBrightness = .9f;

        static RenderTarget2D renderTarget;
        static Texture2D mainRenderImage;

        static RenderTarget2D albedoRT, normalRT, depthRT;


        static List<AnimatedBodyPartGroup> CharactersForThisFrame = new List<AnimatedBodyPartGroup>();


        public static void LoadContent(ContentManager content)
        {
            // Create a new SpriteBatch, which can be used to drawForChunk textures.
            device = Main.graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);
            effect = content.Load<Effect>("effects");
            
            sky = new Sky();
            sky.loadContent(content);

            ocean = new Ocean();
            ocean.loadContent(content);
            int screenWidth = device.PresentationParameters.BackBufferWidth;
            int screenHeight = device.PresentationParameters.BackBufferHeight;
            renderTarget = new RenderTarget2D(device, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight,
                false, device.DisplayMode.Format, DepthFormat.Depth24Stencil8, 4, RenderTargetUsage.DiscardContents);
        }

        public static void drawFinalImageFirst(Player player, bool isAnimating)
        {
            effect.CurrentTechnique = effect.Techniques["Colored"];
            UpdateViewMatrix(player.getCameraLoc(), player.getCameraRotation());
        }

        public static void display(GameWorld.World world, Player player, Character doNotDisplay)
        {
            DisplayParameters displayParameters = new DisplayParameters();
            if (player.wantsStockpilesDisplayed())
            {
                displayParameters.addParameter(DisplayParameter.drawStockpiles);
            }
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            device.RasterizerState = rs;
            device.BlendState = BlendState.Opaque;

            setStatesForMainDraw(player);

            world.runPreDrawCalculations();


            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.Solid;
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            device.SetRenderTarget(renderTarget);


            device.BlendState = BlendState.NonPremultiplied;
            device.DepthStencilState = new DepthStencilState()
            {
                DepthBufferEnable = true
            };

            display3DObjects(world, player, doNotDisplay, effect, displayParameters);
            
            

            effect.Parameters["xWorld"].SetValue(Matrix.Identity);



            mainRenderImage = (Texture2D)renderTarget;

            device.SetRenderTarget(null);

            
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
            SamplerState.PointClamp, DepthStencilState.Default,
            RasterizerState.CullNone);
            spriteBatch.Draw(mainRenderImage, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            WorldMarkupHandler.resetWorldMarkup();

        }

        private static void setStatesForMainDraw(Player player)
        {
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(getPerspectiveMatrix(1000));


            effect.Parameters["xEnableLighting"].SetValue(true);
            Vector3 lightDirection = new Vector3(-.3f, .5f, -1f);
            lightDirection.Normalize();
            lightDirection *= (float).3f;
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(ambientBrightness);
            effect.Parameters["xOpacity"].SetValue(1f);
            effect.Parameters["xCamPos"].SetValue(player.getCameraLoc());
            effect.Parameters["xTint"].SetValue(Color.White.ToVector4());
        }

        private static void display3DObjects(GameWorld.World world, Player player, Character doNotDisplay, Effect effectToUse, DisplayParameters displayParameters)
        {



            effect.Parameters["xOpacity"].SetValue(1f);
            
            world.displayActors(device, effectToUse, doNotDisplay);
            world.displayIslands(device, effectToUse, new BoundingFrustum(viewMatrix * getPerspectiveMatrix(1000)), displayParameters);
            player.display3D();
            foreach (AnimatedBodyPartGroup group in CharactersForThisFrame)
            {
                group.draw(device, effectToUse);
            }
            CharactersForThisFrame.Clear();


            effect.Parameters["xProjection"].SetValue(getPerspectiveMatrix(2000));
            sky.draw(device, effectToUse, viewMatrix, getPerspectiveMatrix(2000), player.getCameraLoc());
            effect.Parameters["xProjection"].SetValue(getPerspectiveMatrix(1000));
            ocean.draw(device, viewMatrix, getPerspectiveMatrix(1000), player.getCameraLoc(), ambientBrightness);

            effect.CurrentTechnique = effect.Techniques["Instanced"];
            WorldMarkupHandler.drawCharacters(device, effect);
            effect.CurrentTechnique = effect.Techniques["Colored"];

        }

        public static Matrix getPerspectiveMatrix(int viewDistance)
        {


            return Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 2.3f, device.Viewport.AspectRatio, 0.07f, viewDistance); //used to be 1 and 300 for the last two arguments
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
            if (Player.galleryMode)
            {
                viewMatrix = Matrix.CreateLookAt(loc, new Vector3(60, 10, 60), Vector3.Up);
            }

        }

        public static void addFlagForThisFrame(Vector3 loc, string Color)
        {
            AnimatedBodyPartGroup flag = new AnimatedBodyPartGroup(ContentDistributor.getEmptyString()+@"worldMarkup\short" + Color + "Flag.chr", 1f / 12f);
            flag.setRootPartLocation((Vector3)loc);
            addAnimatedBodyPartGroupForThisFrame(flag);
        }

        public static void addAnimatedBodyPartGroupForThisFrame(AnimatedBodyPartGroup toAdd)
        {
            CharactersForThisFrame.Add(toAdd);
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
