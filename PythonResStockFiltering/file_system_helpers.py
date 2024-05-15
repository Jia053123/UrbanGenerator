import os

def get_child_directories(parent_directory):
    child_dirs = []
    for item in os.listdir(parent_directory):
        path_to_item = os.path.join(parent_directory, item)
        if os.path.isdir(path_to_item): 
            child_dirs.append(path_to_item)
    return child_dirs

def get_child_paths_with_extension(extension, parent_directories):
    child_paths = []
    for parent_directory in parent_directories: 
        for item in os.listdir(parent_directory): 
            path_to_item = os.path.join(parent_directory, item)
            if os.path.isfile(path_to_item) and item.endswith(extension):
                child_paths.append(path_to_item)
    return child_paths

def get_child_paths_with_name(name, parent_directories):
    child_paths = []
    for parent_directory in parent_directories: 
        for item in os.listdir(parent_directory): 
            path_to_item = os.path.join(parent_directory, item)
            if item == name:
                child_paths.append(path_to_item)
    return child_paths
