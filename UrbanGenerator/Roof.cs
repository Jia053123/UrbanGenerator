using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace UrbanGenerator
{
    class Roof
    {
        public float Area { get; }
        public int Azimuth { get; }
        public float Pitch { get; }

        public PlaneSurface RoofSurface;

        public Roof(int azimuth, float area, float pitch)
        {
            this.Azimuth = azimuth;
            this.Area = area;
            this.Pitch = pitch;
        }
    }
}
