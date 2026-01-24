using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CYCustomControl;
using System.Threading;
using CYAutoFramework;

namespace CYStandardProcedure
{
    public partial class CapacityForm : Form
    {
        private AutoSizeMDIForm mAutosize = new AutoSizeMDIForm();

        /***按钮和窗体字典***/
        private Dictionary<ToolStripButton, Form> mFormDic = new Dictionary<ToolStripButton, Form>();
        private Dictionary<string, List<Image>> mBtnDic = new Dictionary<string, List<Image>>();
        /***当前窗体***/
        private Form mCurForm;
        /***当前按钮***/
        private ToolStripButton mCurBtn;


        public CapacityForm()
        {
            InitializeComponent();
        }

        public void SwitchWnd(ToolStripButton btn)
        {
            if (mCurBtn != btn)
            {
                btn.Image = mBtnDic[btn.Name][1];
                foreach (ToolStripButton va in toolStrip1.Items)
                {
                    if (va.Name != btn.Name)
                    {
                        va.Image = mBtnDic[va.Name][0];
                    }
                }
                mCurBtn = btn;
                if (mCurForm != null)
                {
                    mCurForm.Hide();
                }
                if (mCurForm != mFormDic[btn])
                {
                    mCurForm = mFormDic[btn];
                    mCurForm.TopLevel = false;
                    mCurForm.Parent = panel1;
                    mCurForm.Dock = DockStyle.Fill;
                    mCurForm.Show();
                }
            }
        }

        private void CapacityForm_Load(object sender, EventArgs e)
        {
            foreach (ToolStripButton va in toolStrip1.Items)
            {
                List<Image> ls = new List<Image>();
                ls.Clear();
                if (va is ToolStripButton)
                {
                    switch (va.Name)
                    {
                        case "btn_yieldStatics":
                            ls.Add(Properties.Resources.产能统计未选中);
                            ls.Add(Properties.Resources.产能统计选中);
                            break;
                        case "btn_yieldQuery":
                            ls.Add(Properties.Resources.产能查询未选中);
                            ls.Add(Properties.Resources.产能查询选中);
                            break;
                    }
                    mBtnDic.Add(va.Name, ls);
                }
            }
            /***按钮和窗体绑定字典***/
            mFormDic.Add(btn_yieldStatics, new YieldStatisticsForm());
            mFormDic.Add(btn_yieldQuery, new YieldQueryForm());
            btn_yieldStatics.PerformClick();
            mAutosize.controllInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += Instance_ChangeLanguageHandle;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void Instance_ChangeLanguageHandle(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
        }

        private void ErrorForm_SizeChanged(object sender, EventArgs e)
        {
            mAutosize.controlAutoSize(this);
        }

        private void btn_dtRecord_Click(object sender, EventArgs e)
        {
            SwitchWnd(btn_yieldStatics);
        }

        private void btn_dtTimeStatis_Click(object sender, EventArgs e)
        {
            SwitchWnd(btn_yieldQuery);
        }
    }
}
