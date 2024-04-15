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
            nsManager.AddNamespace("ns0", model.DocumentElement.GetNamespaceOfPrefix("ns0"));
        }

        public XmlNodeList Walls
        {
            get
            {
                XmlNode walls = model.DocumentElement.SelectSingleNode("/ns0:HPXML/ns0:Building/ns0:BuildingDetails/ns0:Enclosure/ns0:Walls", this.nsManager);
                return walls.ChildNodes;
            }
        }
    }
}
