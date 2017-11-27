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
        public DataHandler dataHandler { get; set; }

        private FormMap Map1 { get; set; }
        private FormMap Map2 { get; set; }
        
        public FormMain(DataHandler dh)
        {
            dataHandler = dh;
            dataHandler.TSCUpdated += (o, e) => { richTextBox1.Text = e.TSC; };

            //Tileset1 = new FormTileset(dataHandler);
            //Tileset2 = new FormTileset(dataHandler);
            Map1 = new FormMap(dataHandler, 1);
            Map2 = new FormMap(dataHandler, 2);

            dataHandler.Map1Loaded += FilesLoaded;
            dataHandler.Map2Loaded += FilesLoaded;

            dataHandler.Map1Loaded += Map1.FileLoaded;
            dataHandler.Map2Loaded += Map2.FileLoaded;
            Map1.TileChanged += dataHandler.UpdateTSC;
            Map2.TileChanged += dataHandler.UpdateTSC;

            dataHandler.Map1Loaded += Map1.tileset.FileLoaded; //TODO maybe don't use this event???????
            dataHandler.Map2Loaded += Map2.tileset.FileLoaded;
            
            //Redirect the FormClosing event to go through my own code
            Map1.FormClosing += (_o, _e) => { _e.Cancel = true; viewMap1.Checked = false; };
            Map2.FormClosing += (_o, _e) => { _e.Cancel = true; viewMap2.Checked = false; };
            Map1.tileset.FormClosing += (_o, _e) => { _e.Cancel = true; viewTileset1.Checked = false; }; ;
            Map2.tileset.FormClosing += (_o, _e) => { _e.Cancel = true; viewTileset2.Checked = false; }; ;

            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //Redirect the CheckedChanged event to go through aprox. the same code
            viewMap1.CheckedChanged += delegate { toggleForm(Map1, viewMap1.Checked); };
            viewMap2.CheckedChanged += delegate { toggleForm(Map2, viewMap2.Checked); };
            viewTileset1.CheckedChanged += delegate { toggleForm(Map1.tileset, viewTileset1.Checked); };
            viewTileset2.CheckedChanged += delegate { toggleForm(Map2.tileset, viewTileset2.Checked); };
        }

        private void FilesLoaded(object sender, DataHandler.FileLoadedEventArgs e)
        {
            if(dataHandler.map1 != null && dataHandler.map2 != null)
            {
                exportToolStripMenuItem.Enabled = true;
                regenerateTSCToolStripMenuItem.Enabled = true;
            }
        }

        #region Opening Code

        private void openMap1_Click(object sender, EventArgs e)
        {
            viewMap1.Checked = openMap(1);
        }

        private void openMap2_Click(object sender, EventArgs e)
        {
            viewMap2.Checked = openMap(2);
        }

        private bool openMap(int mapNumber)
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
                    dataHandler.Load(omd.FileName, otd.FileName, mapNumber);
                    if (dataHandler.map2 == null && mapNumber == 1)
                    {
                        if (MessageBox.Show("Would you like to load this map as map 2 as well?", "Map Loading", MessageBoxButtons.YesNo) == DialogResult.No)
                            viewMap2.Checked = openMap(2);
                        else
                            dataHandler.Load(omd.FileName, otd.FileName, 2);
                    }
                    openMap2.Enabled = true;
                    openTileset2.Enabled = true;
                    return true;
                }
            }
            return false;
        }

        #endregion

        private static void toggleForm(Form form, bool state)
        {
            if (state)
                form.Show();
            else
                form.Hide();
        }

        private void cMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataHandler.TSCType = true;
            cMPToolStripMenuItem.Checked = true;
            lMPToolStripMenuItem.Checked = false;
            dataHandler.GenerateTSC();
        }

        private void lMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataHandler.TSCType = false;
            cMPToolStripMenuItem.Checked = false;
            lMPToolStripMenuItem.Checked = true;
            dataHandler.GenerateTSC();
        }

        private void ignoreIdenticalTilesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            dataHandler.ignoreIdenticalTiles = ignoreIdenticalTilesToolStripMenuItem.Checked;
            dataHandler.GenerateTSC();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void regenerateTSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataHandler.GenerateTSC();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Export TSC...",
                Filter = "Text Files|*.txt|Text Script Files|*.tsc",
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
