using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IslandGame
{
    public enum DisplayParameter
    {
        drawStockpiles
    }

    public class DisplayParameters
    {
        HashSet<DisplayParameter> parameters;

        public DisplayParameters()
        {
            parameters = new HashSet<DisplayParameter>();
        }

        public void addParameter(DisplayParameter toAdd)
        {
            parameters.Add(toAdd);
        }

        public bool hasParameter(DisplayParameter parameter)
        {
            return parameters.Contains(parameter);
        }
    }
}
