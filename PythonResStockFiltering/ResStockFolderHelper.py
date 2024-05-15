import os
import shutil
import xml.etree.ElementTree as ET
from tqdm.notebook import tqdm
from file_system_helpers import get_child_directories


def copy_models_with_id(building_ids, energy_models_directory, destination_directory): 
    child_directories = get_child_directories(energy_models_directory)
    for child_dir in tqdm(child_directories):
        modelFolderName = os.path.basename(child_dir)
        # print(modelFolderName)
        for id in building_ids:
            formatted_building_id = str(id).zfill(7) # Format with leading zeros
            # print(id)
            if modelFolderName.startswith(f"bldg{formatted_building_id}"):
                copy_destination = os.path.join(destination_directory, modelFolderName)
                print(child_dir)
                print(copy_destination)
                shutil.copytree(child_dir, copy_destination)
                break; 