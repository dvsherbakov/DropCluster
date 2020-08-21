using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrepareImageFrm
{
    public partial class DetectParams : Form
    {

        public delegate void DelegateSetupParam(int bt, int gp, int mar, int mp);
        public event DelegateSetupParam OnAplyParam;

        public DetectParams(int bt, int gp, int mar, int mp)
        {
            InitializeComponent();
            nudBT.Value = bt;
            nudGp.Value = gp;
            nudMAR.Value = mar;
            nudMP.Value = mp;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OnAplyParam?.Invoke((int)nudBT.Value, (int)nudGp.Value, (int)nudMAR.Value, (int)nudMP.Value);
        }
    }
}
