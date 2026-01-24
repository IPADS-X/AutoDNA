namespace CYStandardProcedure
{
    partial class ToshibalRobotForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToshibalRobotForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btn_Save = new CYCustomControl.RoundButton();
            this.btn_Move = new CYCustomControl.RoundButton();
            this.btn_GetPos = new CYCustomControl.RoundButton();
            this.lab_PointTp = new System.Windows.Forms.Label();
            this.cmb_PointType = new System.Windows.Forms.ComboBox();
            this.lab_RobotTp = new System.Windows.Forms.Label();
            this.cmb_RobotType = new System.Windows.Forms.ComboBox();
            this.btn_Power = new CYCustomControl.RoundButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btn_ResetErr = new CYCustomControl.RoundButton();
            this.btn_TMel = new System.Windows.Forms.Button();
            this.btn_TPel = new System.Windows.Forms.Button();
            this.btn_CMel = new System.Windows.Forms.Button();
            this.btn_CPel = new System.Windows.Forms.Button();
            this.btn_ZMel = new System.Windows.Forms.Button();
            this.btn_ZPel = new System.Windows.Forms.Button();
            this.btn_YMel = new System.Windows.Forms.Button();
            this.btn_YPel = new System.Windows.Forms.Button();
            this.btn_XMel = new System.Windows.Forms.Button();
            this.btn_XPel = new System.Windows.Forms.Button();
            this.lab_RobotCoord = new System.Windows.Forms.Label();
            this.cmb_RobotCoord = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lab_MoveVel = new System.Windows.Forms.Label();
            this.cmb_MoveVel = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Slab_EmgInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_ErrorInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_CoordInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_Vel = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_AlMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_XPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_YPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel10 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_ZPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel8 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_CPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel12 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_TPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel20 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_Config = new System.Windows.Forms.ToolStripStatusLabel();
            this.Slab_Runsts = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.cmb_MoveMode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lab_MoveUnit = new System.Windows.Forms.Label();
            this.cmb_MoveUnit = new System.Windows.Forms.ComboBox();
            this.btn_Break = new CYCustomControl.RoundButton();
            this.lab_JogVel = new System.Windows.Forms.Label();
            this.cmb_JogVel = new System.Windows.Forms.ComboBox();
            this.lab_MoveCurve = new System.Windows.Forms.Label();
            this.cmb_MoveCurve = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_Restart = new CYCustomControl.RoundButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(8, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1036, 301);
            this.dataGridView1.TabIndex = 0;
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
            this.btn_Save.Location = new System.Drawing.Point(975, 317);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Radius = 24;
            this.btn_Save.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Save.Size = new System.Drawing.Size(70, 70);
            this.btn_Save.SpliteButtonWidth = 18;
            this.btn_Save.TabIndex = 110;
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
            this.btn_Move.Location = new System.Drawing.Point(894, 317);
            this.btn_Move.Name = "btn_Move";
            this.btn_Move.Radius = 24;
            this.btn_Move.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Move.Size = new System.Drawing.Size(70, 70);
            this.btn_Move.SpliteButtonWidth = 18;
            this.btn_Move.TabIndex = 109;
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
            this.btn_GetPos.Location = new System.Drawing.Point(813, 317);
            this.btn_GetPos.Name = "btn_GetPos";
            this.btn_GetPos.Radius = 24;
            this.btn_GetPos.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_GetPos.Size = new System.Drawing.Size(70, 70);
            this.btn_GetPos.SpliteButtonWidth = 18;
            this.btn_GetPos.TabIndex = 108;
            this.btn_GetPos.UseVisualStyleBackColor = false;
            this.btn_GetPos.Click += new System.EventHandler(this.btn_GetPos_Click);
            // 
            // lab_PointTp
            // 
            this.lab_PointTp.AutoSize = true;
            this.lab_PointTp.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_PointTp.Location = new System.Drawing.Point(192, 340);
            this.lab_PointTp.Name = "lab_PointTp";
            this.lab_PointTp.Size = new System.Drawing.Size(65, 20);
            this.lab_PointTp.TabIndex = 151;
            this.lab_PointTp.Text = "点位分类";
            // 
            // cmb_PointType
            // 
            this.cmb_PointType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_PointType.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_PointType.FormattingEnabled = true;
            this.cmb_PointType.Location = new System.Drawing.Point(189, 364);
            this.cmb_PointType.Name = "cmb_PointType";
            this.cmb_PointType.Size = new System.Drawing.Size(129, 25);
            this.cmb_PointType.TabIndex = 150;
            this.cmb_PointType.SelectedIndexChanged += new System.EventHandler(this.cmb_PointType_SelectedIndexChanged);
            // 
            // lab_RobotTp
            // 
            this.lab_RobotTp.AutoSize = true;
            this.lab_RobotTp.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_RobotTp.Location = new System.Drawing.Point(24, 340);
            this.lab_RobotTp.Name = "lab_RobotTp";
            this.lab_RobotTp.Size = new System.Drawing.Size(79, 20);
            this.lab_RobotTp.TabIndex = 149;
            this.lab_RobotTp.Text = "机器人种类";
            // 
            // cmb_RobotType
            // 
            this.cmb_RobotType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_RobotType.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_RobotType.FormattingEnabled = true;
            this.cmb_RobotType.Location = new System.Drawing.Point(21, 364);
            this.cmb_RobotType.Name = "cmb_RobotType";
            this.cmb_RobotType.Size = new System.Drawing.Size(129, 25);
            this.cmb_RobotType.TabIndex = 148;
            this.cmb_RobotType.SelectedIndexChanged += new System.EventHandler(this.cmb_RobotType_SelectedIndexChanged);
            // 
            // btn_Power
            // 
            this.btn_Power.BackColor = System.Drawing.Color.Transparent;
            this.btn_Power.BackgroundImage = global::CYStandardProcedure.Properties.Resources.上电;
            this.btn_Power.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Power.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Power.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Power.ContextOffset = 0;
            this.btn_Power.FlatAppearance.BorderSize = 0;
            this.btn_Power.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Power.ImageHeight = 80;
            this.btn_Power.ImageWidth = 80;
            this.btn_Power.Location = new System.Drawing.Point(21, 475);
            this.btn_Power.Name = "btn_Power";
            this.btn_Power.Radius = 24;
            this.btn_Power.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Power.Size = new System.Drawing.Size(70, 70);
            this.btn_Power.SpliteButtonWidth = 18;
            this.btn_Power.TabIndex = 152;
            this.btn_Power.UseVisualStyleBackColor = false;
            this.btn_Power.Click += new System.EventHandler(this.btn_Power_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btn_ResetErr
            // 
            this.btn_ResetErr.BackColor = System.Drawing.Color.Transparent;
            this.btn_ResetErr.BackgroundImage = global::CYStandardProcedure.Properties.Resources.清除;
            this.btn_ResetErr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_ResetErr.BaseColor = System.Drawing.Color.Transparent;
            this.btn_ResetErr.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_ResetErr.ContextOffset = 0;
            this.btn_ResetErr.FlatAppearance.BorderSize = 0;
            this.btn_ResetErr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ResetErr.ImageHeight = 80;
            this.btn_ResetErr.ImageWidth = 80;
            this.btn_ResetErr.Location = new System.Drawing.Point(97, 475);
            this.btn_ResetErr.Name = "btn_ResetErr";
            this.btn_ResetErr.Radius = 24;
            this.btn_ResetErr.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_ResetErr.Size = new System.Drawing.Size(70, 70);
            this.btn_ResetErr.SpliteButtonWidth = 18;
            this.btn_ResetErr.TabIndex = 153;
            this.btn_ResetErr.UseVisualStyleBackColor = false;
            this.btn_ResetErr.Click += new System.EventHandler(this.btn_ResetErr_Click);
            // 
            // btn_TMel
            // 
            this.btn_TMel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_TMel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_TMel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_TMel.Location = new System.Drawing.Point(380, 618);
            this.btn_TMel.Name = "btn_TMel";
            this.btn_TMel.Size = new System.Drawing.Size(82, 39);
            this.btn_TMel.TabIndex = 163;
            this.btn_TMel.Text = "T-";
            this.btn_TMel.UseVisualStyleBackColor = false;
            this.btn_TMel.Click += new System.EventHandler(this.btn_TMel_Click);
            this.btn_TMel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_TMel_MouseDown);
            this.btn_TMel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_TMel_MouseUp);
            // 
            // btn_TPel
            // 
            this.btn_TPel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_TPel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_TPel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_TPel.Location = new System.Drawing.Point(380, 561);
            this.btn_TPel.Name = "btn_TPel";
            this.btn_TPel.Size = new System.Drawing.Size(82, 39);
            this.btn_TPel.TabIndex = 162;
            this.btn_TPel.Text = "T+";
            this.btn_TPel.UseVisualStyleBackColor = false;
            this.btn_TPel.Click += new System.EventHandler(this.btn_TPel_Click);
            this.btn_TPel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_TPel_MouseDown);
            this.btn_TPel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_TPel_MouseUp);
            // 
            // btn_CMel
            // 
            this.btn_CMel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_CMel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_CMel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_CMel.Location = new System.Drawing.Point(292, 618);
            this.btn_CMel.Name = "btn_CMel";
            this.btn_CMel.Size = new System.Drawing.Size(82, 39);
            this.btn_CMel.TabIndex = 161;
            this.btn_CMel.Text = "C-";
            this.btn_CMel.UseVisualStyleBackColor = false;
            this.btn_CMel.Click += new System.EventHandler(this.btn_CMel_Click);
            this.btn_CMel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_CMel_MouseDown);
            this.btn_CMel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_CMel_MouseUp);
            // 
            // btn_CPel
            // 
            this.btn_CPel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_CPel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_CPel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_CPel.Location = new System.Drawing.Point(292, 561);
            this.btn_CPel.Name = "btn_CPel";
            this.btn_CPel.Size = new System.Drawing.Size(82, 39);
            this.btn_CPel.TabIndex = 160;
            this.btn_CPel.Text = "C+";
            this.btn_CPel.UseVisualStyleBackColor = false;
            this.btn_CPel.Click += new System.EventHandler(this.btn_CPel_Click);
            this.btn_CPel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_CPel_MouseDown);
            this.btn_CPel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_CPel_MouseUp);
            // 
            // btn_ZMel
            // 
            this.btn_ZMel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_ZMel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_ZMel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_ZMel.Location = new System.Drawing.Point(204, 618);
            this.btn_ZMel.Name = "btn_ZMel";
            this.btn_ZMel.Size = new System.Drawing.Size(82, 39);
            this.btn_ZMel.TabIndex = 159;
            this.btn_ZMel.Text = "Z-";
            this.btn_ZMel.UseVisualStyleBackColor = false;
            this.btn_ZMel.Click += new System.EventHandler(this.btn_ZMel_Click);
            this.btn_ZMel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_ZMel_MouseDown);
            this.btn_ZMel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_ZMel_MouseUp);
            // 
            // btn_ZPel
            // 
            this.btn_ZPel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_ZPel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_ZPel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_ZPel.Location = new System.Drawing.Point(204, 561);
            this.btn_ZPel.Name = "btn_ZPel";
            this.btn_ZPel.Size = new System.Drawing.Size(82, 39);
            this.btn_ZPel.TabIndex = 158;
            this.btn_ZPel.Text = "Z+";
            this.btn_ZPel.UseVisualStyleBackColor = false;
            this.btn_ZPel.Click += new System.EventHandler(this.btn_ZPel_Click);
            this.btn_ZPel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_ZPel_MouseDown);
            this.btn_ZPel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_ZPel_MouseUp);
            // 
            // btn_YMel
            // 
            this.btn_YMel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_YMel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_YMel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_YMel.Location = new System.Drawing.Point(116, 618);
            this.btn_YMel.Name = "btn_YMel";
            this.btn_YMel.Size = new System.Drawing.Size(82, 39);
            this.btn_YMel.TabIndex = 157;
            this.btn_YMel.Text = "Y-";
            this.btn_YMel.UseVisualStyleBackColor = false;
            this.btn_YMel.Click += new System.EventHandler(this.btn_YMel_Click);
            this.btn_YMel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_YMel_MouseDown);
            this.btn_YMel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_YMel_MouseUp);
            // 
            // btn_YPel
            // 
            this.btn_YPel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_YPel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_YPel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_YPel.Location = new System.Drawing.Point(116, 561);
            this.btn_YPel.Name = "btn_YPel";
            this.btn_YPel.Size = new System.Drawing.Size(82, 39);
            this.btn_YPel.TabIndex = 156;
            this.btn_YPel.Text = "Y+";
            this.btn_YPel.UseVisualStyleBackColor = false;
            this.btn_YPel.Click += new System.EventHandler(this.btn_YPel_Click);
            this.btn_YPel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_YPel_MouseDown);
            this.btn_YPel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_YPel_MouseUp);
            // 
            // btn_XMel
            // 
            this.btn_XMel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_XMel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_XMel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_XMel.Location = new System.Drawing.Point(28, 618);
            this.btn_XMel.Name = "btn_XMel";
            this.btn_XMel.Size = new System.Drawing.Size(82, 39);
            this.btn_XMel.TabIndex = 155;
            this.btn_XMel.Text = "X-";
            this.btn_XMel.UseVisualStyleBackColor = false;
            this.btn_XMel.Click += new System.EventHandler(this.btn_XMel_Click);
            this.btn_XMel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_XMel_MouseDown);
            this.btn_XMel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_XMel_MouseUp);
            // 
            // btn_XPel
            // 
            this.btn_XPel.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_XPel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_XPel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_XPel.Location = new System.Drawing.Point(28, 561);
            this.btn_XPel.Name = "btn_XPel";
            this.btn_XPel.Size = new System.Drawing.Size(82, 39);
            this.btn_XPel.TabIndex = 154;
            this.btn_XPel.Text = "X+";
            this.btn_XPel.UseVisualStyleBackColor = false;
            this.btn_XPel.Click += new System.EventHandler(this.btn_XPel_Click);
            this.btn_XPel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_XPel_MouseDown);
            this.btn_XPel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_XPel_MouseUp);
            // 
            // lab_RobotCoord
            // 
            this.lab_RobotCoord.AutoSize = true;
            this.lab_RobotCoord.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_RobotCoord.Location = new System.Drawing.Point(24, 408);
            this.lab_RobotCoord.Name = "lab_RobotCoord";
            this.lab_RobotCoord.Size = new System.Drawing.Size(51, 20);
            this.lab_RobotCoord.TabIndex = 165;
            this.lab_RobotCoord.Text = "坐标系";
            // 
            // cmb_RobotCoord
            // 
            this.cmb_RobotCoord.AutoCompleteCustomSource.AddRange(new string[] {
            "Joint",
            "Base",
            "Tool",
            "User"});
            this.cmb_RobotCoord.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_RobotCoord.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_RobotCoord.FormattingEnabled = true;
            this.cmb_RobotCoord.Items.AddRange(new object[] {
            "Joint",
            "Tool",
            "Work",
            "World"});
            this.cmb_RobotCoord.Location = new System.Drawing.Point(21, 432);
            this.cmb_RobotCoord.Name = "cmb_RobotCoord";
            this.cmb_RobotCoord.Size = new System.Drawing.Size(92, 25);
            this.cmb_RobotCoord.TabIndex = 164;
            this.cmb_RobotCoord.SelectedIndexChanged += new System.EventHandler(this.cmb_RobotCoord_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(614, 434);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 20);
            this.label6.TabIndex = 168;
            this.label6.Text = "%";
            // 
            // lab_MoveVel
            // 
            this.lab_MoveVel.AutoSize = true;
            this.lab_MoveVel.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_MoveVel.Location = new System.Drawing.Point(528, 408);
            this.lab_MoveVel.Name = "lab_MoveVel";
            this.lab_MoveVel.Size = new System.Drawing.Size(65, 20);
            this.lab_MoveVel.TabIndex = 167;
            this.lab_MoveVel.Text = "点位速度";
            // 
            // cmb_MoveVel
            // 
            this.cmb_MoveVel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_MoveVel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_MoveVel.FormattingEnabled = true;
            this.cmb_MoveVel.Items.AddRange(new object[] {
            "80",
            "50",
            "20",
            "10",
            "5",
            "1"});
            this.cmb_MoveVel.Location = new System.Drawing.Point(525, 432);
            this.cmb_MoveVel.Name = "cmb_MoveVel";
            this.cmb_MoveVel.Size = new System.Drawing.Size(85, 25);
            this.cmb_MoveVel.TabIndex = 166;
            this.cmb_MoveVel.SelectedIndexChanged += new System.EventHandler(this.cmb_MoveVel_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Slab_EmgInfo,
            this.Slab_ErrorInfo,
            this.Slab_CoordInfo,
            this.toolStripStatusLabel3,
            this.Slab_Vel,
            this.Slab_AlMsg});
            this.statusStrip1.Location = new System.Drawing.Point(0, 705);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1054, 28);
            this.statusStrip1.TabIndex = 169;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Slab_EmgInfo
            // 
            this.Slab_EmgInfo.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_EmgInfo.Name = "Slab_EmgInfo";
            this.Slab_EmgInfo.Size = new System.Drawing.Size(40, 23);
            this.Slab_EmgInfo.Text = "EMG";
            // 
            // Slab_ErrorInfo
            // 
            this.Slab_ErrorInfo.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_ErrorInfo.Name = "Slab_ErrorInfo";
            this.Slab_ErrorInfo.Size = new System.Drawing.Size(42, 23);
            this.Slab_ErrorInfo.Text = "Error";
            // 
            // Slab_CoordInfo
            // 
            this.Slab_CoordInfo.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_CoordInfo.Name = "Slab_CoordInfo";
            this.Slab_CoordInfo.Size = new System.Drawing.Size(48, 23);
            this.Slab_CoordInfo.Text = "World";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(81, 23);
            this.toolStripStatusLabel3.Text = "CurrentVel：";
            // 
            // Slab_Vel
            // 
            this.Slab_Vel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_Vel.Name = "Slab_Vel";
            this.Slab_Vel.Size = new System.Drawing.Size(50, 23);
            this.Slab_Vel.Text = "100.00";
            // 
            // Slab_AlMsg
            // 
            this.Slab_AlMsg.Name = "Slab_AlMsg";
            this.Slab_AlMsg.Size = new System.Drawing.Size(131, 23);
            this.Slab_AlMsg.Text = "toolStripStatusLabel1";
            // 
            // statusStrip2
            // 
            this.statusStrip2.AutoSize = false;
            this.statusStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.Slab_XPos,
            this.toolStripStatusLabel4,
            this.Slab_YPos,
            this.toolStripStatusLabel10,
            this.Slab_ZPos,
            this.toolStripStatusLabel8,
            this.Slab_CPos,
            this.toolStripStatusLabel12,
            this.Slab_TPos,
            this.toolStripStatusLabel20,
            this.Slab_Config,
            this.Slab_Runsts});
            this.statusStrip2.Location = new System.Drawing.Point(0, 677);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(1054, 28);
            this.statusStrip2.TabIndex = 170;
            this.statusStrip2.Text = "statusStrip2";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(40, 23);
            this.toolStripStatusLabel2.Text = "XPos:";
            // 
            // Slab_XPos
            // 
            this.Slab_XPos.AutoSize = false;
            this.Slab_XPos.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_XPos.Name = "Slab_XPos";
            this.Slab_XPos.Size = new System.Drawing.Size(60, 23);
            this.Slab_XPos.Text = "999.999";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(39, 23);
            this.toolStripStatusLabel4.Text = "YPos:";
            // 
            // Slab_YPos
            // 
            this.Slab_YPos.AutoSize = false;
            this.Slab_YPos.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_YPos.Name = "Slab_YPos";
            this.Slab_YPos.Size = new System.Drawing.Size(60, 23);
            this.Slab_YPos.Text = "999.999";
            // 
            // toolStripStatusLabel10
            // 
            this.toolStripStatusLabel10.Name = "toolStripStatusLabel10";
            this.toolStripStatusLabel10.Size = new System.Drawing.Size(39, 23);
            this.toolStripStatusLabel10.Text = "ZPos:";
            // 
            // Slab_ZPos
            // 
            this.Slab_ZPos.AutoSize = false;
            this.Slab_ZPos.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_ZPos.Name = "Slab_ZPos";
            this.Slab_ZPos.Size = new System.Drawing.Size(60, 23);
            this.Slab_ZPos.Text = "999.999";
            // 
            // toolStripStatusLabel8
            // 
            this.toolStripStatusLabel8.Name = "toolStripStatusLabel8";
            this.toolStripStatusLabel8.Size = new System.Drawing.Size(40, 23);
            this.toolStripStatusLabel8.Text = "CPos:";
            // 
            // Slab_CPos
            // 
            this.Slab_CPos.AutoSize = false;
            this.Slab_CPos.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_CPos.Name = "Slab_CPos";
            this.Slab_CPos.Size = new System.Drawing.Size(60, 23);
            this.Slab_CPos.Text = "999.999";
            // 
            // toolStripStatusLabel12
            // 
            this.toolStripStatusLabel12.Name = "toolStripStatusLabel12";
            this.toolStripStatusLabel12.Size = new System.Drawing.Size(39, 23);
            this.toolStripStatusLabel12.Text = "TPos:";
            // 
            // Slab_TPos
            // 
            this.Slab_TPos.AutoSize = false;
            this.Slab_TPos.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_TPos.Name = "Slab_TPos";
            this.Slab_TPos.Size = new System.Drawing.Size(60, 23);
            this.Slab_TPos.Text = "999.999";
            // 
            // toolStripStatusLabel20
            // 
            this.toolStripStatusLabel20.Name = "toolStripStatusLabel20";
            this.toolStripStatusLabel20.Size = new System.Drawing.Size(49, 23);
            this.toolStripStatusLabel20.Text = "Config:";
            // 
            // Slab_Config
            // 
            this.Slab_Config.AutoSize = false;
            this.Slab_Config.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.Slab_Config.Name = "Slab_Config";
            this.Slab_Config.Size = new System.Drawing.Size(58, 23);
            this.Slab_Config.Text = "RIGHTY";
            // 
            // Slab_Runsts
            // 
            this.Slab_Runsts.Name = "Slab_Runsts";
            this.Slab_Runsts.Size = new System.Drawing.Size(41, 23);
            this.Slab_Runsts.Text = "Move";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(360, 340);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 172;
            this.label1.Text = "运动模式";
            // 
            // cmb_MoveMode
            // 
            this.cmb_MoveMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_MoveMode.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_MoveMode.FormattingEnabled = true;
            this.cmb_MoveMode.Items.AddRange(new object[] {
            "ContinuousMode",
            "InchingMode"});
            this.cmb_MoveMode.Location = new System.Drawing.Point(357, 364);
            this.cmb_MoveMode.Name = "cmb_MoveMode";
            this.cmb_MoveMode.Size = new System.Drawing.Size(129, 25);
            this.cmb_MoveMode.TabIndex = 171;
            this.cmb_MoveMode.SelectedIndexChanged += new System.EventHandler(this.cmb_MoveMode_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(446, 434);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 20);
            this.label2.TabIndex = 175;
            this.label2.Text = "mm/°";
            // 
            // lab_MoveUnit
            // 
            this.lab_MoveUnit.AutoSize = true;
            this.lab_MoveUnit.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_MoveUnit.Location = new System.Drawing.Point(360, 408);
            this.lab_MoveUnit.Name = "lab_MoveUnit";
            this.lab_MoveUnit.Size = new System.Drawing.Size(37, 20);
            this.lab_MoveUnit.TabIndex = 174;
            this.lab_MoveUnit.Text = "步距";
            // 
            // cmb_MoveUnit
            // 
            this.cmb_MoveUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_MoveUnit.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_MoveUnit.FormattingEnabled = true;
            this.cmb_MoveUnit.Items.AddRange(new object[] {
            "50",
            "20",
            "10",
            "5",
            "2",
            "1",
            "0.5",
            "0.1",
            "0.05",
            "0.01"});
            this.cmb_MoveUnit.Location = new System.Drawing.Point(357, 432);
            this.cmb_MoveUnit.Name = "cmb_MoveUnit";
            this.cmb_MoveUnit.Size = new System.Drawing.Size(85, 25);
            this.cmb_MoveUnit.TabIndex = 173;
            // 
            // btn_Break
            // 
            this.btn_Break.BackColor = System.Drawing.Color.Transparent;
            this.btn_Break.BackgroundImage = global::CYStandardProcedure.Properties.Resources.急停;
            this.btn_Break.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Break.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Break.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Break.ContextOffset = 0;
            this.btn_Break.FlatAppearance.BorderSize = 0;
            this.btn_Break.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Break.ImageHeight = 80;
            this.btn_Break.ImageWidth = 80;
            this.btn_Break.Location = new System.Drawing.Point(173, 475);
            this.btn_Break.Name = "btn_Break";
            this.btn_Break.Radius = 24;
            this.btn_Break.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Break.Size = new System.Drawing.Size(70, 70);
            this.btn_Break.SpliteButtonWidth = 18;
            this.btn_Break.TabIndex = 176;
            this.btn_Break.UseVisualStyleBackColor = false;
            this.btn_Break.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // lab_JogVel
            // 
            this.lab_JogVel.AutoSize = true;
            this.lab_JogVel.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_JogVel.Location = new System.Drawing.Point(192, 408);
            this.lab_JogVel.Name = "lab_JogVel";
            this.lab_JogVel.Size = new System.Drawing.Size(99, 20);
            this.lab_JogVel.TabIndex = 178;
            this.lab_JogVel.Text = "Jog速度(寸动)";
            // 
            // cmb_JogVel
            // 
            this.cmb_JogVel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_JogVel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_JogVel.FormattingEnabled = true;
            this.cmb_JogVel.Items.AddRange(new object[] {
            "LOW",
            "MID",
            "HIGH"});
            this.cmb_JogVel.Location = new System.Drawing.Point(189, 432);
            this.cmb_JogVel.Name = "cmb_JogVel";
            this.cmb_JogVel.Size = new System.Drawing.Size(85, 25);
            this.cmb_JogVel.TabIndex = 177;
            this.cmb_JogVel.SelectedIndexChanged += new System.EventHandler(this.cmb_JogVel_SelectedIndexChanged);
            // 
            // lab_MoveCurve
            // 
            this.lab_MoveCurve.AutoSize = true;
            this.lab_MoveCurve.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_MoveCurve.Location = new System.Drawing.Point(528, 340);
            this.lab_MoveCurve.Name = "lab_MoveCurve";
            this.lab_MoveCurve.Size = new System.Drawing.Size(65, 20);
            this.lab_MoveCurve.TabIndex = 180;
            this.lab_MoveCurve.Text = "轨迹模式";
            // 
            // cmb_MoveCurve
            // 
            this.cmb_MoveCurve.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_MoveCurve.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_MoveCurve.FormattingEnabled = true;
            this.cmb_MoveCurve.Items.AddRange(new object[] {
            "Move（弧线运动）",
            "MoveS（直线运动）",
            "MoveJ（拱形运动）"});
            this.cmb_MoveCurve.Location = new System.Drawing.Point(525, 364);
            this.cmb_MoveCurve.Name = "cmb_MoveCurve";
            this.cmb_MoveCurve.Size = new System.Drawing.Size(129, 25);
            this.cmb_MoveCurve.TabIndex = 179;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(878, 576);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 47);
            this.button1.TabIndex = 181;
            this.button1.Text = "对针模式（点胶）";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_Restart
            // 
            this.btn_Restart.BackColor = System.Drawing.Color.Transparent;
            this.btn_Restart.BackgroundImage = global::CYStandardProcedure.Properties.Resources.重启;
            this.btn_Restart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Restart.BaseColor = System.Drawing.Color.Transparent;
            this.btn_Restart.BaseColorEnd = System.Drawing.SystemColors.GradientActiveCaption;
            this.btn_Restart.ContextOffset = 0;
            this.btn_Restart.FlatAppearance.BorderSize = 0;
            this.btn_Restart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Restart.ImageHeight = 80;
            this.btn_Restart.ImageWidth = 80;
            this.btn_Restart.Location = new System.Drawing.Point(249, 475);
            this.btn_Restart.Name = "btn_Restart";
            this.btn_Restart.Radius = 24;
            this.btn_Restart.RoundStyle = CYCustomControl.RoundStyle.None;
            this.btn_Restart.Size = new System.Drawing.Size(70, 70);
            this.btn_Restart.SpliteButtonWidth = 18;
            this.btn_Restart.TabIndex = 182;
            this.btn_Restart.UseVisualStyleBackColor = false;
            this.btn_Restart.Click += new System.EventHandler(this.btn_Restart_Click);
            // 
            // ToshibalRobotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1054, 733);
            this.Controls.Add(this.btn_Restart);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lab_MoveCurve);
            this.Controls.Add(this.cmb_MoveCurve);
            this.Controls.Add(this.lab_JogVel);
            this.Controls.Add(this.cmb_JogVel);
            this.Controls.Add(this.btn_Break);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lab_MoveUnit);
            this.Controls.Add(this.cmb_MoveUnit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmb_MoveMode);
            this.Controls.Add(this.statusStrip2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lab_MoveVel);
            this.Controls.Add(this.cmb_MoveVel);
            this.Controls.Add(this.lab_RobotCoord);
            this.Controls.Add(this.cmb_RobotCoord);
            this.Controls.Add(this.btn_TMel);
            this.Controls.Add(this.btn_TPel);
            this.Controls.Add(this.btn_CMel);
            this.Controls.Add(this.btn_CPel);
            this.Controls.Add(this.btn_ZMel);
            this.Controls.Add(this.btn_ZPel);
            this.Controls.Add(this.btn_YMel);
            this.Controls.Add(this.btn_YPel);
            this.Controls.Add(this.btn_XMel);
            this.Controls.Add(this.btn_XPel);
            this.Controls.Add(this.btn_ResetErr);
            this.Controls.Add(this.btn_Power);
            this.Controls.Add(this.lab_PointTp);
            this.Controls.Add(this.cmb_PointType);
            this.Controls.Add(this.lab_RobotTp);
            this.Controls.Add(this.cmb_RobotType);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Move);
            this.Controls.Add(this.btn_GetPos);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ToshibalRobotForm";
            this.Text = "ToshibalRobotForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ToshibalRobotForm_FormClosing);
            this.Load += new System.EventHandler(this.ToshibalRobotForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private CYCustomControl.RoundButton btn_Save;
        private CYCustomControl.RoundButton btn_Move;
        private CYCustomControl.RoundButton btn_GetPos;
        private System.Windows.Forms.Label lab_PointTp;
        private System.Windows.Forms.ComboBox cmb_PointType;
        private System.Windows.Forms.Label lab_RobotTp;
        private System.Windows.Forms.ComboBox cmb_RobotType;
        private CYCustomControl.RoundButton btn_Power;
        private System.Windows.Forms.Timer timer1;
        private CYCustomControl.RoundButton btn_ResetErr;
        private System.Windows.Forms.Button btn_TMel;
        private System.Windows.Forms.Button btn_TPel;
        private System.Windows.Forms.Button btn_CMel;
        private System.Windows.Forms.Button btn_CPel;
        private System.Windows.Forms.Button btn_ZMel;
        private System.Windows.Forms.Button btn_ZPel;
        private System.Windows.Forms.Button btn_YMel;
        private System.Windows.Forms.Button btn_YPel;
        private System.Windows.Forms.Button btn_XMel;
        private System.Windows.Forms.Button btn_XPel;
        private System.Windows.Forms.Label lab_RobotCoord;
        private System.Windows.Forms.ComboBox cmb_RobotCoord;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lab_MoveVel;
        private System.Windows.Forms.ComboBox cmb_MoveVel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel Slab_EmgInfo;
        private System.Windows.Forms.ToolStripStatusLabel Slab_ErrorInfo;
        private System.Windows.Forms.ToolStripStatusLabel Slab_CoordInfo;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel Slab_Vel;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel Slab_XPos;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel Slab_YPos;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel10;
        private System.Windows.Forms.ToolStripStatusLabel Slab_ZPos;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel8;
        private System.Windows.Forms.ToolStripStatusLabel Slab_CPos;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel12;
        private System.Windows.Forms.ToolStripStatusLabel Slab_TPos;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel20;
        private System.Windows.Forms.ToolStripStatusLabel Slab_Config;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmb_MoveMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lab_MoveUnit;
        private System.Windows.Forms.ComboBox cmb_MoveUnit;
        private CYCustomControl.RoundButton btn_Break;
        private System.Windows.Forms.Label lab_JogVel;
        private System.Windows.Forms.ComboBox cmb_JogVel;
        private System.Windows.Forms.Label lab_MoveCurve;
        private System.Windows.Forms.ComboBox cmb_MoveCurve;
        private System.Windows.Forms.Button button1;
        private CYCustomControl.RoundButton btn_Restart;
        private System.Windows.Forms.ToolStripStatusLabel Slab_AlMsg;
        private System.Windows.Forms.ToolStripStatusLabel Slab_Runsts;
    }
}