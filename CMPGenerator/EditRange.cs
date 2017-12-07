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
        DataHandler.Map map { get; }
        public event EventHandler RangeEdited = new EventHandler((o, e) => { });

        public void updateMapSize()
        {
            xOffsetNumericUpDown.Value = Math.Min(xOffsetNumericUpDown.Value, xOffsetNumericUpDown.Maximum = map.width - map.xRange);
            yOffsetNumericUpDown.Value = Math.Min(yOffsetNumericUpDown.Value, yOffsetNumericUpDown.Maximum = map.height - map.yRange);
            xRangeNumericUpDown.Value = Math.Min(xRangeNumericUpDown.Value, xRangeNumericUpDown.Maximum = map.width);
            yRangeNumericUpDown.Value = Math.Min(yRangeNumericUpDown.Value, yRangeNumericUpDown.Maximum = map.height);
        }

        public EditRange(DataHandler.Map map)
        {
            this.map = map;
            InitializeComponent();

            xOffsetNumericUpDown.Value = map.xOffset;
            xOffsetNumericUpDown.Maximum = map.width - map.xRange;

            yOffsetNumericUpDown.Value = map.yOffset;
            yOffsetNumericUpDown.Maximum = map.height - map.yRange;

            xRangeNumericUpDown.Value = map.xRange;
            xRangeNumericUpDown.Maximum = map.width;

            yRangeNumericUpDown.Value = map.yRange;
            yRangeNumericUpDown.Maximum = map.height;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            map.xOffset = (short)xOffsetNumericUpDown.Value;
            RangeEdited(this, new EventArgs());
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            map.yOffset = (short)yOffsetNumericUpDown.Value;
            RangeEdited(this, new EventArgs());
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            map.xRange = (short)xRangeNumericUpDown.Value;
            xOffsetNumericUpDown.Value = Math.Min(xOffsetNumericUpDown.Value, xOffsetNumericUpDown.Maximum = map.width - map.xRange);
            RangeEdited(this, new EventArgs());
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            map.yRange = (short)yRangeNumericUpDown.Value;
            yOffsetNumericUpDown.Maximum = Math.Min(yOffsetNumericUpDown.Value, map.height - map.yRange);
            RangeEdited(this, new EventArgs());
        }
    }
}
