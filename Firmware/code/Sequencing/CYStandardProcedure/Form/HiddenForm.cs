using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CYAutoFramework;
using System.IO;

namespace CYStandardProcedure
{
    public partial class HiddenForm : Form
    {
        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        public HiddenForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void HiddenForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;

            this.panel1.MouseDown += new MouseEventHandler(panel1_MouseDown);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += HiddenForm_LanguageChangeEvent; ;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void HiddenForm_LanguageChangeEvent(string strLanguage)
        {
            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变Panel容器内控件文本***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(rbt_Show, "显示硬盘");
                toolTip1.SetToolTip(rbt_hide, "隐藏硬盘");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(rbt_Show, "Display Hard Drive");
                toolTip1.SetToolTip(rbt_hide, "Hidden Hard Drive");
            }
            else
            {
                toolTip1.SetToolTip(rbt_Show, "Hiển thị đĩa cứng");
                toolTip1.SetToolTip(rbt_hide, "Ổ cứng ẩn");
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x02, 0);
            }
        }

        private void rbt_Show_Click(object sender, EventArgs e)
        {
            HideBoot.Show();
            DialogResult = DialogResult.Yes;
        }

        private void rbt_hide_Click(object sender, EventArgs e)
        {
            HideBoot.Hide(HardName.E.ToString());
            DialogResult = DialogResult.Yes;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
