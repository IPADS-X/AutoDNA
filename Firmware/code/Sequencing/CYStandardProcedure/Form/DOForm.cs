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
using CYCustomControl;

namespace CYStandardProcedure
{
    public partial class DOForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        #region 窗体控件自适应代码         
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        #endregion
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        /***设备总输入点***/
        private int mTotalOutput;
        /***输入点页面数(48个点为一页)***/
        private int mTotalPage;
        /***当前页面***/
        private int mCurPage;
        /***页面与输入点数的字典***/
        private Dictionary<int, int> PageDic = new Dictionary<int, int>();
        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();
        /***语言索引***/
        private int mIndex;

        public DOForm()
        {
            InitializeComponent();
        }

        private void DOForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            if (LanguageConfig.Instance.Language == "CH")
            {
                toolTip1.SetToolTip(btn_Prev, "上一页");
                toolTip1.SetToolTip(btn_Next, "下一页");
            }
            else if (LanguageConfig.Instance.Language == "EN")
            {
                toolTip1.SetToolTip(btn_Prev, "Previous Page");
                toolTip1.SetToolTip(btn_Next, "Next Page");
            }
            else
            {
                toolTip1.SetToolTip(btn_Prev, "trang trước");
                toolTip1.SetToolTip(btn_Next, "trang kế");
            }
            timer1.Interval = 200;
            timer1.Enabled = true;
            /***得到总输入点个数***/
            mTotalOutput = IOConfig.Instance.OutputNames[0].Count;
            int modvalue = 0;
            modvalue = mTotalOutput % 48;
            /***得到输入点页数***/
            if (mTotalOutput > 48)
            {
                if (modvalue == 0)
                {
                    mTotalPage = mTotalOutput / 48;
                }
                else
                {
                    mTotalPage = (mTotalOutput - modvalue) / 48 + 1;
                }
            }
            else
            {
                mTotalPage = 1;
            }
            mCurPage = 1;
            for (int i = 1; i < mTotalPage + 1; i++)
            {
                if (i == mTotalPage)
                {
                    if (modvalue == 0)
                    {
                        PageDic.Add(i, 48);
                    }
                    else
                    {
                        PageDic.Add(i, modvalue);
                    }
                }
                else
                {
                    PageDic.Add(i, 48);
                }
            }
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += DOForm_LanguageChangeEvent; ;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void DOForm_LanguageChangeEvent(string strLanguage)
        {
            if(strLanguage=="CH")
            {
                mIndex = 0;
                toolTip1.SetToolTip(btn_Prev, "上一页");
                toolTip1.SetToolTip(btn_Next, "下一页");
            }
            else if(strLanguage=="EN")
            {
                mIndex = 1;
                toolTip1.SetToolTip(btn_Prev, "Previous Page");
                toolTip1.SetToolTip(btn_Next, "Next Page");
            }
            else
            {
                mIndex = 2;
                toolTip1.SetToolTip(btn_Prev, "trang trước");
                toolTip1.SetToolTip(btn_Next, "trang kế");
            }
            btn_Prev_Click(null, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (mIndex == 0)
                {
                    lab_Page.Text = string.Format("第{0}页", mCurPage);
                }
                else if (mIndex == 1)
                {
                    lab_Page.Text = string.Format("The {0} Page", mCurPage);
                }
                else
                {
                    lab_Page.Text = string.Format("{0} số trang", mCurPage);
                }
                foreach (Control con in this.Controls)
                {
                    if (PageDic[mCurPage] == 48)
                    {
                        if (con.Name.Contains("label"))
                        {
                            (con as Label).Image = IOConfig.Instance.OutputStatus[int.Parse(con.Name.Substring(5)) + (mCurPage - 1) * 48] ? imageList1.Images[1] : imageList1.Images[0];
                        }
                    }
                    else
                    {
                        if (con.Name.Contains("label"))
                        {
                            if (int.Parse(con.Name.Substring(5)) < PageDic[mCurPage])
                            {
                                (con as Label).Image = IOConfig.Instance.OutputStatus[int.Parse(con.Name.Substring(5)) + (mCurPage - 1) * 48] ? imageList1.Images[1] : imageList1.Images[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void btn_Prev_Click(object sender, EventArgs e)
        {
            if (mCurPage != 1)
            {
                mCurPage--;
            }
            foreach (Control con in this.Controls)
            {
                if (PageDic[mCurPage] == 48)
                {
                    if (con.Name.Contains("label"))
                    {
                        con.Enabled = true;
                        con.Visible = true;
                        con.Text = IOConfig.Instance.OutputNames[mIndex][int.Parse(con.Name.Substring(5)) + (mCurPage - 1) * 48];
                    }
                }
                else
                {
                    if (con.Name.Contains("label"))
                    {
                        if (int.Parse(con.Name.Substring(5)) >= PageDic[mCurPage])
                        {
                            con.Enabled = false;
                            con.Visible = false;
                        }
                        else
                        {
                            con.Enabled = true;
                            con.Visible = true;
                            con.Text = IOConfig.Instance.OutputNames[mIndex][int.Parse(con.Name.Substring(5)) + (mCurPage - 1) * 48];
                        }
                    }
                }
            }
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (mCurPage != mTotalPage)
            {
                mCurPage++;
            }
            foreach (Control con in this.Controls)
            {
                if (PageDic[mCurPage] == 48)
                {
                    if (con.Name.Contains("label"))
                    {
                        con.Enabled = true;
                        con.Visible = true;
                        con.Text = IOConfig.Instance.OutputNames[mIndex][int.Parse(con.Name.Substring(5)) + (mCurPage - 1) * 48];
                    }
                }
                else
                {
                    if (con.Name.Contains("label"))
                    {
                        if (int.Parse(con.Name.Substring(5)) >= PageDic[mCurPage])
                        {
                            con.Enabled = false;
                            con.Visible = false;
                        }
                        else
                        {
                            con.Enabled = true;
                            con.Visible = true;
                            con.Text = IOConfig.Instance.OutputNames[mIndex][int.Parse(con.Name.Substring(5)) + (mCurPage - 1) * 48];
                        }
                    }
                }
            }
        }
    }
}
