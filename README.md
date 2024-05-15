# UrbanGenerator
Urban Planning with ResStock Models

To compile code, look at the "Geting Started" section of https://developer.rhino3d.com/guides/rhinocommon/ 

To change the grid proportion, adjust private int DisplayGridSizeX parameter in UrbanGeneratorComponent.cs

To reshuffle the models, recompute the grasshopper script (F5) 

To filter, first do filtering with the metadata file and get the filtered building IDs with Excel, then use PythonResStockFiltering/Filters.ipynb to move the models corresponding to these IDs to desired folder for visualization. 