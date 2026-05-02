import os
import re

def remove_comments_from_file(filepath):
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            text = f.read()
    except Exception as e:
        print(f"Error reading {filepath}: {e}")
        return

    # Pattern matches string literals, multi-line comments, and single-line comments.
    pattern = re.compile(
        r'(?P<string>'
        r'@"(?:[^"]|"")*"|'  # verbatim strings
        r'\$@"(?:[^"]|"")*"|' # interpolated verbatim strings
        r'@\$"(?:[^"]|"")*"|' # interpolated verbatim strings
        r'"(?:[^"\\]|\\.)*"'  # regular strings
        r')|(?P<mcomment>/\*.*?\*/)|(?P<scomment>//[^\n]*)',
        re.DOTALL
    )

    def replacer(match):
        if match.group('string') is not None:
            return match.group('string')
        elif match.group('mcomment') is not None:
            return ''
        elif match.group('scomment') is not None:
            return ''
        return match.group(0)

    new_text = pattern.sub(replacer, text)

    # Remove trailing spaces on lines if any, and clean empty lines that were created by stripping the comment
    lines = new_text.split('\n')
    cleaned_lines = []
    
    # Read original lines to see if a line was originally empty vs made empty
    orig_lines = text.split('\n')
    
    for i in range(len(lines)):
        # If the stripped line is universally empty but original wasn't, we can skip it 
        # Actually it's safer just to write new_text as is to perfectly preserve structure
        pass

    if new_text != text:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_text)

def process_dir(directory):
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith('.cs'):
                remove_comments_from_file(os.path.join(root, file))

if __name__ == '__main__':
    import sys
    if len(sys.argv) > 1:
        target_dir = sys.argv[1]
    else:
        target_dir = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'KingdomEnhanced')
    process_dir(target_dir)
