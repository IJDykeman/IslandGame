using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace IslandGame.audio
{
    class SoundGroup
    {
        SoundEffect[] effects;
        SoundEffectInstance voice;
        public SoundGroup(String[] pathsToLoad, ContentManager content)
        {
            effects = new SoundEffect[pathsToLoad.Length];
            for (int i = 0; i < pathsToLoad.Length; i++)
            {
                effects[i] = content.Load<SoundEffect>(pathsToLoad[i]);
            }
        }

        public void playASound(Vector3 listenerLoc, Vector3 emitterLoc, float volumeNormal)
        {
            Random rand = new Random();
            //effects[rand.Next(effects.Length)].Play();

            
            voice = effects[rand.Next(effects.Length)].CreateInstance();

            voice.Volume = MathHelper.Clamp(10.0f / (float)Math.Pow(Vector3.Distance(listenerLoc, emitterLoc), 1.7), 0, 1) * volumeNormal;
            //voice.Pitch = -1;
            //voice.Apply3D(listener, emitter);

            //voice.IsLooped = isLooped;
            
            voice.Play();

            //sounds.Add(voice);
        }
    }
}
