using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using THLCommunicateLib;

namespace CYStandardProcedure
{
    public partial class ToshibalRobotForm : Form
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
            foreach (var va in TsRemoteRobotConfig.Instance.TsRemoteRobotDic)
            {
                if (str == va.Value.RobotName_CH || str == va.Value.RobotName_EN || str == va.Value.RobotName_VN)
                {
                    nameStr = va.Value.RobotName_CH;
                    break;
                }
            }
            string pointTpStr = string.Empty;
            string str1 = cmb_PointType.Text.Trim();
            foreach (var va in TsRemoteRobotConfig.Instance.TsRemoteRobotDic[nameStr].RobotPointDic)
            {
                if (str1 == va.Key)
                {
                    pointTpStr = va.Key;
                    break;
                }
            }
            TsRemoteRobotConfig.Instance.UpdateRobotParameToGrid(dataGridView1, nameStr, pointTpStr);
        }

        public ToshibalRobotForm()
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

        private void ToshibalRobotForm_Load(object sender, EventArgs e)
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
            listCH = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.Keys.ToList();
            for (int i = 0; i < listCH.Count; i++)
            {
                listEN.Add(TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(i).Value.RobotName_EN);
                listVN.Add(TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(i).Value.RobotName_VN);
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
            cmb_RobotType.SelectedIndex = 0;

            /***加载其它选项***/
            cmb_RobotCoord.SelectedIndex = 3;
            cmb_MoveMode.SelectedIndex = 0;
            cmb_MoveVel.SelectedIndex = 2;
            cmb_MoveUnit.SelectedIndex = 3;
            cmb_JogVel.SelectedIndex = 0;
            cmb_MoveCurve.SelectedIndex = 0;
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
                toolTip1.SetToolTip(btn_Break, "暂停运动");
                toolTip1.SetToolTip(btn_Restart, "机器人重启");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_GetPos, "Coordinate extraction");
                toolTip1.SetToolTip(btn_Move, "Point move");
                toolTip1.SetToolTip(btn_Save, "Point save");
                toolTip1.SetToolTip(btn_Power, "Enable operation");
                toolTip1.SetToolTip(btn_ResetErr, "Exception clear");
                toolTip1.SetToolTip(btn_Break, "Program break");
                toolTip1.SetToolTip(btn_Restart, "Robot return to zero");
            }
            else
            {
                toolTip1.SetToolTip(btn_GetPos, "Phối hợp khai thác");
                toolTip1.SetToolTip(btn_Move, "Di chuyển điểm");
                toolTip1.SetToolTip(btn_Save, "Lưu điểm");
                toolTip1.SetToolTip(btn_Power, "Kích hoạt hoạt động");
                toolTip1.SetToolTip(btn_ResetErr, "Ngoại lệ rõ ràng");
                toolTip1.SetToolTip(btn_Break, "Nhấn dừng khẩn cấp");
                toolTip1.SetToolTip(btn_Restart, "Robot trở về số không");
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
            listCH = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.Keys.ToList();
            for (int i = 0; i < listCH.Count; i++)
            {
                listEN.Add(TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(i).Value.RobotName_EN);
                listVN.Add(TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(i).Value.RobotName_VN);
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
            if (!TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.
                RobotStatus.RobotIsEnable)
            {
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotPowerHandle(true);
            }
            else
            {
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotPowerHandle(false);
            }
        }

        private void btn_ResetErr_Click(object sender, EventArgs e)
        {
            if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsError)
            {
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotResetErrHandle();
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            if (!TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotMontionDone)
            {
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.BreakMove();
            }
        }

        private void btn_Restart_Click(object sender, EventArgs e)
        {
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotRestart();
        }

        private void btn_GetPos_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                this.dataGridView1[1, index].Value = Slab_XPos.Text.Trim();
                this.dataGridView1[2, index].Value = Slab_YPos.Text.Trim();
                this.dataGridView1[3, index].Value = Slab_ZPos.Text.Trim();
                this.dataGridView1[4, index].Value = Slab_CPos.Text.Trim();
                this.dataGridView1[5, index].Value = Slab_TPos.Text.Trim();
                this.dataGridView1[6, index].Value = Slab_Config.Text.Trim();
                this.dataGridView1[7, index].Value = Slab_CoordInfo.Text.Trim();
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
                if (LanguageConfig.Instance.Language == "CH")
                {
                    MessageBox.Show("机器人点位信息提取失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (LanguageConfig.Instance.Language == "EN")
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
                PosInfo runpos = new PosInfo();
                runpos.Xpos = double.Parse(dataGridView1[1, index].Value.ToString());
                runpos.Ypos = double.Parse(dataGridView1[2, index].Value.ToString());
                runpos.Zpos = double.Parse(dataGridView1[3, index].Value.ToString());
                runpos.Cpos = double.Parse(dataGridView1[4, index].Value.ToString());
                runpos.Tpos = double.Parse(dataGridView1[5, index].Value.ToString());
                runpos.SixPos = 0;
                switch (dataGridView1[6, index].Value.ToString())
                {
                    case "FREE":
                        runpos.Pose = PosConfig.FREE;
                        break;
                    case "LEFTY":
                        runpos.Pose = PosConfig.LEFTY;
                        break;
                    case "RIGHTY":
                        runpos.Pose = PosConfig.RIGHTY;
                        break;
                }
                if (!TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.CheckRobotBeforeRun())
                {
                    if (LanguageConfig.Instance.Language == "CH")
                    {
                        MessageBox.Show("机器人未准备好，点位移动失败！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (LanguageConfig.Instance.Language == "EN")
                    {
                        MessageBox.Show("The robot is not ready and the point movement failed！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Robot chưa sẵn sàng và chuyển động điểm không thành công！", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                cmb_MoveVel_SelectedIndexChanged(null, null);//设置机器人速度
                #region 机器人运动
                if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotAxisCount == 4)
                {
                    /***四轴机器人执行曲线运动***/
                    if (cmb_MoveCurve.SelectedIndex == 0)
                    {
                        if (!TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.MovePoint(runpos))
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
                    }
                    /***四轴机器人执行直线运动***/
                    else if (cmb_MoveCurve.SelectedIndex == 1)
                    {
                        if (!TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.MoveS(runpos))
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
                    }
                    /***四轴机器人执行拱形运动***/
                    else if (cmb_MoveCurve.SelectedIndex == 2)
                    {
                        if (!TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.MoveJ(runpos, 40, 20, 20))
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
                    }
                }
                #endregion

                Thread.Sleep(10);
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
                foreach (var va in TsRemoteRobotConfig.Instance.TsRemoteRobotDic)
                {
                    if (str == va.Value.RobotName_CH || str == va.Value.RobotName_EN || str == va.Value.RobotName_VN)
                    {
                        nameStr = va.Value.RobotName_CH;
                        break;
                    }
                }
                string pointTpStr = string.Empty;
                string str1 = cmb_PointType.Text.Trim();
                foreach (var va in TsRemoteRobotConfig.Instance.TsRemoteRobotDic[nameStr].RobotPointDic)
                {
                    if (str1 == va.Key)
                    {
                        pointTpStr = va.Key;
                        break;
                    }
                }

                /***保存机器人的点位***/
                if (TsRemoteRobotConfig.Instance.UpdateGridToRobotXML(dataGridView1, nameStr, pointTpStr))
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
            foreach (var va in TsRemoteRobotConfig.Instance.TsRemoteRobotDic)
            {
                if (str == va.Value.RobotName_CH || str == va.Value.RobotName_EN || str == va.Value.RobotName_VN)
                {
                    nameStr = va.Value.RobotName_CH;
                    break;
                }
            }
            listPointTp = TsRemoteRobotConfig.Instance.TsRemoteRobotDic[nameStr].RobotPointDic.Keys.ToList();
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
            foreach (var va in TsRemoteRobotConfig.Instance.TsRemoteRobotDic)
            {
                if (str == va.Value.RobotName_CH || str == va.Value.RobotName_EN || str == va.Value.RobotName_VN)
                {
                    nameStr = va.Value.RobotName_CH;
                    break;
                }
            }
            string pointTpStr = string.Empty;
            string str1 = cmb_PointType.Text.Trim();
            foreach (var va in TsRemoteRobotConfig.Instance.TsRemoteRobotDic[nameStr].RobotPointDic)
            {
                if (str1 == va.Key)
                {
                    pointTpStr = va.Key;
                    break;
                }
            }
            TsRemoteRobotConfig.Instance.UpdateRobotParameToGrid(dataGridView1, nameStr, pointTpStr);
            /***判断T轴是否可以运动***/
            if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotAxisCount == 4)
            {
                btn_TMel.Enabled = false;
                btn_TPel.Enabled = false;
            }
            else
            {
                btn_TMel.Enabled = true;
                btn_TPel.Enabled = true;
            }
        }

        private List<int> RobotCoordIndexRecord = new List<int>();
        private void cmb_RobotCoord_SelectedIndexChanged(object sender, EventArgs e)
        {
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotSetCoord(cmb_RobotCoord.SelectedIndex);
            if (RobotCoordIndexRecord.Count > 0)
            {
                if (cmb_RobotCoord.SelectedIndex == 2)
                {
                    TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.CreateTsTransS(20, 10, 5);
                }
                else
                {
                    if (RobotCoordIndexRecord.Last() == 2)
                    {
                        TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.DeleteTsTrans();
                    }
                }
            }
            RobotCoordIndexRecord.Add(cmb_RobotCoord.SelectedIndex);
        }

        private void cmb_MoveMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotSetJogRemote(Convert.ToBoolean(cmb_MoveMode.SelectedIndex));
        }

        private void cmb_MoveVel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (StationConfig.Instance.MainStation.mCurStatus == ObjectStation._StationStatus.Run)
            {
                return;
            }
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.
                RobotSetVel(double.Parse(cmb_MoveVel.Text.Trim()), 100, 100);
        }

        private void cmb_JogVel_SelectedIndexChanged(object sender, EventArgs e)
        {
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.SetJogSpeed(cmb_JogVel.SelectedIndex);
        }

        #region Jog运动
        private void btn_XPel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(1, 1);
        }

        private void btn_XPel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(1, 0);
        }

        private void btn_XMel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(1, -1);
        }

        private void btn_XMel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(1, 0);
        }

        private void btn_YPel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(2, 1);
        }

        private void btn_YPel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(2, 0);
        }

        private void btn_YMel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(2, -1);
        }

        private void btn_YMel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(2, 0);
        }

        private void btn_ZPel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(3, 1);
        }

        private void btn_ZPel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(3, 0);
        }

        private void btn_ZMel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(3, -1);
        }

        private void btn_ZMel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(3, 0);
        }

        private void btn_CPel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(4, 1);
        }

        private void btn_CPel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(4, 0);
        }

        private void btn_CMel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(4, -1);
        }

        private void btn_CMel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(4, 0);
        }

        private void btn_TPel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(5, 1);
        }

        private void btn_TPel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(5, 0);
        }

        private void btn_TMel_MouseDown(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(5, -1);
        }

        private void btn_TMel_MouseUp(object sender, MouseEventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 0)
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJogMove(5, 0);
        }
        #endregion

        #region inch寸动
        private void btn_XPel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            {
                float step = float.Parse(cmb_MoveUnit.Text);
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotInchMove(1, 1, step);
            }
        }

        private void btn_XMel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            {
                float step = float.Parse(cmb_MoveUnit.Text);
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotInchMove(1, -1, step);
            }
        }

        private void btn_YPel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            {
                float step = float.Parse(cmb_MoveUnit.Text);
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotInchMove(2, 1, step);
            }
        }

        private void btn_YMel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            {
                float step = float.Parse(cmb_MoveUnit.Text);
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotInchMove(2, -1, step);
            }
        }

        private void btn_ZPel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            {
                float step = float.Parse(cmb_MoveUnit.Text);
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotInchMove(3, 1, step);
            }
        }

        private void btn_ZMel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            {
                float step = float.Parse(cmb_MoveUnit.Text);
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotInchMove(3, -1, step);
            }
        }

        private void btn_CPel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            {
                float step = float.Parse(cmb_MoveUnit.Text);
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotInchMove(4, 1, step);
            }
        }

        private void btn_CMel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            {
                float step = float.Parse(cmb_MoveUnit.Text);
                TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotInchMove(4, -1, step);
            }
        }

        private void btn_TPel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            { }
        }

        private void btn_TMel_Click(object sender, EventArgs e)
        {
            if (cmb_MoveMode.SelectedIndex == 1)
            { }
        }
        #endregion

        private bool alarmMsgFlag = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Slab_XPos.Text = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.Xpos.ToString("f3");
                Slab_YPos.Text = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.Ypos.ToString("f3");
                Slab_ZPos.Text = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.Zpos.ToString("f3");
                Slab_CPos.Text = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.Cpos.ToString("f3");
                Slab_TPos.Text = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.Tpos.ToString("f3");
                Slab_Config.Text = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCurPos.Pose.ToString();
                /***机器人运动状态显示***/
                if (!TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotMontionDone)
                {
                    Slab_Runsts.Text = "Moving";
                    Slab_Runsts.BackColor = Color.Orange;
                }
                else
                {
                    Slab_Runsts.Text = "Stop";
                    Slab_Runsts.BackColor = Color.White;
                }
                /***有无报警***/
                if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsEmg)
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
                if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsError)
                {
                    Slab_ErrorInfo.Text = "Error";
                    Slab_ErrorInfo.BackColor = Color.Red;
                    if (!alarmMsgFlag)
                    {
                        if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.Alarms.Count > 0)
                        {
                            Slab_AlMsg.Text = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.Alarms[0];
                            Slab_AlMsg.BackColor = Color.Red;
                        }
                        alarmMsgFlag = true;
                    }
                }
                else
                {
                    Slab_ErrorInfo.Text = "NoError";
                    Slab_ErrorInfo.BackColor = Color.Lime;
                    Slab_AlMsg.Text = "";
                    Slab_AlMsg.BackColor = Color.White;
                    alarmMsgFlag = false;
                }
                /***有无上电***/
                if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotIsEnable)
                {
                    btn_Power.BackgroundImage = Properties.Resources.上电;
                    //btn_Restart.Enabled = false;
                    btn_Restart.BackgroundImage = Properties.Resources.重启;
                }
                else
                {
                    btn_Power.BackgroundImage = Properties.Resources.未上电;
                    //btn_Restart.Enabled = true;
                    btn_Restart.BackgroundImage = Properties.Resources.回零;
                }
                /***显示当前速度***/
                Slab_Vel.Text = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.
                    RobotStatus.RobotCurVel.ToString("f2");
                /***显示坐标系***/
                if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCoord ==
                    GuideCoord.关节坐标)
                {
                    Slab_CoordInfo.Text = "Joint";
                }
                else if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCoord ==
                    GuideCoord.工具坐标)
                {
                    Slab_CoordInfo.Text = "Tool";
                }
                else if (TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotStatus.RobotCoord ==
                    GuideCoord.工件坐标)
                {
                    Slab_CoordInfo.Text = "Work";
                }
                else
                {
                    Slab_CoordInfo.Text = "World";
                }
            }
            catch
            {
                ;
            }
        }

        private void ToshibalRobotForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (timer1 != null)
                {
                    timer1.Stop();
                }
                for (int i = 0; i < cmb_RobotType.Items.Count; i++)
                {
                    TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(i).Value.DisconnectRobot();
                }
            }
            catch
            { }
        }

        #region 方法测试
        private void button1_Click(object sender, EventArgs e)
        {
            //bool res = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.MovesCalibration
            //     (_InputCollect.急停按钮.ToString(),
            //     TsRemoteRobotConfig.Instance.TsRemoteRobotDic[RobotName.搬运机器人.ToString()].RobotPointDic[PointSheetName.Point_ok.ToString()][Point_ok_name.点位4.ToString()].RobotPos,
            //     TsRemoteRobotConfig.Instance.TsRemoteRobotDic[RobotName.搬运机器人.ToString()].RobotPointDic[PointSheetName.Point_ok.ToString()][Point_ok_name.点位5.ToString()].RobotPos,
            //     5,
            //     1);
            //if (!res)
            //{
            //    MessageBox.Show("对针失败！");
            //}
            //else
            //{
            //    MessageBox.Show("对针成功！");
            //}
        }

        PosInfo ps = new PosInfo();
        private void button2_Click(object sender, EventArgs e)
        {
            ps = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.GetCurrentPoint();
            //label3.Text = string.Format("{0} {1} {2} {3} {4}", ps.Xpos, ps.Ypos, ps.Zpos, ps.Cpos, ps.Pose.ToString());
            ps = TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.GetTsTransPoint();
            //label4.Text = string.Format("{0} {1} {2} {3} {4}", ps.Xpos, ps.Ypos, ps.Zpos, ps.Cpos, ps.Pose.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.WaitRobotTsTransMoveS(ps, 20);

            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.WaitRobotTsTransMoveS(ps, 20, 0, 0, 0, 0);

            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.WaitRobotTsTransMoveS(ps, 20, 1.2, -3.4, 1.6, 5.4);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //控制单关节运动
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.MoveA(4, -15);

            int index = dataGridView1.CurrentRow.Index;
            PosInfo runpos = new PosInfo();
            runpos.Xpos = double.Parse(dataGridView1[1, index].Value.ToString());
            runpos.Ypos = double.Parse(dataGridView1[2, index].Value.ToString());
            runpos.Zpos = double.Parse(dataGridView1[3, index].Value.ToString());
            runpos.Cpos = double.Parse(dataGridView1[4, index].Value.ToString());
            runpos.Tpos = double.Parse(dataGridView1[5, index].Value.ToString());
            runpos.SixPos = 0;
            switch (dataGridView1[6, index].Value.ToString())
            {
                case "FREE":
                    runpos.Pose = PosConfig.FREE;
                    break;
                case "LEFTY":
                    runpos.Pose = PosConfig.LEFTY;
                    break;
                case "RIGHTY":
                    runpos.Pose = PosConfig.RIGHTY;
                    break;
            }
            //关节插补：弧线运动
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.MovePoint(runpos.Xpos, runpos.Ypos, runpos.Zpos, runpos.Cpos, runpos.Pose);
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.MovePointOffset(runpos, 1.2, -3.4, 1.6, 5.4);

            string posType = cmb_PointType.Text.Trim();
            string posName = dataGridView1[0, index].Value.ToString();
            int posVel = int.Parse(cmb_MoveVel.Text.Trim());
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotMoveJ(posType, posName, 50);

            PosOffset offset = new PosOffset();
            offset.x = 1.2; offset.y = -3.4; offset.z = 1.6; offset.r = 5.4;
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotMoveJ(posType, posName, 1, offset);

            //直线插补：直线运动
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.MoveS(runpos.Xpos, runpos.Ypos, runpos.Zpos, runpos.Cpos, runpos.Pose);
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.MoveSOffset(runpos, 1.2, -3.4, 1.6, 5.4);
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotMoveL(posType, posName, 50);
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotMoveL(posType, posName, 50, offset);

            //跳跃运动：拱形运动
            //TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJump(posType, posName, 20, 5, 5, 50);
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.RobotJump(posType, posName, 20, 5, 5, 2, offset);
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            PosInfo runpos = new PosInfo();
            runpos.Xpos = -131;
            runpos.Ypos = -423;
            runpos.Zpos = 69;
            runpos.Cpos = -113;
            runpos.Tpos = 0;
            runpos.SixPos = 0;
            runpos.Pose = PosConfig.LEFTY;
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.SelectExecutionFile("TEST0126");
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.ChangeGlobalPoint("A1", runpos, true);
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.ChangeGlobalPoint("A2", 395, -220, 74, -33, 0, PosConfig.LEFTY, true);
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.ChangeGlobalValue("B1", 20, true);
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.ChangeGlobalValue("B2", 5.1, true);
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.ChangeTransPoint("B4", 5.1, 5.1, 5.1, 5.1, true);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.ExecutionRun();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.ExecutionStop();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TsRemoteRobotConfig.Instance.TsRemoteRobotDic.ElementAt(cmb_RobotType.SelectedIndex).Value.ExexutionBreak();
        }
        #endregion
    }
}
