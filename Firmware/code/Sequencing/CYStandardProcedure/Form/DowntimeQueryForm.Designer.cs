namespace CYStandardProcedure
{
    partial class DowntimeQueryForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DowntimeQueryForm));
            this.dtp_Date = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cyListView_Today = new CYCustomControl.CYListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chart_Downtime = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btn_Query = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Downtime)).BeginInit();
            this.SuspendLayout();
            // 
            // dtp_Date
            // 
            this.dtp_Date.CalendarFont = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtp_Date.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtp_Date.Location = new System.Drawing.Point(871, 405);
            this.dtp_Date.Name = "dtp_Date";
            this.dtp_Date.Size = new System.Drawing.Size(171, 26);
            this.dtp_Date.TabIndex = 136;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(873, 382);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 137;
            this.label1.Text = "日期选择";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(12, 344);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 20);
            this.label3.TabIndex = 143;
            this.label3.Text = "宕机时间查询结果";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(11, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 20);
            this.label4.TabIndex = 145;
            this.label4.Text = "宕机记录查询结果";
            // 
            // cyListView_Today
            // 
            this.cyListView_Today.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.cyListView_Today.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cyListView_Today.FullRowSelect = true;
            this.cyListView_Today.GridLines = true;
            this.cyListView_Today.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.cyListView_Today.HideSelection = false;
            this.cyListView_Today.Location = new System.Drawing.Point(12, 40);
            this.cyListView_Today.Name = "cyListView_Today";
            this.cyListView_Today.OwnerDraw = true;
            this.cyListView_Today.Size = new System.Drawing.Size(1030, 293);
            this.cyListView_Today.TabIndex = 144;
            this.cyListView_Today.UseCompatibleStateImageBehavior = false;
            this.cyListView_Today.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader10
            // 
            this.columnHeader10.DisplayIndex = 4;
            this.columnHeader10.Text = "";
            this.columnHeader10.Width = 0;
            // 
            // columnHeader5
            // 
            this.columnHeader5.DisplayIndex = 0;
            this.columnHeader5.Text = "错误时间";
            this.columnHeader5.Width = 270;
            // 
            // columnHeader6
            // 
            this.columnHeader6.DisplayIndex = 1;
            this.columnHeader6.Text = "错误信息";
            this.columnHeader6.Width = 500;
            // 
            // columnHeader7
            // 
            this.columnHeader7.DisplayIndex = 2;
            this.columnHeader7.Text = "错误代码";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 150;
            // 
            // columnHeader8
            // 
            this.columnHeader8.DisplayIndex = 3;
            this.columnHeader8.Text = "解决方案";
            this.columnHeader8.Width = 1200;
            // 
            // chart_Downtime
            // 
            this.chart_Downtime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_Downtime.BackColor = System.Drawing.Color.LightPink;
            this.chart_Downtime.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_Downtime.BorderlineColor = System.Drawing.Color.Maroon;
            this.chart_Downtime.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart_Downtime.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea2.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chartArea2.AxisX.Interval = 1D;
            chartArea2.AxisX.IsLabelAutoFit = false;
            chartArea2.AxisX.LabelStyle.Angle = 45;
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea2.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea2.AxisY.LogarithmBase = 50D;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.DarkGray;
            chartArea2.AxisY.MinorGrid.LineWidth = 2;
            chartArea2.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Rotated270;
            chartArea2.AxisY.Title = "Minute";
            chartArea2.AxisY.TitleAlignment = System.Drawing.StringAlignment.Far;
            chartArea2.AxisY.TitleFont = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea2.AxisY2.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea2.AxisY2.Interval = 20D;
            chartArea2.AxisY2.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea2.AxisY2.MajorGrid.Enabled = false;
            chartArea2.AxisY2.Maximum = 100D;
            chartArea2.AxisY2.Minimum = 0D;
            chartArea2.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea2.Name = "ChartArea1";
            chartArea2.ShadowColor = System.Drawing.Color.Black;
            this.chart_Downtime.ChartAreas.Add(chartArea2);
            this.chart_Downtime.Location = new System.Drawing.Point(8, 367);
            this.chart_Downtime.Name = "chart_Downtime";
            series2.ChartArea = "ChartArea1";
            series2.Color = System.Drawing.Color.Red;
            series2.CustomProperties = "PointWidth=0.4";
            series2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series2.IsValueShownAsLabel = true;
            series2.LabelBackColor = System.Drawing.Color.Transparent;
            series2.Name = "SeriesTotal";
            this.chart_Downtime.Series.Add(series2);
            this.chart_Downtime.Size = new System.Drawing.Size(857, 319);
            this.chart_Downtime.TabIndex = 146;
            this.chart_Downtime.Text = "chart1";
            title2.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title2.Name = "Title1";
            this.chart_Downtime.Titles.Add(title2);
            // 
            // btn_Query
            // 
            this.btn_Query.FlatAppearance.BorderSize = 0;
            this.btn_Query.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Query.Image = ((System.Drawing.Image)(resources.GetObject("btn_Query.Image")));
            this.btn_Query.Location = new System.Drawing.Point(982, 579);
            this.btn_Query.Name = "btn_Query";
            this.btn_Query.Size = new System.Drawing.Size(60, 60);
            this.btn_Query.TabIndex = 142;
            this.btn_Query.UseVisualStyleBackColor = true;
            this.btn_Query.Click += new System.EventHandler(this.btn_Query_Click);
            // 
            // DowntimeQueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1054, 680);
            this.Controls.Add(this.chart_Downtime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cyListView_Today);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_Query);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtp_Date);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DowntimeQueryForm";
            this.Text = "DowntimeQueryForm";
            this.Load += new System.EventHandler(this.DowntimeClassifyForm_Load);
            this.SizeChanged += new System.EventHandler(this.DowntimeClassifyForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.chart_Downtime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DateTimePicker dtp_Date;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Query;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private CYCustomControl.CYListView cyListView_Today;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Downtime;
    }
}