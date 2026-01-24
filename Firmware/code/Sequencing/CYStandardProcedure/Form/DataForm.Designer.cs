namespace CYStandardProcedure
{
    partial class DataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataForm));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.list_File = new System.Windows.Forms.ListBox();
            this.list_Info = new System.Windows.Forms.ListBox();
            this.btn_Next = new CYCustomControl.RoundButton();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_Prev = new CYCustomControl.RoundButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.list_File);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.list_Info);
            this.splitContainer2.Size = new System.Drawing.Size(924, 593);
            this.splitContainer2.SplitterDistance = 228;
            this.splitContainer2.TabIndex = 0;
            // 
            // list_File
            // 
            this.list_File.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list_File.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.list_File.FormattingEnabled = true;
            this.list_File.ItemHeight = 25;
            this.list_File.Location = new System.Drawing.Point(0, 0);
            this.list_File.Name = "list_File";
            this.list_File.Size = new System.Drawing.Size(228, 593);
            this.list_File.TabIndex = 0;
            this.list_File.SelectedIndexChanged += new System.EventHandler(this.list_File_SelectedIndexChanged);
            // 
            // list_Info
            // 
            this.list_Info.BackColor = System.Drawing.Color.LightGray;
            this.list_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list_Info.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.list_Info.FormattingEnabled = true;
            this.list_Info.HorizontalScrollbar = true;
            this.list_Info.ItemHeight = 25;
            this.list_Info.Location = new System.Drawing.Point(0, 0);
            this.list_Info.Name = "list_Info";
            this.list_Info.Size = new System.Drawing.Size(692, 593);
            this.list_Info.TabIndex = 0;
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
            this.btn_Next.Location = new System.Drawing.Point(597, 665);
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.Radius = 24;
            this.btn_Next.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Next.Size = new System.Drawing.Size(83, 61);
            this.btn_Next.SpliteButtonWidth = 18;
            this.btn_Next.TabIndex = 18;
            this.btn_Next.UseVisualStyleBackColor = false;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeView1.ImageIndex = 1;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(299, 593);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "文件.png");
            this.imageList1.Images.SetKeyName(1, "文件夹.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(12, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1227, 593);
            this.splitContainer1.SplitterDistance = 299;
            this.splitContainer1.TabIndex = 19;
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
            this.btn_Prev.Location = new System.Drawing.Point(440, 665);
            this.btn_Prev.Name = "btn_Prev";
            this.btn_Prev.PressOffset = false;
            this.btn_Prev.Radius = 24;
            this.btn_Prev.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Prev.Size = new System.Drawing.Size(83, 61);
            this.btn_Prev.SpliteButtonWidth = 18;
            this.btn_Prev.TabIndex = 17;
            this.btn_Prev.UseVisualStyleBackColor = false;
            this.btn_Prev.Click += new System.EventHandler(this.btn_Prev_Click);
            // 
            // DataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1251, 751);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btn_Next);
            this.Controls.Add(this.btn_Prev);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DataForm";
            this.Text = "DataForm";
            this.Load += new System.EventHandler(this.DataForm_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox list_File;
        private System.Windows.Forms.ListBox list_Info;
        private CYCustomControl.RoundButton btn_Next;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private CYCustomControl.RoundButton btn_Prev;
    }
}