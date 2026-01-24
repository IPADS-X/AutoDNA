namespace CYStandardProcedure
{
    partial class ImageForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_Open = new CYCustomControl.RoundButton();
            this.btn_Next = new CYCustomControl.RoundButton();
            this.btn_Prev = new CYCustomControl.RoundButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(12, 23);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1212, 632);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // btn_Open
            // 
            this.btn_Open.BackColor = System.Drawing.Color.Transparent;
            this.btn_Open.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Open.BackgroundImage")));
            this.btn_Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Open.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Open.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Open.FlatAppearance.BorderSize = 0;
            this.btn_Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Open.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Open.ImageHeight = 80;
            this.btn_Open.ImageWidth = 80;
            this.btn_Open.Location = new System.Drawing.Point(364, 669);
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.PressOffset = false;
            this.btn_Open.Radius = 24;
            this.btn_Open.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Open.Size = new System.Drawing.Size(79, 72);
            this.btn_Open.SpliteButtonWidth = 18;
            this.btn_Open.TabIndex = 20;
            this.btn_Open.UseVisualStyleBackColor = false;
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // btn_Next
            // 
            this.btn_Next.BackColor = System.Drawing.Color.Transparent;
            this.btn_Next.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Next.BackgroundImage")));
            this.btn_Next.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Next.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Next.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Next.FlatAppearance.BorderSize = 0;
            this.btn_Next.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Next.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Next.ImageHeight = 80;
            this.btn_Next.ImageWidth = 80;
            this.btn_Next.Location = new System.Drawing.Point(694, 669);
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.PressOffset = false;
            this.btn_Next.Radius = 24;
            this.btn_Next.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Next.Size = new System.Drawing.Size(79, 72);
            this.btn_Next.SpliteButtonWidth = 18;
            this.btn_Next.TabIndex = 19;
            this.btn_Next.UseVisualStyleBackColor = false;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // btn_Prev
            // 
            this.btn_Prev.BackColor = System.Drawing.Color.Transparent;
            this.btn_Prev.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Prev.BackgroundImage")));
            this.btn_Prev.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Prev.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Prev.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Prev.FlatAppearance.BorderSize = 0;
            this.btn_Prev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Prev.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Prev.ImageHeight = 80;
            this.btn_Prev.ImageWidth = 80;
            this.btn_Prev.Location = new System.Drawing.Point(529, 669);
            this.btn_Prev.Name = "btn_Prev";
            this.btn_Prev.PressOffset = false;
            this.btn_Prev.Radius = 24;
            this.btn_Prev.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Prev.Size = new System.Drawing.Size(79, 72);
            this.btn_Prev.SpliteButtonWidth = 18;
            this.btn_Prev.TabIndex = 18;
            this.btn_Prev.UseVisualStyleBackColor = false;
            this.btn_Prev.Click += new System.EventHandler(this.btn_Prev_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "文件.png");
            this.imageList1.Images.SetKeyName(1, "文件夹.png");
            // 
            // ImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1251, 751);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_Open);
            this.Controls.Add(this.btn_Next);
            this.Controls.Add(this.btn_Prev);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ImageForm";
            this.Text = "ImageForm";
            this.Load += new System.EventHandler(this.ImageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private CYCustomControl.RoundButton btn_Open;
        private CYCustomControl.RoundButton btn_Next;
        private CYCustomControl.RoundButton btn_Prev;
        private System.Windows.Forms.ImageList imageList1;
    }
}