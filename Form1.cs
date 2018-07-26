using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;

namespace JoystickLauncher
{
    public partial class Form1 : Form
    {
        private JL JLLibrary;
        int EmulatorIndex = 0;
        int RomIndex = 0;
        int ScreensaverCounter = 0;
        List<Rom> CurrentRomList = new List<Rom>();
        Emulator CurrentEmulator = new Emulator();
        Point OriginalMenuPosition;
        string SoundEffectFile = "";
        string LaunchEffectFile = "";
        SoundPlayer soundeffect = new SoundPlayer();
        
        //string VideoPlaying = "";
        public Form1()
        {
            
            
            JLLibrary = new JL();
            InitializeComponent();
            //start screensaver timer
            
            JLLibrary.LoadEmulators();
            CurrentEmulator = JLLibrary.Emulators[EmulatorIndex];

            
            JLLibrary.LoadRoms();
            ReloadRoms();

            
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            //OriginalMenuPosition = menulabel1.Location;

            

            //load all settings
            var appSettings = ConfigurationManager.AppSettings;



            //look and feel

            int ConfigScreen = Convert.ToInt32(ConfigurationManager.AppSettings["ScreenNumber"]);

            SoundEffectFile = ConfigurationManager.AppSettings["SoundEffectFile"];
            LaunchEffectFile = ConfigurationManager.AppSettings["LaunchEffectFile"];

            soundeffect = new SoundPlayer(SoundEffectFile);
            Screen[] screens = Screen.AllScreens;

            if (screens.Length < (ConfigScreen + 1))
            {
                //if we pick an invalid screen, auto select 0 
                ConfigScreen = 0;
            }

            Rectangle bounds = screens[ConfigScreen].Bounds;
            this.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);


            this.BackColor = (Color)System.Drawing.ColorTranslator.FromHtml(ConfigurationManager.AppSettings["BackgroundColor"]);


            this.Show();
            
            UpdateScreen();
            Cursor.Hide();

            //if there is an autolaunch rom set up, then auto-launch it
            //note only set up for mame

            string RomAutoLaunch = ConfigurationManager.AppSettings["RomAutoLaunch"];
            if(RomAutoLaunch.Length > 0)
            {
                foreach (Rom PossibleRom in CurrentRomList)
                {
                    if(PossibleRom.FileName.Equals(RomAutoLaunch))
                    {
                        JLLibrary.Launch(CurrentEmulator, PossibleRom);
                    }
                }
            }

        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            MessageBox.Show("Form.KeyPress: '" +
                    e.KeyChar.ToString() + "' pressed.");

        }

        private void NextEmulator()
        {
            EmulatorIndex++;
            if (EmulatorIndex == JLLibrary.Emulators.Count)
            {
                EmulatorIndex = 0;
            }
            RomIndex = 0;
            CurrentEmulator = JLLibrary.Emulators[EmulatorIndex];
            ReloadRoms();
            UpdateScreen();

        }
        
        private void PreviousEmulator()
        {
            EmulatorIndex--;

            if(EmulatorIndex<0)
            {
                EmulatorIndex = JLLibrary.Emulators.Count - 1;
            }

            RomIndex = 0;
            CurrentEmulator = JLLibrary.Emulators[EmulatorIndex];
            ReloadRoms();
            UpdateScreen();
        }

        private void NextRom()
        {
            RomIndex++;
            if (RomIndex == CurrentRomList.Count) //menulabel1.Text.Length > 0)
            {
                
                RomIndex = 0;
            }

            UpdateScreen();

        }

        private void PreviousRom()
        {

            RomIndex--;
            if (RomIndex == -1)
            {
                RomIndex = CurrentRomList.Count-1;
            }
            UpdateScreen();

        }

        private void ReloadRoms()
        {

            //reset the RomIndex
            RomIndex = 0;
            CurrentEmulator = JLLibrary.Emulators[EmulatorIndex];

            label1.Text = CurrentEmulator.Name;

            CurrentRomList.Clear();


            foreach (Rom CurrentRom in JLLibrary.Roms)
            {
                if (CurrentRom.EmulatorName.Equals(CurrentEmulator.Name))
                {
                    if(CurrentRom.Display.Equals("Yes"))
                    {
                        CurrentRomList.Add(CurrentRom);
                    }

                }
            }

            //now update the GUI
            string RomList = "";
            foreach (Rom CurrentRom in CurrentRomList)
            {
                if (CurrentRom.EmulatorName.Equals(CurrentEmulator.Name))
                {

                    RomList = RomList + CurrentRom.Name + "\n";
                }
            }


            //menulabel1.Text = RomList;

            if (CurrentRomList.Count > 0)
            {
                UpdateScreen();
            }
            else
            {
                selectedLabel1.Text = "";
            }

        }


        public void RefreshSampleVideo()
        {

            

        }

        public void UpdateScreen()
        {
            //reset screensaver count to zero because if we get here there was a menu move
            ScreensaverCounter = 0;

            // re-load and re-align the Rom Set, Marquee, title, and snapshot
            string FontName = ConfigurationManager.AppSettings["Font"];
            int SystemFontSize = Convert.ToInt32(ConfigurationManager.AppSettings["SystemFontSize"]);
            int ButtonFontSize = Convert.ToInt32(ConfigurationManager.AppSettings["ButtonFontSize"]);
            int GameFontSize = Convert.ToInt32(ConfigurationManager.AppSettings["GameFontSize"]);
            int TopMargin = Convert.ToInt32(ConfigurationManager.AppSettings["TopMargin"]);
            int MarqueeWidth = Convert.ToInt32(ConfigurationManager.AppSettings["MarqueeWidth"]);
            int ScreenshotWidth = Convert.ToInt32(ConfigurationManager.AppSettings["ScreenshotWidth"]);
            int ScreenshotHeight = Convert.ToInt32(ConfigurationManager.AppSettings["ScreenshotHeight"]);
            int VerticalSpacing = Convert.ToInt32(ConfigurationManager.AppSettings["VerticalSpacing"]);
            int ButtonCount = Convert.ToInt32(ConfigurationManager.AppSettings["ButtonCount"]);

            // Rom set / system name
            // centered and set to Top Margin (height in px from top of screen)
            label1.Top = TopMargin;
            label1.Left = (this.ClientSize.Width / 2) - (label1.Width / 2);
            label1.Font = new Font(FontName, SystemFontSize);
            label1.Show();


            // now game title
            selectedLabel1.Text = CurrentRomList[RomIndex].Name;
            // centered
            selectedLabel1.Left = (this.ClientSize.Width / 2) - (selectedLabel1.Width / 2);
            //offset 
            selectedLabel1.Top = label1.Top + label1.Height + VerticalSpacing;
            selectedLabel1.Font = new Font(FontName, GameFontSize);
            selectedLabel1.Show();

            //new control panel code
            //center
            controlpanel.Left = (this.ClientSize.Width / 2) - (controlpanel.Width / 2);
            //offset from game title
            controlpanel.Top = selectedLabel1.Top + selectedLabel1.Height + VerticalSpacing;
            //build the control panel
            if (CurrentRomList[RomIndex].Joystick.Equals("4-Way"))
            {
                joystick.ImageLocation = @"joystick_4way.png";
                joystick2.ImageLocation = @"joystick_4way.png";
            }
            else if (CurrentRomList[RomIndex].Joystick.Equals("8-Way") )
            {
                joystick.ImageLocation = @"joystick_8way.png";
                joystick2.ImageLocation = @"joystick_8way.png";
            }
            else if (CurrentRomList[RomIndex].Joystick.Equals("None"))
            {
                joystick.ImageLocation = @"joystick_black.png";
                joystick2.ImageLocation = @"joystick_black.png";
            }

            string[] Buttons = ConfigurationManager.AppSettings["ButtonOrder"].Split(',');
            //color in buttons
            button1.ImageLocation = Buttons[0] + "_button.png";
            button1label.Font = new Font(FontName, ButtonFontSize);
            button2.ImageLocation = Buttons[1] + "_button.png";
            button2label.Font = new Font(FontName, ButtonFontSize);
            button3.ImageLocation = Buttons[2] + "_button.png";
            button3label.Font = new Font(FontName, ButtonFontSize);
            button4.ImageLocation = Buttons[3] + "_button.png";
            button4label.Font = new Font(FontName, ButtonFontSize);
            button5.ImageLocation = Buttons[4] + "_button.png";
            button5label.Font = new Font(FontName, ButtonFontSize);
            button6.ImageLocation = Buttons[5] + "_button.png";
            button6label.Font = new Font(FontName, ButtonFontSize);

            controlpanel.Show();
            joystick.Show();
            button1.Show();
            button1label.Show();
            button2.Show();
            button2label.Show();
            button3.Show();
            button3label.Show();
            
            button4.Show();
            button4label.Show();
            button5.Show();
            button5label.Show();
            button6.Show();
            button6label.Show();



            //now that we built the control panel, let's customize it

            ArrayList ControlPanelButtons = new ArrayList();

            
            if (CurrentEmulator.Buttons.Length > 0)
            {
                //if there is a system-wide control scheme (ex. NES) let's color and assign accordingly
                string[] ControlPanelButtonArray = CurrentEmulator.Buttons.Split('-');
                
                foreach(string ControlPanelButtonDefinition in ControlPanelButtonArray)
                {
                    ControlPanelButtons.Add(ControlPanelButtonDefinition);
                }
                
            }
            else
            {
                //so there's no system-wide control scheme (ex. MAME) let's color and assign by ROM 
                string[] ControlPanelButtonArray = CurrentRomList[RomIndex].Buttons.Split('-');
                foreach (string ControlPanelButtonDefinition in ControlPanelButtonArray)
                {
                    ControlPanelButtons.Add(ControlPanelButtonDefinition);
                }
            }


            //if the buttonorder says "DualJoystick" then hide all the buttons and just show joystick 2

            if (CurrentRomList[RomIndex].Buttons.Equals("DualJoystick"))
            {
                button1.Hide();
                button1label.Text = "";
                button2.Hide();
                button2label.Text = "";
                button3.Hide();
                button3label.Text = "";
                button4.Hide();
                button4label.Text = "";
                button5.Hide();
                button5label.Text = "";
                button6.Hide();
                button6label.Text = "";

                joystick2.Show();
                joystick2.Top = joystick.Top;
                joystick2.Left = button3.Left;

            }


            else

            {
                joystick2.Hide();
                //now fill in the actual button labels and darken all unused buttons
                while (ControlPanelButtons.Count < 6)
                {
                    //Add blank buttons
                    ControlPanelButtons.Add("");
                }
                //now that we have the buttons in memory, let's assign them
                bool ButtonsExist = false;
                //button 1
                if (ControlPanelButtons[0].ToString().Length == 0)
                {
                    //darken button and blank out function
                    button1label.Text = "";
                    button1.ImageLocation = @"black_button.png";
                }
                else
                {
                    //set this button to the assigned function
                    button1label.Text = ControlPanelButtons[0].ToString();
                    //re-align
                    button1label.Left = button1.Left + (button1.Width / 2) - (button1label.Width / 2);
                    ButtonsExist = true;
                }

                //button 2
                if (ControlPanelButtons[1].ToString().Length == 0)
                {
                    //darken button and blank out function
                    button2label.Text = "";
                    button2.ImageLocation = @"black_button.png";
                }
                else
                {
                    //set this button to the assigned function
                    button2label.Text = ControlPanelButtons[1].ToString();
                    button2label.Left = button2.Left + (button2.Width / 2) - (button2label.Width / 2);
                    ButtonsExist = true;
                }

                //button 3
                if (ControlPanelButtons[2].ToString().Length == 0)
                {
                    //darken button and blank out function
                    button3label.Text = "";
                    button3.ImageLocation = @"black_button.png";
                }
                else
                {
                    //set this button to the assigned function
                    button3label.Text = ControlPanelButtons[2].ToString();
                    button3label.Left = button3.Left + (button3.Width / 2) - (button3label.Width / 2);
                    ButtonsExist = true;
                }

                //button 4
                if (ControlPanelButtons[3].ToString().Length == 0)
                {
                    //darken button and blank out function
                    button4label.Text = "";
                    button4.ImageLocation = @"black_button.png";
                }
                else
                {
                    //set this button to the assigned function
                    button4label.Text = ControlPanelButtons[3].ToString();
                    button4label.Left = button4.Left + (button4.Width / 2) - (button4label.Width / 2);
                    ButtonsExist = true;
                }

                //button 5
                if (ControlPanelButtons[4].ToString().Length == 0)
                {
                    //darken button and blank out function
                    button5label.Text = "";
                    button5.ImageLocation = @"black_button.png";
                }
                else
                {
                    //set this button to the assigned function
                    button5label.Text = ControlPanelButtons[4].ToString();
                    button5label.Left = button5.Left + (button5.Width / 2) - (button5label.Width / 2);
                    ButtonsExist = true;
                }

                //button 6
                if (ControlPanelButtons[5].ToString().Length == 0)
                {
                    //darken button and blank out function
                    button6label.Text = "";
                    button6.ImageLocation = @"black_button.png";
                }
                else
                {
                    //set this button to the assigned function
                    button6label.Text = ControlPanelButtons[5].ToString();
                    button6label.Left = button6.Left + (button6.Width / 2) - (button6label.Width / 2);
                    ButtonsExist = true;
                }

                //hide bottom 3 buttons for the classic cabinet
                if (Convert.ToInt32(ConfigurationManager.AppSettings["ButtonCount"]) == 3)
                {
                    button4.Hide();
                    button4label.Hide();
                    button5.Hide();
                    button5label.Hide();
                    button6.Hide();
                    button6label.Hide();
                    controlpanel.Height = 180;
                }

            }
            // marquee (we are removing this for now - replaced with control panel code above)
            //if (CurrentEmulator.MarqueeLocation.Length == 0)
            //{
            //    //no marquee
            //    pictureBox1.Hide();
            //}
            //else
            //{
            //    string MarqueeFile = CurrentEmulator.MarqueeLocation + CurrentRomList[RomIndex].Marquee;
            //    if (File.Exists(MarqueeFile))
            //    {
            //        pictureBox1.ImageLocation = MarqueeFile;

            //    }
            //    else
            //    {
            //        pictureBox1.ImageLocation = CurrentEmulator.MarqueeLocation + "unknown.png";

            //    }

            //    pictureBox1.Load();
            //    pictureBox1.Width = MarqueeWidth;
            //    // centered
            //    pictureBox1.Left = (this.ClientSize.Width - pictureBox1.Width) / 2;
            //    pictureBox1.Height = Convert.ToInt32(pictureBox1.Width * pictureBox1.Image.Size.Height / pictureBox1.Image.Size.Width);

            //    //offset 25px
            //    pictureBox1.Top = selectedLabel1.Top + selectedLabel1.Height + VerticalSpacing;
            //    pictureBox1.Show();


            //}
            //// last screenshot (if available) - this has to offset by 50px "south" of the marquee


            string Screenshot = CurrentEmulator.ScreenshotLocation + CurrentRomList[RomIndex].Screenshot;
            
            if (File.Exists(Screenshot))
            {


                //check for video
                if(Screenshot.EndsWith(".mp4"))
                {
                    
                }
                else
                {
                    //no it's a pic

                    pictureBox2.ImageLocation = Screenshot;
                    pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                    bool portrait = false;
                    pictureBox2.Load();
                    pictureBox2.Show();
                    if (pictureBox2.Width / pictureBox2.Height < 1)
                    {
                        portrait = true; 
                    }

                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

                    pictureBox2.Height = ScreenshotHeight;
                    // centered
                    pictureBox2.Show();
                    
                    if (portrait)
                    {

                        //pictureBox2.Height = Convert.ToInt32(pictureBox2.Width * pictureBox2.Image.Size.Height / pictureBox2.Image.Size.Width);
                        pictureBox2.Width = Convert.ToInt32(pictureBox2.Height / 1.28);

                    }
                    else
                    {
                        //landscape
                        //pictureBox2.Height = pictureBox2.Width; //Convert.ToInt32(pictureBox2.Width * (5/4));
                        pictureBox2.Width = Convert.ToInt32(pictureBox2.Height / 0.8);
                    }
                    //offset from control panel
                    pictureBox2.Top = controlpanel.Top + controlpanel.Height + VerticalSpacing;
                    //center
                    pictureBox2.Left = (this.ClientSize.Width - pictureBox2.Width) / 2;

                    pictureBox2.Show();

                }
            }
            else
            {

                //nothing - don't load
                pictureBox2.Hide();
                //pictureBox1.ImageLocation = @"c:\emulation\mame\marquees\unknown.png";
                //pictureBox1.Load();
            }

            
            

        }


        //UpdateScreenHorizontal
        public void UpdateScreenHorizontal()
        {
            // re-load and re-align the Rom Set, Marquee, title, and snapshot
            //axWindowsMediaPlayer1.Hide();
            string FontName = ConfigurationManager.AppSettings["Font"];
            int SystemFontSize = Convert.ToInt32(ConfigurationManager.AppSettings["SystemFontSize"]);
            int ButtonFontSize = Convert.ToInt32(ConfigurationManager.AppSettings["ButtonFontSize"]);
            int GameFontSize = Convert.ToInt32(ConfigurationManager.AppSettings["GameFontSize"]);
            int TopMargin = Convert.ToInt32(ConfigurationManager.AppSettings["TopMargin"]);
            int MarqueeWidth = Convert.ToInt32(ConfigurationManager.AppSettings["MarqueeWidth"]);
            int ScreenshotWidth = Convert.ToInt32(ConfigurationManager.AppSettings["ScreenshotWidth"]);
            int VerticalSpacing = Convert.ToInt32(ConfigurationManager.AppSettings["VerticalSpacing"]);
            int HorizontalSpacing = Convert.ToInt32(ConfigurationManager.AppSettings["HorizontalSpacing"]);
            int ButtonCount = Convert.ToInt32(ConfigurationManager.AppSettings["ButtonCount"]);

            // Rom set / system name
            // centered and set to Top Margin (height in px from top of screen)
            label1.Top = TopMargin;
            label1.Left = (this.ClientSize.Width / 2) - (label1.Width / 2);
            label1.Font = new Font(FontName, SystemFontSize);
            label1.Show();


            // now game title
            selectedLabel1.Text = CurrentRomList[RomIndex].Name;
            // centered in left side of screen
            selectedLabel1.Left = (this.ClientSize.Width / 2) - (selectedLabel1.Width / 2);
            //offset 
            selectedLabel1.Top = label1.Top + label1.Height + VerticalSpacing;
            selectedLabel1.Font = new Font(FontName, GameFontSize);
            selectedLabel1.Show();

            //new control panel code
            //centered in left side of screen - under the game title
            controlpanel.Left = (this.ClientSize.Width / 4) - (controlpanel.Width / 2) + HorizontalSpacing;
            //offset from game title
            controlpanel.Top = selectedLabel1.Top + selectedLabel1.Height + VerticalSpacing;
            //build the control panel
            joystick.ImageLocation = @"joystick.png";
            if (CurrentRomList[RomIndex].Joystick.Equals("None"))
            {
                joystick.ImageLocation = @"joystick_black.png";
            }
            string[] Buttons = ConfigurationManager.AppSettings["ButtonOrder"].Split(',');
            //color in buttons
            button1.ImageLocation = Buttons[0] + "_button.png";
            button1label.Font = new Font(FontName, ButtonFontSize);
            button2.ImageLocation = Buttons[1] + "_button.png";
            button2label.Font = new Font(FontName, ButtonFontSize);
            button3.ImageLocation = Buttons[2] + "_button.png";
            button3label.Font = new Font(FontName, ButtonFontSize);
            button4.ImageLocation = Buttons[3] + "_button.png";
            button4label.Font = new Font(FontName, ButtonFontSize);
            button5.ImageLocation = Buttons[4] + "_button.png";
            button5label.Font = new Font(FontName, ButtonFontSize);
            button6.ImageLocation = Buttons[5] + "_button.png";
            button6label.Font = new Font(FontName, ButtonFontSize);

            controlpanel.Show();
            joystick.Show();
            button1.Show();
            button1label.Show();
            button2.Show();
            button2label.Show();
            button3.Show();
            button3label.Show();

            button4.Show();
            button4label.Show();
            button5.Show();
            button5label.Show();
            button6.Show();
            button6label.Show();



            //now that we built the control panel, let's customize it

            ArrayList ControlPanelButtons = new ArrayList();


            if (CurrentEmulator.Buttons.Length > 0)
            {
                //if there is a system-wide control scheme (ex. NES) let's color and assign accordingly
                string[] ControlPanelButtonArray = CurrentEmulator.Buttons.Split('-');

                foreach (string ControlPanelButtonDefinition in ControlPanelButtonArray)
                {
                    ControlPanelButtons.Add(ControlPanelButtonDefinition);
                }

            }
            else
            {
                //so there's no system-wide control scheme (ex. MAME) let's color and assign by ROM 
                string[] ControlPanelButtonArray = CurrentRomList[RomIndex].Buttons.Split('-');
                foreach (string ControlPanelButtonDefinition in ControlPanelButtonArray)
                {
                    ControlPanelButtons.Add(ControlPanelButtonDefinition);
                }
            }

            //now fill in the actual button labels and darken all unused buttons
            while (ControlPanelButtons.Count < 6)
            {
                //Add blank buttons
                ControlPanelButtons.Add("");
            }
            //now that we have the buttons in memory, let's assign them
            bool ButtonsExist = false;
            //button 1
            if (ControlPanelButtons[0].ToString().Length == 0)
            {
                //darken button and blank out function
                button1label.Text = "";
                button1.ImageLocation = @"black_button.png";
            }
            else
            {
                //set this button to the assigned function
                button1label.Text = ControlPanelButtons[0].ToString();
                //re-align
                button1label.Left = button1.Left + (button1.Width / 2) - (button1label.Width / 2);
                ButtonsExist = true;
            }

            //button 2
            if (ControlPanelButtons[1].ToString().Length == 0)
            {
                //darken button and blank out function
                button2label.Text = "";
                button2.ImageLocation = @"black_button.png";
            }
            else
            {
                //set this button to the assigned function
                button2label.Text = ControlPanelButtons[1].ToString();
                button2label.Left = button2.Left + (button2.Width / 2) - (button2label.Width / 2);
                ButtonsExist = true;
            }

            //button 3
            if (ControlPanelButtons[2].ToString().Length == 0)
            {
                //darken button and blank out function
                button3label.Text = "";
                button3.ImageLocation = @"black_button.png";
            }
            else
            {
                //set this button to the assigned function
                button3label.Text = ControlPanelButtons[2].ToString();
                button3label.Left = button3.Left + (button3.Width / 2) - (button3label.Width / 2);
                ButtonsExist = true;
            }

            //button 4
            if (ControlPanelButtons[3].ToString().Length == 0)
            {
                //darken button and blank out function
                button4label.Text = "";
                button4.ImageLocation = @"black_button.png";
            }
            else
            {
                //set this button to the assigned function
                button4label.Text = ControlPanelButtons[3].ToString();
                button4label.Left = button4.Left + (button4.Width / 2) - (button4label.Width / 2);
                ButtonsExist = true;
            }

            //button 5
            if (ControlPanelButtons[4].ToString().Length == 0)
            {
                //darken button and blank out function
                button5label.Text = "";
                button5.ImageLocation = @"black_button.png";
            }
            else
            {
                //set this button to the assigned function
                button5label.Text = ControlPanelButtons[4].ToString();
                button5label.Left = button5.Left + (button5.Width / 2) - (button5label.Width / 2);
                ButtonsExist = true;
            }

            //button 6
            if (ControlPanelButtons[5].ToString().Length == 0)
            {
                //darken button and blank out function
                button6label.Text = "";
                button6.ImageLocation = @"black_button.png";
            }
            else
            {
                //set this button to the assigned function
                button6label.Text = ControlPanelButtons[5].ToString();
                button6label.Left = button6.Left + (button6.Width / 2) - (button6label.Width / 2);
                ButtonsExist = true;
            }

            //hide bottom 3 buttons for the classic cabinet
            if (Convert.ToInt32(ConfigurationManager.AppSettings["ButtonCount"]) == 3)
            {
                button4.Hide();
                button4label.Hide();
                button5.Hide();
                button5label.Hide();
                button6.Hide();
                button6label.Hide();
                controlpanel.Height = 180;
            }


            // marquee (we are removing this for now - replaced with control panel code above)
            //if (CurrentEmulator.MarqueeLocation.Length == 0)
            //{
            //    //no marquee
            //    pictureBox1.Hide();
            //}
            //else
            //{
            //    string MarqueeFile = CurrentEmulator.MarqueeLocation + CurrentRomList[RomIndex].Marquee;
            //    if (File.Exists(MarqueeFile))
            //    {
            //        pictureBox1.ImageLocation = MarqueeFile;

            //    }
            //    else
            //    {
            //        pictureBox1.ImageLocation = CurrentEmulator.MarqueeLocation + "unknown.png";

            //    }

            //    pictureBox1.Load();
            //    pictureBox1.Width = MarqueeWidth;
            //    // centered
            //    pictureBox1.Left = (this.ClientSize.Width - pictureBox1.Width) / 2;
            //    pictureBox1.Height = Convert.ToInt32(pictureBox1.Width * pictureBox1.Image.Size.Height / pictureBox1.Image.Size.Width);

            //    //offset 25px
            //    pictureBox1.Top = selectedLabel1.Top + selectedLabel1.Height + VerticalSpacing;
            //    pictureBox1.Show();


            //}
            //// last screenshot (if available) - this has to offset by 50px "south" of the marquee


            string Screenshot = CurrentEmulator.ScreenshotLocation + CurrentRomList[RomIndex].Screenshot;

            if (File.Exists(Screenshot))
            {


                //check for video
                if (Screenshot.EndsWith(".mp4"))
                {

                }
                else
                {
                    //no it's a pic

                    pictureBox2.ImageLocation = Screenshot;
                    pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                    bool portrait = false;
                    pictureBox2.Load();
                    if (pictureBox2.Height > pictureBox2.Width)
                    {
                        portrait = true;
                    }

                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

                    pictureBox2.Width = ScreenshotWidth;
                    // centered in right side of screen
                    pictureBox2.Left = (3 * (this.ClientSize.Width / 4))  - (pictureBox2.Width / 2) - 100;
                    if (portrait)
                    {

                        pictureBox2.Height = Convert.ToInt32(pictureBox2.Width * pictureBox2.Image.Size.Height / pictureBox2.Image.Size.Width);
                    }
                    else
                    {
                        //landscape
                        //pictureBox2.Height = pictureBox2.Width; //Convert.ToInt32(pictureBox2.Width * (5/4));
                        pictureBox2.Height = Convert.ToInt32(pictureBox2.Width * 0.75);
                    }
                    //offset from emulator label
                    pictureBox2.Top = selectedLabel1.Top + selectedLabel1.Height + VerticalSpacing;


                    pictureBox2.Show();

                }
            }
            else
            {

                //nothing - don't load
                pictureBox2.Hide();
                //pictureBox1.ImageLocation = @"c:\emulation\mame\marquees\unknown.png";
                //pictureBox1.Load();
            }




        }


        //handles up/down/left/right
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //for all items play the sound effect
            
            switch(keyData)
            {
                case Keys.Down:
                    soundeffect.Play();
                    NextRom();
                    break;
                case Keys.Up:
                    soundeffect.Play();
                    PreviousRom();
                    break;
                case Keys.Right:
                    soundeffect.Play();
                    NextEmulator();
                    break;
                case Keys.Left:
                    soundeffect.Play();
                    PreviousEmulator();
                    break;

                case Keys.ControlKey | Keys.Control:
                    //first player button
                    soundeffect = new SoundPlayer(LaunchEffectFile);
                    soundeffect.Play();
                    //Launch emulator and Rom
                    JLLibrary.Launch(CurrentEmulator, CurrentRomList[RomIndex]);
                    soundeffect = new SoundPlayer(SoundEffectFile);
                    break;
                case Keys.Escape:

                    Form2 frm2 = new Form2();
                    frm2.Show();
                    
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        

        

        
        private void menulabel1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            

        }

       

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }

    public class MenuLabel : Label
    {
        public MenuLabel()
        {
            OutlineForeColor = Color.Green;
            OutlineWidth = 2;
        }
        public Color OutlineForeColor { get; set; }
        public float OutlineWidth { get; set; }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen outline = new Pen(OutlineForeColor, OutlineWidth)
            { LineJoin = LineJoin.Round })
            using (StringFormat sf = new StringFormat())
            using (Brush foreBrush = new SolidBrush(ForeColor))
            {
                gp.AddString(Text, Font.FontFamily, (int)Font.Style,
                    Font.Size, ClientRectangle, sf);
                e.Graphics.ScaleTransform(1.3f, 1.35f);
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.DrawPath(outline, gp);
                e.Graphics.FillPath(foreBrush, gp);
            }
        }


    }




    public class TitleLabel : Label
    {
        public TitleLabel()
        {
            OutlineForeColor = Color.Black;
            OutlineWidth = 2;
        }
        public Color OutlineForeColor { get; set; }
        public float OutlineWidth { get; set; }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen outline = new Pen(OutlineForeColor, OutlineWidth)
            { LineJoin = LineJoin.Round })
            using (StringFormat sf = new StringFormat())
            using (Brush foreBrush = new SolidBrush(ForeColor))
            {
                gp.AddString(Text, Font.FontFamily, (int)Font.Style,
                    Font.Size, ClientRectangle, sf);
                e.Graphics.ScaleTransform(1.3f, 1.35f);
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.DrawPath(outline, gp);
                e.Graphics.FillPath(foreBrush, gp);
            }
        }


    }



    public class SelectedLabel : Label
    {
        public SelectedLabel()
        {
            OutlineForeColor = Color.Green;
            OutlineWidth = 2;
        }
        public Color OutlineForeColor { get; set; }
        public float OutlineWidth { get; set; }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen outline = new Pen(OutlineForeColor, OutlineWidth)
            { LineJoin = LineJoin.Round })
            using (StringFormat sf = new StringFormat())
            using (Brush foreBrush = new SolidBrush(ForeColor))
            {
                gp.AddString(Text, Font.FontFamily, (int)Font.Style,
                    Font.Size, ClientRectangle, sf);
                e.Graphics.ScaleTransform(1.3f, 1.35f);
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.DrawPath(outline, gp);
                e.Graphics.FillPath(foreBrush, gp);
            }
        }


    }


}
