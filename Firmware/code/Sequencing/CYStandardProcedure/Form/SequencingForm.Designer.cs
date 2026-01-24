namespace CYStandardProcedure
{
    partial class SequencingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SequencingForm));
            this.label06_A45 = new System.Windows.Forms.Label();
            this.rch_receive = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rch_send = new System.Windows.Forms.RichTextBox();
            this.rbt_connect = new CYCustomControl.RoundButton();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.richTextBox4 = new System.Windows.Forms.RichTextBox();
            this.rbt_youwu = new CYCustomControl.RoundButton();
            this.txt_RunID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.txt_num = new System.Windows.Forms.TextBox();
            this.txt_time = new System.Windows.Forms.TextBox();
            this.txt_type = new System.Windows.Forms.TextBox();
            this.txt_box = new System.Windows.Forms.TextBox();
            this.txt_speed = new System.Windows.Forms.TextBox();
            this.txt_short = new System.Windows.Forms.TextBox();
            this.txt_model = new System.Windows.Forms.TextBox();
            this.rbt_start = new CYCustomControl.RoundButton();
            this.rbt_pause = new CYCustomControl.RoundButton();
            this.rbt_continue = new CYCustomControl.RoundButton();
            this.rbt_stop = new CYCustomControl.RoundButton();
            this.rbt_check = new CYCustomControl.RoundButton();
            this.rbt_currentstate = new CYCustomControl.RoundButton();
            this.rbt_filecopy = new CYCustomControl.RoundButton();
            this.rbt_copystate = new CYCustomControl.RoundButton();
            this.rbt_jianji = new CYCustomControl.RoundButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.roundButton1 = new CYCustomControl.RoundButton();
            this.rbt_save = new CYCustomControl.RoundButton();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label06_A45
            // 
            this.label06_A45.AutoSize = true;
            this.label06_A45.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label06_A45.Location = new System.Drawing.Point(27, 520);
            this.label06_A45.Name = "label06_A45";
            this.label06_A45.Size = new System.Drawing.Size(65, 20);
            this.label06_A45.TabIndex = 90;
            this.label06_A45.Text = "查询反馈";
            // 
            // rch_receive
            // 
            this.rch_receive.Location = new System.Drawing.Point(31, 544);
            this.rch_receive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rch_receive.Name = "rch_receive";
            this.rch_receive.Size = new System.Drawing.Size(683, 122);
            this.rch_receive.TabIndex = 89;
            this.rch_receive.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(27, 377);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 92;
            this.label1.Text = "发送内容";
            // 
            // rch_send
            // 
            this.rch_send.Location = new System.Drawing.Point(31, 401);
            this.rch_send.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rch_send.Name = "rch_send";
            this.rch_send.Size = new System.Drawing.Size(683, 106);
            this.rch_send.TabIndex = 91;
            this.rch_send.Text = "";
            // 
            // rbt_connect
            // 
            this.rbt_connect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_connect.BackColor = System.Drawing.Color.Transparent;
            this.rbt_connect.BaseColor = System.Drawing.Color.Tan;
            this.rbt_connect.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_connect.FlatAppearance.BorderSize = 0;
            this.rbt_connect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_connect.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_connect.ImageHeight = 80;
            this.rbt_connect.ImageWidth = 80;
            this.rbt_connect.Location = new System.Drawing.Point(24, 31);
            this.rbt_connect.Name = "rbt_connect";
            this.rbt_connect.Radius = 24;
            this.rbt_connect.Size = new System.Drawing.Size(136, 36);
            this.rbt_connect.SpliteButtonWidth = 18;
            this.rbt_connect.TabIndex = 110;
            this.rbt_connect.Text = "检查网络连接";
            this.rbt_connect.UseVisualStyleBackColor = false;
            this.rbt_connect.Click += new System.EventHandler(this.rbt_connect_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.BackColor = System.Drawing.Color.White;
            this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox2.ForeColor = System.Drawing.Color.Green;
            this.richTextBox2.Location = new System.Drawing.Point(3, 19);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new System.Drawing.Size(254, 182);
            this.richTextBox2.TabIndex = 234;
            this.richTextBox2.Text = resources.GetString("richTextBox2.Text");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBox2);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(767, 462);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 204);
            this.groupBox1.TabIndex = 235;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "任务状态state";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.richTextBox4);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(767, 315);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(260, 135);
            this.groupBox3.TabIndex = 237;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "响应码code";
            // 
            // richTextBox4
            // 
            this.richTextBox4.BackColor = System.Drawing.Color.White;
            this.richTextBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox4.ForeColor = System.Drawing.Color.Green;
            this.richTextBox4.Location = new System.Drawing.Point(3, 19);
            this.richTextBox4.Name = "richTextBox4";
            this.richTextBox4.ReadOnly = true;
            this.richTextBox4.Size = new System.Drawing.Size(254, 113);
            this.richTextBox4.TabIndex = 234;
            this.richTextBox4.Text = "    响应码:\n       0：正常\n       9001：未知的URL\n       9002：必传参数缺失\n       9003：网络连接异常(950" +
    "2/22端口)\n       9004：检测进行中，禁止操作\n       9005：参数异常\n       9006：其他异常\n       9007：文件拷" +
    "贝进行中，禁止操作";
            // 
            // rbt_youwu
            // 
            this.rbt_youwu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_youwu.BackColor = System.Drawing.Color.Transparent;
            this.rbt_youwu.BaseColor = System.Drawing.Color.Tan;
            this.rbt_youwu.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_youwu.FlatAppearance.BorderSize = 0;
            this.rbt_youwu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_youwu.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_youwu.ImageHeight = 80;
            this.rbt_youwu.ImageWidth = 80;
            this.rbt_youwu.Location = new System.Drawing.Point(250, 31);
            this.rbt_youwu.Name = "rbt_youwu";
            this.rbt_youwu.Radius = 24;
            this.rbt_youwu.Size = new System.Drawing.Size(136, 36);
            this.rbt_youwu.SpliteButtonWidth = 18;
            this.rbt_youwu.TabIndex = 238;
            this.rbt_youwu.Text = "检查有无芯片";
            this.rbt_youwu.UseVisualStyleBackColor = false;
            this.rbt_youwu.Click += new System.EventHandler(this.rbt_youwu_Click);
            // 
            // txt_RunID
            // 
            this.txt_RunID.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_RunID.Location = new System.Drawing.Point(305, 353);
            this.txt_RunID.Name = "txt_RunID";
            this.txt_RunID.Size = new System.Drawing.Size(409, 26);
            this.txt_RunID.TabIndex = 239;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(215, 356);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 20);
            this.label2.TabIndex = 241;
            this.label2.Text = "RunID :";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(11, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.TabIndex = 242;
            this.label3.Text = "       实验名称 :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(11, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 20);
            this.label4.TabIndex = 243;
            this.label4.Text = "测序芯片类型 :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(11, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 20);
            this.label5.TabIndex = 244;
            this.label5.Text = "       样本编号 :";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(11, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 20);
            this.label6.TabIndex = 245;
            this.label6.Text = "          试剂盒 :";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(11, 156);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 20);
            this.label7.TabIndex = 246;
            this.label7.Text = "          速度 :";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(11, 192);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 20);
            this.label8.TabIndex = 247;
            this.label8.Text = "实验时长 :";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(11, 228);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 20);
            this.label9.TabIndex = 248;
            this.label9.Text = "最短读长 :";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(11, 264);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 20);
            this.label10.TabIndex = 249;
            this.label10.Text = "碱基识别模型 :";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_name
            // 
            this.txt_name.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_name.Location = new System.Drawing.Point(126, 9);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(417, 26);
            this.txt_name.TabIndex = 250;
            this.txt_name.Text = "20240316";
            this.txt_name.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_num
            // 
            this.txt_num.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_num.Location = new System.Drawing.Point(126, 81);
            this.txt_num.Name = "txt_num";
            this.txt_num.Size = new System.Drawing.Size(417, 26);
            this.txt_num.TabIndex = 251;
            this.txt_num.Text = "single001";
            this.txt_num.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_time
            // 
            this.txt_time.Enabled = false;
            this.txt_time.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_time.Location = new System.Drawing.Point(126, 189);
            this.txt_time.Name = "txt_time";
            this.txt_time.Size = new System.Drawing.Size(417, 26);
            this.txt_time.TabIndex = 252;
            this.txt_time.Text = "72";
            this.txt_time.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_type
            // 
            this.txt_type.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_type.Location = new System.Drawing.Point(126, 45);
            this.txt_type.Name = "txt_type";
            this.txt_type.Size = new System.Drawing.Size(417, 26);
            this.txt_type.TabIndex = 253;
            this.txt_type.Text = "FLO-MIN114";
            this.txt_type.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_box
            // 
            this.txt_box.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_box.Location = new System.Drawing.Point(126, 117);
            this.txt_box.Name = "txt_box";
            this.txt_box.Size = new System.Drawing.Size(417, 26);
            this.txt_box.TabIndex = 254;
            this.txt_box.Text = "SQK-NBD114-24";
            this.txt_box.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_speed
            // 
            this.txt_speed.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_speed.Location = new System.Drawing.Point(126, 153);
            this.txt_speed.Name = "txt_speed";
            this.txt_speed.Size = new System.Drawing.Size(417, 26);
            this.txt_speed.TabIndex = 255;
            this.txt_speed.Text = "400";
            this.txt_speed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_short
            // 
            this.txt_short.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_short.Location = new System.Drawing.Point(126, 225);
            this.txt_short.Name = "txt_short";
            this.txt_short.Size = new System.Drawing.Size(417, 26);
            this.txt_short.TabIndex = 256;
            this.txt_short.Text = "20";
            this.txt_short.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_model
            // 
            this.txt_model.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_model.Location = new System.Drawing.Point(126, 261);
            this.txt_model.Name = "txt_model";
            this.txt_model.Size = new System.Drawing.Size(417, 26);
            this.txt_model.TabIndex = 257;
            this.txt_model.Text = "dna_r10.4.1_e8.2_400bps_5khz_hac_mk1c.cfg";
            this.txt_model.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // rbt_start
            // 
            this.rbt_start.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_start.BackColor = System.Drawing.Color.Transparent;
            this.rbt_start.BaseColor = System.Drawing.Color.PaleGreen;
            this.rbt_start.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_start.FlatAppearance.BorderSize = 0;
            this.rbt_start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_start.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_start.ImageHeight = 80;
            this.rbt_start.ImageWidth = 80;
            this.rbt_start.Location = new System.Drawing.Point(24, 139);
            this.rbt_start.Name = "rbt_start";
            this.rbt_start.Radius = 24;
            this.rbt_start.Size = new System.Drawing.Size(136, 36);
            this.rbt_start.SpliteButtonWidth = 18;
            this.rbt_start.TabIndex = 258;
            this.rbt_start.Text = "启动测序";
            this.rbt_start.UseVisualStyleBackColor = false;
            this.rbt_start.Click += new System.EventHandler(this.rbt_start_Click);
            // 
            // rbt_pause
            // 
            this.rbt_pause.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_pause.BackColor = System.Drawing.Color.Transparent;
            this.rbt_pause.BaseColor = System.Drawing.Color.Orange;
            this.rbt_pause.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_pause.FlatAppearance.BorderSize = 0;
            this.rbt_pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_pause.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_pause.ImageHeight = 80;
            this.rbt_pause.ImageWidth = 80;
            this.rbt_pause.Location = new System.Drawing.Point(24, 242);
            this.rbt_pause.Name = "rbt_pause";
            this.rbt_pause.Radius = 24;
            this.rbt_pause.Size = new System.Drawing.Size(82, 36);
            this.rbt_pause.SpliteButtonWidth = 18;
            this.rbt_pause.TabIndex = 259;
            this.rbt_pause.Text = "暂停";
            this.rbt_pause.UseVisualStyleBackColor = false;
            this.rbt_pause.Click += new System.EventHandler(this.rbt_pause_Click);
            // 
            // rbt_continue
            // 
            this.rbt_continue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_continue.BackColor = System.Drawing.Color.Transparent;
            this.rbt_continue.BaseColor = System.Drawing.Color.Orange;
            this.rbt_continue.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_continue.FlatAppearance.BorderSize = 0;
            this.rbt_continue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_continue.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_continue.ImageHeight = 80;
            this.rbt_continue.ImageWidth = 80;
            this.rbt_continue.Location = new System.Drawing.Point(164, 242);
            this.rbt_continue.Name = "rbt_continue";
            this.rbt_continue.Radius = 24;
            this.rbt_continue.Size = new System.Drawing.Size(82, 36);
            this.rbt_continue.SpliteButtonWidth = 18;
            this.rbt_continue.TabIndex = 260;
            this.rbt_continue.Text = "继续";
            this.rbt_continue.UseVisualStyleBackColor = false;
            this.rbt_continue.Click += new System.EventHandler(this.rbt_continue_Click);
            // 
            // rbt_stop
            // 
            this.rbt_stop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_stop.BackColor = System.Drawing.Color.Transparent;
            this.rbt_stop.BaseColor = System.Drawing.Color.Orange;
            this.rbt_stop.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_stop.FlatAppearance.BorderSize = 0;
            this.rbt_stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_stop.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_stop.ImageHeight = 80;
            this.rbt_stop.ImageWidth = 80;
            this.rbt_stop.Location = new System.Drawing.Point(304, 242);
            this.rbt_stop.Name = "rbt_stop";
            this.rbt_stop.Radius = 24;
            this.rbt_stop.Size = new System.Drawing.Size(82, 36);
            this.rbt_stop.SpliteButtonWidth = 18;
            this.rbt_stop.TabIndex = 261;
            this.rbt_stop.Text = "停止";
            this.rbt_stop.UseVisualStyleBackColor = false;
            this.rbt_stop.Click += new System.EventHandler(this.rbt_stop_Click);
            // 
            // rbt_check
            // 
            this.rbt_check.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_check.BackColor = System.Drawing.Color.Transparent;
            this.rbt_check.BaseColor = System.Drawing.Color.LimeGreen;
            this.rbt_check.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_check.FlatAppearance.BorderSize = 0;
            this.rbt_check.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_check.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_check.ImageHeight = 80;
            this.rbt_check.ImageWidth = 80;
            this.rbt_check.Location = new System.Drawing.Point(250, 139);
            this.rbt_check.Name = "rbt_check";
            this.rbt_check.Radius = 24;
            this.rbt_check.Size = new System.Drawing.Size(136, 36);
            this.rbt_check.SpliteButtonWidth = 18;
            this.rbt_check.TabIndex = 262;
            this.rbt_check.Text = "芯片质检";
            this.rbt_check.UseVisualStyleBackColor = false;
            this.rbt_check.Click += new System.EventHandler(this.rbt_check_Click);
            // 
            // rbt_currentstate
            // 
            this.rbt_currentstate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_currentstate.BackColor = System.Drawing.Color.Transparent;
            this.rbt_currentstate.BaseColor = System.Drawing.Color.PaleGoldenrod;
            this.rbt_currentstate.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_currentstate.FlatAppearance.BorderSize = 0;
            this.rbt_currentstate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_currentstate.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_currentstate.ImageHeight = 80;
            this.rbt_currentstate.ImageWidth = 80;
            this.rbt_currentstate.Location = new System.Drawing.Point(24, 191);
            this.rbt_currentstate.Name = "rbt_currentstate";
            this.rbt_currentstate.Radius = 24;
            this.rbt_currentstate.Size = new System.Drawing.Size(136, 36);
            this.rbt_currentstate.SpliteButtonWidth = 18;
            this.rbt_currentstate.TabIndex = 263;
            this.rbt_currentstate.Text = "查询当前状态";
            this.rbt_currentstate.UseVisualStyleBackColor = false;
            this.rbt_currentstate.Click += new System.EventHandler(this.rbt_currentstate_Click);
            // 
            // rbt_filecopy
            // 
            this.rbt_filecopy.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_filecopy.BackColor = System.Drawing.Color.Transparent;
            this.rbt_filecopy.BaseColor = System.Drawing.Color.SkyBlue;
            this.rbt_filecopy.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_filecopy.FlatAppearance.BorderSize = 0;
            this.rbt_filecopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_filecopy.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_filecopy.ImageHeight = 80;
            this.rbt_filecopy.ImageWidth = 80;
            this.rbt_filecopy.Location = new System.Drawing.Point(24, 83);
            this.rbt_filecopy.Name = "rbt_filecopy";
            this.rbt_filecopy.Radius = 24;
            this.rbt_filecopy.Size = new System.Drawing.Size(136, 36);
            this.rbt_filecopy.SpliteButtonWidth = 18;
            this.rbt_filecopy.TabIndex = 264;
            this.rbt_filecopy.Text = "文件拷贝";
            this.rbt_filecopy.UseVisualStyleBackColor = false;
            this.rbt_filecopy.Click += new System.EventHandler(this.rbt_filecopy_Click);
            // 
            // rbt_copystate
            // 
            this.rbt_copystate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_copystate.BackColor = System.Drawing.Color.Transparent;
            this.rbt_copystate.BaseColor = System.Drawing.Color.SkyBlue;
            this.rbt_copystate.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_copystate.FlatAppearance.BorderSize = 0;
            this.rbt_copystate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_copystate.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_copystate.ImageHeight = 80;
            this.rbt_copystate.ImageWidth = 80;
            this.rbt_copystate.Location = new System.Drawing.Point(250, 83);
            this.rbt_copystate.Name = "rbt_copystate";
            this.rbt_copystate.Radius = 24;
            this.rbt_copystate.Size = new System.Drawing.Size(136, 36);
            this.rbt_copystate.SpliteButtonWidth = 18;
            this.rbt_copystate.TabIndex = 265;
            this.rbt_copystate.Text = "查询文件拷贝状态";
            this.rbt_copystate.UseVisualStyleBackColor = false;
            this.rbt_copystate.Click += new System.EventHandler(this.rbt_copystate_Click);
            // 
            // rbt_jianji
            // 
            this.rbt_jianji.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_jianji.BackColor = System.Drawing.Color.Transparent;
            this.rbt_jianji.BaseColor = System.Drawing.Color.PaleGoldenrod;
            this.rbt_jianji.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_jianji.FlatAppearance.BorderSize = 0;
            this.rbt_jianji.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_jianji.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_jianji.ImageHeight = 80;
            this.rbt_jianji.ImageWidth = 80;
            this.rbt_jianji.Location = new System.Drawing.Point(250, 191);
            this.rbt_jianji.Name = "rbt_jianji";
            this.rbt_jianji.Radius = 24;
            this.rbt_jianji.Size = new System.Drawing.Size(136, 36);
            this.rbt_jianji.SpliteButtonWidth = 18;
            this.rbt_jianji.TabIndex = 266;
            this.rbt_jianji.Text = "查询碱基识别进度";
            this.rbt_jianji.UseVisualStyleBackColor = false;
            this.rbt_jianji.Click += new System.EventHandler(this.rbt_jianji_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbt_check);
            this.groupBox2.Controls.Add(this.rbt_jianji);
            this.groupBox2.Controls.Add(this.rbt_copystate);
            this.groupBox2.Controls.Add(this.rbt_connect);
            this.groupBox2.Controls.Add(this.rbt_filecopy);
            this.groupBox2.Controls.Add(this.rbt_youwu);
            this.groupBox2.Controls.Add(this.rbt_currentstate);
            this.groupBox2.Controls.Add(this.rbt_start);
            this.groupBox2.Controls.Add(this.rbt_stop);
            this.groupBox2.Controls.Add(this.rbt_pause);
            this.groupBox2.Controls.Add(this.rbt_continue);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(608, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(419, 291);
            this.groupBox2.TabIndex = 267;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "指令";
            // 
            // roundButton1
            // 
            this.roundButton1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.roundButton1.BackColor = System.Drawing.Color.Transparent;
            this.roundButton1.BaseColor = System.Drawing.Color.Gold;
            this.roundButton1.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.roundButton1.FlatAppearance.BorderSize = 0;
            this.roundButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.roundButton1.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.roundButton1.ImageHeight = 80;
            this.roundButton1.ImageWidth = 80;
            this.roundButton1.Location = new System.Drawing.Point(126, 302);
            this.roundButton1.Name = "roundButton1";
            this.roundButton1.Radius = 24;
            this.roundButton1.Size = new System.Drawing.Size(162, 36);
            this.roundButton1.SpliteButtonWidth = 18;
            this.roundButton1.TabIndex = 268;
            this.roundButton1.Text = "设置标签对应碱基";
            this.roundButton1.UseVisualStyleBackColor = false;
            this.roundButton1.Click += new System.EventHandler(this.roundButton1_Click);
            // 
            // rbt_save
            // 
            this.rbt_save.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_save.BackColor = System.Drawing.Color.Transparent;
            this.rbt_save.BaseColor = System.Drawing.Color.Gold;
            this.rbt_save.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_save.FlatAppearance.BorderSize = 0;
            this.rbt_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_save.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_save.ImageHeight = 80;
            this.rbt_save.ImageWidth = 80;
            this.rbt_save.Location = new System.Drawing.Point(451, 302);
            this.rbt_save.Name = "rbt_save";
            this.rbt_save.Radius = 24;
            this.rbt_save.Size = new System.Drawing.Size(92, 36);
            this.rbt_save.SpliteButtonWidth = 18;
            this.rbt_save.TabIndex = 269;
            this.rbt_save.Text = "保存参数";
            this.rbt_save.UseVisualStyleBackColor = false;
            this.rbt_save.Click += new System.EventHandler(this.rbt_save_Click);
            // 
            // SequencingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1054, 689);
            this.Controls.Add(this.rbt_save);
            this.Controls.Add(this.roundButton1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txt_model);
            this.Controls.Add(this.txt_short);
            this.Controls.Add(this.txt_speed);
            this.Controls.Add(this.txt_box);
            this.Controls.Add(this.txt_type);
            this.Controls.Add(this.txt_time);
            this.Controls.Add(this.txt_num);
            this.Controls.Add(this.txt_name);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_RunID);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rch_send);
            this.Controls.Add(this.label06_A45);
            this.Controls.Add(this.rch_receive);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SequencingForm";
            this.Text = "测序仪测试";
            this.Load += new System.EventHandler(this.SequencingForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label06_A45;
        private System.Windows.Forms.RichTextBox rch_receive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rch_send;
        private CYCustomControl.RoundButton rbt_connect;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox richTextBox4;
        private CYCustomControl.RoundButton rbt_youwu;
        private System.Windows.Forms.TextBox txt_RunID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.TextBox txt_num;
        private System.Windows.Forms.TextBox txt_time;
        private System.Windows.Forms.TextBox txt_type;
        private System.Windows.Forms.TextBox txt_box;
        private System.Windows.Forms.TextBox txt_speed;
        private System.Windows.Forms.TextBox txt_short;
        private System.Windows.Forms.TextBox txt_model;
        private CYCustomControl.RoundButton rbt_start;
        private CYCustomControl.RoundButton rbt_pause;
        private CYCustomControl.RoundButton rbt_continue;
        private CYCustomControl.RoundButton rbt_stop;
        private CYCustomControl.RoundButton rbt_check;
        private CYCustomControl.RoundButton rbt_currentstate;
        private CYCustomControl.RoundButton rbt_filecopy;
        private CYCustomControl.RoundButton rbt_copystate;
        private CYCustomControl.RoundButton rbt_jianji;
        private System.Windows.Forms.GroupBox groupBox2;
        private CYCustomControl.RoundButton roundButton1;
        private CYCustomControl.RoundButton rbt_save;
    }
}