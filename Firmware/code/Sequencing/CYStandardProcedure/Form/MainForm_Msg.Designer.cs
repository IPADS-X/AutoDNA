namespace CYStandardProcedure
{
    partial class MainForm_Msg
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lab_data_state = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lab_Robot_state = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lab_Sequence_state = new System.Windows.Forms.Label();
            this.lab_Feed_state = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lab_Carry_state = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lab_clawrobot = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lab_carryRun = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lab_claw = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lab_gun = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lab_huanliao = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lab_buliao = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.txt_snMsg = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_inputSN = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.gbx_handle = new System.Windows.Forms.GroupBox();
            this.button12 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gbx_handle.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.lab_data_state);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lab_Robot_state);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lab_Sequence_state);
            this.groupBox1.Controls.Add(this.lab_Feed_state);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.lab_Carry_state);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(25, 22);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(450, 237);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "工位状态";
            // 
            // lab_data_state
            // 
            this.lab_data_state.BackColor = System.Drawing.Color.LightCyan;
            this.lab_data_state.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_data_state.Location = new System.Drawing.Point(229, 195);
            this.lab_data_state.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_data_state.Name = "lab_data_state";
            this.lab_data_state.Size = new System.Drawing.Size(175, 30);
            this.lab_data_state.TabIndex = 29;
            this.lab_data_state.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(16, 200);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(189, 20);
            this.label6.TabIndex = 28;
            this.label6.Text = "数据处理线程状态：";
            // 
            // lab_Robot_state
            // 
            this.lab_Robot_state.BackColor = System.Drawing.Color.LightCyan;
            this.lab_Robot_state.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Robot_state.Location = new System.Drawing.Point(229, 155);
            this.lab_Robot_state.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_Robot_state.Name = "lab_Robot_state";
            this.lab_Robot_state.Size = new System.Drawing.Size(175, 30);
            this.lab_Robot_state.TabIndex = 27;
            this.lab_Robot_state.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(16, 160);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(169, 20);
            this.label7.TabIndex = 24;
            this.label7.Text = "机器人工位状态：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(16, 120);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(169, 20);
            this.label5.TabIndex = 22;
            this.label5.Text = "测序仪工位状态：";
            // 
            // lab_Sequence_state
            // 
            this.lab_Sequence_state.BackColor = System.Drawing.Color.LightCyan;
            this.lab_Sequence_state.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Sequence_state.Location = new System.Drawing.Point(229, 115);
            this.lab_Sequence_state.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_Sequence_state.Name = "lab_Sequence_state";
            this.lab_Sequence_state.Size = new System.Drawing.Size(175, 30);
            this.lab_Sequence_state.TabIndex = 21;
            this.lab_Sequence_state.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lab_Feed_state
            // 
            this.lab_Feed_state.BackColor = System.Drawing.Color.LightCyan;
            this.lab_Feed_state.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Feed_state.Location = new System.Drawing.Point(230, 35);
            this.lab_Feed_state.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_Feed_state.Name = "lab_Feed_state";
            this.lab_Feed_state.Size = new System.Drawing.Size(175, 30);
            this.lab_Feed_state.TabIndex = 17;
            this.lab_Feed_state.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(16, 40);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(189, 20);
            this.label14.TabIndex = 16;
            this.label14.Text = "供料线程工位状态：";
            // 
            // lab_Carry_state
            // 
            this.lab_Carry_state.BackColor = System.Drawing.Color.LightCyan;
            this.lab_Carry_state.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Carry_state.Location = new System.Drawing.Point(230, 75);
            this.lab_Carry_state.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_Carry_state.Name = "lab_Carry_state";
            this.lab_Carry_state.Size = new System.Drawing.Size(175, 30);
            this.lab_Carry_state.TabIndex = 10;
            this.lab_Carry_state.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(16, 80);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(149, 20);
            this.label8.TabIndex = 4;
            this.label8.Text = "搬运工位状态：";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.lab_clawrobot);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.lab_carryRun);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lab_claw);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.lab_gun);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(25, 280);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(450, 187);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "搬运模组工艺点";
            // 
            // lab_clawrobot
            // 
            this.lab_clawrobot.BackColor = System.Drawing.Color.LightCyan;
            this.lab_clawrobot.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_clawrobot.Location = new System.Drawing.Point(230, 73);
            this.lab_clawrobot.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_clawrobot.Name = "lab_clawrobot";
            this.lab_clawrobot.Size = new System.Drawing.Size(175, 30);
            this.lab_clawrobot.TabIndex = 21;
            this.lab_clawrobot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(16, 78);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(169, 20);
            this.label10.TabIndex = 20;
            this.label10.Text = "机器人夹爪状态：";
            // 
            // lab_carryRun
            // 
            this.lab_carryRun.BackColor = System.Drawing.Color.LightCyan;
            this.lab_carryRun.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_carryRun.Location = new System.Drawing.Point(230, 149);
            this.lab_carryRun.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_carryRun.Name = "lab_carryRun";
            this.lab_carryRun.Size = new System.Drawing.Size(175, 30);
            this.lab_carryRun.TabIndex = 19;
            this.lab_carryRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(16, 154);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 20);
            this.label3.TabIndex = 18;
            this.label3.Text = "工作状态：";
            // 
            // lab_claw
            // 
            this.lab_claw.BackColor = System.Drawing.Color.LightCyan;
            this.lab_claw.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_claw.Location = new System.Drawing.Point(230, 35);
            this.lab_claw.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_claw.Name = "lab_claw";
            this.lab_claw.Size = new System.Drawing.Size(175, 30);
            this.lab_claw.TabIndex = 17;
            this.lab_claw.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(16, 40);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(149, 20);
            this.label9.TabIndex = 16;
            this.label9.Text = "搬运夹爪状态：";
            // 
            // lab_gun
            // 
            this.lab_gun.BackColor = System.Drawing.Color.LightCyan;
            this.lab_gun.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_gun.Location = new System.Drawing.Point(230, 111);
            this.lab_gun.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_gun.Name = "lab_gun";
            this.lab_gun.Size = new System.Drawing.Size(175, 30);
            this.lab_gun.TabIndex = 10;
            this.lab_gun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(16, 116);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(129, 20);
            this.label11.TabIndex = 4;
            this.label11.Text = "移液枪状态：";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.lab_huanliao);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.lab_buliao);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(25, 488);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(450, 126);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "补/换料提示";
            // 
            // lab_huanliao
            // 
            this.lab_huanliao.BackColor = System.Drawing.Color.LightCyan;
            this.lab_huanliao.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_huanliao.Location = new System.Drawing.Point(230, 35);
            this.lab_huanliao.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_huanliao.Name = "lab_huanliao";
            this.lab_huanliao.Size = new System.Drawing.Size(175, 30);
            this.lab_huanliao.TabIndex = 17;
            this.lab_huanliao.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(16, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = "当前换料区域:";
            // 
            // lab_buliao
            // 
            this.lab_buliao.BackColor = System.Drawing.Color.LightCyan;
            this.lab_buliao.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_buliao.Location = new System.Drawing.Point(230, 74);
            this.lab_buliao.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_buliao.Name = "lab_buliao";
            this.lab_buliao.Size = new System.Drawing.Size(175, 30);
            this.lab_buliao.TabIndex = 10;
            this.lab_buliao.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(16, 79);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "当前补料区域:";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(26, 53);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(134, 42);
            this.button1.TabIndex = 8;
            this.button1.Text = "DNA样本进料标志";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(26, 131);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(134, 42);
            this.button2.TabIndex = 9;
            this.button2.Text = "总控允许实验标志";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.Location = new System.Drawing.Point(26, 209);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(134, 42);
            this.button3.TabIndex = 10;
            this.button3.Text = "地轨给1000枪头";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.Location = new System.Drawing.Point(26, 287);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(134, 42);
            this.button4.TabIndex = 11;
            this.button4.Text = "地轨给200枪头";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button5.Location = new System.Drawing.Point(26, 365);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(134, 42);
            this.button5.TabIndex = 12;
            this.button5.Text = "地轨给50枪头";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.Location = new System.Drawing.Point(26, 521);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(134, 42);
            this.button6.TabIndex = 13;
            this.button6.Text = "地轨给离心管";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7.Location = new System.Drawing.Point(26, 443);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(134, 42);
            this.button7.TabIndex = 14;
            this.button7.Text = "地轨给低温试剂";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button8.Location = new System.Drawing.Point(259, 53);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(134, 42);
            this.button8.TabIndex = 15;
            this.button8.Text = "给测序完成标志";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button9.Location = new System.Drawing.Point(259, 131);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(134, 42);
            this.button9.TabIndex = 16;
            this.button9.Text = "给孵育完成标志";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // txt_snMsg
            // 
            this.txt_snMsg.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_snMsg.Location = new System.Drawing.Point(293, 220);
            this.txt_snMsg.Name = "txt_snMsg";
            this.txt_snMsg.Size = new System.Drawing.Size(100, 23);
            this.txt_snMsg.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(256, 223);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 14);
            this.label1.TabIndex = 18;
            this.label1.Text = "SN：";
            // 
            // btn_inputSN
            // 
            this.btn_inputSN.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_inputSN.Location = new System.Drawing.Point(317, 258);
            this.btn_inputSN.Name = "btn_inputSN";
            this.btn_inputSN.Size = new System.Drawing.Size(76, 35);
            this.btn_inputSN.TabIndex = 19;
            this.btn_inputSN.Text = "写入";
            this.btn_inputSN.UseVisualStyleBackColor = true;
            this.btn_inputSN.Click += new System.EventHandler(this.btn_inputSN_Click);
            // 
            // button10
            // 
            this.button10.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button10.Location = new System.Drawing.Point(259, 443);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(134, 42);
            this.button10.TabIndex = 20;
            this.button10.Text = "空料标志";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button11.Location = new System.Drawing.Point(259, 365);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(134, 42);
            this.button11.TabIndex = 21;
            this.button11.Text = "满料标志";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // gbx_handle
            // 
            this.gbx_handle.Controls.Add(this.button12);
            this.gbx_handle.Controls.Add(this.button9);
            this.gbx_handle.Controls.Add(this.button7);
            this.gbx_handle.Controls.Add(this.button11);
            this.gbx_handle.Controls.Add(this.button6);
            this.gbx_handle.Controls.Add(this.button5);
            this.gbx_handle.Controls.Add(this.button8);
            this.gbx_handle.Controls.Add(this.button4);
            this.gbx_handle.Controls.Add(this.button10);
            this.gbx_handle.Controls.Add(this.button3);
            this.gbx_handle.Controls.Add(this.txt_snMsg);
            this.gbx_handle.Controls.Add(this.button2);
            this.gbx_handle.Controls.Add(this.btn_inputSN);
            this.gbx_handle.Controls.Add(this.button1);
            this.gbx_handle.Controls.Add(this.label1);
            this.gbx_handle.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gbx_handle.Location = new System.Drawing.Point(519, 22);
            this.gbx_handle.Name = "gbx_handle";
            this.gbx_handle.Size = new System.Drawing.Size(431, 591);
            this.gbx_handle.TabIndex = 22;
            this.gbx_handle.TabStop = false;
            this.gbx_handle.Text = "手动调试按钮";
            // 
            // button12
            // 
            this.button12.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button12.Location = new System.Drawing.Point(250, 523);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(143, 42);
            this.button12.TabIndex = 22;
            this.button12.Text = "测序所需数据量0Mb";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // MainForm_Msg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(992, 625);
            this.Controls.Add(this.gbx_handle);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm_Msg";
            this.Text = "StationMsgForm";
            this.Load += new System.EventHandler(this.MainForm_Msg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbx_handle.ResumeLayout(false);
            this.gbx_handle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Label lab_Robot_state;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label lab_Sequence_state;
        public System.Windows.Forms.Label lab_Feed_state;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.Label lab_Carry_state;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.Label lab_claw;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.Label lab_gun;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.Label lab_huanliao;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label lab_buliao;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        public System.Windows.Forms.Label lab_carryRun;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label lab_data_state;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label lab_clawrobot;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_snMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_inputSN;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.GroupBox gbx_handle;
        private System.Windows.Forms.Button button12;
    }
}