using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CMPGenerator
{
    public partial class FormMain : Form
    {
        public MapComparer TSCComparer { get; set; }

        private FormMap Map1 { get; set; }
        private FormMap Map2 { get; set; }
        
        public FormMain()
        {
            Map1 = new FormMap(true);
            Map2 = new FormMap(false);

            TSCComparer = new MapComparer(Map1.map, Map2.map);
            TSCComparer.TSCModeChanged += updateTSCModeSwitches;
            TSCComparer.TSCModeLockChanged += lockTSCModeSwitches;
            TSCComparer.TSCUpdated += (o, e) =>
            {
                richTextBox1.Text = e.TSC;
            };

            Map1.map.MapLoaded += FilesLoaded;
            Map2.map.MapLoaded += FilesLoaded;

            Map1.FormClosing += (_o, _e) => { _e.Cancel = true; viewMap1.Checked = false; };
            Map2.FormClosing += (_o, _e) => { _e.Cancel = true; viewMap2.Checked = false; };

            InitializeComponent();

            viewMap1.CheckedChanged += delegate { Map1.Visible = viewMap1.Checked; };
            viewMap2.CheckedChanged += delegate { Map2.Visible = viewMap2.Checked; };
        }        

        private void FilesLoaded(object sender, EventArgs e)
        {
            if(Map1.map.Loaded && Map2.map.Loaded)
            {
                exportToolStripMenuItem.Enabled = true;
                regenerateTSCToolStripMenuItem.Enabled = true;
                //UNSURE could unsubcribe from both events now...?
            }
        }

        #region Opening Code

        //HACK part of this can probably be put into a function

        private void openMap1_Click(object sender, EventArgs e)
        {
            Tuple<string, string> mapAndTilesetPaths = ShowOpenMapUI();
            if ((mapAndTilesetPaths != null) ? Map1.map.Load(mapAndTilesetPaths.Item1, mapAndTilesetPaths.Item2) : false)
            {
                viewMap1.Checked = true;
                openMap2.Enabled = true;
                openTileset2.Enabled = true;
                if (!Map2.map.Loaded)
                {
                    if (MessageBox.Show("Would you like to load this map as map 2 as well?", "Map Loading", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        openMap2.PerformClick();
                    }
                    else if (Map2.map.Load(mapAndTilesetPaths.Item1, mapAndTilesetPaths.Item2)) //UNSURE need to test
                    {
                        viewMap2.Checked = true;
                    }
                }
            }
        }

        private void openMap2_Click(object sender, EventArgs e)
        {
            Tuple<string, string> mapAndTilesetPaths = ShowOpenMapUI();
            if (mapAndTilesetPaths != null && Map2.map.Load(mapAndTilesetPaths.Item1, mapAndTilesetPaths.Item2))
            {
                viewMap2.Checked = true;
            }
        }

        private Tuple<string,string> ShowOpenMapUI()
        {
            OpenFileDialog omd = new OpenFileDialog()
            {
                Title = "Choose a map to edit...",
                Filter = "Pixel Map Files|*.pxm|All Files|*.*"
            };
            if (omd.ShowDialog() == DialogResult.OK)
            {
                OpenFileDialog otd = new OpenFileDialog()
                {
                    Title = "Choose a tilset to use...",
                    Filter = "Bitmaps|*.bmp;*.pbm|All Files|*.*"
                };
                if (otd.ShowDialog() == DialogResult.OK)
                {
                    return new Tuple<string, string>(omd.FileName, otd.FileName);
                }
            }
            return null;
        }

        #endregion

        #region TSC Mode Switching Stuff

        private void cMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSCComparer.TSCMode = MapComparer.TSCType.CMP;
        }

        private void sMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSCComparer.TSCMode = MapComparer.TSCType.SMP;
        }

        private void updateTSCModeSwitches()
        {
            //UNSURE simplified... not sure if it's worth it though...
            sMPToolStripMenuItem.Checked = !(cMPToolStripMenuItem.Checked = (TSCComparer.TSCMode == MapComparer.TSCType.CMP));
            //sMPToolStripMenuItem.Checked = TSCComparer.TSCMode == MapComparer.TSCType.SMP;
        }

        private void lockTSCModeSwitches()
        {
            cMPToolStripMenuItem.Enabled = sMPToolStripMenuItem.Enabled = !TSCComparer.TSCModeLocked;
        }

        #endregion

        private void ignoreIdenticalTilesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            TSCComparer.ignoreIdenticalTiles = ignoreIdenticalTilesToolStripMenuItem.Checked;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void regenerateTSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TSCComparer.GenerateTSC();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Export TSC...",
                Filter = "Text Files (*.txt)|*.txt|Text Script Files (*.tsc)|*.tsc",
            };
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                byte[] text = Encoding.ASCII.GetBytes(richTextBox1.Text);
                if (sfd.FilterIndex == 2)
                {
                    byte key = text[text.Length / 2];
                    for (int i = 0; i < text.Length; i++)
                        text[i] += key;
                    text[text.Length / 2] = key;
                }
                File.WriteAllBytes(sfd.FileName, text);
            }
        }
    }
}
