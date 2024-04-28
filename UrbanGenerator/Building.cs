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

        public PointF CentroidLocation 
        {
            get => this._centriodLocation;
            set 
            {
                var currentCentroid = this.FindWallsCentroid();
                var translation = new Vector3d(value.X - currentCentroid.X, value.Y - currentCentroid.Y, 0);

                foreach (var wall in this.MajorWalls)
                {
                    wall.GroundLine.Translate(translation);
                    wall.WallSurface.Translate(translation);
                }
                foreach (var roof in this.Roofs)
                {
                    if (roof.RoofSurface is null) continue;
                    roof.RoofSurface.Translate(translation);
                }

                this._centriodLocation = value;
            } 
        }

        private PointF _centriodLocation = PointF.Empty;

        /// <summary>
        /// All walls sorted by azimuth, in order of north, east, south, west
        /// </summary>
        public List<Wall> MajorWalls { get; }

        /// <summary>
        /// All roofs sorted by area, large to small
        /// </summary>
        public List<Roof> Roofs { get; }

        public Building(ModelParser modelParser, PointF centroidLocation)
        {
            this.Parser = modelParser;
            var majorWalls = this.Parser.Walls
                .FindAll(wall => wall.ExteriorAdjacentTo == "outside" || wall.ExteriorAdjacentTo == "other housing unit")
                .FindAll(wall => wall.InteriorAdjacentTo == "living space" || wall.InteriorAdjacentTo == "garage");
            this.MajorWalls = majorWalls.OrderBy(wall => wall.Azimuth).ToList(); // Sort by orientation, north, east, south, west

            InitializeWalls();

            if (!(this.Parser.Roofs is null))
            {
                this.Roofs = this.Parser.Roofs.OrderByDescending(roof => roof.Area).ToList(); // Sort by size large to small
                InitializeRoofs();
            } 
            else
            {
                this.Roofs = new List<Roof>();
            }

            this.CentroidLocation = centroidLocation;
        }

        private void InitializeWalls()
        {
            // Step1: create end points
            PointF currentPoint = new PointF(0, 0);
            foreach (var wall in this.MajorWalls)
            {
                wall.RelativeEndPoint1 = currentPoint;
                float azimuthRad = (float)(wall.Azimuth * (Math.PI / 180));
                currentPoint.X += (float)(Math.Cos(azimuthRad) * wall.Length);
                currentPoint.Y += (float)(Math.Sin(azimuthRad) * wall.Length);
                wall.RelativeEndPoint2 = currentPoint;
            }

            //Walls.Last().RelEndPoint2 = Walls.First().RelEndPoint1;

            // Step2: extrude surfaces
            foreach (var wall in this.MajorWalls)
            {
                var pointFrom = new Point3d(wall.RelativeEndPoint1.X, wall.RelativeEndPoint1.Y, 0);
                var pointTo = new Point3d(wall.RelativeEndPoint2.X, wall.RelativeEndPoint2.Y, 0);
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
                foreach (var roof in this.Roofs)
                {
                    var length = this.MajorWalls.Where(wall => wall.Azimuth == roof.Azimuth).Select(wall => wall.Length).Sum();
                    var width = roof.Area / length;

                    var plane = Plane.WorldXY;
                    float azimuthRad = (float)(roof.Azimuth * (Math.PI / 180));
                    float pitchRad = (float)Math.Atan2(6, 12);
                    plane.Rotate(-1 * pitchRad, Vector3d.XAxis);
                    plane.Rotate(azimuthRad, Vector3d.ZAxis);
                    var cetroid2d = this.FindWallsCentroid();
                    var roofHeight = this.MajorWalls.First().Height + Math.Sin(pitchRad) * width;
                    plane.Translate(new Vector3d(cetroid2d.X, cetroid2d.Y, roofHeight));
                    var xExtents = new Interval(0, width);
                    var yExtents = new Interval(length/2*-1, length/2);
                    var newSurface = new PlaneSurface(plane, yExtents, xExtents);
                    roof.RoofSurface = newSurface;
                }
            }
        }

        private PointF FindWallsCentroid()
        {
            var endPoint1s = this.MajorWalls.Select(wall => wall.RelativeEndPoint1).ToList();
            return GetCentroid(endPoint1s);
        }

        /// <summary>
        /// Method to compute the centroid of a polygon. This does NOT work for a complex polygon.
        /// https://coding-experiments.blogspot.com/2009/09/xna-quest-for-centroid-of-polygon.html
        /// https://stackoverflow.com/questions/9815699/how-to-calculate-centroid
        /// </summary>
        /// <param name="poly">points that define the polygon</param>
        /// <returns>centroid point, or PointF.Empty if something wrong</returns>
        private PointF GetCentroid(List<PointF> poly)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                accumulatedArea += temp;
                centerX += (poly[i].X + poly[j].X) * temp;
                centerY += (poly[i].Y + poly[j].Y) * temp;
            }

            if (Math.Abs(accumulatedArea) < 1E-7f)
                return PointF.Empty;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new PointF(centerX / accumulatedArea, centerY / accumulatedArea);
        }
    }
}

