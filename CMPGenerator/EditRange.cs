using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//TODO make the ancors better
namespace CMPGenerator
{
    public partial class EditRange : Form
    {
        int n1 { get; set; }
        int n2 { get; set; }
        int n1Max { get; set; }
        int n2Max { get; set; }

        public EditRange(int x, int y, int xmax, int ymax)
        {
            n1 = x;
            n2 = y;
            n1Max = xmax;
            n2Max = ymax;

            InitializeComponent();
        }

        private void EditRange_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = n1;
            numericUpDown1.Maximum = n1Max;
            numericUpDown2.Value = n2;
            numericUpDown2.Maximum = n2Max;
        }
    }
}
