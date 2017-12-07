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

        private void DrawMap()
        {
            Bitmap b = new Bitmap(tileset.image);
            using (Graphics g = Graphics.FromImage(b))
            {

                int y = tileset.selectedTile / tileset.width;
                int x = tileset.selectedTile - (tileset.width * y);

                g.DrawRectangle(new Pen(Color.White, 1), x * 16, y * 16, 16, 16);
            }
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            pictureBox1.Image = b;
            pictureBox1.Refresh();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Image == null && tileset != null)
            {
                DrawMap();
                
                //Sets the window as big as the map, or as big as the screen if that would cuase it to go offscreen
                var monitor = Screen.FromControl(this);
                Height = Math.Min(pictureBox1.Image.Height + 39, monitor.Bounds.Height - Top);
                Width = Math.Min(pictureBox1.Image.Width + 16, monitor.Bounds.Width - Left);

                pictureBox1.Invalidate();
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
                DrawMap();
                //pictureBox1.Refresh();
            }
        }
    }
}
