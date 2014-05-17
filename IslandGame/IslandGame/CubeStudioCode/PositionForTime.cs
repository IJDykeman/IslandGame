using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeAnimator
{
    [Serializable] 
    public class PositionForTime
    {
        public int countDown;
        public List<AnimationType> type;
        public PositionForTime(int nCountDown, AnimationType nType)
        {
            countDown = nCountDown;
            //taskType = nType;
            type = new List<AnimationType>();
            type.Add(nType);
        }
        public PositionForTime(int nCountDown, List<AnimationType> nType)
        {
            countDown = nCountDown;
            type = nType;
        }


    }
}
