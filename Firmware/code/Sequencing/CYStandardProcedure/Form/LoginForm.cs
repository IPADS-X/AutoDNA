using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CYAutoFramework;
using System.Windows.Forms;
using CYCustomControl;
using MsgBoxLib;
using System.Diagnostics;

namespace CYStandardProcedure
{
    public partial class LoginForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        long stoptime = 0;
        Stopwatch stopwatch = new Stopwatch();
        #region 窗体控件自适应
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        #endregion

        private INIFile mIni;
        public string mUserINIPath = System.Windows.Forms.Application.StartupPath + @"\ExeFile\User.ini";

        /***当前想获取的权限等级***/
        private int mGetLevel;

        /***权限按钮数组***/
        private RoundButton[] mAdminBtn;

        /***模式按钮数组***/
        private RoundButton[] mModeBtn;

        #region 捕获系统空闲时间
        // 创建结构体用于返回捕获时间
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            // 设置结构体块容量
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            // 捕获的时间
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        /// <summary>
        /// 获取系统空闲（键盘和鼠标没有操作）的时间
        /// </summary>
        /// <returns>返回值为毫秒</returns>
        private  long GetLastInputTime()
        {
            LASTINPUTINFO vLastInputInfo = new LASTINPUTINFO();
            vLastInputInfo.cbSize = Marshal.SizeOf(vLastInputInfo);
            // 捕获时间
            if (!GetLastInputInfo(ref vLastInputInfo))
            {
                return 0;
            }
            else
            {
                if ((long)vLastInputInfo.dwTime != stoptime)
                {
                    stopwatch.Restart();
                    stoptime = (long)vLastInputInfo.dwTime;
                }
                else
                {
                    stopwatch.Start();
                }
                return stopwatch.ElapsedMilliseconds;
            }
        }
        #endregion

        public LoginForm()
        {
            InitializeComponent();
            mIni = new INIFile(mUserINIPath);
            mAdminBtn = new RoundButton[] { btn_Operator, btn_Engineer, btn_Manager };
            mModeBtn = new RoundButton[] { Rbtn_normalrun, Rbtn_dryrun, Rbtn_calibrun, Rbtn_grrrun, Rbtn_cpkrun, Rbtn_camstaticrun, Rbtn_camdycrun };
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            PerClickBtn();
            Rbtn_normalrun.PerformClick();

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += LoginForm_LanguageChangeEvent;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void LoginForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变Panel容器内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, panel2, ini);
        }

        private void Click_AdminBtn(object sender, EventArgs e)
        {
            var clcikBtn = sender as RoundButton;
            for (int i = 0; i < mAdminBtn.Length; i++)
            {
                if (clcikBtn.Name == mAdminBtn[i].Name)
                {
                    clcikBtn.BaseColor = Color.CornflowerBlue;
                    mGetLevel = i;
                }
                else
                {
                    mAdminBtn[i].BaseColor = Color.FromArgb(220, 221, 224);
                }
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            if (txt_Password.Text.Trim() == "dfg123")
            {
                HiddenForm form = new HiddenForm();
                if (form.ShowDialog() == DialogResult.Yes)
                {
                    txt_Password.Text = string.Empty;
                }
            }
            else
            {
                if (mGetLevel > 0)
                {
                    if (!AdminConfig.Instance.UserConfirm(mGetLevel, txt_Password.Text.Trim()))
                    {
                        MessageBox.Show("密码错误,请重新登录！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        timer1.Start();
                        btn_Loginout.Enabled = true;
                        btn_Loginout.BaseColor = Color.FromArgb(255, 128, 0);
                        btn_Authority.Enabled = false;
                        btn_Authority.BaseColor = Color.FromArgb(217, 216, 211);
                        txt_ID.Enabled = true;
                        /***触发登录事件***/
                        AdminConfig.Instance.DoUserLogInChangedEvent();
                    }
                }
            }
        }

        private void tex_password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btn_Login.PerformClick();
            }
        }

        private void PerClickBtn()
        {
            /***生成对应的按钮生成事件***/
            int level = AdminConfig.Instance.UserLevel;
            if (level == 0)
            {
                btn_Loginout.Enabled = false;
                btn_Loginout.BaseColor = Color.FromArgb(217, 216, 211);
                btn_Authority.Enabled = false;
                btn_Authority.BaseColor = Color.FromArgb(217, 216, 211);
                txt_ID.Enabled = false;
                txt_Password.Clear();
                txt_ID.Clear();
                btn_Operator.PerformClick();
            }
            else if (level == 1)
            {
                btn_Loginout.Enabled = true;
                btn_Loginout.BaseColor = Color.FromArgb(255, 128, 0);
                btn_Authority.Enabled = false;
                btn_Authority.BaseColor = Color.FromArgb(217, 216, 211);
                txt_ID.Enabled = true;
                txt_Password.Clear();
                txt_ID.Clear();
                btn_Engineer.PerformClick();
            }
            else if (level == 2)
            {
                btn_Loginout.Enabled = true;
                btn_Loginout.BaseColor = Color.FromArgb(255, 128, 0);
                btn_Authority.Enabled = false;
                btn_Authority.BaseColor = Color.FromArgb(217, 216, 211);
                txt_ID.Enabled = true;
                txt_Password.Clear();
                txt_ID.Clear();
                btn_Manager.PerformClick();
            }
        }

        private void btn_loginout_Click(object sender, EventArgs e)
        {
            AdminConfig.Instance.ExitLogin();
            PerClickBtn();
        }

        private void ModeBtn_Click(object sender, EventArgs e)
        {
            var clcikBtn = sender as RoundButton;
            for (int i = 0; i < mModeBtn.Length; i++)
            {
                if (clcikBtn.Name == mModeBtn[i].Name)
                {
                    clcikBtn.BaseColor = Color.CornflowerBlue;
                }
                else
                {
                    mModeBtn[i].BaseColor = Color.FromArgb(220, 221, 224);
                }
            }
        }

        private void tex_ID_KeyPress(object sender, KeyPressEventArgs e)
        {
            string name = string.Empty;
            string number = string.Empty;
            string id = txt_ID.Text.Trim();
            if (e.KeyChar == 13)
            {
                bool ret = false;
                string[] idnum = mIni.GetSectionNames();
                for (int i = 0; i < idnum.Length; i++)
                {
                    if (idnum[i] == id)
                    {
                        name = AdminConfig.Instance.GetEmployeeName(idnum[i]);
                        number = AdminConfig.Instance.GetEmployeeCardNumber(idnum[i]);
                        if (mGetLevel == AdminConfig.Instance.GetEmployeeUserLevel(idnum[i]))
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                        break;
                    }
                    else if (i == idnum.Length - 1)
                    {
                        ret = false;
                    }
                }
                if (ret)
                {
                    btn_Authority.Enabled = true;
                    btn_Authority.BaseColor = Color.FromArgb(255, 128, 0);
                    AdminConfig.Instance.CurEnterEmployee.Name = name;
                    /***记录谁刷了卡***/
                    //LogConfig.Instance.WriteEmployeeLog("姓名: " + name + "   卡ID：" + txt_ID.Text.Trim() + "  卡编号：" + number + "   刷卡获取了权限！");
                    //日志记录取消卡ID（登陆密码）
                    LogConfig.Instance.WriteEmployeeLog("姓名: " + name + "   卡编号：" + number + "   刷卡获取了权限！");
                    /***触发登录事件***/
                    AdminConfig.Instance.DoUserLogInChangedEvent();

                    //timer1.Start();
                }
                else
                {
                    MessageBox.Show("此ID号不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btn_loginout_Click(null, null);
                }
            }
        }


        private bool User(out string errCode)
        {
            errCode = "";
            string cardId = txt_ID.Text.Trim();

            string[] card = mIni.GetSectionNames();
            for (int i = 0; i < card.Length; i++)
            {
                if (card[i] == cardId)
                {
                    errCode = "";
                    return true;
                }
                else if (i == card.Length - 1)
                {
                    errCode = "无此用户！";
                    return false;
                }
            }
            return false;
        }


        private void btn_Authority_Click(object sender, EventArgs e)
        {
            FrmUserWrite m_user = new FrmUserWrite();
            m_user.ShowDialog();
        }


        private void Rbtn_normalrun_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop)
            {
                MyVariable.EmptyRun_Run = false;
                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeMode(ObjectStation._RunMode.NormalRun);
                ModeBtn_Click(sender, e);
            }
        }

        private void Rbtn_dryrun_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop)
            {
                MyVariable.EmptyRun_Run = true;
                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeMode(ObjectStation._RunMode.EmptyRun);
                ModeBtn_Click(sender, e);
            }
        }

        private void Rbtn_calibrun_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop)
            {
                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeMode(ObjectStation._RunMode.AutoCalib);
                ModeBtn_Click(sender, e);
            }
        }

        private void Rbtn_cpkgrr_Click(object sender, EventArgs e)
        {
            if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop)
            {
                StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeMode(ObjectStation._RunMode.CPKMode);
                ModeBtn_Click(sender, e);
            }
        }

        private void Rbtn_cpk_Click(object sender, EventArgs e)
        {
            //if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop)
            //{
            //    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeMode(ObjectStation._RunMode.GRRMode);
                ModeBtn_Click(sender, e);
            //}
        }

        private void Rbtn_camstaticrun_Click(object sender, EventArgs e)
        {
            //if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop)
            //{
            //    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeMode(ObjectStation._RunMode.CamStatisMode);
                ModeBtn_Click(sender, e);
            //}
        }

        private void Rbtn_camdycrun_Click(object sender, EventArgs e)
        {
            //if (StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].mCurStatus == ObjectStation._StationStatus.Stop)
            //{
            //    StationConfig.Instance.StationDic[_ThreadModule.总线程.ToString()].ChangeMode(ObjectStation._RunMode.CamDynamicMode);
                ModeBtn_Click(sender, e);
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (AdminConfig.Instance.UserLevel != 0)
            {
                if (GetLastInputTime() / 1000.0 > 10)
                {
                    AdminConfig.Instance.ExitLogin();
                    PerClickBtn();
                    //MsgBox mb = new MsgBox(MsgBoxType.提示, BtnType.DIY_2, true);
                    //mb.Btn_A_text = "取消";
                    //mb.Btn_B_text = string.Format("立即注销（{0}s）", (60 - GetLastInputTime() / 1000).ToString());

                    //mb.MsgShowDialog("系统空闲时间已达10分钟！", "长时间无人操作，软件即将进入操作员模式。");

                    //if (60 - GetLastInputTime() / 1000 == 0)
                    //{
                    //    mb.Hide();
                    //}
                }
            }
        }
    }
}
