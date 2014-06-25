using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IslandGame.GameWorld
{

    public static class NoiseGenerator
    {
        public static int Seed { get; private set; }

        public static int Octaves { get; set; }

        public static double Amplitude { get; set; }

        public static double Persistence { get; set; } 

        public static double Frequency { get; set; }

        //RANGE seems to be from -amplitude/2 to amplitude/2

        static NoiseGenerator()
        {
            Random r = new Random();
            NoiseGenerator.Seed = r.Next(Int32.MaxValue);
            NoiseGenerator.Octaves = 8;
            NoiseGenerator.Amplitude = 1;///1;
            NoiseGenerator.Frequency = .025;//0.015;
            NoiseGenerator.Persistence = .32;// 0.65;
        }

        public static void randomizeSeed()
        {
            Random r = new Random();
            NoiseGenerator.Seed = r.Next(Int32.MaxValue);
        }


        public static void setValuesForPass(int seed, int octaves, float amplitude, float frequency, float persistance)
        {

            NoiseGenerator.Seed = seed;
            NoiseGenerator.Octaves = octaves;
            NoiseGenerator.Amplitude = amplitude;///1;
            NoiseGenerator.Frequency = frequency;//0.015;
            NoiseGenerator.Persistence = persistance;// 0.65;
        }

        public static double Noise(int x, int y)
        {
            //returns -1 to 1
            double total = 0.0;
            double freq = NoiseGenerator.Frequency, amp = NoiseGenerator.Amplitude;
            for (int i = 0; i < NoiseGenerator.Octaves; ++i)
            {
                total = total + NoiseGenerator.Smooth(x * freq, y * freq) * amp;
                freq *= 2;
                amp *= NoiseGenerator.Persistence;
            }
            if (total < -2.4) total = -2.4;
            else if (total > 2.4) total = 2.4;

            return (total / 2.4);
        }

        public static double NoiseGeneration(int x, int y)
        {
            int n = x + y * 57;
            n = (n << 13) ^ n;

            return (1.0 - ((n * (n * n * 15731 + 789221) + NoiseGenerator.Seed) & 0x7fffffff) / 1073741824.0);
        }

        private static double Interpolate(double x, double y, double a)
        {
            double value = (1 - Math.Cos(a * Math.PI)) * 0.5;
            return x * (1 - value) + y * value;
        }

        private static double Smooth(double x, double y)
        {
            double n1 = NoiseGeneration((int)x, (int)y);
            double n2 = NoiseGeneration((int)x + 1, (int)y);
            double n3 = NoiseGeneration((int)x, (int)y + 1);
            double n4 = NoiseGeneration((int)x + 1, (int)y + 1);

            double i1 = Interpolate(n1, n2, x - (int)x);
            double i2 = Interpolate(n3, n4, x - (int)x);

            return Interpolate(i1, i2, y - (int)y);
        }

        public static Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
    }

}
