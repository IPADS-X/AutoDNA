namespace CYStandardProcedure
{
    partial class DowntimeStatisticsForm
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
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chart_CurDowntime = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chart_Week = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart_CurDowntime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Week)).BeginInit();
            this.SuspendLayout();
            // 
            // chart_CurDowntime
            // 
            this.chart_CurDowntime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_CurDowntime.BackColor = System.Drawing.Color.LightPink;
            this.chart_CurDowntime.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_CurDowntime.BorderlineColor = System.Drawing.Color.Maroon;
            this.chart_CurDowntime.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart_CurDowntime.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea1.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Angle = 45;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea1.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.AxisY.LogarithmBase = 50D;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisY.MinorGrid.LineWidth = 2;
            chartArea1.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Rotated270;
            chartArea1.AxisY.Title = "Minute";
            chartArea1.AxisY.TitleAlignment = System.Drawing.StringAlignment.Far;
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY2.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea1.AxisY2.Interval = 20D;
            chartArea1.AxisY2.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisY2.MajorGrid.Enabled = false;
            chartArea1.AxisY2.Maximum = 100D;
            chartArea1.AxisY2.Minimum = 0D;
            chartArea1.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea1.Name = "ChartArea1";
            chartArea1.ShadowColor = System.Drawing.Color.Black;
            this.chart_CurDowntime.ChartAreas.Add(chartArea1);
            this.chart_CurDowntime.Location = new System.Drawing.Point(1, 33);
            this.chart_CurDowntime.Name = "chart_CurDowntime";
            series1.ChartArea = "ChartArea1";
            series1.Color = System.Drawing.Color.Red;
            series1.CustomProperties = "PointWidth=0.4";
            series1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series1.IsValueShownAsLabel = true;
            series1.LabelBackColor = System.Drawing.Color.Transparent;
            series1.Name = "SeriesTotal";
            this.chart_CurDowntime.Series.Add(series1);
            this.chart_CurDowntime.Size = new System.Drawing.Size(520, 635);
            this.chart_CurDowntime.TabIndex = 131;
            this.chart_CurDowntime.Text = "chart1";
            title1.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title1.Name = "Title1";
            this.chart_CurDowntime.Titles.Add(title1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 20);
            this.label1.TabIndex = 133;
            this.label1.Text = "当天宕机时间统计";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(532, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 20);
            this.label2.TabIndex = 134;
            this.label2.Text = "一周宕机时间统计";
            // 
            // chart_Week
            // 
            this.chart_Week.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_Week.BackColor = System.Drawing.Color.LightPink;
            this.chart_Week.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_Week.BorderlineColor = System.Drawing.Color.Maroon;
            this.chart_Week.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart_Week.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea2.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea2.AxisX.Interval = 1D;
            chartArea2.AxisX.IsLabelAutoFit = false;
            chartArea2.AxisX.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea2.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea2.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Rotated270;
            chartArea2.AxisY.Title = "Mintue";
            chartArea2.AxisY.TitleAlignment = System.Drawing.StringAlignment.Far;
            chartArea2.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea2.AxisY2.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea2.AxisY2.Interval = 20D;
            chartArea2.AxisY2.MajorGrid.Enabled = false;
            chartArea2.AxisY2.Maximum = 100D;
            chartArea2.AxisY2.Minimum = 0D;
            chartArea2.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea2.Name = "ChartArea1";
            this.chart_Week.ChartAreas.Add(chartArea2);
            legend1.Alignment = System.Drawing.StringAlignment.Far;
            legend1.BackColor = System.Drawing.Color.Gainsboro;
            legend1.BorderColor = System.Drawing.Color.Silver;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Name = "Legend1";
            this.chart_Week.Legends.Add(legend1);
            this.chart_Week.Location = new System.Drawing.Point(519, 33);
            this.chart_Week.Name = "chart_Week";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
            series2.Color = System.Drawing.Color.Red;
            series2.CustomProperties = "PointWidth=0.3";
            series2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series2.IsValueShownAsLabel = true;
            series2.LabelBackColor = System.Drawing.Color.Transparent;
            series2.Legend = "Legend1";
            series2.LegendText = "Day";
            series2.MarkerBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            series2.MarkerColor = System.Drawing.Color.LightPink;
            series2.Name = "Series1";
            series2.YValuesPerPoint = 2;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
            series3.Color = System.Drawing.Color.Gray;
            series3.CustomProperties = "PointWidth=0.3";
            series3.IsValueShownAsLabel = true;
            series3.Legend = "Legend1";
            series3.LegendText = "Night";
            series3.MarkerColor = System.Drawing.Color.Blue;
            series3.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series3.Name = "Series2";
            this.chart_Week.Series.Add(series2);
            this.chart_Week.Series.Add(series3);
            this.chart_Week.Size = new System.Drawing.Size(532, 635);
            this.chart_Week.TabIndex = 136;
            this.chart_Week.Text = "chart1";
            title2.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Bold);
            title2.Name = "Title1";
            this.chart_Week.Titles.Add(title2);
            // 
            // DowntimeStatisticsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1054, 680);
            this.Controls.Add(this.chart_Week);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chart_CurDowntime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DowntimeStatisticsForm";
            this.Text = "DowntimeStatisticsForm";
            this.Load += new System.EventHandler(this.DowntimeStatisticsForm_Load);
            this.SizeChanged += new System.EventHandler(this.DowntimeStatisticsForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.chart_CurDowntime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Week)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_CurDowntime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Week;
    }
}