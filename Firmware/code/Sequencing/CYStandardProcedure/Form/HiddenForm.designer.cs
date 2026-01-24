namespace CYStandardProcedure
{
    partial class HiddenForm
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
            this.rbt_hide = new CYCustomControl.RoundButton();
            this.rbt_Show = new CYCustomControl.RoundButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_close = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbt_hide
            // 
            this.rbt_hide.BackColor = System.Drawing.Color.Transparent;
            this.rbt_hide.BaseColor = System.Drawing.Color.White;
            this.rbt_hide.BaseColorEnd = System.Drawing.Color.White;
            this.rbt_hide.FlatAppearance.BorderSize = 0;
            this.rbt_hide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_hide.Image = global::CYStandardProcedure.Properties.Resources.隐藏;
            this.rbt_hide.ImageHeight = 80;
            this.rbt_hide.ImageWidth = 80;
            this.rbt_hide.Location = new System.Drawing.Point(210, 75);
            this.rbt_hide.Name = "rbt_hide";
            this.rbt_hide.Radius = 24;
            this.rbt_hide.Size = new System.Drawing.Size(100, 70);
            this.rbt_hide.SpliteButtonWidth = 18;
            this.rbt_hide.TabIndex = 1;
            this.rbt_hide.UseVisualStyleBackColor = false;
            this.rbt_hide.Click += new System.EventHandler(this.rbt_hide_Click);
            // 
            // rbt_Show
            // 
            this.rbt_Show.BackColor = System.Drawing.Color.Transparent;
            this.rbt_Show.BaseColor = System.Drawing.Color.White;
            this.rbt_Show.BaseColorEnd = System.Drawing.Color.White;
            this.rbt_Show.FlatAppearance.BorderSize = 0;
            this.rbt_Show.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_Show.Image = global::CYStandardProcedure.Properties.Resources.显示;
            this.rbt_Show.ImageHeight = 80;
            this.rbt_Show.ImageWidth = 80;
            this.rbt_Show.Location = new System.Drawing.Point(50, 75);
            this.rbt_Show.Name = "rbt_Show";
            this.rbt_Show.Radius = 24;
            this.rbt_Show.Size = new System.Drawing.Size(100, 70);
            this.rbt_Show.SpliteButtonWidth = 18;
            this.rbt_Show.TabIndex = 0;
            this.rbt_Show.UseVisualStyleBackColor = false;
            this.rbt_Show.Click += new System.EventHandler(this.rbt_Show_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.PaleTurquoise;
            this.panel1.Controls.Add(this.btn_close);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(360, 27);
            this.panel1.TabIndex = 54;
            // 
            // btn_close
            // 
            this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_close.BackgroundImage = global::CYStandardProcedure.Properties.Resources._close;
            this.btn_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_close.Location = new System.Drawing.Point(334, 1);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(25, 25);
            this.btn_close.TabIndex = 6;
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "隐藏/显示 硬盘";
            // 
            // HiddenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(360, 200);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.rbt_hide);
            this.Controls.Add(this.rbt_Show);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "HiddenForm";
            this.Text = "HiddenForm";
            this.Load += new System.EventHandler(this.HiddenForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CYCustomControl.RoundButton rbt_Show;
        private CYCustomControl.RoundButton rbt_hide;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Label label1;
    }
}