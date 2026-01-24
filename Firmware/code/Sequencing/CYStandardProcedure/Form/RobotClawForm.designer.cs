namespace CYStandardProcedure
{
    partial class RobotClawForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RobotClawForm));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txt_DataBits = new System.Windows.Forms.TextBox();
            this.cmb_StopBits = new System.Windows.Forms.ComboBox();
            this.cmb_Parity = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmb_DataFormat = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_SlaveAdd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_Paud = new System.Windows.Forms.TextBox();
            this.cmb_Port = new System.Windows.Forms.ComboBox();
            this.rb_Save = new CYCustomControl.RoundButton();
            this.rb_Close = new CYCustomControl.RoundButton();
            this.rb_Open = new CYCustomControl.RoundButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lb_Err = new System.Windows.Forms.Label();
            this.tx_FeedbackPosition = new System.Windows.Forms.TextBox();
            this.label88 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label107 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label103 = new System.Windows.Forms.Label();
            this.tx_FeedbackVelocity = new System.Windows.Forms.TextBox();
            this.tx_Torque = new System.Windows.Forms.TextBox();
            this.tx_ForceSensor = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.nud_OrientationRange = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.nud_PushVM = new System.Windows.Forms.NumericUpDown();
            this.nud_TimeRange = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.nud_PushAcc = new System.Windows.Forms.NumericUpDown();
            this.nud_PushForce = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.ntx_Distance = new HZH_Controls.Controls.UCNumTextBox();
            this.dgv_data = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btn_Save = new CYCustomControl.RoundButton();
            this.btn_Move = new CYCustomControl.RoundButton();
            this.btn_GetPos = new CYCustomControl.RoundButton();
            this.btn_MoveP = new CYCustomControl.RoundButton();
            this.btn_MoveN = new CYCustomControl.RoundButton();
            this.btn_ResetError = new CYCustomControl.RoundButton();
            this.btn_StopMove = new CYCustomControl.RoundButton();
            this.btn_Svo = new CYCustomControl.RoundButton();
            this.btn_Home = new CYCustomControl.RoundButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_OrientationRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PushVM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TimeRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PushAcc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PushForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_data)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 1000;
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 1000;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 1000;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // txt_DataBits
            // 
            this.txt_DataBits.Font = new System.Drawing.Font("宋体", 10F);
            this.txt_DataBits.Location = new System.Drawing.Point(675, 41);
            this.txt_DataBits.Name = "txt_DataBits";
            this.txt_DataBits.Size = new System.Drawing.Size(66, 23);
            this.txt_DataBits.TabIndex = 1;
            this.txt_DataBits.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmb_StopBits
            // 
            this.cmb_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_StopBits.Font = new System.Drawing.Font("宋体", 10F);
            this.cmb_StopBits.FormattingEnabled = true;
            this.cmb_StopBits.Location = new System.Drawing.Point(834, 42);
            this.cmb_StopBits.Name = "cmb_StopBits";
            this.cmb_StopBits.Size = new System.Drawing.Size(81, 21);
            this.cmb_StopBits.TabIndex = 5;
            // 
            // cmb_Parity
            // 
            this.cmb_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Parity.Font = new System.Drawing.Font("宋体", 10F);
            this.cmb_Parity.FormattingEnabled = true;
            this.cmb_Parity.Location = new System.Drawing.Point(484, 41);
            this.cmb_Parity.Name = "cmb_Parity";
            this.cmb_Parity.Size = new System.Drawing.Size(84, 21);
            this.cmb_Parity.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 10F);
            this.label10.Location = new System.Drawing.Point(610, 45);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 14);
            this.label10.TabIndex = 4;
            this.label10.Text = "数据位：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 10F);
            this.label8.Location = new System.Drawing.Point(757, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 14);
            this.label8.TabIndex = 4;
            this.label8.Text = "停止位：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10F);
            this.label9.Location = new System.Drawing.Point(423, 45);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 14);
            this.label9.TabIndex = 0;
            this.label9.Text = "校验位：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10F);
            this.label2.Location = new System.Drawing.Point(210, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "波特率：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10F);
            this.label1.Location = new System.Drawing.Point(26, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "端口号：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmb_DataFormat);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txt_SlaveAdd);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txt_Paud);
            this.groupBox1.Controls.Add(this.cmb_Port);
            this.groupBox1.Controls.Add(this.txt_DataBits);
            this.groupBox1.Controls.Add(this.cmb_StopBits);
            this.groupBox1.Controls.Add(this.cmb_Parity);
            this.groupBox1.Controls.Add(this.rb_Save);
            this.groupBox1.Controls.Add(this.rb_Close);
            this.groupBox1.Controls.Add(this.rb_Open);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 10F);
            this.groupBox1.Location = new System.Drawing.Point(21, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1005, 139);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "通信参数";
            // 
            // cmb_DataFormat
            // 
            this.cmb_DataFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_DataFormat.FormattingEnabled = true;
            this.cmb_DataFormat.Location = new System.Drawing.Point(250, 95);
            this.cmb_DataFormat.Name = "cmb_DataFormat";
            this.cmb_DataFormat.Size = new System.Drawing.Size(81, 21);
            this.cmb_DataFormat.TabIndex = 266;
            this.cmb_DataFormat.SelectedIndexChanged += new System.EventHandler(this.cmb_DataFormat_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(181, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 14);
            this.label7.TabIndex = 265;
            this.label7.Text = "数据格式：";
            // 
            // txt_SlaveAdd
            // 
            this.txt_SlaveAdd.Location = new System.Drawing.Point(94, 93);
            this.txt_SlaveAdd.Name = "txt_SlaveAdd";
            this.txt_SlaveAdd.Size = new System.Drawing.Size(72, 23);
            this.txt_SlaveAdd.TabIndex = 264;
            this.txt_SlaveAdd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 97);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 14);
            this.label6.TabIndex = 263;
            this.label6.Text = "从站地址：";
            // 
            // txt_Paud
            // 
            this.txt_Paud.Font = new System.Drawing.Font("宋体", 10F);
            this.txt_Paud.Location = new System.Drawing.Point(278, 39);
            this.txt_Paud.Name = "txt_Paud";
            this.txt_Paud.Size = new System.Drawing.Size(101, 23);
            this.txt_Paud.TabIndex = 196;
            this.txt_Paud.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmb_Port
            // 
            this.cmb_Port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Port.Font = new System.Drawing.Font("宋体", 10F);
            this.cmb_Port.FormattingEnabled = true;
            this.cmb_Port.Location = new System.Drawing.Point(94, 41);
            this.cmb_Port.Name = "cmb_Port";
            this.cmb_Port.Size = new System.Drawing.Size(81, 21);
            this.cmb_Port.TabIndex = 8;
            // 
            // rb_Save
            // 
            this.rb_Save.BackColor = System.Drawing.Color.Transparent;
            this.rb_Save.BackgroundImage = global::CYStandardProcedure.Properties.Resources.保存;
            this.rb_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rb_Save.BaseColor = System.Drawing.Color.Transparent;
            this.rb_Save.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rb_Save.ContextOffset = 0;
            this.rb_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rb_Save.FlatAppearance.BorderSize = 0;
            this.rb_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rb_Save.ImageHeight = 80;
            this.rb_Save.ImageWidth = 80;
            this.rb_Save.Location = new System.Drawing.Point(913, 77);
            this.rb_Save.Name = "rb_Save";
            this.rb_Save.Radius = 24;
            this.rb_Save.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rb_Save.Size = new System.Drawing.Size(52, 53);
            this.rb_Save.SpliteButtonWidth = 18;
            this.rb_Save.TabIndex = 195;
            this.rb_Save.UseVisualStyleBackColor = false;
            this.rb_Save.Click += new System.EventHandler(this.rb_Save_Click);
            // 
            // rb_Close
            // 
            this.rb_Close.BackColor = System.Drawing.Color.Transparent;
            this.rb_Close.BackgroundImage = global::CYStandardProcedure.Properties.Resources.断开连接;
            this.rb_Close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rb_Close.BaseColor = System.Drawing.Color.Transparent;
            this.rb_Close.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rb_Close.ContextOffset = 0;
            this.rb_Close.FlatAppearance.BorderSize = 0;
            this.rb_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rb_Close.ImageHeight = 80;
            this.rb_Close.ImageWidth = 80;
            this.rb_Close.Location = new System.Drawing.Point(786, 76);
            this.rb_Close.Name = "rb_Close";
            this.rb_Close.Radius = 24;
            this.rb_Close.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rb_Close.Size = new System.Drawing.Size(52, 53);
            this.rb_Close.SpliteButtonWidth = 18;
            this.rb_Close.TabIndex = 194;
            this.rb_Close.UseVisualStyleBackColor = false;
            this.rb_Close.Click += new System.EventHandler(this.rb_Close_Click);
            // 
            // rb_Open
            // 
            this.rb_Open.BackColor = System.Drawing.Color.Transparent;
            this.rb_Open.BackgroundImage = global::CYStandardProcedure.Properties.Resources.连接;
            this.rb_Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rb_Open.BaseColor = System.Drawing.Color.Transparent;
            this.rb_Open.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rb_Open.ContextOffset = 0;
            this.rb_Open.FlatAppearance.BorderSize = 0;
            this.rb_Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rb_Open.ImageHeight = 80;
            this.rb_Open.ImageWidth = 80;
            this.rb_Open.Location = new System.Drawing.Point(654, 76);
            this.rb_Open.Name = "rb_Open";
            this.rb_Open.Radius = 24;
            this.rb_Open.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rb_Open.Size = new System.Drawing.Size(52, 53);
            this.rb_Open.SpliteButtonWidth = 18;
            this.rb_Open.TabIndex = 193;
            this.rb_Open.UseVisualStyleBackColor = false;
            this.rb_Open.Click += new System.EventHandler(this.rb_Open_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lb_Err);
            this.groupBox2.Controls.Add(this.tx_FeedbackPosition);
            this.groupBox2.Controls.Add(this.label88);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label107);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label103);
            this.groupBox2.Controls.Add(this.tx_FeedbackVelocity);
            this.groupBox2.Controls.Add(this.tx_Torque);
            this.groupBox2.Controls.Add(this.tx_ForceSensor);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 10F);
            this.groupBox2.Location = new System.Drawing.Point(21, 159);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1002, 79);
            this.groupBox2.TabIndex = 227;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "信息监控";
            // 
            // lb_Err
            // 
            this.lb_Err.BackColor = System.Drawing.Color.Green;
            this.lb_Err.Location = new System.Drawing.Point(957, 30);
            this.lb_Err.Name = "lb_Err";
            this.lb_Err.Size = new System.Drawing.Size(34, 29);
            this.lb_Err.TabIndex = 0;
            // 
            // tx_FeedbackPosition
            // 
            this.tx_FeedbackPosition.BackColor = System.Drawing.Color.Black;
            this.tx_FeedbackPosition.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.tx_FeedbackPosition.ForeColor = System.Drawing.Color.Lime;
            this.tx_FeedbackPosition.Location = new System.Drawing.Point(78, 30);
            this.tx_FeedbackPosition.Margin = new System.Windows.Forms.Padding(2);
            this.tx_FeedbackPosition.Name = "tx_FeedbackPosition";
            this.tx_FeedbackPosition.ReadOnly = true;
            this.tx_FeedbackPosition.Size = new System.Drawing.Size(137, 25);
            this.tx_FeedbackPosition.TabIndex = 193;
            this.tx_FeedbackPosition.Text = "0";
            this.tx_FeedbackPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label88
            // 
            this.label88.AutoSize = true;
            this.label88.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.label88.Location = new System.Drawing.Point(882, 33);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(69, 19);
            this.label88.TabIndex = 199;
            this.label88.Text = "报警状态:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label3.Location = new System.Drawing.Point(433, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 20);
            this.label3.TabIndex = 198;
            this.label3.Text = "传感器读数:";
            // 
            // label107
            // 
            this.label107.AutoSize = true;
            this.label107.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label107.Location = new System.Drawing.Point(9, 33);
            this.label107.Name = "label107";
            this.label107.Size = new System.Drawing.Size(68, 20);
            this.label107.TabIndex = 194;
            this.label107.Text = "反馈位置:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label4.Location = new System.Drawing.Point(656, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 20);
            this.label4.TabIndex = 198;
            this.label4.Text = "力矩反馈:";
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.label103.Location = new System.Drawing.Point(221, 32);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(68, 20);
            this.label103.TabIndex = 198;
            this.label103.Text = "反馈速度:";
            // 
            // tx_FeedbackVelocity
            // 
            this.tx_FeedbackVelocity.BackColor = System.Drawing.Color.Black;
            this.tx_FeedbackVelocity.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.tx_FeedbackVelocity.ForeColor = System.Drawing.Color.Lime;
            this.tx_FeedbackVelocity.Location = new System.Drawing.Point(289, 30);
            this.tx_FeedbackVelocity.Margin = new System.Windows.Forms.Padding(2);
            this.tx_FeedbackVelocity.Name = "tx_FeedbackVelocity";
            this.tx_FeedbackVelocity.ReadOnly = true;
            this.tx_FeedbackVelocity.Size = new System.Drawing.Size(137, 25);
            this.tx_FeedbackVelocity.TabIndex = 197;
            this.tx_FeedbackVelocity.Text = "0";
            this.tx_FeedbackVelocity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tx_Torque
            // 
            this.tx_Torque.BackColor = System.Drawing.Color.Black;
            this.tx_Torque.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.tx_Torque.ForeColor = System.Drawing.Color.Lime;
            this.tx_Torque.Location = new System.Drawing.Point(726, 30);
            this.tx_Torque.Margin = new System.Windows.Forms.Padding(2);
            this.tx_Torque.Name = "tx_Torque";
            this.tx_Torque.ReadOnly = true;
            this.tx_Torque.Size = new System.Drawing.Size(137, 25);
            this.tx_Torque.TabIndex = 197;
            this.tx_Torque.Text = "0";
            this.tx_Torque.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tx_ForceSensor
            // 
            this.tx_ForceSensor.BackColor = System.Drawing.Color.Black;
            this.tx_ForceSensor.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.tx_ForceSensor.ForeColor = System.Drawing.Color.Lime;
            this.tx_ForceSensor.Location = new System.Drawing.Point(515, 30);
            this.tx_ForceSensor.Margin = new System.Windows.Forms.Padding(2);
            this.tx_ForceSensor.Name = "tx_ForceSensor";
            this.tx_ForceSensor.ReadOnly = true;
            this.tx_ForceSensor.Size = new System.Drawing.Size(137, 25);
            this.tx_ForceSensor.TabIndex = 197;
            this.tx_ForceSensor.Text = "0";
            this.tx_ForceSensor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 10F);
            this.label12.Location = new System.Drawing.Point(615, 312);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 14);
            this.label12.TabIndex = 230;
            this.label12.Text = "推压力:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 10F);
            this.label15.Location = new System.Drawing.Point(407, 315);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 14);
            this.label15.TabIndex = 231;
            this.label15.Text = "定位范围:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 10F);
            this.label11.Location = new System.Drawing.Point(9, 315);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 14);
            this.label11.TabIndex = 232;
            this.label11.Text = "速度:";
            // 
            // nud_OrientationRange
            // 
            this.nud_OrientationRange.BackColor = System.Drawing.Color.White;
            this.nud_OrientationRange.Font = new System.Drawing.Font("宋体", 10F);
            this.nud_OrientationRange.Location = new System.Drawing.Point(481, 311);
            this.nud_OrientationRange.Margin = new System.Windows.Forms.Padding(2);
            this.nud_OrientationRange.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_OrientationRange.Name = "nud_OrientationRange";
            this.nud_OrientationRange.Size = new System.Drawing.Size(119, 23);
            this.nud_OrientationRange.TabIndex = 233;
            this.nud_OrientationRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nud_OrientationRange.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 10F);
            this.label14.Location = new System.Drawing.Point(804, 312);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(98, 14);
            this.label14.TabIndex = 228;
            this.label14.Text = "时间范围(ms):";
            // 
            // nud_PushVM
            // 
            this.nud_PushVM.BackColor = System.Drawing.Color.White;
            this.nud_PushVM.Font = new System.Drawing.Font("宋体", 10F);
            this.nud_PushVM.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nud_PushVM.Location = new System.Drawing.Point(55, 312);
            this.nud_PushVM.Margin = new System.Windows.Forms.Padding(2);
            this.nud_PushVM.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nud_PushVM.Name = "nud_PushVM";
            this.nud_PushVM.Size = new System.Drawing.Size(128, 23);
            this.nud_PushVM.TabIndex = 234;
            this.nud_PushVM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nud_PushVM.Value = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            // 
            // nud_TimeRange
            // 
            this.nud_TimeRange.BackColor = System.Drawing.Color.White;
            this.nud_TimeRange.Font = new System.Drawing.Font("宋体", 10F);
            this.nud_TimeRange.Location = new System.Drawing.Point(902, 308);
            this.nud_TimeRange.Margin = new System.Windows.Forms.Padding(2);
            this.nud_TimeRange.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nud_TimeRange.Name = "nud_TimeRange";
            this.nud_TimeRange.Size = new System.Drawing.Size(123, 23);
            this.nud_TimeRange.TabIndex = 236;
            this.nud_TimeRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nud_TimeRange.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 10F);
            this.label13.Location = new System.Drawing.Point(203, 315);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 14);
            this.label13.TabIndex = 229;
            this.label13.Text = "加速度:";
            // 
            // nud_PushAcc
            // 
            this.nud_PushAcc.BackColor = System.Drawing.Color.White;
            this.nud_PushAcc.Font = new System.Drawing.Font("宋体", 10F);
            this.nud_PushAcc.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nud_PushAcc.Location = new System.Drawing.Point(271, 312);
            this.nud_PushAcc.Margin = new System.Windows.Forms.Padding(2);
            this.nud_PushAcc.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nud_PushAcc.Name = "nud_PushAcc";
            this.nud_PushAcc.Size = new System.Drawing.Size(117, 23);
            this.nud_PushAcc.TabIndex = 237;
            this.nud_PushAcc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nud_PushAcc.Value = new decimal(new int[] {
            5000000,
            0,
            0,
            0});
            // 
            // nud_PushForce
            // 
            this.nud_PushForce.BackColor = System.Drawing.Color.White;
            this.nud_PushForce.Font = new System.Drawing.Font("宋体", 10F);
            this.nud_PushForce.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nud_PushForce.Location = new System.Drawing.Point(675, 309);
            this.nud_PushForce.Margin = new System.Windows.Forms.Padding(2);
            this.nud_PushForce.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nud_PushForce.Name = "nud_PushForce";
            this.nud_PushForce.Size = new System.Drawing.Size(121, 23);
            this.nud_PushForce.TabIndex = 235;
            this.nud_PushForce.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nud_PushForce.Value = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(17, 363);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 20);
            this.label5.TabIndex = 256;
            this.label5.Text = "移动距离(um)";
            // 
            // ntx_Distance
            // 
            this.ntx_Distance.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ntx_Distance.InputType = HZH_Controls.TextInputType.Number;
            this.ntx_Distance.IsNumCanInput = true;
            this.ntx_Distance.KeyBoardType = HZH_Controls.Controls.KeyBoardType.Null;
            this.ntx_Distance.Location = new System.Drawing.Point(138, 348);
            this.ntx_Distance.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ntx_Distance.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ntx_Distance.Name = "ntx_Distance";
            this.ntx_Distance.Num = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ntx_Distance.Padding = new System.Windows.Forms.Padding(2);
            this.ntx_Distance.Size = new System.Drawing.Size(152, 48);
            this.ntx_Distance.TabIndex = 255;
            // 
            // dgv_data
            // 
            this.dgv_data.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_data.Location = new System.Drawing.Point(17, 410);
            this.dgv_data.Name = "dgv_data";
            this.dgv_data.RowTemplate.Height = 23;
            this.dgv_data.Size = new System.Drawing.Size(1009, 132);
            this.dgv_data.TabIndex = 259;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.Transparent;
            this.btn_Save.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Save.BackgroundImage")));
            this.btn_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Save.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Save.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Save.ContextOffset = 0;
            this.btn_Save.FlatAppearance.BorderSize = 0;
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.ImageHeight = 80;
            this.btn_Save.ImageWidth = 80;
            this.btn_Save.Location = new System.Drawing.Point(902, 548);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Radius = 24;
            this.btn_Save.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Save.Size = new System.Drawing.Size(70, 70);
            this.btn_Save.SpliteButtonWidth = 18;
            this.btn_Save.TabIndex = 262;
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Move
            // 
            this.btn_Move.BackColor = System.Drawing.Color.Transparent;
            this.btn_Move.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Move.BackgroundImage")));
            this.btn_Move.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Move.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Move.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Move.ContextOffset = 0;
            this.btn_Move.FlatAppearance.BorderSize = 0;
            this.btn_Move.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Move.ImageHeight = 80;
            this.btn_Move.ImageWidth = 80;
            this.btn_Move.Location = new System.Drawing.Point(689, 548);
            this.btn_Move.Name = "btn_Move";
            this.btn_Move.Radius = 24;
            this.btn_Move.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Move.Size = new System.Drawing.Size(70, 70);
            this.btn_Move.SpliteButtonWidth = 18;
            this.btn_Move.TabIndex = 261;
            this.btn_Move.UseVisualStyleBackColor = false;
            this.btn_Move.Click += new System.EventHandler(this.btn_Move_Click);
            // 
            // btn_GetPos
            // 
            this.btn_GetPos.BackColor = System.Drawing.Color.Transparent;
            this.btn_GetPos.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_GetPos.BackgroundImage")));
            this.btn_GetPos.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_GetPos.BaseColor = System.Drawing.Color.Transparent;
            this.btn_GetPos.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_GetPos.ContextOffset = 0;
            this.btn_GetPos.FlatAppearance.BorderSize = 0;
            this.btn_GetPos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_GetPos.ImageHeight = 80;
            this.btn_GetPos.ImageWidth = 80;
            this.btn_GetPos.Location = new System.Drawing.Point(476, 548);
            this.btn_GetPos.Name = "btn_GetPos";
            this.btn_GetPos.Radius = 24;
            this.btn_GetPos.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_GetPos.Size = new System.Drawing.Size(70, 70);
            this.btn_GetPos.SpliteButtonWidth = 18;
            this.btn_GetPos.TabIndex = 260;
            this.btn_GetPos.UseVisualStyleBackColor = false;
            this.btn_GetPos.Click += new System.EventHandler(this.btn_GetPos_Click);
            // 
            // btn_MoveP
            // 
            this.btn_MoveP.BackColor = System.Drawing.Color.Transparent;
            this.btn_MoveP.BackgroundImage = global::CYStandardProcedure.Properties.Resources.向右;
            this.btn_MoveP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_MoveP.BaseColor = System.Drawing.Color.Transparent;
            this.btn_MoveP.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_MoveP.ContextOffset = 0;
            this.btn_MoveP.FlatAppearance.BorderSize = 0;
            this.btn_MoveP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_MoveP.ImageHeight = 80;
            this.btn_MoveP.ImageWidth = 80;
            this.btn_MoveP.Location = new System.Drawing.Point(430, 348);
            this.btn_MoveP.Name = "btn_MoveP";
            this.btn_MoveP.Radius = 24;
            this.btn_MoveP.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_MoveP.Size = new System.Drawing.Size(52, 53);
            this.btn_MoveP.SpliteButtonWidth = 18;
            this.btn_MoveP.TabIndex = 258;
            this.btn_MoveP.UseVisualStyleBackColor = false;
            this.btn_MoveP.Click += new System.EventHandler(this.btn_MoveP_Click);
            // 
            // btn_MoveN
            // 
            this.btn_MoveN.BackColor = System.Drawing.Color.Transparent;
            this.btn_MoveN.BackgroundImage = global::CYStandardProcedure.Properties.Resources.向左;
            this.btn_MoveN.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_MoveN.BaseColor = System.Drawing.Color.Transparent;
            this.btn_MoveN.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_MoveN.ContextOffset = 0;
            this.btn_MoveN.FlatAppearance.BorderSize = 0;
            this.btn_MoveN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_MoveN.ImageHeight = 80;
            this.btn_MoveN.ImageWidth = 80;
            this.btn_MoveN.Location = new System.Drawing.Point(333, 348);
            this.btn_MoveN.Name = "btn_MoveN";
            this.btn_MoveN.Radius = 24;
            this.btn_MoveN.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_MoveN.Size = new System.Drawing.Size(52, 53);
            this.btn_MoveN.SpliteButtonWidth = 18;
            this.btn_MoveN.TabIndex = 257;
            this.btn_MoveN.UseVisualStyleBackColor = false;
            this.btn_MoveN.Click += new System.EventHandler(this.btn_MoveN_Click);
            // 
            // btn_ResetError
            // 
            this.btn_ResetError.BackColor = System.Drawing.Color.Transparent;
            this.btn_ResetError.BackgroundImage = global::CYStandardProcedure.Properties.Resources.解除报警;
            this.btn_ResetError.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_ResetError.BaseColor = System.Drawing.Color.Transparent;
            this.btn_ResetError.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_ResetError.ContextOffset = 0;
            this.btn_ResetError.FlatAppearance.BorderSize = 0;
            this.btn_ResetError.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ResetError.ImageHeight = 80;
            this.btn_ResetError.ImageWidth = 80;
            this.btn_ResetError.Location = new System.Drawing.Point(310, 247);
            this.btn_ResetError.Name = "btn_ResetError";
            this.btn_ResetError.Radius = 24;
            this.btn_ResetError.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_ResetError.Size = new System.Drawing.Size(52, 53);
            this.btn_ResetError.SpliteButtonWidth = 18;
            this.btn_ResetError.TabIndex = 254;
            this.btn_ResetError.UseVisualStyleBackColor = false;
            this.btn_ResetError.Click += new System.EventHandler(this.btn_ResetError_Click);
            // 
            // btn_StopMove
            // 
            this.btn_StopMove.BackColor = System.Drawing.Color.Transparent;
            this.btn_StopMove.BackgroundImage = global::CYStandardProcedure.Properties.Resources.急停;
            this.btn_StopMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_StopMove.BaseColor = System.Drawing.Color.Transparent;
            this.btn_StopMove.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_StopMove.ContextOffset = 0;
            this.btn_StopMove.FlatAppearance.BorderSize = 0;
            this.btn_StopMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_StopMove.ImageHeight = 80;
            this.btn_StopMove.ImageWidth = 80;
            this.btn_StopMove.Location = new System.Drawing.Point(425, 245);
            this.btn_StopMove.Name = "btn_StopMove";
            this.btn_StopMove.Radius = 24;
            this.btn_StopMove.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_StopMove.Size = new System.Drawing.Size(52, 53);
            this.btn_StopMove.SpliteButtonWidth = 18;
            this.btn_StopMove.TabIndex = 197;
            this.btn_StopMove.UseVisualStyleBackColor = false;
            this.btn_StopMove.Click += new System.EventHandler(this.btn_StopMove_Click);
            // 
            // btn_Svo
            // 
            this.btn_Svo.BackColor = System.Drawing.Color.Transparent;
            this.btn_Svo.BackgroundImage = global::CYStandardProcedure.Properties.Resources.NoSvo;
            this.btn_Svo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Svo.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Svo.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Svo.ContextOffset = 0;
            this.btn_Svo.FlatAppearance.BorderSize = 0;
            this.btn_Svo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Svo.ImageHeight = 80;
            this.btn_Svo.ImageWidth = 80;
            this.btn_Svo.Location = new System.Drawing.Point(19, 245);
            this.btn_Svo.Name = "btn_Svo";
            this.btn_Svo.Radius = 24;
            this.btn_Svo.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Svo.Size = new System.Drawing.Size(62, 55);
            this.btn_Svo.SpliteButtonWidth = 18;
            this.btn_Svo.TabIndex = 253;
            this.btn_Svo.Tag = "失使能";
            this.btn_Svo.UseVisualStyleBackColor = false;
            this.btn_Svo.Click += new System.EventHandler(this.btn_Svo_Click);
            // 
            // btn_Home
            // 
            this.btn_Home.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Home.BackColor = System.Drawing.Color.Transparent;
            this.btn_Home.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btn_Home.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.btn_Home.FlatAppearance.BorderSize = 0;
            this.btn_Home.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Home.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btn_Home.ImageHeight = 80;
            this.btn_Home.ImageWidth = 80;
            this.btn_Home.Location = new System.Drawing.Point(115, 248);
            this.btn_Home.Name = "btn_Home";
            this.btn_Home.Radius = 24;
            this.btn_Home.Size = new System.Drawing.Size(137, 47);
            this.btn_Home.SpliteButtonWidth = 18;
            this.btn_Home.TabIndex = 252;
            this.btn_Home.Text = "回零";
            this.btn_Home.UseVisualStyleBackColor = false;
            this.btn_Home.Click += new System.EventHandler(this.btn_Home_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(19, 560);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 46);
            this.button1.TabIndex = 263;
            this.button1.Text = "绝对运动";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(168, 560);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 46);
            this.button2.TabIndex = 264;
            this.button2.Text = "推压运动";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(299, 560);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(117, 46);
            this.button3.TabIndex = 265;
            this.button3.Text = "复位";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // RobotClawForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1038, 650);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Move);
            this.Controls.Add(this.btn_GetPos);
            this.Controls.Add(this.dgv_data);
            this.Controls.Add(this.btn_MoveP);
            this.Controls.Add(this.btn_MoveN);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ntx_Distance);
            this.Controls.Add(this.btn_ResetError);
            this.Controls.Add(this.btn_StopMove);
            this.Controls.Add(this.btn_Svo);
            this.Controls.Add(this.btn_Home);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.nud_OrientationRange);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.nud_PushVM);
            this.Controls.Add(this.nud_TimeRange);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.nud_PushAcc);
            this.Controls.Add(this.nud_PushForce);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RobotClawForm";
            this.Text = "MaterialsForm";
            this.Load += new System.EventHandler(this.GripPawl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_OrientationRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PushVM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TimeRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PushAcc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PushForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_data)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txt_DataBits;
        private System.Windows.Forms.ComboBox cmb_StopBits;
        private System.Windows.Forms.ComboBox cmb_Parity;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private CYCustomControl.RoundButton rb_Save;
        private CYCustomControl.RoundButton rb_Close;
        private CYCustomControl.RoundButton rb_Open;
        private System.Windows.Forms.ComboBox cmb_Port;
        private System.Windows.Forms.TextBox txt_Paud;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lb_Err;
        private System.Windows.Forms.TextBox tx_FeedbackPosition;
        private System.Windows.Forms.Label label88;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label107;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label103;
        private System.Windows.Forms.TextBox tx_FeedbackVelocity;
        private System.Windows.Forms.TextBox tx_Torque;
        private System.Windows.Forms.TextBox tx_ForceSensor;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nud_OrientationRange;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nud_PushVM;
        private System.Windows.Forms.NumericUpDown nud_TimeRange;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nud_PushAcc;
        private System.Windows.Forms.NumericUpDown nud_PushForce;
        private CYCustomControl.RoundButton btn_Svo;
        private CYCustomControl.RoundButton btn_Home;
        private CYCustomControl.RoundButton btn_StopMove;
        private CYCustomControl.RoundButton btn_ResetError;
        private System.Windows.Forms.Label label5;
        private HZH_Controls.Controls.UCNumTextBox ntx_Distance;
        private CYCustomControl.RoundButton btn_MoveN;
        private CYCustomControl.RoundButton btn_MoveP;
        private System.Windows.Forms.DataGridView dgv_data;
        private CYCustomControl.RoundButton btn_Save;
        private CYCustomControl.RoundButton btn_Move;
        private CYCustomControl.RoundButton btn_GetPos;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox cmb_DataFormat;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_SlaveAdd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}