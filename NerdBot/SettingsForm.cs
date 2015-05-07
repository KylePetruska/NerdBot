using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NerdBot
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            textBoxChannel.Text = Properties.Settings.Default.channel;
            textBoxName.Text = Properties.Settings.Default.name;
            textBoxOAuth.Text = Properties.Settings.Default.oauthKey;
            textBoxIP.Text = Properties.Settings.Default.databaseIP;
            textBoxPass.Text = Properties.Settings.Default.databasePass;
            textBoxDBName.Text = Properties.Settings.Default.databaseName;
            textBoxUser.Text = Properties.Settings.Default.databaseUser;
        }

        private void close_Click(object sender, EventArgs e)
        {
            Save();
            this.Close();
        }

        private void Save()
        {
            Properties.Settings.Default.channel = textBoxChannel.Text;
            Properties.Settings.Default.name = textBoxName.Text;
            Properties.Settings.Default.oauthKey = textBoxOAuth.Text;
            Properties.Settings.Default.databaseIP = textBoxIP.Text;
            Properties.Settings.Default.databasePass = textBoxPass.Text;
            Properties.Settings.Default.databaseName = textBoxDBName.Text;
            Properties.Settings.Default.databaseUser = textBoxUser.Text;

            Properties.Settings.Default.Save();
        }

    }
}
