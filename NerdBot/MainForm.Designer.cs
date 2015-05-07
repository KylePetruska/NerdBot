namespace NerdBot
{
    partial class formMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.labelNumNewFollowers = new System.Windows.Forms.Label();
            this.labelNewFollowers = new System.Windows.Forms.Label();
            this.labelCurFollowers = new System.Windows.Forms.Label();
            this.labelCurrentFollowers = new System.Windows.Forms.Label();
            this.buttonFollowGreet = new System.Windows.Forms.Button();
            this.comboBoxGreetFrequency = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBoxAutoBroadcastInterval = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAutoBroadcast = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelBanLinks = new System.Windows.Forms.Label();
            this.labelBanASCII = new System.Windows.Forms.Label();
            this.buttonBanASCII = new System.Windows.Forms.Button();
            this.buttonBanLinks = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelStatusState = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(341, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.labelNumNewFollowers);
            this.groupBox4.Controls.Add(this.labelNewFollowers);
            this.groupBox4.Controls.Add(this.labelCurFollowers);
            this.groupBox4.Controls.Add(this.labelCurrentFollowers);
            this.groupBox4.Controls.Add(this.buttonFollowGreet);
            this.groupBox4.Location = new System.Drawing.Point(166, 124);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(166, 89);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Follower Greeting";
            // 
            // labelNumNewFollowers
            // 
            this.labelNumNewFollowers.AutoSize = true;
            this.labelNumNewFollowers.Location = new System.Drawing.Point(107, 69);
            this.labelNumNewFollowers.Name = "labelNumNewFollowers";
            this.labelNumNewFollowers.Size = new System.Drawing.Size(14, 13);
            this.labelNumNewFollowers.TabIndex = 15;
            this.labelNumNewFollowers.Text = "#";
            // 
            // labelNewFollowers
            // 
            this.labelNewFollowers.AutoSize = true;
            this.labelNewFollowers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelNewFollowers.Location = new System.Drawing.Point(6, 69);
            this.labelNewFollowers.Name = "labelNewFollowers";
            this.labelNewFollowers.Size = new System.Drawing.Size(100, 13);
            this.labelNewFollowers.TabIndex = 14;
            this.labelNewFollowers.Text = "Followers (Session):";
            this.labelNewFollowers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCurFollowers
            // 
            this.labelCurFollowers.AutoSize = true;
            this.labelCurFollowers.Location = new System.Drawing.Point(107, 52);
            this.labelCurFollowers.Name = "labelCurFollowers";
            this.labelCurFollowers.Size = new System.Drawing.Size(14, 13);
            this.labelCurFollowers.TabIndex = 13;
            this.labelCurFollowers.Text = "#";
            // 
            // labelCurrentFollowers
            // 
            this.labelCurrentFollowers.AutoSize = true;
            this.labelCurrentFollowers.Location = new System.Drawing.Point(6, 52);
            this.labelCurrentFollowers.Name = "labelCurrentFollowers";
            this.labelCurrentFollowers.Size = new System.Drawing.Size(87, 13);
            this.labelCurrentFollowers.TabIndex = 12;
            this.labelCurrentFollowers.Text = "Followers (Total):";
            this.labelCurrentFollowers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonFollowGreet
            // 
            this.buttonFollowGreet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFollowGreet.ForeColor = System.Drawing.Color.Green;
            this.buttonFollowGreet.Location = new System.Drawing.Point(46, 19);
            this.buttonFollowGreet.Name = "buttonFollowGreet";
            this.buttonFollowGreet.Size = new System.Drawing.Size(75, 23);
            this.buttonFollowGreet.TabIndex = 11;
            this.buttonFollowGreet.Text = "On";
            this.buttonFollowGreet.UseVisualStyleBackColor = true;
            // 
            // comboBoxGreetFrequency
            // 
            this.comboBoxGreetFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGreetFrequency.FormattingEnabled = true;
            this.comboBoxGreetFrequency.Items.AddRange(new object[] {
            "Everytime",
            "Daily",
            "Once"});
            this.comboBoxGreetFrequency.Location = new System.Drawing.Point(73, 54);
            this.comboBoxGreetFrequency.Name = "comboBoxGreetFrequency";
            this.comboBoxGreetFrequency.Size = new System.Drawing.Size(71, 21);
            this.comboBoxGreetFrequency.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Frequency:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxAutoBroadcastInterval);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.buttonAutoBroadcast);
            this.groupBox3.Location = new System.Drawing.Point(166, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(167, 91);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "AutoBroadcast";
            // 
            // comboBoxAutoBroadcastInterval
            // 
            this.comboBoxAutoBroadcastInterval.DisplayMember = "fdfsfd";
            this.comboBoxAutoBroadcastInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAutoBroadcastInterval.FormattingEnabled = true;
            this.comboBoxAutoBroadcastInterval.Items.AddRange(new object[] {
            "1",
            "2",
            "5",
            "10",
            "15",
            "20"});
            this.comboBoxAutoBroadcastInterval.Location = new System.Drawing.Point(110, 53);
            this.comboBoxAutoBroadcastInterval.Name = "comboBoxAutoBroadcastInterval";
            this.comboBoxAutoBroadcastInterval.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.comboBoxAutoBroadcastInterval.Size = new System.Drawing.Size(45, 21);
            this.comboBoxAutoBroadcastInterval.TabIndex = 2;
            this.comboBoxAutoBroadcastInterval.SelectionChangeCommitted += new System.EventHandler(this.comboBoxAutoBroadcastInterval_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Interval (In Minutes):";
            // 
            // buttonAutoBroadcast
            // 
            this.buttonAutoBroadcast.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAutoBroadcast.ForeColor = System.Drawing.Color.Green;
            this.buttonAutoBroadcast.Location = new System.Drawing.Point(46, 20);
            this.buttonAutoBroadcast.Name = "buttonAutoBroadcast";
            this.buttonAutoBroadcast.Size = new System.Drawing.Size(75, 23);
            this.buttonAutoBroadcast.TabIndex = 0;
            this.buttonAutoBroadcast.Text = "On";
            this.buttonAutoBroadcast.UseVisualStyleBackColor = true;
            this.buttonAutoBroadcast.Click += new System.EventHandler(this.buttonAutoBroadcast_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelBanLinks);
            this.groupBox2.Controls.Add(this.labelBanASCII);
            this.groupBox2.Controls.Add(this.buttonBanASCII);
            this.groupBox2.Controls.Add(this.buttonBanLinks);
            this.groupBox2.Location = new System.Drawing.Point(12, 219);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(148, 80);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Chat";
            // 
            // labelBanLinks
            // 
            this.labelBanLinks.AutoSize = true;
            this.labelBanLinks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBanLinks.ForeColor = System.Drawing.Color.Red;
            this.labelBanLinks.Location = new System.Drawing.Point(95, 56);
            this.labelBanLinks.Name = "labelBanLinks";
            this.labelBanLinks.Size = new System.Drawing.Size(24, 13);
            this.labelBanLinks.TabIndex = 10;
            this.labelBanLinks.Text = "Off";
            // 
            // labelBanASCII
            // 
            this.labelBanASCII.AutoSize = true;
            this.labelBanASCII.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBanASCII.ForeColor = System.Drawing.Color.Red;
            this.labelBanASCII.Location = new System.Drawing.Point(95, 26);
            this.labelBanASCII.Name = "labelBanASCII";
            this.labelBanASCII.Size = new System.Drawing.Size(28, 13);
            this.labelBanASCII.TabIndex = 9;
            this.labelBanASCII.Text = "NYI";
            // 
            // buttonBanASCII
            // 
            this.buttonBanASCII.Location = new System.Drawing.Point(5, 21);
            this.buttonBanASCII.Name = "buttonBanASCII";
            this.buttonBanASCII.Size = new System.Drawing.Size(75, 23);
            this.buttonBanASCII.TabIndex = 7;
            this.buttonBanASCII.Text = "Ban ASCII";
            this.buttonBanASCII.UseVisualStyleBackColor = true;
            // 
            // buttonBanLinks
            // 
            this.buttonBanLinks.Location = new System.Drawing.Point(5, 51);
            this.buttonBanLinks.Name = "buttonBanLinks";
            this.buttonBanLinks.Size = new System.Drawing.Size(75, 23);
            this.buttonBanLinks.TabIndex = 8;
            this.buttonBanLinks.Text = "Ban Links";
            this.buttonBanLinks.UseVisualStyleBackColor = true;
            this.buttonBanLinks.Click += new System.EventHandler(this.buttonBanLinks_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonStart);
            this.groupBox1.Controls.Add(this.labelStatus);
            this.groupBox1.Controls.Add(this.labelStatusState);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(148, 90);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(37, 52);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(6, 29);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(40, 13);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Status:";
            // 
            // labelStatusState
            // 
            this.labelStatusState.AutoSize = true;
            this.labelStatusState.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatusState.ForeColor = System.Drawing.Color.Red;
            this.labelStatusState.Location = new System.Drawing.Point(52, 29);
            this.labelStatusState.Name = "labelStatusState";
            this.labelStatusState.Size = new System.Drawing.Size(58, 13);
            this.labelStatusState.TabIndex = 1;
            this.labelStatusState.Text = "OFFLINE";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.comboBoxGreetFrequency);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Location = new System.Drawing.Point(12, 124);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(148, 89);
            this.groupBox5.TabIndex = 20;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Join Greeting";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Green;
            this.button1.Location = new System.Drawing.Point(37, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "On";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Location = new System.Drawing.Point(166, 219);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(166, 80);
            this.groupBox6.TabIndex = 21;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Stats";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(107, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "#";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Location = new System.Drawing.Point(6, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Chatters:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(107, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "#";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Viewers:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(341, 307);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "formMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nerdbot v0.01";
            this.Load += new System.EventHandler(this.formMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBoxGreetFrequency;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label labelNumNewFollowers;
        private System.Windows.Forms.Label labelNewFollowers;
        public System.Windows.Forms.Label labelCurFollowers;
        private System.Windows.Forms.Label labelCurrentFollowers;
        private System.Windows.Forms.Button buttonFollowGreet;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBoxAutoBroadcastInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAutoBroadcast;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonBanASCII;
        private System.Windows.Forms.Button buttonBanLinks;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelStatusState;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox6;
        public System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelBanLinks;
        private System.Windows.Forms.Label labelBanASCII;
    }
}

