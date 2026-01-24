namespace CYStandardProcedure
{
    partial class CapacityForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btn_yieldStatics = new System.Windows.Forms.ToolStripButton();
            this.btn_yieldQuery = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(1, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1250, 682);
            this.panel1.TabIndex = 20;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(55, 55);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_yieldStatics,
            this.btn_yieldQuery});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1251, 62);
            this.toolStrip1.TabIndex = 58;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btn_yieldStatics
            // 
            this.btn_yieldStatics.AutoSize = false;
            this.btn_yieldStatics.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_yieldStatics.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_yieldStatics.Image = global::CYStandardProcedure.Properties.Resources.产能统计未选中;
            this.btn_yieldStatics.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_yieldStatics.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_yieldStatics.Name = "btn_yieldStatics";
            this.btn_yieldStatics.Size = new System.Drawing.Size(60, 60);
            this.btn_yieldStatics.Click += new System.EventHandler(this.btn_dtRecord_Click);
            // 
            // btn_yieldQuery
            // 
            this.btn_yieldQuery.AutoSize = false;
            this.btn_yieldQuery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_yieldQuery.Image = global::CYStandardProcedure.Properties.Resources.产能查询未选中;
            this.btn_yieldQuery.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_yieldQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_yieldQuery.Name = "btn_yieldQuery";
            this.btn_yieldQuery.Size = new System.Drawing.Size(60, 60);
            this.btn_yieldQuery.Click += new System.EventHandler(this.btn_dtTimeStatis_Click);
            // 
            // CapacityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1251, 751);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CapacityForm";
            this.Text = "9";
            this.Load += new System.EventHandler(this.CapacityForm_Load);
            this.SizeChanged += new System.EventHandler(this.ErrorForm_SizeChanged);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btn_yieldStatics;
        private System.Windows.Forms.ToolStripButton btn_yieldQuery;
    }
}