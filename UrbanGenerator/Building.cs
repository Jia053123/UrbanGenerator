﻿using System;
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
        public ParsedModel Parser { get; }

        public float ConditionedFloorArea { get; }
        public int NumOfConditionedFloorsAboveGrade { get; }
        public float AverageCeilingHeight { get; }

        /// <summary>
        /// All walls sorted by azimuth, in order of north, east, south, west
        /// </summary>
        public List<Wall> MajorWalls { get; }
        public List<Window> Windows { get; }

        /// <summary>
        /// All roofs sorted by area, large to small
        /// </summary>
        public List<Roof> Roofs { get; }

        public Brep FirstFloorSurface { get; set; }

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

                foreach (var window in this.Windows)
                {
                    window.WindowSurface.Translate(translation);
                    if (!(window.Overhang is null))
                    {
                        window.Overhang.OverhangSurface.Translate(translation);
                    }
                }
                
                foreach (var roof in this.Roofs)
                {
                    if (roof.RoofSurface is null) continue;
                    roof.RoofSurface.Translate(translation);
                }

                this.FirstFloorSurface.Translate(translation);

                this._centriodLocation = value;
            } 
        }

        private PointF _centriodLocation = PointF.Empty;

        public Building(ParsedModel parsedModel, PointF centroidLocation)
        {
            this.Parser = parsedModel;
            this.ConditionedFloorArea = parsedModel.ConditionedFloorArea;
            this.NumOfConditionedFloorsAboveGrade = parsedModel.NumOfConditionedFloorsAboveGrade;
            this.AverageCeilingHeight = parsedModel.AverageCeilingHeight;

            var majorWalls = this.Parser.Walls
            //.FindAll(wall => wall.ExteriorAdjacentTo == "outside" || wall.ExteriorAdjacentTo == "other housing unit")
            .FindAll(wall => wall.InteriorAdjacentTo == "living space"); //|| wall.InteriorAdjacentTo == "garage");

            this.MajorWalls = majorWalls;//.OrderBy(wall => wall.Azimuth).ToList(); // Sort by orientation, north, east, south, west

            InitializeWalls();

            this.Windows = this.Parser.Windows;
            InitializeWindows();

            if (!(this.Parser.Roofs is null))
            {
                this.Roofs = this.Parser.Roofs.OrderByDescending(roof => roof.Area).ToList(); // Sort by size large to small
                InitializeRoofs();
            } 
            else
            {
                this.Roofs = new List<Roof>();
            }

            InitializeFirstFloor();

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

        private void InitializeWindows()
        {
            foreach(var window in this.Windows)
            {
                var wallsAttached = this.MajorWalls.Where(w => w.ID == window.WallIdAttachedTo);
                if(wallsAttached.Count() > 0)
                {
                    var wallAttached = wallsAttached.First();
                    wallAttached.WallSurface.TryGetPlane(out var windowPlane);
                    var wallAttachedBoundingBox = wallAttached.WallSurface.GetBoundingBox(windowPlane);
                    var diff = wallAttachedBoundingBox.PointAt(0.5, 0.5, 1) - wallAttachedBoundingBox.PointAt(0, 0, 1);
                    var windowCenteringTranslation = new Vector3d(diff.Z, diff.Y, diff.X);
                    float azimuthRad = (float)((window.Azimuth-90) * (Math.PI / 180));
                    windowCenteringTranslation.Rotate(azimuthRad, Vector3d.ZAxis);

                    float windowWidth;
                    float windowHeight;
                    var overhangOnRoofTranslation = new Vector3d(0, 0, 0);
                    if (!(window.Overhang is null))
                    {
                        windowHeight = window.Overhang.DistanceToBottomOfWindow - window.Overhang.DistanceToTopOfWindow;
                        windowWidth = window.Area / windowHeight;

                        var overhangPlane = windowPlane.Clone();
                        var overhangRotateAxis = Vector3d.YAxis;
                        overhangRotateAxis.Rotate(azimuthRad, Vector3d.ZAxis);
                        overhangPlane.Rotate(Math.PI / 2, overhangRotateAxis);
                        var overhangYExtent = new Interval(-1 * windowWidth / 2, windowWidth / 2);
                        var overhangXExtent = new Interval(0, window.Overhang.Depth);
                        var overhangSurface = new PlaneSurface(overhangPlane, overhangXExtent, overhangYExtent);
                        overhangSurface.Translate(windowCenteringTranslation);
                        var overhangZTranslation = new Vector3d(0,0, windowHeight / 2.0f + window.Overhang.DistanceToTopOfWindow);
                        overhangSurface.Translate(overhangZTranslation);
                        overhangOnRoofTranslation = new Vector3d(0, 0, wallAttached.Height - overhangSurface.GetBoundingBox(true).Center.Z);
                        overhangSurface.Translate(overhangOnRoofTranslation);
                        window.Overhang.OverhangSurface = overhangSurface;
                    }
                    else
                    {
                        windowHeight = (float)Math.Sqrt(window.Area);
                        windowWidth = windowHeight;
                    }
                    var yExtent = new Interval(-1 * windowWidth / 2, windowWidth / 2); 
                    var xExtent = new Interval(-1 * windowHeight / 2, windowHeight / 2);
                    var windowSurface = new PlaneSurface(windowPlane, xExtent, yExtent);
                    windowSurface.Translate(windowCenteringTranslation);
                    windowSurface.Translate(overhangOnRoofTranslation);
                    window.WindowSurface = windowSurface;
                }
            }
        }

        private void InitializeRoofs()
        {
            if (this.Roofs.Count > 4)
            {
                throw new Exception("roof count greater than 4");
            }
            foreach (var roof in this.Roofs)
            {
                float length;
                float width;
                float azimuthRad;
                float pitchRad;
                var plane = Plane.WorldXY;
                var wallsFacingSameDirection = this.MajorWalls.Where(wall => wall.Azimuth == roof.Azimuth);
                if (wallsFacingSameDirection.Count() == 0)
                {
                    wallsFacingSameDirection = this.MajorWalls.Where(wall => wall.Azimuth == this.MajorWalls.First().Azimuth);
                }
                length = wallsFacingSameDirection.Select(wall => wall.Length).Sum();
                width = roof.Area / length;
                if (roof.Pitch > 0)
                {
                    azimuthRad = (float)(roof.Azimuth * (Math.PI / 180));
                    pitchRad = (float)Math.Atan2(roof.Pitch, 12);
                }
                else
                {
                    // flat roof
                    azimuthRad = (float)(wallsFacingSameDirection.First().Azimuth * (Math.PI / 180));
                    pitchRad = 0.0f;
                }

                var xExtents = new Interval(0, width);
                var yExtents = new Interval(length / 2 * -1, length / 2);
                var newSurface = new PlaneSurface(plane, yExtents, xExtents);

                newSurface.Rotate(pitchRad, Vector3d.XAxis, Point3d.Origin);
                newSurface.Rotate(azimuthRad, Vector3d.ZAxis, Point3d.Origin);

                var firstPoint = wallsFacingSameDirection.First().RelativeEndPoint1;
                var secondPoint = wallsFacingSameDirection.Last().RelativeEndPoint2;
                var longestWall = wallsFacingSameDirection.OrderByDescending(w => w.Length).First();
                var firstEndPointForRoofLine = longestWall.GroundLine.Line.ClosestPoint(new Point3d(firstPoint.X, firstPoint.Y, 0), false);
                var secondEndPointForRoofLine = longestWall.GroundLine.Line.ClosestPoint(new Point3d(secondPoint.X, secondPoint.Y, 0), false);
                var roofLine = new LineCurve(firstEndPointForRoofLine, secondEndPointForRoofLine);
                var roofLineMiddlePoint = roofLine.GetBoundingBox(true).Center;

                var roofHeight = this.MajorWalls.First().Height; // + Math.Sin(pitchRad) * width;
                newSurface.Translate(new Vector3d(roofLineMiddlePoint.X, roofLineMiddlePoint.Y, roofHeight));

                roof.RoofSurface = newSurface;
            }
        }

        private void InitializeFirstFloor()
        {
            var endPoints = this.MajorWalls.Select(w => new Point3d(w.RelativeEndPoint1.X, w.RelativeEndPoint1.Y, 0)).ToList();
            endPoints.Add(new Point3d(this.MajorWalls.First().RelativeEndPoint1.X, this.MajorWalls.First().RelativeEndPoint1.Y, 0));
            var firstFloorCurve = new PolylineCurve(endPoints);
            this.FirstFloorSurface = Brep.CreatePlanarBreps(firstFloorCurve, 0.001).First();
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

