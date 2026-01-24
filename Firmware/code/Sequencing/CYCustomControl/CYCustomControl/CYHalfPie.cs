using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CYCustomControl
{
    /// <summary>
    /// 半圆环良率显示控件
    /// </summary>
    // Token: 0x02000002 RID: 2
    public class HalfRing : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public HalfRing()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container"></param>
        // Token: 0x06000002 RID: 2 RVA: 0x0000210C File Offset: 0x0000030C
        public HalfRing(IContainer container)
        {
            container.Add(this);
            this.InitializeComponent();
        }

        /// <summary>
        /// 中心圆半径
        /// </summary>
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000003 RID: 3 RVA: 0x000021D0 File Offset: 0x000003D0
        // (set) Token: 0x06000004 RID: 4 RVA: 0x000021E8 File Offset: 0x000003E8
        [Browsable(true)]
        [Description("中心圆半径")]
        public int setCircleRadius
        {
            get
            {
                return this._CircleRadius;
            }
            set
            {
                bool flag = value < 0;
                if (flag)
                {
                    this._CircleRadius = 0;
                }
                else
                {
                    this._CircleRadius = value;
                }
                base.Refresh();
            }
        }

        /// <summary>
        /// 内圆环半径
        /// </summary>
        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000005 RID: 5 RVA: 0x0000221C File Offset: 0x0000041C
        // (set) Token: 0x06000006 RID: 6 RVA: 0x00002234 File Offset: 0x00000434
        [Browsable(true)]
        [Description("内圆环半径")]
        public int setInRadius
        {
            get
            {
                return this._InRadius;
            }
            set
            {
                bool flag = value < 0;
                if (flag)
                {
                    this._InRadius = 0;
                }
                else
                {
                    this._InRadius = value;
                }
                base.Refresh();
            }
        }

        /// <summary>
        /// 外圆环半径
        /// </summary>
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000007 RID: 7 RVA: 0x00002268 File Offset: 0x00000468
        // (set) Token: 0x06000008 RID: 8 RVA: 0x00002280 File Offset: 0x00000480
        [Browsable(true)]
        [Description("外圆环半径")]
        public int setOutRadius
        {
            get
            {
                return this._OutRadius;
            }
            set
            {
                bool flag = value < 0;
                if (flag)
                {
                    this._OutRadius = 0;
                }
                else
                {
                    this._OutRadius = value;
                }
                base.Refresh();
            }
        }

        /// <summary>
        /// 圆环的宽度
        /// </summary>
        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000009 RID: 9 RVA: 0x000022B4 File Offset: 0x000004B4
        // (set) Token: 0x0600000A RID: 10 RVA: 0x000022CC File Offset: 0x000004CC
        [Browsable(true)]
        [Description("圆环的宽度")]
        public int setLength
        {
            get
            {
                return this._Length;
            }
            set
            {
                bool flag = value < 0;
                if (flag)
                {
                    this._Length = 0;
                }
                else
                {
                    this._Length = value;
                }
                base.Refresh();
            }
        }

        /// <summary>
        /// ok的颜色
        /// </summary>
        // Token: 0x17000005 RID: 5
        // (get) Token: 0x0600000B RID: 11 RVA: 0x00002300 File Offset: 0x00000500
        // (set) Token: 0x0600000C RID: 12 RVA: 0x00002318 File Offset: 0x00000518
        [Browsable(true)]
        [Description("表示OK部分的颜色")]
        public Color setColorGreen
        {
            get
            {
                return this._ColorGreen;
            }
            set
            {
                this._ColorGreen = value;
                base.Refresh();
            }
        }

        /// <summary>
        /// Fail部分的颜色
        /// </summary>
        // Token: 0x17000006 RID: 6
        // (get) Token: 0x0600000D RID: 13 RVA: 0x0000232C File Offset: 0x0000052C
        // (set) Token: 0x0600000E RID: 14 RVA: 0x00002344 File Offset: 0x00000544
        [Browsable(true)]
        [Description("表示FAIL部分的颜色")]
        public Color setColorRed
        {
            get
            {
                return this._ColorRed;
            }
            set
            {
                this._ColorRed = value;
                base.Refresh();
            }
        }

        /// <summary>
        /// 外环部分的比率
        /// </summary>
        // Token: 0x17000007 RID: 7
        // (get) Token: 0x0600000F RID: 15 RVA: 0x00002358 File Offset: 0x00000558
        // (set) Token: 0x06000010 RID: 16 RVA: 0x00002370 File Offset: 0x00000570
        [Browsable(true)]
        [Description("外环部分的比率")]
        public float setRateOut
        {
            get
            {
                return this._RateOut;
            }
            set
            {
                this._RateOut = value;
                base.Refresh();
            }
        }

        /// <summary>
        /// 内环部分的比率
        /// </summary>
        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000011 RID: 17 RVA: 0x00002384 File Offset: 0x00000584
        // (set) Token: 0x06000012 RID: 18 RVA: 0x0000239C File Offset: 0x0000059C
        [Browsable(true)]
        [Description("内环部分的比率")]
        public float setRateIn
        {
            get
            {
                return this._RateIn;
            }
            set
            {
                this._RateIn = value;
                base.Refresh();
            }
        }

        /// <summary>
        /// 表示当前的结果
        /// </summary>
        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000013 RID: 19 RVA: 0x000023B0 File Offset: 0x000005B0
        // (set) Token: 0x06000014 RID: 20 RVA: 0x000023C8 File Offset: 0x000005C8
        [Browsable(true)]
        [Description("表示当前的结果")]
        public bool setResult
        {
            get
            {
                return this._bResult;
            }
            set
            {
                this._bResult = value;
                base.Refresh();
            }
        }

        /// <summary>
        /// 显示结果的字体
        /// </summary>
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000015 RID: 21 RVA: 0x000023DC File Offset: 0x000005DC
        // (set) Token: 0x06000016 RID: 22 RVA: 0x000023F4 File Offset: 0x000005F4
        [Browsable(true)]
        [Description("显示结果的字体")]
        public Font setResultFont
        {
            get
            {
                return this._ResultFont;
            }
            set
            {
                this._ResultFont = value;
                base.Refresh();
            }
        }

        /// <summary>
        /// 外环的注解
        /// </summary>
        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000017 RID: 23 RVA: 0x00002408 File Offset: 0x00000608
        // (set) Token: 0x06000018 RID: 24 RVA: 0x00002420 File Offset: 0x00000620
        [Browsable(true)]
        [Description("外环的注解")]
        public string setScriptOut
        {
            get
            {
                return this._strOut;
            }
            set
            {
                this._strOut = value;
                base.Refresh();
            }
        }

        /// <summary>
        /// 内环的注解
        /// </summary>
        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000019 RID: 25 RVA: 0x00002434 File Offset: 0x00000634
        // (set) Token: 0x0600001A RID: 26 RVA: 0x0000244C File Offset: 0x0000064C
        [Browsable(true)]
        [Description("内环的注解")]
        public string setScriptIn
        {
            get
            {
                return this._strIn;
            }
            set
            {
                this._strIn = value;
                base.Refresh();
            }
        }

        /// <summary>
        /// 绘图，重绘响应
        /// </summary>
        /// <param name="pe"></param>
        // Token: 0x0600001B RID: 27 RVA: 0x00002460 File Offset: 0x00000660
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            int num = base.ClientRectangle.Height / 2;
            int num2 = base.ClientRectangle.Width / 2;
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            SolidBrush brush = new SolidBrush(this._ColorGreen);
            SolidBrush brush2 = new SolidBrush(this._ColorRed);
            SolidBrush brush3 = new SolidBrush(this.BackColor);
            Rectangle rectangle = new Rectangle(num2 - this._OutRadius, num - this._OutRadius, this._OutRadius * 2, this._OutRadius * 2);
            pe.Graphics.FillPie(brush, rectangle, 0f, -180f * this._RateOut);
            pe.Graphics.FillPie(brush2, rectangle, -180f * this._RateOut, -180f * (1f - this._RateOut));
            string text = string.Format("{0:00.0}%", this._RateOut * 100f);
            TextRenderer.DrawText(pe.Graphics, text, this.Font, new Rectangle(num2 - this._OutRadius, num - this._Length, this._Length, this._Length), this.ForeColor, TextFormatFlags.Bottom | TextFormatFlags.Right);
            rectangle.X += this._Length;
            rectangle.Y += this._Length;
            rectangle.Width -= this._Length * 2;
            rectangle.Height -= this._Length * 2;
            pe.Graphics.FillPie(brush3, rectangle, 1f, -182f);
            rectangle.X = num2 - this._InRadius;
            rectangle.Y = num - this._InRadius;
            rectangle.Width = this._InRadius * 2;
            rectangle.Height = this._InRadius * 2;
            pe.Graphics.FillPie(brush, rectangle, 0f, -180f * this._RateIn);
            pe.Graphics.FillPie(brush2, rectangle, -180f * this._RateIn, -180f * (1f - this._RateIn));
            text = string.Format("{0:00.0}%", this._RateIn * 100f);
            TextRenderer.DrawText(pe.Graphics, text, this.Font, new Rectangle(num2 - this._InRadius, num - this._Length, this._Length, this._Length), this.ForeColor, TextFormatFlags.Bottom | TextFormatFlags.Right);
            rectangle.X += this._Length;
            rectangle.Y += this._Length;
            rectangle.Width -= this._Length * 2;
            rectangle.Height -= this._Length * 2;
            pe.Graphics.FillPie(brush3, rectangle, 1f, -182f);
            rectangle.X = num2 - this._CircleRadius;
            rectangle.Y = num - this._CircleRadius;
            rectangle.Width = this._CircleRadius * 2;
            rectangle.Height = this._CircleRadius * 2;
            bool bResult = this._bResult;
            if (bResult)
            {
                pe.Graphics.FillEllipse(brush, rectangle);
                TextRenderer.DrawText(pe.Graphics, "OK", this._ResultFont, rectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            else
            {
                pe.Graphics.FillEllipse(brush2, rectangle);
                TextRenderer.DrawText(pe.Graphics, "NG", this._ResultFont, rectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            this.DrawStringAndRotateAt(pe.Graphics, num2, num, this._OutRadius, this._strOut);
            this.DrawStringAndRotateAt(pe.Graphics, num2, num, this._InRadius, this._strIn);
        }

        /// <summary>
        /// 控件大小变更事件响应
        /// </summary>
        /// <param name="eventargs"></param>
        // Token: 0x0600001C RID: 28 RVA: 0x00002873 File Offset: 0x00000A73
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            base.Refresh();
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00002888 File Offset: 0x00000A88
        private void DrawStringAndRotateAt(Graphics g, int xCenter, int yCenter, int nRadius, string strText)
        {
            SizeF size = g.MeasureString(strText, this.Font);
            PointF rotatePoint = new PointF((float)(xCenter - nRadius + this._Length / 2) - size.Height / 2f, (float)yCenter + size.Width);
            Matrix myMatrix = new Matrix();
            myMatrix.RotateAt(90f, rotatePoint, MatrixOrder.Append);
            g.Transform = myMatrix;
            g.DrawString(strText, this.Font, new SolidBrush(Color.Black), rotatePoint.X - size.Width, rotatePoint.Y - size.Height);
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        // Token: 0x0600001E RID: 30 RVA: 0x00002928 File Offset: 0x00000B28
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
        // Token: 0x0600001F RID: 31 RVA: 0x0000295F File Offset: 0x00000B5F
        private void InitializeComponent()
        {
            this.components = new Container();
            base.AutoScaleMode = AutoScaleMode.Font;
        }

        // Token: 0x04000001 RID: 1
        private int _CircleRadius = 25;

        // Token: 0x04000002 RID: 2
        private int _InRadius = 70;

        // Token: 0x04000003 RID: 3
        private int _OutRadius = 100;

        // Token: 0x04000004 RID: 4
        private int _Length = 10;

        // Token: 0x04000005 RID: 5
        private Color _ColorGreen = Color.FromArgb(107, 187, 63);

        // Token: 0x04000006 RID: 6
        private Color _ColorRed = Color.FromArgb(200, 37, 6);

        // Token: 0x04000007 RID: 7
        private float _RateOut = 0.5f;

        // Token: 0x04000008 RID: 8
        private float _RateIn = 0.5f;

        // Token: 0x04000009 RID: 9
        private bool _bResult = false;

        // Token: 0x0400000A RID: 10
        private Font _ResultFont = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);

        // Token: 0x0400000B RID: 11
        private string _strOut = "外环";

        // Token: 0x0400000C RID: 12
        private string _strIn = "内环";

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        // Token: 0x0400000D RID: 13
        private IContainer components = null;
    }
}

