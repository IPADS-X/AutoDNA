namespace CYStandardProcedure
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lab_Alarm = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.xListBox_Run = new CYCustomControl.XListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.xListBox_NG = new CYCustomControl.XListBox();
            this.xListBox_Alarm = new CYCustomControl.XListBox();
            this.roundPanel1 = new CYCustomControl.RoundPanel(this.components);
            this.btn_stationData = new CYCustomControl.RoundButton();
            this.btn_stationMsg = new CYCustomControl.RoundButton();
            this.btn_Maininfo = new CYCustomControl.RoundButton();
            this.btn_Mainccd = new CYCustomControl.RoundButton();
            this.rbt_Show = new CYCustomControl.RoundButton();
            this.roundButton1 = new CYCustomControl.RoundButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lab_Alarm
            // 
            this.lab_Alarm.AutoSize = true;
            this.lab_Alarm.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lab_Alarm.Location = new System.Drawing.Point(928, 504);
            this.lab_Alarm.Name = "lab_Alarm";
            this.lab_Alarm.Size = new System.Drawing.Size(69, 20);
            this.lab_Alarm.TabIndex = 16;
            this.lab_Alarm.Text = "报警信息";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.tabControl1.Location = new System.Drawing.Point(925, 59);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(318, 442);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.xListBox_Run);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(310, 409);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "运行信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // xListBox_Run
            // 
            this.xListBox_Run.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xListBox_Run.Location = new System.Drawing.Point(3, 3);
            this.xListBox_Run.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.xListBox_Run.Name = "xListBox_Run";
            this.xListBox_Run.Size = new System.Drawing.Size(304, 403);
            this.xListBox_Run.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.xListBox_NG);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(310, 409);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "NG信息";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // xListBox_NG
            // 
            this.xListBox_NG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xListBox_NG.Location = new System.Drawing.Point(3, 3);
            this.xListBox_NG.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.xListBox_NG.Name = "xListBox_NG";
            this.xListBox_NG.Size = new System.Drawing.Size(304, 403);
            this.xListBox_NG.TabIndex = 0;
            // 
            // xListBox_Alarm
            // 
            this.xListBox_Alarm.Font = new System.Drawing.Font("微软雅黑", 11.5F);
            this.xListBox_Alarm.Location = new System.Drawing.Point(929, 529);
            this.xListBox_Alarm.Margin = new System.Windows.Forms.Padding(5);
            this.xListBox_Alarm.Name = "xListBox_Alarm";
            this.xListBox_Alarm.Size = new System.Drawing.Size(314, 208);
            this.xListBox_Alarm.TabIndex = 20;
            // 
            // roundPanel1
            // 
            this.roundPanel1._setRoundRadius = 12;
            this.roundPanel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.roundPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel1.Location = new System.Drawing.Point(5, 59);
            this.roundPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel1.Name = "roundPanel1";
            this.roundPanel1.Size = new System.Drawing.Size(917, 683);
            this.roundPanel1.TabIndex = 11;
            // 
            // btn_stationData
            // 
            this.btn_stationData.BackColor = System.Drawing.Color.Transparent;
            this.btn_stationData.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_stationData.BackgroundImage")));
            this.btn_stationData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_stationData.BaseColor = System.Drawing.Color.Transparent;
            this.btn_stationData.BaseColorEnd = System.Drawing.Color.Transparent;
            this.btn_stationData.FlatAppearance.BorderSize = 0;
            this.btn_stationData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_stationData.ImageHeight = 80;
            this.btn_stationData.ImageWidth = 80;
            this.btn_stationData.Location = new System.Drawing.Point(209, 6);
            this.btn_stationData.Name = "btn_stationData";
            this.btn_stationData.Radius = 24;
            this.btn_stationData.Size = new System.Drawing.Size(50, 50);
            this.btn_stationData.SpliteButtonWidth = 18;
            this.btn_stationData.TabIndex = 23;
            this.btn_stationData.UseVisualStyleBackColor = false;
            this.btn_stationData.Click += new System.EventHandler(this.roundButton1_Click);
            // 
            // btn_stationMsg
            // 
            this.btn_stationMsg.BackColor = System.Drawing.Color.Transparent;
            this.btn_stationMsg.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_stationMsg.BackgroundImage")));
            this.btn_stationMsg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_stationMsg.BaseColor = System.Drawing.Color.Transparent;
            this.btn_stationMsg.BaseColorEnd = System.Drawing.Color.Transparent;
            this.btn_stationMsg.FlatAppearance.BorderSize = 0;
            this.btn_stationMsg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_stationMsg.ImageHeight = 80;
            this.btn_stationMsg.ImageWidth = 80;
            this.btn_stationMsg.Location = new System.Drawing.Point(142, 6);
            this.btn_stationMsg.Name = "btn_stationMsg";
            this.btn_stationMsg.Radius = 24;
            this.btn_stationMsg.Size = new System.Drawing.Size(50, 50);
            this.btn_stationMsg.SpliteButtonWidth = 18;
            this.btn_stationMsg.TabIndex = 22;
            this.btn_stationMsg.UseVisualStyleBackColor = false;
            this.btn_stationMsg.Click += new System.EventHandler(this.btn_stationMsg_Click);
            // 
            // btn_Maininfo
            // 
            this.btn_Maininfo.BackColor = System.Drawing.Color.Transparent;
            this.btn_Maininfo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Maininfo.BackgroundImage")));
            this.btn_Maininfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_Maininfo.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Maininfo.BaseColorEnd = System.Drawing.Color.Transparent;
            this.btn_Maininfo.FlatAppearance.BorderSize = 0;
            this.btn_Maininfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Maininfo.ImageHeight = 80;
            this.btn_Maininfo.ImageWidth = 80;
            this.btn_Maininfo.Location = new System.Drawing.Point(75, 6);
            this.btn_Maininfo.Name = "btn_Maininfo";
            this.btn_Maininfo.Radius = 24;
            this.btn_Maininfo.Size = new System.Drawing.Size(50, 50);
            this.btn_Maininfo.SpliteButtonWidth = 18;
            this.btn_Maininfo.TabIndex = 19;
            this.btn_Maininfo.UseVisualStyleBackColor = false;
            this.btn_Maininfo.Click += new System.EventHandler(this.btn_maininfo_Click_1);
            // 
            // btn_Mainccd
            // 
            this.btn_Mainccd.BackColor = System.Drawing.Color.Transparent;
            this.btn_Mainccd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Mainccd.BackgroundImage")));
            this.btn_Mainccd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_Mainccd.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Mainccd.BaseColorEnd = System.Drawing.Color.Transparent;
            this.btn_Mainccd.FlatAppearance.BorderSize = 0;
            this.btn_Mainccd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Mainccd.ImageHeight = 80;
            this.btn_Mainccd.ImageWidth = 80;
            this.btn_Mainccd.Location = new System.Drawing.Point(8, 6);
            this.btn_Mainccd.Name = "btn_Mainccd";
            this.btn_Mainccd.Radius = 24;
            this.btn_Mainccd.Size = new System.Drawing.Size(50, 50);
            this.btn_Mainccd.SpliteButtonWidth = 18;
            this.btn_Mainccd.TabIndex = 18;
            this.btn_Mainccd.UseVisualStyleBackColor = false;
            this.btn_Mainccd.Click += new System.EventHandler(this.btn_mainccd_Click_1);
            // 
            // rbt_Show
            // 
            this.rbt_Show.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_Show.BackColor = System.Drawing.Color.Transparent;
            this.rbt_Show.BaseColor = System.Drawing.Color.Tomato;
            this.rbt_Show.BaseColorEnd = System.Drawing.Color.Tomato;
            this.rbt_Show.FlatAppearance.BorderSize = 0;
            this.rbt_Show.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_Show.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_Show.ImageHeight = 80;
            this.rbt_Show.ImageWidth = 80;
            this.rbt_Show.Location = new System.Drawing.Point(517, 11);
            this.rbt_Show.Name = "rbt_Show";
            this.rbt_Show.Radius = 24;
            this.rbt_Show.Size = new System.Drawing.Size(114, 36);
            this.rbt_Show.SpliteButtonWidth = 18;
            this.rbt_Show.TabIndex = 214;
            this.rbt_Show.Text = "单机参观模式";
            this.rbt_Show.UseVisualStyleBackColor = false;
            this.rbt_Show.Click += new System.EventHandler(this.rbt_Show_Click);
            // 
            // roundButton1
            // 
            this.roundButton1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.roundButton1.BackColor = System.Drawing.Color.Transparent;
            this.roundButton1.BaseColor = System.Drawing.Color.Tomato;
            this.roundButton1.BaseColorEnd = System.Drawing.Color.Tomato;
            this.roundButton1.FlatAppearance.BorderSize = 0;
            this.roundButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.roundButton1.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.roundButton1.ImageHeight = 80;
            this.roundButton1.ImageWidth = 80;
            this.roundButton1.Location = new System.Drawing.Point(705, 12);
            this.roundButton1.Name = "roundButton1";
            this.roundButton1.Radius = 24;
            this.roundButton1.Size = new System.Drawing.Size(114, 36);
            this.roundButton1.SpliteButtonWidth = 18;
            this.roundButton1.TabIndex = 215;
            this.roundButton1.Text = "流转参观模式";
            this.roundButton1.UseVisualStyleBackColor = false;
            this.roundButton1.Click += new System.EventHandler(this.roundButton1_Click_1);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1251, 751);
            this.Controls.Add(this.roundButton1);
            this.Controls.Add(this.rbt_Show);
            this.Controls.Add(this.btn_stationData);
            this.Controls.Add(this.btn_stationMsg);
            this.Controls.Add(this.xListBox_Alarm);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.roundPanel1);
            this.Controls.Add(this.lab_Alarm);
            this.Controls.Add(this.btn_Maininfo);
            this.Controls.Add(this.btn_Mainccd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private CYCustomControl.RoundPanel roundPanel1;
        private System.Windows.Forms.Label lab_Alarm;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private CYCustomControl.RoundButton btn_Mainccd;
        private CYCustomControl.RoundButton btn_Maininfo;
        private CYCustomControl.XListBox xListBox_Run;
        private CYCustomControl.XListBox xListBox_NG;
        private CYCustomControl.XListBox xListBox_Alarm;
        private CYCustomControl.RoundButton btn_stationMsg;
        private CYCustomControl.RoundButton btn_stationData;
        public CYCustomControl.RoundButton roundButton1;
        public CYCustomControl.RoundButton rbt_Show;
    }
}