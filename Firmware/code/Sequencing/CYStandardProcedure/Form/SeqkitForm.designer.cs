namespace CYStandardProcedure
{
    partial class SeqkitForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvResult = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txt_JianJi = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lab_matchJianJi = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lab_totalDNA = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lab_matchDNA = new System.Windows.Forms.Label();
            this.btnSelectFolder = new CYCustomControl.RoundButton();
            this.btnExec = new CYCustomControl.RoundButton();
            this.lblCount = new System.Windows.Forms.Label();
            this.txtIDNA = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblExecMsg = new System.Windows.Forms.Label();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvResult
            // 
            this.dgvResult.AllowUserToAddRows = false;
            this.dgvResult.AllowUserToDeleteRows = false;
            this.dgvResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResult.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResult.Location = new System.Drawing.Point(0, 214);
            this.dgvResult.Margin = new System.Windows.Forms.Padding(2);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.ReadOnly = true;
            this.dgvResult.RowTemplate.Height = 27;
            this.dgvResult.Size = new System.Drawing.Size(806, 383);
            this.dgvResult.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txt_JianJi);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lab_matchJianJi);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lab_totalDNA);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lab_matchDNA);
            this.panel1.Controls.Add(this.btnSelectFolder);
            this.panel1.Controls.Add(this.btnExec);
            this.panel1.Controls.Add(this.lblCount);
            this.panel1.Controls.Add(this.txtIDNA);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblExecMsg);
            this.panel1.Controls.Add(this.txtFolderPath);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(806, 214);
            this.panel1.TabIndex = 11;
            // 
            // txt_JianJi
            // 
            this.txt_JianJi.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_JianJi.Location = new System.Drawing.Point(137, 107);
            this.txt_JianJi.Margin = new System.Windows.Forms.Padding(2);
            this.txt_JianJi.Multiline = true;
            this.txt_JianJi.Name = "txt_JianJi";
            this.txt_JianJi.Size = new System.Drawing.Size(244, 26);
            this.txt_JianJi.TabIndex = 222;
            this.txt_JianJi.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(41, 110);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 21);
            this.label5.TabIndex = 221;
            this.label5.Text = "碱基：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(496, 168);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 21);
            this.label1.TabIndex = 220;
            this.label1.Text = "iDNA及碱基正确配对条数:";
            // 
            // lab_matchJianJi
            // 
            this.lab_matchJianJi.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_matchJianJi.Location = new System.Drawing.Point(698, 161);
            this.lab_matchJianJi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_matchJianJi.Name = "lab_matchJianJi";
            this.lab_matchJianJi.Size = new System.Drawing.Size(74, 36);
            this.lab_matchJianJi.TabIndex = 219;
            this.lab_matchJianJi.Text = "0";
            this.lab_matchJianJi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(21, 168);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 21);
            this.label4.TabIndex = 218;
            this.label4.Text = "链条数:";
            // 
            // lab_totalDNA
            // 
            this.lab_totalDNA.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_totalDNA.Location = new System.Drawing.Point(87, 161);
            this.lab_totalDNA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_totalDNA.Name = "lab_totalDNA";
            this.lab_totalDNA.Size = new System.Drawing.Size(81, 36);
            this.lab_totalDNA.TabIndex = 217;
            this.lab_totalDNA.Text = "0";
            this.lab_totalDNA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(224, 168);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 21);
            this.label3.TabIndex = 216;
            this.label3.Text = "iDNA正确配对条数:";
            // 
            // lab_matchDNA
            // 
            this.lab_matchDNA.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_matchDNA.Location = new System.Drawing.Point(378, 161);
            this.lab_matchDNA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_matchDNA.Name = "lab_matchDNA";
            this.lab_matchDNA.Size = new System.Drawing.Size(75, 36);
            this.lab_matchDNA.TabIndex = 215;
            this.lab_matchDNA.Text = "0";
            this.lab_matchDNA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelectFolder.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectFolder.BaseColor = System.Drawing.Color.Orange;
            this.btnSelectFolder.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.btnSelectFolder.FlatAppearance.BorderSize = 0;
            this.btnSelectFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectFolder.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btnSelectFolder.ImageHeight = 80;
            this.btnSelectFolder.ImageWidth = 80;
            this.btnSelectFolder.Location = new System.Drawing.Point(12, 17);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Radius = 24;
            this.btnSelectFolder.Size = new System.Drawing.Size(102, 36);
            this.btnSelectFolder.SpliteButtonWidth = 18;
            this.btnSelectFolder.TabIndex = 214;
            this.btnSelectFolder.Text = "选择文件夹";
            this.btnSelectFolder.UseVisualStyleBackColor = false;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // btnExec
            // 
            this.btnExec.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnExec.BackColor = System.Drawing.Color.Transparent;
            this.btnExec.BaseColor = System.Drawing.Color.DeepSkyBlue;
            this.btnExec.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.btnExec.FlatAppearance.BorderSize = 0;
            this.btnExec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExec.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btnExec.ImageHeight = 80;
            this.btnExec.ImageWidth = 80;
            this.btnExec.Location = new System.Drawing.Point(509, 103);
            this.btnExec.Name = "btnExec";
            this.btnExec.Radius = 24;
            this.btnExec.Size = new System.Drawing.Size(102, 36);
            this.btnExec.SpliteButtonWidth = 18;
            this.btnExec.TabIndex = 213;
            this.btnExec.Text = "执行处理";
            this.btnExec.UseVisualStyleBackColor = false;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // lblCount
            // 
            this.lblCount.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCount.Location = new System.Drawing.Point(636, 12);
            this.lblCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(159, 36);
            this.lblCount.TabIndex = 10;
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtIDNA
            // 
            this.txtIDNA.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtIDNA.Location = new System.Drawing.Point(137, 63);
            this.txtIDNA.Margin = new System.Windows.Forms.Padding(2);
            this.txtIDNA.Multiline = true;
            this.txtIDNA.Name = "txtIDNA";
            this.txtIDNA.Size = new System.Drawing.Size(474, 26);
            this.txtIDNA.TabIndex = 9;
            this.txtIDNA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(33, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 21);
            this.label2.TabIndex = 8;
            this.label2.Text = "iDNA：";
            // 
            // lblExecMsg
            // 
            this.lblExecMsg.BackColor = System.Drawing.Color.White;
            this.lblExecMsg.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblExecMsg.Location = new System.Drawing.Point(636, 103);
            this.lblExecMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExecMsg.Name = "lblExecMsg";
            this.lblExecMsg.Size = new System.Drawing.Size(155, 36);
            this.lblExecMsg.TabIndex = 7;
            this.lblExecMsg.Text = "0/0";
            this.lblExecMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFolderPath.Location = new System.Drawing.Point(137, 16);
            this.txtFolderPath.Margin = new System.Windows.Forms.Padding(2);
            this.txtFolderPath.Multiline = true;
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(474, 28);
            this.txtFolderPath.TabIndex = 5;
            this.txtFolderPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SeqkitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(806, 597);
            this.Controls.Add(this.dgvResult);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SeqkitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SeqkitForm  请先将 seqkit.exe 放置在 C:\\Windows\\System32";
            this.Load += new System.EventHandler(this.SeqkitForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvResult;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblExecMsg;
        private System.Windows.Forms.TextBox txtIDNA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblCount;
        private CYCustomControl.RoundButton btnExec;
        private CYCustomControl.RoundButton btnSelectFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lab_totalDNA;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lab_matchDNA;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lab_matchJianJi;
        private System.Windows.Forms.TextBox txt_JianJi;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtFolderPath;
    }
}