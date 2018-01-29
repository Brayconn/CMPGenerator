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
    //TODO make sure the form actually has proper width
    public partial class MapImporter : Form
    {
        public MapComparer.Map.OverwriteMode overwriteMode = MapComparer.Map.OverwriteMode.NullAndAppend;
        public bool resize = true;
        public bool coords = false;

        public MapImporter()
        {
            InitializeComponent();

            comboBox1.DataSource = Enum.GetValues(typeof(MapComparer.Map.OverwriteMode));
            comboBox1.SelectedItem = overwriteMode;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            overwriteMode = (MapComparer.Map.OverwriteMode)Enum.Parse(typeof(MapComparer.Map.OverwriteMode), comboBox1.SelectedValue.ToString());
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            resize = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            coords = checkBox2.Checked;
        }
    }
}
