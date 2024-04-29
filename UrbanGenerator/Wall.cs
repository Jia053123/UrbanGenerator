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
        public string ID { get; }
        public float AverageCeilingHeight { get; }
        public int NumOfConditionedFloorsAboveGrade { get; }
        public int Azimuth { get; }
        public float Area { get; }
        public string ExteriorAdjacentTo { get; }
        public string InteriorAdjacentTo { get; }

        public float Height => this.AverageCeilingHeight * this.NumOfConditionedFloorsAboveGrade; // + floor thickness! 
        public float Length => this.Area / this.Height;

        public PointF RelativeEndPoint1;
        public PointF RelativeEndPoint2;

        public LineCurve GroundLine;
        public PlaneSurface WallSurface;

        public Wall(string id, float averageCeilingHeight, int numOfConditionedFloorsAboveGrade, int azimuth, float area, string exteriorAdjacentTo, string interiorAdjacentTo)
        {
            this.ID = id;
            this.AverageCeilingHeight = averageCeilingHeight;
            this.NumOfConditionedFloorsAboveGrade = numOfConditionedFloorsAboveGrade;
            this.Azimuth = azimuth;
            this.Area = area;
            this.ExteriorAdjacentTo = exteriorAdjacentTo;
            this.InteriorAdjacentTo = interiorAdjacentTo;
        }
    }
}
