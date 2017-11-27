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
    public partial class FormTileset : Form
    {
        private DataHandler.Tileset tileset { get; set; }

        public FormTileset()
        {
            InitializeComponent();
        }

        public void FileLoaded(object sender, DataHandler.FileLoadedEventArgs e)
        {
            tileset = e.map.tileset;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (tileset != null)
            {
                //e.Graphics.Clear(Color.Gray);
                e.Graphics.DrawImage(tileset.image, 0,0);

                int x = tileset.selectedTile - (tileset.width * (tileset.selectedTile / tileset.width));
                int y = (tileset.selectedTile - x) / tileset.width;
                e.Graphics.DrawRectangle(new Pen(Color.White, 1), x*16, y*16, 16, 16);
                //e.Graphics.DrawImageUnscaledAndClipped(dataHandler.loadedMaps[tilesetNumber].tileset, e.ClipRectangle);
            }
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            //TODO maybe add a hover thingy here?
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //TODO use this and MouseUnClick to implement multiselect
            int x = e.X / 16;
            int y = e.Y / 16;

            if (x <= tileset.width && y <= tileset.height)
            {             
                tileset.selectedTile =(byte)(x + y*(tileset.width));
                pictureBox1.Refresh();
            }
        }
    }
}
