using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CYAutoFramework;
using CYCustomControl;
using System.IO;
using MsgBoxLib;
using System.Threading;

namespace CYStandardProcedure
{
    public partial class MainForm : Form
    {
        private AutoSizeMDIForm mAutoSize = new AutoSizeMDIForm();
        /***主界面窗体自适应对象***/
        /***主界面UI图表路径***/
        private string mPath = AppDomain.CurrentDomain.BaseDirectory + @"\UIImage\MainIcon\";
        /***主界面UI图表集合***/
        private List<Image> mImageList = new List<Image>();
        /***按钮和窗体字典***/
        private Dictionary<RoundButton, Form> mFormDic = new Dictionary<RoundButton, Form>();
        /***按钮和索引字典***/
        private Dictionary<RoundButton, int> mBtnIndexDic = new Dictionary<RoundButton, int>();
        /***当前窗体***/
        private Form mCurForm;
        /***当前按钮***/
        private RoundButton mRoundBtn;
        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();
        public static MainForm mainform;


        ResultFolderVM resultFolderModel = null;
        /// <summary>
        /// 文件夹层级
        /// </summary>
        int MaxFolderLevel = 0;
        /// <summary>
        /// 总文件计数
        /// </summary>
        int TotalFileCount = 0;
        /// <summary>
        /// 已匹配文件计数
        /// </summary>
        int MatchedFileCount = 0;
        /// <summary>
        /// 链条数
        /// </summary>
        int TotalDNACounts = 0;
        /// <summary>
        /// 正确配对数
        /// </summary>
        int MatchDNACounts = 0;
        int MatchJianJiCounts = 0;
        bool IsProgress = false;
        private string cexuFilePath = @"E:\test\modeShow";
        private string IDNA_string = "ATCAGTACGGTGCACCACCATGAA";

        public MainForm()
        {
            InitializeComponent();
            mainform = this;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            toolTip1 = new ToolTip();
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;

            /***加载图标***/
            mImageList.Clear();
            DirectoryInfo d = new System.IO.DirectoryInfo(mPath);
            FileInfo[] fisBMP = d.GetFiles("*.png");
            int imagesCount = fisBMP.Length;
            for (int i = 0; i < imagesCount; i++)
            {
                Image img = Image.FromFile(mPath + i.ToString() + ".png");
                this.mImageList.Add(img);
            }
            /***按钮和整数绑定字典***/
            mBtnIndexDic.Add(btn_Mainccd, 0);
            mBtnIndexDic.Add(btn_Maininfo, 2);
            mBtnIndexDic.Add(btn_stationMsg, 4);
            mBtnIndexDic.Add(btn_stationData, 6);
            /***按钮赋图片***/
            btn_Mainccd.BackgroundImage = mImageList[0];
            btn_Maininfo.BackgroundImage = mImageList[2];
            btn_stationMsg.BackgroundImage = mImageList[4];
            btn_stationData.BackgroundImage = mImageList[6];
            /***按钮和窗体绑定字典***/
            mFormDic.Add(btn_Mainccd, new MainForm_CCD());
            mFormDic.Add(btn_Maininfo, new MainForm_info());
            mFormDic.Add(btn_stationMsg, new MainForm_Msg());
            mFormDic.Add(btn_stationData, new MainForm_Data());
            btn_stationData.PerformClick();
            btn_Mainccd.PerformClick();
            /***设定ListBox控件***/
            LogConfig.Instance.SetListBoxMsgControl(xListBox_Run, xListBox_NG, xListBox_Alarm);
            /***窗体控件自适应***/
            mAutoSize.controllInitializeSize(this);
            this.SizeChanged += MainForm_SizeChanged;

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += MainForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void MainForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
            /***清除报警信息***/
            LogConfig.Instance.ClearListMessage("Run");
            LogConfig.Instance.ClearListMessage("NG");
            LogConfig.Instance.ClearListMessage("Alarm");
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Mainccd, "图像界面");
                toolTip1.SetToolTip(btn_Maininfo, "主控界面");
                toolTip1.SetToolTip(btn_stationMsg, "信息界面");
                toolTip1.SetToolTip(btn_stationData, "测序数据解析界面");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Mainccd, "Image Interface");
                toolTip1.SetToolTip(btn_Maininfo, "MainControl Interface");
                toolTip1.SetToolTip(btn_stationMsg, "信息界面");
                toolTip1.SetToolTip(btn_stationData, "测序数据解析界面");
            }
            else
            {
                toolTip1.SetToolTip(btn_Mainccd, "Giao diện ảnh");
                toolTip1.SetToolTip(btn_Maininfo, "Giao diện điều khiển chính");
                toolTip1.SetToolTip(btn_stationMsg, "信息界面");
                toolTip1.SetToolTip(btn_stationData, "测序数据解析界面");
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            mAutoSize.controlAutoSize(this);
        }

        public void SwitchWnd(RoundButton btn)
        {
            int index;
            if (mRoundBtn != btn)
            {
                if (mRoundBtn != null)
                {
                    index = mBtnIndexDic[mRoundBtn];
                    mRoundBtn.BackgroundImage = mImageList[index];
                }
                mRoundBtn = btn;
                index = mBtnIndexDic[mRoundBtn] + 1;
                mRoundBtn.BackgroundImage = mImageList[index];
                if (mCurForm != null)
                    mCurForm.Hide();
                if (mCurForm != mFormDic[btn])
                {
                    mCurForm = mFormDic[btn];
                    mCurForm.TopLevel = false;
                    mCurForm.Parent = roundPanel1;
                    mCurForm.Dock = DockStyle.Fill;
                    mCurForm.Show();
                }
            }
        }

        private void btn_mainccd_Click_1(object sender, EventArgs e)
        {
            SwitchWnd(btn_Mainccd);
        }

        private void btn_maininfo_Click_1(object sender, EventArgs e)
        {
            SwitchWnd(btn_Maininfo);
        }

        private void btn_stationMsg_Click(object sender, EventArgs e)
        {
            SwitchWnd(btn_stationMsg);
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            SwitchWnd(btn_stationData);
        }


        private void rbt_Show_Click(object sender, EventArgs e)
        {
            //判断设备是否是停止中状态
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus != ObjectStation._StationStatus.Stop)
            {
                LogConfig.Instance.ShowMessageToList("Run", "设备状态不是停止中,无法切换模式!", MsgType.Success, Color.Red);
                return;
            }

            //判断机台是否是初始状态
            if (SerializeClass.mMemory.CarryStation_state != MemoryClass.CarryStation_State.空闲
             || SerializeClass.mMemory.FeedingStation_state != MemoryClass.FeedingStation_State.空闲
             || SerializeClass.mMemory.SequencingStation_state != MemoryClass.SequencingStation_State.空闲
             || SerializeClass.mMemory.DataProcessingStation_state != MemoryClass.DataProcessingStation_State.空闲
             || SerializeClass.mMemory.RobotStation_state != MemoryClass.RobotStation_State.空闲)
            {
                LogConfig.Instance.ShowMessageToList("Run", "当前设备有任务执行,无法切换模式!", MsgType.Success, Color.Red);
                return;
            }

            //判断样本载具是否放入设备
            if (!MyVariable.show_IsOpen)
            {
                if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.光电8联排试管区1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.光电8联排试管区2])
                {
                    LogConfig.Instance.ShowMessageToList("Run", "样本未放入工位,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                {
                    LogConfig.Instance.ShowMessageToList("Run", "PLC交互屏蔽中,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (!Program.modbusTcp_PLC.Connect())
                {
                    LogConfig.Instance.ShowMessageToList("Run", "PLC连接失败,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
            }

            //开启参观模式
            if (!MyVariable.show_IsOpen)
            {
                MsgBox mb2 = new MsgBox(MsgBoxType.提示, BtnType.YesNo, false);
                mb2.MsgShowDialog("提示", "输入密码,确认是否开启参观模式");
                string btn2 = mb2.ret.SelectedBtn;
                string id2 = mb2.ret.RichText.Trim().Replace("\n", "");
                if (btn2 == "btn_A")
                {
                    if (id2 != "cy123")
                    {
                        MessageBox.Show("密码错误,模式切换失败");
                        return;
                    }
                    if (!CanGuanStart())
                    {
                        MessageBox.Show("共享文件写入失败,检查总控是否开机!检查网络!");
                        LogConfig.Instance.ShowMessageToList("Run", "共享文件写入失败,模式切换失败", MsgType.Success, Color.Red);
                        return;
                    }
                    #region 判断是否需要供料机上料
                    MsgBox mbx = new MsgBox(MsgBoxType.提示, BtnType.YesNo, true);
                    mbx.MsgShowDialog("是否需要补耗材", "耗材载具未放入设备,需要联合地轨供料机上耗材,选择  是                                                                                                耗材载具已放入设备,并且耗材补满,选择  否");
                    string btn = mbx.ret.SelectedBtn;
                    if (btn == "btn_A")
                    {
                        if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "1.5试管载具区有载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.低温区光电])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "低温载具区有载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "1000枪头载具区1有载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "1000枪头载具区2有载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "200枪头载具区有载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "50枪头载具区有载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        //耗材空料赋值
                        MyVariable.area_QiangTou1.num_Remain = 0;
                        MyVariable.area_QiangTou2.num_Remain = 0;
                        MyVariable.consumables_Empty[0] = true;
                        MyVariable.area_QiangTou3.num_Remain = 0;
                        MyVariable.consumables_Empty[1] = true;
                        MyVariable.area_QiangTou4.num_Remain = 0;
                        MyVariable.consumables_Empty[2] = true;
                        MyVariable.area_DiWen_FCT.num_Remain = 0;
                        MyVariable.consumables_Empty[3] = true;
                        MyVariable.area_DiWen_FCF.num_Remain = 0;
                        MyVariable.area_DiWen_SB.num_Remain = 0;
                        MyVariable.area_DiWen_LIB.num_Remain = 0;
                        MyVariable.area_DiWen_DIL.num_Remain = 0;
                        MyVariable.area_DiWen_WMX.num_Remain = 0;
                        MyVariable.area_DiWen_S.num_Remain = 0;
                        MyVariable.area_LiXinGuan.num_Remain = 0;
                        MyVariable.consumables_Empty[4] = true;
                    }
                    if (btn == "btn_B")
                    {
                        if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "1.5试管载具区无载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.低温区光电])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "低温载具区无载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "1000枪头载具区1无载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "1000枪头载具区2无载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "200枪头载具区无载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电2])
                        {
                            LogConfig.Instance.ShowMessageToList("Run", "50枪头载具区无载具,模式切换失败", MsgType.Success, Color.Red);
                            return;
                        }
                        MyVariable.area_QiangTou1.num_Remain = 80;
                        MyVariable.area_QiangTou2.num_Remain = 80;
                        MyVariable.consumables_Empty[0] = false;
                        MyVariable.area_QiangTou3.num_Remain = 80;
                        MyVariable.consumables_Empty[1] = false;
                        MyVariable.area_QiangTou4.num_Remain = 80;
                        MyVariable.consumables_Empty[2] = false;
                        MyVariable.area_DiWen_FCT.num_Remain = MyVariable.FCT_MAX;
                        MyVariable.consumables_Empty[3] = false;
                        MyVariable.area_DiWen_FCF.num_Remain = MyVariable.FCF_MAX;
                        MyVariable.area_DiWen_SB.num_Remain = MyVariable.SB_MAX;
                        MyVariable.area_DiWen_LIB.num_Remain = MyVariable.LIB_MAX;
                        MyVariable.area_DiWen_DIL.num_Remain = MyVariable.DIL_MAX;
                        MyVariable.area_DiWen_WMX.num_Remain = MyVariable.WMX_MAX;
                        MyVariable.area_DiWen_S.num_Remain = MyVariable.S_MAX;
                        MyVariable.area_LiXinGuan.num_Remain = 18;
                        MyVariable.consumables_Empty[4] = false;
                    }
                    #endregion

                    #region 初始化记忆
                    SerializeClass.mMemory.FeedingStation_state = MemoryClass.FeedingStation_State.空闲;
                    SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.空闲;
                    SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.空闲;
                    SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.空闲;
                    SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪默认松开;
                    SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪默认松开;
                    SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.未取枪头;
                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区1;
                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                    SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                    SerializeClass.mMemory.DataProcessingStation_state = MemoryClass.DataProcessingStation_State.空闲;
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64600, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64614, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64615, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64616, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64622, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64634, 0);
                    MyVariable.b_StatusToControl = false;
                    MyVariable.need_Completed = false;
                    MyVariable.CCD_QiPao = false;
                    MyVariable.JianJiShiBie_Start = false;
                    MyVariable.show_memory = 0;
                    MyVariable.File_Copy.Clear();
                    MyVariable.FunctionStep = 0;
                    LogConfig.Instance.ShowMessageToList("Run", "初始化记忆成功！", MsgType.Success, Color.Green);
                    #endregion

                    rbt_Show.Text = "退出参观模式";
                    rbt_Show.BaseColor = Color.MediumTurquoise;
                    rbt_Show.BaseColorEnd = Color.MediumTurquoise;
                    roundButton1.Visible = false;
                    //参观模式试剂用量缩减
                    MyVariable.ReadShowVolume();
                    //参观模式标志
                    MyVariable.show_IsOpen = true;
                    MyVariable.show_Repeat = false;

                    IOConfig.Instance.SetSingleOut(_OutputCollect.三色灯红.ToString(), 0);
                    IOConfig.Instance.SetSingleOut(_OutputCollect.三色灯绿.ToString(), 0);
                    IOConfig.Instance.SetSingleOut(_OutputCollect.三色灯黄.ToString(), 0);

                    SoftWareForm.m_softwarmform.Invoke(new Action(() =>
                    {
                        SoftWareForm.m_softwarmform.lab_RunMode.Text = "单机参观模式";
                    }));
                    MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                    {
                        MainForm_Data.mMainForm_Data.label1.Visible = false;
                        MainForm_Data.mMainForm_Data.txt_FolderPath.Visible = false;
                        MainForm_Data.mMainForm_Data.lblCount.Visible = false;
                        MainForm_Data.mMainForm_Data.lab_ZongKongJJ.Visible = false;
                        MainForm_Data.mMainForm_Data.label5.Visible = false;
                        MainForm_Data.mMainForm_Data.txt_JianJiMsg.Visible = false;
                    }));
                    LogConfig.Instance.ShowMessageToList("Run", "单机参观模式已启用!", MsgType.Success, Color.Green);
                    Task.Run(() =>
                    {
                        if (!Directory.Exists(cexuFilePath))
                        {
                            Directory.CreateDirectory(cexuFilePath);
                        }
                        DataAnalysisMethods(cexuFilePath);
                    });
                }
            }
            //关闭参观模式
            else
            {
                rbt_Show.Text = "单机参观模式";
                rbt_Show.BaseColor = Color.Tomato;
                rbt_Show.BaseColorEnd = Color.Tomato;
                roundButton1.Visible = true;
                //退出参观模式,试剂用量恢复
                MyVariable.ReadPipetteParam();
                //关闭参观模式标志
                MyVariable.show_IsOpen = false;
                SoftWareForm.m_softwarmform.Invoke(new Action(() =>
                {
                    SoftWareForm.m_softwarmform.lab_RunMode.Text = "自动运行模式";
                }));
                MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                {
                    MainForm_Data.mMainForm_Data.label1.Visible = true;
                    MainForm_Data.mMainForm_Data.txt_FolderPath.Visible = true;
                    MainForm_Data.mMainForm_Data.lblCount.Visible = true;
                    MainForm_Data.mMainForm_Data.lab_ZongKongJJ.Visible = true;
                    MainForm_Data.mMainForm_Data.lblExecMsg.Visible = true;
                    MainForm_Data.mMainForm_Data.label5.Visible = true;
                    MainForm_Data.mMainForm_Data.txt_JianJiMsg.Visible = true;
                }));

                LogConfig.Instance.ShowMessageToList("Run", "单机参观模式已退出!", MsgType.Success, Color.Green);
            }

        }

        public bool CanGuanStart()
        {
            try
            {
                string str2 = @"\\" + ParameConfig.Instance.SystemParameDic[_ParamName.GeneralShareIP.ToString()].CurrentValue + @"\Cexu\Start";
                if (Directory.Exists(str2))
                {
                    MyVariable.DeleteFilesInDirectory(str2);
                    string filePath = Path.Combine(str2, "start.txt");
                    File.Create(filePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception d)
            {
                return false;
            }
        }



        /// <summary>
        /// 解析序列
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public bool DataAnalysisMethods(string filePath)
        {
            if (IsProgress)
            {
                LogConfig.Instance.ShowMessageToList("Run", "参观模式文件解析中", MsgType.Success, Color.Red);
                return false;
            };
            IsProgress = true;
            MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
            {
                MainForm_Data.mMainForm_Data.chart1.Series[0].Points.Clear(); // 清空所有数据点
                MainForm_Data.mMainForm_Data.cbx_barcode.Items.Clear();
                MainForm_Data.mMainForm_Data.txt_JianJiMsg.Text = "";
                MainForm_Data.mMainForm_Data.lab_jianjiMax.Text = "测序结果(饼状图中百分比最高的碱基)：";
            }));
            MyVariable.AutoAllJianJiDics.Clear();
            MyVariable.AutoJianJiDicsMost.Clear();
            MyVariable.AutoJianJiList.Clear();
            MyVariable.AutoNumList.Clear();

            try
            {
                string folderPath = filePath;
                string[] subdirectories = Directory.GetDirectories(folderPath);
                TotalFileCount = 0;
                resultFolderModel = new ResultFolderVM()
                {
                    FolderLevel = 0,
                    FolderPath = folderPath,
                    FolderName = new DirectoryInfo(folderPath).Name,
                    FileList = SeqkitHelper.GetFileList(folderPath),
                    SubFolderList = GetSubFolderLists(folderPath, 1)
                };
                TotalFileCount += resultFolderModel.FileList.Count;
                TotalDNACounts = 0;
                MatchDNACounts = 0;
                MatchJianJiCounts = 0;
                MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                {
                    MainForm_Data.mMainForm_Data.lblCount.Text = $"测序文件{TotalFileCount}个";
                    MainForm_Data.mMainForm_Data.lab_totalDNA.Text = TotalDNACounts.ToString();
                    MainForm_Data.mMainForm_Data.lab_matchDNA.Text = MatchDNACounts.ToString();
                    MainForm_Data.mMainForm_Data.txt_FolderPath.Text = cexuFilePath;
                    MainForm_Data.mMainForm_Data.txt_IDNA.Text = IDNA_string;
                }));
                MatchedFileCount = 0;

                MatchDatas(resultFolderModel);

                //深拷贝,创建新对象
                foreach (var kvp in MyVariable.AutoAllJianJiDics)
                {
                    // 创建新的字典并复制内容
                    MyVariable.AutoJianJiDicsMost[kvp.Key] = new Dictionary<string, int>(kvp.Value);
                }
                //超过5种碱基用others代替总和
                foreach (var item in MyVariable.AutoJianJiDicsMost)
                {
                    if (MyVariable.AutoJianJiDicsMost[item.Key].Count > 5)
                    {
                        int sum = MyVariable.AutoJianJiDicsMost[item.Key].Values.Skip(4).Sum(); // 计算第五项开始到最后的所有值的和
                        string key = "others"; // 新的键名
                                               // 删除第五项及之后的键值对
                        var keysToDelete = MyVariable.AutoJianJiDicsMost[item.Key].Keys.Skip(4).ToList();
                        foreach (var k in keysToDelete)
                        {
                            MyVariable.AutoJianJiDicsMost[item.Key].Remove(k);
                        }
                        MyVariable.AutoJianJiDicsMost[item.Key][key] = sum; // 更新字典，键为"others"，值为sum
                    }
                }

                //编译字符给数字孪生
                string s = "";
                foreach (var item in MyVariable.AutoJianJiDicsMost)
                {
                    if (MyVariable.AutoJianJiDicsMost[item.Key].Count == 0)
                    {
                        continue;
                    }
                    s += Convert.ToInt32(item.Key.Replace("barcode", "")) + "-";
                    foreach (var members in MyVariable.AutoJianJiDicsMost[item.Key])
                    {
                        s += members.Key + "*" + members.Value + "&";
                    }
                    s = s.Substring(0, s.Length - 1) + "|";
                }
                if (s.Length > 0)
                {
                    s = s.Substring(0, s.Length - 1);
                }
                SerializeClass.animationParam.BaseMsg = s;

                ////将每一个barcode中第一个(数量最多)碱基名称复制到一个新的字典中,用于总控比对
                //foreach (var item in MyVariable.AutoAllJianJiDics)
                //{
                //    if (MyVariable.AutoAllJianJiDics[item.Key].Count != 0)
                //    {
                //        MyVariable.inferJianJiDic.Add(Convert.ToInt32(item.Key.Replace("barcode", "")), MyVariable.AutoAllJianJiDics[item.Key].First().Key);
                //    }
                //}

                MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                {
                    foreach (var item in MyVariable.AutoAllJianJiDics)
                    {
                        MainForm_Data.mMainForm_Data.cbx_barcode.Items.Add(item.Key);
                    }
                    if (MainForm_Data.mMainForm_Data.cbx_barcode.Items.Count != 0)
                    {
                        MainForm_Data.mMainForm_Data.cbx_barcode.SelectedIndex = 0;
                    }
                    TotalDNACounts = 36748;
                    MatchDNACounts = 9757;
                    MainForm_Data.mMainForm_Data.lab_totalDNA.Text = TotalDNACounts.ToString();
                    MainForm_Data.mMainForm_Data.lab_matchDNA.Text = MatchDNACounts.ToString();
                    MainForm_Data.mMainForm_Data.lblExecMsg.Text = $"执行成功！";
                    MainForm_Data.mMainForm_Data.lblExecMsg.BackColor = Color.White;
                    MainForm_Data.mMainForm_Data.lblExecMsg.Visible = false;
                }));
                IsProgress = false;
                SerializeClass.mChipDataReportingToControl.chipTotalCount = TotalDNACounts;
                SerializeClass.mChipDataReportingToControl.chipMatchCount = MatchJianJiCounts;
                return true;
            }
            catch (Exception ex)
            {
                IsProgress = false;
                return false;
            }
        }
        /// <summary>
        /// 递归查找文件夹及文件
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="folerLevel"></param>
        /// <returns></returns>
        List<ResultFolderVM> GetSubFolderLists(string basePath, int folerLevel)
        {
            var modelList = new List<ResultFolderVM>();
            foreach (var folderPath in Directory.GetDirectories(basePath))
            {
                var model = new ResultFolderVM()
                {
                    FolderLevel = folerLevel,
                    FolderPath = folderPath,
                    FolderName = new DirectoryInfo(folderPath).Name,
                    FileList = SeqkitHelper.GetFileList(folderPath),
                    SubFolderList = GetSubFolderLists(folderPath, folerLevel + 1)
                };
                TotalFileCount += model.FileList.Count();
                modelList.Add(model);
            }
            if (folerLevel > MaxFolderLevel)
            {
                MaxFolderLevel = folerLevel;
            }
            return modelList;
        }


        /// <summary>
        /// 递归匹配处理
        /// </summary>
        /// <param name="model"></param>
        void MatchDatas(ResultFolderVM model)
        {
            foreach (var item in model.FileList)
            {
                if (!item.MatchedTxtPath.Contains("pass") || !item.MatchedTxtPath.Contains("fastq") || !item.MatchedTxtPath.Contains("barcode"))
                {
                    continue;
                }
                SeqkitHelper.MatcheAsTxt(IDNA_string, item.FilePath, item.MatchedTxtPath);
                item.OriginalCount = SeqkitHelper.GetOriginalCount(item.FilePath);
                item.MatchedCount = SeqkitHelper.GetMatcheCount(IDNA_string, item.FilePath);
                MyVariable.AutoSingleJianJiDics = SeqkitHelper.SingleJianJiInfer(5, IDNA_string, item.MatchedTxtPath);
                if (!MyVariable.AutoAllJianJiDics.ContainsKey(item.FolderName))
                {
                    MyVariable.AutoAllJianJiDics.Add(item.FolderName, MyVariable.AutoSingleJianJiDics);
                }
                else
                {
                    //两个字典合并成一个
                    var combinedDic = MyVariable.AutoAllJianJiDics[item.FolderName]
                        .Concat(MyVariable.AutoSingleJianJiDics)
                        .GroupBy(kvp => kvp.Key)
                        .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));

                    //排序
                    var sortedElementCounts = combinedDic
                        .OrderByDescending(kvp => kvp.Value)
                        .ToList();
                    combinedDic = sortedElementCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    MyVariable.AutoAllJianJiDics[item.FolderName] = combinedDic;
                }
                TotalDNACounts = TotalDNACounts + item.OriginalCount;
                MatchDNACounts = MatchDNACounts + item.MatchedCount;
                MatchJianJiCounts = MatchJianJiCounts + item.DNAMatchedCount;
                MatchedFileCount++;
                MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                {
                    MainForm_Data.mMainForm_Data.lblExecMsg.Text = $"正在处理 {MatchedFileCount}/{TotalFileCount}";
                }));
                Thread.Sleep(50);
            }
            foreach (var item in model.SubFolderList)
            {
                MatchDatas(item);
            }
        }

        private void roundButton1_Click_1(object sender, EventArgs e)
        {
            //判断设备是否是停止中状态
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus != ObjectStation._StationStatus.Stop)
            {
                LogConfig.Instance.ShowMessageToList("Run", "设备状态不是停止中,无法切换模式!", MsgType.Success, Color.Red);
                return;
            }
            //判断机台是否是初始状态
            if (SerializeClass.mMemory.CarryStation_state != MemoryClass.CarryStation_State.空闲
             || SerializeClass.mMemory.FeedingStation_state != MemoryClass.FeedingStation_State.空闲
             || SerializeClass.mMemory.SequencingStation_state != MemoryClass.SequencingStation_State.空闲
             || SerializeClass.mMemory.DataProcessingStation_state != MemoryClass.DataProcessingStation_State.空闲
             || SerializeClass.mMemory.RobotStation_state != MemoryClass.RobotStation_State.空闲)
            {
                LogConfig.Instance.ShowMessageToList("Run", "当前设备有任务执行,无法切换模式!", MsgType.Success, Color.Red);
                return;
            }
            //判断样本载具是否放入设备
            if (!MyVariable.newshow_IsOpenOver)
            {
                if (IOConfig.Instance.InputsStatus[(Int32)_InputCollect.光电8联排试管区1] || IOConfig.Instance.InputsStatus[(Int32)_InputCollect.光电8联排试管区2])
                {
                    LogConfig.Instance.ShowMessageToList("Run", "样本区存在载具,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.离心管试管区光电2])
                {
                    LogConfig.Instance.ShowMessageToList("Run", "1.5试管载具区无载具,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.低温区光电])
                {
                    LogConfig.Instance.ShowMessageToList("Run", "低温载具区无载具,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区1光电2])
                {
                    LogConfig.Instance.ShowMessageToList("Run", "1000枪头载具区1无载具,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区2光电2])
                {
                    LogConfig.Instance.ShowMessageToList("Run", "1000枪头载具区2无载具,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区3光电2])
                {
                    LogConfig.Instance.ShowMessageToList("Run", "200枪头载具区无载具,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (!IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电1] || !IOConfig.Instance.InputsStatus[(Int32)_InputCollect.枪头区4光电2])
                {
                    LogConfig.Instance.ShowMessageToList("Run", "50枪头载具区无载具,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledPLC.ToString()].CurrentValue)))
                {
                    LogConfig.Instance.ShowMessageToList("Run", "PLC交互屏蔽中,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (Convert.ToBoolean(int.Parse(ParameConfig.Instance.SystemParameDic[_ParamName.ShiledMainControl.ToString()].CurrentValue)))
                {
                    LogConfig.Instance.ShowMessageToList("Run", "总控交互屏蔽中,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
                if (!Program.modbusTcp_PLC.Connect())
                {
                    LogConfig.Instance.ShowMessageToList("Run", "PLC连接失败,模式切换失败", MsgType.Success, Color.Red);
                    return;
                }
            }
            //开启参观模式
            if (!MyVariable.newshow_IsOpenOver)
            {
                MsgBox mb2 = new MsgBox(MsgBoxType.提示, BtnType.YesNo, false);
                mb2.MsgShowDialog("提示", "输入密码,确认是否开启参观模式");
                string btn2 = mb2.ret.SelectedBtn;
                string id2 = mb2.ret.RichText.Trim().Replace("\n", "");
                if (btn2 == "btn_A")
                {
                    if (id2 != "cy123")
                    {
                        MessageBox.Show("密码错误,模式切换失败");
                        return;
                    }
                    MyVariable.area_QiangTou1.num_Remain = 80;
                    MyVariable.area_QiangTou2.num_Remain = 80;
                    MyVariable.consumables_Empty[0] = false;
                    MyVariable.area_QiangTou3.num_Remain = 80;
                    MyVariable.consumables_Empty[1] = false;
                    MyVariable.area_QiangTou4.num_Remain = 80;
                    MyVariable.consumables_Empty[2] = false;
                    MyVariable.area_DiWen_FCT.num_Remain = MyVariable.FCT_MAX;
                    MyVariable.consumables_Empty[3] = false;
                    MyVariable.area_DiWen_FCF.num_Remain = MyVariable.FCF_MAX;
                    MyVariable.area_DiWen_SB.num_Remain = MyVariable.SB_MAX;
                    MyVariable.area_DiWen_LIB.num_Remain = MyVariable.LIB_MAX;
                    MyVariable.area_DiWen_DIL.num_Remain = MyVariable.DIL_MAX;
                    MyVariable.area_DiWen_WMX.num_Remain = MyVariable.WMX_MAX;
                    MyVariable.area_DiWen_S.num_Remain = MyVariable.S_MAX;
                    MyVariable.area_LiXinGuan.num_Remain = 18;
                    MyVariable.consumables_Empty[4] = false;


                    #region 初始化记忆
                    SerializeClass.mMemory.FeedingStation_state = MemoryClass.FeedingStation_State.空闲;
                    SerializeClass.mMemory.CarryStation_state = MemoryClass.CarryStation_State.空闲;
                    SerializeClass.mMemory.SequencingStation_state = MemoryClass.SequencingStation_State.空闲;
                    SerializeClass.mMemory.RobotStation_state = MemoryClass.RobotStation_State.空闲;
                    SerializeClass.mMemory.clamping_jaw_technology = MemoryClass.Clamping_jaw_technology.夹爪默认松开;
                    SerializeClass.mMemory.robotclaw_technology = MemoryClass.RobotClaw_technology.夹爪默认松开;
                    SerializeClass.mMemory.pipette_gun_technology = MemoryClass.Pipette_gun_technology.未取枪头;
                    SerializeClass.mMemory.area = MemoryClass.Area.枪头区1;
                    SerializeClass.mMemory.area_noout = MemoryClass.NoOutArea.NULL;
                    SerializeClass.mMemory.carrystation_working = MemoryClass.CarryStation_Working.开始工作;
                    SerializeClass.mMemory.DataProcessingStation_state = MemoryClass.DataProcessingStation_State.空闲;
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64600, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64601, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64614, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64615, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64616, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64622, 0);
                    Program.modbusTcp_PLC.WriteSingleRegister(1, 64634, 0);
                    MyVariable.b_StatusToControl = false;
                    MyVariable.need_Completed = false;
                    MyVariable.CCD_QiPao = false;
                    MyVariable.JianJiShiBie_Start = false;
                    MyVariable.newshow_step1 = false;
                    MyVariable.File_Copy.Clear();
                    MyVariable.show_memory = 0;
                    MyVariable.FunctionStep = 0;
                    LogConfig.Instance.ShowMessageToList("Run", "初始化记忆成功！", MsgType.Success, Color.Green);
                    #endregion

                    roundButton1.Text = "退出参观模式";
                    roundButton1.BaseColor = Color.MediumTurquoise;
                    roundButton1.BaseColorEnd = Color.MediumTurquoise;
                    rbt_Show.Visible = false;
                    //参观模式试剂用量缩减
                    MyVariable.ReadShowVolume();
                    //参观模式标志
                    MyVariable.newshow_IsOpen = true;
                    MyVariable.newshow_IsOpenOver = true;

                    IOConfig.Instance.SetSingleOut(_OutputCollect.三色灯红.ToString(), 0);
                    IOConfig.Instance.SetSingleOut(_OutputCollect.三色灯绿.ToString(), 0);
                    IOConfig.Instance.SetSingleOut(_OutputCollect.三色灯黄.ToString(), 0);

                    SoftWareForm.m_softwarmform.Invoke(new Action(() =>
                    {
                        SoftWareForm.m_softwarmform.lab_RunMode.Text = "流转参观模式";
                    }));
                    MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                    {
                        MainForm_Data.mMainForm_Data.label1.Visible = false;
                        MainForm_Data.mMainForm_Data.txt_FolderPath.Visible = false;
                        MainForm_Data.mMainForm_Data.lblCount.Visible = false;
                        MainForm_Data.mMainForm_Data.lab_ZongKongJJ.Visible = false;
                        MainForm_Data.mMainForm_Data.label5.Visible = false;
                        MainForm_Data.mMainForm_Data.txt_JianJiMsg.Visible = false;
                    }));
                    LogConfig.Instance.ShowMessageToList("Run", "流转参观模式已启用!", MsgType.Success, Color.Green);
                    Task.Run(() =>
                    {
                        if (!Directory.Exists(cexuFilePath))
                        {
                            Directory.CreateDirectory(cexuFilePath);
                        }
                        DataAnalysisMethods(cexuFilePath);
                    });
                }
            }
            //关闭参观模式
            else
            {
                roundButton1.Text = "流转参观模式";
                roundButton1.BaseColor = Color.Tomato;
                roundButton1.BaseColorEnd = Color.Tomato;
                rbt_Show.Visible = true;
                //退出参观模式,试剂用量恢复
                MyVariable.ReadPipetteParam();
                //关闭参观模式标志
                MyVariable.newshow_IsOpen = false;
                MyVariable.newshow_IsOpenOver = false;
                MyVariable.show_IsOpen = false;
                SoftWareForm.m_softwarmform.Invoke(new Action(() =>
                {
                    SoftWareForm.m_softwarmform.lab_RunMode.Text = "自动运行模式";
                }));
                MainForm_Data.mMainForm_Data.Invoke(new Action(() =>
                {
                    MainForm_Data.mMainForm_Data.label1.Visible = true;
                    MainForm_Data.mMainForm_Data.txt_FolderPath.Visible = true;
                    MainForm_Data.mMainForm_Data.lblCount.Visible = true;
                    MainForm_Data.mMainForm_Data.lab_ZongKongJJ.Visible = true;
                    MainForm_Data.mMainForm_Data.lblExecMsg.Visible = true;
                    MainForm_Data.mMainForm_Data.label5.Visible = true;
                    MainForm_Data.mMainForm_Data.txt_JianJiMsg.Visible = true;
                }));
                LogConfig.Instance.ShowMessageToList("Run", "流转参观模式已退出!", MsgType.Success, Color.Green);
            }
        }
    }
}
