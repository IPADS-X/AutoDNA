using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class InovanceRobotForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        /***提示语***/
        private ToolTip toolTip1 = new ToolTip();

        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            /***从Xml文件重新加载对应的机器人点位***/
            string nameStr = string.Empty;
            string str = cmb_RobotType.Text.Trim();
            foreach(var va in InovanceRobotConfig.Instance.InovanceRobotDic)
            {
                if(str == va.Value.RobotName_CH || str == va.Value.RobotName_EN || str == va.Value.RobotName_VN)
                {
                    nameStr = va.Value.RobotName_CH;
                    break;
                }
            }
            string pointTpStr = string.Empty;
            string str1 = cmb_PointType.Text.Trim();
            foreach (var va in InovanceRobotConfig.Instance.InovanceRobotDic[nameStr].RobotPointDic)
            {
                if (str1 == va.Key)
                {
                    pointTpStr = va.Key;
                    break;
                }
            }
            InovanceRobotConfig.Instance.UpdateRobotParameToGrid(dataGridView1, nameStr, pointTpStr);
        }

        public InovanceRobotForm()
        {
            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        private void CommunicationForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            /***加载机器人种类***/
            cmb_RobotType.Items.Clear();
            List<string> listCH = new List<string>();
            List<string> listEN = new List<string>();
            List<string> listVN = new List<string>();
            listCH.Clear();
            listEN.Clear();
            listVN.Clear();
            listCH = InovanceRobotConfig.Instance.InovanceRobotDic.Keys.ToList();
            for (int i=0;i<listCH.Count;i++)
            {
                listEN.Add(InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(i).Value.RobotName_EN);
                listVN.Add(InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(i).Value.RobotName_VN);
            }
            if(LanguageConfig.Instance.Language=="CH")
            {
                cmb_RobotType.Items.AddRange(listCH.ToArray());
            }
            else if(LanguageConfig.Instance.Language=="EN")
            {
                cmb_RobotType.Items.AddRange(listEN.ToArray());
            }
            else
            {
                cmb_RobotType.Items.AddRange(listVN.ToArray());
            }        
            cmb_RobotType.SelectedIndex = 0;
            /***加载其它选项***/
            cmb_RobotCoord.SelectedIndex = 0;
            cmb_TechMode.SelectedIndex = 0;
            cmb_Vel.SelectedIndex = 1;
            cmb_SpeedGrade.SelectedIndex = 1;
            cmb_Distance.SelectedIndex = 2;
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += RobotForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
            /***开启定时器***/
            timer1.Enabled = true;
            timer1.Interval = 200;
            timer1.Start();
        }

        private void RobotForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变Panel容器内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_GetPos, "坐标提取");
                toolTip1.SetToolTip(btn_Move, "点位移动");
                toolTip1.SetToolTip(btn_Save, "点位保存");
                toolTip1.SetToolTip(btn_Power, "使能操作");
                toolTip1.SetToolTip(btn_ResetErr, "异常清除");
                toolTip1.SetToolTip(btn_Emg, "按下急停");
                toolTip1.SetToolTip(btn_ResetEmg, "解除急停");
                toolTip1.SetToolTip(btn_Home, "机器人回零");
                toolTip1.SetToolTip(btn_DsMode, "数据流模式操作");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_GetPos, "Coordinate extraction");
                toolTip1.SetToolTip(btn_Move, "Point move");
                toolTip1.SetToolTip(btn_Save, "Point save");
                toolTip1.SetToolTip(btn_Power, "Enable operation");
                toolTip1.SetToolTip(btn_ResetErr, "Exception clear");
                toolTip1.SetToolTip(btn_Emg, "Press emergency stop");
                toolTip1.SetToolTip(btn_ResetEmg, "Cancel emergency stop");
                toolTip1.SetToolTip(btn_Home, "Robot return to zero");
                toolTip1.SetToolTip(btn_DsMode, "Data flow mode operation");
            }
            else
            {
                toolTip1.SetToolTip(btn_GetPos, "Phối hợp khai thác");
                toolTip1.SetToolTip(btn_Move, "Di chuyển điểm");
                toolTip1.SetToolTip(btn_Save, "Lưu điểm");
                toolTip1.SetToolTip(btn_Power, "Kích hoạt hoạt động");
                toolTip1.SetToolTip(btn_ResetErr, "Ngoại lệ rõ ràng");
                toolTip1.SetToolTip(btn_Emg, "Nhấn dừng khẩn cấp");
                toolTip1.SetToolTip(btn_ResetEmg, "Hủy dừng khẩn cấp");
                toolTip1.SetToolTip(btn_Home, "Robot trở về số không");
                toolTip1.SetToolTip(btn_DsMode, "Chế độ luồng dữ liệu hoạt động");
            }
            /***加载机器人种类***/
            int ii = cmb_RobotType.SelectedIndex;
            cmb_RobotType.Items.Clear();
            List<string> listCH = new List<string>();
            List<string> listEN = new List<string>();
            List<string> listVN = new List<string>();
            listCH.Clear();
            listEN.Clear();
            listVN.Clear();
            listCH = InovanceRobotConfig.Instance.InovanceRobotDic.Keys.ToList();
            for (int i = 0; i < listCH.Count; i++)
            {
                listEN.Add(InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(i).Value.RobotName_EN);
                listVN.Add(InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(i).Value.RobotName_VN);
            }
            if (LanguageConfig.Instance.Language == "CH")
            {
                cmb_RobotType.Items.AddRange(listCH.ToArray());
            }
            else if (LanguageConfig.Instance.Language == "EN")
            {
                cmb_RobotType.Items.AddRange(listEN.ToArray());
            }
            else
            {
                cmb_RobotType.Items.AddRange(listVN.ToArray());
            }
            cmb_RobotType.SelectedIndex = ii;
        }

        private void btn_Power_Click(object sender, EventArgs e)
        {
            if(!InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.
                RobotStatus.RobotIsEnable)
            {
                InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotPowerHandle(true);
            }
            else
            {
                InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotPowerHandle(false);
            }
        }

        private void btn_ResetErr_Click(object sender, EventArgs e)
        {
            if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsError)
            {
                InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotResetErrHandle();
            }
        }

        private void btn_Emg_Click(object sender, EventArgs e)
        {
            InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotEmgHnadle(true);
        }

        private void btn_ResetEmg_Click(object sender, EventArgs e)
        {
            if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsEmg)
            {
                InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotEmgHnadle(false);
            }
        }

        private void btn_DsMode_Click(object sender, EventArgs e)
        {
            /***运动指令需要在开启了数据流模式之后才可以启动，当机器人在运动的过程中关闭了数据流模式相当于机器人的运动内暂停***/
            if (!InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsInDSMode)
            {
                InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotSetDSMode(true);
            }
            else
            {
                InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotSetDSMode(false);
            }
        }

        private void btn_Home_Click(object sender, EventArgs e)
        {
            if (!InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsInDSMode)
            {
                InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotSetDSMode(true);
            }
            InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotGOHome(0);
        }

        private void btn_GetPos_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                this.dataGridView1[1, index].Value = Slab_J1Pos.Text.Trim();
                this.dataGridView1[2, index].Value = Slab_J2Pos.Text.Trim();
                this.dataGridView1[3, index].Value = Slab_J3Pos.Text.Trim();
                this.dataGridView1[4, index].Value = Slab_J4Pos.Text.Trim();
                this.dataGridView1[5, index].Value = Slab_J5Pos.Text.Trim();
                this.dataGridView1[6, index].Value = Slab_J6Pos.Text.Trim();
                this.dataGridView1[7, index].Value = Slab_J1Arm.Text.Trim();
                this.dataGridView1[8, index].Value = Slab_J2Arm.Text.Trim();
                this.dataGridView1[9, index].Value = Slab_J3Arm.Text.Trim();
                this.dataGridView1[10, index].Value = Slab_J4Arm.Text.Trim();
                switch (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.
                    RobotStatus.RobotCoord)
                {
                    case Coord.关节坐标系:
                        this.dataGridView1[11, index].Value = "Joint";
                        break;
                    case Coord.基础坐标系:
                        this.dataGridView1[11, index].Value = "Basis";
                        break;
                    case Coord.工具坐标系:
                        this.dataGridView1[11, index].Value = "Tool";
                        break;
                    case Coord.用户坐标系:
                        this.dataGridView1[11, index].Value = "User";
                        break;
                }
                this.dataGridView1[12, index].Value = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).
                    Value.RobotStatus.RobotCurPos.toolNo;
                this.dataGridView1[13, index].Value = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).
                    Value.RobotStatus.RobotCurPos.userNo;
                if (LanguageConfig.Instance.Language == "CH")
                {
                    MessageBox.Show("机器人点位信息提取成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    MessageBox.Show("The robot point information was extracted successfully！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Thông tin điểm rô bốt đã được trích xuất thành công！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                if(LanguageConfig.Instance.Language=="CH")
                {
                    MessageBox.Show("机器人点位信息提取失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(LanguageConfig.Instance.Language=="EN")
                {
                    MessageBox.Show("Robot point information extraction failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Trích xuất thông tin điểm rô bốt không thành công！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_Move_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                ROBOT_POS runpos = new ROBOT_POS();
                runpos.pos = new double[6];
                runpos.armType = new int[4];
                runpos.pos[0] = double.Parse(dataGridView1[1, index].Value.ToString());
                runpos.pos[1] = double.Parse(dataGridView1[2, index].Value.ToString());
                runpos.pos[2] = double.Parse(dataGridView1[3, index].Value.ToString());
                runpos.pos[3] = double.Parse(dataGridView1[4, index].Value.ToString());
                runpos.pos[4] = double.Parse(dataGridView1[5, index].Value.ToString());
                runpos.pos[5] = double.Parse(dataGridView1[6, index].Value.ToString());
                runpos.armType[0] = int.Parse(dataGridView1[7, index].Value.ToString());
                runpos.armType[1] = int.Parse(dataGridView1[8, index].Value.ToString());
                runpos.armType[2] = int.Parse(dataGridView1[9, index].Value.ToString());
                runpos.armType[3] = int.Parse(dataGridView1[10, index].Value.ToString());
                switch (dataGridView1[11, index].Value.ToString())
                {
                    case "Joint":
                        runpos.coord = 1;
                        break;
                    case "Basis":
                        runpos.coord = 2;
                        break;
                    case "Tool":
                        runpos.coord = 3;
                        break;
                    case "User":
                        runpos.coord = 4;
                        break;
                }
                runpos.toolNo = int.Parse(dataGridView1[12, index].Value.ToString());
                runpos.userNo = int.Parse(dataGridView1[13, index].Value.ToString());
                if(!InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.CheckRobotBeforeRun())
                {
                    if(LanguageConfig.Instance.Language=="CH")
                    {
                        MessageBox.Show("机器人未准备好，点位移动失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if(LanguageConfig.Instance.Language=="EN")
                    {
                        MessageBox.Show("The robot is not ready and the point movement failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Robot chưa sẵn sàng và chuyển động điểm không thành công！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotAxisCount == 4)
                {
                    /***四轴机器人执行Jump运动***/
                    if (!InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value
                        .RobotJumpMove(runpos, 20, -100, 20, int.Parse(cmb_Vel.Text.Trim())))
                    {
                        if (LanguageConfig.Instance.Language == "CH")
                        {
                            MessageBox.Show("机器人运动失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if(LanguageConfig.Instance.Language=="EN")
                        {
                            MessageBox.Show("Robot movement failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Chuyển động của robot không thành công！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }
                }
                else
                {
                    /***六轴机器人执行关节插补运动***/
                    if (!InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value
                        .RobotJMove(runpos, int.Parse(cmb_Vel.Text.Trim())))
                    {
                        if (LanguageConfig.Instance.Language == "CH")
                        {
                            MessageBox.Show("机器人运动失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (LanguageConfig.Instance.Language == "EN")
                        {
                            MessageBox.Show("Robot movement failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Chuyển động của robot không thành công！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }
                    //InovanceRobotConfig.Instance.InovanceRobotDic["搬运机器人"].WaitRobotJMove("Point_ok", "点位3", 15, 20);
                }
            }
            catch
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    MessageBox.Show("机器人运动失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    MessageBox.Show("Robot movement failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Chuyển động của robot không thành công！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                string nameStr = string.Empty;
                string str = cmb_RobotType.Text.Trim();
                foreach (var va in InovanceRobotConfig.Instance.InovanceRobotDic)
                {
                    if (str == va.Value.RobotName_CH || str == va.Value.RobotName_EN || str == va.Value.RobotName_VN)
                    {
                        nameStr = va.Value.RobotName_CH;
                        break;
                    }
                }
                string pointTpStr = string.Empty;
                string str1 = cmb_PointType.Text.Trim();
                foreach (var va in InovanceRobotConfig.Instance.InovanceRobotDic[nameStr].RobotPointDic)
                {
                    if (str1 == va.Key)
                    {
                        pointTpStr = va.Key;
                        break;
                    }
                }

                #region 测试代码
                //string pointNameStr = string.Empty;
                //int index = dataGridView1.CurrentRow.Index;
                //string str2 = dataGridView1[0, index].Value.ToString();
                //foreach (var va in InovanceRobotConfig.Instance.InovanceRobotDic[nameStr].RobotPointDic[pointTpStr])
                //{
                //    if (str2 == va.Value.PointName_CH || str2 == va.Value.PointName_EN || str2 == va.Value.PointName_VN)
                //    {
                //        pointNameStr = va.Value.PointName_CH;
                //        break;
                //    }
                //}
                //ROBOT_POS runpos = new ROBOT_POS();
                //#region 机器人坐标赋值
                //runpos.pos = new double[6];
                //runpos.armType = new int[4];
                //runpos.pos[0] = double.Parse(dataGridView1[1, index].Value.ToString());
                //runpos.pos[1] = double.Parse(dataGridView1[2, index].Value.ToString());
                //runpos.pos[2] = double.Parse(dataGridView1[3, index].Value.ToString());
                //runpos.pos[3] = double.Parse(dataGridView1[4, index].Value.ToString());
                //runpos.pos[4] = double.Parse(dataGridView1[5, index].Value.ToString());
                //runpos.pos[5] = double.Parse(dataGridView1[6, index].Value.ToString());
                //runpos.armType[0] = int.Parse(dataGridView1[7, index].Value.ToString());
                //runpos.armType[1] = int.Parse(dataGridView1[8, index].Value.ToString());
                //runpos.armType[2] = int.Parse(dataGridView1[9, index].Value.ToString());
                //runpos.armType[3] = int.Parse(dataGridView1[10, index].Value.ToString());
                //switch (dataGridView1[11, index].Value.ToString())
                //{
                //    case "Joint":
                //        runpos.coord = 1;
                //        break;
                //    case "Basis":
                //        runpos.coord = 2;
                //        break;
                //    case "Tool":
                //        runpos.coord = 3;
                //        break;
                //    case "User":
                //        runpos.coord = 4;
                //        break;
                //}
                //runpos.toolNo = int.Parse(dataGridView1[12, index].Value.ToString());
                //runpos.userNo = int.Parse(dataGridView1[13, index].Value.ToString());
                //#endregion
                //InovanceRobotConfig.Instance.UpdatePointParamToRobotXML(nameStr, pointTpStr, pointNameStr, runpos);
                #endregion

                /***保存机器人的点位***/
                if (InovanceRobotConfig.Instance.UpdateGridToRobotXML(dataGridView1, nameStr, pointTpStr))
                {
                    if (LanguageConfig.Instance.Language == "CH")
                    {
                        MessageBox.Show("机器人点位保存成功！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (LanguageConfig.Instance.Language == "EN")
                    {
                        MessageBox.Show("The robot point is saved successfully！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Điểm rô bốt đã được lưu thành công！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (LanguageConfig.Instance.Language == "CH")
                    {
                        MessageBox.Show("机器人点位保存失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (LanguageConfig.Instance.Language == "EN")
                    {
                        MessageBox.Show("Robot point save failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Không lưu được điểm rô bốt！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    MessageBox.Show("机器人点位保存失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    MessageBox.Show("Robot point save failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Không lưu được điểm rô bốt！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmb_RobotType_SelectedIndexChanged(object sender, EventArgs e)
        {
            /***加载点位分类***/
            int ii = cmb_PointType.SelectedIndex;
            cmb_PointType.Items.Clear();
            List<string> listPointTp = new List<string>();
            listPointTp.Clear();
            string nameStr = string.Empty;
            string str = cmb_RobotType.Text.Trim();
            foreach (var va in InovanceRobotConfig.Instance.InovanceRobotDic)
            {
                if (str == va.Value.RobotName_CH || str == va.Value.RobotName_EN || str == va.Value.RobotName_VN)
                {
                    nameStr = va.Value.RobotName_CH;
                    break;
                }
            }
            listPointTp = InovanceRobotConfig.Instance.InovanceRobotDic[nameStr].RobotPointDic.Keys.ToList();
            cmb_PointType.Items.AddRange(listPointTp.ToArray());
            if (ii != -1 && ii < cmb_PointType.Items.Count)
            {
                cmb_PointType.SelectedIndex = ii;
            }
            else
            {
                cmb_PointType.SelectedIndex = 0;
            }
        }

        private void cmb_PointType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            /***从Xml文件重新加载对应的机器人点位***/
            string nameStr = string.Empty;
            string str = cmb_RobotType.Text.Trim();
            foreach (var va in InovanceRobotConfig.Instance.InovanceRobotDic)
            {
                if (str == va.Value.RobotName_CH || str == va.Value.RobotName_EN || str == va.Value.RobotName_VN)
                {
                    nameStr = va.Value.RobotName_CH;
                    break;
                }
            }
            string pointTpStr = string.Empty;
            string str1 = cmb_PointType.Text.Trim();
            foreach (var va in InovanceRobotConfig.Instance.InovanceRobotDic[nameStr].RobotPointDic)
            {
                if (str1 == va.Key)
                {
                    pointTpStr = va.Key;
                    break;
                }
            }
            InovanceRobotConfig.Instance.UpdateRobotParameToGrid(dataGridView1, nameStr, pointTpStr);
            /***判断是四轴还是六轴***/
            if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotAxisCount == 4)
            {
                btn_VMel.Enabled = false;
                btn_VMel.Visible = false;
                btn_VPel.Enabled = false;
                btn_VPel.Visible = false;
                btn_WMel.Enabled = false;
                btn_WMel.Visible = false;
                btn_WPel.Enabled = false;
                btn_WPel.Visible = false;
            }
            else
            {
                btn_VMel.Enabled = true;
                btn_VMel.Visible = true;
                btn_VPel.Enabled = true;
                btn_VPel.Visible = true;
                btn_WMel.Enabled = true;
                btn_WMel.Visible = true;
                btn_WPel.Enabled = true;
                btn_WPel.Visible = true;
            }
        }

        private void cmb_RobotCoord_SelectedIndexChanged(object sender, EventArgs e)
        {
            InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.
                RobotSetCoord((Coord)cmb_RobotCoord.SelectedIndex + 1);
        }

        private void cmb_TechMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.
                RobotSetInchMode((TechMode)cmb_TechMode.SelectedIndex);
        }

        private void cmb_SpeedGrade_SelectedIndexChanged(object sender, EventArgs e)
        {
            /***设置机器人速度等级，可改变Jog运动和寸动的运行速度***/
            InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.
                RobotSetVel(int.Parse(cmb_SpeedGrade.Text.Trim()));
        }

        private void cmb_Vel_SelectedIndexChanged(object sender, EventArgs e)
        {
            /***机器人速度等级默认是100%，通过机器人运动指令的速度参数改变运行速度***/
            return;
        }

        private void btn_XPel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(1, 1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_XMel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(1, -1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_XPel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(1, 1);
        }

        private void btn_XPel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(1, 0);
        }

        private void btn_XMel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(1, -1);
        }

        private void btn_XMel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(1, 0);
        }

        private void btn_YPel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(2, 1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_YMel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(2, -1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_YPel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(2, 1);
        }

        private void btn_YPel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(2, 0);
        }

        private void btn_YMel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(2, -1);
        }

        private void btn_YMel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(2, 0);
        }

        private void btn_ZPel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(3, 1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_ZMel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(3, -1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_ZPel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(3, 1);
        }

        private void btn_ZPel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(3, 0);
        }

        private void btn_ZMel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(3, -1);
        }

        private void btn_ZMel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(3, 0);
        }

        private void btn_UPel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(4, 1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_UMel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(4, -1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_UPel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(4, 1);
        }

        private void btn_UPel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(4, 0);
        }

        private void btn_UMel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(4, -1);
        }

        private void btn_UMel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(4, 0);
        }

        private void btn_VPel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(5, 1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_VMel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(5, -1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_VPel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(5, 1);
        }

        private void btn_VPel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(5, 0);
        }

        private void btn_VMel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(5, -1);
        }

        private void btn_VMel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(5, 0);
        }

        private void btn_WPel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(6, 1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_WMel_Click(object sender, EventArgs e)
        {
            RobotRleativeMove(6, -1, float.Parse(cmb_Distance.Text.Trim()));
        }

        private void btn_WPel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(6, 1);
        }

        private void btn_WPel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(6, 0);
        }

        private void btn_WMel_MouseDown(object sender, MouseEventArgs e)
        {
            RobotContinueMove(6, -1);
        }

        private void btn_WMel_MouseUp(object sender, MouseEventArgs e)
        {
            RobotContinueMove(6, 0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Slab_J1Pos.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.pos[0].ToString("f3");
                Slab_J2Pos.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.pos[1].ToString("f3");
                Slab_J3Pos.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.pos[2].ToString("f3");
                Slab_J4Pos.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.pos[3].ToString("f3");
                Slab_J5Pos.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.pos[4].ToString("f3");
                Slab_J6Pos.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.pos[5].ToString("f3");
                Slab_J1Arm.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.armType[0].ToString();
                Slab_J2Arm.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.armType[1].ToString();
                Slab_J3Arm.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.armType[2].ToString();
                Slab_J4Arm.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.armType[3].ToString();
                /***有无报警***/
                if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsEmg)
                {
                    Slab_EmgInfo.Text = "EMG";
                    Slab_EmgInfo.BackColor = Color.Red;
                }
                else
                {
                    Slab_EmgInfo.Text = "NoEmg";
                    Slab_EmgInfo.BackColor = Color.Lime;
                }
                /***有无报错***/
                if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsError)
                {
                    Slab_ErrorInfo.Text = "Error";
                    Slab_ErrorInfo.BackColor = Color.Red;
                }
                else
                {
                    Slab_ErrorInfo.Text = "NoError";
                    Slab_ErrorInfo.BackColor = Color.Lime;
                }
                /***有无上电***/
                if(InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsEnable)
                {
                    btn_Power.BackgroundImage = Properties.Resources.上电;
                }
                else
                {
                    btn_Power.BackgroundImage = Properties.Resources.未上电;
                }
                /***数据流有无开启***/
                if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsInDSMode)
                {
                    btn_DsMode.BackgroundImage = Properties.Resources.数据流开启;
                }
                else
                {
                    btn_DsMode.BackgroundImage = Properties.Resources.数据流关闭;
                }
                /***显示当前速度***/
                Slab_Vel.Text = InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.
                    RobotStatus.RobotCurVel.ToString();
                /***显示示教模式***/
                if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotInchMode ==
                    TechMode.连续示教)
                {
                    Slab_TechInfo.Text = "Continuous";
                }
                else
                {
                    Slab_TechInfo.Text = "Lnching";
                }
                /***显示坐标系***/
                if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCoord ==
                    Coord.关节坐标系)
                {
                    Slab_CoordInfo.Text = "Joint";
                }
                else if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCoord ==
                    Coord.基础坐标系)
                {
                    Slab_CoordInfo.Text = "Basis";
                }
                else if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCoord ==
                    Coord.工具坐标系)
                {
                    Slab_CoordInfo.Text = "Tool";
                }
                else
                {
                    Slab_CoordInfo.Text = "User";
                }
            }
            catch {; }
        }

        /// <summary>
        /// 机器人寸动示教
        /// </summary>
        /// <param name="axisindex">轴号(1-6)</param>
        /// <param name="cmd">示教命令0,停止|1,正向示教|-1,反向示教</param>
        /// <param name="sp">寸动的步距</param>
        private void RobotRleativeMove(int axisindex, int cmd, float sp)
        {
            if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value
                .RobotStatus.RobotInchMode == TechMode.连续示教)
            {
                return;
            }
            if (cmb_Distance.Text.Trim() == "")
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    MessageBox.Show("请设定寸动步距！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if(LanguageConfig.Instance.Language=="EN")
                {
                    MessageBox.Show("Please set the inching step！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Vui lòng đặt bước nhích！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            if (!InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value
                .RobotRelMove(axisindex, cmd, sp))
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    MessageBox.Show("机器人" + axisindex.ToString() + "轴寸动失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(LanguageConfig.Instance.Language == "EN")
                {
                    MessageBox.Show("Robot" + axisindex.ToString() + "Axis inching failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Robot" + axisindex.ToString() + "Trục nhích không thành công！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 机器人连续示教
        /// </summary>
        /// <param name="axisindex"></param>
        /// <param name="cmd"></param>
        private void RobotContinueMove(int axisindex, int cmd)
        {
            if (InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value
                .RobotStatus.RobotInchMode == TechMode.寸动示教)
            {
                return;
            }
            InovanceRobotConfig.Instance.InovanceRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(axisindex, cmd);
        }

    }
}
