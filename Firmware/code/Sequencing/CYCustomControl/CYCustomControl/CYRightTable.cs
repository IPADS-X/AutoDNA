using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CYCustomControl
{
    [ToolboxBitmap(typeof(TabControl))]
    public class RightTab : TabControl
    {
        public RightTab()
        {
            this.InitializeComponent();
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        [Browsable(true)]
        public Color TabColor
        {
            get
            {
                return this.mTabColor;
            }
            set
            {
                this.mTabColor = value;
                base.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.DrawControl(e.Graphics);
        }

        internal void DrawControl(Graphics g)
        {
            bool flag = !base.Visible;
            if (!flag)
            {
                Rectangle TabControlArea = base.ClientRectangle;
                Rectangle TabArea = this.DisplayRectangle;
                Brush br = new SolidBrush(base.Parent.BackColor);
                g.FillRectangle(br, TabControlArea);
                br.Dispose();
                Pen border = new Pen(SystemColors.ControlDark);
                border.DashStyle = DashStyle.Dash;
                g.DrawRectangle(border, TabArea);
                border.Dispose();
                int num;
                for (int i = 0; i < base.TabCount; i = num + 1)
                {
                    this.DrawTab(g, base.TabPages[i], i);
                    num = i;
                }
            }
        }

        internal void DrawTab(Graphics g, TabPage tabPage, int nIndex)
        {
            Rectangle recBounds = base.GetTabRect(nIndex);
            RectangleF tabTextArea = base.GetTabRect(nIndex);
            bool bSelected = base.SelectedIndex == nIndex;
            Point[] pt = new Point[7];
            bool flag = base.Alignment == TabAlignment.Top;
            if (flag)
            {
                pt[0] = new Point(recBounds.Left, recBounds.Bottom);
                pt[1] = new Point(recBounds.Left, recBounds.Top + 5);
                pt[2] = new Point(recBounds.Left + 5, recBounds.Top);
                pt[3] = new Point(recBounds.Right - 5, recBounds.Top);
                pt[4] = new Point(recBounds.Right, recBounds.Top + 5);
                pt[5] = new Point(recBounds.Right, recBounds.Bottom);
                pt[6] = new Point(recBounds.Left, recBounds.Bottom);
            }
            else
            {
                bool flag2 = base.Alignment == TabAlignment.Bottom;
                if (flag2)
                {
                    pt[0] = new Point(recBounds.Left, recBounds.Top);
                    pt[1] = new Point(recBounds.Right, recBounds.Top);
                    pt[2] = new Point(recBounds.Right, recBounds.Bottom - 5);
                    pt[3] = new Point(recBounds.Right - 5, recBounds.Bottom);
                    pt[4] = new Point(recBounds.Left + 5, recBounds.Bottom);
                    pt[5] = new Point(recBounds.Left, recBounds.Bottom - 5);
                    pt[6] = new Point(recBounds.Left, recBounds.Top);
                }
                else
                {
                    bool flag3 = base.Alignment == TabAlignment.Left;
                    if (flag3)
                    {
                        pt[0] = new Point(recBounds.Left, recBounds.Top + 5);
                        pt[1] = new Point(recBounds.Left + 5, recBounds.Top);
                        pt[2] = new Point(recBounds.Right, recBounds.Top);
                        pt[3] = new Point(recBounds.Right, recBounds.Bottom);
                        pt[4] = new Point(recBounds.Left + 5, recBounds.Bottom);
                        pt[5] = new Point(recBounds.Left, recBounds.Bottom - 5);
                        pt[6] = new Point(recBounds.Left, recBounds.Top + 5);
                    }
                    else
                    {
                        pt[0] = new Point(recBounds.Left, recBounds.Top);
                        pt[1] = new Point(recBounds.Right - 5, recBounds.Top);
                        pt[2] = new Point(recBounds.Right, recBounds.Top + 5);
                        pt[3] = new Point(recBounds.Right, recBounds.Bottom - 5);
                        pt[4] = new Point(recBounds.Right - 5, recBounds.Bottom);
                        pt[5] = new Point(recBounds.Left, recBounds.Bottom);
                        pt[6] = new Point(recBounds.Left, recBounds.Top);
                    }
                }
            }
            bool flag4 = bSelected;
            if (flag4)
            {
                Brush brush = new SolidBrush(tabPage.BackColor);
                g.FillPolygon(brush, pt);
                brush.Dispose();
                g.DrawPolygon(SystemPens.ControlDark, pt);
            }
            else
            {
                Brush brush2 = new SolidBrush(this.TabColor);
                g.FillPolygon(brush2, pt);
                brush2.Dispose();
                g.DrawPolygon(SystemPens.ControlDark, pt);
            }
            bool flag5 = tabPage.ImageIndex >= 0 && base.ImageList != null && base.ImageList.Images[tabPage.ImageIndex] != null;
            if (flag5)
            {
                int nLeftMargin = 8;
                int nRightMargin = 2;
                Image img = base.ImageList.Images[tabPage.ImageIndex];
                Rectangle rimage = new Rectangle(recBounds.X + nLeftMargin, recBounds.Y + 1, img.Width, img.Height);
                float nAdj = (float)(nLeftMargin + img.Width + nRightMargin);
				Rectangle ptr = rimage;
                ptr.Y += (recBounds.Height - img.Height) / 2;
				RectangleF ptr2 = tabTextArea;
                ptr2.X += nAdj;
                ptr2 = tabTextArea;
                ptr2.Width -= nAdj;
                g.DrawImage(img, rimage);
            }
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            Brush br = new SolidBrush(tabPage.ForeColor);
            g.DrawString(tabPage.Text, this.Font, br, tabTextArea, stringFormat);
        }

        protected override void Dispose(bool disposing)
        {
            bool flag = disposing && this.components != null;
            if (flag)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.Multiline = true;
            this.ResumeLayout(false);

        }

        private Color mTabColor = SystemColors.Control;

        private IContainer components = null;
    }
}


