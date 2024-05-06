using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace UrbanGenerator
{
    public class UrbanGeneratorComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public UrbanGeneratorComponent()
          : base("UrbanGenerator", "UG",
            "Regional design simulation with ResStock",
            "ResStock", "")
        {
        }

        private float DisplayGridDistance => 135.0f;
        private int DisplayGridSizeX => 15;
        //private int DisplayGridSizeY => 3;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ModelDirectory", "Dir", "folder containing the ResStock models", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Footprint", "F", "Building Footprint", GH_ParamAccess.list);
            pManager.AddBrepParameter("FirstFloors", "FF", "First Floors Visual", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Walls", "Wa", "Building Walls", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Windows", "Win", "Building Windows", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Overhangs", "Over", "Window Overhangs", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Roofs", "R", "Building Roofs", GH_ParamAccess.list);
            pManager.AddTextParameter("debug", "d", "debug message", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string modelsDir = null;
            if (!DA.GetData(0, ref modelsDir)) return;

            var modelDirs = Directory.GetDirectories(modelsDir);
            var pathsToModels = modelDirs.Select(md => Directory.GetFiles(md, "*.xml")[0]);

            var listOfLines = new List<LineCurve>();
            var listOfFirstFloors = new List<Brep>();
            var listOfWalls = new List<PlaneSurface>();
            var listOfWindows = new List<PlaneSurface>();
            var listOfOverhangs = new List<PlaneSurface>();
            var listOfRoofs = new List<PlaneSurface>();

            int x_loc = 0;
            int y_loc = 0;

            foreach (string pathToModel in pathsToModels)
            {

                ParsedModel modelParser = new ParsedModel(pathToModel);
                var building = new Building(modelParser, new PointF(x_loc * this.DisplayGridDistance, y_loc * this.DisplayGridDistance));

                listOfLines.AddRange(building.MajorWalls.Select(w => w.GroundLine).ToList());
                listOfFirstFloors.Add(building.FirstFloorSurface);
                listOfWalls.AddRange(building.MajorWalls.Select(w => w.WallSurface).ToList());
                listOfWindows.AddRange(building.Windows.Select(w => w.WindowSurface).ToList());
                listOfOverhangs.AddRange(building.Windows.Select(w => w.Overhang.OverhangSurface).ToList());
                listOfRoofs.AddRange(building.Roofs.Select(r => r.RoofSurface).ToList());

                x_loc += 1;
                if (x_loc > (this.DisplayGridSizeX-1))
                {
                    x_loc = 0;
                    y_loc += 1;
                }
            }

            DA.SetDataList(0, listOfLines);
            DA.SetDataList(1, listOfFirstFloors);
            DA.SetDataList(2, listOfWalls);
            DA.SetDataList(3, listOfWindows);
            DA.SetDataList(4, listOfOverhangs);
            DA.SetDataList(5, listOfRoofs);
            DA.SetData(6, listOfLines.Count.ToString());
        }

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("64E057DF-2928-4330-84A5-E015A9C5716F");
    }
}