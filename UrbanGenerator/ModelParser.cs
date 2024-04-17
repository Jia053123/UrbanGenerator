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

        private XmlNodeList WallsNodes
        {
            get
            {
                XmlNode walls = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:Enclosure/ns0:Walls", this.nsManager);
                return walls.ChildNodes;
            }
        }

        //private XmlNodeList Floors
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
        }

        private void InitializeWalls()
        {
            this.Walls = new List<Wall>();

            foreach (XmlNode wallNode in this.WallsNodes)
            {
                int.TryParse(wallNode.SelectSingleNode("ns0:Azimuth", this.nsManager).InnerText, out int azimuth);
                float.TryParse(wallNode.SelectSingleNode("ns0:Area", this.nsManager).InnerText, out float area);
                var newWall = new Wall(this.AverageCeilingHeight, this.NumOfConditionedFloorsAboveGrade, azimuth, area);
                this.Walls.Add(newWall);
            }
        }
    }
}
