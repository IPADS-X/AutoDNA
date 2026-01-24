namespace CYStandardProcedure
{
    partial class DowntimeRecordForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cyListView_30Day = new CYCustomControl.CYListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cyListView_Today = new CYCustomControl.CYListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(17, 324);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 20);
            this.label2.TabIndex = 27;
            this.label2.Text = "近30天宕机记录";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(16, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 20);
            this.label1.TabIndex = 26;
            this.label1.Text = "当天宕机记录";
            // 
            // cyListView_30Day
            // 
            this.cyListView_30Day.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.cyListView_30Day.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cyListView_30Day.FullRowSelect = true;
            this.cyListView_30Day.GridLines = true;
            this.cyListView_30Day.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.cyListView_30Day.HideSelection = false;
            this.cyListView_30Day.Location = new System.Drawing.Point(9, 348);
            this.cyListView_30Day.Name = "cyListView_30Day";
            this.cyListView_30Day.OwnerDraw = true;
            this.cyListView_30Day.Size = new System.Drawing.Size(1034, 287);
            this.cyListView_30Day.TabIndex = 25;
            this.cyListView_30Day.UseCompatibleStateImageBehavior = false;
            this.cyListView_30Day.View = System.Windows.Forms.View.Details;
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
            // cyListView_Today
            // 
            this.cyListView_Today.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.cyListView_Today.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cyListView_Today.FullRowSelect = true;
            this.cyListView_Today.GridLines = true;
            this.cyListView_Today.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.cyListView_Today.HideSelection = false;
            this.cyListView_Today.Location = new System.Drawing.Point(8, 30);
            this.cyListView_Today.Name = "cyListView_Today";
            this.cyListView_Today.OwnerDraw = true;
            this.cyListView_Today.Size = new System.Drawing.Size(1034, 289);
            this.cyListView_Today.TabIndex = 24;
            this.cyListView_Today.UseCompatibleStateImageBehavior = false;
            this.cyListView_Today.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.DisplayIndex = 4;
            this.columnHeader9.Text = "";
            this.columnHeader9.Width = 0;
            // 
            // columnHeader1
            // 
            this.columnHeader1.DisplayIndex = 0;
            this.columnHeader1.Text = "错误时间";
            this.columnHeader1.Width = 270;
            // 
            // columnHeader2
            // 
            this.columnHeader2.DisplayIndex = 1;
            this.columnHeader2.Text = "错误信息";
            this.columnHeader2.Width = 500;
            // 
            // columnHeader3
            // 
            this.columnHeader3.DisplayIndex = 2;
            this.columnHeader3.Text = "错误代码";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 150;
            // 
            // columnHeader4
            // 
            this.columnHeader4.DisplayIndex = 3;
            this.columnHeader4.Text = "解决方案";
            this.columnHeader4.Width = 1200;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 641);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 38);
            this.button2.TabIndex = 29;
            this.button2.Text = "轴宕机";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(109, 641);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 38);
            this.button1.TabIndex = 30;
            this.button1.Text = "真空吸宕机";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(207, 641);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(77, 38);
            this.button3.TabIndex = 31;
            this.button3.Text = "磁环宕机";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(308, 641);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(77, 38);
            this.button4.TabIndex = 32;
            this.button4.Text = "光电超时";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // DowntimeRecordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1054, 680);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cyListView_30Day);
            this.Controls.Add(this.cyListView_Today);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DowntimeRecordForm";
            this.Text = "DowntimeRecordForm";
            this.Load += new System.EventHandler(this.DownTimeRecordForm_Load);
            this.SizeChanged += new System.EventHandler(this.DownTimeRecordForm_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private CYCustomControl.CYListView cyListView_30Day;
        private CYCustomControl.CYListView cyListView_Today;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}