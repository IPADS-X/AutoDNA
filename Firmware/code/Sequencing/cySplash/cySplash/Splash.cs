using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Threading.Tasks;

namespace cySplash
{
    public class Splash
    {
        public static SplashForm mSplashForm;
        public static ProgressBar mProgressBar;
        public static Label mLabel;
        private delegate void ShowMessageEventHandler(string s, Color cr);
        public static ManualResetEvent mEvent = mEvent = new ManualResetEvent(false);

        /// <summary>
        /// 显示加载窗体
        /// </summary>
        /// <param name="objName">封面图片路径</param>
        public static void ShowSplash(object objName)
        {
            mSplashForm = new SplashForm();
            if (objName != null)
            {
                string imageName = objName.ToString();
                if (imageName != "" && System.IO.File.Exists(imageName))
                {
                    PictureBox pb = mSplashForm.Controls["pictureBox1"] as PictureBox;
                    pb.BackgroundImageLayout = ImageLayout.Stretch;
                    pb.BackgroundImage = System.Drawing.Image.FromFile(imageName);
                }
            }
            mLabel = mSplashForm.Controls["label1"] as Label;
            mProgressBar = mSplashForm.Controls["progressBar1"] as ProgressBar;
            mProgressBar.Style = ProgressBarStyle.Marquee;
            mProgressBar.MarqueeAnimationSpeed = 20;
            mEvent.Set();
            mSplashForm.ShowDialog();
        }

        /// <summary>
        /// 销毁加载窗体
        /// </summary>
        public static void KillSplash()
        {
            if (mSplashForm.InvokeRequired)
            {
                mSplashForm.Invoke(new MethodInvoker(KillSplash), new object[] { });
            }
            else
            {
                if (mSplashForm.Created)
                {
                    mSplashForm.Close();
                    mSplashForm.Dispose();
                }
            }
        }

        /// <summary>
        /// 更新加载信息
        /// </summary>
        /// <param name="tip">加载进度提示</param>
        /// <param name="col">背景颜色</param>
        public static void ShowSplashTisp(string tip , Color col)
        {
            if (mLabel.InvokeRequired)
            {
                mLabel.Invoke(new ShowMessageEventHandler(ShowSplashTisp), new object[] { tip, col });
            }
            else
            {
                mLabel.Text = tip;
                mLabel.BackColor = col;
            }
        }
    }
}
