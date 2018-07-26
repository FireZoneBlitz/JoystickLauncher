import json


romfile=open('Roms.json')

romlist = json.load(romfile)

for rom in romlist:
    print rom["Name"]

x = 0 #debug holder
