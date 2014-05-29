using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IslandGame.GameWorld;
using CubeAnimator;


namespace IslandGame
{
    static class WorldMarkupHandler
    {
        

        private static Dictionary<string, List<PositionScaleOpacity>> FilePathsAndPositions = new Dictionary<string, List<PositionScaleOpacity>>();
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

        public static void addFlagPathWithPosition(string path, Vector3 position,float scale, float opacity)
        {
            path = path.ToUpper();
            if (FilePathsAndPositions.ContainsKey(path))
            {
                FilePathsAndPositions[path].Add(new PositionScaleOpacity(position, scale,opacity));
            }
            else
            {
                List<PositionScaleOpacity> PosScaleListForNewPath = new List<PositionScaleOpacity>();
                PosScaleListForNewPath.Add(new PositionScaleOpacity(position, scale,opacity));
                FilePathsAndPositions.Add(path, PosScaleListForNewPath);

            }
        }

        public static void addFlagPathWithPosition(string path, Vector3 position, float scale)
        {
            addFlagPathWithPosition(path, position, scale, 1);
        }


        public static void draw(Microsoft.Xna.Framework.Graphics.GraphicsDevice device, Microsoft.Xna.Framework.Graphics.Effect effect)
        {
            foreach (KeyValuePair<string, List<PositionScaleOpacity>> entry in FilePathsAndPositions)
            {
                if (entry.Key.Contains(("Outline.chr").ToUpper()) || entry.Key.Contains(("farmMarker.chr").ToUpper()))
                {
                    effect.Parameters["xOpacity"].SetValue(.4f);

                }
                

                AnimatedBodyPartGroup flag = new AnimatedBodyPartGroup( entry.Key, 1f);

                if (entry.Value.Count > 0)
                {
                    PositionScaleOpacity posScaleOpacity = entry.Value[0];
                    flag.setScale(posScaleOpacity.scale);
                    flag.setRootPartLocation(posScaleOpacity.loc);
                    effect.Parameters["xOpacity"].SetValue(posScaleOpacity.opacity);
                    flag.draw(device, effect);
                    


                    for (int i = 1; i < entry.Value.Count; i++)
                    {
                        posScaleOpacity = entry.Value[i];
                        flag.setScale(posScaleOpacity.scale);
                        flag.setRootPartLocation(posScaleOpacity.loc);
                        effect.Parameters["xOpacity"].SetValue(posScaleOpacity.opacity);
                        if (flag.isSinglePart())
                        {
                            flag.drawWithPresetBuffers(effect);
                        }
                        else
                        {
                            flag.draw(device, effect);
                        }
                    }
                }
            }

            

            resetWorldMarkup();
            effect.Parameters["xOpacity"].SetValue(1f);
        }

        private static void resetWorldMarkup()
        {
            FilePathsAndPositions.Clear();
        }



    }
}
