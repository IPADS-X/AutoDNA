namespace CYStandardProcedure
{
    partial class DowntimeDiscardForm
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
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DowntimeDiscardForm));
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_Select = new System.Windows.Forms.ComboBox();
            this.chart_Show = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart_Query = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmb_Query = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmb_Day = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dtp_Date = new System.Windows.Forms.DateTimePicker();
            this.btn_Query = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Show)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Query)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(24, 612);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 138;
            this.label2.Text = "抛料类型";
            // 
            // cmb_Select
            // 
            this.cmb_Select.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Select.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_Select.FormattingEnabled = true;
            this.cmb_Select.Location = new System.Drawing.Point(28, 639);
            this.cmb_Select.Name = "cmb_Select";
            this.cmb_Select.Size = new System.Drawing.Size(158, 25);
            this.cmb_Select.TabIndex = 139;
            // 
            // chart_Show
            // 
            this.chart_Show.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_Show.BackColor = System.Drawing.Color.LightPink;
            this.chart_Show.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_Show.BorderlineColor = System.Drawing.Color.Maroon;
            this.chart_Show.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart_Show.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
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
            this.chart_Show.ChartAreas.Add(chartArea1);
            this.chart_Show.Location = new System.Drawing.Point(11, 36);
            this.chart_Show.Name = "chart_Show";
            series1.ChartArea = "ChartArea1";
            series1.Color = System.Drawing.Color.Red;
            series1.CustomProperties = "PointWidth=0.4";
            series1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series1.IsValueShownAsLabel = true;
            series1.LabelBackColor = System.Drawing.Color.Transparent;
            series1.Name = "SeriesTotal";
            this.chart_Show.Series.Add(series1);
            this.chart_Show.Size = new System.Drawing.Size(509, 573);
            this.chart_Show.TabIndex = 140;
            this.chart_Show.Text = "chart1";
            title1.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title1.Name = "Title1";
            this.chart_Show.Titles.Add(title1);
            // 
            // chart_Query
            // 
            this.chart_Query.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_Query.BackColor = System.Drawing.Color.LightPink;
            this.chart_Query.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_Query.BorderlineColor = System.Drawing.Color.Maroon;
            this.chart_Query.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart_Query.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
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
            this.chart_Query.ChartAreas.Add(chartArea2);
            this.chart_Query.Location = new System.Drawing.Point(526, 36);
            this.chart_Query.Name = "chart_Query";
            series2.ChartArea = "ChartArea1";
            series2.Color = System.Drawing.Color.Red;
            series2.CustomProperties = "PointWidth=0.4";
            series2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series2.IsValueShownAsLabel = true;
            series2.LabelBackColor = System.Drawing.Color.Transparent;
            series2.Name = "SeriesTotal";
            this.chart_Query.Series.Add(series2);
            this.chart_Query.Size = new System.Drawing.Size(509, 573);
            this.chart_Query.TabIndex = 141;
            this.chart_Query.Text = "chart1";
            title2.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title2.Name = "Title1";
            this.chart_Query.Titles.Add(title2);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(24, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 142;
            this.label1.Text = "抛料统计";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(540, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 20);
            this.label4.TabIndex = 144;
            this.label4.Text = "抛料查询";
            // 
            // cmb_Query
            // 
            this.cmb_Query.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Query.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_Query.FormattingEnabled = true;
            this.cmb_Query.Location = new System.Drawing.Point(769, 641);
            this.cmb_Query.Name = "cmb_Query";
            this.cmb_Query.Size = new System.Drawing.Size(158, 25);
            this.cmb_Query.TabIndex = 146;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(771, 613);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 145;
            this.label3.Text = "查询类型";
            // 
            // cmb_Day
            // 
            this.cmb_Day.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Day.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_Day.FormattingEnabled = true;
            this.cmb_Day.Items.AddRange(new object[] {
            "Day",
            "Night"});
            this.cmb_Day.Location = new System.Drawing.Point(671, 641);
            this.cmb_Day.Name = "cmb_Day";
            this.cmb_Day.Size = new System.Drawing.Size(92, 25);
            this.cmb_Day.TabIndex = 148;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(674, 613);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 20);
            this.label5.TabIndex = 147;
            this.label5.Text = "查询班次";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(522, 613);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 20);
            this.label6.TabIndex = 149;
            this.label6.Text = "查询日期";
            // 
            // dtp_Date
            // 
            this.dtp_Date.CalendarFont = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtp_Date.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtp_Date.Location = new System.Drawing.Point(526, 640);
            this.dtp_Date.Name = "dtp_Date";
            this.dtp_Date.Size = new System.Drawing.Size(139, 26);
            this.dtp_Date.TabIndex = 150;
            // 
            // btn_Query
            // 
            this.btn_Query.FlatAppearance.BorderSize = 0;
            this.btn_Query.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Query.Image = ((System.Drawing.Image)(resources.GetObject("btn_Query.Image")));
            this.btn_Query.Location = new System.Drawing.Point(966, 608);
            this.btn_Query.Name = "btn_Query";
            this.btn_Query.Size = new System.Drawing.Size(60, 60);
            this.btn_Query.TabIndex = 151;
            this.btn_Query.UseVisualStyleBackColor = true;
            this.btn_Query.Click += new System.EventHandler(this.btn_Query_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(249, 631);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(53, 32);
            this.button1.TabIndex = 152;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(372, 631);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(53, 32);
            this.button2.TabIndex = 153;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DowntimeDiscardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1054, 680);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_Query);
            this.Controls.Add(this.dtp_Date);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmb_Day);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmb_Query);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chart_Query);
            this.Controls.Add(this.chart_Show);
            this.Controls.Add(this.cmb_Select);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DowntimeDiscardForm";
            this.Text = "DowntimeDiscardForm";
            this.Load += new System.EventHandler(this.DownTimeRecordForm_Load);
            this.SizeChanged += new System.EventHandler(this.DownTimeRecordForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.chart_Show)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Query)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_Select;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Show;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Query;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmb_Query;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmb_Day;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtp_Date;
        private System.Windows.Forms.Button btn_Query;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}