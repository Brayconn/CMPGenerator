using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//TODO make the anchors better
namespace CMPGenerator
{
    public partial class EditRange : Form
    {
        private static short Clamp(short val, short min, short max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        MapComparer.Map map { get; }

        private void UpdateMapSize(object sender, EventArgs e)
        {
            UpdateMapSize();
        }
        private void UpdateMapSize()
        {
            xRangeNumericUpDown.Value = Math.Min(map.xRange, xRangeNumericUpDown.Maximum = map.width);
            yRangeNumericUpDown.Value = Math.Min(map.yRange, yRangeNumericUpDown.Maximum = map.height);

            xOffsetNumericUpDown.Value = Clamp(map.xOffset, 0, (short)(xOffsetNumericUpDown.Maximum = map.width - map.xRange + 1));
            yOffsetNumericUpDown.Value = Clamp(map.yOffset, 0, (short)(yOffsetNumericUpDown.Maximum = map.height - map.yRange + 1));
        }

        public EditRange(MapComparer.Map map)
        {
            this.map = map;
            map.RangeUpdated += UpdateMapSize;
            map.MapResized += UpdateMapSize;

            InitializeComponent();

            UpdateMapSize();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            map.xOffset = (short)xOffsetNumericUpDown.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            map.yOffset = (short)yOffsetNumericUpDown.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            map.xRange = (short)xRangeNumericUpDown.Value;
            //xOffsetNumericUpDown.Value = Math.Min(xOffsetNumericUpDown.Value, xOffsetNumericUpDown.Maximum = map.width - map.xRange); //UNSURE use conditional operator?
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            map.yRange = (short)yRangeNumericUpDown.Value;
            //yOffsetNumericUpDown.Maximum = Math.Min(yOffsetNumericUpDown.Value, yOffsetNumericUpDown.Maximum = map.height - map.yRange); //UNSURE use conditional operator?
        }
    }
}
