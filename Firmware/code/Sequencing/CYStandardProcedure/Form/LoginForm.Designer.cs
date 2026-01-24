namespace CYStandardProcedure
{
    partial class LoginForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.txt_Password = new System.Windows.Forms.TextBox();
            this.txt_ID = new System.Windows.Forms.TextBox();
            this.roundPanel3 = new CYCustomControl.RoundPanel(this.components);
            this.btn_Manager = new CYCustomControl.RoundButton();
            this.btn_Engineer = new CYCustomControl.RoundButton();
            this.btn_Operator = new CYCustomControl.RoundButton();
            this.roundPanel2 = new CYCustomControl.RoundPanel(this.components);
            this.Rbtn_camstaticrun = new CYCustomControl.RoundButton();
            this.Rbtn_camdycrun = new CYCustomControl.RoundButton();
            this.Rbtn_calibrun = new CYCustomControl.RoundButton();
            this.Rbtn_dryrun = new CYCustomControl.RoundButton();
            this.Rbtn_grrrun = new CYCustomControl.RoundButton();
            this.Rbtn_cpkrun = new CYCustomControl.RoundButton();
            this.Rbtn_normalrun = new CYCustomControl.RoundButton();
            this.roundPanel1 = new CYCustomControl.RoundPanel(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Authority = new CYCustomControl.RoundButton();
            this.btn_Loginout = new CYCustomControl.RoundButton();
            this.btn_Login = new CYCustomControl.RoundButton();
            this.label8 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.roundPanel3.SuspendLayout();
            this.roundPanel2.SuspendLayout();
            this.roundPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_Password
            // 
            this.txt_Password.Location = new System.Drawing.Point(118, 34);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.PasswordChar = '*';
            this.txt_Password.Size = new System.Drawing.Size(192, 23);
            this.txt_Password.TabIndex = 1;
            this.txt_Password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tex_password_KeyPress);
            // 
            // txt_ID
            // 
            this.txt_ID.Location = new System.Drawing.Point(118, 70);
            this.txt_ID.Name = "txt_ID";
            this.txt_ID.PasswordChar = '*';
            this.txt_ID.Size = new System.Drawing.Size(192, 23);
            this.txt_ID.TabIndex = 41;
            this.txt_ID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tex_ID_KeyPress);
            // 
            // roundPanel3
            // 
            this.roundPanel3._setRoundRadius = 8;
            this.roundPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.roundPanel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel3.Controls.Add(this.btn_Manager);
            this.roundPanel3.Controls.Add(this.btn_Engineer);
            this.roundPanel3.Controls.Add(this.btn_Operator);
            this.roundPanel3.Location = new System.Drawing.Point(9, 9);
            this.roundPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel3.Name = "roundPanel3";
            this.roundPanel3.Size = new System.Drawing.Size(884, 733);
            this.roundPanel3.TabIndex = 0;
            // 
            // btn_Manager
            // 
            this.btn_Manager.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Manager.BackColor = System.Drawing.Color.Transparent;
            this.btn_Manager.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.btn_Manager.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.btn_Manager.FlatAppearance.BorderSize = 0;
            this.btn_Manager.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Manager.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btn_Manager.ImageHeight = 80;
            this.btn_Manager.ImageWidth = 80;
            this.btn_Manager.Location = new System.Drawing.Point(272, 450);
            this.btn_Manager.Name = "btn_Manager";
            this.btn_Manager.Radius = 24;
            this.btn_Manager.Size = new System.Drawing.Size(341, 74);
            this.btn_Manager.SpliteButtonWidth = 18;
            this.btn_Manager.TabIndex = 24;
            this.btn_Manager.Tag = "2";
            this.btn_Manager.Text = "Manager";
            this.btn_Manager.UseVisualStyleBackColor = false;
            this.btn_Manager.Click += new System.EventHandler(this.Click_AdminBtn);
            // 
            // btn_Engineer
            // 
            this.btn_Engineer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Engineer.BackColor = System.Drawing.Color.Transparent;
            this.btn_Engineer.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.btn_Engineer.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.btn_Engineer.FlatAppearance.BorderSize = 0;
            this.btn_Engineer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Engineer.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btn_Engineer.ImageHeight = 80;
            this.btn_Engineer.ImageWidth = 80;
            this.btn_Engineer.Location = new System.Drawing.Point(272, 329);
            this.btn_Engineer.Name = "btn_Engineer";
            this.btn_Engineer.Radius = 24;
            this.btn_Engineer.Size = new System.Drawing.Size(341, 74);
            this.btn_Engineer.SpliteButtonWidth = 18;
            this.btn_Engineer.TabIndex = 23;
            this.btn_Engineer.Tag = "1";
            this.btn_Engineer.Text = "Engineer";
            this.btn_Engineer.UseVisualStyleBackColor = false;
            this.btn_Engineer.Click += new System.EventHandler(this.Click_AdminBtn);
            // 
            // btn_Operator
            // 
            this.btn_Operator.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Operator.BackColor = System.Drawing.Color.Transparent;
            this.btn_Operator.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.btn_Operator.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.btn_Operator.FlatAppearance.BorderSize = 0;
            this.btn_Operator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Operator.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btn_Operator.ImageHeight = 80;
            this.btn_Operator.ImageWidth = 80;
            this.btn_Operator.Location = new System.Drawing.Point(272, 208);
            this.btn_Operator.Name = "btn_Operator";
            this.btn_Operator.Radius = 24;
            this.btn_Operator.Size = new System.Drawing.Size(341, 74);
            this.btn_Operator.SpliteButtonWidth = 18;
            this.btn_Operator.TabIndex = 22;
            this.btn_Operator.Tag = "0";
            this.btn_Operator.Text = "Operator";
            this.btn_Operator.UseVisualStyleBackColor = false;
            this.btn_Operator.Click += new System.EventHandler(this.Click_AdminBtn);
            // 
            // roundPanel2
            // 
            this.roundPanel2._setRoundRadius = 8;
            this.roundPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.roundPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.roundPanel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel2.Controls.Add(this.Rbtn_camstaticrun);
            this.roundPanel2.Controls.Add(this.Rbtn_camdycrun);
            this.roundPanel2.Controls.Add(this.Rbtn_calibrun);
            this.roundPanel2.Controls.Add(this.Rbtn_dryrun);
            this.roundPanel2.Controls.Add(this.Rbtn_grrrun);
            this.roundPanel2.Controls.Add(this.Rbtn_cpkrun);
            this.roundPanel2.Controls.Add(this.Rbtn_normalrun);
            this.roundPanel2.Location = new System.Drawing.Point(906, 321);
            this.roundPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel2.Name = "roundPanel2";
            this.roundPanel2.Size = new System.Drawing.Size(336, 421);
            this.roundPanel2.TabIndex = 1;
            // 
            // Rbtn_camstaticrun
            // 
            this.Rbtn_camstaticrun.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Rbtn_camstaticrun.BackColor = System.Drawing.Color.Transparent;
            this.Rbtn_camstaticrun.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.Rbtn_camstaticrun.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.Rbtn_camstaticrun.FlatAppearance.BorderSize = 0;
            this.Rbtn_camstaticrun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rbtn_camstaticrun.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.Rbtn_camstaticrun.ImageHeight = 80;
            this.Rbtn_camstaticrun.ImageWidth = 80;
            this.Rbtn_camstaticrun.Location = new System.Drawing.Point(59, 297);
            this.Rbtn_camstaticrun.Name = "Rbtn_camstaticrun";
            this.Rbtn_camstaticrun.Radius = 24;
            this.Rbtn_camstaticrun.Size = new System.Drawing.Size(247, 45);
            this.Rbtn_camstaticrun.SpliteButtonWidth = 18;
            this.Rbtn_camstaticrun.TabIndex = 27;
            this.Rbtn_camstaticrun.Text = "Cam Statis Mode";
            this.Rbtn_camstaticrun.UseVisualStyleBackColor = false;
            this.Rbtn_camstaticrun.Visible = false;
            this.Rbtn_camstaticrun.Click += new System.EventHandler(this.Rbtn_camstaticrun_Click);
            // 
            // Rbtn_camdycrun
            // 
            this.Rbtn_camdycrun.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Rbtn_camdycrun.BackColor = System.Drawing.Color.Transparent;
            this.Rbtn_camdycrun.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.Rbtn_camdycrun.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.Rbtn_camdycrun.FlatAppearance.BorderSize = 0;
            this.Rbtn_camdycrun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rbtn_camdycrun.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.Rbtn_camdycrun.ImageHeight = 80;
            this.Rbtn_camdycrun.ImageWidth = 80;
            this.Rbtn_camdycrun.Location = new System.Drawing.Point(59, 353);
            this.Rbtn_camdycrun.Name = "Rbtn_camdycrun";
            this.Rbtn_camdycrun.Radius = 24;
            this.Rbtn_camdycrun.Size = new System.Drawing.Size(247, 45);
            this.Rbtn_camdycrun.SpliteButtonWidth = 18;
            this.Rbtn_camdycrun.TabIndex = 28;
            this.Rbtn_camdycrun.Text = "Cam Dynamic Mode";
            this.Rbtn_camdycrun.UseVisualStyleBackColor = false;
            this.Rbtn_camdycrun.Visible = false;
            this.Rbtn_camdycrun.Click += new System.EventHandler(this.Rbtn_camdycrun_Click);
            // 
            // Rbtn_calibrun
            // 
            this.Rbtn_calibrun.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Rbtn_calibrun.BackColor = System.Drawing.Color.Transparent;
            this.Rbtn_calibrun.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.Rbtn_calibrun.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.Rbtn_calibrun.FlatAppearance.BorderSize = 0;
            this.Rbtn_calibrun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rbtn_calibrun.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.Rbtn_calibrun.ImageHeight = 80;
            this.Rbtn_calibrun.ImageWidth = 80;
            this.Rbtn_calibrun.Location = new System.Drawing.Point(59, 129);
            this.Rbtn_calibrun.Name = "Rbtn_calibrun";
            this.Rbtn_calibrun.Radius = 24;
            this.Rbtn_calibrun.Size = new System.Drawing.Size(247, 45);
            this.Rbtn_calibrun.SpliteButtonWidth = 18;
            this.Rbtn_calibrun.TabIndex = 26;
            this.Rbtn_calibrun.Text = "上相机标定模式";
            this.Rbtn_calibrun.UseVisualStyleBackColor = false;
            this.Rbtn_calibrun.Click += new System.EventHandler(this.Rbtn_calibrun_Click);
            // 
            // Rbtn_dryrun
            // 
            this.Rbtn_dryrun.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Rbtn_dryrun.BackColor = System.Drawing.Color.Transparent;
            this.Rbtn_dryrun.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.Rbtn_dryrun.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.Rbtn_dryrun.FlatAppearance.BorderSize = 0;
            this.Rbtn_dryrun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rbtn_dryrun.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.Rbtn_dryrun.ImageHeight = 80;
            this.Rbtn_dryrun.ImageWidth = 80;
            this.Rbtn_dryrun.Location = new System.Drawing.Point(59, 73);
            this.Rbtn_dryrun.Name = "Rbtn_dryrun";
            this.Rbtn_dryrun.Radius = 24;
            this.Rbtn_dryrun.Size = new System.Drawing.Size(247, 45);
            this.Rbtn_dryrun.SpliteButtonWidth = 18;
            this.Rbtn_dryrun.TabIndex = 16;
            this.Rbtn_dryrun.Text = "空载具回收模式";
            this.Rbtn_dryrun.UseVisualStyleBackColor = false;
            this.Rbtn_dryrun.Click += new System.EventHandler(this.Rbtn_dryrun_Click);
            // 
            // Rbtn_grrrun
            // 
            this.Rbtn_grrrun.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Rbtn_grrrun.BackColor = System.Drawing.Color.Transparent;
            this.Rbtn_grrrun.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.Rbtn_grrrun.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.Rbtn_grrrun.FlatAppearance.BorderSize = 0;
            this.Rbtn_grrrun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rbtn_grrrun.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.Rbtn_grrrun.ImageHeight = 80;
            this.Rbtn_grrrun.ImageWidth = 80;
            this.Rbtn_grrrun.Location = new System.Drawing.Point(59, 185);
            this.Rbtn_grrrun.Name = "Rbtn_grrrun";
            this.Rbtn_grrrun.Radius = 24;
            this.Rbtn_grrrun.Size = new System.Drawing.Size(247, 45);
            this.Rbtn_grrrun.SpliteButtonWidth = 18;
            this.Rbtn_grrrun.TabIndex = 19;
            this.Rbtn_grrrun.Text = "下相机标定模式";
            this.Rbtn_grrrun.UseVisualStyleBackColor = false;
            this.Rbtn_grrrun.Click += new System.EventHandler(this.Rbtn_cpkgrr_Click);
            // 
            // Rbtn_cpkrun
            // 
            this.Rbtn_cpkrun.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Rbtn_cpkrun.BackColor = System.Drawing.Color.Transparent;
            this.Rbtn_cpkrun.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.Rbtn_cpkrun.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.Rbtn_cpkrun.FlatAppearance.BorderSize = 0;
            this.Rbtn_cpkrun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rbtn_cpkrun.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.Rbtn_cpkrun.ImageHeight = 80;
            this.Rbtn_cpkrun.ImageWidth = 80;
            this.Rbtn_cpkrun.Location = new System.Drawing.Point(59, 241);
            this.Rbtn_cpkrun.Name = "Rbtn_cpkrun";
            this.Rbtn_cpkrun.Radius = 24;
            this.Rbtn_cpkrun.Size = new System.Drawing.Size(247, 45);
            this.Rbtn_cpkrun.SpliteButtonWidth = 18;
            this.Rbtn_cpkrun.TabIndex = 25;
            this.Rbtn_cpkrun.Text = "CPK Mode";
            this.Rbtn_cpkrun.UseVisualStyleBackColor = false;
            this.Rbtn_cpkrun.Visible = false;
            this.Rbtn_cpkrun.Click += new System.EventHandler(this.Rbtn_cpk_Click);
            // 
            // Rbtn_normalrun
            // 
            this.Rbtn_normalrun.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Rbtn_normalrun.BackColor = System.Drawing.Color.Transparent;
            this.Rbtn_normalrun.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(221)))), ((int)(((byte)(224)))));
            this.Rbtn_normalrun.BaseColorEnd = System.Drawing.Color.CornflowerBlue;
            this.Rbtn_normalrun.FlatAppearance.BorderSize = 0;
            this.Rbtn_normalrun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rbtn_normalrun.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.Rbtn_normalrun.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Rbtn_normalrun.ImageHeight = 80;
            this.Rbtn_normalrun.ImageWidth = 80;
            this.Rbtn_normalrun.Location = new System.Drawing.Point(59, 17);
            this.Rbtn_normalrun.Name = "Rbtn_normalrun";
            this.Rbtn_normalrun.Radius = 24;
            this.Rbtn_normalrun.Size = new System.Drawing.Size(247, 45);
            this.Rbtn_normalrun.SpliteButtonWidth = 18;
            this.Rbtn_normalrun.TabIndex = 15;
            this.Rbtn_normalrun.Text = "Normal Run Mode";
            this.Rbtn_normalrun.UseVisualStyleBackColor = false;
            this.Rbtn_normalrun.Click += new System.EventHandler(this.Rbtn_normalrun_Click);
            // 
            // roundPanel1
            // 
            this.roundPanel1._setRoundRadius = 8;
            this.roundPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.roundPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel1.Controls.Add(this.pictureBox1);
            this.roundPanel1.Controls.Add(this.panel2);
            this.roundPanel1.Location = new System.Drawing.Point(906, 9);
            this.roundPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel1.Name = "roundPanel1";
            this.roundPanel1.Size = new System.Drawing.Size(336, 297);
            this.roundPanel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(17, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(73, 69);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.txt_ID);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btn_Authority);
            this.panel2.Controls.Add(this.btn_Loginout);
            this.panel2.Controls.Add(this.btn_Login);
            this.panel2.Controls.Add(this.txt_Password);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Location = new System.Drawing.Point(6, 95);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(323, 196);
            this.panel2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.label1.Location = new System.Drawing.Point(31, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 20);
            this.label1.TabIndex = 40;
            this.label1.Text = "ID:";
            // 
            // btn_Authority
            // 
            this.btn_Authority.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Authority.BackColor = System.Drawing.Color.Transparent;
            this.btn_Authority.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btn_Authority.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.btn_Authority.FlatAppearance.BorderSize = 0;
            this.btn_Authority.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Authority.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btn_Authority.ImageHeight = 80;
            this.btn_Authority.ImageWidth = 80;
            this.btn_Authority.Location = new System.Drawing.Point(25, 149);
            this.btn_Authority.Name = "btn_Authority";
            this.btn_Authority.Radius = 24;
            this.btn_Authority.Size = new System.Drawing.Size(114, 36);
            this.btn_Authority.SpliteButtonWidth = 18;
            this.btn_Authority.TabIndex = 39;
            this.btn_Authority.Text = "权限管理";
            this.btn_Authority.UseVisualStyleBackColor = false;
            this.btn_Authority.Click += new System.EventHandler(this.btn_Authority_Click);
            // 
            // btn_Loginout
            // 
            this.btn_Loginout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Loginout.BackColor = System.Drawing.Color.Transparent;
            this.btn_Loginout.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btn_Loginout.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.btn_Loginout.FlatAppearance.BorderSize = 0;
            this.btn_Loginout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Loginout.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btn_Loginout.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btn_Loginout.ImageHeight = 80;
            this.btn_Loginout.ImageWidth = 80;
            this.btn_Loginout.Location = new System.Drawing.Point(185, 107);
            this.btn_Loginout.Name = "btn_Loginout";
            this.btn_Loginout.Radius = 24;
            this.btn_Loginout.Size = new System.Drawing.Size(115, 36);
            this.btn_Loginout.SpliteButtonWidth = 18;
            this.btn_Loginout.TabIndex = 38;
            this.btn_Loginout.Text = "退出登录";
            this.btn_Loginout.UseVisualStyleBackColor = false;
            this.btn_Loginout.Click += new System.EventHandler(this.btn_loginout_Click);
            // 
            // btn_Login
            // 
            this.btn_Login.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Login.BackColor = System.Drawing.Color.Transparent;
            this.btn_Login.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btn_Login.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.btn_Login.FlatAppearance.BorderSize = 0;
            this.btn_Login.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Login.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btn_Login.ImageHeight = 80;
            this.btn_Login.ImageWidth = 80;
            this.btn_Login.Location = new System.Drawing.Point(24, 107);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Radius = 24;
            this.btn_Login.Size = new System.Drawing.Size(115, 36);
            this.btn_Login.SpliteButtonWidth = 18;
            this.btn_Login.TabIndex = 10;
            this.btn_Login.Text = "登录";
            this.btn_Login.UseVisualStyleBackColor = false;
            this.btn_Login.Click += new System.EventHandler(this.btn_login_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.label8.Location = new System.Drawing.Point(31, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 20);
            this.label8.TabIndex = 0;
            this.label8.Text = "Password:";
            // 
            // timer1
            // 
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1251, 751);
            this.Controls.Add(this.roundPanel3);
            this.Controls.Add(this.roundPanel2);
            this.Controls.Add(this.roundPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.roundPanel3.ResumeLayout(false);
            this.roundPanel2.ResumeLayout(false);
            this.roundPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private CYCustomControl.RoundPanel roundPanel2;
        private CYCustomControl.RoundPanel roundPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private CYCustomControl.RoundButton btn_Authority;
        private CYCustomControl.RoundButton btn_Loginout;
        private CYCustomControl.RoundButton btn_Login;
        private System.Windows.Forms.TextBox txt_Password;
        private System.Windows.Forms.Label label8;
        private CYCustomControl.RoundButton Rbtn_dryrun;
        private CYCustomControl.RoundButton Rbtn_normalrun;
        private System.Windows.Forms.TextBox txt_ID;
        private System.Windows.Forms.Label label1;
        private CYCustomControl.RoundPanel roundPanel3;
        private CYCustomControl.RoundButton btn_Manager;
        private CYCustomControl.RoundButton btn_Engineer;
        private CYCustomControl.RoundButton btn_Operator;
        private CYCustomControl.RoundButton Rbtn_grrrun;
        private CYCustomControl.RoundButton Rbtn_cpkrun;
        private CYCustomControl.RoundButton Rbtn_calibrun;
        private CYCustomControl.RoundButton Rbtn_camstaticrun;
        private CYCustomControl.RoundButton Rbtn_camdycrun;
        private System.Windows.Forms.Timer timer1;
    }
}

