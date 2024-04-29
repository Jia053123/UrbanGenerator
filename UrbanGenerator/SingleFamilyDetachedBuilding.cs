using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanGenerator
{
    class SingleFamilyDetachedBuilding : Building
    {
        public SingleFamilyDetachedBuilding(ParsedModel modelParser, PointF centroidLocation) : base(modelParser, centroidLocation) 
        {

        }
    }
}
