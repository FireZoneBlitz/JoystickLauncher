import json
from Tkinter import *

# rom list i/o file
romfile=open('Roms.json')

# data structure to hold list of roms, decoded from json file
romlist = json.load(romfile)

# start at first rom in the list
RomIndex = 0

# now that data is loaded, let's work on the display
root = Tk()
# background black
root["bg"] = "black"
# fullscreen
root.attributes('-fullscreen', True)
# hide mouse
root.config(cursor="none")


root.mainloop()


def PreviousRom():
    RomIndex -= 1
    if RomIndex == 0:
        RomIndex = len(romlist) - 1
    UpdateScreen()

def NextRom():
    RomIndex += 1
    if RomIndex == len(romlist):
        RomIndex = 0
    UpdateScreen()


def UpdateScreen():
    x = 0 #holder