using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class DowntimeDiscardForm : Form
    {

        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        private List<int> throwSelectRecord = new List<int>();//抛料类型选项记录
        private List<int> throwSelectQuery = new List<int>();//查询类型选项记录

        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        public DowntimeDiscardForm()
        {
            InitializeComponent();
        }

        private void DownTimeRecordForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;

            cmb_Day.SelectedIndex = 0;
            HashSet<string> ls = new HashSet<string>();
            foreach (var va in Discard.Instance.DicThrowType)
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    ls.Add(va.Value.ThrowNameCH);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    ls.Add(va.Value.ThrowNameEN);
                }
                else
                {
                    ls.Add(va.Value.ThrowNameVN);
                }
            }
            if (Discard.Instance.DicThrowType.Count > 0)
            {
                cmb_Select.Items.AddRange(ls.ToList().ToArray());
                cmb_Query.Items.AddRange(ls.ToList().ToArray());
                cmb_Query.SelectedIndexChanged += Cmb_Query_SelectedIndexChanged;
                cmb_Select.SelectedIndex = 0;
                cmb_Query.SelectedIndex = 0;
                throwSelectRecord.Add(cmb_Select.SelectedIndex);
                cmb_Select.SelectedIndexChanged += Cmb_Select_SelectedIndexChanged;
                string ss = cmb_Select.Text.Trim();
                ThrowType throwtp = Discard.Instance.DicThrowType.Values.ToList().Find(x => x.ThrowNameCH == ss || x.ThrowNameEN == ss ||
                    x.ThrowNameVN == ss);
                string transName = throwtp.ThrowNameCH;
                Discard.Instance.GetThrowMaterial(transName);
            }

            Discard.Instance.ThrowMaterialUpdateEvent += Instance_ThrowMaterialUpdateEvent;
            Discard.Instance.ThrowMaterialQueryEvent += Instance_ThrowMaterialQueryEvent;

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += Instance_ChangeLanguageHandle;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);

            mAutosize.ControlInitializeSize(this);
        }

        private void Instance_ChangeLanguageHandle(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);

            #region 按钮提示语
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Query, "抛料查询");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Query, "Material throwing query");
            }
            else
            {
                toolTip1.SetToolTip(btn_Query, "Ném yêu cầu");
            }
            #endregion

            /***抛料类型选项刷新***/
            cmb_Select.SelectedIndexChanged -= new EventHandler(Cmb_Select_SelectedIndexChanged);
            int index = throwSelectRecord.Last();
            cmb_Select.Items.Clear();
            HashSet<string> ls = new HashSet<string>();
            foreach (var va in Discard.Instance.DicThrowType)
            {
                switch (strLanguage)
                {
                    case "CH":
                        ls.Add(va.Value.ThrowNameCH);
                        break;
                    case "EN":
                        ls.Add(va.Value.ThrowNameEN);
                        break;
                    case "VN":
                        ls.Add(va.Value.ThrowNameVN);
                        break;
                }
            }
            if (Discard.Instance.DicThrowType.Count > 0)
            {
                cmb_Select.Items.AddRange(ls.ToList().ToArray());
            }
            cmb_Select.SelectedIndex = index;
            cmb_Select.SelectedIndexChanged += new EventHandler(Cmb_Select_SelectedIndexChanged);
            /***查询类型选项刷新***/
            cmb_Query.SelectedIndexChanged -= new EventHandler(Cmb_Query_SelectedIndexChanged);
            index = throwSelectQuery.Last();
            cmb_Query.Items.Clear();
            HashSet<string> ls2 = new HashSet<string>();
            foreach (var va in Discard.Instance.DicThrowType)
            {
                switch (strLanguage)
                {
                    case "CH":
                        ls2.Add(va.Value.ThrowNameCH);
                        break;
                    case "EN":
                        ls2.Add(va.Value.ThrowNameEN);
                        break;
                    case "VN":
                        ls2.Add(va.Value.ThrowNameVN);
                        break;
                }
            }
            if (Discard.Instance.DicThrowType.Count > 0)
            {
                cmb_Query.Items.AddRange(ls2.ToList().ToArray());
            }
            cmb_Query.SelectedIndex = index;
            cmb_Query.SelectedIndexChanged += new EventHandler(Cmb_Query_SelectedIndexChanged);
        }

        private void Cmb_Query_SelectedIndexChanged(object sender, EventArgs e)
        {
            throwSelectQuery.Add(cmb_Query.SelectedIndex);
        }

        private void Cmb_Select_SelectedIndexChanged(object sender, EventArgs e)
        {
            throwSelectRecord.Add(cmb_Select.SelectedIndex);
            string ss = cmb_Select.Text.Trim();
            ThrowType throwtp = Discard.Instance.DicThrowType.Values.ToList().Find(x => x.ThrowNameCH == ss || x.ThrowNameEN == ss ||
                x.ThrowNameVN == ss);
            string transName = throwtp.ThrowNameCH;
            Discard.Instance.GetThrowMaterial(transName);
        }

        private void Instance_ThrowMaterialQueryEvent(object sender, Dictionary<string, int> e)
        {
            this.Invoke(new Action(() =>
            {
                chart_Query.Series[0].Points.DataBindXY(e.Keys.ToList(), e.Values.ToList());
            }));
        }

        private void Instance_ThrowMaterialUpdateEvent(string name, Dictionary<string, int> dc)
        {
            this.Invoke(new Action(() =>
            {
                string ss = cmb_Select.Text.Trim();
                ThrowType throwtp = Discard.Instance.DicThrowType.Values.ToList().Find(x => x.ThrowNameCH == ss || x.ThrowNameEN == ss ||
                          x.ThrowNameVN == ss);
                string transName = throwtp.ThrowNameCH;
                if (transName == name)
                {
                    chart_Show.Series[0].Points.DataBindXY(dc.Keys.ToList(), dc.Values.ToList());
                }
            }));
        }

        private void DownTimeRecordForm_SizeChanged(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }

        private void btn_Query_Click(object sender, EventArgs e)
        {
            string ss = cmb_Query.Text.Trim();
            ThrowType throwtp = Discard.Instance.DicThrowType.Values.ToList().Find(x => x.ThrowNameCH == ss || x.ThrowNameEN == ss ||
                x.ThrowNameVN == ss);
            string transName = throwtp.ThrowNameCH;
            Discard.Instance.QueryThrowMaterial(transName, dtp_Date.Value, cmb_Day.Text.Trim());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Discard.Instance.UpdateThrowMaterial("飞达1抛料");
            Discard.Instance.UpdateThrowMaterial("飞达2抛料");
            Discard.Instance.UpdateThrowMaterial("飞达3抛料");
            Discard.Instance.UpdateThrowMaterial("飞达4抛料");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dic = Discard.Instance.GetThrowMaterialDic("飞达2抛料");
        }
    }
}
