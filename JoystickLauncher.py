import json
from Tkinter import *
import subprocess
from subprocess import call


###########  Global Settings For Your Arcade System


BackgroundColor = "black"
FontName = "Arcade Normal"
FontColor = "white"
SystemFontSize = 20
ButtonFontSize = 13
GameFontSize = 25
TopMargin = 120
MarqueeWidth = 500
ScreenshotWidth = 450
ScreenshowHeight = 550
VerticalSpacing = 100
# total buttons per user on console
ButtonCount = 2
ButtonOrder = "red,red"

###########  End Global Settings

###########  Initialize Variables
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

def upKey(event):
    PreviousRom()

def downKey(event):
    NextRom()

def controlKey(event):
    #os.system("aplay " + launchEffectFile)
    call(["mame", "pbobblen"])

def PreviousRom():
    global RomIndex
    global CurrentRom
    RomIndex -= 1
    if RomIndex == 0:
        RomIndex = len(romlist) - 1
    CurrentRom = romlist[RomIndex]
    UpdateScreen()

def NextRom():
    global RomIndex
    global CurrentRom
    RomIndex += 1
    if RomIndex == len(romlist):
        RomIndex = 0
    CurrentRom = romlist[RomIndex]
    UpdateScreen()


def UpdateScreen():
    # where the magic happens

    # clear the screen
    p = root.winfo_children()
 
    for wid in p:
        wid.destroy()

    ScreenHeight = int(root.winfo_screenheight())
    ScreenWidth = int(root.winfo_screenwidth())

    ### Rom Set label (arcade, nes, etc)
    


    RomSetLabel = Label(root,bg=BackgroundColor,fg=FontColor,text=CurrentRom["System"],font=(FontName,SystemFontSize))
    RomSetLabel.pack()
    

    xloc = int(ScreenWidth) / 2 - int(RomSetLabel.winfo_reqwidth() / 2)
    yloc = TopMargin

    RomSetLabel.place(x=xloc,y=yloc)


    ### Rom Name label
    
    RomLabel = Label(root,bg=BackgroundColor,fg=FontColor,text=CurrentRom["Name"],font=(FontName,GameFontSize))
    RomLabel.pack()

    xloc = int(ScreenWidth) / 2 - int(RomLabel.winfo_reqwidth() / 2)
    yloc = yloc + int(RomSetLabel.winfo_reqheight()) + VerticalSpacing

    RomLabel.place(x=xloc,y=yloc)


    ### Control Panel (show joystick and used buttons)
    ControlPanelLabel = Label(root,bg=BackgroundColor,fg=FontColor,text="Control Panel",font=(FontName,GameFontSize))
    ControlPanelLabel.pack()

    xloc = int(ScreenWidth) / 2 - int(ControlPanelLabel.winfo_reqwidth() / 2)
    yloc = yloc + int(RomLabel.winfo_reqheight()) + VerticalSpacing

    ControlPanelLabel.place(x=xloc,y=yloc)

    ### Screenshot
    ScreenshotLabel = Label(root,bg=BackgroundColor,fg=FontColor,text="Screenshot",font=(FontName,GameFontSize))
    ScreenshotLabel.pack()

    xloc = int(ScreenWidth) / 2 - int(ScreenshotLabel.winfo_reqwidth() / 2)
    yloc = yloc + int(ControlPanelLabel.winfo_reqheight()) + VerticalSpacing

    ScreenshotLabel.place(x=xloc,y=yloc)




def UpdateRoms():
    x = 0 ## placeholder


###########  Main Program Area

#lock handlers to window
root.bind("<Up>", upKey)
root.bind("<Down>", downKey)
root.bind("<Control_L>", controlKey)

UpdateScreen()


root.mainloop()


