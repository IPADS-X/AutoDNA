using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CYAutoFramework;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class ImageForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        #region 窗体控件自适应代码

        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }
        #endregion

        /***按钮提示语***/
        private ToolTip toolTip1 = new ToolTip();
        /***文件夹图片集合***/
        private List<Image> mImageList = new List<Image>();
        /***图片索引***/
        private int imageindex = 0;
        public ImageForm()
        {
            InitializeComponent();
        }

        private void ImageForm_Load(object sender, EventArgs e)
        {
            toolTip1.InitialDelay = 200;
            toolTip1.ReshowDelay = 200;
            toolTip1.ShowAlways = true;
            toolTip1.IsBalloon = true;
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += ImageForm_LanguageChangeEvent; 
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void ImageForm_LanguageChangeEvent(string strLanguage)
        {
            if (strLanguage == "CH")
            {
                toolTip1.SetToolTip(btn_Open, "选择图片文件夹");
                toolTip1.SetToolTip(btn_Prev, "上一张图片");
                toolTip1.SetToolTip(btn_Next, "下一张图片");
            }
            else if (strLanguage == "EN")
            {
                toolTip1.SetToolTip(btn_Open, "Select Picture folder");
                toolTip1.SetToolTip(btn_Prev, "Last picture");
                toolTip1.SetToolTip(btn_Next, "Next picture");
            }
            else
            {
                toolTip1.SetToolTip(btn_Open, "Chọn thư mục ảnh");
                toolTip1.SetToolTip(btn_Prev, "Ảnh cuối");
                toolTip1.SetToolTip(btn_Next, "Ảnh kế");
            }
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFile = new FolderBrowserDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                this.mImageList.Clear();
                string pathName = openFile.SelectedPath;
                DirectoryInfo d = new System.IO.DirectoryInfo(pathName);
                FileInfo[] fisBMP = d.GetFiles("*.png");
                //FileInfo[] fisBMP = d.GetFiles("*.bmp");
                int imagesCount = fisBMP.Length;
                for (int i = 0; i < imagesCount; i++)
                {
                    Image img = Image.FromFile(fisBMP[i].FullName);
                    this.mImageList.Add(img);
                }
            }
        }

        private void btn_Prev_Click(object sender, EventArgs e)
        {
            if (this.mImageList.Count != 0)
            {
                this.imageindex--;

                if (this.imageindex >= 0)
                {
                    if (this.imageindex == this.mImageList.Count)
                    {
                        this.imageindex = this.mImageList.Count - 2;
                    }
                    Image img = this.mImageList[this.imageindex];
                    pictureBox1.BackgroundImage = img;
                    pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    if (LanguageConfig.Instance.Language == "CH")
                    {
                        MessageBox.Show("已经是第一张", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (LanguageConfig.Instance.Language == "EN")
                    {
                        MessageBox.Show("It's already the first one", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nó đã là cái đầu tiên rồi.", "Mẹo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    this.imageindex = 0;
                }
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    MessageBox.Show("图片未加载", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    MessageBox.Show("Picture not loaded", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Hình chưa tải", "Mẹo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (this.mImageList.Count != 0)
            {
                this.imageindex++;

                if (this.imageindex < this.mImageList.Count)
                {
                    if (this.imageindex <= -1)
                    {
                        this.imageindex = 1;
                    }
                    Image img = this.mImageList[this.imageindex];
                    pictureBox1.BackgroundImage = img;
                    pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    if (LanguageConfig.Instance.Language == "CH")
                    {
                        MessageBox.Show("已经是最后一张", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (LanguageConfig.Instance.Language == "EN")
                    {
                        MessageBox.Show("It's the last one", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Đó là cái cuối cùng.", "Mẹo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    this.imageindex = this.mImageList.Count - 1;
                }
            }
            else
            {
                if (LanguageConfig.Instance.Language == "CH")
                {
                    MessageBox.Show("图片未加载", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (LanguageConfig.Instance.Language == "EN")
                {
                    MessageBox.Show("Picture not loaded", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Hình chưa tải", "Mẹo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
