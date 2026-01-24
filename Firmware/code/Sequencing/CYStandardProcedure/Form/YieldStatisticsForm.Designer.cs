namespace CYStandardProcedure
{
    partial class YieldStatisticsForm
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
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YieldStatisticsForm));
            this.chart_Yield = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart_Week = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart_Statist = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_HandHour = new System.Windows.Forms.TextBox();
            this.txt_HnadMin = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btn_Write = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Yield)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Week)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Statist)).BeginInit();
            this.SuspendLayout();
            // 
            // chart_Yield
            // 
            this.chart_Yield.BackColor = System.Drawing.Color.AntiqueWhite;
            this.chart_Yield.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_Yield.BorderlineColor = System.Drawing.Color.Maroon;
            this.chart_Yield.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart_Yield.BorderSkin.BackColor = System.Drawing.Color.Gainsboro;
            this.chart_Yield.BorderSkin.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart_Yield.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea1.Area3DStyle.Enable3D = true;
            chartArea1.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea1.Name = "ChartArea1";
            this.chart_Yield.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart_Yield.Legends.Add(legend1);
            this.chart_Yield.Location = new System.Drawing.Point(830, 12);
            this.chart_Yield.Name = "chart_Yield";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series1.CustomProperties = "PieLabelStyle=Disabled";
            series1.Legend = "Legend1";
            series1.LegendText = "#AXISLABEL  [#PERCENT{P1}]";
            series1.Name = "Series1";
            this.chart_Yield.Series.Add(series1);
            this.chart_Yield.Size = new System.Drawing.Size(420, 367);
            this.chart_Yield.TabIndex = 19;
            this.chart_Yield.Text = "chart1";
            title1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title1.Name = "Title1";
            this.chart_Yield.Titles.Add(title1);
            // 
            // chart_Week
            // 
            this.chart_Week.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_Week.BackColor = System.Drawing.Color.AntiqueWhite;
            this.chart_Week.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_Week.BorderlineColor = System.Drawing.Color.Maroon;
            this.chart_Week.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart_Week.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea2.AxisX.Interval = 1D;
            chartArea2.AxisX.IsLabelAutoFit = false;
            chartArea2.AxisX.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea2.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea2.AxisY2.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea2.AxisY2.Interval = 20D;
            chartArea2.AxisY2.MajorGrid.Enabled = false;
            chartArea2.AxisY2.Maximum = 100D;
            chartArea2.AxisY2.Minimum = 0D;
            chartArea2.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea2.Name = "ChartArea1";
            this.chart_Week.ChartAreas.Add(chartArea2);
            legend2.Alignment = System.Drawing.StringAlignment.Center;
            legend2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            legend2.BorderColor = System.Drawing.Color.LightSkyBlue;
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend2.Name = "Legend1";
            this.chart_Week.Legends.Add(legend2);
            this.chart_Week.Location = new System.Drawing.Point(12, 379);
            this.chart_Week.Name = "chart_Week";
            series2.ChartArea = "ChartArea1";
            series2.Color = System.Drawing.Color.RoyalBlue;
            series2.CustomProperties = "PointWidth=0.3";
            series2.IsValueShownAsLabel = true;
            series2.LabelBackColor = System.Drawing.Color.Transparent;
            series2.LabelForeColor = System.Drawing.Color.DeepSkyBlue;
            series2.Legend = "Legend1";
            series2.LegendText = "总数";
            series2.MarkerBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            series2.MarkerColor = System.Drawing.Color.Red;
            series2.Name = "Series1";
            series3.ChartArea = "ChartArea1";
            series3.Color = System.Drawing.Color.LimeGreen;
            series3.CustomProperties = "PointWidth=0.3";
            series3.IsValueShownAsLabel = true;
            series3.LabelForeColor = System.Drawing.Color.LimeGreen;
            series3.Legend = "Legend1";
            series3.LegendText = "OK数";
            series3.Name = "Series3";
            series4.ChartArea = "ChartArea1";
            series4.Color = System.Drawing.Color.Red;
            series4.CustomProperties = "PointWidth=0.3";
            series4.IsValueShownAsLabel = true;
            series4.LabelForeColor = System.Drawing.Color.Red;
            series4.Legend = "Legend1";
            series4.LegendText = "NG数";
            series4.Name = "Series4";
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Color = System.Drawing.Color.LimeGreen;
            series5.IsValueShownAsLabel = true;
            series5.Label = "#VAL %";
            series5.LabelForeColor = System.Drawing.Color.LimeGreen;
            series5.Legend = "Legend1";
            series5.LegendText = "良率";
            series5.Name = "Series2";
            series5.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.chart_Week.Series.Add(series2);
            this.chart_Week.Series.Add(series3);
            this.chart_Week.Series.Add(series4);
            this.chart_Week.Series.Add(series5);
            this.chart_Week.Size = new System.Drawing.Size(812, 360);
            this.chart_Week.TabIndex = 17;
            this.chart_Week.Text = "chart1";
            title2.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Bold);
            title2.Name = "Title1";
            this.chart_Week.Titles.Add(title2);
            // 
            // chart_Statist
            // 
            this.chart_Statist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_Statist.BackColor = System.Drawing.Color.AntiqueWhite;
            this.chart_Statist.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_Statist.BorderlineColor = System.Drawing.Color.Maroon;
            this.chart_Statist.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart_Statist.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea3.AxisX.Interval = 1D;
            chartArea3.AxisX.MajorGrid.Enabled = false;
            chartArea3.AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea3.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea3.AxisY.LogarithmBase = 50D;
            chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea3.AxisY.MinorGrid.LineWidth = 2;
            chartArea3.AxisY2.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea3.AxisY2.Interval = 20D;
            chartArea3.AxisY2.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.AxisY2.MajorGrid.Enabled = false;
            chartArea3.AxisY2.Maximum = 100D;
            chartArea3.AxisY2.Minimum = 0D;
            chartArea3.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea3.Name = "ChartArea1";
            chartArea3.ShadowColor = System.Drawing.Color.Black;
            this.chart_Statist.ChartAreas.Add(chartArea3);
            legend3.Alignment = System.Drawing.StringAlignment.Center;
            legend3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            legend3.BorderColor = System.Drawing.Color.LightSkyBlue;
            legend3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend3.Font = new System.Drawing.Font("微软雅黑", 8F);
            legend3.IsTextAutoFit = false;
            legend3.Name = "Legend1";
            legend3.TitleBackColor = System.Drawing.Color.LightBlue;
            legend3.TitleFont = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            legend3.TitleSeparator = System.Windows.Forms.DataVisualization.Charting.LegendSeparatorStyle.ThickLine;
            this.chart_Statist.Legends.Add(legend3);
            this.chart_Statist.Location = new System.Drawing.Point(12, 12);
            this.chart_Statist.Name = "chart_Statist";
            series6.ChartArea = "ChartArea1";
            series6.Color = System.Drawing.Color.RoyalBlue;
            series6.CustomProperties = "PointWidth=0.4";
            series6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series6.IsValueShownAsLabel = true;
            series6.LabelBackColor = System.Drawing.Color.Transparent;
            series6.LabelForeColor = System.Drawing.Color.DeepSkyBlue;
            series6.Legend = "Legend1";
            series6.LegendText = "总数";
            series6.Name = "SeriesTotal";
            series7.ChartArea = "ChartArea1";
            series7.Color = System.Drawing.Color.LimeGreen;
            series7.CustomProperties = "PointWidth=0.4";
            series7.IsValueShownAsLabel = true;
            series7.LabelForeColor = System.Drawing.Color.LimeGreen;
            series7.Legend = "Legend1";
            series7.LegendText = "OK数";
            series7.Name = "SeriesOK";
            series8.ChartArea = "ChartArea1";
            series8.Color = System.Drawing.Color.Red;
            series8.CustomProperties = "PointWidth=0.4";
            series8.IsValueShownAsLabel = true;
            series8.LabelForeColor = System.Drawing.Color.Red;
            series8.Legend = "Legend1";
            series8.LegendText = "NG数";
            series8.Name = "SeriesNG";
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series9.Color = System.Drawing.Color.LimeGreen;
            series9.CustomProperties = "LabelStyle=Bottom";
            series9.IsValueShownAsLabel = true;
            series9.Label = "#VAL %";
            series9.LabelForeColor = System.Drawing.Color.LimeGreen;
            series9.Legend = "Legend1";
            series9.LegendText = "良率";
            series9.Name = "SeriesScale";
            series9.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.chart_Statist.Series.Add(series6);
            this.chart_Statist.Series.Add(series7);
            this.chart_Statist.Series.Add(series8);
            this.chart_Statist.Series.Add(series9);
            this.chart_Statist.Size = new System.Drawing.Size(812, 367);
            this.chart_Statist.TabIndex = 16;
            this.chart_Statist.Text = "chart1";
            title3.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title3.Name = "Title1";
            this.chart_Statist.Titles.Add(title3);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(843, 405);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "换班小时";
            // 
            // txt_HandHour
            // 
            this.txt_HandHour.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_HandHour.Location = new System.Drawing.Point(847, 434);
            this.txt_HandHour.MaxLength = 1;
            this.txt_HandHour.Name = "txt_HandHour";
            this.txt_HandHour.Size = new System.Drawing.Size(61, 26);
            this.txt_HandHour.TabIndex = 21;
            this.txt_HandHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_HandHour.TextChanged += new System.EventHandler(this.txt_HandHour_TextChanged);
            this.txt_HandHour.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_HandHour_KeyPress);
            // 
            // txt_HnadMin
            // 
            this.txt_HnadMin.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_HnadMin.Location = new System.Drawing.Point(847, 500);
            this.txt_HnadMin.MaxLength = 2;
            this.txt_HnadMin.Name = "txt_HnadMin";
            this.txt_HnadMin.Size = new System.Drawing.Size(61, 26);
            this.txt_HnadMin.TabIndex = 23;
            this.txt_HnadMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_HnadMin.TextChanged += new System.EventHandler(this.txt_HnadMin_TextChanged);
            this.txt_HnadMin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_HnadMin_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(843, 474);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 22;
            this.label2.Text = "换班分钟";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(912, 437);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 20);
            this.label3.TabIndex = 54;
            this.label3.Text = "(5--9)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(912, 503);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 20);
            this.label4.TabIndex = 55;
            this.label4.Text = "(0--59)";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1158, 692);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(81, 35);
            this.button3.TabIndex = 125;
            this.button3.Text = "NG";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1158, 642);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(81, 35);
            this.button4.TabIndex = 124;
            this.button4.Text = "OK";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // btn_Write
            // 
            this.btn_Write.FlatAppearance.BorderSize = 0;
            this.btn_Write.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Write.Image = ((System.Drawing.Image)(resources.GetObject("btn_Write.Image")));
            this.btn_Write.Location = new System.Drawing.Point(1044, 434);
            this.btn_Write.Name = "btn_Write";
            this.btn_Write.Size = new System.Drawing.Size(60, 60);
            this.btn_Write.TabIndex = 123;
            this.btn_Write.UseVisualStyleBackColor = true;
            this.btn_Write.Click += new System.EventHandler(this.btn_Write_Click);
            // 
            // YieldStatisticsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1251, 751);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btn_Write);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_HnadMin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_HandHour);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chart_Yield);
            this.Controls.Add(this.chart_Week);
            this.Controls.Add(this.chart_Statist);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "YieldStatisticsForm";
            this.Text = "YieldForm";
            this.Load += new System.EventHandler(this.YieldStatistics_Load);
            this.Shown += new System.EventHandler(this.YieldStatistics_Shown);
            this.SizeChanged += new System.EventHandler(this.YieldStatistics_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.chart_Yield)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Week)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Statist)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Yield;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Week;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Statist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_HandHour;
        private System.Windows.Forms.TextBox txt_HnadMin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_Write;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}