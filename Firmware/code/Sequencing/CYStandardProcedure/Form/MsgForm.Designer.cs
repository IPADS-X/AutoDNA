namespace CYStandardProcedure
{
    partial class MsgForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MsgForm));
            this.txt_CardID = new System.Windows.Forms.TextBox();
            this.lbl_Title = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_NewSN = new System.Windows.Forms.TextBox();
            this.btn_Yes = new System.Windows.Forms.Button();
            this.btn_No = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txt_CardID
            // 
            this.txt_CardID.Location = new System.Drawing.Point(140, 119);
            this.txt_CardID.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txt_CardID.Name = "txt_CardID";
            this.txt_CardID.Size = new System.Drawing.Size(417, 29);
            this.txt_CardID.TabIndex = 0;
            // 
            // lbl_Title
            // 
            this.lbl_Title.AutoSize = true;
            this.lbl_Title.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Title.Location = new System.Drawing.Point(157, 41);
            this.lbl_Title.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Size = new System.Drawing.Size(350, 28);
            this.lbl_Title.TabIndex = 1;
            this.lbl_Title.Text = "Planned Downtime Information";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 122);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Card ID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 194);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "New_SN:";
            // 
            // txt_NewSN
            // 
            this.txt_NewSN.Location = new System.Drawing.Point(140, 191);
            this.txt_NewSN.Margin = new System.Windows.Forms.Padding(5);
            this.txt_NewSN.Name = "txt_NewSN";
            this.txt_NewSN.Size = new System.Drawing.Size(417, 29);
            this.txt_NewSN.TabIndex = 3;
            // 
            // btn_Yes
            // 
            this.btn_Yes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btn_Yes.Location = new System.Drawing.Point(101, 277);
            this.btn_Yes.Name = "btn_Yes";
            this.btn_Yes.Size = new System.Drawing.Size(131, 35);
            this.btn_Yes.TabIndex = 5;
            this.btn_Yes.Text = "Yes";
            this.btn_Yes.UseVisualStyleBackColor = false;
            this.btn_Yes.Click += new System.EventHandler(this.btn_Yes_Click);
            // 
            // btn_No
            // 
            this.btn_No.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btn_No.Location = new System.Drawing.Point(441, 277);
            this.btn_No.Name = "btn_No";
            this.btn_No.Size = new System.Drawing.Size(131, 35);
            this.btn_No.TabIndex = 6;
            this.btn_No.Text = "No";
            this.btn_No.UseVisualStyleBackColor = false;
            this.btn_No.Click += new System.EventHandler(this.btn_No_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(96, 30);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 50);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // MsgForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(646, 367);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_No);
            this.Controls.Add(this.btn_Yes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_NewSN);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl_Title);
            this.Controls.Add(this.txt_CardID);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MsgForm";
            this.Text = "MsgForm";
            this.Load += new System.EventHandler(this.MsgForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbl_Title;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Yes;
        private System.Windows.Forms.Button btn_No;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txt_CardID;
        private System.Windows.Forms.TextBox txt_NewSN;
    }
}