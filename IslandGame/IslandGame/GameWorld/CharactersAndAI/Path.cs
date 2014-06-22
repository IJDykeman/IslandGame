using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame.GameWorld
{
    public class Path
    {
        List<BlockLoc> list;

        public Path()
        {
            list = new List<BlockLoc>();
        }

        public Path(List<BlockLoc> nList)
        {
            if (nList == null)
            {
                list = new List<BlockLoc>();
            }
            else
            {
                list = nList;
            }
        }

        public void add(BlockLoc toAdd)
        {
            list.Add(toAdd);
        }

        public BlockLoc getAt(int pos)
        {
            return list[pos];
        }

        public void removeAt(int pos)
        {
             list.RemoveAt(pos);
        }

        public BlockLoc? getLast()
        {
            if (list.Count == 0)
            {
                return null;
            }
            else
            {
                return list[list.Count - 1];
            }
        }

        public BlockLoc getFirst()
        {
            return list[0];
        }

        public int length()
        {
            if (list == null)
            {
                return 0;
            }
            else
            {
                return list.Count;
            }
        }

        public bool isUseable()
        {
            return list.Count > 0;
        }

    }
}
