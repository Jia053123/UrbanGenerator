using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq; 
using System.Drawing;

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

            this.InitializeWalls();
        }

        private void InitializeWalls()
        {
            // Step1: create end points
            PointF currentPoint = new PointF(0, 0);
            foreach(var wall in this.Walls)
            {
                wall.RelEndPoint1 = currentPoint;
                float azimuthRad = (float)(wall.Azimuth * (Math.PI / 180));
                currentPoint.X += (float)(Math.Cos(azimuthRad) * wall.Length);
                currentPoint.Y += (float)(Math.Sin(azimuthRad) * wall.Length);
                wall.RelEndPoint2 = currentPoint;
            }

            Walls.Last().RelEndPoint2 = Walls.First().RelEndPoint1;

            // Step2: extrude surfaces

        }
    }
}

