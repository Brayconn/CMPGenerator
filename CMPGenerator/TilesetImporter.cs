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
    public partial class TilesetImporter : Form
    {
        //TODO check UI
        public TilesetImporter()
        {
            InitializeComponent();
        }

        public int tileSize = 0;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            tileSize = (int)numericUpDown1.Value;
        }
    }
}
