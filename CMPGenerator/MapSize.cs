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
    public partial class MapSize : Form
    {
        public short width { get; private set; }
        public short height { get; private set; }
        public DataHandler.Map.ResizeMode resizeMode { get; private set; } = DataHandler.Map.ResizeMode.Default;

        public MapSize(short width, short height)
        {
            InitializeComponent();

            mapWidthNumericUpDown.Value = width;

            mapHeightNumericUpDown.Value = height;    
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            width = (short)mapWidthNumericUpDown.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            height = (short)mapHeightNumericUpDown.Value;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            resizeMode = (radioButton1.Checked) ? DataHandler.Map.ResizeMode.Default : DataHandler.Map.ResizeMode.OutOfBounds;
        }
    }
}
