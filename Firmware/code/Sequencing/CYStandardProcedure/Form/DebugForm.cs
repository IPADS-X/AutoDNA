using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CYAutoFramework;
using System.IO;
using CYCustomControl;

namespace CYStandardProcedure
{
    public partial class DebugForm : Form
    {
     
        private AutoSizeMDIForm mAutoSize = new AutoSizeMDIForm();

        private string mPath = AppDomain.CurrentDomain.BaseDirectory + @"\UIImage\DebugIcon\";
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
        private ToolTip toolTip1= new ToolTip();

        public DebugForm()
        {
            InitializeComponent();
        }

        private void DebugForm_Load(object sender, EventArgs e)
        {
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
            mBtnIndexDic.Add(rbt_Input, 0);
            mBtnIndexDic.Add(rbt_Output, 2);
            mBtnIndexDic.Add(rbt_OutputDebug, 4);
            mBtnIndexDic.Add(rbt_Parameter, 6);
            mBtnIndexDic.Add(rbt_Motor, 8);
            mBtnIndexDic.Add(rbt_AxisDebug, 10);
            mBtnIndexDic.Add(rbt_Net, 12);
            mBtnIndexDic.Add(rbt_Server, 14);
            mBtnIndexDic.Add(rbt_Serial, 16);
            mBtnIndexDic.Add(rbt_Robot, 18);
            mBtnIndexDic.Add(rbt_Mes, 20);
            mBtnIndexDic.Add(rbt_Silo, 22);
            mBtnIndexDic.Add(rbt_Hive, 24);
            mBtnIndexDic.Add(rbt_carryClaw, 26);
            mBtnIndexDic.Add(rbt_Sequence, 28);
            mBtnIndexDic.Add(rbt_YiYeQiang, 30);
            mBtnIndexDic.Add(rbt_robotClaw, 32);
            mBtnIndexDic.Add(rbt_DataAnalysis, 34);
            mBtnIndexDic.Add(rbt_BaseChart, 36);
            mBtnIndexDic.Add(rbt_RobNewClaw, 38);
            /***按钮赋图片***/
            rbt_Input.BackgroundImage = mImageList[0];
            rbt_Output.BackgroundImage = mImageList[2];
            rbt_OutputDebug.BackgroundImage = mImageList[4];
            rbt_Parameter.BackgroundImage = mImageList[6];
            rbt_Motor.BackgroundImage = mImageList[8];
            rbt_AxisDebug.BackgroundImage = mImageList[10];
            rbt_Net.BackgroundImage = mImageList[12];
            rbt_Server.BackgroundImage = mImageList[14];
            rbt_Serial.BackgroundImage = mImageList[16];
            rbt_Robot.BackgroundImage = mImageList[18];
            rbt_Mes.BackgroundImage = mImageList[20];
            rbt_Silo.BackgroundImage = mImageList[22];
            rbt_Hive.BackgroundImage = mImageList[24];
            rbt_carryClaw.BackgroundImage = mImageList[26];
            rbt_Sequence.BackgroundImage = mImageList[28];
            rbt_YiYeQiang.BackgroundImage = mImageList[30];
            rbt_robotClaw.BackgroundImage = mImageList[32];
            rbt_DataAnalysis.BackgroundImage = mImageList[34];
            rbt_BaseChart.BackgroundImage = mImageList[36];
            rbt_RobNewClaw.BackgroundImage = mImageList[38];
            /***按钮和窗体绑定字典***/
            mFormDic.Add(rbt_Input, new DIForm());
            mFormDic.Add(rbt_Output, new DOForm());
            mFormDic.Add(rbt_OutputDebug, new DOHandleForm());
            mFormDic.Add(rbt_Parameter, new RunParameForm());
            mFormDic.Add(rbt_Motor, new AxisParameForm());
            mFormDic.Add(rbt_AxisDebug, new AxisDebugForm());
            mFormDic.Add(rbt_Net, new NetSetForm());
            mFormDic.Add(rbt_Server, new ServerForm());
            mFormDic.Add(rbt_Serial, new SerialSetForm());
            //mFormDic.Add(rbt_Robot, new InovanceRobotForm());
            mFormDic.Add(rbt_Robot, new ToshibalRobotForm());
            mFormDic.Add(rbt_Mes, new ShopFloorForm());
            mFormDic.Add(rbt_Silo, new MaterialsForm());
            mFormDic.Add(rbt_Hive, new HiveForm());
            mFormDic.Add(rbt_Sequence, new SequencingForm());
            mFormDic.Add(rbt_YiYeQiang, new PipetteGunForm());
            mFormDic.Add(rbt_DataAnalysis, new SeqkitForm());
            mFormDic.Add(rbt_BaseChart, new BaseChartForm());
            mFormDic.Add(rbt_RobNewClaw, new RobotNewClawForm());
            if (SoftWareForm.carryclaw_initialize==null)
            {
                SoftWareForm.carryclaw_initialize = new CarryClawForm();
                mFormDic.Add(rbt_carryClaw, SoftWareForm.carryclaw_initialize);
            }
            else
            {
                mFormDic.Add(rbt_carryClaw, SoftWareForm.carryclaw_initialize);
            }
            if (SoftWareForm.robotclaw_initialize == null)
            {
                SoftWareForm.robotclaw_initialize = new RobotClawForm();
                mFormDic.Add(rbt_robotClaw, SoftWareForm.robotclaw_initialize);
            }
            else
            {
                mFormDic.Add(rbt_robotClaw, SoftWareForm.robotclaw_initialize);
            }

            rbt_Input.PerformClick();
            /***控价大小自适应***/
            mAutoSize.controllInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += DebugForm_LanguageChangeEvent; 
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void DebugForm_LanguageChangeEvent(string strLanguage)
        {
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(rbt_Input, "输入信号监控");
                toolTip1.SetToolTip(rbt_Output, "输出信号监控");
                toolTip1.SetToolTip(rbt_OutputDebug, "输出调试");
                toolTip1.SetToolTip(rbt_Parameter, "运行参数");
                toolTip1.SetToolTip(rbt_Motor, "电机配置");
                toolTip1.SetToolTip(rbt_AxisDebug, "电机调试");
                toolTip1.SetToolTip(rbt_Net, "客户端测试");
                toolTip1.SetToolTip(rbt_Server, "服务器测试");
                toolTip1.SetToolTip(rbt_Serial, "串口测试");
                toolTip1.SetToolTip(rbt_Robot, "机器人配置");
                toolTip1.SetToolTip(rbt_Silo, "弹仓配置");
                toolTip1.SetToolTip(rbt_Mes, "ShopFloor测试");
                toolTip1.SetToolTip(rbt_Hive, "Hive测试");
                toolTip1.SetToolTip(rbt_carryClaw, "搬运电动夹爪调试");
                toolTip1.SetToolTip(rbt_Sequence, "测序仪测试");
                toolTip1.SetToolTip(rbt_YiYeQiang, "移液枪测试");
                toolTip1.SetToolTip(rbt_robotClaw, "机器人电动夹爪调试");
                toolTip1.SetToolTip(rbt_DataAnalysis, "测序文件解析");
                toolTip1.SetToolTip(rbt_BaseChart, "碱基查询");
                toolTip1.SetToolTip(rbt_RobNewClaw, "机器人电动夹爪调试");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(rbt_Input, "InputSignal Monitoring");
                toolTip1.SetToolTip(rbt_Output, "OutputSignal Monitoring");
                toolTip1.SetToolTip(rbt_OutputDebug, "Output Debugging");
                toolTip1.SetToolTip(rbt_Parameter, "Operating Parameters");
                toolTip1.SetToolTip(rbt_Motor, "Motor Configuration");
                toolTip1.SetToolTip(rbt_AxisDebug, "Motor Debugging");
                toolTip1.SetToolTip(rbt_Net, "Client Test");
                toolTip1.SetToolTip(rbt_Server, "Server Test");
                toolTip1.SetToolTip(rbt_Serial, "Serial Test");
                toolTip1.SetToolTip(rbt_Robot, "Robot Configuration");
                toolTip1.SetToolTip(rbt_Silo, "Magazine Configuration");
                toolTip1.SetToolTip(rbt_Mes, "ShopFloor Test");
                toolTip1.SetToolTip(rbt_Hive, "Hive Test");
                toolTip1.SetToolTip(rbt_carryClaw, "电动夹爪调试");
                toolTip1.SetToolTip(rbt_Sequence, "测序仪测试");
                toolTip1.SetToolTip(rbt_YiYeQiang, "移液枪测试");
                toolTip1.SetToolTip(rbt_robotClaw, "机器人电动夹爪调试");
                toolTip1.SetToolTip(rbt_DataAnalysis, "测序文件解析");
                toolTip1.SetToolTip(rbt_BaseChart, "碱基查询");
                toolTip1.SetToolTip(rbt_RobNewClaw, "机器人电动夹爪调试");
            }
            else
            {
                toolTip1.SetToolTip(rbt_Input, "Kiểm tra tín hiệu nhập");
                toolTip1.SetToolTip(rbt_Output, "Kiểm tra tín hiệu xuất");
                toolTip1.SetToolTip(rbt_OutputDebug, "Xuất gỡ lỗi");
                toolTip1.SetToolTip(rbt_Parameter, "Tham số hoạt động");
                toolTip1.SetToolTip(rbt_Motor, "Cấu hình động cơ");
                toolTip1.SetToolTip(rbt_AxisDebug, "Khởi động cơ");
                toolTip1.SetToolTip(rbt_Net, "Khách hàng");
                toolTip1.SetToolTip(rbt_Server, "người phục vụ");
                toolTip1.SetToolTip(rbt_Serial, "Kiểm tra vòng lặp");
                toolTip1.SetToolTip(rbt_Robot, "Cấu hình Robot");
                toolTip1.SetToolTip(rbt_Silo, "Cấu hình tạp chí");
                toolTip1.SetToolTip(rbt_Mes, "Thử thách MES");
                toolTip1.SetToolTip(rbt_Hive, "Thử thách Hive");
                toolTip1.SetToolTip(rbt_carryClaw, "电动夹爪调试");
                toolTip1.SetToolTip(rbt_Sequence, "测序仪测试");
                toolTip1.SetToolTip(rbt_YiYeQiang, "移液枪测试");
                toolTip1.SetToolTip(rbt_robotClaw, "机器人电动夹爪调试");
                toolTip1.SetToolTip(rbt_DataAnalysis, "测序文件解析");
                toolTip1.SetToolTip(rbt_BaseChart, "碱基查询");
                toolTip1.SetToolTip(rbt_RobNewClaw, "机器人电动夹爪调试");
            }
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

        private void rbt_Input_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Input);
        }

        private void rbt_Output_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Output);
        }

        private void rbt_OutputDebug_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_OutputDebug);
        }

        private void rbt_Parameter_Click(object sender, EventArgs e)
        {
            //if (AdminConfig.Instance.UserLevel == 0)
            //{
            //    MessageBox.Show("用户权限等级不够！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //if (AdminConfig.Instance.CurEnterEmployee.Name == string.Empty)
            //{
            //    MessageBox.Show("登录者未刷ID卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            SwitchWnd(rbt_Parameter);
        }

        private void rbt_Motor_Click(object sender, EventArgs e)
        {
            //if (AdminConfig.Instance.UserLevel == 0)
            //{
            //    MessageBox.Show("用户权限等级不够！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //if (AdminConfig.Instance.CurEnterEmployee.Name == string.Empty)
            //{
            //    MessageBox.Show("登录者未刷ID卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            SwitchWnd(rbt_Motor);
        }

        private void rbt_AxisDebug_Click(object sender, EventArgs e)
        {
            //if (AdminConfig.Instance.UserLevel == 0)
            //{
            //    MessageBox.Show("用户权限等级不够！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //if (AdminConfig.Instance.CurEnterEmployee.Name == string.Empty)
            //{
            //    MessageBox.Show("登录者未刷ID卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            SwitchWnd(rbt_AxisDebug);
        }

        private void rbt_Net_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Net);
        }

        private void rbt_Mes_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Mes);
        }

        private void rbt_Silo_Click(object sender, EventArgs e)
        {
            //SwitchWnd(rbt_Silo);
        }

        private void rbt_Serial_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Serial);
        }

        private void rbt_Robot_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Robot);
        }

        private void DebugForm_SizeChanged(object sender, EventArgs e)
        {
            mAutoSize.controlAutoSize(this);
        }

        private void rbt_Server_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Server);
        }

        private void rbt_Hive_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Hive);
        }

        private void rbt_Claw_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_carryClaw);
        }

        private void rbt_Sequence_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_Sequence);
        }

        private void rbt_YiYeQiang_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_YiYeQiang);
        }

        private void rbt_robotClaw_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_robotClaw);
        }

        private void rbt_DataAnalysis_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_DataAnalysis);
        }

        private void rbt_BaseChart_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_BaseChart);
        }

        private void rbt_RobNewClaw_Click(object sender, EventArgs e)
        {
            SwitchWnd(rbt_RobNewClaw);
        }
    }
}
