using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq; 
using System.Drawing;
using Rhino.Geometry;

namespace UrbanGenerator
{
    class Building
    {
        public ModelParser Parser { get; }

        /// <summary>
        /// All walls sorted by azimuth
        /// </summary>
        public List<Wall> MajorWalls { get; }
        public List<Roof> Roofs { get; }

        public Building(ModelParser modelParser)
        {
            this.Parser = modelParser;
            var majorWalls = this.Parser.Walls
                .FindAll(wall => wall.ExteriorAdjacentTo == "outside" || wall.ExteriorAdjacentTo == "other housing unit")
                .FindAll(wall => wall.InteriorAdjacentTo == "living space" || wall.InteriorAdjacentTo == "garage");
            this.MajorWalls = majorWalls.OrderBy(wall => wall.Azimuth).ToList();

            InitializeWalls();

            if (!(this.Parser.Roofs is null))
            {
                this.Roofs = this.Parser.Roofs.OrderBy(roof => roof.Azimuth).ToList();
                InitializeRoofs();
            }
        }

        private void InitializeWalls()
        {
            // Step1: create end points
            PointF currentPoint = new PointF(0, 0);
            foreach (var wall in this.MajorWalls)
            {
                wall.RelEndPoint1 = currentPoint;
                float azimuthRad = (float)(wall.Azimuth * (Math.PI / 180));
                currentPoint.X += (float)(Math.Cos(azimuthRad) * wall.Length);
                currentPoint.Y += (float)(Math.Sin(azimuthRad) * wall.Length);
                wall.RelEndPoint2 = currentPoint;
            }

            //Walls.Last().RelEndPoint2 = Walls.First().RelEndPoint1;

            // Step2: extrude surfaces
            foreach (var wall in this.MajorWalls)
            {
                var pointFrom = new Point3d(wall.RelEndPoint1.X, wall.RelEndPoint1.Y, 0);
                var pointTo = new Point3d(wall.RelEndPoint2.X, wall.RelEndPoint2.Y, 0);
                wall.GroundLine = new LineCurve(pointFrom, pointTo);

                var plane = Plane.WorldZX;
                float azimuthRad = (float)(wall.Azimuth * (Math.PI / 180));
                plane.Rotate(azimuthRad, Vector3d.ZAxis);
                var xExtents = new Interval(0, wall.GroundLine.GetLength());
                var yExtents = new Interval(0, wall.Height);
                var newSurface = new PlaneSurface(plane, yExtents, xExtents);
                newSurface.Translate(new Vector3d(wall.GroundLine.PointAtStart));
                wall.WallSurface = newSurface;
            }
        }

        private void InitializeRoofs()
        {
            if (this.Roofs.Count == 2)
            {

            }
        }
    }
}

