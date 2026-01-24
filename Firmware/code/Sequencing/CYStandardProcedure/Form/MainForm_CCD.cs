using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using CYAutoFramework;

namespace CYStandardProcedure
{
    public partial class MainForm_CCD : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        public static MainForm_CCD mainformccd;
        #region 窗体控件自适应代码          
        private void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
        }

        #endregion 窗体控件自适应代码

        public MainForm_CCD()
        {
            InitializeComponent();
            mainformccd = this;
        }

        private void MainForm_CCD_Load(object sender, EventArgs e)
        {
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
            Global_loadPicStart();
            RobotStation.LoadPicAction += Global_loadPic1Action;
            RobotStation.LoadPicAction2 += Global_loadPic2Action;
            RobotStation.LoadPicAction3 += Global_loadPic3Action;
            RobotStation.LoadPicAction4 += Global_loadPic4Action;
            RobotStation.LoadPicAction5 += Global_loadPic5Action;
            RobotStation.LoadPicAction6 += Global_loadPic6Action;
            RobotStation.LoadPicAction7 += Global_loadPic7Action;
        }
        /// <summary>
        /// 开机填充图片
        /// </summary>
        public void Global_loadPicStart()
        {
            try
            {
                string paths = "";
                if (!Directory.Exists(Application.StartupPath + @"\CCDImage"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\CCDImage");
                }
                pictureBox1.BackgroundImage = null;
                paths = Application.StartupPath + @"\CCDImage\1.jpg";
                if (File.Exists(paths))
                {
                    FileStream fs = new FileStream(paths, FileMode.Open, FileAccess.Read);
                    Image img = Image.FromStream(fs);
                    pictureBox1.BackgroundImage = img;
                    fs.Close();
                }

                pictureBox3.BackgroundImage = null;
                paths = Application.StartupPath + @"\CCDImage\2.jpg";
                if (File.Exists(paths))
                {
                    FileStream fs = new FileStream(paths, FileMode.Open, FileAccess.Read);
                    Image img = Image.FromStream(fs);
                    pictureBox3.BackgroundImage = img;
                    fs.Close();
                }

                pictureBox4.BackgroundImage = null;
                paths = Application.StartupPath + @"\CCDImage\3.jpg";
                if (File.Exists(paths))
                {
                    FileStream fs = new FileStream(paths, FileMode.Open, FileAccess.Read);
                    Image img = Image.FromStream(fs);
                    pictureBox4.BackgroundImage = img;
                    fs.Close();
                }

                pictureBox5.BackgroundImage = null;
                paths = Application.StartupPath + @"\CCDImage\4.jpg";
                if (File.Exists(paths))
                {
                    FileStream fs = new FileStream(paths, FileMode.Open, FileAccess.Read);
                    Image img = Image.FromStream(fs);
                    pictureBox5.BackgroundImage = img;
                    fs.Close();
                }

                pictureBox2.BackgroundImage = null;
                paths = Application.StartupPath + @"\CCDImage\5.jpg";
                if (File.Exists(paths))
                {
                    FileStream fs = new FileStream(paths, FileMode.Open, FileAccess.Read);
                    Image img = Image.FromStream(fs);
                    pictureBox2.BackgroundImage = img;
                    fs.Close();
                }

                pictureBox6.BackgroundImage = null;
                paths = Application.StartupPath + @"\CCDImage\6.bmp";
                if (File.Exists(paths))
                {
                    FileStream fs = new FileStream(paths, FileMode.Open, FileAccess.Read);
                    Image img = Image.FromStream(fs);
                    pictureBox6.BackgroundImage = img;
                    fs.Close();
                }
                pictureBox7.BackgroundImage = null;
                paths = Application.StartupPath + @"\CCDImage\7.bmp";
                if (File.Exists(paths))
                {
                    FileStream fs = new FileStream(paths, FileMode.Open, FileAccess.Read);
                    Image img = Image.FromStream(fs);
                    pictureBox7.BackgroundImage = img;
                    fs.Close();
                }

            }
            catch { }
        }

        /// <summary>
        /// 液体检测
        /// </summary>
        public void Global_loadPic1Action()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    if (!Directory.Exists("E:\\ShowImage\\上相机工控界面"))
                    {
                        Directory.CreateDirectory("E:\\ShowImage\\上相机工控界面");
                    }
                    pictureBox1.BackgroundImage = null;
                    string path = "E:\\SaveImage\\Tcp\\0\\1.jpg";
                    string path2 = "E:\\ShowImage\\上相机工控界面\\1.jpg";
                    if (File.Exists(path))
                    {
                        File.Copy(path, path2, true);
                        FileStream fs = new FileStream(path2, FileMode.Open, FileAccess.Read);
                        Image img = Image.FromStream(fs);
                        pictureBox1.BackgroundImage = img;
                        fs.Close();
                    }
                }));
            }
            catch { }
        }
        /// <summary>
        /// 取料定位
        /// </summary>
        public void Global_loadPic2Action()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    if (!Directory.Exists("E:\\ShowImage\\下相机工控界面"))
                    {
                        Directory.CreateDirectory("E:\\ShowImage\\下相机工控界面");
                    }
                    pictureBox2.BackgroundImage = null;
                    string path = "E:\\SaveImage\\Tcp\\1\\1.jpg";
                    string path2 = "E:\\ShowImage\\下相机工控界面\\1.jpg";
                    if (File.Exists(path))
                    {
                        File.Copy(path, path2, true);
                        FileStream fs = new FileStream(path2, FileMode.Open, FileAccess.Read);
                        Image img = Image.FromStream(fs);
                        pictureBox2.BackgroundImage = img;
                        fs.Close();
                    }
                }));
            }
            catch { }
        }
        /// <summary>
        /// 盖板类型
        /// </summary>
        public void Global_loadPic3Action()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    if (!Directory.Exists("E:\\ShowImage\\上相机工控界面"))
                    {
                        Directory.CreateDirectory("E:\\ShowImage\\上相机工控界面");
                    }
                    pictureBox3.BackgroundImage = null;
                    string path = "E:\\SaveImage\\Tcp\\0\\1.jpg";
                    string path2 = "E:\\ShowImage\\上相机工控界面\\1.jpg";
                    if (File.Exists(path))
                    {
                        File.Copy(path, path2, true);
                        FileStream fs = new FileStream(path2, FileMode.Open, FileAccess.Read);
                        Image img = Image.FromStream(fs);
                        pictureBox3.BackgroundImage = img;
                        fs.Close();
                    }
                }));
            }
            catch { }
        }
        /// <summary>
        /// 盖板有无
        /// </summary>
        public void Global_loadPic4Action()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    if (!Directory.Exists("E:\\ShowImage\\上相机工控界面"))
                    {
                        Directory.CreateDirectory("E:\\ShowImage\\上相机工控界面");
                    }
                    pictureBox4.BackgroundImage = null;
                    string path = "E:\\SaveImage\\Tcp\\0\\1.jpg";
                    string path2 = "E:\\ShowImage\\上相机工控界面\\1.jpg";
                    if (File.Exists(path))
                    {
                        File.Copy(path, path2, true);
                        FileStream fs = new FileStream(path2, FileMode.Open, FileAccess.Read);
                        Image img = Image.FromStream(fs);
                        pictureBox4.BackgroundImage = img;
                        fs.Close();
                    }
                }));
            }
            catch { }
        }
        /// <summary>
        /// 放料定位
        /// </summary>
        public void Global_loadPic5Action()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    if (!Directory.Exists("E:\\ShowImage\\上相机工控界面"))
                    {
                        Directory.CreateDirectory("E:\\ShowImage\\上相机工控界面");
                    }
                    pictureBox5.BackgroundImage = null;
                    string path = "E:\\SaveImage\\Tcp\\0\\1.jpg";
                    string path2 = "E:\\ShowImage\\上相机工控界面\\1.jpg";
                    if (File.Exists(path))
                    {
                        File.Copy(path, path2, true);
                        FileStream fs = new FileStream(path2, FileMode.Open, FileAccess.Read);
                        Image img = Image.FromStream(fs);
                        pictureBox5.BackgroundImage = img;
                        fs.Close();
                    }
                }));
            }
            catch { }
        }

        /// <summary>
        /// 上3D相机平整度
        /// </summary>
        public void Global_loadPic6Action()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    if (!Directory.Exists("E:\\ShowImage\\上3D相机工控界面"))
                    {
                        Directory.CreateDirectory("E:\\ShowImage\\上3D相机工控界面");
                    }
                    pictureBox7.BackgroundImage = null;
                    string path = "E:\\SaveImage\\Tcp\\2\\1.bmp";
                    string path2 = "E:\\ShowImage\\上3D相机工控界面\\1.bmp";
                    if (File.Exists(path))
                    {
                        File.Copy(path, path2, true);
                        FileStream fs = new FileStream(path2, FileMode.Open, FileAccess.Read);
                        Image img = Image.FromStream(fs);
                        pictureBox7.BackgroundImage = img;
                        fs.Close();
                    }
                }));
            }
            catch { }
        }
        /// <summary>
        /// 下3D相机pin针
        /// </summary>
        public void Global_loadPic7Action()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    if (!Directory.Exists("E:\\ShowImage\\下3D相机工控界面"))
                    {
                        Directory.CreateDirectory("E:\\ShowImage\\下3D相机工控界面");
                    }
                    pictureBox6.BackgroundImage = null;
                    string path = "E:\\SaveImage\\Tcp\\3\\1.bmp";
                    string path2 = "E:\\ShowImage\\下3D相机工控界面\\1.bmp";
                    if (File.Exists(path))
                    {
                        File.Copy(path, path2, true);
                        FileStream fs = new FileStream(path2, FileMode.Open, FileAccess.Read);
                        Image img = Image.FromStream(fs);
                        pictureBox6.BackgroundImage = img;
                        fs.Close();
                    }
                }));
            }
            catch { }
        }


        public void PicClear()
        {
            this.Invoke(new Action(() =>
            {
                pictureBox1.BackgroundImage = null;
                pictureBox2.BackgroundImage = null;
            }));
        }

    }
}