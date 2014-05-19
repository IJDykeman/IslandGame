using System;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using Keys = Microsoft.Xna.Framework.Input.Keys;
using IslandGame;
using IslandGame.GameWorld;

namespace CubeAnimator{

    [Serializable] 
    public class AnimatedBodyPartGroup
    {
        protected BodyPart main;
        protected List<PositionForTime> positionQueue;
        private Vector3 modelLocation;
        private Quaternion modelRotation = Quaternion.Identity;


        protected AnimatedBodyPartGroup() { }

        public AnimatedBodyPartGroup(string path, float scale)
        {
            setupAnimatedBodyPartGroup(path, scale);
        }



        protected void setupAnimatedBodyPartGroup(string path)
        {
            positionQueue = new List<PositionForTime>();
            loadFromFile(path);
            main.setAnimations();


        }

        protected void setupAnimatedBodyPartGroup(string path, float scale)
        {
            positionQueue = new List<PositionForTime>();
            loadFromFile(path);
            main.setAnimations();

            setScale(scale);
        }

        public bool hasOnlyOnePart()
        {
            return main.getNumChildren() == 0;
        }

        public void setMip(int mipLevel)
        {
            main.setMipRecursive(mipLevel);
        }

        public void updateAnimations(List<AnimationType> animations)
        {
            updatePositionQueue();

            List<AnimationType> order = new List<AnimationType>();

            if (positionQueue.Count > 0)
            {
                order.AddRange(positionQueue[0].type);
            }

            order.AddRange(animations);


            main.orderAnimation(order, new noAnimation());
        }

        protected static List<PositionForTime> getHammerAnimation()
        {
            List<PositionForTime> result = new List<PositionForTime>();

            result.Add(new PositionForTime(10, AnimationType.hammerHitRaisedLeftArm));

            List<AnimationType> downSwing = new List<AnimationType>();
            downSwing.Add(AnimationType.hammerHitLoweredLeftArm);
            //downSwing.Add(AnimationType.torsoLeftShoulderForward);
            result.Add(new PositionForTime(20, downSwing));
            return result;
        }

        public void saveToFile(string path)
        {

            if(new FileInfo(path).Extension != ".chr")
            {
                path += ".chr";
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            List<string> fileContents = main.saveToFolder(path, new List<string>());

            using (StreamWriter sw = File.CreateText(path))
            {
                foreach (string write in fileContents)
                {
                    sw.WriteLine(write);
                }

            }

        }

        void updatePositionQueue()
        {
            if(positionQueue.Count==0)
            {
                return;
            }

            positionQueue[0].countDown--;
            if (positionQueue[0].countDown == 0)
            {
                positionQueue.RemoveAt(0);
            }
        }

        public void loadFromFile(string path)
        {
            string[] file = System.IO.File.ReadAllLines(path);
            main = new BodyPart();
            main.loadFromFile(file, 0, new FileInfo(path));
        }

        public void setScale(float scale)
        {
            main.setScale(scale);
        }


        public void setRootPartLocation(Vector3 newLoc)
        {
            modelLocation = newLoc;
        }

        public void setRootPartRotationOffset(Quaternion toSet)
        {
            modelRotation = toSet;
        }

        public Quaternion getRootPartRotationOffset()
        {
            return modelRotation;
        }

        public void draw(GraphicsDevice device, Effect effect)
        {

            setGraphicsStateForDraw(device);
            main.draw(device, effect, Matrix.CreateTranslation(modelLocation), modelRotation);
            resetGraphicsStateAfterDraw(effect);
        }

        public void drawWithPartTypeExcluded(GraphicsDevice device, Effect effect, BodyPartType toNotDraw)
        {

            setGraphicsStateForDraw(device);
            main.drawWithPartTypeExcluded(device, effect, Matrix.CreateTranslation(modelLocation), modelRotation, toNotDraw);
            resetGraphicsStateAfterDraw(effect);
        }

        private static void resetGraphicsStateAfterDraw(Effect effect)
        {
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
        }

        private static void setGraphicsStateForDraw(GraphicsDevice device)
        {
            Vector3 lightDirection = new Vector3(-.3f, .5f, -1f);
            lightDirection.Normalize();
            lightDirection *= (float).3f;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.Solid;
            device.RasterizerState = rasterizerState;
        }

        public void drawWithPresetBuffers(Effect effect)
        {

            Vector3 lightDirection = new Vector3(-.3f, .5f, -1f);
            lightDirection.Normalize();
            lightDirection *= (float).3f;
            main.drawWithPresetBuffers(effect, Matrix.CreateTranslation(modelLocation), modelRotation);
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
        }

        public bool isSinglePart()
        {
            return main.getNumChildren() == 0;
        }

        public Quaternion getWholeBodyRotation()
        {
            return modelRotation;
        }

        public void StartStrikeAnimation()
        {
            positionQueue = new List<PositionForTime>();
            positionQueue.Add(new PositionForTime(5, AnimationType.stabLeftArm));
        }

        public void StartHammerAnimation()
        {
            positionQueue = new List<PositionForTime>();
            positionQueue.AddRange(getHammerAnimation());
        }

        
    }
}
