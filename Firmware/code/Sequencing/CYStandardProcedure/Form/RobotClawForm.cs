using CYAutoFramework;
using CYCustomControl;
using CYStandardProcedure;
using Modbus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class RobotClawForm : Form
    {

        int[] result = null;

        bool[] resultBool = null;

        bool isWrite = false;



        public ModbusRtu Rtu_robotClaw = new ModbusRtu();

        public string path_RobotClaw = Application.StartupPath + "\\ExeFile\\RobotClaw\\ModbusRtuConfig.xml";

        public string path2_RobotClaw = Application.StartupPath + "\\ExeFile\\RobotClaw\\MoveConfig.xml";

        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        /***提示语***/
        private ToolTip toolTip = new ToolTip();

        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        public RobotClawForm()
        {
            InitializeComponent();
        }

        private void GripPawl_Load(object sender, EventArgs e)
        {
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += DebugForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);

            #region 初始化控件

            //端口号

            string[] PortList = SerialPort.GetPortNames();

            if (PortList.Length > 0)
            {
                this.cmb_Port.Items.AddRange(PortList);
                this.cmb_Port.SelectedIndex = 0;
            }

            //校验位
            this.cmb_Parity.DataSource = Enum.GetNames(typeof(Parity));

            //停止位
            this.cmb_StopBits.DataSource = Enum.GetNames(typeof(StopBits));

            //数据格式
            this.cmb_DataFormat.DataSource = Enum.GetNames(typeof(DataFormat));


            Program.robotClawConfig = Xml_SerializerHelper.XmlDeserialize<ModbusRtuConfig>(path_RobotClaw);
            cmb_Port.Text = Program.robotClawConfig.iPortName;
            txt_Paud.Text = Program.robotClawConfig.iBaudRate.ToString();
            cmb_Parity.Text = Program.robotClawConfig.iParity.ToString();
            txt_DataBits.Text = Program.robotClawConfig.iDataBits.ToString();
            cmb_StopBits.Text = Program.robotClawConfig.iStopBits.ToString();
            txt_SlaveAdd.Text = Program.robotClawConfig.DevAdd.ToString();
            cmb_DataFormat.Text = Program.robotClawConfig.DataFormat.ToString();
            #endregion

            #region 加载modbusRtu参数

            Rtu_robotClaw.DataFormat = Program.robotClawConfig.DataFormat;

            #endregion



            #region 添加表头

            this.dgv_data.AutoGenerateColumns = false;

            this.dgv_data.Columns.Clear();

            DataGridViewTextBoxColumn dgvc1 = new DataGridViewTextBoxColumn();
            dgvc1.HeaderText = "点位名";
            dgvc1.ReadOnly = true;
            dgvc1.Width = 200;
            dgvc1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgv_data.Columns.Add(dgvc1);


            DataGridViewTextBoxColumn dgvc2 = new DataGridViewTextBoxColumn();
            dgvc2.HeaderText = "位置(单位：um)";
            dgvc2.ReadOnly = false;
            dgvc2.Width = 200;
            dgvc2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgv_data.Columns.Add(dgvc2);

            DataGridViewTextBoxColumn dgvc3 = new DataGridViewTextBoxColumn();
            dgvc3.HeaderText = "速度(单位：um/s)";
            dgvc3.ReadOnly = false;
            dgvc3.Width = 200;
            dgvc3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgv_data.Columns.Add(dgvc3);

            DataGridViewTextBoxColumn dgvc4 = new DataGridViewTextBoxColumn();
            dgvc4.HeaderText = "加速度(单位：um/s^2)";
            dgvc4.ReadOnly = false;
            dgvc4.Width = 200;
            dgvc4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgv_data.Columns.Add(dgvc4);

            DataGridViewTextBoxColumn dgvc5 = new DataGridViewTextBoxColumn();
            dgvc5.HeaderText = "定位区间(单位：um)";
            dgvc5.ReadOnly = false;
            dgvc5.Width = 200;
            dgvc5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgv_data.Columns.Add(dgvc5);


            DataGridViewTextBoxColumn dgvc6 = new DataGridViewTextBoxColumn();
            dgvc6.HeaderText = "推压力";
            dgvc6.ReadOnly = false;
            dgvc6.Width = 200;
            dgvc6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgv_data.Columns.Add(dgvc6);

            DataGridViewTextBoxColumn dgvc7 = new DataGridViewTextBoxColumn();
            dgvc7.HeaderText = "推压距离(单位：um)";
            dgvc7.ReadOnly = false;
            dgvc7.Width = 200;
            dgvc7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgv_data.Columns.Add(dgvc7);


            DataGridViewTextBoxColumn dgvc8 = new DataGridViewTextBoxColumn();
            dgvc8.HeaderText = "延时时间(单位：ms)";
            dgvc8.ReadOnly = false;
            dgvc8.Width = 200;
            dgvc8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dgv_data.Columns.Add(dgvc8);

            #endregion


            #region 加载电动夹爪运动参数

            Program.robotClawConfigList = Xml_SerializerHelper.XmlDeserialize<List<GripPawlConfig>>(path2_RobotClaw);
            Set_dgv();

            #endregion

        }

        private void DebugForm_LanguageChangeEvent(string strLanguage)
        {
            if (strLanguage == "CH")
            {
                toolTip.SetToolTip(rb_Open, "打开串口");
                toolTip.SetToolTip(rb_Close, "关闭串口");
                toolTip.SetToolTip(rb_Save, "保存");
                toolTip.SetToolTip(btn_Svo, "使能");
                toolTip.SetToolTip(btn_Home, "回零");
                toolTip.SetToolTip(btn_ResetError, "报警清除");
                toolTip.SetToolTip(btn_StopMove, "停止");
                toolTip.SetToolTip(ntx_Distance, "移动距离");
                toolTip.SetToolTip(btn_MoveN, "负向运动");
                toolTip.SetToolTip(btn_MoveP, "正向运动");
                toolTip.SetToolTip(btn_GetPos, "提取坐标");
                toolTip.SetToolTip(btn_Move, "点运动");
                toolTip.SetToolTip(btn_Save, "点保存");

            }
            else if (strLanguage == "EN")
            {

            }
            else
            {

            }
        }

        private void rb_Save_Click(object sender, EventArgs e)
        {
            Program.robotClawConfig.iPortName = cmb_Port.Text;
            Program.robotClawConfig.iBaudRate = int.Parse(txt_Paud.Text);
            Program.robotClawConfig.iParity = (Parity)Enum.Parse(typeof(Parity), cmb_Parity.Text);
            Program.robotClawConfig.iDataBits = int.Parse(txt_DataBits.Text);
            Program.robotClawConfig.iStopBits = (StopBits)Enum.Parse(typeof(StopBits), cmb_StopBits.Text);
            Program.robotClawConfig.DevAdd = int.Parse(txt_SlaveAdd.Text);
            Program.robotClawConfig.DataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), cmb_DataFormat.Text);

            #region 保存modbusRtu参数

            Rtu_robotClaw.DataFormat = Program.robotClawConfig.DataFormat;

            #endregion


            if (!File.Exists(path_RobotClaw))
            {
                FileStream fs = File.Create(path_RobotClaw);
                fs.Close();
            }
            if (Xml_SerializerHelper.XmlSerializer(Program.robotClawConfig, path_RobotClaw))
            {
                MessageBox.Show("保存数据成功");
            }

        }

        private void rb_Open_Click(object sender, EventArgs e)
        {
            if (Rtu_robotClaw.OpenMyCom(Program.robotClawConfig.iBaudRate, Program.robotClawConfig.iPortName, Program.robotClawConfig.iDataBits, Program.robotClawConfig.iParity, Program.robotClawConfig.iStopBits))
            {
                MessageBox.Show("打开串口成功");
            }
            else
            {
                MessageBox.Show("打开串口失败");
            }
        }

        private void rb_Close_Click(object sender, EventArgs e)
        {
            Rtu_robotClaw.CloseMyCom();

        }


        int[] result2 = null;

        bool[] resultBool2 = null;

        bool isWrite2 = false;
        private void timer1_Tick(object sender, EventArgs e)
        {

            #region 读取位置

            result2 = null;
            result2 = Rtu_robotClaw.ReadInputRegInt(Program.robotClawConfig.DevAdd, 0, 1);
            if (result2 != null && result2.Length == 1)
            {
                tx_FeedbackPosition.Text = result2[0].ToString();   //单位um
            }

            #endregion


            #region 读取速度

            result2 = null;
            result2 = Rtu_robotClaw.ReadInputRegInt(Program.robotClawConfig.DevAdd, 2, 1);
            if (result2 != null && result2.Length == 1)
            {
                tx_FeedbackVelocity.Text = result2[0].ToString();   //单位um
            }


            #endregion


            #region 读取力矩

            result2 = null;
            result2 = Rtu_robotClaw.ReadKeepRegInt(Program.robotClawConfig.DevAdd, 2154, 1);
            if (result2 != null && result2.Length == 1)
            {
                tx_Torque.Text = result2[0].ToString();   //单位um
            }

            #endregion

            #region 读取报警状态

            resultBool2 = null;
            resultBool2 = Rtu_robotClaw.ReadOutputStatusBool(Program.robotClawConfig.DevAdd, 0, 1);
            if (resultBool2 != null && resultBool2.Length == 1)
            {
                if (resultBool2[0]) //报警
                {
                    lb_Err.BackColor = Color.Red;
                }
                else
                {
                    lb_Err.BackColor = Color.Green;
                }
            }

            #endregion

            #region 读取使能状态

            resultBool2 = null;
            resultBool2 = Rtu_robotClaw.ReadOutputStatusBool(Program.robotClawConfig.DevAdd, 1, 1);
            if (resultBool2 != null && resultBool2.Length == 1 && resultBool2[0] == true)
            {
                btn_Svo.Tag = "上使能";
                btn_Svo.BackgroundImage = Properties.Resources.Svo;
            }
            else
            {
                btn_Svo.Tag = "失使能";
                btn_Svo.BackgroundImage = Properties.Resources.NoSvo;
            }
           

            #endregion
        }

        private void btn_Svo_Click(object sender, EventArgs e)
        {
            if (btn_Svo.Tag.ToString() == "失使能")
            {
                if (Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1, true))
                {
                    btn_Svo.Tag = "上使能";
                    btn_Svo.BackgroundImage = Properties.Resources.Svo;
                }

            }
            else if (btn_Svo.Tag.ToString() == "上使能")
            {

                if (Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1, false))
                {
                    btn_Svo.Tag = "失使能";
                    btn_Svo.BackgroundImage = Properties.Resources.NoSvo;
                }

            }
        }

        private void btn_Home_Click(object sender, EventArgs e)
        {
            isWrite = false;
            isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 17, false);
            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 17, true);
            }
        }

        private void btn_ResetError_Click(object sender, EventArgs e)
        {
            isWrite = false;
            isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 0, false);
            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 0, true);
            }

        }

        private void btn_StopMove_Click(object sender, EventArgs e)
        {
            isWrite = false;
            isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 3, false);
            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 3, true);
            }
        }

        private void btn_MoveN_Click(object sender, EventArgs e)
        {
            result = null;
            result = Rtu_robotClaw.ReadInputRegInt(Program.robotClawConfig.DevAdd, 0, 1);   //读取当前位置  //单位um
            if (result != null && result.Length == 1)
            {
                /*
                指令类型
                位置
                速度
                加速度
                减速度
                定位区间
                *
                *
                *
                下一步指令序号
                */

                isWrite = false;
                isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5000, new int[]
                {
                    3,
                    result[0]- Convert.ToInt32(ntx_Distance.Num),
                    Convert.ToInt32(nud_PushVM.Value),
                    Convert.ToInt32(nud_PushAcc.Value),
                    Convert.ToInt32(nud_PushAcc.Value),
                    Convert.ToInt32(nud_OrientationRange.Value),
                });
            }

            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5000 + 2, new int[] { -1 });
            }

            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000, false);
            }
            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000, true);
            }
        }

        private void btn_MoveP_Click(object sender, EventArgs e)
        {
            result = null;
            result = Rtu_robotClaw.ReadInputRegInt(Program.robotClawConfig.DevAdd, 0, 1);   //读取当前位置  //单位um
            if (result != null && result.Length == 1)
            {
                /*
                指令类型
                位置
                速度
                加速度
                减速度
                定位区间
                *
                *
                *
                下一步指令序号
                */

                isWrite = false;
                isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5000, new int[]  //5040
                {
                    3,
                    result[0]+ Convert.ToInt32(ntx_Distance.Num),
                    Convert.ToInt32(nud_PushVM.Value),
                    Convert.ToInt32(nud_PushAcc.Value),
                    Convert.ToInt32(nud_PushAcc.Value),
                    Convert.ToInt32(nud_OrientationRange.Value),

                });
            }

            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5000 + 2, new int[] { -1 });
            }

            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000, false);
            }
            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000, true);
            }
        }


        private void btn_Save_Click(object sender, EventArgs e)
        {

            Get_dgv();
            if (!File.Exists(path2_RobotClaw))
            {
                FileStream fs = File.Create(path2_RobotClaw);
                fs.Close();
            }
            if (Xml_SerializerHelper.XmlSerializer(Program.robotClawConfigList, path2_RobotClaw))
            {
                MessageBox.Show("保存数据成功");
            }
        }


        #region 保存数据到dvg中

        /// <summary>
        /// 保存数据到dvg中
        /// </summary>
        public void Set_dgv()
        {

            this.dgv_data.Rows.Clear();

            for (int i = 0; i < Program.robotClawConfigList.Count; i++)
            {
                int index = this.dgv_data.Rows.Add();
                this.dgv_data.Rows[i].Height = 25;  //设置行的高度
                this.dgv_data.Rows[i].Resizable = DataGridViewTriState.False;  //不能设置行的高度


                this.dgv_data.Rows[index].Cells[0].Value = Program.robotClawConfigList[i].PointName;
                                                                   
                this.dgv_data.Rows[index].Cells[1].Value = Program.robotClawConfigList[i].PushDistance;
                                                                   
                this.dgv_data.Rows[index].Cells[2].Value = Program.robotClawConfigList[i].PushVM;
                                                                   
                this.dgv_data.Rows[index].Cells[3].Value = Program.robotClawConfigList[i].PushAcc;
                                                                   
                this.dgv_data.Rows[index].Cells[4].Value = Program.robotClawConfigList[i].OrientationRange;
                                                                   
                this.dgv_data.Rows[index].Cells[5].Value = Program.robotClawConfigList[i].PushForce;
                                                                   
                this.dgv_data.Rows[index].Cells[6].Value = Program.robotClawConfigList[i].ForceDistance;
                                                                   
                this.dgv_data.Rows[index].Cells[7].Value = Program.robotClawConfigList[i].TimeRange;
            }

        }

        #endregion


        #region 获取dgv的数据

        /// <summary>
        /// 获取dgv的数据
        /// </summary>
        public void Get_dgv()
        {
            for (int i = 0; i < this.dgv_data.Rows.Count - 1; i++)
            {

                Program.robotClawConfigList[i].PointName = this.dgv_data.Rows[i].Cells[0].Value.ToString();
                        
                Program.robotClawConfigList[i].PushDistance = Convert.ToInt32(this.dgv_data.Rows[i].Cells[1].Value);
                        
                Program.robotClawConfigList[i].PushVM = Convert.ToInt32(this.dgv_data.Rows[i].Cells[2].Value);
                        
                Program.robotClawConfigList[i].PushAcc = Convert.ToInt32(this.dgv_data.Rows[i].Cells[3].Value);
                        
                Program.robotClawConfigList[i].OrientationRange = Convert.ToInt32(this.dgv_data.Rows[i].Cells[4].Value);
                        
                Program.robotClawConfigList[i].PushForce = Convert.ToInt32(this.dgv_data.Rows[i].Cells[5].Value);
                        
                Program.robotClawConfigList[i].ForceDistance = Convert.ToInt32(this.dgv_data.Rows[i].Cells[6].Value);
                        
                Program.robotClawConfigList[i].TimeRange = Convert.ToInt32(this.dgv_data.Rows[i].Cells[7].Value);

            }

        }

        #endregion

        private void btn_Move_Click(object sender, EventArgs e)
        {
            /***选中单元格  选中整行，列为零***/
            int colIndex = dgv_data.CurrentCell.ColumnIndex;
            int rowindex = dgv_data.CurrentCell.RowIndex;

            if (rowindex < this.dgv_data.Rows.Count)
            {
                /*
                指令类型
                位置
                速度
                加速度
                减速度
                定位区间
                *
                *
                *
                下一步指令序号
                */

                isWrite = false;
                isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5064, new int[]
                {
                    3,
                    Convert.ToInt32(this.dgv_data.Rows[rowindex].Cells[1].Value),
                    Convert.ToInt32(this.dgv_data.Rows[rowindex].Cells[2].Value),
                    Convert.ToInt32(this.dgv_data.Rows[rowindex].Cells[3].Value),
                    Convert.ToInt32(this.dgv_data.Rows[rowindex].Cells[3].Value),
                    Convert.ToInt32(this.dgv_data.Rows[rowindex].Cells[4].Value),
                });


                if (isWrite)
                {
                    isWrite = false;
                    isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5064 + 2, new int[] { -1 });
                }

                if (isWrite)
                {
                    isWrite = false;
                    isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000 + 4, false);
                }
                if (isWrite)
                {
                    isWrite = false;
                    isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000 + 4, true);
                }

            }

        }

        private void btn_GetPos_Click(object sender, EventArgs e)
        {
            /***选中单元格  选中整行，列为零***/
            int colIndex = dgv_data.CurrentCell.ColumnIndex;
            int rowindex = dgv_data.CurrentCell.RowIndex;

            if (rowindex < this.dgv_data.Rows.Count)
            {

                // 读取位置
                result = null;
                result = Rtu_robotClaw.ReadInputRegInt(Program.robotClawConfig.DevAdd, 0, 1);
                if (result != null && result.Length == 1)
                {

                    this.dgv_data.Rows[rowindex].Cells[1].Value = result[0].ToString();   //单位um
                    this.dgv_data.Rows[rowindex].Cells[6].Value = result[0].ToString();   //单位um
                    MessageBox.Show("提取点位成功");
                }
                else
                {
                    MessageBox.Show("提取点位失败");
                }

            }
            else
            {
                MessageBox.Show("提取点位失败");
            }
        }

        private void cmb_DataFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            Rtu_robotClaw.DataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), cmb_DataFormat.Text);
        }


        //绝对
        private void button1_Click(object sender, EventArgs e)
        {
            if (WaitRobotClawAbsMove(Program.robotClawConfigList[0], 3000))
            {
                MessageBox.Show("绝对运动成功");
            }
            else
            {
                MessageBox.Show("绝对运动失败");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (WaitRobotClawForceMove(Program.robotClawConfigList[1], 5000))
            {
                MessageBox.Show("推压运动成功");
            }
            else
            {
                MessageBox.Show("推压运动失败");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (WaitRobotClawHome(3000))
            {
                MessageBox.Show("复位成功");
            }
            else
            {
                MessageBox.Show("复位失败");
            }

        }


        #region 等待电动夹爪绝对运动

        /// <summary>
        /// 等待机器人电动夹爪绝对运动
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="timeOut">超时时间 单位：ms</param>
        /// <returns>运动成功：true  运动馆失败 false</returns>
        public bool WaitRobotClawAbsMove(GripPawlConfig parameter, double timeOut)
        {
            bool isWrite = false;
            int[] result = null;
            bool[] resultBool = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();

            /* 
                指令类型
                位置
                速度
                加速度
                减速度
                定位区间
                *
                *
                *
                下一步指令序号
                */

            isWrite = false;
            isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5032, new int[]
                {
                    3,
                    parameter.PushDistance,
                    parameter.PushVM,
                    parameter.PushAcc,
                    parameter.PushAcc,
                   parameter.OrientationRange,
                });


            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5032 + 2, new int[] { -1 });
            }

            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000 + 2, false);
            }
            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000 + 2, true);
            }

            while (true)
            {
                if (stopwatch.ElapsedMilliseconds >= timeOut || !isWrite)
                {
                    return false;
                }
                else
                {
                    //第一种方式：控制器已到达点位n
                    //resultBool = null;
                    //resultBool = modbusRtu.ReadOutputStatusBool(Program.nodbusRtuConfig.DevAdd, 1506 + 2, 1);
                    //if (resultBool != null && resultBool.Length == 1 && resultBool[0] == true)
                    //{
                    //    return true;
                    //}


                    //第二种方式：位置到位
                    result = null;
                    result = Rtu_robotClaw.ReadInputRegInt(Program.robotClawConfig.DevAdd, 0, 1);
                    if (result != null && result.Length == 1 && (Math.Abs(parameter.PushDistance - result[0])) / 1000 < 0.5)
                    {
                        return true;
                    }

                }
            }
        }

        #endregion

        #region 等待电动夹爪推压运动

        /// <summary>
        /// 等待机器人电动夹爪推压运动
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="timeOut">超时时间 单位：ms</param>
        /// <returns>运动成功：true 运动失败：false</returns>
        public bool WaitRobotClawForceMove(GripPawlConfig parameter, double timeOut)
        {
            bool isWrite = false;
            int[] result = null;
            bool[] resultBool = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            /*
                指令类型
                位置
                速度
                加速度
                减速度
                定位区间
                推压力
                推压距离
                延时时间
                下一步指令序号
                */


            isWrite = false;
            isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5048, new int[]
                {
                    4,
                    parameter.PushDistance,
                    parameter.PushVM,
                    parameter.PushAcc,
                    parameter.PushAcc,
                    parameter.OrientationRange,
                    parameter.PushForce,
                    parameter.ForceDistance,
                    parameter.TimeRange,
                });


            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.PreSetMultiReg(Program.robotClawConfig.DevAdd, 5048 + 2, new int[] { -1 });
            }

            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000 + 3, false);
            }
            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 1000 + 3, true);
            }


            while (true)
            {
                if (stopwatch.ElapsedMilliseconds >= timeOut || !isWrite)
                {
                    return false;
                }
                else
                {
                    //第一种方式：控制器已到达点位n
                    //bool[] result = modbusRtu.ReadOutputStatusBool(Program.nodbusRtuConfig.DevAdd, 1506 + 3, 1);
                    //if (result != null && result.Length == 1 && result[0] == true)
                    //{
                    //    return true;
                    //}


                    //第二种方式：位置到位
                    //result = null;
                    //result = modbusRtu.ReadInputRegInt(Program.nodbusRtuConfig.DevAdd, 0, 1);
                    //if (result != null && result.Length == 1 && (Math.Abs(parameter.PushDistance - result[0])) / 1000 < 0.5)
                    //{
                    //    return true;
                    //}


                    //第三种方式：推力到位
                    //到达信号 ON+运动中 ON，为推到 / 夹到工件。
                    resultBool = null;
                    resultBool = Rtu_robotClaw.ReadOutputStatusBool(Program.robotClawConfig.DevAdd, 1000 + 3, 1);
                    if (resultBool != null && resultBool.Length == 1 && resultBool[0] == true)
                    {
                        resultBool = null;
                        resultBool = Rtu_robotClaw.ReadOutputStatusBool(Program.robotClawConfig.DevAdd, 8, 1);
                        if (resultBool != null && resultBool.Length == 1 && resultBool[0] == true)
                        {
                            return true;
                        }
                    }

                }
            }

        }

        #endregion

        #region 等待复位
        /// <summary>
        /// 等待机器人电动夹爪复位
        /// </summary>
        /// <param name="timeOut">超时时间 单位：ms</param>
        /// <returns>复位成功：true 复位失败：false</returns>
        public bool WaitRobotClawHome(double timeOut)
        {
            bool isWrite;
            bool[] resultBool = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            isWrite = false;
            isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 17, false);
            if (isWrite)
            {
                isWrite = false;
                isWrite = Rtu_robotClaw.ForceCoil(Program.robotClawConfig.DevAdd, 17, true);
            }
            while (true)
            {
                if (stopwatch.ElapsedMilliseconds >= timeOut || !isWrite)
                {
                    return false;
                }
                else
                {
                    resultBool = null;
                    resultBool = Rtu_robotClaw.ReadOutputStatusBool(Program.robotClawConfig.DevAdd, 1037, 1);
                    if (resultBool != null && resultBool.Length == 1 && resultBool[0] == true)
                    {
                        return true;
                    }
                }
            }
        }


        #endregion


    }
}
