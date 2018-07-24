using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace PUBG_NO_RECOIL_MANAGER
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    static class NoReciolScript
    {
        static string gun_key = "akm";
        static bool ump9_key = false;
        static bool akm_key = true;
        static bool m16a4_key = false;
        static bool m416_key = false;
        static bool scarl_key = false;
        static bool uzi_key = false;
        static bool qbz_key = false;

        //select gun
        public static void selectGun(bool ump, bool akm, bool m, bool mfour, bool scarl, bool uzi,bool qbz, string gun)
        {
            gun_key = gun;
            ump9_key = ump;
            akm_key = akm;
            m16a4_key = m;
            m416_key = mfour;
            scarl_key = scarl;
            uzi_key = uzi;
            qbz_key = qbz;
        }

        //checks for update from github project
        public static void checkForUpdate()
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://raw.githubusercontent.com/minglich/logitech-pubg/master/weaponspeed_mode.lua");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            if (!buildScrip(content).Trim().Equals(buildScrip(Properties.Settings.Default.pubgScript).Trim()))
            {
                MessageBox.Show("Script Updated! \n \n" +
                    "https://raw.githubusercontent.com/minglich/logitech-pubg/master/weaponspeed_mode.lua \n \n" +
                    "If its not working manually update script to old scrip.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Properties.Settings.Default.pubgScript = content;
                Properties.Settings.Default.Save();
            }
        }

        //start logictec driver
        public static void startLogiDriver()
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Program Files\\Logitech Gaming Software\\LCore.exe",
                    Arguments = "/minimized",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
        }

        //kills logictec driver
        public static void killLogiDriver()
        {
            Process[] proc = Process.GetProcesses();
            foreach (Process item in proc)
            {
                if (item.ProcessName.Equals("LCore"))
                {

                    item.Kill();
                }
            }
        }

        //check if profile has pubg reference
        public static void checkProfile()
        {
            string text = System.IO.File.ReadAllText(Properties.Settings.Default.profilePath);
            if (!text.Contains("TSLGAME.EXE"))
            {
                MessageBox.Show("Warning profile does not match!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        //write the script to profile
        public static void updateProfile()
        {
            string text = System.IO.File.ReadAllText(Properties.Settings.Default.profilePath);
            string top = text.Split(new[] { "<script>" }, StringSplitOptions.None)[0];
            string bottom = text.Split(new[] { "</script>" }, StringSplitOptions.None)[1];
            String all = top + "<script>" + Environment.NewLine + buildScrip(Properties.Settings.Default.pubgScript) + Environment.NewLine + "</script>" + bottom;
            System.IO.File.WriteAllText(Properties.Settings.Default.profilePath, all);          
        }

        //make pubg profile the defualt profile
        private static string defualtProfileName = "";
        public static void makeProfileDefualt()
        {

            string[] text = System.IO.File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Logitech\\Logitech Gaming Software\\settings.json");
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].ToLower().Contains(("\"defaultProfile\"").ToString().ToLower()))
                {
                    defualtProfileName = text[i].Split(':')[1].Replace(",", "").Replace("\"", "").Trim();
                    Console.Write(defualtProfileName);
                    string pubgprofile = Properties.Settings.Default.profilePath.Split(new[] { "\\" }, StringSplitOptions.None)[Properties.Settings.Default.profilePath.Split(new[] { "\\" }, StringSplitOptions.None).Length - 1].Split('.')[0];
                    text[i] = "\"defaultProfile\" : \"" + pubgprofile + "\"";
                }
            }
            string complete = "";
            foreach (var item in text)
            {
                complete += item + Environment.NewLine;
            }
            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Logitech\\Logitech Gaming Software\\settings.json", complete);
        }

        //resets the default profiel back to actual defualt
        public static void resetProfileDefualt()
        {
            if (!defualtProfileName.Equals(""))
            {
                string[] text = System.IO.File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Logitech\\Logitech Gaming Software\\settings.json");
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i].ToLower().Contains(("\"defaultProfile\"").ToString().ToLower()))
                    {
                        text[i] = "\"defaultProfile\" : \"" + defualtProfileName + "\"";
                    }
                }
                string complete = "";
                foreach (var item in text)
                {
                    complete += item + Environment.NewLine;
                }
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Logitech\\Logitech Gaming Software\\settings.json", complete);
            }           
        }

        //builds the actual script
        public static string buildScrip(String input)
        {
            string[] scriptArray = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < scriptArray.Length; i++)
            {
                //vertical_sensitivity 
                if (scriptArray[i].Contains("local vertical_sensitivity ="))
                {
                    scriptArray[i] = "local vertical_sensitivity = " + Properties.Settings.Default.verticalSen;
                }
                //target_sensitivity  
                if (scriptArray[i].Contains("local target_sensitivity ="))
                {
                    scriptArray[i] = "local target_sensitivity = " + Properties.Settings.Default.targetSen;
                }
                //scope_sensitivity 
                if (scriptArray[i].Contains("local scope_sensitivity ="))
                {
                    scriptArray[i] = "local scope_sensitivity = " + Properties.Settings.Default.scopeSen;
                }
                //scope4x_sensitivity 
                if (scriptArray[i].Contains("local scope4x_sensitivity ="))
                {
                    scriptArray[i] = "local scope4x_sensitivity = " + Properties.Settings.Default.scopeFourSen;
                }

                //ump
                if (scriptArray[i].Contains("local ump9_key ="))
                {
                    if (ump9_key)
                    {
                        scriptArray[i] = "local ump9_key = " + Properties.Settings.Default.mouseActivate;
                    }
                    else
                    {
                        scriptArray[i] = "local ump9_key = nil";
                    }
                }
                //akm
                if (scriptArray[i].Contains("local akm_key ="))
                {
                    if (akm_key)
                    {
                        scriptArray[i] = "local akm_key = " + Properties.Settings.Default.mouseActivate;
                    }
                    else
                    {
                        scriptArray[i] = "local akm_key = nil";
                    }
                }
                //m16a4
                if (scriptArray[i].Contains("local m16a4_key ="))
                {
                    if (m16a4_key)
                    {
                        scriptArray[i] = "local m16a4_key = " + Properties.Settings.Default.mouseActivate;
                    }
                    else
                    {
                        scriptArray[i] = "local m16a4_key = nil";
                    }
                }
                //m416
                if (scriptArray[i].Contains("local m416_key ="))
                {
                    if (m416_key)
                    {
                        scriptArray[i] = "local m416_key = " + Properties.Settings.Default.mouseActivate;
                    }
                    else
                    {
                        scriptArray[i] = "local m416_key = nil";
                    }
                }
                //scarl
                if (scriptArray[i].Contains("local scarl_key ="))
                {
                    if (scarl_key)
                    {
                        scriptArray[i] = "local scarl_key = " + Properties.Settings.Default.mouseActivate;
                    }
                    else
                    {
                        scriptArray[i] = "local scarl_key = nil";
                    }
                }
                //uzi_key 
                if (scriptArray[i].Contains("local uzi_key ="))
                {
                    if (uzi_key)
                    {
                        scriptArray[i] = "local uzi_key = " + Properties.Settings.Default.mouseActivate;
                    }
                    else
                    {
                        scriptArray[i] = "local uzi_key = nil";
                    }
                }
                //qbz_key 
                if (scriptArray[i].Contains("local qbz_key ="))
                {
                    if (qbz_key)
                    {
                        scriptArray[i] = "local qbz_key = " + Properties.Settings.Default.mouseActivate;
                    }
                    else
                    {
                        scriptArray[i] = "local qbz_key = nil";
                    }
                }


                //set_off_key  
                if (scriptArray[i].Contains("local set_off_key ="))
                {
                    scriptArray[i] = "local set_off_key = " + Properties.Settings.Default.mouseDisable;// + Environment.NewLine + "local onRun = 1";
                }

                //script on launch
                if (scriptArray[i].Contains("PROFILE_DEACTIVATED"))
                {
                    scriptArray[i] = "current_weapon = \"" + gun_key +"\"" + Environment.NewLine + scriptArray[i];
                }

            }
            string complete = "";
            foreach (var item in scriptArray)
            {
                complete += item + Environment.NewLine;
            }
            return complete;
        }
    }
}