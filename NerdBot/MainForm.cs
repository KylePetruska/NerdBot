using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Collections.Concurrent;

namespace NerdBot
{
    public partial class formMain : Form
    {
        public Irc chat;
        public Database db;
        public Stats stats;
        private Logger _logger;

        public static formMain mainForm;
        private SettingsForm settingsForm = new SettingsForm();

        private string _channel;

        public bool FollowAlertOn { get; set; }
        public bool JoinGreetOn { get; set; }
        public bool AutoBroadcastOn { get; set; }
        public bool AutoBroadcastInterval { get; set; }
        public bool BanASCII { get; set; }
        public bool BanLinks { get; set; }
        public bool EmoteLimit { get; set; }

        public formMain()
        {
            InitializeComponent();
            FollowAlertOn = Properties.Settings.Default.followAlertOn;
            JoinGreetOn = Properties.Settings.Default.joinGreetOn;
            AutoBroadcastOn = Properties.Settings.Default.autoBroadcastOn;
            BanLinks = Properties.Settings.Default.banLinks;
            Channel = Properties.Settings.Default.channel;
        }

        public string Channel 
        {
            get { return _channel; }
            set
            {
                if (value.StartsWith("#"))
                {
                    _channel = value.ToLower();
                }
                else
                {
                    _channel = "#" + value.ToLower();
                }
            }
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            if (mainForm == null)
                mainForm = this;

            _logger = new Logger("MainForm");

            comboBoxAutoBroadcastInterval.SelectedItem = Properties.Settings.Default.autoBroadcastInterval.ToString();
            comboBoxGreetFrequency.SelectedIndex = Properties.Settings.Default.joinGreetFrequency;

            if (AutoBroadcastOn)
            {
                buttonAutoBroadcast.Text = "On";
                buttonAutoBroadcast.ForeColor = Color.Green;
            }
            else
            {
                buttonAutoBroadcast.Text = "Off";
                buttonAutoBroadcast.ForeColor = Color.Red;
            }

            if (BanLinks)
            {
                labelBanLinks.Text = "On";
                labelBanLinks.ForeColor = Color.Green;
            }
            else
            {
                labelBanLinks.Text = "Off";
                labelBanLinks.ForeColor = Color.Red;
            }

            EmoteLimit = true;

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            labelStatusState.Text = "Connecting...";
            labelStatusState.ForeColor = Color.Black;

            //Connect to DB
            db = new Database();

            //Connect to IRC
            if (db.myDB.State == ConnectionState.Open)
            {
                chat = new Irc();
            }
            else
            {
                labelStatusState.Text = "DB Error";
                labelStatusState.ForeColor = Color.Red;
                return;
            }

            if (!chat.irc.Connected)
            {
                labelStatusState.Text = "IRC Error";
                labelStatusState.ForeColor = Color.Red;

                if (db != null)
                {
                    db.myDB.Close();
                    db = null;
                }
            }
            
            if (chat.irc.Connected && db.myDB.State == ConnectionState.Open)
            {
                labelStatusState.Text = "Connected";
                labelStatusState.ForeColor = Color.Green;
                stats = new Stats();
            }

        }

        public void SetCurrentFollowers(string num)
        {
            //Need to do this since stats is on a different thread
            if (labelCurFollowers.InvokeRequired)
            {
                Invoke(new Action<string>(SetCurrentFollowers), num);
            }
            else
                this.labelCurFollowers.Text = num;
        }

        public void SetNewFollowers(int num)
        {
            //Need to do this since stats is on a different thread
            if (labelNumNewFollowers.InvokeRequired)
            {
                Invoke(new Action<int>(SetNewFollowers), num);
            }
            else
                this.labelNumNewFollowers.Text = num.ToString();
        }

        private void buttonAutoBroadcast_Click(object sender, EventArgs e)
        {
            AutoBroadcastOn = !AutoBroadcastOn;

            if (AutoBroadcastOn)
            {
                buttonAutoBroadcast.Text = "On";
                buttonAutoBroadcast.ForeColor = Color.Green;
            }
            else
            {
                buttonAutoBroadcast.Text = "Off";
                buttonAutoBroadcast.ForeColor = Color.Red;
            }

            Properties.Settings.Default.autoBroadcastOn = AutoBroadcastOn;
            Properties.Settings.Default.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonBanLinks_Click(object sender, EventArgs e)
        {
            BanLinks = !BanLinks;

            if (BanLinks)
            {
                labelBanLinks.Text = "On";
                labelBanLinks.ForeColor = Color.Green;
            }
            else
            {
                labelBanLinks.Text = "Off";
                labelBanLinks.ForeColor = Color.Red;
            }

            Properties.Settings.Default.banLinks = BanLinks;
            Properties.Settings.Default.Save();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (chat == null)
            {
                settingsForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("You must first stop the bot before changing any settings!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }       
        }

        private void comboBoxAutoBroadcastInterval_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int interval = Convert.ToInt16(comboBoxAutoBroadcastInterval.SelectedItem);
            Properties.Settings.Default.autoBroadcastInterval = interval;
            Properties.Settings.Default.Save();

            if (chat != null)
            {
                chat.ChangeAutoBroadcastInterval(interval);
            } 
        }
    }
}
