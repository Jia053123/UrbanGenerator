using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Rhino.Geometry;

namespace UrbanGenerator
{
    class Window
    {
        public int Azimuth { get; }
        public float Area { get; }
        public float UFactor { get; }
        public float SHGC { get; }
        public string WallIdAttachedTo { get; }

        public WindowOverhang Overhang { get; set; }

        public PlaneSurface WindowSurface;

        public Window(int azimuth, float area, float uFactor, float shgc, string wallIdAttachedTo)
        {
            this.Azimuth = azimuth;
            this.Area = area;
            this.UFactor = uFactor;
            this.SHGC = shgc;
            this.WallIdAttachedTo = wallIdAttachedTo;
        }
    }
}
