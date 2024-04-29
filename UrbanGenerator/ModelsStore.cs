using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanGenerator
{
    class ModelsStore
    {
        private List<ParsedModel> AllModels;
        //public List<List<ParsedModel>> ModelsToDisplay;

        public ModelsStore(List<string> pathsToModels)
        {
            this.AllModels = pathsToModels.Select(ptm => new ParsedModel(ptm)).ToList();
        }

        //public List<ParsedModel> GetModelsSatisfyingFilter(Filter filter)
        //{

        //}
    }
}
