using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMPGenerator
{
    public partial class ApplyTSC : Form
    {
        public string Tsc { get; private set; }

        public ApplyTSC()
        {
            InitializeComponent();
        }

        private void RichTextbox_TextChanged(object sender, EventArgs e)
        {
            Tsc = RichTextbox.Text;
        }
    }
}
