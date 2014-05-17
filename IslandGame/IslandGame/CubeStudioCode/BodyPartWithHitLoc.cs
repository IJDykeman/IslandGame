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


namespace CubeAnimator
{
    [Serializable] 
    public struct BodyPartWithHitLoc
    {
        public CubeAnimator.BodyPart part;
        public Vector3 locInSpace;
        public Vector3 locInWorld;

        public BodyPartWithHitLoc(CubeAnimator.BodyPart nPart, Vector3 nlocInSpace, Vector3 nLocInWorldSpace)
        {
            part = nPart;
            locInSpace = nlocInSpace;
            locInWorld = nLocInWorldSpace;
        }
    }
}
