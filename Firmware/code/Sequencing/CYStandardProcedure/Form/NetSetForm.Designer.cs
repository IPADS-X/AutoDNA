namespace CYStandardProcedure
{
    partial class NetSetForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.txt_Receive = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Send = new System.Windows.Forms.TextBox();
            this.cmb_Net = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btn_Receive = new CYCustomControl.RoundButton();
            this.btn_Send = new CYCustomControl.RoundButton();
            this.pic_NetStatus = new System.Windows.Forms.PictureBox();
            this.btn_Save = new CYCustomControl.RoundButton();
            this.btn_Connect = new CYCustomControl.RoundButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.rbt_queryPLC = new CYCustomControl.RoundButton();
            this.rbt_sendPLC = new CYCustomControl.RoundButton();
            this.txt_sendnum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_sendaddr = new System.Windows.Forms.TextBox();
            this.txt_queryaddr = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.rbt_connectPLC = new CYCustomControl.RoundButton();
            this.txt_queryresult = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbt_string = new System.Windows.Forms.RadioButton();
            this.rbt_float = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.cbx_receive = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbx_send = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txt_gen_taskid = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txt_gen_match = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_gen_total = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txt_gen_sn = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.rbt_workquery = new CYCustomControl.RoundButton();
            this.rbt_result = new CYCustomControl.RoundButton();
            this.rbt_finish = new CYCustomControl.RoundButton();
            this.rbt_start = new CYCustomControl.RoundButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_NetStatus)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(20, 456);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 158;
            this.label3.Text = "接收字符";
            // 
            // txt_Receive
            // 
            this.txt_Receive.Location = new System.Drawing.Point(125, 456);
            this.txt_Receive.Name = "txt_Receive";
            this.txt_Receive.Size = new System.Drawing.Size(264, 133);
            this.txt_Receive.TabIndex = 159;
            this.txt_Receive.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(20, 347);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 157;
            this.label2.Text = "发送字符";
            // 
            // txt_Send
            // 
            this.txt_Send.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_Send.Location = new System.Drawing.Point(125, 347);
            this.txt_Send.Multiline = true;
            this.txt_Send.Name = "txt_Send";
            this.txt_Send.Size = new System.Drawing.Size(264, 65);
            this.txt_Send.TabIndex = 165;
            // 
            // cmb_Net
            // 
            this.cmb_Net.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Net.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_Net.FormattingEnabled = true;
            this.cmb_Net.Location = new System.Drawing.Point(125, 252);
            this.cmb_Net.Name = "cmb_Net";
            this.cmb_Net.Size = new System.Drawing.Size(264, 24);
            this.cmb_Net.TabIndex = 172;
            this.cmb_Net.SelectedIndexChanged += new System.EventHandler(this.cmb_Net_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(20, 254);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 173;
            this.label1.Text = "网络列表";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1030, 208);
            this.dataGridView1.TabIndex = 188;
            // 
            // btn_Receive
            // 
            this.btn_Receive.BackColor = System.Drawing.Color.Transparent;
            this.btn_Receive.BackgroundImage = global::CYStandardProcedure.Properties.Resources.接收;
            this.btn_Receive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Receive.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Receive.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Receive.ContextOffset = 0;
            this.btn_Receive.FlatAppearance.BorderSize = 0;
            this.btn_Receive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Receive.ImageHeight = 80;
            this.btn_Receive.ImageWidth = 80;
            this.btn_Receive.Location = new System.Drawing.Point(411, 455);
            this.btn_Receive.Name = "btn_Receive";
            this.btn_Receive.Radius = 24;
            this.btn_Receive.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Receive.Size = new System.Drawing.Size(60, 53);
            this.btn_Receive.SpliteButtonWidth = 18;
            this.btn_Receive.TabIndex = 193;
            this.btn_Receive.UseVisualStyleBackColor = false;
            this.btn_Receive.Click += new System.EventHandler(this.btn_Receive_Click);
            // 
            // btn_Send
            // 
            this.btn_Send.BackColor = System.Drawing.Color.Transparent;
            this.btn_Send.BackgroundImage = global::CYStandardProcedure.Properties.Resources.发送;
            this.btn_Send.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Send.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Send.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Send.ContextOffset = 0;
            this.btn_Send.FlatAppearance.BorderSize = 0;
            this.btn_Send.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Send.ImageHeight = 80;
            this.btn_Send.ImageWidth = 80;
            this.btn_Send.Location = new System.Drawing.Point(411, 347);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Radius = 24;
            this.btn_Send.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Send.Size = new System.Drawing.Size(60, 53);
            this.btn_Send.SpliteButtonWidth = 18;
            this.btn_Send.TabIndex = 192;
            this.btn_Send.UseVisualStyleBackColor = false;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // pic_NetStatus
            // 
            this.pic_NetStatus.BackgroundImage = global::CYStandardProcedure.Properties.Resources.ConNG;
            this.pic_NetStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_NetStatus.Location = new System.Drawing.Point(411, 250);
            this.pic_NetStatus.Name = "pic_NetStatus";
            this.pic_NetStatus.Size = new System.Drawing.Size(64, 32);
            this.pic_NetStatus.TabIndex = 191;
            this.pic_NetStatus.TabStop = false;
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.Transparent;
            this.btn_Save.BackgroundImage = global::CYStandardProcedure.Properties.Resources.保存;
            this.btn_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Save.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Save.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Save.ContextOffset = 0;
            this.btn_Save.FlatAppearance.BorderSize = 0;
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.ImageHeight = 80;
            this.btn_Save.ImageWidth = 80;
            this.btn_Save.Location = new System.Drawing.Point(899, 226);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Radius = 24;
            this.btn_Save.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Save.Size = new System.Drawing.Size(52, 53);
            this.btn_Save.SpliteButtonWidth = 18;
            this.btn_Save.TabIndex = 190;
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Connect
            // 
            this.btn_Connect.BackColor = System.Drawing.Color.Transparent;
            this.btn_Connect.BackgroundImage = global::CYStandardProcedure.Properties.Resources.连接;
            this.btn_Connect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Connect.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Connect.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Connect.ContextOffset = 0;
            this.btn_Connect.FlatAppearance.BorderSize = 0;
            this.btn_Connect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Connect.ImageHeight = 80;
            this.btn_Connect.ImageWidth = 80;
            this.btn_Connect.Location = new System.Drawing.Point(990, 226);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Radius = 24;
            this.btn_Connect.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Connect.Size = new System.Drawing.Size(52, 53);
            this.btn_Connect.SpliteButtonWidth = 18;
            this.btn_Connect.TabIndex = 189;
            this.btn_Connect.UseVisualStyleBackColor = false;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(523, 250);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(120, 16);
            this.checkBox1.TabIndex = 194;
            this.checkBox1.Text = "自动接收显示信息";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // rbt_queryPLC
            // 
            this.rbt_queryPLC.BackColor = System.Drawing.Color.Transparent;
            this.rbt_queryPLC.BackgroundImage = global::CYStandardProcedure.Properties.Resources.接收;
            this.rbt_queryPLC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_queryPLC.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_queryPLC.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_queryPLC.ContextOffset = 0;
            this.rbt_queryPLC.FlatAppearance.BorderSize = 0;
            this.rbt_queryPLC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_queryPLC.ImageHeight = 80;
            this.rbt_queryPLC.ImageWidth = 80;
            this.rbt_queryPLC.Location = new System.Drawing.Point(428, 208);
            this.rbt_queryPLC.Name = "rbt_queryPLC";
            this.rbt_queryPLC.Radius = 24;
            this.rbt_queryPLC.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_queryPLC.Size = new System.Drawing.Size(60, 53);
            this.rbt_queryPLC.SpliteButtonWidth = 18;
            this.rbt_queryPLC.TabIndex = 199;
            this.rbt_queryPLC.UseVisualStyleBackColor = false;
            this.rbt_queryPLC.Click += new System.EventHandler(this.rbt_queryPLC_Click);
            // 
            // rbt_sendPLC
            // 
            this.rbt_sendPLC.BackColor = System.Drawing.Color.Transparent;
            this.rbt_sendPLC.BackgroundImage = global::CYStandardProcedure.Properties.Resources.发送;
            this.rbt_sendPLC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_sendPLC.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_sendPLC.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_sendPLC.ContextOffset = 0;
            this.rbt_sendPLC.FlatAppearance.BorderSize = 0;
            this.rbt_sendPLC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_sendPLC.ImageHeight = 80;
            this.rbt_sendPLC.ImageWidth = 80;
            this.rbt_sendPLC.Location = new System.Drawing.Point(428, 103);
            this.rbt_sendPLC.Name = "rbt_sendPLC";
            this.rbt_sendPLC.Radius = 24;
            this.rbt_sendPLC.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_sendPLC.Size = new System.Drawing.Size(60, 53);
            this.rbt_sendPLC.SpliteButtonWidth = 18;
            this.rbt_sendPLC.TabIndex = 198;
            this.rbt_sendPLC.UseVisualStyleBackColor = false;
            this.rbt_sendPLC.Click += new System.EventHandler(this.rbt_sendPLC_Click);
            // 
            // txt_sendnum
            // 
            this.txt_sendnum.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_sendnum.Location = new System.Drawing.Point(296, 144);
            this.txt_sendnum.Name = "txt_sendnum";
            this.txt_sendnum.Size = new System.Drawing.Size(84, 26);
            this.txt_sendnum.TabIndex = 197;
            this.txt_sendnum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(218, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 20);
            this.label4.TabIndex = 194;
            this.label4.Text = "发送数字 :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(218, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 20);
            this.label6.TabIndex = 200;
            this.label6.Text = "发送地址 :";
            // 
            // txt_sendaddr
            // 
            this.txt_sendaddr.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_sendaddr.Location = new System.Drawing.Point(296, 100);
            this.txt_sendaddr.Name = "txt_sendaddr";
            this.txt_sendaddr.Size = new System.Drawing.Size(84, 26);
            this.txt_sendaddr.TabIndex = 201;
            this.txt_sendaddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_queryaddr
            // 
            this.txt_queryaddr.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_queryaddr.Location = new System.Drawing.Point(296, 200);
            this.txt_queryaddr.Name = "txt_queryaddr";
            this.txt_queryaddr.Size = new System.Drawing.Size(84, 26);
            this.txt_queryaddr.TabIndex = 205;
            this.txt_queryaddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(218, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 20);
            this.label5.TabIndex = 204;
            this.label5.Text = "查询地址 :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(218, 245);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 20);
            this.label7.TabIndex = 202;
            this.label7.Text = "查询结果 :";
            // 
            // rbt_connectPLC
            // 
            this.rbt_connectPLC.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_connectPLC.BackColor = System.Drawing.Color.Transparent;
            this.rbt_connectPLC.BaseColor = System.Drawing.Color.PaleGoldenrod;
            this.rbt_connectPLC.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_connectPLC.FlatAppearance.BorderSize = 0;
            this.rbt_connectPLC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_connectPLC.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_connectPLC.ImageHeight = 80;
            this.rbt_connectPLC.ImageWidth = 80;
            this.rbt_connectPLC.Location = new System.Drawing.Point(51, 20);
            this.rbt_connectPLC.Name = "rbt_connectPLC";
            this.rbt_connectPLC.Radius = 24;
            this.rbt_connectPLC.Size = new System.Drawing.Size(100, 36);
            this.rbt_connectPLC.SpliteButtonWidth = 18;
            this.rbt_connectPLC.TabIndex = 213;
            this.rbt_connectPLC.Text = "连接PLC";
            this.rbt_connectPLC.UseVisualStyleBackColor = false;
            this.rbt_connectPLC.Click += new System.EventHandler(this.rbt_connectPLC_Click);
            // 
            // txt_queryresult
            // 
            this.txt_queryresult.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_queryresult.Location = new System.Drawing.Point(296, 244);
            this.txt_queryresult.Name = "txt_queryresult";
            this.txt_queryresult.Size = new System.Drawing.Size(84, 26);
            this.txt_queryresult.TabIndex = 203;
            this.txt_queryresult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cbx_receive);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cbx_send);
            this.groupBox1.Controls.Add(this.txt_queryaddr);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txt_queryresult);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txt_sendaddr);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.rbt_queryPLC);
            this.groupBox1.Controls.Add(this.rbt_sendPLC);
            this.groupBox1.Controls.Add(this.txt_sendnum);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(518, 298);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(524, 291);
            this.groupBox1.TabIndex = 195;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PLC通讯测试";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbt_string);
            this.groupBox2.Controls.Add(this.rbt_float);
            this.groupBox2.Controls.Add(this.rbt_connectPLC);
            this.groupBox2.Location = new System.Drawing.Point(5, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(512, 64);
            this.groupBox2.TabIndex = 196;
            this.groupBox2.TabStop = false;
            // 
            // rbt_string
            // 
            this.rbt_string.AutoSize = true;
            this.rbt_string.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbt_string.Location = new System.Drawing.Point(351, 25);
            this.rbt_string.Name = "rbt_string";
            this.rbt_string.Size = new System.Drawing.Size(76, 25);
            this.rbt_string.TabIndex = 215;
            this.rbt_string.Text = "字符串";
            this.rbt_string.UseVisualStyleBackColor = true;
            this.rbt_string.CheckedChanged += new System.EventHandler(this.rbt_string_CheckedChanged);
            // 
            // rbt_float
            // 
            this.rbt_float.AutoSize = true;
            this.rbt_float.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbt_float.Location = new System.Drawing.Point(217, 25);
            this.rbt_float.Name = "rbt_float";
            this.rbt_float.Size = new System.Drawing.Size(76, 25);
            this.rbt_float.TabIndex = 214;
            this.rbt_float.Text = "浮点数";
            this.rbt_float.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(12, 224);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 20);
            this.label9.TabIndex = 218;
            this.label9.Text = "读取方式 :";
            // 
            // cbx_receive
            // 
            this.cbx_receive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_receive.FormattingEnabled = true;
            this.cbx_receive.Items.AddRange(new object[] {
            "ABCD",
            "BADC",
            "CDAB",
            "DCBA"});
            this.cbx_receive.Location = new System.Drawing.Point(90, 220);
            this.cbx_receive.Name = "cbx_receive";
            this.cbx_receive.Size = new System.Drawing.Size(104, 30);
            this.cbx_receive.TabIndex = 217;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(12, 119);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 20);
            this.label8.TabIndex = 216;
            this.label8.Text = "写入方式 :";
            // 
            // cbx_send
            // 
            this.cbx_send.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_send.FormattingEnabled = true;
            this.cbx_send.Items.AddRange(new object[] {
            "ABCD",
            "BADC",
            "CDAB",
            "DCBA"});
            this.cbx_send.Location = new System.Drawing.Point(90, 115);
            this.cbx_send.Name = "cbx_send";
            this.cbx_send.Size = new System.Drawing.Size(104, 30);
            this.cbx_send.TabIndex = 215;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txt_gen_taskid);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.txt_gen_match);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.txt_gen_total);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.txt_gen_sn);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.rbt_workquery);
            this.groupBox3.Controls.Add(this.rbt_result);
            this.groupBox3.Controls.Add(this.rbt_finish);
            this.groupBox3.Controls.Add(this.rbt_start);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(340, 595);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(702, 117);
            this.groupBox3.TabIndex = 196;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "总控通讯测试";
            // 
            // txt_gen_taskid
            // 
            this.txt_gen_taskid.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_gen_taskid.Location = new System.Drawing.Point(234, 34);
            this.txt_gen_taskid.Name = "txt_gen_taskid";
            this.txt_gen_taskid.Size = new System.Drawing.Size(84, 26);
            this.txt_gen_taskid.TabIndex = 229;
            this.txt_gen_taskid.Text = "1";
            this.txt_gen_taskid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(167, 34);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 20);
            this.label13.TabIndex = 228;
            this.label13.Text = "TaskID :";
            // 
            // txt_gen_match
            // 
            this.txt_gen_match.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_gen_match.Location = new System.Drawing.Point(608, 34);
            this.txt_gen_match.Name = "txt_gen_match";
            this.txt_gen_match.Size = new System.Drawing.Size(84, 26);
            this.txt_gen_match.TabIndex = 227;
            this.txt_gen_match.Text = "9999";
            this.txt_gen_match.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(516, 34);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 20);
            this.label11.TabIndex = 226;
            this.label11.Text = "正确匹配数 :";
            // 
            // txt_gen_total
            // 
            this.txt_gen_total.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_gen_total.Location = new System.Drawing.Point(416, 34);
            this.txt_gen_total.Name = "txt_gen_total";
            this.txt_gen_total.Size = new System.Drawing.Size(84, 26);
            this.txt_gen_total.TabIndex = 225;
            this.txt_gen_total.Text = "99999";
            this.txt_gen_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(338, 34);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 20);
            this.label12.TabIndex = 224;
            this.label12.Text = "链条总数 :";
            // 
            // txt_gen_sn
            // 
            this.txt_gen_sn.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_gen_sn.Location = new System.Drawing.Point(63, 34);
            this.txt_gen_sn.Name = "txt_gen_sn";
            this.txt_gen_sn.Size = new System.Drawing.Size(84, 26);
            this.txt_gen_sn.TabIndex = 221;
            this.txt_gen_sn.Text = "B00001";
            this.txt_gen_sn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(19, 34);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 20);
            this.label10.TabIndex = 220;
            this.label10.Text = "SN :";
            // 
            // rbt_workquery
            // 
            this.rbt_workquery.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_workquery.BackColor = System.Drawing.Color.Transparent;
            this.rbt_workquery.BaseColor = System.Drawing.Color.Moccasin;
            this.rbt_workquery.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_workquery.FlatAppearance.BorderSize = 0;
            this.rbt_workquery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_workquery.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_workquery.ImageHeight = 80;
            this.rbt_workquery.ImageWidth = 80;
            this.rbt_workquery.Location = new System.Drawing.Point(590, 75);
            this.rbt_workquery.Name = "rbt_workquery";
            this.rbt_workquery.Radius = 24;
            this.rbt_workquery.Size = new System.Drawing.Size(100, 36);
            this.rbt_workquery.SpliteButtonWidth = 18;
            this.rbt_workquery.TabIndex = 217;
            this.rbt_workquery.Text = "后续有无任务";
            this.rbt_workquery.UseVisualStyleBackColor = false;
            this.rbt_workquery.Click += new System.EventHandler(this.rbt_workquery_Click);
            // 
            // rbt_result
            // 
            this.rbt_result.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_result.BackColor = System.Drawing.Color.Transparent;
            this.rbt_result.BaseColor = System.Drawing.Color.Pink;
            this.rbt_result.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_result.FlatAppearance.BorderSize = 0;
            this.rbt_result.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_result.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_result.ImageHeight = 80;
            this.rbt_result.ImageWidth = 80;
            this.rbt_result.Location = new System.Drawing.Point(401, 75);
            this.rbt_result.Name = "rbt_result";
            this.rbt_result.Radius = 24;
            this.rbt_result.Size = new System.Drawing.Size(100, 36);
            this.rbt_result.SpliteButtonWidth = 18;
            this.rbt_result.TabIndex = 216;
            this.rbt_result.Text = "测序结果上报";
            this.rbt_result.UseVisualStyleBackColor = false;
            this.rbt_result.Click += new System.EventHandler(this.rbt_result_Click);
            // 
            // rbt_finish
            // 
            this.rbt_finish.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_finish.BackColor = System.Drawing.Color.Transparent;
            this.rbt_finish.BaseColor = System.Drawing.Color.PaleTurquoise;
            this.rbt_finish.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_finish.FlatAppearance.BorderSize = 0;
            this.rbt_finish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_finish.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.rbt_finish.ImageHeight = 80;
            this.rbt_finish.ImageWidth = 80;
            this.rbt_finish.Location = new System.Drawing.Point(212, 75);
            this.rbt_finish.Name = "rbt_finish";
            this.rbt_finish.Radius = 24;
            this.rbt_finish.Size = new System.Drawing.Size(100, 36);
            this.rbt_finish.SpliteButtonWidth = 18;
            this.rbt_finish.TabIndex = 215;
            this.rbt_finish.Text = "完成时上报";
            this.rbt_finish.UseVisualStyleBackColor = false;
            this.rbt_finish.Click += new System.EventHandler(this.rbt_finish_Click);
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
            this.rbt_start.Location = new System.Drawing.Point(23, 75);
            this.rbt_start.Name = "rbt_start";
            this.rbt_start.Radius = 24;
            this.rbt_start.Size = new System.Drawing.Size(100, 36);
            this.rbt_start.SpliteButtonWidth = 18;
            this.rbt_start.TabIndex = 214;
            this.rbt_start.Text = "开始时上报";
            this.rbt_start.UseVisualStyleBackColor = false;
            this.rbt_start.Click += new System.EventHandler(this.rbt_start_Click);
            // 
            // NetSetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1054, 718);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btn_Receive);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.pic_NetStatus);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Connect);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmb_Net);
            this.Controls.Add(this.txt_Send);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_Receive);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NetSetForm";
            this.Text = "通讯测试";
            this.Load += new System.EventHandler(this.SerialSetForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_NetStatus)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox txt_Receive;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Send;
        private System.Windows.Forms.ComboBox cmb_Net;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private CYCustomControl.RoundButton btn_Connect;
        private CYCustomControl.RoundButton btn_Save;
        private System.Windows.Forms.PictureBox pic_NetStatus;
        private CYCustomControl.RoundButton btn_Send;
        private CYCustomControl.RoundButton btn_Receive;
        private System.Windows.Forms.CheckBox checkBox1;
        private CYCustomControl.RoundButton rbt_queryPLC;
        private CYCustomControl.RoundButton rbt_sendPLC;
        private System.Windows.Forms.TextBox txt_sendnum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_queryaddr;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_sendaddr;
        private System.Windows.Forms.Label label6;
        private CYCustomControl.RoundButton rbt_connectPLC;
        private System.Windows.Forms.TextBox txt_queryresult;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbx_receive;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbx_send;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbt_string;
        private System.Windows.Forms.RadioButton rbt_float;
        private System.Windows.Forms.GroupBox groupBox3;
        private CYCustomControl.RoundButton rbt_workquery;
        private CYCustomControl.RoundButton rbt_result;
        private CYCustomControl.RoundButton rbt_finish;
        private CYCustomControl.RoundButton rbt_start;
        private System.Windows.Forms.TextBox txt_gen_match;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_gen_total;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txt_gen_sn;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_gen_taskid;
        private System.Windows.Forms.Label label13;
    }
}