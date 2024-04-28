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

        private float DisplayGridDistance => 80.0f;
        private int DisplayGridSizeX => 2;
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
            pManager.AddSurfaceParameter("Walls", "W", "Building Walls", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Roofs", "R", "Building Roofs", GH_ParamAccess.list);
            pManager.AddTextParameter("debug", "d", "debug message", GH_ParamAccess.item);
            //pManager.AddBrepParameter("Models", "M", "Model Visualizations", GH_ParamAccess.list);
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
            var listOfLines = new List<LineCurve>();
            var listOfWalls = new List<PlaneSurface>();
            var listOfRoofs = new List<PlaneSurface>();

            int x_loc = 0;
            int y_loc = 0;
            foreach (string modelDir in modelDirs)
            {
                string pathToModel = Directory.GetFiles(modelDir, "*.xml")[0];

                //string modelDir = Directory.GetDirectories(modelsDir)[0];
                ModelParser modelParser = new ModelParser(pathToModel);
                var building = new Building(modelParser, new PointF(x_loc * this.DisplayGridDistance, y_loc * this.DisplayGridDistance));

                listOfLines.AddRange(building.MajorWalls.Select(w => w.GroundLine).ToList());
                listOfWalls.AddRange(building.MajorWalls.Select(w => w.WallSurface).ToList());
                listOfRoofs.AddRange(building.Roofs.Select(r => r.RoofSurface).ToList());

                x_loc += 1;
                if (x_loc > (this.DisplayGridSizeX-1))
                {
                    x_loc = 0;
                    y_loc += 1;
                }
            }

            DA.SetDataList(0, listOfLines);
            DA.SetDataList(1, listOfWalls);
            DA.SetDataList(2, listOfRoofs);
            DA.SetData(3, listOfLines.Count.ToString());
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