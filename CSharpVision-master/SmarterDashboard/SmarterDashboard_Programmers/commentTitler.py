#! /usr/bin/env python
import sys

def commentsTitler(filename):
    '''WARNING: This will overwrite the file content, make sure you have a backup'''
    # Get The Appropriate Comment Prefix For The File Type
    ext = {'py' : '#', 'cs' : '//', 'cpp' : '//', 'h' : '//', 'asm' : ';', 'gcode' : ';', 'java' : '//', 'js' : '//'}
    COMMENTS_PREFIX = ext[filename[filename.find('.') + 1:].lower()]
    # Get All The Lines In The File
    with open(filename, 'r') as file:
        data = file.readlines()
    # Iterate Over All The Lines In The File
    for line in xrange(len(data)):
        # Find The Comment'S Index
        comment = data[line].find(COMMENTS_PREFIX)
        # If Not Found, Go Over To The Next Line
        if comment != -1:
          # If The Comment Comes After A Line Of Code (U-G-L-Y), Only Title The Comment To Avoid Messing Up With The Code
            if comment != 0:
                data[line] = data[line][:comment] + data[line][comment:].title()
          # Else, Just Title The Entire Line
            else:
                data[line] = data[line].title()
    # Finally, Overwrite The File With The Modified Content
    with open(filename, 'w') as file:
        file.writelines(data)
            
if __name__ == '__main__':
    if len(sys.argv) != 2:
        print('Usage: Python Title_Comments.Py <File With Comments>')
        sys.exit(1)
    commentsTitler(str(sys.argv[1]))