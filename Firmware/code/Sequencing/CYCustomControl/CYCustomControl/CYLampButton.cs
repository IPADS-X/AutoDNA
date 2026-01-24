using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CYCustomControl
{
    public class LampButton : Button
    {
        /// <summary>
        ///
        /// </summary>
        // Token: 0x06000020 RID: 32 RVA: 0x00002975 File Offset: 0x00000B75
        public LampButton()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        // Token: 0x06000021 RID: 33 RVA: 0x00002994 File Offset: 0x00000B94
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this._state = LampButton.ControlState.Normal;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mevent"></param>
        // Token: 0x06000022 RID: 34 RVA: 0x000029A6 File Offset: 0x00000BA6
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            this._state = LampButton.ControlState.Hover;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        // Token: 0x06000023 RID: 35 RVA: 0x000029B8 File Offset: 0x00000BB8
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this._state = LampButton.ControlState.Pressed;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        // Token: 0x06000024 RID: 36 RVA: 0x000029CA File Offset: 0x00000BCA
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this._state = LampButton.ControlState.Hover;
        }

        // Token: 0x06000025 RID: 37 RVA: 0x000029DC File Offset: 0x00000BDC
        private void CalculateRect(out Rectangle imageRect, out Rectangle textRect, Graphics g)
        {
            bool flag = base.Image != null;
            if (flag)
            {
                imageRect = new Rectangle(0, (base.ClientRectangle.Height - base.Image.Size.Height) / 2, base.Image.Size.Width, base.Image.Size.Height);
                textRect = new Rectangle(base.Image.Size.Width, 0, base.ClientRectangle.Width - base.Image.Width, base.ClientRectangle.Height);
            }
            else
            {
                imageRect = new Rectangle(0, 0, 0, 0);
                textRect = base.ClientRectangle;
            }
        }

        /// <summary>
        /// 画边框与背景
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="style"></param>
        /// <param name="roundWidth"></param>
        // Token: 0x06000026 RID: 38 RVA: 0x00002AB8 File Offset: 0x00000CB8
        internal void RenderBackGroundInternal(Graphics g, Rectangle rect, RoundStyle style, int roundWidth)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        // Token: 0x06000027 RID: 39 RVA: 0x00002ABC File Offset: 0x00000CBC
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;
            Rectangle imageRect;
            Rectangle textRect;
            this.CalculateRect(out imageRect, out textRect, g);
            g.SmoothingMode = SmoothingMode.HighQuality;
            bool flag = this._state == LampButton.ControlState.Normal;
            if (flag)
            {
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                {
                    g.FillRectangle(brush, base.ClientRectangle);
                }
            }
            else
            {
                bool flag2 = this._state == LampButton.ControlState.Hover;
                if (flag2)
                {
                    using (SolidBrush brush2 = new SolidBrush(Color.LightGray))
                    {
                        g.FillRectangle(brush2, base.ClientRectangle);
                    }
                }
                else
                {
                    bool flag3 = this._state == LampButton.ControlState.Pressed;
                    if (flag3)
                    {
                        using (SolidBrush brush3 = new SolidBrush(Color.Gainsboro))
                        {
                            g.FillRectangle(brush3, base.ClientRectangle);
                        }
                    }
                }
            }
            bool flag4 = base.Image != null;
            if (flag4)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImageUnscaled(base.Image, imageRect.Left, imageRect.Top);
            }
            bool flag5 = this.Text != "";
            if (flag5)
            {
                TextRenderer.DrawText(g, this.Text, this.Font, textRect, this.ForeColor, TextFormatFlags.VerticalCenter);
            }
        }

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        // Token: 0x06000028 RID: 40 RVA: 0x00002C38 File Offset: 0x00000E38
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
        // Token: 0x06000029 RID: 41 RVA: 0x00002C70 File Offset: 0x00000E70
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

        // Token: 0x0400000E RID: 14
        private LampButton.ControlState _state = LampButton.ControlState.Normal;

        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        // Token: 0x0400000F RID: 15
        private IContainer components = null;

        /// <summary>
        ///
        /// </summary>
        // Token: 0x0200000A RID: 10
        public enum ControlState
        {
            /// <summary>
            ///
            /// </summary>
            // Token: 0x0400003B RID: 59
            Normal,
            /// <summary>
            ///
            /// </summary>
            // Token: 0x0400003C RID: 60
            Hover,
            /// <summary>
            ///
            /// </summary>
            // Token: 0x0400003D RID: 61
            Pressed
        }
    }
}

