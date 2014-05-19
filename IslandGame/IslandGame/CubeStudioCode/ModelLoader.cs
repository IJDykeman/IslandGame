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


namespace CubeAnimator  ///ARRAYS ARE PASSED BY REFERENCE!!!!!!!! EVERYBODY HAS THE SAME ARRAY POINTERS!!!!!!!!!!!!!!!!!!
{
    static class ModelLoader
    {
        static Dictionary<string, LoadedCubeSpaceData> loadedByteArrays = new Dictionary<string, LoadedCubeSpaceData>();
        static Dictionary<string, VertexAndIndexBuffers> memoizedMips = new Dictionary<string, VertexAndIndexBuffers>();


        public static PaintedCubeSpace loadSpaceFromName(string folderPath)
        {
            if(loadedByteArrays.ContainsKey(folderPath))
            {
                return loadSpaceFromLoadedSpaces(folderPath);
            }

            return loadSpaceFromDisk(folderPath);
        }


        static PaintedCubeSpace loadSpaceFromDisk(string path)
        {

            

            byte[, ,] byteArray;

            Stream stream = File.Open(path, FileMode.Open);

            BinaryFormatter formatter = new BinaryFormatter();
            byteArray = (byte[, ,])formatter.Deserialize(stream);

            FileInfo fileInfo = new FileInfo(path);
            long fileLength = fileInfo.Length;
            int spaceWidth = (int)Math.Pow(fileLength, 1.0 / 3.0);
            int spaceHeight = (int)Math.Pow(fileLength, 1.0 / 3.0);

            stream.Close();

            //loadedByteArrays.Add(path, byteArray);
            PaintedCubeSpace paintedCubeSpace = new PaintedCubeSpace(1, 1, new Vector3());
            paintedCubeSpace.spaceWidth = spaceWidth;
            paintedCubeSpace.spaceHeight = spaceHeight;
            paintedCubeSpace.array = byteArray;

            paintedCubeSpace.createModel(Main.graphics.GraphicsDevice);

            LoadedCubeSpaceData dataToSave = new LoadedCubeSpaceData();
            dataToSave.array = byteArray;
            dataToSave.spaceHeight = spaceHeight;
            dataToSave.spaceWidth = spaceWidth;
            dataToSave.vertexBuffer = paintedCubeSpace.getVertexBuffer();
            dataToSave.indexBuffer = paintedCubeSpace.getIndexBuffer();
            loadedByteArrays.Add(path, dataToSave);

            paintedCubeSpace.setLoadedFrompath(path);
            return paintedCubeSpace;
        }

        static PaintedCubeSpace loadSpaceFromLoadedSpaces(string path)
        {
            LoadedCubeSpaceData data = loadedByteArrays[path];

            PaintedCubeSpace space = new PaintedCubeSpace(data.spaceWidth, data.spaceHeight, new Vector3());
            space.array = /*new byte[data.spaceWidth, data.spaceHeight, data.spaceWidth];*/data.array;
            //data.unmippedArray.CopyTo(space.unmippedArray, 0);
            space.setUnmippedBuffers(data.vertexBuffer, data.indexBuffer);
            space.setLoadedFrompath(path);
            return space;
        }

        public static bool hasMipAtPathAndLevel(string path, int mipLevel)
        {
            return memoizedMips.Keys.Contains(getPathWidthMipString(path, mipLevel));
        }

        public static VertexAndIndexBuffers getBuffersAtPathAtMipLevel(string path, int mipLevel)
        {
            return memoizedMips[getPathWidthMipString(path, mipLevel)];
        }

        public static void addPathAndMipForMemoization(string path, int mipLevel, VertexAndIndexBuffers buffers)
        {
            if (!memoizedMips.Keys.Contains(getPathWidthMipString(path, mipLevel)))
            {
                memoizedMips.Add(getPathWidthMipString(path, mipLevel), buffers);
            }
        }

        private static string getPathWidthMipString(string path, int mipLevel)
        {
            return path + mipLevel;
        }

    }
}
