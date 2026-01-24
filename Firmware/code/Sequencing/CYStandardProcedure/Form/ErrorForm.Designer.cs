namespace CYStandardProcedure
{
    partial class ErrorForm
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
            this.btn_dtRecord = new System.Windows.Forms.ToolStripButton();
            this.btn_dtTimeStatis = new System.Windows.Forms.ToolStripButton();
            this.btn_dtTimeClassify = new System.Windows.Forms.ToolStripButton();
            this.btn_dtDiscard = new System.Windows.Forms.ToolStripButton();
            this.btn_ClearDownTime = new System.Windows.Forms.Button();
            this.rbt_initial = new CYCustomControl.RoundButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(1, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1250, 618);
            this.panel1.TabIndex = 20;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(55, 55);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_dtRecord,
            this.btn_dtTimeStatis,
            this.btn_dtTimeClassify,
            this.btn_dtDiscard});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1251, 62);
            this.toolStrip1.TabIndex = 58;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btn_dtRecord
            // 
            this.btn_dtRecord.AutoSize = false;
            this.btn_dtRecord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_dtRecord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_dtRecord.Image = global::CYStandardProcedure.Properties.Resources.宕机记录未选中;
            this.btn_dtRecord.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_dtRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_dtRecord.Name = "btn_dtRecord";
            this.btn_dtRecord.Size = new System.Drawing.Size(60, 60);
            this.btn_dtRecord.Click += new System.EventHandler(this.btn_dtRecord_Click);
            // 
            // btn_dtTimeStatis
            // 
            this.btn_dtTimeStatis.AutoSize = false;
            this.btn_dtTimeStatis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_dtTimeStatis.Image = global::CYStandardProcedure.Properties.Resources.宕机时间统计未选中;
            this.btn_dtTimeStatis.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_dtTimeStatis.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_dtTimeStatis.Name = "btn_dtTimeStatis";
            this.btn_dtTimeStatis.Size = new System.Drawing.Size(60, 60);
            this.btn_dtTimeStatis.Visible = false;
            this.btn_dtTimeStatis.Click += new System.EventHandler(this.btn_dtTimeStatis_Click);
            // 
            // btn_dtTimeClassify
            // 
            this.btn_dtTimeClassify.AutoSize = false;
            this.btn_dtTimeClassify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_dtTimeClassify.Image = global::CYStandardProcedure.Properties.Resources.异常统计未选中;
            this.btn_dtTimeClassify.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_dtTimeClassify.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_dtTimeClassify.Name = "btn_dtTimeClassify";
            this.btn_dtTimeClassify.Size = new System.Drawing.Size(60, 60);
            this.btn_dtTimeClassify.Visible = false;
            this.btn_dtTimeClassify.Click += new System.EventHandler(this.btn_dtTimeClassify_Click);
            // 
            // btn_dtDiscard
            // 
            this.btn_dtDiscard.AutoSize = false;
            this.btn_dtDiscard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_dtDiscard.Image = global::CYStandardProcedure.Properties.Resources.抛料未选中;
            this.btn_dtDiscard.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btn_dtDiscard.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_dtDiscard.Name = "btn_dtDiscard";
            this.btn_dtDiscard.Size = new System.Drawing.Size(60, 60);
            this.btn_dtDiscard.Visible = false;
            this.btn_dtDiscard.Click += new System.EventHandler(this.btn_dtDiscard_Click);
            // 
            // btn_ClearDownTime
            // 
            this.btn_ClearDownTime.FlatAppearance.BorderSize = 0;
            this.btn_ClearDownTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ClearDownTime.Image = global::CYStandardProcedure.Properties.Resources.解除报警;
            this.btn_ClearDownTime.Location = new System.Drawing.Point(546, 689);
            this.btn_ClearDownTime.Name = "btn_ClearDownTime";
            this.btn_ClearDownTime.Size = new System.Drawing.Size(60, 60);
            this.btn_ClearDownTime.TabIndex = 57;
            this.btn_ClearDownTime.UseVisualStyleBackColor = true;
            this.btn_ClearDownTime.Click += new System.EventHandler(this.btn_ClearDownTime_Click);
            // 
            // rbt_initial
            // 
            this.rbt_initial.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rbt_initial.BackColor = System.Drawing.Color.Transparent;
            this.rbt_initial.BaseColor = System.Drawing.Color.DarkOrange;
            this.rbt_initial.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.rbt_initial.FlatAppearance.BorderSize = 0;
            this.rbt_initial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_initial.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbt_initial.ImageHeight = 80;
            this.rbt_initial.ImageWidth = 80;
            this.rbt_initial.Location = new System.Drawing.Point(438, 12);
            this.rbt_initial.Name = "rbt_initial";
            this.rbt_initial.Radius = 24;
            this.rbt_initial.Size = new System.Drawing.Size(123, 47);
            this.rbt_initial.SpliteButtonWidth = 18;
            this.rbt_initial.TabIndex = 213;
            this.rbt_initial.Text = "初始化记忆";
            this.rbt_initial.UseVisualStyleBackColor = false;
            this.rbt_initial.Click += new System.EventHandler(this.rbt_initial_Click);
            // 
            // ErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1251, 751);
            this.Controls.Add(this.rbt_initial);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btn_ClearDownTime);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ErrorForm";
            this.Text = "9";
            this.Load += new System.EventHandler(this.ErrorForm_Load);
            this.SizeChanged += new System.EventHandler(this.ErrorForm_SizeChanged);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_ClearDownTime;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btn_dtRecord;
        private System.Windows.Forms.ToolStripButton btn_dtTimeStatis;
        private System.Windows.Forms.ToolStripButton btn_dtTimeClassify;
        private System.Windows.Forms.ToolStripButton btn_dtDiscard;
        private CYCustomControl.RoundButton rbt_initial;
    }
}