using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IslandGame.GameWorld;
using CubeAnimator;
using System.IO;


namespace IslandGame
{
    static class WorldMarkupHandler
    {


        private static Dictionary<string, MemoizedModelAndPoses> FilePathsAndPositions = new Dictionary<string, MemoizedModelAndPoses>();
        //private static Dictionary

        public static void addFlagPathWithPosition(string path, Vector3 position)
        {
            path = getProperPath(path);
            if (path.Contains(("Outline.chr").ToUpper()) || path.Contains(("Marker.chr").ToUpper()) || path.Contains(("wheatGrowthStage").ToUpper()))
            {
                float scale = 1.0f / 12f;
                addFlagPathWithPosition(path, position, scale);
            }
            else if (path.Contains((@"\log.chr").ToUpper()))
            {
                addFlagPathWithPosition(path, position, 1f / 7f);
            }
            else
            {

                addFlagPathWithPosition(path, position, 1);
            }
        }

        private static string getProperPath(string path)
        {
            path = path.ToUpper();
            if (!path.Contains(@"C:\".ToUpper()))
            {
                path = ContentDistributor.getRootPath() + path;
            }
            return path;
        }

        public static void addCharacter(string path, Vector3 position,float scale, float opacity)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Extension.ToUpper().Equals(".VOX"))
            {
                

            }
            else if (fileInfo.Extension.ToUpper().Equals(".CHR"))
            {
                AnimatedBodyPartGroup character = new AnimatedBodyPartGroup(path, scale);
                character.addToWorldMarkup(Matrix.CreateTranslation(position), Quaternion.Identity);
            }
        }

        public static void addCharacter(AnimatedBodyPartGroup group, Vector3 position)
        {

            group.addToWorldMarkup(Matrix.CreateTranslation(position), Quaternion.Identity);
            
        }

        /*
        public static void addFlagWithMatrix(string path,Matrix matrix)
        {
            //path = path.ToUpper();
            if (FilePathsAndPositions.ContainsKey(path.ToUpper()))
            {
                FilePathsAndPositions[path.ToUpper()].addPose(new MatrixAndOpacity(matrix));
            }
            else
            {
                List<MatrixAndOpacity> PosScaleListForNewPath = new List<MatrixAndOpacity>();
                PosScaleListForNewPath.Add(new MatrixAndOpacity(matrix));
                FilePathsAndPositions.Add(path.ToUpper(), PosScaleListForNewPath);

            }
        }*/

        public static void addFlagWithMatrix(string path, Matrix matrix, PaintedCubeSpaceDisplayComponant model)
        {
            path = path.ToUpper();
            if (FilePathsAndPositions.ContainsKey(path))
            {

                //MemoizedModelAndPoses memoized = new MemoizedModelAndPoses(model);
                //memoized.addPose( new MatrixAndOpacity(position, scale,opacity));
                FilePathsAndPositions[path].addPose(new MatrixAndOpacity(matrix));
            }
            else
            {

                MemoizedModelAndPoses memoized = new MemoizedModelAndPoses(model);
                memoized.addPose(new MatrixAndOpacity(matrix));
                FilePathsAndPositions.Add(path, memoized);

            }
        }

        public static void addFlagPathWithPosition(string path, Vector3 position, float scale)
        {
            addCharacter(path, position, scale, 1);
        }


        public static void drawCharacters(Microsoft.Xna.Framework.Graphics.GraphicsDevice device, Microsoft.Xna.Framework.Graphics.Effect effect)
        {
            foreach (KeyValuePair<string, MemoizedModelAndPoses> entry in FilePathsAndPositions)
            {
                if (entry.Key.Contains(("Outline.chr").ToUpper()) || entry.Key.Contains(("farmMarker.chr").ToUpper()))
                {
                    effect.Parameters["xOpacity"].SetValue(.4f);

                }

                entry.Value.draw(device, effect);
                
                
            }

            

            effect.Parameters["xOpacity"].SetValue(1f);
        }

        private static MatrixAndOpacity setBodyPartGroupToParams(AnimatedBodyPartGroup flag, MatrixAndOpacity posScaleOpacity)
        {
            Vector3 scale;
            Vector3 translation;
            Quaternion rotation;
            posScaleOpacity.matrix.Decompose(out scale, out rotation, out translation);
            flag.setScale(scale.X);
            flag.setRootPartLocation(translation);

            flag.setRootPartRotationOffset(rotation);
            return posScaleOpacity;
        }

        public static void resetWorldMarkup()
        {
            FilePathsAndPositions.Clear();
        }



    }
}
