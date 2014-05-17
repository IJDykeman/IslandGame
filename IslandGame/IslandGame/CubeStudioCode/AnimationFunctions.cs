using System;
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
    static class AnimationFunctions
    {
        public static Quaternion mirror(Quaternion toMirror)
        {
            //return Matrix.Transform(Matrix.CreateScale(-1, 1, 1), toMirror);
            return new Quaternion(-toMirror.X, toMirror.Y, toMirror.Z, -toMirror.W);
            //return Conversions.EulerToQuaternion(Conversions.QuaternionToEulerX(q0), -1 * Conversions.QuaternionToEulerY(q0), Conversions.QuaternionToEulerZ(q0));
        }
        public static float UnsignedAngleBetweenTwoV3(Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();
            double Angle = (float)Math.Acos(Vector3.Dot(v1, v2));
            return (float)Angle;
        }

        public static float angleBetweenQuaternions(Quaternion q1, Quaternion q2)
        {
            return UnsignedAngleBetweenTwoV3(Vector3.Transform(Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ, Matrix.CreateFromQuaternion(q1)),
                Vector3.Transform(Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ, Matrix.CreateFromQuaternion(q2)));
        }

        public static void mirrorThisQuaternion(Quaternion test)
        {
            test.W *= -1;
            test.X *= -1;
            test.Y *= -1;
            test.Z *= -1;
            
        }
    }
}
