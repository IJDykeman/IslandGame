using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame
{
    [Serializable] 
    public struct IntVector3
    {
        public int X;
        public int Y;
        public int Z;

        public IntVector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public IntVector3(Vector3 floatingPointLocation)
        {
            X = (int)floatingPointLocation.X;
            Y = (int)floatingPointLocation.Y;
            Z = (int)floatingPointLocation.Z;

            if (floatingPointLocation.X < 0)
            {
                //X--;
            }
            if (floatingPointLocation.Y < 0)
            {
                //Y--;
            }
            if (floatingPointLocation.Z < 0)
            {
                //Z--;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is IntVector3)
            {
                return ((IntVector3)obj) == this;
            }
            return false;
        }

        public Vector3 toVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public static bool operator ==(IntVector3 value1, IntVector3 value2)
        {
            return (value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z);
        }

        public static bool operator !=(IntVector3 value1, IntVector3 value2)
        {
            return (value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z);
        }

        public static IntVector3 operator +(IntVector3 value1, IntVector3 value2)
        {
            return new IntVector3(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);
        }

        public int getHashCode()
        {
            string toHash = "X:" + X + "Y:" + Y + "Z:" + Z;
            return toHash.GetHashCode();
        }


    }
}
