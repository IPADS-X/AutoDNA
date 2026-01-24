using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace CYCustomControl
{
    public enum ControlState
    {
        /// <summary>
        ///
        /// </summary>
        // Token: 0x04000017 RID: 23
        Normal,
        /// <summary>
        ///
        /// </summary>
        // Token: 0x04000018 RID: 24
        Hover,
        /// <summary>
        ///
        /// </summary>
        // Token: 0x04000019 RID: 25
        Pressed
    }
    public enum RoundStyle
    {
        /// <summary>
        ///
        /// </summary>
        // Token: 0x0400001B RID: 27
        None,
        /// <summary>
        ///
        /// </summary>
        // Token: 0x0400001C RID: 28
        All,
        /// <summary>
        ///
        /// </summary>
        // Token: 0x0400001D RID: 29
        Left,
        /// <summary>
        ///
        /// </summary>
        // Token: 0x0400001E RID: 30
        Right,
        /// <summary>
        ///
        /// </summary>
        // Token: 0x0400001F RID: 31
        Top,
        /// <summary>
        ///
        /// </summary>
        // Token: 0x04000020 RID: 32
        Bottom,
        /// <summary>
        ///
        /// </summary>
        // Token: 0x04000021 RID: 33
        BottomLeft,
        /// <summary>
        ///
        /// </summary>
        // Token: 0x04000022 RID: 34
        BottomRight
    }
    /// <summary>
    ///
    /// </summary>
    // Token: 0x02000008 RID: 8
    public class RoundButton : Button
    {

        public enum ButtonMousePosition
        {
            None,
            Button,
            Splitebutton
        }

        private Color _baseColor = Color.FromArgb(174, 218, 151);

        private Color _baseColorEnd = Color.FromArgb(174, 218, 151);

        private Color _arrowColor = Color.FromArgb(64, 64, 64);

        private int _imageWidth = 80;

        private int _imageHeight = 80;

        private RoundStyle _roundStyle = RoundStyle.All;

        private int _radius = 24;

        private int _imageTextSpace = 2;

        private bool _pressOffset = true;

        private bool _alwaysShowBorder = false;

        private bool _showSpliteButton = false;

        private int _spliteButtonWidth = 18;

        private ControlState _controlState;

        private RoundButton.ButtonMousePosition _mousePosition;

        private bool _contextHandle;

        private bool _contextOpened;

        private int _contextOffset = 5;

        private IContainer components = null;

        //[method: CompilerGenerated]
        //[DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        public event EventHandler OnButtonClick;

        //[method: CompilerGenerated]
        //[DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
        public event EventHandler OnSpliteButtonClick;

        [DefaultValue(5), Description("下拉菜单与按钮的距离")]
        public int ContextOffset
        {
            get
            {
                return this._contextOffset;
            }
            set
            {
                this._contextOffset = value;
            }
        }

        [DefaultValue(false), Description("是否启用分割按钮")]
        public bool ShowSpliteButton
        {
            get
            {
                return this._showSpliteButton;
            }
            set
            {
                bool flag = this._showSpliteButton != value;
                if (flag)
                {
                    this._showSpliteButton = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(0), Description("分割按钮的宽度")]
        public int SpliteButtonWidth
        {
            get
            {
                return this._spliteButtonWidth;
            }
            set
            {
                bool flag = this._spliteButtonWidth != value;
                if (flag)
                {
                    this._spliteButtonWidth = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(true), Description("当鼠标按下时图片和文字是否产生偏移")]
        public bool PressOffset
        {
            get
            {
                return this._pressOffset;
            }
            set
            {
                this._pressOffset = value;
            }
        }

        [DefaultValue(false), Description("是否一直显示按钮边框\n设置为false则只在鼠标经过和按下时显示边框")]
        public bool AlwaysShowBorder
        {
            get
            {
                return this._alwaysShowBorder;
            }
            set
            {
                bool flag = this._alwaysShowBorder != value;
                if (flag)
                {
                    this._alwaysShowBorder = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Color), "10 ,66, 204, 160"), Description("鼠标经过和按下时按钮的渐变背景颜色")]
        public Color BaseColor
        {
            get
            {
                return this._baseColor;
            }
            set
            {
                bool flag = this._baseColor != value;
                if (flag)
                {
                    this._baseColor = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Color), "200 ,66, 204, 160"), Description("鼠标经过和按下时按钮的渐变背景颜色")]
        public Color BaseColorEnd
        {
            get
            {
                return this._baseColorEnd;
            }
            set
            {
                bool flag = this._baseColorEnd != value;
                if (flag)
                {
                    this._baseColorEnd = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(24), Description("图片宽度")]
        public int ImageWidth
        {
            get
            {
                return this._imageWidth;
            }
            set
            {
                bool flag = value != this._imageWidth;
                if (flag)
                {
                    this._imageWidth = ((value < 12) ? 12 : value);
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(24), Description("图片高度")]
        public int ImageHeight
        {
            get
            {
                return this._imageHeight;
            }
            set
            {
                bool flag = value != this._imageHeight;
                if (flag)
                {
                    this._imageHeight = ((value < 12) ? 12 : value);
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(RoundStyle), "1"), Description("按钮圆角样式")]
        public RoundStyle RoundStyle
        {
            get
            {
                return this._roundStyle;
            }
            set
            {
                bool flag = this._roundStyle != value;
                if (flag)
                {
                    this._roundStyle = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(2), Description("按钮圆角弧度")]
        public int Radius
        {
            get
            {
                return this._radius;
            }
            set
            {
                bool flag = this._radius != value;
                if (flag)
                {
                    this._radius = ((value < 2) ? 2 : value);
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(2), Description("图片与文字之间的间距")]
        public int ImageTextSpace
        {
            get
            {
                return this._imageTextSpace;
            }
            set
            {
                bool flag = this._imageTextSpace != value;
                if (flag)
                {
                    this._imageTextSpace = ((value < 0) ? 0 : value);
                    base.Invalidate();
                }
            }
        }

        internal ControlState ControlState
        {
            get
            {
                return this._controlState;
            }
            set
            {
                bool flag = this._controlState != value;
                if (flag)
                {
                    this._controlState = value;
                    base.Invalidate();
                }
            }
        }

        internal RoundButton.ButtonMousePosition CurrentMousePosition
        {
            get
            {
                return this._mousePosition;
            }
            set
            {
                bool flag = this._mousePosition != value;
                if (flag)
                {
                    this._mousePosition = value;
                    base.Invalidate();
                }
            }
        }

        internal Rectangle ButtonRect
        {
            get
            {
                bool showSpliteButton = this.ShowSpliteButton;
                Rectangle result;
                if (showSpliteButton)
                {
                    result = new Rectangle(0, 0, base.ClientRectangle.Width - this.SpliteButtonWidth, base.ClientRectangle.Height);
                }
                else
                {
                    result = base.ClientRectangle;
                }
                return result;
            }
        }

        internal Rectangle SpliteButtonRect
        {
            get
            {
                bool showSpliteButton = this.ShowSpliteButton;
                Rectangle result;
                if (showSpliteButton)
                {
                    result = new Rectangle(base.ClientRectangle.Width - this.SpliteButtonWidth, 0, this.SpliteButtonWidth, base.ClientRectangle.Height);
                }
                else
                {
                    result = Rectangle.Empty;
                }
                return result;
            }
        }

        public RoundButton()
        {
            base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.Opaque, false);
            this._controlState = ControlState.Normal;
            this.BackColor = Color.Transparent;
            base.FlatStyle = FlatStyle.Flat;
            base.FlatAppearance.BorderSize = 0;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            bool flag = !this._contextOpened;
            if (flag)
            {
                this.ControlState = ControlState.Normal;
                this.CurrentMousePosition = RoundButton.ButtonMousePosition.None;
            }
        }

        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            bool flag = base.ClientRectangle.Contains(mevent.Location);
            if (flag)
            {
                this.ControlState = ControlState.Hover;
                bool showSpliteButton = this.ShowSpliteButton;
                if (showSpliteButton)
                {
                    this.CurrentMousePosition = (this.ButtonRect.Contains(mevent.Location) ? RoundButton.ButtonMousePosition.Button : RoundButton.ButtonMousePosition.Splitebutton);
                }
                else
                {
                    this.CurrentMousePosition = RoundButton.ButtonMousePosition.Button;
                }
            }
            else
            {
                this.ControlState = ControlState.Normal;
                this.CurrentMousePosition = RoundButton.ButtonMousePosition.None;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            bool flag = e.Button == MouseButtons.Left && e.Clicks == 1;
            if (flag)
            {
                this.ControlState = ControlState.Pressed;
                bool showSpliteButton = this.ShowSpliteButton;
                if (showSpliteButton)
                {
                    this.CurrentMousePosition = (this.ButtonRect.Contains(e.Location) ? RoundButton.ButtonMousePosition.Button : RoundButton.ButtonMousePosition.Splitebutton);
                }
                else
                {
                    this.CurrentMousePosition = RoundButton.ButtonMousePosition.Button;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            bool flag = e.Button == MouseButtons.Left && e.Clicks == 1;
            if (flag)
            {
                bool flag2 = base.ClientRectangle.Contains(e.Location);
                if (flag2)
                {
                    this.ControlState = ControlState.Hover;
                    bool showSpliteButton = this.ShowSpliteButton;
                    if (showSpliteButton)
                    {
                        this.CurrentMousePosition = (this.ButtonRect.Contains(e.Location) ? RoundButton.ButtonMousePosition.Button : RoundButton.ButtonMousePosition.Splitebutton);
                        bool flag3 = this.CurrentMousePosition == RoundButton.ButtonMousePosition.Splitebutton;
                        if (flag3)
                        {
                            bool flag4 = this.OnSpliteButtonClick != null;
                            if (flag4)
                            {
                                this.OnSpliteButtonClick(this, EventArgs.Empty);
                            }
                            bool flag5 = this.ContextMenuStrip != null;
                            if (flag5)
                            {
                                bool flag6 = !this._contextHandle;
                                if (flag6)
                                {
                                    this._contextHandle = true;
                                    this.ContextMenuStrip.Opening += new CancelEventHandler(this.ContextMenuStrip_Opening);
                                    this.ContextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler(this.ContextMenuStrip_Closed);
                                }
                                this.ContextMenuStrip.Opacity = 1.0;
                                this.ContextMenuStrip.Show(this, 0, base.Height + this.ContextOffset);
                            }
                        }
                        else
                        {
                            bool flag7 = this.OnButtonClick != null;
                            if (flag7)
                            {
                                this.OnButtonClick(this, EventArgs.Empty);
                            }
                        }
                    }
                    else
                    {
                        this.CurrentMousePosition = RoundButton.ButtonMousePosition.Button;
                    }
                }
                else
                {
                    this.ControlState = ControlState.Normal;
                    this.CurrentMousePosition = RoundButton.ButtonMousePosition.None;
                }
            }
        }

        private void ContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this._contextOpened = false;
            this.ControlState = ControlState.Normal;
            this.CurrentMousePosition = RoundButton.ButtonMousePosition.None;
            base.Invalidate();
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this._contextOpened = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Rectangle destRect;
            Rectangle bounds;
            this.CalculateRect(out destRect, out bounds, graphics);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            this.RenderBackGroundInternal(graphics, base.ClientRectangle, this.RoundStyle, this.Radius);
            bool flag = base.Image != null;
            if (flag)
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                graphics.DrawImage(base.Image, destRect, 0, 0, base.Image.Width, base.Image.Height, GraphicsUnit.Pixel);
            }
            bool flag2 = this.Text != "";
            if (flag2)
            {
                TextRenderer.DrawText(graphics, this.Text, this.Font, bounds, this.ForeColor, RoundButton.GetTextFormatFlags(this.TextAlign, this.RightToLeft == RightToLeft.Yes));
            }
            bool showSpliteButton = this.ShowSpliteButton;
            if (showSpliteButton)
            {
                this.RenderSpliteButton(graphics, base.ClientRectangle);
            }
        }

        private void CalculateRect(out Rectangle imageRect, out Rectangle textRect, Graphics g)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;
            SizeF sizeF = g.MeasureString(this.Text, this.Font);
            bool flag = this.Text == "";
            if (flag)
            {
                ContentAlignment imageAlign = base.ImageAlign;
                if (imageAlign <= ContentAlignment.MiddleCenter)
                {
                    switch (imageAlign)
                    {
                        case ContentAlignment.TopLeft:
                            imageRect = new Rectangle(2, 2, this.ImageWidth, this.ImageHeight);
                            break;
                        case ContentAlignment.TopCenter:
                            imageRect = new Rectangle((this.ButtonRect.Width - this.ImageWidth) / 2, 2, this.ImageWidth, this.ImageHeight);
                            break;
                        case (ContentAlignment)3:
                            break;
                        case ContentAlignment.TopRight:
                            imageRect = new Rectangle(this.ButtonRect.Width - this.ImageWidth - 3, 2, this.ImageWidth, this.ImageHeight);
                            break;
                        default:
                            if (imageAlign != ContentAlignment.MiddleLeft)
                            {
                                if (imageAlign == ContentAlignment.MiddleCenter)
                                {
                                    imageRect = new Rectangle((this.ButtonRect.Width - this.ImageWidth) / 2, (base.Height - this.ImageHeight) / 2, this.ImageWidth, this.ImageHeight);
                                }
                            }
                            else
                            {
                                imageRect = new Rectangle(2, (base.Height - this.ImageHeight) / 2, this.ImageWidth, this.ImageHeight);
                            }
                            break;
                    }
                }
                else if (imageAlign <= ContentAlignment.BottomLeft)
                {
                    if (imageAlign != ContentAlignment.MiddleRight)
                    {
                        if (imageAlign == ContentAlignment.BottomLeft)
                        {
                            imageRect = new Rectangle(2, base.Height - this.ImageHeight - 3, this.ImageWidth, this.ImageHeight);
                        }
                    }
                    else
                    {
                        imageRect = new Rectangle(this.ButtonRect.Width - this.ImageWidth - 3, (base.Height - this.ImageHeight) / 2, this.ImageWidth, this.ImageHeight);
                    }
                }
                else if (imageAlign != ContentAlignment.BottomCenter)
                {
                    if (imageAlign == ContentAlignment.BottomRight)
                    {
                        imageRect = new Rectangle(this.ButtonRect.Width - this.ImageWidth - 3, base.Height - this.ImageHeight - 3, this.ImageWidth, this.ImageHeight);
                    }
                }
                else
                {
                    imageRect = new Rectangle((this.ButtonRect.Width - this.ImageWidth) / 2, base.Height - this.ImageHeight - 3, this.ImageWidth, this.ImageHeight);
                }
                bool flag2 = this.PressOffset && this.ControlState == ControlState.Pressed && this.CurrentMousePosition == RoundButton.ButtonMousePosition.Button;
                if (flag2)
                {
                    imageRect.X++;
                    imageRect.Y++;
                }
                bool flag3 = this.RightToLeft == RightToLeft.Yes;
                if (flag3)
                {
                    imageRect.X = this.ButtonRect.Width - imageRect.Right;
                }
            }
            else
            {
                switch (base.TextImageRelation)
                {
                    case TextImageRelation.Overlay:
                        imageRect = new Rectangle(this.ButtonRect.Left, this.ButtonRect.Top, this.ButtonRect.Width, this.ButtonRect.Height);
                        textRect = new Rectangle(this.ButtonRect.Left, this.ButtonRect.Top, this.ButtonRect.Width, this.ButtonRect.Height);
                        break;
                    case TextImageRelation.ImageAboveText:
                        imageRect = new Rectangle((this.ButtonRect.Width - this.ImageWidth) / 2, (base.Height - this.ImageHeight - (int)sizeF.Height - this.ImageTextSpace) / 2, this.ImageWidth, this.ImageHeight);
                        textRect = new Rectangle((this.ButtonRect.Width - (int)sizeF.Width) / 2, imageRect.Bottom + this.ImageTextSpace, (int)sizeF.Width, (int)sizeF.Height);
                        break;
                    case TextImageRelation.TextAboveImage:
                        textRect = new Rectangle((this.ButtonRect.Width - (int)sizeF.Width) / 2, (base.Height - (int)sizeF.Height - this.ImageHeight - this.ImageTextSpace) / 2, (int)sizeF.Width, (int)sizeF.Height);
                        imageRect = new Rectangle((this.ButtonRect.Width - this.ImageWidth) / 2, textRect.Bottom + this.ImageTextSpace, this.ImageWidth, this.ImageHeight);
                        break;
                    case TextImageRelation.ImageBeforeText:
                        imageRect = new Rectangle((this.ButtonRect.Width - this.ImageWidth - (int)sizeF.Width - this.ImageTextSpace) / 2, (base.Height - this.ImageHeight) / 2, this.ImageWidth, this.ImageHeight);
                        textRect = new Rectangle(imageRect.Right + this.ImageTextSpace, (base.Height - (int)sizeF.Height) / 2, (int)sizeF.Width, (int)sizeF.Height);
                        break;
                    case TextImageRelation.TextBeforeImage:
                        textRect = new Rectangle((this.ButtonRect.Width - this.ImageWidth - (int)sizeF.Width - this.ImageTextSpace) / 2, (base.Height - (int)sizeF.Height) / 2, (int)sizeF.Width, (int)sizeF.Height);
                        imageRect = new Rectangle(textRect.Right + this.ImageTextSpace, (base.Height - this.ImageHeight) / 2, this.ImageWidth, this.ImageHeight);
                        break;
                }
                bool flag4 = this.PressOffset && this.ControlState == ControlState.Pressed && this.CurrentMousePosition == RoundButton.ButtonMousePosition.Button;
                if (flag4)
                {
                    imageRect.X++;
                    imageRect.Y++;
                    textRect.X++;
                    textRect.Y++;
                }
                bool flag5 = this.RightToLeft == RightToLeft.Yes;
                if (flag5)
                {
                    imageRect.X = this.ButtonRect.Width - imageRect.Right;
                    textRect.X = this.ButtonRect.Width - textRect.Right;
                }
            }
        }

        internal void RenderBackGroundInternal(Graphics g, Rectangle rect, RoundStyle style, int roundWidth)
        {
            int num = rect.Width;
            rect.Width = num - 1;
            num = rect.Height;
            rect.Height = num - 1;
            bool flag = style > RoundStyle.None;
            if (flag)
            {
                using (GraphicsPath graphicsPath = GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                {
                    bool flag2 = this.ControlState == ControlState.Normal;
                    if (flag2)
                    {
                        using (SolidBrush solidBrush = new SolidBrush(this._baseColor))
                        {
                            bool flag3 = !this.ShowSpliteButton;
                            if (flag3)
                            {
                                g.FillPath(solidBrush, graphicsPath);
                            }
                        }
                    }
                    else
                    {
                        using (LinearGradientBrush linearGradientBrush = (this.ControlState == ControlState.Pressed) ? new LinearGradientBrush(rect, this._baseColorEnd, this._baseColor, LinearGradientMode.ForwardDiagonal) : new LinearGradientBrush(rect, this._baseColor, this._baseColorEnd, LinearGradientMode.Vertical))
                        {
                            bool flag4 = !this.ShowSpliteButton;
                            if (flag4)
                            {
                                g.FillPath(linearGradientBrush, graphicsPath);
                            }
                            else
                            {
                                bool flag5 = this.CurrentMousePosition == RoundButton.ButtonMousePosition.Button;
                                if (flag5)
                                {
                                    using (GraphicsPath graphicsPath2 = GraphicsPathHelper.CreatePath(this.ButtonRect, roundWidth, RoundStyle.Left, true))
                                    {
                                        g.FillPath(linearGradientBrush, graphicsPath2);
                                    }
                                }
                                else
                                {
                                    using (GraphicsPath graphicsPath3 = GraphicsPathHelper.CreatePath(this.SpliteButtonRect, roundWidth, RoundStyle.Right, true))
                                    {
                                        g.FillPath(linearGradientBrush, graphicsPath3);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                bool flag6 = this.ControlState > ControlState.Normal;
                if (flag6)
                {
                    bool flag7 = this.ControlState == ControlState.Normal;
                    if (flag7)
                    {
                        using (SolidBrush solidBrush2 = new SolidBrush(this._baseColor))
                        {
                            bool flag8 = !this.ShowSpliteButton;
                            if (flag8)
                            {
                                g.FillRectangle(solidBrush2, rect);
                            }
                        }
                    }
                    else
                    {
                        using (LinearGradientBrush linearGradientBrush2 = (this.ControlState == ControlState.Pressed) ? new LinearGradientBrush(rect, this._baseColorEnd, this._baseColor, LinearGradientMode.ForwardDiagonal) : new LinearGradientBrush(rect, this._baseColor, this._baseColorEnd, LinearGradientMode.Vertical))
                        {
                            bool flag9 = !this.ShowSpliteButton;
                            if (flag9)
                            {
                                g.FillRectangle(linearGradientBrush2, rect);
                            }
                            else
                            {
                                bool flag10 = this.CurrentMousePosition == RoundButton.ButtonMousePosition.Button;
                                if (flag10)
                                {
                                    g.FillRectangle(linearGradientBrush2, this.ButtonRect);
                                }
                                else
                                {
                                    g.FillRectangle(linearGradientBrush2, this.SpliteButtonRect);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void RenderSpliteButton(Graphics g, Rectangle rect)
        {
            Point[] array = new Point[]
            {
                new Point(rect.Width - this.SpliteButtonWidth + 2, (rect.Height - 4) / 2),
                new Point(rect.Width - this.SpliteButtonWidth + 2 + 8, (rect.Height - 4) / 2),
                new Point(rect.Width - this.SpliteButtonWidth + 2 + 4, (rect.Height - 4) / 2 + 4)
            };
            bool flag = this.PressOffset && this.ControlState == ControlState.Pressed && this.CurrentMousePosition == RoundButton.ButtonMousePosition.Splitebutton;
            if (flag)
            {
                Point[] var_2_AF_cp_0 = array;
                int var_2_AF_cp_1 = 0;
                var_2_AF_cp_0[var_2_AF_cp_1].X = var_2_AF_cp_0[var_2_AF_cp_1].X + 1;
                Point[] var_2_C6_cp_0 = array;
                int var_2_C6_cp_1 = 0;
                var_2_C6_cp_0[var_2_C6_cp_1].Y = var_2_C6_cp_0[var_2_C6_cp_1].Y + 1;
                Point[] var_2_DD_cp_0 = array;
                int var_2_DD_cp_1 = 1;
                var_2_DD_cp_0[var_2_DD_cp_1].X = var_2_DD_cp_0[var_2_DD_cp_1].X + 1;
                Point[] var_2_F4_cp_0 = array;
                int var_2_F4_cp_1 = 1;
                var_2_F4_cp_0[var_2_F4_cp_1].Y = var_2_F4_cp_0[var_2_F4_cp_1].Y + 1;
                Point[] var_2_10B_cp_0 = array;
                int var_2_10B_cp_1 = 2;
                var_2_10B_cp_0[var_2_10B_cp_1].X = var_2_10B_cp_0[var_2_10B_cp_1].X + 1;
                Point[] var_2_122_cp_0 = array;
                int var_2_122_cp_1 = 2;
                var_2_122_cp_0[var_2_122_cp_1].Y = var_2_122_cp_0[var_2_122_cp_1].Y + 1;
            }
            using (SolidBrush solidBrush = new SolidBrush(this._arrowColor))
            {
                g.FillPolygon(solidBrush, array);
            }
        }

        internal static TextFormatFlags GetTextFormatFlags(ContentAlignment alignment, bool rightToleft)
        {
            TextFormatFlags textFormatFlags = TextFormatFlags.WordBreak;
            if (rightToleft)
            {
                textFormatFlags |= (TextFormatFlags.Right | TextFormatFlags.RightToLeft);
            }
            if (alignment <= ContentAlignment.MiddleCenter)
            {
                switch (alignment)
                {
                    case ContentAlignment.TopLeft:
                        textFormatFlags |= TextFormatFlags.Default;
                        break;
                    case ContentAlignment.TopCenter:
                        textFormatFlags |= TextFormatFlags.HorizontalCenter;
                        break;
                    case (ContentAlignment)3:
                        break;
                    case ContentAlignment.TopRight:
                        textFormatFlags |= TextFormatFlags.Right;
                        break;
                    default:
                        if (alignment != ContentAlignment.MiddleLeft)
                        {
                            if (alignment == ContentAlignment.MiddleCenter)
                            {
                                textFormatFlags |= (TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                            }
                        }
                        else
                        {
                            textFormatFlags |= TextFormatFlags.VerticalCenter;
                        }
                        break;
                }
            }
            else if (alignment <= ContentAlignment.BottomLeft)
            {
                if (alignment != ContentAlignment.MiddleRight)
                {
                    if (alignment == ContentAlignment.BottomLeft)
                    {
                        textFormatFlags |= TextFormatFlags.Bottom;
                    }
                }
                else
                {
                    textFormatFlags |= (TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                }
            }
            else if (alignment != ContentAlignment.BottomCenter)
            {
                if (alignment == ContentAlignment.BottomRight)
                {
                    textFormatFlags |= (TextFormatFlags.Bottom | TextFormatFlags.Right);
                }
            }
            else
            {
                textFormatFlags |= (TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter);
            }
            return textFormatFlags;
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
            this.components = new Container();
        }
    }
}

