import os

def merge_files():
    # File types to include
    exts = {'.cs', '.bat', '.txt'}
    output_file = "full_project_code.txt"
    
    with open(output_file, 'w', encoding='utf-8') as outfile:
        # Walk through all folders
        for root, dirs, files in os.walk("."):
            # Skip hidden folders or build artifacts like 'bin' and 'obj'
            dirs[:] = [d for d in dirs if d not in {'.git', '.vs', 'bin', 'obj'}]
            
            for file in files:
                if os.path.splitext(file)[1].lower() in exts and file != "pack.py":
                    path = os.path.join(root, file)
                    try:
                        with open(path, 'r', encoding='utf-8') as infile:
                            outfile.write(f"\n{'='*50}\nFILE: {path}\n{'='*50}\n\n")
                            outfile.write(infile.read())
                            outfile.write("\n")
                    except Exception as e:
                        print(f"Skipped {file}: {e}")
    print(f"Done! Upload '{output_file}' here.")

if __name__ == "__main__":
    merge_files()