using CYAutoFramework;
using CYCustomControl;
using CYStandardProcedure;
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
    public partial class MaterialsForm : Form
    {
        private AutoSizeMDIChild mAutosize = new AutoSizeMDIChild();     
        void Form1_Resize(object sender, EventArgs e)
        {
            mAutosize.ControlAutoSize(this);            
        }
        public MaterialsForm()
        {
            InitializeComponent();
        }

        private void MaterialsForm_Load(object sender, EventArgs e)
        {        
            this.SizeChanged += new EventHandler(Form1_Resize);
            mAutosize.ControlInitializeSize(this);
        }      
    }
}
