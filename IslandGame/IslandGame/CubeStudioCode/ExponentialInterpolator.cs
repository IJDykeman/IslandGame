using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeAnimator
{
    [Serializable] 
    public class ExponentialInterpolator
    {
        public float value;
        public float idealValue;
        public readonly float lowerLimit;
        public readonly float upperLimit;
        public float speed;

        public ExponentialInterpolator(float nValue, float nIdealValue, float nLowerLimit, float nUpperLimmit, float nSpeed)
        {
            value = nValue;
            idealValue = nIdealValue;
            lowerLimit = nLowerLimit;
            upperLimit = nUpperLimmit;
            speed = nSpeed;
        }

        public void update()
        {

            if (idealValue > upperLimit)
            {
                idealValue = upperLimit;
            }

            if (idealValue < lowerLimit)
            {
                idealValue = lowerLimit;
            }

            value -= (value - idealValue) * speed;

            if (value > upperLimit)
            {
                value = upperLimit;
            }

            if (value < lowerLimit)
            {
                value = lowerLimit;
            }
        }
    }
}
