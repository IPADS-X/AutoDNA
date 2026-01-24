using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using MsgBoxLib;

namespace CYStandardProcedure
{
    public partial class AxisDebugForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        /***运动模式***/
        private enum _MoveMode
        {
            点动模式,
            寸动模式
        }

        #region 窗体控件自适应代    
        private void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
            cmb_Station_SelectedIndexChanged(new object(), new EventArgs());
            /***当窗体大小改变时候也需要重新设置标题语言***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        #endregion 窗体控件自适应代码
        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();
        /***点前电机移动模式***/
        private _MoveMode mMoveMode;

        public AxisDebugForm()
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

        private void AxisDebugForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            rdb_Lnching.Checked = true;
            /***开启定时器***/
            timer1.Enabled = true;
            timer1.Interval = 30;
            /***显示轴列表***/
            for (int i = 0; i < ParameConfig.Instance.AxisParameDic.Count; i++)
            {
                cmb_Axis.Items.Add(ParameConfig.Instance.AxisParameDic.ElementAt(i).Value.AxisName);
            }
            cmb_Axis.SelectedIndex = 0;
            /***显示工位列表***/
            for (int j = 0; j < ParameConfig.Instance.PointParamTypeNameList.Count; j++)
            {
                cmb_Station.Items.Add(ParameConfig.Instance.PointParamTypeNameList[j]);
            }
            cmb_Station.SelectedIndex = 0;
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += AxisDebugForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void AxisDebugForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变Panel容器内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_GetPos, "坐标提取");
                toolTip1.SetToolTip(btn_Move, "点位运动");
                toolTip1.SetToolTip(btn_Save, "点位写入");
                toolTip1.SetToolTip(btn_Svo, "使能操作");
                toolTip1.SetToolTip(btn_Home, "回零操作");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_GetPos, "Coordinate Extraction");
                toolTip1.SetToolTip(btn_Move, "Point Move");
                toolTip1.SetToolTip(btn_Save, "Point Write");
                toolTip1.SetToolTip(btn_Svo, "Enable Operation");
                toolTip1.SetToolTip(btn_Home, "Back To Zero");
            }
            else
            {
                toolTip1.SetToolTip(btn_GetPos, "Phối hợp thoát");
                toolTip1.SetToolTip(btn_Move, "Chuyển động điểm");
                toolTip1.SetToolTip(btn_Save, "Ghi điểm");
                toolTip1.SetToolTip(btn_Svo, "Bật hoạt động");
                toolTip1.SetToolTip(btn_Home, "Về tới số không");
            }
            /***重新加载***/
            cmb_Station_SelectedIndexChanged(new object(), new EventArgs());
        }

        private void btn_GetPos_Click(object sender, EventArgs e)
        {
            try
            {
                /***选中点位名称，提取所有轴点位坐标，选中单轴对应的单元格，提取单轴点位坐标***/
                int colIndex = dataGridView1.CurrentCell.ColumnIndex;
                int rowindex = dataGridView1.CurrentCell.RowIndex;
                /***列索引为0表示选中整行***/
                if (colIndex == 0)
                {
                    for (int i = 1; i < ParameConfig.Instance.StationAxisNameDic[cmb_Station.SelectedItem.ToString()].Length + 1; i++)
                    {
                        if (dataGridView1[i, rowindex].Value.ToString() != "NA")
                        {
                            string axisNum = ParameConfig.Instance.StationAxisNameDic[cmb_Station.SelectedItem.ToString()][i - 1];
                            int num = ParameConfig.Instance.AxisParameDic.Keys.ToList().LastIndexOf(axisNum);
                            dataGridView1[i, rowindex].Value = MotionConfig.Instance.CurPos[num];
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                /***表示选中单列***/
                else
                {
                    if (dataGridView1[colIndex, rowindex].Value.ToString() != "NA")
                    {
                        string axisNum = ParameConfig.Instance.StationAxisNameDic[cmb_Station.SelectedItem.ToString()][colIndex - 1];
                        int num = ParameConfig.Instance.AxisParameDic.Keys.ToList().LastIndexOf(axisNum);
                        dataGridView1[colIndex, rowindex].Value = MotionConfig.Instance.CurPos[num];
                    }
                }
            }
            catch (Exception ex)
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    MessageBox.Show("坐标提取失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(LanguageConfig.Instance.Language=="EN")
                {
                    MessageBox.Show("Coordinate Extraction Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Lỗi rượt đuổi！", "Mẹo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            btn_GetPos.BaseColorEnd = Color.Transparent;
        }

        private void btn_Move_Click(object sender, EventArgs e)
        {
            try
            {
                if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ResetDone == false)
                {
                    if (LanguageConfig.Instance.Language == "CH")
                    {
                        MessageBox.Show("设备未复位,点位移动失败！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (LanguageConfig.Instance.Language == "EN")
                    {
                        MessageBox.Show("The device is not reset, and the point fails to move！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Thiết bị không được đặt lại, di chuyển điểm không thành công！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                /***选中点位名称，除了NA点，轴一起运动，选中单轴对应的单元格，单轴移动***/
                int colIndex = dataGridView1.CurrentCell.ColumnIndex;
                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                /***联动坐标***/
                string[] arrayPos = new string[ParameConfig.Instance.StationAxisNameDic[cmb_Station.SelectedItem.ToString()].Length];
                /***单动坐标***/
                string pos;
                /***列索引为0表示选中整行***/
                if (dataGridView1.CurrentCell.ColumnIndex == 0)
                {
                    for (int i = 0; i < arrayPos.Length; i++)
                    {
                        arrayPos[i] = dataGridView1[i + 1, rowIndex].Value.ToString();
                    }
                    for (int j = 0; j < arrayPos.Length; j++)
                    {
                        if (arrayPos[j] != "NA")
                        {
                            string axisNum = ParameConfig.Instance.StationAxisNameDic[cmb_Station.SelectedItem.ToString()][j];
                            int num = ParameConfig.Instance.AxisParameDic.Keys.ToList().LastIndexOf(axisNum);

                            if (StationConfig.Instance.MainStation.mCurStatus == ObjectStation._StationStatus.Alarm)
                            {
                                MessageBox.Show("Move Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (MotionConfig.Instance.MotionStatusList[num].Alm)
                            {
                                MessageBox.Show(axisNum + " Alarm,Move Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (MotionConfig.Instance.MotionStatusList[num].Moving)
                            {
                                return;
                            }
                            if (!MotionConfig.Instance.MotionStatusList[num].Svo)
                            {
                                MotionConfig.Instance.ServoOn(dataGridView1.Columns[j + 1].HeaderText);
                            }
                            if (cmb_Station.Text.Contains("搬运模组XY"))
                            {
                                if ((Convert.ToDouble(MotionConfig.Instance.CurPos[(int)_Axis.搬运ZAxis]) - ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]) > 1)
                                {
                                    MotionConfig.Instance.AbsoluteMove(_CarryStation2Axis.搬运ZAxis.ToString(), ParameConfig.Instance.PointParameDic[_PointArray.试管搬运上升位置.ToString()].PosList[(int)_CarryStation2Axis.搬运ZAxis]);
                                    Thread.Sleep(20);
                                    while (true)
                                    {
                                        Thread.Sleep(5);
                                        if (MotionConfig.Instance.MotionStatusList[num].Emg)
                                        {
                                            return;
                                        }
                                        if (MotionConfig.Instance.MotionStatusList[(int)_Axis.搬运ZAxis].MoveDone)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            MotionConfig.Instance.AbsoluteMove(dataGridView1.Columns[j + 1].HeaderText, double.Parse(arrayPos[j]));
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                /***表示选中单列***/
                else
                {
                    pos = dataGridView1[colIndex, rowIndex].Value.ToString();
                    if (pos != "NA")
                    {
                        string axisNum = ParameConfig.Instance.StationAxisNameDic[cmb_Station.SelectedItem.ToString()][colIndex - 1];
                        int num = ParameConfig.Instance.AxisParameDic.Keys.ToList().LastIndexOf(axisNum);

                        if (StationConfig.Instance.MainStation.mCurStatus == ObjectStation._StationStatus.Alarm)
                        {
                            MessageBox.Show("Machine Alarm" + axisNum + "Move Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (MotionConfig.Instance.MotionStatusList[num].Alm)
                        {
                            MessageBox.Show(axisNum + " Alarm,Move Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (!MotionConfig.Instance.MotionStatusList[num].MoveDone)
                        {
                            return;
                        }
                        if (!MotionConfig.Instance.MotionStatusList[num].Svo)
                        {
                            MotionConfig.Instance.ServoOn(dataGridView1.Columns[colIndex].HeaderText);
                        }
                        MotionConfig.Instance.AbsoluteMove(dataGridView1.Columns[colIndex].HeaderText, double.Parse(pos));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Point Movement Failure！", "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btn_Move.BaseColorEnd = Color.Transparent;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!ParameConfig.Instance.ParameCfgDic["Point"].UpdateGridToFile(dataGridView1, cmb_Station.SelectedIndex))
            {
                MessageBox.Show("Point Parameter Save Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Point Parameter Save Successful！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            btn_Save.BaseColorEnd = Color.Transparent;
        }

        private void rdb_PointMove_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_PointMove.Checked)
            {
                mMoveMode = _MoveMode.点动模式;
            }
        }

        private void rdb_Lnching_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_Lnching.Checked)
            {
                mMoveMode = _MoveMode.寸动模式;
            }
        }

        /// <summary>
        /// 寸动模式
        /// </summary>
        /// <param name="dir">寸动方向</param>
        private void LnchingRun(string dir)
        {
            try
            {
                if (mMoveMode == _MoveMode.点动模式)
                {
                    return;
                }
                else
                {
                    if (StationConfig.Instance.MainStation.mCurStatus == ObjectStation._StationStatus.Alarm)
                    {
                        MessageBox.Show("Machine Alarm," + cmb_Axis.Text + "Inching Move Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (MotionConfig.Instance.MotionStatusList[cmb_Axis.SelectedIndex].Alm)
                    {
                        MessageBox.Show(cmb_Axis.Text + "Alarm！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (tb_Speed.Value == 0)
                    {
                        MessageBox.Show(cmb_Axis.Text + "Speed Not Set！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (MotionConfig.Instance.MotionStatusList[cmb_Axis.SelectedIndex].Moving)
                    {
                        return;
                    }
                    if (!MotionConfig.Instance.MotionStatusList[cmb_Axis.SelectedIndex].Svo)
                    {
                        MotionConfig.Instance.ServoOn(cmb_Axis.Text);
                    }
                    Thread.Sleep(100);
                    MotionConfig.Instance.RelativeMove(cmb_Axis.Text,
                        ParameConfig.Instance.AxisParameDic[cmb_Axis.Text].Acc,
                        ParameConfig.Instance.AxisParameDic[cmb_Axis.Text].Dec, tb_Speed.Value, (double)ntx_Distance.Num, dir);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(cmb_Axis.Text + "Inching Move Failed！", "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 点动模式
        /// </summary>
        /// <param name="dir">点动方向</param>
        private void StartPointRun(string dir)
        {
            try
            {
                if (mMoveMode == _MoveMode.寸动模式)
                {
                    return;
                }
                else
                {
                    if (StationConfig.Instance.MainStation.mCurStatus == ObjectStation._StationStatus.Alarm)
                    {
                        MessageBox.Show("Machine Alarm," + cmb_Axis.Text + "Inching Move Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (MotionConfig.Instance.MotionStatusList[cmb_Axis.SelectedIndex].Alm)
                    {
                        MessageBox.Show(cmb_Axis.Text + "Alarm！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (tb_Speed.Value == 0)
                    {
                        MessageBox.Show(cmb_Axis.Text + "Speed Not Set！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!MotionConfig.Instance.MotionStatusList[cmb_Axis.SelectedIndex].Svo)
                    {
                        MotionConfig.Instance.ServoOn(cmb_Axis.Text);
                    }
                    Thread.Sleep(100);
                    MotionConfig.Instance.StartJOGMove(cmb_Axis.Text,
                        ParameConfig.Instance.AxisParameDic[cmb_Axis.Text].Acc,
                        ParameConfig.Instance.AxisParameDic[cmb_Axis.Text].Dec, tb_Speed.Value, dir);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(cmb_Axis.Text + "ContinuousMove Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 停止点动
        /// </summary>
        private void StopPointRun()
        {
            try
            {
                if (mMoveMode == _MoveMode.寸动模式)
                {
                    return;
                }
                else
                {
                    MotionConfig.Instance.StopJOGMove(cmb_Axis.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(cmb_Axis.Text + "Stop Failed！", "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rbt_FrontMove_Click(object sender, EventArgs e)
        {
            LnchingRun("负");
        }

        private void rbt_FrontMove_MouseDown(object sender, MouseEventArgs e)
        {
            StartPointRun("负");
        }

        private void rbt_FrontMove_MouseUp(object sender, MouseEventArgs e)
        {
            StopPointRun();
        }

        private void rbt_BackMove_Click(object sender, EventArgs e)
        {
            LnchingRun("正");
        }

        private void rbt_BackMove_MouseDown(object sender, MouseEventArgs e)
        {
            StartPointRun("正");
        }

        private void rbt_BackMove_MouseUp(object sender, MouseEventArgs e)
        {
            StopPointRun();
        }

        private void rbt_LeftMove_Click(object sender, EventArgs e)
        {
            LnchingRun("负");
        }

        private void rbt_LeftMove_MouseDown(object sender, MouseEventArgs e)
        {
            StartPointRun("负");
        }

        private void rbt_LeftMove_MouseUp(object sender, MouseEventArgs e)
        {
            StopPointRun();
        }

        private void rbt_RightMove_Click(object sender, EventArgs e)
        {
            LnchingRun("正");
        }

        private void rbt_RightMove_MouseDown(object sender, MouseEventArgs e)
        {
            StartPointRun("正");
        }

        private void rbt_RightMove_MouseUp(object sender, MouseEventArgs e)
        {
            StopPointRun();
        }

        private void rbt_UpMove_Click(object sender, EventArgs e)
        {
            LnchingRun("正");
        }

        private void rbt_UpMove_MouseDown(object sender, MouseEventArgs e)
        {
            StartPointRun("正");
        }

        private void rbt_UpMove_MouseUp(object sender, MouseEventArgs e)
        {
            StopPointRun();
        }

        private void rbt_DwMove_Click(object sender, EventArgs e)
        {
            LnchingRun("负");
        }

        private void rbt_DwMove_MouseDown(object sender, MouseEventArgs e)
        {
            StartPointRun("负");
        }

        private void rbt_DwMove_MouseUp(object sender, MouseEventArgs e)
        {
            StopPointRun();
        }

        private void rbt_AlongRotate_Click(object sender, EventArgs e)
        {
            LnchingRun("正");
        }

        private void rbt_AlongRotate_MouseDown(object sender, MouseEventArgs e)
        {
            StartPointRun("正");
        }

        private void rbt_AlongRotate_MouseUp(object sender, MouseEventArgs e)
        {
            StopPointRun();
        }

        private void rbt_InverseRotate_Click(object sender, EventArgs e)
        {
            LnchingRun("负");
        }

        private void rbt_InverseRotate_MouseDown(object sender, MouseEventArgs e)
        {
            StartPointRun("负");
        }

        private void rbt_InverseRotate_MouseUp(object sender, MouseEventArgs e)
        {
            StopPointRun();
        }

        private void btn_Home_Click(object sender, EventArgs e)
        {
            try
            {
                if (StationConfig.Instance.MainStation.mCurStatus == ObjectStation._StationStatus.Alarm)
                {
                    MessageBox.Show("Machine Alarm," + cmb_Axis.Text + "Home Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (MotionConfig.Instance.MotionStatusList[cmb_Axis.SelectedIndex].Alm)
                {
                    MessageBox.Show(cmb_Axis.Text + "Alarm,Home Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!MotionConfig.Instance.MotionStatusList[cmb_Axis.SelectedIndex].Svo)
                {
                    MotionConfig.Instance.ServoOn(cmb_Axis.Text);
                }
                Thread.Sleep(100);
                if (MotionConfig.Instance.MotionStatusList[cmb_Axis.SelectedIndex].Homing)
                {
                    return;
                }
                MotionConfig.Instance.HomeStart(cmb_Axis.Text);

                int AxisIndex = cmb_Axis.SelectedIndex;
                string AxiaName = cmb_Axis.Text;
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(5);
                        if (!MotionConfig.Instance.MotionStatusList[AxisIndex].Homing)
                        {
                            MotionConfig.Instance.HomeCancel(AxiaName);
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(cmb_Axis.Text + "Home Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                /***轴状态IO***/
                int index = cmb_Axis.SelectedIndex;
                pic_Homing.BackColor = MotionConfig.Instance.MotionStatusList[index].Homing ? Color.Red : Color.Black;
                pic_Alm.BackColor = MotionConfig.Instance.MotionStatusList[index].Alm ? Color.Red : Color.Black;
                pic_Svon.BackColor = MotionConfig.Instance.MotionStatusList[index].Svo ? Color.Red : Color.Black;
                pic_Pel.BackColor = MotionConfig.Instance.MotionStatusList[index].Pel ? Color.Red : Color.Black;
                pic_Mel.BackColor = MotionConfig.Instance.MotionStatusList[index].Mel ? Color.Red : Color.Black;
                pic_Org.BackColor = MotionConfig.Instance.MotionStatusList[index].Ori ? Color.Red : Color.Black;
                pic_Nstp.BackColor = MotionConfig.Instance.MotionStatusList[index].Moving ? Color.Red : Color.Black;

                if (MotionConfig.Instance.MotionStatusList[index].Svo)
                {
                    btn_Svo.BackgroundImage = Properties.Resources.Svo;
                }
                else
                {
                    btn_Svo.BackgroundImage = Properties.Resources.NoSvo;
                }

                /***当前轴位置***/
                lab_CurPos.Text = MotionConfig.Instance.CurPos[index].ToString();
            }
            catch (Exception ex)
            { }
        }

        private void rbt_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                MotionConfig.Instance.EmgAxisMove(cmb_Axis.Text);
                MotionConfig.Instance.HomeCancel(cmb_Axis.Text);
            }
            catch (Exception ex)
            { }
        }

        private void btn_Svo_Click(object sender, EventArgs e)
        {
            try
            {
                if (StationConfig.Instance.MainStation.mCurStatus == ObjectStation._StationStatus.Alarm)
                {
                    MessageBox.Show("Device Alarm, Enable Operation Failed！", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (MotionConfig.Instance.MotionStatusList[cmb_Axis.SelectedIndex].Svo)
                {
                    MotionConfig.Instance.ServoOff(cmb_Axis.Text);
                }
                else
                {
                    MotionConfig.Instance.ServoOn(cmb_Axis.Text);
                }
            }
            catch (Exception ex)
            { }
        }

        private void cmb_Axis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_Axis.Text.Contains("X"))
            {
                rbt_LeftMove.Enabled = true;
                rbt_RightMove.Enabled = true;
                rbt_UpMove.Enabled = false;
                rbt_DwMove.Enabled = false;
                rbt_FrontMove.Enabled = false;
                rbt_BackMove.Enabled = false;
                rbt_InverseRotate.Enabled = false;
                rbt_AlongRotate.Enabled = false;
            }
            else if (cmb_Axis.Text.Contains("Y"))
            {
                rbt_LeftMove.Enabled = false;
                rbt_RightMove.Enabled = false;
                rbt_FrontMove.Enabled = true;
                rbt_BackMove.Enabled = true;
                rbt_UpMove.Enabled = false;
                rbt_DwMove.Enabled = false;
                rbt_AlongRotate.Enabled = false;
                rbt_InverseRotate.Enabled = false;
            }
            else if (cmb_Axis.Text.Contains("Z"))
            {
                rbt_LeftMove.Enabled = false;
                rbt_RightMove.Enabled = false;
                rbt_FrontMove.Enabled = false;
                rbt_BackMove.Enabled = false;
                rbt_UpMove.Enabled = true;
                rbt_DwMove.Enabled = true;
                rbt_InverseRotate.Enabled = false;
                rbt_AlongRotate.Enabled = false;
            }
            else if (cmb_Axis.Text.Contains("R"))
            {
                rbt_LeftMove.Enabled = false;
                rbt_RightMove.Enabled = false;
                rbt_FrontMove.Enabled = false;
                rbt_BackMove.Enabled = false;
                rbt_UpMove.Enabled = false;
                rbt_DwMove.Enabled = false;
                rbt_AlongRotate.Enabled = true;
                rbt_InverseRotate.Enabled = true;
            }
        }

        private void cmb_Station_SelectedIndexChanged(object sender, EventArgs e)
        {
            /***清除表格***/
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.DataSource = null;
            ParameConfig.Instance.ParameCfgDic["Point"].UpdateParameterToGrid(dataGridView1, cmb_Station.SelectedItem.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string[] name = new string[]
            //{
            //    _Axis.XAxis.ToString(),
            //    _Axis.YAxis.ToString(),
            //    _Axis.ZAxis.ToString()
            //};
            //double[] pos = new double[]
            //{
            //    20,//X轴的取值范围：
            //    65,//Y轴的取值范围：
            //    60,//Z轴的取值范围：
            //};
            //double[] gearRatio = new double[]
            //{
            //    2.0,//从轴的齿轮比
            //    3.0,//从轴的齿轮比
            //};

            //int sw = 1;
            //int acc = 800;
            //int dec = 800;
            //double vel = 80;

            ///*** 833x卡线性插补测试 ***/
            //switch (sw)
            //{
            //    case 0:
            //        MotionConfig.Instance.LinearInterpolationMove(name, pos);
            //        break;
            //    case 1:
            //        MotionConfig.Instance.LinearInterpolationMove(name, acc, dec, vel, pos);
            //        break;
            //}

            ///*** 833x卡电子齿轮模式测试 ***/
            //pos[0] = -20;
            //switch (sw)
            //{
            //    case 0:
            //        MotionConfig.Instance.StartGearMove(name, gearRatio, pos[0]);
            //        break;
            //    case 1:
            //        MotionConfig.Instance.StartGearMove(name, gearRatio, acc, dec, vel, pos[0]);
            //        break;
            //}
            //MotionConfig.Instance.StopGearMove(name);

            /*** 模拟量输入测试 ***/
            //string analogInName = "";//模拟量输入点名称
            //double aivalue = 0;
            //IOInfo aiInfo;
            //bool result = IOConfig.Instance.AnalogInputDic.TryGetValue(analogInName, out aiInfo);
            //if (result)
            //{
            //    int cardIndex = aiInfo.CardNumber - 1;//卡ID
            //    int port = aiInfo.CardPort;//M60卡：port参数不起作用，8001：端口号
            //    int pointIndex = aiInfo.PointIndex;//点索引，从1开始
            //    IOConfig.Instance.ListIOCard[cardIndex].GetAnalogInBit(port, pointIndex, out aivalue);
            //}
        }
    }
}