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
    public partial class DowntimeStatisticsForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();

        public DowntimeStatisticsForm()
        {
            InitializeComponent();
        }

        private void DowntimeStatisticsForm_Load(object sender, EventArgs e)
        {
            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += Instance_ChangeLanguageHandle;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);

            mAutosize.ControlInitializeSize(this);

            DownTime.Instance.UpdateDayDownTimeEvent += Instance_UpdateDayDownTimeEvent;
            DownTime.Instance.UpdateWeekDownTimeEvent += Instance_UpdateWeekDownTimeEvent;

            DownTime.Instance.GetDayDowntime();
            DownTime.Instance.GetWeekDowntime();
        }

        private void Instance_UpdateWeekDownTimeEvent(string[] tmstr, List<double> dayls, List<double> nightls)
        {
            this.Invoke(new Action(() =>
            {
                chart_Week.Series[0].Points.DataBindXY(tmstr, dayls);
                chart_Week.Series[1].Points.DataBindXY(tmstr, nightls);
            }));
        }

        private void Instance_UpdateDayDownTimeEvent(string[] tmstr, double[] minnute)
        {
            this.Invoke(new Action(() =>
            {
                chart_CurDowntime.Series[0].Points.DataBindXY(tmstr.ToList(), minnute.ToList());
            }));
        }

        private void Instance_ChangeLanguageHandle(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
        }

        private void Instance_UpdateCurDayDowntimeEvent(string[] ss, double[] dd)
        {
            this.Invoke(new Action(() =>
            {
                chart_CurDowntime.Series[0].Points.DataBindXY(ss.ToList(), dd.ToList());
            }));
        }

        private void DowntimeStatisticsForm_SizeChanged(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
    }
}
