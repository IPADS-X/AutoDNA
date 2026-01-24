using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CYCustomControl
{
    public class RoundPanel : Panel
    {
        /// <summary>
        ///
        /// </summary>
        // Token: 0x06000063 RID: 99 RVA: 0x00004FCC File Offset: 0x000031CC
        public RoundPanel()
        {
            this.InitializeComponent();
            base.Padding = new Padding(0, 0, 0, 0);
            base.Margin = new Padding(0, 0, 0, 0);
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        /// <summary>圆角弧度(0为不要圆角)</summary>
        // Token: 0x1700001E RID: 30
        // (get) Token: 0x06000064 RID: 100 RVA: 0x00005020 File Offset: 0x00003220
        // (set) Token: 0x06000065 RID: 101 RVA: 0x00005038 File Offset: 0x00003238
        [Browsable(true)]
        [Description("圆角弧度(0为不要圆角)")]
        public int _setRoundRadius
        {
            get
            {
                return this._Radius;
            }
            set
            {
                bool flag = value < 0;
                if (flag)
                {
                    this._Radius = 0;
                }
                else
                {
                    this._Radius = value;
                }
                base.Refresh();
            }
        }

        /// <summary>
        /// 圆角代码
        /// </summary>
        /// <param name="region"></param>
        // Token: 0x06000066 RID: 102 RVA: 0x0000506C File Offset: 0x0000326C
        public void Round(Region region)
        {
            GraphicsPath oPath = new GraphicsPath();
            int x = 0;
            int y = 0;
            int thisWidth = base.Width;
            int thisHeight = base.Height;
            int angle = this._Radius;
            bool flag = angle > 0;
            if (flag)
            {
                Graphics g = base.CreateGraphics();
                oPath.AddArc(x, y, angle, angle, 180f, 90f);
                oPath.AddArc(thisWidth - angle, y, angle, angle, 270f, 90f);
                oPath.AddArc(thisWidth - angle, thisHeight - angle, angle, angle, 0f, 90f);
                oPath.AddArc(x, thisHeight - angle, angle, angle, 90f, 90f);
                oPath.CloseAllFigures();
                base.Region = new Region(oPath);
            }
            else
            {
                oPath.AddLine(x + angle, y, thisWidth - angle, y);
                oPath.AddLine(thisWidth, y + angle, thisWidth, thisHeight - angle);
                oPath.AddLine(thisWidth - angle, thisHeight, x + angle, thisHeight);
                oPath.AddLine(x, y + angle, x, thisHeight - angle);
                oPath.CloseAllFigures();
                base.Region = new Region(oPath);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="container"></param>
        // Token: 0x06000067 RID: 103 RVA: 0x00005191 File Offset: 0x00003391
        public RoundPanel(IContainer container)
        {
            container.Add(this);
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pe"></param>
        // Token: 0x06000068 RID: 104 RVA: 0x000051B8 File Offset: 0x000033B8
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            this.Round(base.Region);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventargs"></param>
        // Token: 0x06000069 RID: 105 RVA: 0x000051D0 File Offset: 0x000033D0
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            base.Refresh();
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        // Token: 0x0600006A RID: 106 RVA: 0x000051E4 File Offset: 0x000033E4
        protected override void Dispose(bool disposing)
        {
            bool flag = disposing && this.components != null;
            if (flag)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        // Token: 0x0600006B RID: 107 RVA: 0x0000521C File Offset: 0x0000341C
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

        // Token: 0x04000037 RID: 55
        private int _Radius = 8;

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        // Token: 0x04000038 RID: 56
        private IContainer components = null;
    }
}
