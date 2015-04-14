using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace TerrainDetectorServices.Objects
{
    public class ColourCount
    {
        public Color Colour { get; set; }
        public int Count {get;set;}

        public ColourCount()
        {
            Count = 0;
        }
    }
}