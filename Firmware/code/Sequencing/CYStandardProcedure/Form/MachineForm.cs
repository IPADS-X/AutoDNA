using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CYAutoFramework;
using System.Threading;

namespace CYStandardProcedure
{
    public partial class MachineForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();
        public MachineForm()
        {
            InitializeComponent();
        }

        private void MachineForm_Load(object sender, EventArgs e)
        {
            /***窗体控件自适应***/
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);

            /***订阅语言改变事件***/
            LanguageConfig.Instance.LanguageChangeEvent += MachineForm_LanguageChangeEvent; ;
            /***触发语言改变事件***/
            LanguageConfig.Instance.ChangeLanguage(LanguageConfig.Instance.Language);
        }

        private void MachineForm_LanguageChangeEvent(string strLanguage)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            //ErrorConfig.Instance.ReadErrorCode(dataGridView1);

            string path = Path.Combine(Application.StartupPath, "Language", strLanguage, this.GetType().Namespace + ".ini");
            INIOperation ini = new INIOperation(path);
            /***改变窗体内控件值***/
            LanguageConfig.Instance.ChangeUIText(this.GetType().Name, this, ini);
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            //ErrorConfig.Instance.ReadErrorCode(dataGridView1);
        }


        #region 测试外部调用Wait方法（轴+IO）
        private void button1_Click(object sender, EventArgs e)
        {
            /***初始化参数***/
            runbreak = false;
            if (rundone == true)
            {
                rundone = false;
                runstep = 0;
            }
            /***开启线程***/
            if (testAxisIOTh == null || !testAxisIOTh.IsAlive)
            {
                //testAxisIOTh = new Thread(axisIOTest);
                //testAxisIOTh.IsBackground = true;
                //testAxisIOTh.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /***关闭线程***/
            runbreak = true;
            if (testAxisIOTh != null)
            {
                testAxisIOTh.Abort();
                Thread.Sleep(100);
            }
        }

        private Thread testAxisIOTh;

        private bool rundone = false;
        private bool runbreak = false;

        private int runstep = 0;

        //private void axisIOTest()
        //{
        //    bool runret = false;
        //    string recstr = string.Empty;
        //    string[] recarry = new string[2] { "", "" };

        //    richTextBox1.Invoke(new Action(() =>
        //    {
        //        richTextBox1.Clear();
        //    }));
        //    while (true)
        //    {
        //        Thread.Sleep(10);
        //        if (rundone == true)
        //        {
        //            //rundone = false;
        //            break;
        //        }
        //        if (runbreak == true)
        //        {
        //            break;
        //        }
        //        try
        //        {
        //            switch (runstep)
        //            {
        //                case 0:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待Z轴回零...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitSingleAxisHome(_Axis.ZAxis.ToString(),
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorHomeTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 1;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 1:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待X轴、Y轴、R轴回零...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitMultipleAxisHome(new string[] { _Axis.XAxis.ToString(), _Axis.YAxis.ToString(), _Axis.RAxis.ToString() },
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorHomeTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 2;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 2:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待R轴至待机位置...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitSingleAxisAbsMove(_Axis.RAxis.ToString(),
        //                        Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_Axis.RAxis]),
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 3;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 3:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待X轴、Y轴、Z轴至待机位置...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitMultipleAxisAbsMove(new string[] { _Axis.XAxis.ToString(), _Axis.YAxis.ToString(), _Axis.ZAxis.ToString() },
        //                        new double[] { Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_Axis.XAxis]),
        //                        Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_Axis.YAxis]),
        //                        Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.待机位置.ToString()].PosList[(int)_Axis.ZAxis])},
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 4;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 4:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待Y轴至吹离子风位置...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitSingleAxisAbsMove(_Axis.YAxis.ToString(),
        //                        100, 100, 10,
        //                        Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.吹离子风位置.ToString()].PosList[(int)_Axis.YAxis]),
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 5;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 5:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待X轴、Z轴、R轴至吹离子风位置...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitMultipleAxisAbsMove(new string[] { _Axis.XAxis.ToString(), _Axis.ZAxis.ToString(), _Axis.RAxis.ToString() },
        //                        new int[] { 100, 100, 600 }, new int[] { 100, 100, 600 }, new double[] { 10, 10, 60 },
        //                        new double[] {
        //                            Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.吹离子风位置.ToString()].PosList[(int)_Axis.XAxis]),
        //                            Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.吹离子风位置.ToString()].PosList[(int)_Axis.ZAxis]),
        //                            Convert.ToDouble(ParameConfig.Instance.PointParameDic[_PointArray.吹离子风位置.ToString()].PosList[(int)_Axis.RAxis])
        //                        },
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 6;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 6:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待Z轴正向100...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitSingleAxisRelMove(_Axis.ZAxis.ToString(), 100, "正",
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 7;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 7:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待X轴负向50...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitSingleAxisRelMove(_Axis.XAxis.ToString(), 100, 100, 10, 50, "负",
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 8;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 8:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待X轴正向100、Y轴负向60、R轴正向100...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitMultipleAxisRelMove(new string[] { _Axis.XAxis.ToString(), _Axis.YAxis.ToString(), _Axis.RAxis.ToString() },
        //                        new double[] { 100, 60, 100 }, new string[] { "正", "负", "正" },
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 9;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 9:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待Y轴正向80、Z轴负向80...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitMultipleAxisRelMove(new string[] { _Axis.YAxis.ToString(), _Axis.ZAxis.ToString() },
        //                        new int[] { 100, 100 }, new int[] { 100, 100 }, new double[] { 10, 10 },
        //                        new double[] { 80, 80 }, new string[] { "正", "负" },
        //                        Convert.ToDouble(ParameConfig.Instance.SystemParameDic[_ParamName.MotorRunTimeOut.ToString()].CurrentValue));
        //                    if (runret == true)
        //                    {
        //                        runstep = 10;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 10:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待安全光幕消失...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitSingleAxisJogMove(_Axis.RAxis.ToString(), "正", _InputCollect.安全光幕.ToString(), 20, false);
        //                    if (runret == true)
        //                    {
        //                        runstep = 11;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 11:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待启动按钮...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitSingleBtnPressDI(_InputCollect.启动按钮.ToString(), 20);
        //                    if (runret == true)
        //                    {
        //                        runstep = 12;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 12:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待安全光幕触发、左门门限消失...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitSingleAxisJogMove(_Axis.RAxis.ToString(), "负",
        //                        new string[] { _InputCollect.安全光幕.ToString(), _InputCollect.左门门限.ToString() },
        //                        new bool[] { true, false }, 40);
        //                    if (runret == true)
        //                    {
        //                        runstep = 13;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 13:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待左门门限触发、启动按钮触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitMultipleBtnPressDI(new string[] { _InputCollect.左门门限.ToString(), _InputCollect.启动按钮.ToString() }, 20);
        //                    if (runret == true)
        //                    {
        //                        runstep = 14;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 14:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待右门门限触发...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitSingleAxisJogMove(_Axis.RAxis.ToString(), 100, 100, 10, "正", _InputCollect.右门门限.ToString(), 20);
        //                    if (runret == true)
        //                    {
        //                        runstep = 15;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 15:
        //                    string signalname = string.Empty;
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待左门门限or安全光幕触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitAnyBtnPressDI(new string[] { _InputCollect.左门门限.ToString(), _InputCollect.安全光幕.ToString() }, 20, out signalname);
        //                    if (runret == true)
        //                    {
        //                        if (signalname == _InputCollect.左门门限.ToString())
        //                        {
        //                            runstep = 16;
        //                        }
        //                        else if (signalname == _InputCollect.安全光幕.ToString())
        //                        {
        //                            runstep = 17;
        //                        }
        //                        else
        //                        {
        //                            runstep = 18;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 16:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待右门门限消失、启动按钮触发...\r\n");
        //                    }));
        //                    runret = MotionConfig.Instance.WaitSingleAxisJogMove(_Axis.RAxis.ToString(), 100, 100, 10, "负",
        //                        new string[] { _InputCollect.右门门限.ToString(), _InputCollect.启动按钮.ToString() },
        //                        new bool[] { false, true }, 40);
        //                    if (runret == true)
        //                    {
        //                        runstep = 17;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 17:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待右门门限触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitSinglePhotoelectricDI(_InputCollect.右门门限.ToString(), 20);
        //                    if (runret == true)
        //                    {
        //                        runstep = 18;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 18:
        //                    runret = MotionConfig.Instance.StartJOGMove(_Axis.RAxis.ToString(), "正");
        //                    if (runret == true)
        //                    {
        //                        runstep = 19;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 19:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待左门门限消失...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitSinglePhotoelectricDI(_InputCollect.左门门限.ToString(), 20, false);
        //                    if (runret == true)
        //                    {
        //                        runstep = 20;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 20:
        //                    runret = MotionConfig.Instance.StartJOGMove(_Axis.RAxis.ToString(), 100, 100, 10, "负");
        //                    if (runret == true)
        //                    {
        //                        runstep = 21;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 21:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待右门门限消失、安全光幕触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitMultiplePhotoelectricDI(
        //                        new string[] { _InputCollect.右门门限.ToString(), _InputCollect.安全光幕.ToString() },
        //                        new bool[] { false, true }, 25);
        //                    if (runret == true)
        //                    {
        //                        runstep = 22;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 22:
        //                    runret = MotionConfig.Instance.StopJOGMove(_Axis.RAxis.ToString());
        //                    if (runret == true)
        //                    {
        //                        runstep = 23;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 23:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("备用3触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.SetSingleDO(_OutputCollect.备用3.ToString(), 1);
        //                    if (runret == true)
        //                    {
        //                        runstep = 24;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 24:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待启动按钮触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitSingleCylinderDI(_InputCollect.启动按钮.ToString(), 20);
        //                    if (runret == true)
        //                    {
        //                        runstep = 25;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 25:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("备用4触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.SetSingleDO(_OutputCollect.备用4.ToString(), 1);
        //                    if (runret == true)
        //                    {
        //                        runstep = 26;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 26:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待左门门限触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitSingleCylinderDI(_InputCollect.左门门限.ToString(), 20, false, 5);
        //                    if (runret == true)
        //                    {
        //                        runstep = 27;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 27:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("备用6触发、备用3取消...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.SetMultipleDO(
        //                        new string[] { _OutputCollect.备用6.ToString(), _OutputCollect.备用3.ToString() },
        //                        new int[] { 1, 0 });
        //                    if (runret == true)
        //                    {
        //                        runstep = 28;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 28:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待右门门限触发、安全光幕触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitMultipleCylinderDI(
        //                        new string[] { _InputCollect.右门门限.ToString(), _InputCollect.安全光幕.ToString() }, 25);
        //                    if (runret == true)
        //                    {
        //                        runstep = 29;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 29:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("备用6取消、备用4取消...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.SetMultipleDO(
        //                        new string[] { _OutputCollect.备用6.ToString(), _OutputCollect.备用4.ToString() },
        //                        new int[] { 0, 0 });
        //                    if (runret == true)
        //                    {
        //                        runstep = 30;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 30:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待右门门限触发、左门门限触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitMultipleCylinderDI(
        //                        new string[] { _InputCollect.右门门限.ToString(), _InputCollect.左门门限.ToString() },
        //                        25, new bool[] { false, true }, 5);
        //                    if (runret == true)
        //                    {
        //                        runstep = 31;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 31:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待安全光幕触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitSingleVacuumDI(_InputCollect.安全光幕.ToString(), 20, false, 5);
        //                    if (runret == true)
        //                    {
        //                        runstep = 32;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 32:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待右门门限触发，超时光源关闭...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitSingleVacuumDI(_InputCollect.右门门限.ToString(), 20, _OutputCollect.光源.ToString());
        //                    if (runret == true)
        //                    {
        //                        runstep = 33;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 33:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待安全光幕触发，左门门限触发...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitMultipleVacuumDI(
        //                        new string[] { _InputCollect.安全光幕.ToString(), _InputCollect.左门门限.ToString() },
        //                        25, new bool[] { false, true }, 5);
        //                    if (runret == true)
        //                    {
        //                        runstep = 34;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 34:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待右门门限、左门门限触发，超时备用6、备用3关闭...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.WaitMultipleVacuumDI(
        //                        new string[] { _InputCollect.右门门限.ToString(), _InputCollect.左门门限.ToString() },
        //                        25, new string[] { _OutputCollect.备用6.ToString(), _OutputCollect.备用3.ToString() });
        //                    if (runret == true)
        //                    {
        //                        runstep = 35;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 35:
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待日光灯打开的信号...\r\n");
        //                    }));
        //                    runret = IOConfig.Instance.GetSingleDO(_OutputCollect.日光灯.ToString());
        //                    if (runret == true)
        //                    {
        //                        runstep = 36;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 36:
        //                    recstr = "";
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待CCDSoftWare的反馈信息...\r\n");
        //                    }));
        //                    runret = TCPClientConfig.Instance.WaitNetData(_TcpClientModule.CCDSoftWare.ToString(), "见贤思齐！", 8, out recstr);
        //                    if (runret == true)
        //                    {
        //                        richTextBox1.Invoke(new Action(() =>
        //                        {
        //                            richTextBox1.AppendText(recstr + "见贤思齐！\r\n");
        //                        }));
        //                        runstep = 37;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 37:
        //                    recstr = "";
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待SweepCodeDevice的反馈信息...\r\n");
        //                    }));
        //                    runret = TCPClientConfig.Instance.WaitNetData(_TcpClientModule.SweepCodeDevice.ToString(), "金粟清蕊！", 8, out recstr, Encoding.Default, Encoding.Default);
        //                    if (runret == true)
        //                    {
        //                        richTextBox1.Invoke(new Action(() =>
        //                        {
        //                            richTextBox1.AppendText(recstr + "金粟清蕊！\r\n");
        //                        }));
        //                        runstep = 38;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 38:
        //                    recarry = new string[2] { "", "" };
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待CCDSoftWare、SweepCodeDevice的反馈信息...\r\n");
        //                    }));
        //                    runret = TCPClientConfig.Instance.WaitNetData(
        //                        new string[] { _TcpClientModule.CCDSoftWare.ToString(), _TcpClientModule.SweepCodeDevice.ToString() },
        //                        new string[] { "君子论迹不论心！", "论心世上无完人！" }, 8, out recarry);
        //                    if (runret == true)
        //                    {
        //                        richTextBox1.Invoke(new Action(() =>
        //                        {
        //                            richTextBox1.AppendText(recarry[0] + "君子论迹不论心！\r\n");
        //                            richTextBox1.AppendText(recarry[1] + "论心世上无完人！\r\n");
        //                        }));
        //                        runstep = 39;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;
        //                case 39:
        //                    recarry = new string[2] { "", "" };
        //                    richTextBox1.Invoke(new Action(() =>
        //                    {
        //                        richTextBox1.AppendText("等待CCDSoftWare、SweepCodeDevice的反馈信息...\r\n");
        //                    }));
        //                    runret = TCPClientConfig.Instance.WaitNetData(
        //                        new string[] { _TcpClientModule.CCDSoftWare.ToString(), _TcpClientModule.SweepCodeDevice.ToString() },
        //                        new string[] { "晓然以至道！", "初心以始终！" }, 8, out recarry,
        //                        new Encoding[] { Encoding.Default, Encoding.Default },
        //                        new Encoding[] { Encoding.Default, Encoding.Default });
        //                    if (runret == true)
        //                    {
        //                        richTextBox1.Invoke(new Action(() =>
        //                        {
        //                            richTextBox1.AppendText(recarry[0] + "晓然以至道！\r\n");
        //                            richTextBox1.AppendText(recarry[1] + "初心以始终！\r\n");
        //                        }));
        //                        runstep = 100;
        //                    }
        //                    else
        //                    {
        //                        runbreak = true;
        //                    }
        //                    break;

        //                case 100:
        //                    rundone = true;
        //                    break;
        //            }
        //        }
        //        catch (ThreadAbortException ex)
        //        {
        //            Thread.ResetAbort();
        //        }
        //        catch (Exception ex)
        //        {
        //            LogConfig.Instance.ShowMessageToList("Alarm", "测试异常！" + Environment.NewLine + ex.Message, MsgType.Alarm, Color.Red);
        //        }
        //    }
        //}
        #endregion

    }
}
