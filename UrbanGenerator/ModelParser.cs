using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UrbanGenerator
{
    class ModelParser
    {
        private XmlDocument model;
        //private string nameSpaceName;
        private XmlNamespaceManager nsManager;

        //public int YearBuilt
        //{
        //    get
        //    {
        //        XmlNode node = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:BuildingSummary/ns0:BuildingConstruction/ns0:YearBuilt", this.nsManager);
        //        int.TryParse(node.InnerText, out int yearBuilt);
        //        return yearBuilt;
        //    }
        //}

        //public string ResidentialFacilityType
        //{
        //    get
        //    {
        //        XmlNode node = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:BuildingSummary/ns0:BuildingConstruction/ns0:ResidentialFacilityType", this.nsManager);
        //        return node.InnerText;
        //    }
        //}

        public int NumOfConditionedFloorsAboveGrade
        {
            get
            {
                XmlNode node = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:BuildingSummary/ns0:BuildingConstruction/ns0:NumberofConditionedFloorsAboveGrade", this.nsManager);
                float.TryParse(node.InnerText, out float numOfFloors);
                return (int)numOfFloors;
            }
        }

        public float AverageCeilingHeight
        {
            get
            {
                XmlNode node = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:BuildingSummary/ns0:BuildingConstruction/ns0:AverageCeilingHeight", this.nsManager);
                float.TryParse(node.InnerText, out float averageCeilingHeight);
                return averageCeilingHeight;
            }
        }

        public List<Wall> Walls;
        public List<Roof> Roofs;

        private XmlNodeList WallsNodes
        {
            get
            {
                XmlNode walls = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:Enclosure/ns0:Walls", this.nsManager);
                return walls.ChildNodes;
            }
        }

        private XmlNodeList RoofsNodes
        {
            get
            {
                XmlNode roofs = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:Enclosure/ns0:Roofs", this.nsManager);
                if (roofs is null)
                {
                    return null;
                }
                return roofs.ChildNodes;
            }
        }

        private XmlNodeList SlabNodes
        {
            get
            {
                XmlNode slabs = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:Enclosure/ns0:Slabs", this.nsManager);
                if (slabs is null)
                {
                    return null;
                }
                return slabs.ChildNodes;
            }
        }

        //private XmlNodeList FloorsThickness
        //{
        //    get
        //    {
        //        XmlNode floors = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:Enclosure/ns0:Floors", this.nsManager);
        //        return floors.ChildNodes;
        //    }
        //}

        public ModelParser(string pathToModel)
        {
            this.model = new XmlDocument();
            this.model.Load(pathToModel);
            this.nsManager = new XmlNamespaceManager(model.NameTable);
            nsManager.AddNamespace("ns0", this.model.DocumentElement.GetNamespaceOfPrefix("ns0"));

            this.InitializeWalls();
            if(!(this.RoofsNodes is null))
            {
                this.InitializeRoofs();
            }
        }

        private void InitializeWalls()
        {
            this.Walls = new List<Wall>();

            foreach (XmlNode wallNode in this.WallsNodes)
            {
                int.TryParse(wallNode.SelectSingleNode("ns0:Azimuth", this.nsManager).InnerText, out int azimuth);
                float.TryParse(wallNode.SelectSingleNode("ns0:Area", this.nsManager).InnerText, out float area);
                var exteriorAdj = wallNode.SelectSingleNode("ns0:ExteriorAdjacentTo", this.nsManager).InnerText;
                var interiorAdj = wallNode.SelectSingleNode("ns0:InteriorAdjacentTo", this.nsManager).InnerText;

                var newWall = new Wall(this.AverageCeilingHeight, this.NumOfConditionedFloorsAboveGrade, azimuth, area, exteriorAdj, interiorAdj);
                this.Walls.Add(newWall);
            }
        }
        private void InitializeRoofs()
        {
            this.Roofs = new List<Roof>();

            foreach (XmlNode roofNode in this.RoofsNodes)
            {
                int.TryParse(roofNode.SelectSingleNode("ns0:Azimuth", this.nsManager).InnerText, out int azimuth);
                float.TryParse(roofNode.SelectSingleNode("ns0:Area", this.nsManager).InnerText, out float area);
                float.TryParse(roofNode.SelectSingleNode("ns0:Pitch", this.nsManager).InnerText, out float pitch);
                var newRoof = new Roof(azimuth, area, pitch);
                this.Roofs.Add(newRoof);
            }
        }
    }
}
