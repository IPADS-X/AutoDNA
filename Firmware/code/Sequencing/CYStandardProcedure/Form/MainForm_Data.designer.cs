namespace CYStandardProcedure
{
    partial class MainForm_Data
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
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lab_totalDNA = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lab_matchDNA = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_FolderPath = new System.Windows.Forms.TextBox();
            this.txt_IDNA = new System.Windows.Forms.TextBox();
            this.lblExecMsg = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.cbx_barcode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_JianJiMsg = new System.Windows.Forms.TextBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lab_jianjiMax = new System.Windows.Forms.Label();
            this.lab_ZongKongJJ = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lab_totalDNA);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lab_matchDNA);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txt_FolderPath);
            this.panel1.Controls.Add(this.txt_IDNA);
            this.panel1.Controls.Add(this.lblExecMsg);
            this.panel1.Controls.Add(this.lblCount);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(806, 144);
            this.panel1.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(2, 56);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 21);
            this.label2.TabIndex = 231;
            this.label2.Text = "用于匹配的靶向iDNA:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(101, 100);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 21);
            this.label4.TabIndex = 228;
            this.label4.Text = "链条数:";
            // 
            // lab_totalDNA
            // 
            this.lab_totalDNA.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_totalDNA.Location = new System.Drawing.Point(167, 93);
            this.lab_totalDNA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_totalDNA.Name = "lab_totalDNA";
            this.lab_totalDNA.Size = new System.Drawing.Size(81, 36);
            this.lab_totalDNA.TabIndex = 227;
            this.lab_totalDNA.Text = "0";
            this.lab_totalDNA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(369, 100);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 21);
            this.label3.TabIndex = 226;
            this.label3.Text = "iDNA正确配对总条数:";
            // 
            // lab_matchDNA
            // 
            this.lab_matchDNA.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_matchDNA.Location = new System.Drawing.Point(523, 93);
            this.lab_matchDNA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_matchDNA.Name = "lab_matchDNA";
            this.lab_matchDNA.Size = new System.Drawing.Size(81, 36);
            this.lab_matchDNA.TabIndex = 225;
            this.lab_matchDNA.Text = "0";
            this.lab_matchDNA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(33, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 25);
            this.label1.TabIndex = 16;
            this.label1.Text = "文件路径：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txt_FolderPath
            // 
            this.txt_FolderPath.Enabled = false;
            this.txt_FolderPath.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_FolderPath.Location = new System.Drawing.Point(171, 11);
            this.txt_FolderPath.Margin = new System.Windows.Forms.Padding(2);
            this.txt_FolderPath.Multiline = true;
            this.txt_FolderPath.Name = "txt_FolderPath";
            this.txt_FolderPath.Size = new System.Drawing.Size(461, 28);
            this.txt_FolderPath.TabIndex = 15;
            this.txt_FolderPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt_IDNA
            // 
            this.txt_IDNA.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_IDNA.Location = new System.Drawing.Point(171, 53);
            this.txt_IDNA.Margin = new System.Windows.Forms.Padding(2);
            this.txt_IDNA.Multiline = true;
            this.txt_IDNA.Name = "txt_IDNA";
            this.txt_IDNA.Size = new System.Drawing.Size(461, 26);
            this.txt_IDNA.TabIndex = 13;
            this.txt_IDNA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblExecMsg
            // 
            this.lblExecMsg.BackColor = System.Drawing.Color.White;
            this.lblExecMsg.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblExecMsg.Location = new System.Drawing.Point(651, 53);
            this.lblExecMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExecMsg.Name = "lblExecMsg";
            this.lblExecMsg.Size = new System.Drawing.Size(132, 36);
            this.lblExecMsg.TabIndex = 12;
            this.lblExecMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCount
            // 
            this.lblCount.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCount.Location = new System.Drawing.Point(651, 11);
            this.lblCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(132, 36);
            this.lblCount.TabIndex = 11;
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbx_barcode
            // 
            this.cbx_barcode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_barcode.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_barcode.FormattingEnabled = true;
            this.cbx_barcode.Location = new System.Drawing.Point(12, 193);
            this.cbx_barcode.Name = "cbx_barcode";
            this.cbx_barcode.Size = new System.Drawing.Size(122, 24);
            this.cbx_barcode.TabIndex = 224;
            this.cbx_barcode.SelectedIndexChanged += new System.EventHandler(this.cbx_barcode_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(15, 262);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 17);
            this.label5.TabIndex = 230;
            this.label5.Text = "碱基及数量具体内容:";
            // 
            // txt_JianJiMsg
            // 
            this.txt_JianJiMsg.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_JianJiMsg.Location = new System.Drawing.Point(20, 291);
            this.txt_JianJiMsg.Margin = new System.Windows.Forms.Padding(2);
            this.txt_JianJiMsg.Multiline = true;
            this.txt_JianJiMsg.Name = "txt_JianJiMsg";
            this.txt_JianJiMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_JianJiMsg.Size = new System.Drawing.Size(114, 290);
            this.txt_JianJiMsg.TabIndex = 229;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend1.IsTextAutoFit = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(139, 219);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(623, 366);
            this.chart1.TabIndex = 228;
            this.chart1.Text = "chart1";
            // 
            // lab_jianjiMax
            // 
            this.lab_jianjiMax.AutoSize = true;
            this.lab_jianjiMax.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_jianjiMax.Location = new System.Drawing.Point(369, 173);
            this.lab_jianjiMax.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_jianjiMax.Name = "lab_jianjiMax";
            this.lab_jianjiMax.Size = new System.Drawing.Size(292, 21);
            this.lab_jianjiMax.TabIndex = 231;
            this.lab_jianjiMax.Text = "测序结果(饼状图中百分比最高的碱基)：";
            // 
            // lab_ZongKongJJ
            // 
            this.lab_ZongKongJJ.AutoSize = true;
            this.lab_ZongKongJJ.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_ZongKongJJ.Location = new System.Drawing.Point(180, 173);
            this.lab_ZongKongJJ.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lab_ZongKongJJ.Name = "lab_ZongKongJJ";
            this.lab_ZongKongJJ.Size = new System.Drawing.Size(110, 21);
            this.lab_ZongKongJJ.TabIndex = 232;
            this.lab_ZongKongJJ.Text = "总控传来碱基:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(11, 173);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 17);
            this.label6.TabIndex = 233;
            this.label6.Text = "标签号:";
            // 
            // MainForm_Data
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(806, 597);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lab_ZongKongJJ);
            this.Controls.Add(this.lab_jianjiMax);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_JianJiMsg);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.cbx_barcode);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm_Data";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.MainForm_Data_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label lblCount;
        public System.Windows.Forms.Label lblExecMsg;
        public System.Windows.Forms.TextBox txt_IDNA;
        public System.Windows.Forms.TextBox txt_FolderPath;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label lab_totalDNA;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label lab_matchDNA;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox cbx_barcode;
        public System.Windows.Forms.TextBox txt_JianJiMsg;
        public System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        public System.Windows.Forms.Label lab_jianjiMax;
        public System.Windows.Forms.Label lab_ZongKongJJ;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label5;
    }
}