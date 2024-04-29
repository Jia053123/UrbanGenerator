using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanGenerator
{
    class Filter
    {
        private string FilterText;
        
        public string Property;
        public string Operation;
        public string Value;

        public Filter (string filterText)
        {
            this.FilterText = filterText;

            // Parse the string
            // Step1: break down by semicolon 

        }
    }
}
