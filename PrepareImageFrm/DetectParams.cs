using System;
using System.Windows.Forms;

namespace PrepareImageFrm
{
    public partial class DetectParams : Form
    {

        public delegate void DelegateSetupParam(int bt, int gp, int mar, int mp, int zm);
        public event DelegateSetupParam OnApplyParam;

        public DetectParams(int bt, int gp, int mar, int mp, int zm)
        {
            InitializeComponent();
            nudBT.Value = bt;
            nudGp.Value = gp;
            nudMAR.Value = mar;
            nudMP.Value = mp;
            nudZm.Value = zm;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OnApplyParam?.Invoke((int)nudBT.Value, (int)nudGp.Value, (int)nudMAR.Value, (int)nudMP.Value, (int)nudZm.Value);
            Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
