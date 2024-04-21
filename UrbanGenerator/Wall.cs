using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Rhino.Geometry;

namespace UrbanGenerator
{
    class Wall
    {
        public float AverageCeilingHeight { get; }
        public int NumOfConditionedFloorsAboveGrade { get; }
        public int Azimuth { get; }
        public float Area { get; }
        public string ExteriorAdjacentTo { get; }
        public string InteriorAdjacentTo { get; }

        public float Height => this.AverageCeilingHeight * this.NumOfConditionedFloorsAboveGrade;
        public float Length => this.Area / this.Height;

        public PointF RelEndPoint1;
        public PointF RelEndPoint2;

        public LineCurve GroundLine;
        public PlaneSurface WallSurface;

        public Wall(float averageCeilingHeight, int numOfConditionedFloorsAboveGrade, int azimuth, float area, string exteriorAdjacentTo, string interiorAdjacentTo)
        {
            this.AverageCeilingHeight = averageCeilingHeight;
            this.NumOfConditionedFloorsAboveGrade = numOfConditionedFloorsAboveGrade;
            this.Azimuth = azimuth;
            this.Area = area;
            this.ExteriorAdjacentTo = exteriorAdjacentTo;
            this.InteriorAdjacentTo = interiorAdjacentTo;
        }
    }
}
