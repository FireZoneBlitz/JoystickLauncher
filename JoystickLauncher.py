import json
from Tkinter import *


###########  Global Settings For Your Arcade System


BackgroundColor = "black"
FontName = "Arcade Normal"
FontColor = "white"
SystemFontSize = 20
ButtonFontSize = 13
GameFontSize = 25
TopMargin = 30
MarqueeWidth = 500
ScreenshotWidth = 450
ScreenshowHeight = 550
VerticalSpacing = 0
# total buttons per user on console
ButtonCount = 2
ButtonOrder = "red,red"

###########  End Global Settings

###########  Initialize Variables
global RomIndex
RomIndex = 0
global romlist
global CurrentRom
romfile=open('Roms.json')
# data structure to hold list of roms, decoded from json file
romlist = json.load(romfile)
# start at first rom in the list
CurrentRom = romlist[RomIndex]

# now that data is loaded, let's work on the display
root = Tk()
# background color
root["bg"] = BackgroundColor
# fullscreen
root.attributes('-fullscreen', True)
# hide mouse
root.config(cursor="none")

###########  Functions


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
    # where the magic happens

    ScreenHeight = int(root.winfo_screenheight())
    ScreenWidth = int(root.winfo_screenwidth())

    ### Rom Set label (arcade, nes, etc)
    
    RomSetLabel = Label(root,bg=BackgroundColor,fg=FontColor,text=CurrentRom["System"],font=(FontName,SystemFontSize))
    RomSetLabel.pack()

    xloc = int(ScreenWidth) / 2 - int(RomSetLabel.winfo_reqwidth() / 2)
    yloc = TopMargin

    RomSetLabel.place(x=xloc,y=yloc)

    
    



def UpdateRoms():
    x = 0 ## placeholder


###########  Main Program Area



UpdateScreen()


root.mainloop()


