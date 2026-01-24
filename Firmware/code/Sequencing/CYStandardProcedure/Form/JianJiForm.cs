using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CYStandardProcedure
{
    public partial class JianJiForm : Form
    {
        INIFile ini = new INIFile(Application.StartupPath + "\\FileINI\\SequenceForm.ini");
        Dictionary<int, string> JianJiFormDic = new Dictionary<int, string>();
        public JianJiForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void JianJiForm_Load(object sender, EventArgs e)
        {
            JianJiFormDic.Clear();

            if (MyVariable.SingleExperiment != "")
            {
                string[] splitBarcode = MyVariable.SingleExperiment.Split('|');
                foreach (var items in splitBarcode)
                {
                    string[] splitJianJi = items.Split('-');
                    JianJiFormDic.Add(int.Parse(splitJianJi[0]), splitJianJi[1]);
                }
                foreach (Control item in this.Controls)
                {
                    if (item is TextBox && item.Name.Contains('_'))
                    {
                        string[] s = item.Name.Split('_');
                        if (JianJiFormDic.ContainsKey(int.Parse(s[1])))
                        {
                            item.Text = JianJiFormDic[int.Parse(s[1])];
                        }
                        else
                        {
                            item.Text = "";
                        }
                    }
                }
            }
            else
            {
                foreach (Control item in this.Controls)
                {
                    if (item is TextBox && item.Name.Contains('_'))
                    {
                        item.Text = "";
                    }
                }
            }

            TextBoxColor();
        }

        string[] singleExp;
        int barcodeNumber;
        private void button1_Click(object sender, EventArgs e)
        {
            MyVariable.SingleExperiment = "";
            try
            {
                foreach (Control item in this.Controls)
                {
                    if ((item is TextBox) && item.Text != "" && item.Name.Contains('_'))
                    {
                        singleExp = item.Name.Split('_');
                        barcodeNumber = int.Parse(singleExp[1]);
                        MyVariable.SingleExperiment += barcodeNumber + "-" + item.Text.Trim() + "|";         //  21-TG|22-TG|23-TG|24-TG    
                    }
                }
                if (MyVariable.SingleExperiment != "")
                {
                    MyVariable.SingleExperiment = MyVariable.SingleExperiment.Remove(MyVariable.SingleExperiment.Length - 1);
                }
                ini.Write("SingleExperiment", "Barcode", MyVariable.SingleExperiment);
                MessageBox.Show("写入成功!");

                TextBoxColor();

            }
            catch (Exception es)
            {
                MyVariable.SingleExperiment = "";
                MessageBox.Show("写入失败!");
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_clean_Click(object sender, EventArgs e)
        {
            foreach (Control item in this.Controls)
            {
                if (item is TextBox)
                {
                    item.Text = "";
                }
            }
        }

        private void TextBoxColor()
        {
            foreach (Control item in this.Controls)
            {
                if (item is TextBox)
                {
                    if (item.Text != "")
                    {
                        item.BackColor = Color.Salmon;
                    }
                    else
                    {
                        item.BackColor = Color.White;
                    }
                }
            }
        }
    }
}
