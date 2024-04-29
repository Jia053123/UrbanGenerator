using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Rhino.Geometry;

namespace UrbanGenerator
{
    class WindowOverhang
    {
        public float Depth { get; }
        public float DistanceToTopOfWindow { get; }
        public float DistanceToBottomOfWindow { get; }

        public PlaneSurface OverhangSurface;

        public WindowOverhang(float depth, float distanceToTopOfWindow, float distanceToBottomOfWindow)
        {
            this.Depth = depth;
            this.DistanceToTopOfWindow = distanceToTopOfWindow;
            this.DistanceToBottomOfWindow = distanceToBottomOfWindow;
        }
    }
}
