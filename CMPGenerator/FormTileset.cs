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
        private MapComparer.Tileset tileset { get; set; }

        public FormTileset(MapComparer.Tileset tileset)
        {
            this.tileset = tileset;
            tileset.TilesetLoaded += TilesetLoaded;
            
            InitializeComponent();

            tileset.SelectedTileChanged += delegate
            {
                selectNullTileToolStripMenuItem.Checked = (tileset.selectedTile == null);
                DrawTileset();
            };
        }

        private void TilesetLoaded()
        {
            if (pictureBox1.Image == null && tileset != null)
            {
                DrawTileset();

                //Sets the window as big as the map, or as big as the screen if that would cuase it to go offscreen
                var monitor = Screen.FromControl(this);
                Height = Math.Min(pictureBox1.Image.Height + 66, monitor.Bounds.Height - Top);
                Width = Math.Min(pictureBox1.Image.Width + 16, monitor.Bounds.Width - Left);

                pictureBox1.Invalidate();
            }
            else
                DrawTileset();
        }

        private void DrawTileset()
        {
            Bitmap b = new Bitmap(tileset.image);
            if (tileset.selectedTile != null)
            {
                //Draw the selected tile thingy
                using (Graphics g = Graphics.FromImage(b))
                {
                    int y = tileset.selectedTile.Value / (tileset.image.Width / tileset.tileSize);
                    int x = tileset.selectedTile.Value - ((tileset.image.Width / tileset.tileSize) * y);

                    g.DrawRectangle(new Pen(Color.White, 1),
                        x * tileset.tileSize,
                        y * tileset.tileSize,
                        tileset.tileSize,
                        tileset.tileSize);
                }
            }
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            pictureBox1.Image = b;
            pictureBox1.Refresh();
        }

        private void selectNullTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!selectNullTileToolStripMenuItem.Checked)
            {
                tileset.selectedTile = null;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / tileset.tileSize;
            int y = e.Y / tileset.tileSize;

            if (x <= 16 && y <= 16)//All tilesets have 16^2 tiles on them
            {             
                tileset.selectedTile = (byte)(x + y*16);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Choose a tilset to load...",
                Filter = "Bitmaps|*.bmp;*.pbm|All Files|*.*"
            };
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                TilesetImporter ti = new TilesetImporter();
                if(ti.ShowDialog() == DialogResult.OK)
                    tileset.Load(ofd.FileName, ti.tileSize);
            }
        }


        //bool drag = false; //TODO use these to implement multi select. will need to change tileset too
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
        
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {

        }
    }
}
