using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace KControlsLib
{
    public class ImageButton : PictureBox, IButtonControl, IDisposable
    {
        #region 新建属性
        [Description("基本图像"), Category("自定义属性")]
        public Image ImageNormal { get { return this.imageNormal; } set { base.Image = this.imageNormal = value; } }

        [Description("悬停后图像"), Category("自定义属性")]
        public Image? ImageNormalHover { get; set; } = null;

        [Description("按下后图像"), Category("自定义属性")]
        public Image? ImageDown { get; set; } = null;

        [Description("Disable后图像"), Category("自定义属性")]
        public Image? ImageDisabled { get; set; } = null;

        [Description("悬停ToolTip文字"), Category("自定义属性")]
        public string HoverText { get; set; } = "";

        [Description("Toggle 模式"), Category("自定义属性")]
        public bool ToggleMode { get; set; } = false;

        [Description("Toggle 模式按下后悬停ToolTip文字"), Category("自定义属性")]
        public string HoverTextToggle { get; set; } = "";

        [Description("Toggle 模式按下后悬停图像"), Category("自定义属性")]
        public Image? ImageDownHover { get; set; } = null;

        [Description("当前是否按下状态"), Category("自定义属性")]
        public bool Down { get { return _down; } set { _down = value; UpdateImage(this.Down ? this.ImageDown : this.ImageNormal); } }
        #endregion

        #region 隐藏无用属性
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image Image { get { return base.Image; } set { base.Image = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new String ImageLocation { get { return base.ImageLocation; } set { base.ImageLocation = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image ErrorImage { get { return base.ErrorImage; } set { base.ErrorImage = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image InitialImage { get { return base.InitialImage; } set { base.InitialImage = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool WaitOnLoad { get { return base.WaitOnLoad; } set { base.WaitOnLoad = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Text { get { return base.Text; } set { base.Text = value; } }
        #endregion

        #region 内部变量等
        private System.ComponentModel.IContainer components = null;
        private Image imageNormal;
        private ToolTip tip = new ToolTip();
        private bool _down = false;
        #endregion

        #region 覆盖基类函数
        protected override void OnBackgroundImageChanged(EventArgs e) { }   // 什么都不做
        protected override void OnTextChanged(EventArgs e) { }  // 什么都不做

        #endregion

        #region 构造函数
        public ImageButton() : base()
        {
            this.InitializeComponent();
            base.SizeMode = PictureBoxSizeMode.StretchImage;
            base.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Margin = new Padding(0, 0, 0, 0);
            this.MouseUp += ImageButton_MouseUp;
            this.MouseDown += ImageButton_MouseDown;
            this.MouseEnter += ImageButton_MouseEnter;
            this.MouseLeave += ImageButton_MouseLeave;
            this.EnabledChanged += ImageButton_EnabledChanged;
            this.MouseHover += ImageButton_MouseHover;
        }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            // this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);//解决闪烁 
            UpdateStyles();
        }
        #endregion

        #region 方法实现
        private void UpdateImage(Image? img)
        {
            if (img != null) base.Image = img;
        }
        private void UpdateTip(string? str)
        {
            if (str == "") return;
            tip.SetToolTip(this, str);
        }

        private void ImageButton_MouseHover(object? sender, EventArgs e)
        {
            if (this.ToggleMode)
                UpdateTip(this.Down ? this.HoverTextToggle : this.HoverText);
            else
                UpdateTip(this.HoverText);
        }

        private void ImageButton_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!this.ToggleMode) this.Down = false;
        }
        private void ImageButton_MouseDown(object? sender, MouseEventArgs e)
        {
            base.Focus();

            if (e.Button != MouseButtons.Left) return;
            if (this.ToggleMode) this.Down = !this.Down;
            else this.Down = true;
        }
        private void ImageButton_MouseEnter(object? sender, EventArgs e)
        {
            if (this.ToggleMode)
            {
                UpdateImage(this.Down ? this.ImageDownHover : this.ImageNormalHover);
            }
            else
            {
                UpdateImage(this.ImageNormalHover);
            }
        }
        private void ImageButton_MouseLeave(object? sender, EventArgs e)
        {
            if (this.ToggleMode)
            {
                UpdateImage(this.Down ? this.ImageDown : this.ImageNormal);
            }
            else
            {
                UpdateImage(this.ImageNormal);
            }
        }
        private void ImageButton_EnabledChanged(object? sender, EventArgs e)
        {
            this.Down = false;
            UpdateImage(this.Enabled ? this.imageNormal : this.ImageDisabled);
        }
        #endregion

        #region IDisposable 接口
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region IButtonControl 接口
        private DialogResult dialogResult;
        private bool IsDefault;
        public DialogResult DialogResult
        {
            get
            {
                return this.dialogResult;
            }

            set
            {
                if (Enum.IsDefined(typeof(DialogResult), value))
                {
                    this.dialogResult = value;
                }
            }
        }
        public void NotifyDefault(bool value)
        {
            this.IsDefault = value;
        }
        public void PerformClick()
        {
            base.OnClick(EventArgs.Empty);
        }

        #endregion

        #region 键盘处理
        /// <summary>
        ///  Enter 直接触发
        ///  Space 按下作为鼠标按下，松开作为鼠标松开，触发
        ///  Space 按下后按Esc或Tab取消触发，但执行鼠标松开
        /// </summary>
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private bool holdingSpace = false;

        public override bool PreProcessMessage(ref Message msg)
        {
            if (msg.Msg == WM_KEYUP)
            {
                if (holdingSpace)
                {
                    if ((int)msg.WParam == (int)Keys.Space)
                    {
                        OnMouseUp(null);
                        PerformClick();
                    }
                    else if ((int)msg.WParam == (int)Keys.Escape
                        || (int)msg.WParam == (int)Keys.Tab)
                    {
                        holdingSpace = false;
                        OnMouseUp(null);
                    }
                }
                return true;
            }
            else if (msg.Msg == WM_KEYDOWN)
            {
                if ((int)msg.WParam == (int)Keys.Space)
                {
                    holdingSpace = true;
                    OnMouseDown(null);
                }
                else if ((int)msg.WParam == (int)Keys.Enter)
                {
                    PerformClick();
                }
                return true;
            }
            else
                return base.PreProcessMessage(ref msg);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            holdingSpace = false;
            OnMouseUp(null);
            base.OnLostFocus(e);
        }

        #endregion
    }
}
