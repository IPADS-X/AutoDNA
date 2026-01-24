namespace CYStandardProcedure
{
    partial class ServerForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerForm));
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbt_Send = new CYCustomControl.RoundButton();
            this.rbt_DisConnectServer = new CYCustomControl.RoundButton();
            this.rbt_StartServer = new CYCustomControl.RoundButton();
            this.rbt_ClearReceive = new CYCustomControl.RoundButton();
            this.rbt_Save = new CYCustomControl.RoundButton();
            this.rbt_ClearSend = new CYCustomControl.RoundButton();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBox2
            // 
            this.richTextBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox2.Location = new System.Drawing.Point(379, 540);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(523, 137);
            this.richTextBox2.TabIndex = 3;
            this.richTextBox2.Text = "";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(379, 192);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(523, 307);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // treeView1
            // 
            this.treeView1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(8, 192);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(352, 485);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Server.png");
            this.imageList1.Images.SetKeyName(1, "Client.png");
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(5, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(897, 157);
            this.dataGridView1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(382, 167);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.TabIndex = 192;
            this.label2.Text = "接收字符";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(382, 513);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 193;
            this.label1.Text = "发送字符";
            // 
            // rbt_Send
            // 
            this.rbt_Send.BackColor = System.Drawing.Color.Transparent;
            this.rbt_Send.BackgroundImage = global::CYStandardProcedure.Properties.Resources.发送数据;
            this.rbt_Send.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_Send.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_Send.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_Send.ContextOffset = 0;
            this.rbt_Send.FlatAppearance.BorderSize = 0;
            this.rbt_Send.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_Send.ImageHeight = 80;
            this.rbt_Send.ImageWidth = 80;
            this.rbt_Send.Location = new System.Drawing.Point(923, 618);
            this.rbt_Send.Name = "rbt_Send";
            this.rbt_Send.Radius = 24;
            this.rbt_Send.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_Send.Size = new System.Drawing.Size(52, 53);
            this.rbt_Send.SpliteButtonWidth = 18;
            this.rbt_Send.TabIndex = 197;
            this.rbt_Send.UseVisualStyleBackColor = false;
            this.rbt_Send.Click += new System.EventHandler(this.rbt_Send_Click);
            // 
            // rbt_DisConnectServer
            // 
            this.rbt_DisConnectServer.BackColor = System.Drawing.Color.Transparent;
            this.rbt_DisConnectServer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_DisConnectServer.BackgroundImage")));
            this.rbt_DisConnectServer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_DisConnectServer.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_DisConnectServer.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_DisConnectServer.ContextOffset = 0;
            this.rbt_DisConnectServer.FlatAppearance.BorderSize = 0;
            this.rbt_DisConnectServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_DisConnectServer.ImageHeight = 80;
            this.rbt_DisConnectServer.ImageWidth = 80;
            this.rbt_DisConnectServer.Location = new System.Drawing.Point(990, 540);
            this.rbt_DisConnectServer.Name = "rbt_DisConnectServer";
            this.rbt_DisConnectServer.Radius = 24;
            this.rbt_DisConnectServer.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_DisConnectServer.Size = new System.Drawing.Size(52, 53);
            this.rbt_DisConnectServer.SpliteButtonWidth = 18;
            this.rbt_DisConnectServer.TabIndex = 196;
            this.rbt_DisConnectServer.UseVisualStyleBackColor = false;
            this.rbt_DisConnectServer.Click += new System.EventHandler(this.rbt_DisConnectServer_Click);
            // 
            // rbt_StartServer
            // 
            this.rbt_StartServer.BackColor = System.Drawing.Color.Transparent;
            this.rbt_StartServer.BackgroundImage = global::CYStandardProcedure.Properties.Resources.服务器;
            this.rbt_StartServer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_StartServer.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_StartServer.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_StartServer.ContextOffset = 0;
            this.rbt_StartServer.FlatAppearance.BorderSize = 0;
            this.rbt_StartServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_StartServer.ImageHeight = 80;
            this.rbt_StartServer.ImageWidth = 80;
            this.rbt_StartServer.Location = new System.Drawing.Point(923, 540);
            this.rbt_StartServer.Name = "rbt_StartServer";
            this.rbt_StartServer.Radius = 24;
            this.rbt_StartServer.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_StartServer.Size = new System.Drawing.Size(52, 53);
            this.rbt_StartServer.SpliteButtonWidth = 18;
            this.rbt_StartServer.TabIndex = 195;
            this.rbt_StartServer.UseVisualStyleBackColor = false;
            this.rbt_StartServer.Click += new System.EventHandler(this.rbt_StartServer_Click);
            // 
            // rbt_ClearReceive
            // 
            this.rbt_ClearReceive.BackColor = System.Drawing.Color.Transparent;
            this.rbt_ClearReceive.BackgroundImage = global::CYStandardProcedure.Properties.Resources.清除;
            this.rbt_ClearReceive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_ClearReceive.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_ClearReceive.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_ClearReceive.ContextOffset = 0;
            this.rbt_ClearReceive.FlatAppearance.BorderSize = 0;
            this.rbt_ClearReceive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_ClearReceive.ImageHeight = 80;
            this.rbt_ClearReceive.ImageWidth = 80;
            this.rbt_ClearReceive.Location = new System.Drawing.Point(919, 203);
            this.rbt_ClearReceive.Name = "rbt_ClearReceive";
            this.rbt_ClearReceive.Radius = 24;
            this.rbt_ClearReceive.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_ClearReceive.Size = new System.Drawing.Size(56, 62);
            this.rbt_ClearReceive.SpliteButtonWidth = 18;
            this.rbt_ClearReceive.TabIndex = 194;
            this.rbt_ClearReceive.UseVisualStyleBackColor = false;
            this.rbt_ClearReceive.Click += new System.EventHandler(this.rbt_ClearReceive_Click);
            // 
            // rbt_Save
            // 
            this.rbt_Save.BackColor = System.Drawing.Color.Transparent;
            this.rbt_Save.BackgroundImage = global::CYStandardProcedure.Properties.Resources.保存;
            this.rbt_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_Save.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_Save.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_Save.ContextOffset = 0;
            this.rbt_Save.FlatAppearance.BorderSize = 0;
            this.rbt_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_Save.ImageHeight = 80;
            this.rbt_Save.ImageWidth = 80;
            this.rbt_Save.Location = new System.Drawing.Point(923, 43);
            this.rbt_Save.Name = "rbt_Save";
            this.rbt_Save.Radius = 24;
            this.rbt_Save.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_Save.Size = new System.Drawing.Size(52, 53);
            this.rbt_Save.SpliteButtonWidth = 18;
            this.rbt_Save.TabIndex = 191;
            this.rbt_Save.UseVisualStyleBackColor = false;
            this.rbt_Save.Click += new System.EventHandler(this.rbt_Save_Click);
            // 
            // rbt_ClearSend
            // 
            this.rbt_ClearSend.BackColor = System.Drawing.Color.Transparent;
            this.rbt_ClearSend.BackgroundImage = global::CYStandardProcedure.Properties.Resources.清除;
            this.rbt_ClearSend.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_ClearSend.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_ClearSend.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_ClearSend.ContextOffset = 0;
            this.rbt_ClearSend.FlatAppearance.BorderSize = 0;
            this.rbt_ClearSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_ClearSend.ImageHeight = 80;
            this.rbt_ClearSend.ImageWidth = 80;
            this.rbt_ClearSend.Location = new System.Drawing.Point(986, 609);
            this.rbt_ClearSend.Name = "rbt_ClearSend";
            this.rbt_ClearSend.Radius = 24;
            this.rbt_ClearSend.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_ClearSend.Size = new System.Drawing.Size(56, 62);
            this.rbt_ClearSend.SpliteButtonWidth = 18;
            this.rbt_ClearSend.TabIndex = 198;
            this.rbt_ClearSend.UseVisualStyleBackColor = false;
            this.rbt_ClearSend.Click += new System.EventHandler(this.rbt_ClearSend_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(11, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 20);
            this.label3.TabIndex = 199;
            this.label3.Text = "服务器列表";
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1054, 689);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rbt_ClearSend);
            this.Controls.Add(this.rbt_Send);
            this.Controls.Add(this.rbt_DisConnectServer);
            this.Controls.Add(this.rbt_StartServer);
            this.Controls.Add(this.rbt_ClearReceive);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rbt_Save);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.treeView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ServerForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ImageList imageList1;
        private CYCustomControl.RoundButton rbt_Save;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private CYCustomControl.RoundButton rbt_ClearReceive;
        private CYCustomControl.RoundButton rbt_StartServer;
        private CYCustomControl.RoundButton rbt_DisConnectServer;
        private CYCustomControl.RoundButton rbt_Send;
        private CYCustomControl.RoundButton rbt_ClearSend;
        private System.Windows.Forms.Label label3;
    }
}

