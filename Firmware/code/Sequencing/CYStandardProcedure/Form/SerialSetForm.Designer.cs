namespace CYStandardProcedure
{
    partial class SerialSetForm
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
            this.btn_Save = new CYCustomControl.RoundButton();
            this.btn_Open = new CYCustomControl.RoundButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.cmb_Serial = new System.Windows.Forms.ComboBox();
            this.txt_Send = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Receive = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Receive = new CYCustomControl.RoundButton();
            this.btn_Send = new CYCustomControl.RoundButton();
            this.pic_SerialStatus = new System.Windows.Forms.PictureBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.richTextBox4 = new System.Windows.Forms.RichTextBox();
            this.cbx_wenkongbiao = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_SerialStatus)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            this.btn_Save.Location = new System.Drawing.Point(898, 244);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Radius = 24;
            this.btn_Save.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Save.Size = new System.Drawing.Size(52, 53);
            this.btn_Save.SpliteButtonWidth = 18;
            this.btn_Save.TabIndex = 193;
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Open
            // 
            this.btn_Open.BackColor = System.Drawing.Color.Transparent;
            this.btn_Open.BackgroundImage = global::CYStandardProcedure.Properties.Resources.连接;
            this.btn_Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Open.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Open.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Open.ContextOffset = 0;
            this.btn_Open.FlatAppearance.BorderSize = 0;
            this.btn_Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Open.ImageHeight = 80;
            this.btn_Open.ImageWidth = 80;
            this.btn_Open.Location = new System.Drawing.Point(989, 244);
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.Radius = 24;
            this.btn_Open.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Open.Size = new System.Drawing.Size(52, 53);
            this.btn_Open.SpliteButtonWidth = 18;
            this.btn_Open.TabIndex = 192;
            this.btn_Open.UseVisualStyleBackColor = false;
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(11, 10);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1030, 228);
            this.dataGridView1.TabIndex = 191;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(19, 280);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 199;
            this.label1.Text = "串口列表";
            // 
            // cmb_Serial
            // 
            this.cmb_Serial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Serial.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_Serial.FormattingEnabled = true;
            this.cmb_Serial.Location = new System.Drawing.Point(181, 280);
            this.cmb_Serial.Name = "cmb_Serial";
            this.cmb_Serial.Size = new System.Drawing.Size(210, 24);
            this.cmb_Serial.TabIndex = 198;
            this.cmb_Serial.SelectedIndexChanged += new System.EventHandler(this.cmb_Serial_SelectedIndexChanged);
            // 
            // txt_Send
            // 
            this.txt_Send.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_Send.Location = new System.Drawing.Point(181, 353);
            this.txt_Send.Name = "txt_Send";
            this.txt_Send.Size = new System.Drawing.Size(210, 26);
            this.txt_Send.TabIndex = 197;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(19, 351);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 194;
            this.label2.Text = "发送字符";
            // 
            // txt_Receive
            // 
            this.txt_Receive.Location = new System.Drawing.Point(181, 428);
            this.txt_Receive.Name = "txt_Receive";
            this.txt_Receive.Size = new System.Drawing.Size(210, 147);
            this.txt_Receive.TabIndex = 196;
            this.txt_Receive.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(19, 427);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 195;
            this.label3.Text = "接收字符";
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
            this.btn_Receive.Location = new System.Drawing.Point(420, 430);
            this.btn_Receive.Name = "btn_Receive";
            this.btn_Receive.Radius = 24;
            this.btn_Receive.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Receive.Size = new System.Drawing.Size(60, 53);
            this.btn_Receive.SpliteButtonWidth = 18;
            this.btn_Receive.TabIndex = 208;
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
            this.btn_Send.Location = new System.Drawing.Point(420, 355);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Radius = 24;
            this.btn_Send.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Send.Size = new System.Drawing.Size(60, 53);
            this.btn_Send.SpliteButtonWidth = 18;
            this.btn_Send.TabIndex = 207;
            this.btn_Send.UseVisualStyleBackColor = false;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // pic_SerialStatus
            // 
            this.pic_SerialStatus.BackgroundImage = global::CYStandardProcedure.Properties.Resources.ConNG;
            this.pic_SerialStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_SerialStatus.Location = new System.Drawing.Point(420, 280);
            this.pic_SerialStatus.Name = "pic_SerialStatus";
            this.pic_SerialStatus.Size = new System.Drawing.Size(64, 32);
            this.pic_SerialStatus.TabIndex = 206;
            this.pic_SerialStatus.TabStop = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(534, 276);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(120, 16);
            this.checkBox1.TabIndex = 209;
            this.checkBox1.Text = "自动接收显示信息";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.richTextBox4);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(702, 454);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(260, 167);
            this.groupBox3.TabIndex = 238;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "温控表指令";
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
            this.richTextBox4.Size = new System.Drawing.Size(254, 145);
            this.richTextBox4.TabIndex = 234;
            this.richTextBox4.Text = "    查询当前温度指令:\n       read-query\n\n    设置温度指令:\n       set-温度  ( 例: 2-37 )\n     ";
            // 
            // cbx_wenkongbiao
            // 
            this.cbx_wenkongbiao.AutoSize = true;
            this.cbx_wenkongbiao.Checked = true;
            this.cbx_wenkongbiao.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbx_wenkongbiao.Location = new System.Drawing.Point(534, 313);
            this.cbx_wenkongbiao.Name = "cbx_wenkongbiao";
            this.cbx_wenkongbiao.Size = new System.Drawing.Size(84, 16);
            this.cbx_wenkongbiao.TabIndex = 239;
            this.cbx_wenkongbiao.Text = "温控表通讯";
            this.cbx_wenkongbiao.UseVisualStyleBackColor = true;
            // 
            // SerialSetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1054, 689);
            this.Controls.Add(this.cbx_wenkongbiao);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btn_Receive);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.pic_SerialStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmb_Serial);
            this.Controls.Add(this.txt_Send);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_Receive);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Open);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SerialSetForm";
            this.Text = "通讯测试";
            this.Load += new System.EventHandler(this.SerialSetForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_SerialStatus)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CYCustomControl.RoundButton btn_Save;
        private CYCustomControl.RoundButton btn_Open;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmb_Serial;
        private System.Windows.Forms.TextBox txt_Send;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txt_Receive;
        private System.Windows.Forms.Label label3;
        private CYCustomControl.RoundButton btn_Receive;
        private CYCustomControl.RoundButton btn_Send;
        private System.Windows.Forms.PictureBox pic_SerialStatus;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox richTextBox4;
        private System.Windows.Forms.CheckBox cbx_wenkongbiao;
    }
}