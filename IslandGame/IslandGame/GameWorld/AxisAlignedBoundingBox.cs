using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace IslandGame.GameWorld
{
    [Serializable]
    public class AxisAlignedBoundingBox
    {
        public Vector3 loc;//at mininum X Y Z
        public float Xwidth;
        public float Zwidth;
        public float height;


        public AxisAlignedBoundingBox(Vector3 Nloc, float NwidthX, float NwidthZ, float Nheight)
        {
            loc = Nloc;
            Zwidth = NwidthZ;
            Xwidth = NwidthX;
            height = Nheight;

        }

        public AxisAlignedBoundingBox(Vector3 min, Vector3 max)
        {
            loc = min;
            Zwidth = max.Z - min.Z;
            Xwidth = max.X - min.X;
            height = max.Y - min.Y;

        }

        public bool hasLegalMinMax()
        {
            Vector3 max = loc + new Vector3(Xwidth, height, Xwidth);
            return (loc.X < max.X && loc.Z < max.Z && loc.Y < max.Y);
        }

        public bool intersectsAABB(AxisAlignedBoundingBox test)
        {
            return (intersectsAABBWithAddonVector(test, new Vector3()));
        }

        public bool intersectsAABBWithAddonVector(AxisAlignedBoundingBox test, Vector3 addOnToLocationOfTester)
        {
            Vector3 testVec = loc + addOnToLocationOfTester;
            if (testVec.Y <= test.loc.Y + test.height && testVec.Y + height >= test.loc.Y)
            {
                if (testVec.Z <= test.loc.Z + test.Zwidth && testVec.Z + Zwidth >= test.loc.Z)
                {
                    if (testVec.X <= test.loc.X + test.Xwidth && testVec.X + Xwidth >= test.loc.X)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public bool lineIntersectsBox(Vector3 line1, Vector3 line2)
        {
            Vector3 test = new Vector3();
            return CheckLineBox(loc, loc + new Vector3(Xwidth, height, Zwidth), line1, line2, ref test);
        }

        public bool CheckLineBox(Vector3 B1, Vector3 B2, Vector3 L1, Vector3 L2, ref Vector3 Hit)
        {
            if (L2.X < B1.X && L1.X < B1.X) return false;
            if (L2.X > B2.X && L1.X > B2.X) return false;
            if (L2.Y < B1.Y && L1.Y < B1.Y) return false;
            if (L2.Y > B2.Y && L1.Y > B2.Y) return false;
            if (L2.Z < B1.Z && L1.Z < B1.Z) return false;
            if (L2.Z > B2.Z && L1.Z > B2.Z) return false;
            if (L1.X > B1.X && L1.X < B2.X &&
                L1.Y > B1.Y && L1.Y < B2.Y &&
                L1.Z > B1.Z && L1.Z < B2.Z)
            {
                Hit = L1;
                return true;
            }
            if ((GetIntersection(L1.X - B1.X, L2.X - B1.X, L1, L2, ref Hit) && InBox(Hit, B1, B2, 1))
              || (GetIntersection(L1.Y - B1.Y, L2.Y - B1.Y, L1, L2, ref Hit) && InBox(Hit, B1, B2, 2))
              || (GetIntersection(L1.Z - B1.Z, L2.Z - B1.Z, L1, L2, ref Hit) && InBox(Hit, B1, B2, 3))
              || (GetIntersection(L1.X - B2.X, L2.X - B2.X, L1, L2, ref Hit) && InBox(Hit, B1, B2, 1))
              || (GetIntersection(L1.Y - B2.Y, L2.Y - B2.Y, L1, L2, ref Hit) && InBox(Hit, B1, B2, 2))
              || (GetIntersection(L1.Z - B2.Z, L2.Z - B2.Z, L1, L2, ref Hit) && InBox(Hit, B1, B2, 3)))
                return true;

            return false;
        }
        bool GetIntersection(float fDst1, float fDst2, Vector3 P1, Vector3 P2, ref Vector3 Hit)
        {
            if ((fDst1 * fDst2) >= 0.0f) return false;
            if (fDst1 == fDst2) return false;
            Hit = P1 + (P2 - P1) * (-fDst1 / (fDst2 - fDst1));
            return true;
        }
        bool InBox(Vector3 Hit, Vector3 B1, Vector3 B2, int Axis)
        {
            if (Axis == 1 && Hit.Z > B1.Z && Hit.Z < B2.Z && Hit.Y > B1.Y && Hit.Y < B2.Y) return true;
            if (Axis == 2 && Hit.Z > B1.Z && Hit.Z < B2.Z && Hit.X > B1.X && Hit.X < B2.X) return true;
            if (Axis == 3 && Hit.X > B1.X && Hit.X < B2.X && Hit.Y > B1.Y && Hit.Y < B2.Y) return true;
            return false;
        }

        public bool pointInBox(Vector3 test)
        {
            if (test.X > loc.X && test.Y > loc.Y && test.Z > loc.Z && test.X < max().X && test.Y < max().Y && test.Z < max().Z)
            {
                return true;
            }
            return false;
        }

        public Vector3 max()
        {
            return loc + new Vector3(Xwidth, height, Zwidth);
        }
        public Vector3 middle()
        {
            return loc + new Vector3(Xwidth / 2.0f, height / 2.0f, Zwidth / 2.0f);
        }

        public AxisAlignedBoundingBox expansion(float expansionAmout)
        {
            return new AxisAlignedBoundingBox(loc - new Vector3(expansionAmout, expansionAmout, expansionAmout), max() + new Vector3(expansionAmout, expansionAmout, expansionAmout));
        }

        public AxisAlignedBoundingBox expansion(AxisAlignedBoundingBox expansionBox)
        {
            return new AxisAlignedBoundingBox(loc - (expansionBox.middle() - expansionBox.loc), max() + (expansionBox.max() - expansionBox.middle()));
        }

        public BoundingBox getBoundingBox()
        {
            return new BoundingBox(loc, loc + new Vector3(Xwidth, height, Zwidth));
        }
    }
}
