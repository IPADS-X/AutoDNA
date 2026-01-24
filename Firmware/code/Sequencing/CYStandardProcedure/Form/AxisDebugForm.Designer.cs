namespace CYStandardProcedure
{
    partial class AxisDebugForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisDebugForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btn_Home = new CYCustomControl.RoundButton();
            this.label11 = new System.Windows.Forms.Label();
            this.rdb_Lnching = new System.Windows.Forms.RadioButton();
            this.rdb_PointMove = new System.Windows.Forms.RadioButton();
            this.label16 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_Speed = new HZH_Controls.Controls.UCTrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.ntx_Distance = new HZH_Controls.Controls.UCNumTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.pic_Alm = new System.Windows.Forms.PictureBox();
            this.btn_Save = new CYCustomControl.RoundButton();
            this.btn_Move = new CYCustomControl.RoundButton();
            this.btn_GetPos = new CYCustomControl.RoundButton();
            this.rbt_InverseRotate = new CYCustomControl.RoundButton();
            this.rbt_DwMove = new CYCustomControl.RoundButton();
            this.rbt_RightMove = new CYCustomControl.RoundButton();
            this.rbt_AlongRotate = new CYCustomControl.RoundButton();
            this.rbt_UpMove = new CYCustomControl.RoundButton();
            this.rbt_LeftMove = new CYCustomControl.RoundButton();
            this.rbt_BackMove = new CYCustomControl.RoundButton();
            this.rbt_FrontMove = new CYCustomControl.RoundButton();
            this.rbt_Stop = new CYCustomControl.RoundButton();
            this.pic_Nstp = new System.Windows.Forms.PictureBox();
            this.pic_Svon = new System.Windows.Forms.PictureBox();
            this.pic_Org = new System.Windows.Forms.PictureBox();
            this.pic_Mel = new System.Windows.Forms.PictureBox();
            this.pic_Pel = new System.Windows.Forms.PictureBox();
            this.btn_Svo = new CYCustomControl.RoundButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lab_CurPos = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmb_Axis = new System.Windows.Forms.ComboBox();
            this.pic_Homing = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmb_Station = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Alm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Nstp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Svon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Org)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Mel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Pel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Homing)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(32, 32);
            this.imageList1.TransparentColor = System.Drawing.Color.White;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btn_Home
            // 
            this.btn_Home.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Home.BackColor = System.Drawing.Color.Transparent;
            this.btn_Home.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btn_Home.BaseColorEnd = System.Drawing.Color.LightSteelBlue;
            this.btn_Home.FlatAppearance.BorderSize = 0;
            this.btn_Home.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Home.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.btn_Home.ImageHeight = 80;
            this.btn_Home.ImageWidth = 80;
            this.btn_Home.Location = new System.Drawing.Point(150, 69);
            this.btn_Home.Name = "btn_Home";
            this.btn_Home.Radius = 24;
            this.btn_Home.Size = new System.Drawing.Size(127, 36);
            this.btn_Home.SpliteButtonWidth = 18;
            this.btn_Home.TabIndex = 109;
            this.btn_Home.Text = "回零";
            this.btn_Home.UseVisualStyleBackColor = false;
            this.btn_Home.Click += new System.EventHandler(this.btn_Home_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(73, 615);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(36, 20);
            this.label11.TabIndex = 107;
            this.label11.Text = "Alm";
            // 
            // rdb_Lnching
            // 
            this.rdb_Lnching.AutoSize = true;
            this.rdb_Lnching.Location = new System.Drawing.Point(24, 186);
            this.rdb_Lnching.Name = "rdb_Lnching";
            this.rdb_Lnching.Size = new System.Drawing.Size(74, 21);
            this.rdb_Lnching.TabIndex = 106;
            this.rdb_Lnching.TabStop = true;
            this.rdb_Lnching.Text = "寸动模式";
            this.rdb_Lnching.UseVisualStyleBackColor = true;
            this.rdb_Lnching.CheckedChanged += new System.EventHandler(this.rdb_Lnching_CheckedChanged);
            // 
            // rdb_PointMove
            // 
            this.rdb_PointMove.AutoSize = true;
            this.rdb_PointMove.Location = new System.Drawing.Point(180, 186);
            this.rdb_PointMove.Name = "rdb_PointMove";
            this.rdb_PointMove.Size = new System.Drawing.Size(74, 21);
            this.rdb_PointMove.TabIndex = 105;
            this.rdb_PointMove.TabStop = true;
            this.rdb_PointMove.Text = "点动模式";
            this.rdb_PointMove.UseVisualStyleBackColor = true;
            this.rdb_PointMove.CheckedChanged += new System.EventHandler(this.rdb_PointMove_CheckedChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(320, 616);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(61, 20);
            this.label16.TabIndex = 91;
            this.label16.Text = "Moving";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(325, 69);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(719, 385);
            this.dataGridView1.TabIndex = 86;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(20, 240);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 20);
            this.label2.TabIndex = 85;
            this.label2.Text = "移动速度设定(mm/s)";
            // 
            // tb_Speed
            // 
            this.tb_Speed.DcimalDigits = 0;
            this.tb_Speed.IsShowTips = true;
            this.tb_Speed.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(231)))), ((int)(((byte)(237)))));
            this.tb_Speed.LineWidth = 10F;
            this.tb_Speed.Location = new System.Drawing.Point(9, 251);
            this.tb_Speed.MaxValue = 200F;
            this.tb_Speed.MinValue = 0F;
            this.tb_Speed.Name = "tb_Speed";
            this.tb_Speed.Size = new System.Drawing.Size(285, 73);
            this.tb_Speed.TabIndex = 76;
            this.tb_Speed.Text = "ucTrackBar1";
            this.tb_Speed.TipsFormat = "当前速度：0";
            this.tb_Speed.Value = 50F;
            this.tb_Speed.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(77)))), ((int)(((byte)(59)))));
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(29, 330);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 20);
            this.label3.TabIndex = 84;
            this.label3.Text = "移动距离(mm)";
            // 
            // ntx_Distance
            // 
            this.ntx_Distance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.ntx_Distance.InputType = HZH_Controls.TextInputType.Number;
            this.ntx_Distance.IsNumCanInput = true;
            this.ntx_Distance.KeyBoardType = HZH_Controls.Controls.KeyBoardType.Null;
            this.ntx_Distance.Location = new System.Drawing.Point(150, 315);
            this.ntx_Distance.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ntx_Distance.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ntx_Distance.Name = "ntx_Distance";
            this.ntx_Distance.Num = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.ntx_Distance.Padding = new System.Windows.Forms.Padding(2);
            this.ntx_Distance.Size = new System.Drawing.Size(152, 48);
            this.ntx_Distance.TabIndex = 83;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 81;
            this.label1.Text = "轴选择";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(176, 615);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(30, 20);
            this.label13.TabIndex = 77;
            this.label13.Text = "Pel";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(226, 615);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 20);
            this.label14.TabIndex = 78;
            this.label14.Text = "Mel";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(275, 615);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(34, 20);
            this.label15.TabIndex = 79;
            this.label15.Text = "Org";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(120, 615);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(42, 20);
            this.label12.TabIndex = 80;
            this.label12.Text = "Svon";
            // 
            // pic_Alm
            // 
            this.pic_Alm.BackColor = System.Drawing.Color.Black;
            this.pic_Alm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_Alm.Location = new System.Drawing.Point(76, 640);
            this.pic_Alm.Name = "pic_Alm";
            this.pic_Alm.Size = new System.Drawing.Size(20, 24);
            this.pic_Alm.TabIndex = 108;
            this.pic_Alm.TabStop = false;
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.Transparent;
            this.btn_Save.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Save.BackgroundImage")));
            this.btn_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Save.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Save.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Save.ContextOffset = 0;
            this.btn_Save.FlatAppearance.BorderSize = 0;
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.ImageHeight = 80;
            this.btn_Save.ImageWidth = 80;
            this.btn_Save.Location = new System.Drawing.Point(840, 477);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Radius = 24;
            this.btn_Save.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Save.Size = new System.Drawing.Size(70, 70);
            this.btn_Save.SpliteButtonWidth = 18;
            this.btn_Save.TabIndex = 104;
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Move
            // 
            this.btn_Move.BackColor = System.Drawing.Color.Transparent;
            this.btn_Move.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Move.BackgroundImage")));
            this.btn_Move.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Move.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Move.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Move.ContextOffset = 0;
            this.btn_Move.FlatAppearance.BorderSize = 0;
            this.btn_Move.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Move.ImageHeight = 80;
            this.btn_Move.ImageWidth = 80;
            this.btn_Move.Location = new System.Drawing.Point(627, 477);
            this.btn_Move.Name = "btn_Move";
            this.btn_Move.Radius = 24;
            this.btn_Move.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Move.Size = new System.Drawing.Size(70, 70);
            this.btn_Move.SpliteButtonWidth = 18;
            this.btn_Move.TabIndex = 103;
            this.btn_Move.UseVisualStyleBackColor = false;
            this.btn_Move.Click += new System.EventHandler(this.btn_Move_Click);
            // 
            // btn_GetPos
            // 
            this.btn_GetPos.BackColor = System.Drawing.Color.Transparent;
            this.btn_GetPos.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_GetPos.BackgroundImage")));
            this.btn_GetPos.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_GetPos.BaseColor = System.Drawing.Color.Transparent;
            this.btn_GetPos.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_GetPos.ContextOffset = 0;
            this.btn_GetPos.FlatAppearance.BorderSize = 0;
            this.btn_GetPos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_GetPos.ImageHeight = 80;
            this.btn_GetPos.ImageWidth = 80;
            this.btn_GetPos.Location = new System.Drawing.Point(414, 477);
            this.btn_GetPos.Name = "btn_GetPos";
            this.btn_GetPos.Radius = 24;
            this.btn_GetPos.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_GetPos.Size = new System.Drawing.Size(70, 70);
            this.btn_GetPos.SpliteButtonWidth = 18;
            this.btn_GetPos.TabIndex = 102;
            this.btn_GetPos.UseVisualStyleBackColor = false;
            this.btn_GetPos.Click += new System.EventHandler(this.btn_GetPos_Click);
            // 
            // rbt_InverseRotate
            // 
            this.rbt_InverseRotate.BackColor = System.Drawing.Color.Transparent;
            this.rbt_InverseRotate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_InverseRotate.BackgroundImage")));
            this.rbt_InverseRotate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_InverseRotate.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_InverseRotate.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_InverseRotate.FlatAppearance.BorderSize = 0;
            this.rbt_InverseRotate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_InverseRotate.ImageHeight = 80;
            this.rbt_InverseRotate.ImageWidth = 80;
            this.rbt_InverseRotate.Location = new System.Drawing.Point(195, 385);
            this.rbt_InverseRotate.Name = "rbt_InverseRotate";
            this.rbt_InverseRotate.Radius = 24;
            this.rbt_InverseRotate.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_InverseRotate.Size = new System.Drawing.Size(70, 70);
            this.rbt_InverseRotate.SpliteButtonWidth = 18;
            this.rbt_InverseRotate.TabIndex = 101;
            this.rbt_InverseRotate.UseVisualStyleBackColor = false;
            this.rbt_InverseRotate.Click += new System.EventHandler(this.rbt_InverseRotate_Click);
            this.rbt_InverseRotate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbt_InverseRotate_MouseDown);
            this.rbt_InverseRotate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rbt_InverseRotate_MouseUp);
            // 
            // rbt_DwMove
            // 
            this.rbt_DwMove.BackColor = System.Drawing.Color.Transparent;
            this.rbt_DwMove.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_DwMove.BackgroundImage")));
            this.rbt_DwMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_DwMove.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_DwMove.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_DwMove.ContextOffset = 0;
            this.rbt_DwMove.FlatAppearance.BorderSize = 0;
            this.rbt_DwMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_DwMove.ImageHeight = 60;
            this.rbt_DwMove.ImageTextSpace = 0;
            this.rbt_DwMove.ImageWidth = 60;
            this.rbt_DwMove.Location = new System.Drawing.Point(195, 521);
            this.rbt_DwMove.Name = "rbt_DwMove";
            this.rbt_DwMove.PressOffset = false;
            this.rbt_DwMove.Radius = 24;
            this.rbt_DwMove.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_DwMove.Size = new System.Drawing.Size(70, 70);
            this.rbt_DwMove.SpliteButtonWidth = 18;
            this.rbt_DwMove.TabIndex = 100;
            this.rbt_DwMove.UseVisualStyleBackColor = false;
            this.rbt_DwMove.Click += new System.EventHandler(this.rbt_DwMove_Click);
            this.rbt_DwMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbt_DwMove_MouseDown);
            this.rbt_DwMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rbt_DwMove_MouseUp);
            // 
            // rbt_RightMove
            // 
            this.rbt_RightMove.BackColor = System.Drawing.Color.Transparent;
            this.rbt_RightMove.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_RightMove.BackgroundImage")));
            this.rbt_RightMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_RightMove.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_RightMove.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_RightMove.FlatAppearance.BorderSize = 0;
            this.rbt_RightMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_RightMove.ImageHeight = 80;
            this.rbt_RightMove.ImageWidth = 80;
            this.rbt_RightMove.Location = new System.Drawing.Point(195, 453);
            this.rbt_RightMove.Name = "rbt_RightMove";
            this.rbt_RightMove.Radius = 24;
            this.rbt_RightMove.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_RightMove.Size = new System.Drawing.Size(70, 70);
            this.rbt_RightMove.SpliteButtonWidth = 18;
            this.rbt_RightMove.TabIndex = 99;
            this.rbt_RightMove.UseVisualStyleBackColor = false;
            this.rbt_RightMove.Click += new System.EventHandler(this.rbt_RightMove_Click);
            this.rbt_RightMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbt_RightMove_MouseDown);
            this.rbt_RightMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rbt_RightMove_MouseUp);
            // 
            // rbt_AlongRotate
            // 
            this.rbt_AlongRotate.BackColor = System.Drawing.Color.Transparent;
            this.rbt_AlongRotate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_AlongRotate.BackgroundImage")));
            this.rbt_AlongRotate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_AlongRotate.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_AlongRotate.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_AlongRotate.FlatAppearance.BorderSize = 0;
            this.rbt_AlongRotate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_AlongRotate.ImageHeight = 80;
            this.rbt_AlongRotate.ImageWidth = 80;
            this.rbt_AlongRotate.Location = new System.Drawing.Point(41, 521);
            this.rbt_AlongRotate.Name = "rbt_AlongRotate";
            this.rbt_AlongRotate.Radius = 24;
            this.rbt_AlongRotate.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_AlongRotate.Size = new System.Drawing.Size(70, 70);
            this.rbt_AlongRotate.SpliteButtonWidth = 18;
            this.rbt_AlongRotate.TabIndex = 98;
            this.rbt_AlongRotate.UseVisualStyleBackColor = false;
            this.rbt_AlongRotate.Click += new System.EventHandler(this.rbt_AlongRotate_Click);
            this.rbt_AlongRotate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbt_AlongRotate_MouseDown);
            this.rbt_AlongRotate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rbt_AlongRotate_MouseUp);
            // 
            // rbt_UpMove
            // 
            this.rbt_UpMove.BackColor = System.Drawing.Color.Transparent;
            this.rbt_UpMove.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_UpMove.BackgroundImage")));
            this.rbt_UpMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_UpMove.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_UpMove.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_UpMove.FlatAppearance.BorderSize = 0;
            this.rbt_UpMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_UpMove.ImageHeight = 80;
            this.rbt_UpMove.ImageWidth = 80;
            this.rbt_UpMove.Location = new System.Drawing.Point(41, 385);
            this.rbt_UpMove.Name = "rbt_UpMove";
            this.rbt_UpMove.Radius = 24;
            this.rbt_UpMove.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_UpMove.Size = new System.Drawing.Size(70, 70);
            this.rbt_UpMove.SpliteButtonWidth = 18;
            this.rbt_UpMove.TabIndex = 97;
            this.rbt_UpMove.UseVisualStyleBackColor = false;
            this.rbt_UpMove.Click += new System.EventHandler(this.rbt_UpMove_Click);
            this.rbt_UpMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbt_UpMove_MouseDown);
            this.rbt_UpMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rbt_UpMove_MouseUp);
            // 
            // rbt_LeftMove
            // 
            this.rbt_LeftMove.BackColor = System.Drawing.Color.Transparent;
            this.rbt_LeftMove.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_LeftMove.BackgroundImage")));
            this.rbt_LeftMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_LeftMove.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_LeftMove.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_LeftMove.FlatAppearance.BorderSize = 0;
            this.rbt_LeftMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_LeftMove.ImageHeight = 80;
            this.rbt_LeftMove.ImageWidth = 80;
            this.rbt_LeftMove.Location = new System.Drawing.Point(41, 453);
            this.rbt_LeftMove.Name = "rbt_LeftMove";
            this.rbt_LeftMove.Radius = 24;
            this.rbt_LeftMove.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_LeftMove.Size = new System.Drawing.Size(70, 70);
            this.rbt_LeftMove.SpliteButtonWidth = 18;
            this.rbt_LeftMove.TabIndex = 96;
            this.rbt_LeftMove.UseVisualStyleBackColor = false;
            this.rbt_LeftMove.Click += new System.EventHandler(this.rbt_LeftMove_Click);
            this.rbt_LeftMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbt_LeftMove_MouseDown);
            this.rbt_LeftMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rbt_LeftMove_MouseUp);
            // 
            // rbt_BackMove
            // 
            this.rbt_BackMove.BackColor = System.Drawing.Color.Transparent;
            this.rbt_BackMove.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_BackMove.BackgroundImage")));
            this.rbt_BackMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_BackMove.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_BackMove.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_BackMove.FlatAppearance.BorderSize = 0;
            this.rbt_BackMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_BackMove.ImageHeight = 80;
            this.rbt_BackMove.ImageWidth = 80;
            this.rbt_BackMove.Location = new System.Drawing.Point(118, 521);
            this.rbt_BackMove.Name = "rbt_BackMove";
            this.rbt_BackMove.Radius = 24;
            this.rbt_BackMove.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_BackMove.Size = new System.Drawing.Size(70, 70);
            this.rbt_BackMove.SpliteButtonWidth = 18;
            this.rbt_BackMove.TabIndex = 95;
            this.rbt_BackMove.UseVisualStyleBackColor = false;
            this.rbt_BackMove.Click += new System.EventHandler(this.rbt_BackMove_Click);
            this.rbt_BackMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbt_BackMove_MouseDown);
            this.rbt_BackMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rbt_BackMove_MouseUp);
            // 
            // rbt_FrontMove
            // 
            this.rbt_FrontMove.BackColor = System.Drawing.Color.Transparent;
            this.rbt_FrontMove.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_FrontMove.BackgroundImage")));
            this.rbt_FrontMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_FrontMove.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_FrontMove.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_FrontMove.ContextOffset = 0;
            this.rbt_FrontMove.FlatAppearance.BorderSize = 0;
            this.rbt_FrontMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_FrontMove.ImageHeight = 80;
            this.rbt_FrontMove.ImageWidth = 80;
            this.rbt_FrontMove.Location = new System.Drawing.Point(118, 385);
            this.rbt_FrontMove.Name = "rbt_FrontMove";
            this.rbt_FrontMove.Radius = 24;
            this.rbt_FrontMove.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_FrontMove.Size = new System.Drawing.Size(70, 70);
            this.rbt_FrontMove.SpliteButtonWidth = 18;
            this.rbt_FrontMove.TabIndex = 94;
            this.rbt_FrontMove.UseVisualStyleBackColor = false;
            this.rbt_FrontMove.Click += new System.EventHandler(this.rbt_FrontMove_Click);
            this.rbt_FrontMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbt_FrontMove_MouseDown);
            this.rbt_FrontMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rbt_FrontMove_MouseUp);
            // 
            // rbt_Stop
            // 
            this.rbt_Stop.BackColor = System.Drawing.Color.Transparent;
            this.rbt_Stop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbt_Stop.BackgroundImage")));
            this.rbt_Stop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbt_Stop.BaseColor = System.Drawing.Color.Transparent;
            this.rbt_Stop.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.rbt_Stop.ContextOffset = 0;
            this.rbt_Stop.FlatAppearance.BorderSize = 0;
            this.rbt_Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbt_Stop.ImageHeight = 60;
            this.rbt_Stop.ImageTextSpace = 0;
            this.rbt_Stop.ImageWidth = 60;
            this.rbt_Stop.Location = new System.Drawing.Point(119, 453);
            this.rbt_Stop.Name = "rbt_Stop";
            this.rbt_Stop.PressOffset = false;
            this.rbt_Stop.Radius = 24;
            this.rbt_Stop.RoundStyle = CYCustomControl.RoundStyle.None;
            this.rbt_Stop.Size = new System.Drawing.Size(70, 70);
            this.rbt_Stop.SpliteButtonWidth = 18;
            this.rbt_Stop.TabIndex = 93;
            this.rbt_Stop.UseVisualStyleBackColor = false;
            this.rbt_Stop.Click += new System.EventHandler(this.rbt_Stop_Click);
            // 
            // pic_Nstp
            // 
            this.pic_Nstp.BackColor = System.Drawing.Color.Black;
            this.pic_Nstp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_Nstp.Location = new System.Drawing.Point(336, 640);
            this.pic_Nstp.Name = "pic_Nstp";
            this.pic_Nstp.Size = new System.Drawing.Size(20, 24);
            this.pic_Nstp.TabIndex = 92;
            this.pic_Nstp.TabStop = false;
            // 
            // pic_Svon
            // 
            this.pic_Svon.BackColor = System.Drawing.Color.Black;
            this.pic_Svon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_Svon.Location = new System.Drawing.Point(128, 640);
            this.pic_Svon.Name = "pic_Svon";
            this.pic_Svon.Size = new System.Drawing.Size(20, 24);
            this.pic_Svon.TabIndex = 90;
            this.pic_Svon.TabStop = false;
            // 
            // pic_Org
            // 
            this.pic_Org.BackColor = System.Drawing.Color.Black;
            this.pic_Org.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_Org.Location = new System.Drawing.Point(284, 640);
            this.pic_Org.Name = "pic_Org";
            this.pic_Org.Size = new System.Drawing.Size(20, 24);
            this.pic_Org.TabIndex = 89;
            this.pic_Org.TabStop = false;
            // 
            // pic_Mel
            // 
            this.pic_Mel.BackColor = System.Drawing.Color.Black;
            this.pic_Mel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_Mel.Location = new System.Drawing.Point(232, 640);
            this.pic_Mel.Name = "pic_Mel";
            this.pic_Mel.Size = new System.Drawing.Size(20, 24);
            this.pic_Mel.TabIndex = 88;
            this.pic_Mel.TabStop = false;
            // 
            // pic_Pel
            // 
            this.pic_Pel.BackColor = System.Drawing.Color.Black;
            this.pic_Pel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_Pel.Location = new System.Drawing.Point(180, 640);
            this.pic_Pel.Name = "pic_Pel";
            this.pic_Pel.Size = new System.Drawing.Size(20, 24);
            this.pic_Pel.TabIndex = 87;
            this.pic_Pel.TabStop = false;
            // 
            // btn_Svo
            // 
            this.btn_Svo.BackColor = System.Drawing.Color.Transparent;
            this.btn_Svo.BackgroundImage = global::CYStandardProcedure.Properties.Resources.Svo;
            this.btn_Svo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Svo.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Svo.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Svo.ContextOffset = 0;
            this.btn_Svo.FlatAppearance.BorderSize = 0;
            this.btn_Svo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Svo.ImageHeight = 80;
            this.btn_Svo.ImageWidth = 80;
            this.btn_Svo.Location = new System.Drawing.Point(49, 61);
            this.btn_Svo.Name = "btn_Svo";
            this.btn_Svo.Radius = 24;
            this.btn_Svo.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Svo.Size = new System.Drawing.Size(62, 55);
            this.btn_Svo.SpliteButtonWidth = 18;
            this.btn_Svo.TabIndex = 110;
            this.btn_Svo.UseVisualStyleBackColor = false;
            this.btn_Svo.Click += new System.EventHandler(this.btn_Svo_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::CYStandardProcedure.Properties.Resources.当前位置;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(75, 128);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(55, 41);
            this.pictureBox1.TabIndex = 111;
            this.pictureBox1.TabStop = false;
            // 
            // lab_CurPos
            // 
            this.lab_CurPos.AutoSize = true;
            this.lab_CurPos.Location = new System.Drawing.Point(156, 143);
            this.lab_CurPos.Name = "lab_CurPos";
            this.lab_CurPos.Size = new System.Drawing.Size(67, 17);
            this.lab_CurPos.TabIndex = 112;
            this.lab_CurPos.Text = "9999.9999";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(229, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 17);
            this.label6.TabIndex = 113;
            this.label6.Text = "mm/°";
            // 
            // cmb_Axis
            // 
            this.cmb_Axis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Axis.FormattingEnabled = true;
            this.cmb_Axis.Location = new System.Drawing.Point(150, 30);
            this.cmb_Axis.Name = "cmb_Axis";
            this.cmb_Axis.Size = new System.Drawing.Size(127, 25);
            this.cmb_Axis.TabIndex = 114;
            this.cmb_Axis.SelectedIndexChanged += new System.EventHandler(this.cmb_Axis_SelectedIndexChanged);
            // 
            // pic_Homing
            // 
            this.pic_Homing.BackColor = System.Drawing.Color.Black;
            this.pic_Homing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_Homing.Location = new System.Drawing.Point(24, 640);
            this.pic_Homing.Name = "pic_Homing";
            this.pic_Homing.Size = new System.Drawing.Size(20, 24);
            this.pic_Homing.TabIndex = 116;
            this.pic_Homing.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(7, 616);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 20);
            this.label10.TabIndex = 115;
            this.label10.Text = "Homing";
            // 
            // cmb_Station
            // 
            this.cmb_Station.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Station.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_Station.FormattingEnabled = true;
            this.cmb_Station.Location = new System.Drawing.Point(446, 27);
            this.cmb_Station.Name = "cmb_Station";
            this.cmb_Station.Size = new System.Drawing.Size(139, 28);
            this.cmb_Station.TabIndex = 118;
            this.cmb_Station.SelectedIndexChanged += new System.EventHandler(this.cmb_Station_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(333, 30);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 20);
            this.label4.TabIndex = 117;
            this.label4.Text = "参数分类";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(715, 596);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 39);
            this.button1.TabIndex = 119;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(42, 371);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 22);
            this.label5.TabIndex = 120;
            this.label5.Text = "Z+";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(267, 569);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 22);
            this.label7.TabIndex = 121;
            this.label7.Text = "Z-";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(275, 477);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 22);
            this.label8.TabIndex = 122;
            this.label8.Text = "X+";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(12, 476);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 22);
            this.label9.TabIndex = 123;
            this.label9.Text = "X-";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(141, 366);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(27, 22);
            this.label17.TabIndex = 124;
            this.label17.Text = "Y-";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label18.Location = new System.Drawing.Point(141, 593);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(32, 22);
            this.label18.TabIndex = 125;
            this.label18.Text = "Y+";
            // 
            // AxisDebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1054, 689);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmb_Station);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pic_Homing);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cmb_Axis);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lab_CurPos);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_Svo);
            this.Controls.Add(this.btn_Home);
            this.Controls.Add(this.pic_Alm);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.rdb_Lnching);
            this.Controls.Add(this.rdb_PointMove);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Move);
            this.Controls.Add(this.btn_GetPos);
            this.Controls.Add(this.rbt_InverseRotate);
            this.Controls.Add(this.rbt_DwMove);
            this.Controls.Add(this.rbt_RightMove);
            this.Controls.Add(this.rbt_AlongRotate);
            this.Controls.Add(this.rbt_UpMove);
            this.Controls.Add(this.rbt_LeftMove);
            this.Controls.Add(this.rbt_BackMove);
            this.Controls.Add(this.rbt_FrontMove);
            this.Controls.Add(this.rbt_Stop);
            this.Controls.Add(this.pic_Nstp);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.pic_Svon);
            this.Controls.Add(this.pic_Org);
            this.Controls.Add(this.pic_Mel);
            this.Controls.Add(this.pic_Pel);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_Speed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ntx_Distance);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label12);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AxisDebugForm";
            this.Text = "电机运动";
            this.Load += new System.EventHandler(this.AxisDebugForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Alm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Nstp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Svon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Org)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Mel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Pel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Homing)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Timer timer1;
        private CYCustomControl.RoundButton btn_Home;
        private System.Windows.Forms.PictureBox pic_Alm;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RadioButton rdb_Lnching;
        private System.Windows.Forms.RadioButton rdb_PointMove;
        private CYCustomControl.RoundButton btn_Save;
        private CYCustomControl.RoundButton btn_Move;
        private CYCustomControl.RoundButton btn_GetPos;
        private CYCustomControl.RoundButton rbt_InverseRotate;
        private CYCustomControl.RoundButton rbt_DwMove;
        private CYCustomControl.RoundButton rbt_RightMove;
        private CYCustomControl.RoundButton rbt_AlongRotate;
        private CYCustomControl.RoundButton rbt_UpMove;
        private CYCustomControl.RoundButton rbt_LeftMove;
        private CYCustomControl.RoundButton rbt_BackMove;
        private CYCustomControl.RoundButton rbt_FrontMove;
        private CYCustomControl.RoundButton rbt_Stop;
        private System.Windows.Forms.PictureBox pic_Nstp;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.PictureBox pic_Svon;
        private System.Windows.Forms.PictureBox pic_Org;
        private System.Windows.Forms.PictureBox pic_Mel;
        private System.Windows.Forms.PictureBox pic_Pel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private HZH_Controls.Controls.UCTrackBar tb_Speed;
        private System.Windows.Forms.Label label3;
        private HZH_Controls.Controls.UCNumTextBox ntx_Distance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        private CYCustomControl.RoundButton btn_Svo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lab_CurPos;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmb_Axis;
        private System.Windows.Forms.PictureBox pic_Homing;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmb_Station;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
    }
}