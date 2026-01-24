namespace CYStandardProcedure
{
    partial class BaseChartForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lab_totalDNA = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lab_matchDNA = new System.Windows.Forms.Label();
            this.btnSelectFolder = new CYCustomControl.RoundButton();
            this.btnExec = new CYCustomControl.RoundButton();
            this.txtIDNA = new System.Windows.Forms.TextBox();
            this.lblExecMsg = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lab_matchJianJi = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cbx_barcode = new System.Windows.Forms.ComboBox();
            this.lab_jianjiMax = new System.Windows.Forms.Label();
            this.txt_JianJiMsg = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtFolderPath);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lab_totalDNA);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lab_matchDNA);
            this.panel1.Controls.Add(this.btnSelectFolder);
            this.panel1.Controls.Add(this.btnExec);
            this.panel1.Controls.Add(this.txtIDNA);
            this.panel1.Controls.Add(this.lblExecMsg);
            this.panel1.Controls.Add(this.lblCount);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(806, 144);
            this.panel1.TabIndex = 12;
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFolderPath.Location = new System.Drawing.Point(198, 55);
            this.txtFolderPath.Margin = new System.Windows.Forms.Padding(2);
            this.txtFolderPath.Multiline = true;
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(313, 26);
            this.txtFolderPath.TabIndex = 219;
            this.txtFolderPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(16, 108);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 21);
            this.label4.TabIndex = 218;
            this.label4.Text = "链条数:";
            // 
            // lab_totalDNA
            // 
            this.lab_totalDNA.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_totalDNA.Location = new System.Drawing.Point(82, 101);
            this.lab_totalDNA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_totalDNA.Name = "lab_totalDNA";
            this.lab_totalDNA.Size = new System.Drawing.Size(81, 36);
            this.lab_totalDNA.TabIndex = 217;
            this.lab_totalDNA.Text = "0";
            this.lab_totalDNA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(282, 108);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 21);
            this.label3.TabIndex = 216;
            this.label3.Text = "iDNA正确配对条数:";
            // 
            // lab_matchDNA
            // 
            this.lab_matchDNA.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_matchDNA.Location = new System.Drawing.Point(436, 101);
            this.lab_matchDNA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_matchDNA.Name = "lab_matchDNA";
            this.lab_matchDNA.Size = new System.Drawing.Size(75, 36);
            this.lab_matchDNA.TabIndex = 215;
            this.lab_matchDNA.Text = "0";
            this.lab_matchDNA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.btnSelectFolder.Location = new System.Drawing.Point(39, 51);
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
            this.btnExec.Location = new System.Drawing.Point(519, 51);
            this.btnExec.Name = "btnExec";
            this.btnExec.Radius = 24;
            this.btnExec.Size = new System.Drawing.Size(102, 36);
            this.btnExec.SpliteButtonWidth = 18;
            this.btnExec.TabIndex = 213;
            this.btnExec.Text = "执行处理";
            this.btnExec.UseVisualStyleBackColor = false;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // txtIDNA
            // 
            this.txtIDNA.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtIDNA.Location = new System.Drawing.Point(198, 13);
            this.txtIDNA.Margin = new System.Windows.Forms.Padding(2);
            this.txtIDNA.Multiline = true;
            this.txtIDNA.Name = "txtIDNA";
            this.txtIDNA.Size = new System.Drawing.Size(313, 26);
            this.txtIDNA.TabIndex = 9;
            this.txtIDNA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblExecMsg
            // 
            this.lblExecMsg.BackColor = System.Drawing.Color.White;
            this.lblExecMsg.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblExecMsg.Location = new System.Drawing.Point(631, 51);
            this.lblExecMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExecMsg.Name = "lblExecMsg";
            this.lblExecMsg.Size = new System.Drawing.Size(132, 36);
            this.lblExecMsg.TabIndex = 7;
            this.lblExecMsg.Text = "0/0";
            this.lblExecMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCount
            // 
            this.lblCount.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCount.Location = new System.Drawing.Point(631, 9);
            this.lblCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(132, 36);
            this.lblCount.TabIndex = 10;
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(16, 13);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 21);
            this.label2.TabIndex = 8;
            this.label2.Text = "用于匹配的靶向iDNA：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(515, 294);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 21);
            this.label1.TabIndex = 220;
            this.label1.Text = "iDNA及碱基正确配对条数:";
            this.label1.Visible = false;
            // 
            // lab_matchJianJi
            // 
            this.lab_matchJianJi.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_matchJianJi.Location = new System.Drawing.Point(651, 294);
            this.lab_matchJianJi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_matchJianJi.Name = "lab_matchJianJi";
            this.lab_matchJianJi.Size = new System.Drawing.Size(74, 36);
            this.lab_matchJianJi.TabIndex = 219;
            this.lab_matchJianJi.Text = "0";
            this.lab_matchJianJi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lab_matchJianJi.Visible = false;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend1.IsTextAutoFit = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(179, 219);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(615, 366);
            this.chart1.TabIndex = 13;
            this.chart1.Text = "chart1";
            // 
            // cbx_barcode
            // 
            this.cbx_barcode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_barcode.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_barcode.FormattingEnabled = true;
            this.cbx_barcode.Location = new System.Drawing.Point(20, 186);
            this.cbx_barcode.Name = "cbx_barcode";
            this.cbx_barcode.Size = new System.Drawing.Size(152, 24);
            this.cbx_barcode.TabIndex = 223;
            this.cbx_barcode.SelectedIndexChanged += new System.EventHandler(this.cbx_barcode_SelectedIndexChanged);
            // 
            // lab_jianjiMax
            // 
            this.lab_jianjiMax.AutoSize = true;
            this.lab_jianjiMax.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_jianjiMax.Location = new System.Drawing.Point(346, 170);
            this.lab_jianjiMax.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_jianjiMax.Name = "lab_jianjiMax";
            this.lab_jianjiMax.Size = new System.Drawing.Size(292, 21);
            this.lab_jianjiMax.TabIndex = 224;
            this.lab_jianjiMax.Text = "测序结果(饼状图中百分比最高的碱基)：";
            // 
            // txt_JianJiMsg
            // 
            this.txt_JianJiMsg.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_JianJiMsg.Location = new System.Drawing.Point(20, 291);
            this.txt_JianJiMsg.Margin = new System.Windows.Forms.Padding(2);
            this.txt_JianJiMsg.Multiline = true;
            this.txt_JianJiMsg.Name = "txt_JianJiMsg";
            this.txt_JianJiMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_JianJiMsg.Size = new System.Drawing.Size(152, 290);
            this.txt_JianJiMsg.TabIndex = 226;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(16, 255);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(158, 21);
            this.label5.TabIndex = 227;
            this.label5.Text = "碱基及数量具体内容:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(17, 166);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 17);
            this.label6.TabIndex = 234;
            this.label6.Text = "标签号:";
            // 
            // BaseChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(806, 597);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_JianJiMsg);
            this.Controls.Add(this.lab_jianjiMax);
            this.Controls.Add(this.cbx_barcode);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lab_matchJianJi);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BaseChartForm";
            this.Text = "BaseChartForm";
            this.Load += new System.EventHandler(this.BaseChartForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lab_matchJianJi;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lab_totalDNA;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lab_matchDNA;
        private CYCustomControl.RoundButton btnSelectFolder;
        private CYCustomControl.RoundButton btnExec;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.TextBox txtIDNA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblExecMsg;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ComboBox cbx_barcode;
        private System.Windows.Forms.Label lab_jianjiMax;
        private System.Windows.Forms.TextBox txt_JianJiMsg;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFolderPath;
    }
}