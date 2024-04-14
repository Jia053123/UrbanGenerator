using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace UrbanGenerator
{
    public class UrbanGeneratorInfo : GH_AssemblyInfo
    {
        public override string Name => "UrbanGenerator";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("5FD375D2-A845-4070-AA7B-9EA483C97B8A");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}