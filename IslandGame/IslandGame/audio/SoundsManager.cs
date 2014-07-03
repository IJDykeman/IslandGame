using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using IslandGame.audio;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    public static class SoundsManager
    {
        private static Player player; //dirty?
        public enum SoundType
        {
            wheatRustling
        }
        static Dictionary<SoundType, SoundGroup> soundGroups;

        public static void loadContent(ContentManager content)
        {
            soundGroups = new Dictionary<SoundType, SoundGroup>();

            string[] rustingPaths = new string[15];
            for (int i = 0; i < rustingPaths.Length; i++)
            {
                rustingPaths[i] = "sounds/rustling/rustle" +(i + 1);
            }
            soundGroups.Add(SoundType.wheatRustling, new SoundGroup(rustingPaths, content));

        }

        public static void playSoundType(SoundType toPlay, Vector3 emitterLoc)
        {
            soundGroups[toPlay].playASound(player.getCameraLoc(),emitterLoc,1);
        }


        public static void playSoundType(SoundType toPlay, Vector3 emitterLoc, float volumeNormal)
        {
            volumeNormal = MathHelper.Clamp(volumeNormal, 0, 1);
            soundGroups[toPlay].playASound(player.getCameraLoc(), emitterLoc, volumeNormal);
        }

        internal static void setPlayer(Player nplayer)
        {
            player = nplayer;
        }
    }
}
