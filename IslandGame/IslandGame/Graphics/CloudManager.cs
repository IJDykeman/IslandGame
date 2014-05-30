using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CubeAnimator;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IslandGame.Graphics
{
    class CloudManager
    {
        //switch this system to work through the WorldMarkupManager!  It will be faster that way.
        List<AnimatedBodyPartGroup> clouds;
        String[] cloudPaths = {ContentDistributor.getRootPath()+@"world_decoration\clouds\cloud1.chr",
                                  ContentDistributor.getRootPath()+@"world_decoration\clouds\cloud2.chr",
                                  ContentDistributor.getRootPath()+@"world_decoration\clouds\cloud3.chr",
                              };


        public CloudManager()
        {
            clouds = new List<AnimatedBodyPartGroup>();
            Random rand = new Random();
            for (int i = 0; i < 60; i++)
            {
                        makeCloud(new Vector3((float)(rand.NextDouble() - .3) * 5000.0f, 400, (float)(rand.NextDouble() - .3) * 5000.0f), rand);
            }


            

        }

        void makeCloud(Vector3 loc, Random rand)
        {
            AnimatedBodyPartGroup cloud = new AnimatedBodyPartGroup(cloudPaths[rand.Next(cloudPaths.Length)], 5+(float)rand.NextDouble()*6.0f);
            cloud.setRootPartLocation(loc);
            clouds.Add(cloud);
        }

        public void display(GraphicsDevice device, Effect effect)
        {
            foreach (AnimatedBodyPartGroup cloud in clouds)
            {
                cloud.draw(device, effect);
            }
        }
    }
}
