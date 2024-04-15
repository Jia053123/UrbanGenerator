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
        public ModelParser(string pathToModel)
        {
            this.model = new XmlDocument();
            this.model.Load(pathToModel);
            this.nsManager = new XmlNamespaceManager(model.NameTable);
            nsManager.AddNamespace("ns0", this.model.DocumentElement.GetNamespaceOfPrefix("ns0"));
        }

        public int YearBuilt
        {
            get
            {
                XmlNode node = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:BuildingSummary/ns0:BuildingConstruction/ns0:YearBuilt", this.nsManager);
                int.TryParse(node.InnerText, out int yearBuilt);
                return yearBuilt;
            }
        }

        public string ResidentialFacilityType
        {
            get
            {
                XmlNode node = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:BuildingSummary/ns0:BuildingConstruction/ns0:ResidentialFacilityType", this.nsManager);
                return node.InnerText;
            }
        }

        public int NumOfConditionedFloorsAboveGrade
        {
            get
            {
                XmlNode node = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:BuildingSummary/ns0:BuildingConstruction/ns0:NumberofConditionedFloorsAboveGrade", this.nsManager);
                int.TryParse(node.InnerText, out int numOfFloors);
                return numOfFloors;
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

        public XmlNodeList Walls
        {
            get
            {
                XmlNode walls = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:Enclosure/ns0:Walls", this.nsManager);
                return walls.ChildNodes;
            }
        }

        public XmlNodeList Floors
        {
            get
            {
                XmlNode floors = this.model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:Enclosure/ns0:Floors", this.nsManager);
                return floors.ChildNodes;
            }
        }
    }
}
