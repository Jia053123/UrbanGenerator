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
        public List<Wall> Walls { get; }

        public Building(ModelParser modelParser)
        {
            this.Parser = modelParser;
            this.Walls = this.Parser.Walls.OrderBy(wall => wall.Azimuth).ToList();

            InitializeWalls();
        }

        private void InitializeWalls()
        {
            // Step1: create end points
            PointF currentPoint = new PointF(0, 0);
            foreach (var wall in this.Walls)
            {
                wall.RelEndPoint1 = currentPoint;
                float azimuthRad = (float)(wall.Azimuth * (Math.PI / 180));
                currentPoint.X += (float)(Math.Cos(azimuthRad) * wall.Length);
                currentPoint.Y += (float)(Math.Sin(azimuthRad) * wall.Length);
                wall.RelEndPoint2 = currentPoint;
            }

            Walls.Last().RelEndPoint2 = Walls.First().RelEndPoint1;

            // Step2: extrude surfaces
            foreach (var wall in this.Walls)
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
    }
}

