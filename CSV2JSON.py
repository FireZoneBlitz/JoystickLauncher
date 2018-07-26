import csv
import json

csvfile = open('Roms.csv', 'r')
jsonfile = open('Roms.json', 'w')

fieldnames = ("Rom Name", "Name", "System", "Display", "Orientation", "Joystick","Buttons","Button Count","Screenshot","Marquee")

reader = csv.DictReader( csvfile, fieldnames)
rowcounter = 0

for row in reader:
    if rowcounter == 0:
        jsonfile.write('[')
        rowcounter = rowcounter + 1
    elif rowcounter == 1:
        #first real data line, NO COMMA NEEDED
        
        json.dump(row, jsonfile)
        jsonfile.write('\n')
        rowcounter = rowcounter + 1

    else:
        #not the first real data line, add a comma
        jsonfile.write(',')
        json.dump(row, jsonfile)
        jsonfile.write('\n')
        rowcounter = rowcounter + 1
#end file
jsonfile.write(']')