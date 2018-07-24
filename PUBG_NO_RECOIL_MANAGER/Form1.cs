using Gma.System.MouseKeyHook;
using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using System.Speech.Synthesis;
using System.Linq;

namespace PUBG_NO_RECOIL_MANAGER
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        SoundPlayer audioStream = new SoundPlayer(Properties.Resources.akm);
        private IKeyboardMouseEvents m_GlobalHook;
        private bool pubgStarted = false;
        private int weaponIndex = 0;
        private bool waitingForInput = false;
        private bool firstClosingBlocked = true;
        private bool started = false;
        private bool guiComplete = false;
        private string pimaryGun = "akm";
        private string secondaryGun = "akm";
        private string currentGun = "akm";

        public Form1()
        {
            InitializeComponent();
            Text = RandomString();
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyDown += GlobalHookKeyDown;
            m_GlobalHook.MouseClick += OnMouseClick;
            synthesizer.Volume = 100;
            synthesizer.Rate = 2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.mouseActivate;
            textBox2.Text = Properties.Settings.Default.mouseDisable;
            textBox3.Text = Properties.Settings.Default.verticalSen;
            textBox4.Text = Properties.Settings.Default.targetSen;
            textBox5.Text = Properties.Settings.Default.scopeSen;
            textBox6.Text = Properties.Settings.Default.scopeFourSen;

            checkBox1.Checked = (bool)Properties.Settings.Default.openWithPUBG;
            if (Properties.Settings.Default.closeWithPUBG)
            {
                checkBox2.Checked = true;
                timer1.Start();
            }
            comboBox1.Items.Add("Press to select");
            comboBox2.Items.Add("Press to select");
            var mButtons = Enum.GetValues(typeof(MouseButtons));
            foreach (var item in mButtons)
            {
                comboBox1.Items.Add(item.ToString());
                comboBox2.Items.Add(item.ToString());
            }
            var kButtons = Enum.GetValues(typeof(Keys));
            foreach (var item in kButtons)
            {
                comboBox1.Items.Add(item.ToString());
                comboBox2.Items.Add(item.ToString());
            }
            comboBox1.Text = Properties.Settings.Default.gunSwitchKey;
            comboBox2.Text = Properties.Settings.Default.gunDisableKey;
            guiComplete = true;
            if (Properties.Settings.Default.autoUpdate)
            {
                NoReciolScript.checkForUpdate();
            }            
        }



        //============================================================================================================================
        //METHODS
        //============================================================================================================================

        //generate random string
        public string RandomString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, random.Next(0, 1000))
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //disable current gun
        private void gunDisabledAsync()
        {
            synthesizer.SpeakAsync("current gun disabled");
            switch (textBox7.Text)
            {
                case "akm":
                    checkBox4.Checked = false;
                    break;
                case "ump9":
                    checkBox5.Checked = false;
                    break;
                case "m416":
                    checkBox6.Checked = false;
                    break;
                case "m16a4":
                    checkBox8.Checked = false;
                    break;
                case "scarl":
                    checkBox7.Checked = false;
                    break;
                case "uzi":
                    checkBox3.Checked = false;
                    break;
                case "qbz":
                    checkBox9.Checked = false;
                    break;
            }
            switchGunRecoilAsync(null);
        }

        //returns number to according to gun name
        private int gunNameToInt(String gunName)
        {
            switch (gunName)
            {
                case "akm": return 0;
                case "ump9": return 1;
                case "m416": return 2;
                case "m16a4": return 3;
                case "scarl": return 4;
                case "uzi": return 5;
                case "qbz": return 6;

            }
            return 0;
        }


        //switch guns
        private async void switchGunRecoilAsync(String overide)
        {
            weaponIndex++;

            if (overide != null)
            {
                weaponIndex = gunNameToInt(overide);
            }

            int temp = weaponIndex;
            
            GUNSWITCH:
            switch (weaponIndex)
            {
                case 0:
                    if (checkBox4.Checked)
                    {
                        currentGun = "akm";
                        textBox7.Text = "akm";
                        NoReciolScript.selectGun(false, true, false, false, false, false, false, "akm");
                        audioStream = new SoundPlayer(Properties.Resources.akm);
                    }
                    else
                    {
                        weaponIndex++;
                        goto GUNSWITCH;
                    }
                    break;
                case 1:
                    if (checkBox5.Checked)
                    {
                        currentGun = "ump9";
                        textBox7.Text = "ump9";
                        NoReciolScript.selectGun(true, false, false, false, false, false, false, "ump9");
                        audioStream = new SoundPlayer(Properties.Resources.ump9);
                    }
                    else
                    {
                        weaponIndex++;
                        goto GUNSWITCH;
                    }
                    break;
                case 2:
                    if (checkBox6.Checked)
                    {
                        currentGun = "m416";
                        textBox7.Text = "m416";
                        NoReciolScript.selectGun(false, false, false, true, false, false, false, "m416");
                        audioStream = new SoundPlayer(Properties.Resources.m416);
                    }
                    else
                    {
                        weaponIndex++;
                        goto GUNSWITCH;
                    }

                    break;
                case 3:
                    if (checkBox8.Checked)
                    {
                        currentGun = "m16a4";
                        textBox7.Text = "m16a4";
                        NoReciolScript.selectGun(false, false, true, false, false, false, false, "m16a4");
                        audioStream = new SoundPlayer(Properties.Resources.m16a4);
                    }
                    else
                    {
                        weaponIndex++;
                        goto GUNSWITCH;
                    }

                    break;
                case 4:
                    if (checkBox7.Checked)
                    {
                        currentGun = "scarl";
                        textBox7.Text = "scarl";
                        NoReciolScript.selectGun(false, false, false, false, true, false, false, "scarl");
                        audioStream = new SoundPlayer(Properties.Resources.scarl);
                    }
                    else
                    {
                        weaponIndex++;
                        goto GUNSWITCH;
                    }

                    break;
                case 5:
                    if (checkBox3.Checked)
                    {
                        currentGun = "uzi";
                        textBox7.Text = "uzi";
                        NoReciolScript.selectGun(false, false, false, false, false, true, false, "uzi");
                        audioStream = new SoundPlayer(Properties.Resources.uzi);
                    }
                    else
                    {
                        weaponIndex++;
                        goto GUNSWITCH;
                    }

                    break;
                case 6:
                    if (checkBox9.Checked)
                    {
                        currentGun = "qbz";
                        textBox7.Text = "qbz";
                        NoReciolScript.selectGun(false, false, false, false, false,  false, true, "qbz");
                        audioStream = new SoundPlayer(Properties.Resources.qbz);
                    }
                    else
                    {
                        weaponIndex++;
                        goto GUNSWITCH;
                    }

                    break;
                case 7:
                    weaponIndex = 0;
                    atleastOneGunOn();
                    goto GUNSWITCH;
            }
            audioStream.Play();
            await PutTaskDelay(3000);
            if (temp == weaponIndex)
            {
                NoReciolScript.killLogiDriver();
                NoReciolScript.updateProfile();
                NoReciolScript.startLogiDriver();

                await PutTaskDelay(3000);
                bool procFound = false;
                Process[] proc = Process.GetProcesses();
                do
                {
                    foreach (Process item in proc)
                    {
                        if (item.ProcessName.Equals("LCore"))
                        {
                            procFound = true;
                            InputSimulator inputSimulator = new InputSimulator();
                            if (!Control.IsKeyLocked(Keys.NumLock))
                                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.NUMLOCK);
                            if (!Control.IsKeyLocked(Keys.Scroll))
                                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.SCROLL);
                            synthesizer.SpeakAsync("ready");
                        }
                    }

                } while (!procFound);
            }
        }

        //update settings 
        private void updateSettings()
        {
            if (guiComplete)
            {
                Properties.Settings.Default.closeWithPUBG = (bool)checkBox2.Checked;
                Properties.Settings.Default.mouseActivate = textBox1.Text;
                Properties.Settings.Default.openWithPUBG = (bool)checkBox1.Checked;
                Properties.Settings.Default.gunDisableKey = comboBox2.Text;
                Properties.Settings.Default.scopeFourSen = textBox6.Text;
                Properties.Settings.Default.gunSwitchKey = comboBox1.Text;
                Properties.Settings.Default.mouseDisable = textBox2.Text;
                Properties.Settings.Default.verticalSen = textBox3.Text;
                Properties.Settings.Default.targetSen = textBox4.Text;
                Properties.Settings.Default.scopeSen = textBox5.Text;
                Properties.Settings.Default.Save();
            }
        }

        //checks if all guns are disabled ifso all enabled
        private void atleastOneGunOn()
        {
            if (checkBox3.Checked == false && checkBox4.Checked == false && checkBox5.Checked == false && checkBox6.Checked == false && checkBox7.Checked == false && checkBox8.Checked == false)
            {
                checkBox3.Checked = true;
                checkBox4.Checked = true;
                checkBox5.Checked = true;
                checkBox6.Checked = true;
                checkBox7.Checked = true;
                checkBox8.Checked = true;
                checkBox9.Checked = true;
            }
        }


        //============================================================================================================================
        //ON EVENTS
        //============================================================================================================================

        //on keyboard press
        private void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            if (waitingForInput)
            {
                if (comboBox1.Text.Equals("Press to select"))
                {
                    comboBox1.Text = e.KeyData.ToString();
                }
                if (comboBox2.Text.Equals("Press to select"))
                {
                    comboBox2.Text = e.KeyData.ToString();
                }
                waitingForInput = false;
                updateSettings();
            }
            else
            {
                if (e.KeyData.ToString().ToLower().Equals(Properties.Settings.Default.gunSwitchKey.ToString().ToLower()) && started)
                {
                    switchGunRecoilAsync(null);
                }
                if (e.KeyData.ToString().ToLower().Equals(Properties.Settings.Default.gunDisableKey.ToString().ToLower()) && started)
                {
                    gunDisabledAsync();
                }
                if (e.KeyData.ToString().ToLower().Equals("F6".ToLower().ToString()) && started)
                {
                    synthesizer.SpeakAsync("set");
                    pimaryGun = currentGun;
                }
                if (e.KeyData.ToString().ToLower().Equals("f7".ToLower().ToString()) && started)
                {
                    synthesizer.SpeakAsync("set");
                    secondaryGun = currentGun;  
                }
                if (e.KeyData.ToString().ToLower().Equals("d1".ToLower().ToString()) && started)
                {
                    if (currentGun != pimaryGun)
                    {
                        switchGunRecoilAsync(pimaryGun);
                    }
                    else
                    {
                        synthesizer.SpeakAsync("ready");
                    }

                }
                if (e.KeyData.ToString().ToLower().Equals("d2".ToLower().ToString()) && started)
                {
                    if (currentGun != secondaryGun)
                    {
                        switchGunRecoilAsync(secondaryGun);
                    }
                    else
                    {
                        synthesizer.SpeakAsync("ready");
                    }
                }
            }
        }

        //on mouse click
        private void OnMouseClick(object sender, MouseEventArgs e)
        {

            if (waitingForInput)
            {
                if (comboBox1.Text.Equals("Press to select"))
                {
                    comboBox1.Text = e.Button.ToString();
                }
                if (comboBox2.Text.Equals("Press to select"))
                {
                    comboBox2.Text = e.Button.ToString();
                }
                waitingForInput = false;
                updateSettings();
            }
            else
            {
                if (e.Button.ToString().ToLower().Equals(Properties.Settings.Default.gunSwitchKey.ToString().ToLower()) && started)
                {
                    switchGunRecoilAsync(null);
                }
                if (e.Button.ToString().ToLower().Equals(Properties.Settings.Default.gunDisableKey.ToString().ToLower()) && started)
                {
                    gunDisabledAsync();
                }
            }
        }

        //sleep
        async Task PutTaskDelay(int deley)
        {
            await Task.Delay(deley);
        }

        //when the form close 
        private async void Form1_FormClosingAsync(object sender, FormClosingEventArgs e)
        {
            if (firstClosingBlocked)
            {
                firstClosingBlocked = false;
                e.Cancel = true;
                updateSettings();
                if (!Properties.Settings.Default.profilePath.Equals(""))
                {
                    NoReciolScript.killLogiDriver();
                    NoReciolScript.resetProfileDefualt();
                    NoReciolScript.startLogiDriver();
                }
                await PutTaskDelay(1000);
                Environment.Exit(0);
            }
        }

        //update settings for on events, should probably remove this but lazy 
        private void updateSettings(object sender, EventArgs e)
        {
            updateSettings();
        }



        //select button
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog FBD = new OpenFileDialog();
            FBD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Logitech\\Logitech Gaming Software\\profiles";
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.profilePath = FBD.FileName;
                Properties.Settings.Default.Save();
            }
            NoReciolScript.checkProfile();
            NoReciolScript.killLogiDriver();
            NoReciolScript.makeProfileDefualt();
            NoReciolScript.startLogiDriver();
        }

        //start button
        private void button2_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.profilePath.Equals(""))
            {
                started = true;

                if (!Properties.Settings.Default.profilePath.Equals(""))
                {
                    NoReciolScript.killLogiDriver();
                    NoReciolScript.makeProfileDefualt();
                    NoReciolScript.startLogiDriver();
                }

                if (Properties.Settings.Default.openWithPUBG)
                {
                    Process.Start("steam://rungameid/578080");
                }

                //WindowState = FormWindowState.Minimized;
                weaponIndex = -1;
                switchGunRecoilAsync(null);
                button2.Text = "Running";
            }          
        }

        //update script button
        private void button3_Click(object sender, EventArgs e)
        {
            Form2 F2 = new Form2();
            F2.Show();
        }

        //gun disable
        private async void comboBox2_SelectedIndexChangedAsync(object sender, EventArgs e)
        {
            if (comboBox2.Text.Equals("Press to select"))
            {
                MessageBox.Show("Press ok then press the button/key in 5 seconds!", "Set Key", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await PutTaskDelay(3500);
                waitingForInput = true;
            }
            updateSettings();
        }

        //gun switch 
        private async void comboBox1_SelectedIndexChangedAsync(object sender, EventArgs e)
        {
            if (comboBox1.Text.Equals("Press to select"))
            {
                MessageBox.Show("Press ok then press the button/key in 5 seconds!", "Set Key", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await PutTaskDelay(3500);
                waitingForInput = true;
            }
            updateSettings();
        }

        //only turned on if closed with pubg is checked
        //closes after pubg closes
        private void timer1_Tick(object sender, EventArgs e)
        {
            bool pubgClosed = true;
            Process[] proc = Process.GetProcesses();
            foreach (Process item in proc)
            {
                if (item.ProcessName.Equals("TslGame"))
                {
                    pubgStarted = true;
                    pubgClosed = false;
                }
            }
            if (pubgStarted && pubgClosed)
            {
                updateSettings();
                if (!Properties.Settings.Default.profilePath.Equals(""))
                {
                    NoReciolScript.killLogiDriver();
                    NoReciolScript.resetProfileDefualt();
                    NoReciolScript.startLogiDriver();
                }
                Environment.Exit(0);
            }
        }
    }
}
