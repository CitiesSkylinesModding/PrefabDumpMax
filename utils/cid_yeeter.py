import os
import shutil
target_dir = os.path.join(os.getcwd(), 'utils', 'cid')
if not os.path.exists(target_dir):
    os.makedirs(target_dir)

def get_unique_path(path):
    """
    If the file already exists, add a suffix to create a unique file path.
    """
    base, extension = os.path.splitext(path)
    counter = 1
    new_path = f"{base}_{counter}{extension}"
    while os.path.exists(new_path):
        counter += 1
        new_path = f"{base}_{counter}{extension}"
    return new_path

for root, dirs, files in os.walk(os.path.join(os.getcwd(), 'Dumps', 'current')):
    for file in files:
        if file.endswith('.cid'):
            file_path = os.path.join(root, file)
            dest_path = os.path.join(target_dir, file)
            if os.path.exists(dest_path):
                dest_path = get_unique_path(dest_path)
            shutil.move(file_path, dest_path)

print("All .cid files have been moved to the 'cid' folder.")
