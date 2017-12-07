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
using System.Drawing.Imaging;

namespace CMPGenerator
{
    public partial class FormMap : Form
    {
        public DataHandler dataHandler { get; private set; }

        public FormTileset tileset { get; private set; }
        private bool tilesetShown { get; set; } = false;

        private EditRange editRange { get; set; }
        private bool editRangeShown { get; set; } = false;

        public DataHandler.Map map { get; private set; }
        private Bitmap mapImage { get; set; } = null;
        private float zoomLevel { get; set; } = 1;
        private int mapNumber { get; set; }

        public FormMap(DataHandler dh, int mn)
        {
            dataHandler = dh;
            tileset = new FormTileset();
            mapNumber = mn;

            tileset.FormClosing += (_o, _e) => { _e.Cancel = true; viewTileset.Checked = false; }; ;

            InitializeComponent();

            viewTileset.CheckedChanged += delegate { toggleForm(tileset, viewTileset.Checked); };

            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if(ModifierKeys == Keys.Control)
            { 
                //TODO memory leak?
                //float newZoom = e.Delta / (float)(Math.Abs(e.Delta) * 4);
                float newZoom = (e.Delta >= 0) ? (float)0.25 : (float)-0.25;
                if (zoomLevel + newZoom > 0 && zoomLevel + newZoom <= 5)
                {
                    zoomLevel += newZoom;
                    DrawMap();
                }
            }
        }

        private static void toggleForm(Form form, bool state)
        {
            if (state)
                form.Show();
            else
                form.Hide();
        }

        public void FileLoaded(object sender, DataHandler.FileLoadedEventArgs e)
        {
            map = e.map;
            this.Text = $"(Map {e.number}: {Path.GetFileName(e.path)})";
            saveAsToolStripMenuItem.Enabled = true;

            viewRangeEditor.Enabled = true;
            viewTileset.Enabled = true;
            applyTSCToolStripMenuItem.Enabled = true;
            editRange = new EditRange(map);
            editRange.RangeEdited += UpdateRange;
            editRange.FormClosing += (_o, _e) => { _e.Cancel = true; viewRangeEditor.Checked = false; };
            viewRangeEditor.CheckedChanged += delegate { toggleForm(editRange, viewRangeEditor.Checked); };

            viewChangeSize.Enabled = true;            
        }

        private void UpdateRange(object sender, EventArgs e)
        {
            map.xOffset = (short)editRange.xOffsetNumericUpDown.Value;
            map.yOffset = (short)editRange.yOffsetNumericUpDown.Value;
            dataHandler.GenerateTSC();
            DrawMap();
        }

        private Bitmap DrawRange(Bitmap input)
        {
            Bitmap b = new Bitmap(input);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.DrawRectangle(new Pen(Color.Red),
                    map.xOffset * map.tileset.tileSize,
                    map.yOffset * map.tileset.tileSize,
                    (map.xRange * map.tileset.tileSize) - 1,
                    (map.yRange * map.tileset.tileSize) - 1
                    );
            }
            return b;
        }

        private void DrawMap()
        {
            if(pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            pictureBox1.Image = new Bitmap(DrawRange(mapImage), (int)(mapImage.Width * zoomLevel), (int)(mapImage.Height * zoomLevel));
            pictureBox1.Refresh();
        }

        private void InitializeMap()
        {
            mapImage = new Bitmap(map.width * map.tileset.tileSize, map.height * map.tileset.tileSize);

            for (int i = 0; i < map.data.Count; i++)
            {
                for (int _i = 0; _i < map.data[i].Count; _i++)
                {
                    DrawTile(_i, i, mapImage);
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (mapImage == null && map != null)
            {
                InitializeMap();
                DrawMap();

                //Sets the window as big as the map, or as big as the screen if that would cuase it to go offscreen
                var monitor = Screen.FromControl(this);
                Height = Math.Min(pictureBox1.Image.Height + 63, monitor.Bounds.Height - Top);
                Width = Math.Min(pictureBox1.Image.Width + 16, monitor.Bounds.Width - Left);

                pictureBox1.Invalidate();
                //pictureBox1.Refresh();
            }
        }        

        private void DrawTile(int mapTileXCoord, int mapTileYCoord, Bitmap outputMap)
        {
            if (mapImage == null)
            {
                InitializeMap();
            }
            else
            {
                using (Graphics g = Graphics.FromImage(mapImage))
                {
                    int tilesetTileYOffset = map.data[mapTileYCoord][mapTileXCoord] / map.tileset.width;
                    int tilesetTileXOffset = map.data[mapTileYCoord][mapTileXCoord] - (map.tileset.width * tilesetTileYOffset);
                    
                    Bitmap tileToDraw = map.tileset.image.Clone(
                        new Rectangle(
                            tilesetTileXOffset * map.tileset.tileSize,
                            tilesetTileYOffset * map.tileset.tileSize,
                            map.tileset.tileSize,
                            map.tileset.tileSize),
                        PixelFormat.Format4bppIndexed //TODO not sure if nessecary?
                        );

                    g.DrawImage(tileToDraw,
                            mapTileXCoord * map.tileset.tileSize,
                            mapTileYCoord * map.tileset.tileSize
                            );
                }
            }
        }

        public class TileChangedEventArgs : EventArgs
        {
            public int x { get; }
            public int y { get; }
                        
            public byte oldTile { get; }
            public byte newTile { get; }

            public int finalTile { get; }

            public TileChangedEventArgs(int x, int y, byte oldTile, byte newTile, int finalTile)
            {
                this.x = x;
                this.y = y;
                this.oldTile = oldTile;
                this.newTile = newTile;
                this.finalTile = finalTile;
            }
        }

        public event EventHandler<TileChangedEventArgs> TileChanged = new EventHandler<TileChangedEventArgs>((o, e) => { });
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / (int)(map.tileset.tileSize * zoomLevel);
            int y = e.Y / (int)(map.tileset.tileSize * zoomLevel);

            if(x <= map.width && y <= map.height)
            {
                if (map.data[y][x] != map.tileset.selectedTile) //HACK wish I didn't have to split this up to avoid indexoutofrange exceptions
                {
                    map.data[y][x] = map.tileset.selectedTile;

                    if (x >= map.xOffset && x < map.xOffset + map.xRange && 
                        y >= map.yOffset && y < map.yOffset + map.yRange) //TODO maybe put this in the UpdateTSC function?
                    {
                        TileChanged(this, new TileChangedEventArgs(x-map.yOffset,
                            y-map.yOffset,
                            map.data[y][x],
                            map.tileset.selectedTile,
                            (map.tileset.width * map.tileset.height) - 1)
                            );
                    }
                    
                    DrawTile(x, y, mapImage);
                    DrawMap();
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Choose where to save the map...",
                Filter = "Pixel Map Files|*.pxm|All Files|*.*"
            };
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                dataHandler.SaveMap(mapNumber, sfd.FileName);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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
                }
            }
        }

        private void applyTSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyTSC at = new ApplyTSC();
            if(at.ShowDialog() == DialogResult.OK && at.Tsc.Length >= 8) //8 is the length of an <FL+/<FL- command, which is the smallest one recognised.
            {
                dataHandler.ApplyTSC(at.Tsc, mapNumber);
                InitializeMap();
                DrawMap();
                dataHandler.GenerateTSC();
            }
        }

        private void changeSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapSize ms = new MapSize(map.width, map.height);

            if(ms.ShowDialog() == DialogResult.OK)
            {
                map.Resize(ms.width, ms.height, ms.resizeMode);
                editRange.updateMapSize();
                InitializeMap();
                DrawMap();
                if (ms.resizeMode == DataHandler.Map.ResizeMode.OutOfBounds)
                    dataHandler.GenerateTSC();
            }
        }

        private void FormMap_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible)
            {
                viewTileset.Checked = tilesetShown;
                viewRangeEditor.Checked = editRangeShown;
            }
            else
            {
                tilesetShown = tileset.Visible;
                editRangeShown = editRange.Visible;

                viewTileset.Checked = false;
                viewRangeEditor.Checked = false;
            }
        }
    }
}
