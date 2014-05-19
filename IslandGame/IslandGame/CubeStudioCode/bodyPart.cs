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
using IslandGame.GameWorld;
using IslandGame;


namespace CubeAnimator
{
    [Serializable] 
    public class BodyPart
    {
        protected PaintedCubeSpace model;
        protected List<BodyPart> children;
        protected AnimationSystem animationSystem;
        protected BodyPartType type;
        protected string fileName;
        protected bool highlighted = false;
        public Quaternion rotationOffset;

        public BodyPart()
        {
            rotationOffset = Quaternion.Identity;
            children = new List<BodyPart>();
            animationSystem = new noAnimation();
        }

        public BodyPart(PaintedCubeSpace nModel, Vector3 nLoc, BodyPartType nType, string nFileName)
        {
            model = nModel;
            model.loc = nLoc;
            children = new List<BodyPart>();
            animationSystem = new noAnimation();
            type = nType;
            fileName = nFileName;
            rotationOffset = Quaternion.Identity;
        }

        public static BodyPartType getBodyPartTypeFromString(string test)
        {
            Array values = Enum.GetValues(typeof(BodyPartType));
            foreach (BodyPartType val in values)
            {
                if (Enum.GetName(typeof(BodyPartType),val).Equals(test))
                {
                    return val;
                }
            }
            return BodyPartType.unknown;
        }

        public static BodyPartType getBodyPartTypeFromFullName(string test)
        {
            switch (test)
            {
                case "Humanoid Body":
                    return BodyPartType.torso;
                case "Humanoid Head":
                    return BodyPartType.head;
                case "Humanoid Left Arm":
                    return BodyPartType.leftArm;
                case "Humanoid Left Lower Arm":
                    return BodyPartType.lowerLeftArm;
                case "Humanoid Right Arm":
                    return BodyPartType.rightArm;
                case "Humanoid Right Lower Arm":
                    return BodyPartType.lowerRightArm;
                case "Humanoid Left Leg":
                    return BodyPartType.leftLeg;
                case "Humanoid Left Lower Leg":
                    return BodyPartType.lowerLeftLeg;
                case "Humanoid Right Leg":
                    return BodyPartType.rightLeg;
                case "Humanoid Right Lower Leg":
                    return BodyPartType.lowerRightLeg;
                case "Wheel":
                    return BodyPartType.wheel;
                case "Rigid Connection":
                    return BodyPartType.rigid;
                default:
                    return BodyPartType.unknown;
            }
        }

        public void draw(GraphicsDevice device, Effect effect,Matrix superMatrix, Quaternion superRotation)
        {
            if (model.canBeDrawn())
            {

                model.drawForBodyPart(device, effect, superMatrix, animationSystem.rotation * rotationOffset * superRotation, highlighted);
                foreach (BodyPart child in children)
                {
                    child.draw(device, effect, model.getMatrix(superMatrix, animationSystem.rotation * rotationOffset * superRotation), Quaternion.Identity);
                }
            }
            else
            {
                model.createModel(device);
            }
        }

        public void drawWithPartTypeExcluded(GraphicsDevice device, Effect effect, Matrix superMatrix, Quaternion superRotation, BodyPartType partTypeNotToDraw)
        {
            if (model.canBeDrawn())
            {
                if (type != BodyPartType.head)
                {
                    model.drawForBodyPart(device, effect, superMatrix, animationSystem.rotation * rotationOffset * superRotation, highlighted);
                }
                foreach (BodyPart child in children)
                {
                    child.drawWithPartTypeExcluded(device, effect, model.getMatrix(superMatrix, animationSystem.rotation * rotationOffset * superRotation), Quaternion.Identity, partTypeNotToDraw);
                }
            }
            else
            {
                model.createModel(device);
            }
        }

        public void drawWithPresetBuffers(Effect effect, Matrix superMatrix, Quaternion superRotation)
        {

           model.drawForBodyPartWithPresetBuffers(effect, superMatrix, animationSystem.rotation * rotationOffset * superRotation, highlighted);

        }

        public int getNumChildren()
        {
            return children.Count;
        }

        public void setAnimations()
        {
            animationSystem = AnimationSystemFactory.getSystemFromType(type);
            foreach (BodyPart child in children)
            {
                child.setAnimations();
            }
        }

        public void orderAnimation(List<AnimationType> type, AnimationSystem parent)
        {
            animationSystem.handleOrder(type, parent);
            foreach (BodyPart child in children)
            {
                child.orderAnimation(type,animationSystem);
            }
        }

        public BodyPartWithHitLoc? getNearestHit(Vector3 firstRef, Vector3 secondRef,Matrix superMatrix, Quaternion rotation)
        {


            Vector3? maybeHit = model.getNearestHitInSpaceContext(firstRef, secondRef,superMatrix,rotation*rotationOffset);
            //IConsole.WriteLine(firstRef + "  " + secondRef);
            List<BodyPartWithHitLoc> results = new List<BodyPartWithHitLoc>();
            if (maybeHit.HasValue)
            {

                // return null;

                Vector3 hit = (Vector3)maybeHit;
                Vector3 hitInWorldSpace = Vector3.Transform(hit, model.getMatrix(superMatrix, rotation*rotationOffset));
                results.Add(new BodyPartWithHitLoc(this, hit, hitInWorldSpace));

            }
            

            foreach (BodyPart child in children)
            {

                BodyPartWithHitLoc? test = child.getNearestHit(firstRef, secondRef, model.getMatrix(superMatrix, rotation * rotationOffset), rotation);
                if (test.HasValue)
                {

                    results.Add((BodyPartWithHitLoc)test);
                }
            }
            
            return (getNearestHitBodyPartTo(firstRef, results));
        }

        BodyPartWithHitLoc? getNearestHitBodyPartTo(Vector3 firstRef, List<BodyPartWithHitLoc> list)
        {
            float distance = float.MaxValue;
            BodyPartWithHitLoc? result = null;

            foreach (BodyPartWithHitLoc test in list)
            {
                if (Vector3.Distance(test.locInWorld, firstRef) < distance)
                {
                    result = test;
                    distance = Vector3.Distance(test.locInWorld, firstRef); 
                }
            }
            return result;
        }

        public List<String> saveToFolder(string descriptionPath, List<String> stringList)
        {

                
                stringList.Add(fileName + " " + model.loc.X + " " + model.loc.Y + " " + model.loc.Z + " " +type + " "
                    + rotationOffset.X + " " + rotationOffset.Y + " " + rotationOffset.Z + " " + rotationOffset.W + " " + model.scale);
                stringList.Add("[");

            foreach (BodyPart child in children)
            {
                child.saveToFolder(descriptionPath, stringList);
            }

                stringList.Add("]");
                return stringList;


        }

        public void loadFromFile(string[] file, int place, FileInfo characterFile)
        {
            int bracketCount = 0;
            string[] firstLine = file[place].Split(' ');
            fileName = firstLine[0];

            string voxelFilePath = fileName;
            if (fileName[0] == '.')
            {
                voxelFilePath = (characterFile.DirectoryName + fileName.Substring(1));
            }

            model = ModelLoader.loadSpaceFromName(voxelFilePath);
            //model.createModel(Main.graphics.GraphicsDevice);
            model.loc = new Vector3((float)Convert.ToDouble(firstLine[1]), (float)Convert.ToDouble(firstLine[2]), (float)Convert.ToDouble(firstLine[3]));

            rotationOffset = new Quaternion((float)Convert.ToDouble(firstLine[5]), (float)Convert.ToDouble(firstLine[6]), (float)Convert.ToDouble(firstLine[7]), (float)Convert.ToDouble(firstLine[8]));
            model.scale = (float)Convert.ToDouble(firstLine[9]);


            type = getBodyPartTypeFromString(firstLine[4]);

            place++;
            for (; place < file.Length; place++)
            {
                if (file[place].Equals("["))
                {
                    bracketCount++;
                }
                else if (file[place].Equals("]"))
                {
                    bracketCount--;
                }
                if (bracketCount == 0)
                {
                    break;
                }
                if (bracketCount == 1 && !file[place].Contains("]") && !file[place].Contains("["))
                {
                    BodyPart newChild = new BodyPart();
                    newChild.loadFromFile(file, place,characterFile);
                    children.Add(newChild);
                }

            }

        }

        public void removePart(BodyPart toRemove)
        {
            for(int i=0;i<children.Count;i++)
            {
                if (children[i] == toRemove)
                {
                    children.RemoveAt(i);
                }
                else
                {
                    children[i].removePart(toRemove);
                }
            }
        }

        public void replacePart(BodyPart toReplace, PaintedCubeSpace replacement)
        {
            if (toReplace == this)
            {
                model.replaceArrayWith(replacement.array);
                
                return;
            }
            foreach (BodyPart child in children)
            {
                child.replacePart(toReplace, replacement);
            }
        }

        public void changePartType(BodyPart toReplace, BodyPartType replacement)
        {
            if (toReplace == this)
            {
                //model.replaceArrayWith(replacement.unmippedArray);
                type = replacement;
                return;
            }
            foreach (BodyPart child in children)
            {
                child.changePartType(toReplace, replacement);
            }
        }

        public void remesh(GraphicsDevice device)
        {
            model.createModel(device);
            foreach (BodyPart child in children)
            {
                child.remesh(device);
            }
        }

        public void clearTransformations()
        {
            foreach (BodyPart child in children)
            {
                child.clearTransformations();
            }
            animationSystem.returnToDefaultPosition();
        }

        public void highlight(BodyPart toHighlight)//passing null will unhighlight all
        {
            highlighted = false;
            if (this == toHighlight)
            {
                highlighted = true;

                //return;
            }
            foreach (BodyPart child in children)
            {
                child.highlight(toHighlight);
            }


        }

        public BodyPart getSelectedPart()
        {
            if (highlighted)
            {
                return this;
            }
            foreach (BodyPart child in children)
            {
                if (child.getSelectedPart() != null)
                {
                    return child.getSelectedPart();
                }
            }
            return null;
        }

        public void setMipRecursive(int mipLevel)
        {
            model.setMipLevel(mipLevel);
            foreach (BodyPart child in children)
            {
                child.setMipRecursive(mipLevel);
            }
        }

        public void setScale(float nScale)
        {
            if (nScale > 0)
            {
                model.scale = nScale;
            }
        }

        public float getScale()
        {
            return model.scale;
        }

        public void moveTo(Vector3 nLoc)
        {
            model.loc = nLoc;
        }

        public Vector3 getLocation()
        {
            return model.loc;
        }



    }
}
