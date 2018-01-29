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
using System.Collections.Specialized;

namespace CMPGenerator
{
    public partial class FormMap : Form
    {
        public MapComparer dataHandler { get; private set; }

        public FormTileset tileset { get; private set; }
        private bool tilesetShown { get; set; } = false;

        private EditRange editRange { get; set; }
        private bool editRangeShown { get; set; } = false;

        private bool mapLoaded { get; set; }
        public MapComparer.Map map { get; }
        private Bitmap mapImage { get; set; } = null;
        private float zoomLevel { get; set; } = 1;
        public bool master { get; }

        public FormMap(bool master) : this(master, new MapComparer.Map()) { }
        public FormMap(bool master, MapComparer.Map map)
        {
            this.master = master;

            this.map = map;
            map.MapLoaded += MapLoaded;
            (map.data as INotifyCollectionChanged).CollectionChanged += Data_CollectionChanged;
            map.tileset.TilesetLoaded += DrawMap;
            map.RangeUpdated += DrawMap;
            map.MapResized += DrawMap;
            
            tileset = new FormTileset(map.tileset);
            tileset.Owner = this;
            tileset.FormClosing += (o, e) => { e.Cancel = true; viewTileset.Checked = false; }; ;
            
            InitializeComponent();

            viewTileset.CheckedChanged += delegate { tileset.Visible = viewTileset.Checked; }; //UNSURE maybe use lamda?            
            viewRangeEditor.CheckedChanged += delegate { editRange.Visible = viewRangeEditor.Checked; };

            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Replace)
            {
                int y = e.OldStartingIndex / map.width;
                int x = e.OldStartingIndex - (y * map.width);
                DrawTile(x, y);
            }
            else
            {
                //TODO maybe use a for loop and drawtile? maybe that would let me get rid of initializeMap?
                InitializeMap();
            }
            DrawMap();
        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if(mapLoaded && ModifierKeys == Keys.Control)
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

        public void MapLoaded(object sender, MapComparer.Map.MapLoadedEventArgs e)
        {
            this.Text = $"Map {(master ? "1" : "2")}: {Path.GetFileName(e.path)}";
            saveAsToolStripMenuItem.Enabled = true;
            mapLoaded = true;
            
            viewRangeEditor.Enabled = true;
            viewTileset.Enabled = true;
            applyTSCToolStripMenuItem.Enabled = true;
            editRange = new EditRange(map);
            editRange.Owner = this;
            editRange.FormClosing += (_o, _e) => { _e.Cancel = true; viewRangeEditor.Checked = false; };

            viewChangeSize.Enabled = true;

            if (mapImage == null && map.tileset.image != null)
            {
                InitializeMap();
                DrawMap();

                //Sets the window as big as the map, or as big as the screen if that would cuase it to go offscreen
                Screen monitor = Screen.FromControl(this);
                Height = Math.Min(pictureBox1.Image.Height + 63, monitor.Bounds.Height - Top);
                Width = Math.Min(pictureBox1.Image.Width + 16, monitor.Bounds.Width - Left);

                pictureBox1.Invalidate();
                //pictureBox1.Refresh();
            }
            else
            {
                InitializeMap();
                DrawMap();
            }
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

        //TODO refactor these five functions
        private void DrawMap(object sender, EventArgs e)
        {
            if(map.Loaded)
            {
                InitializeMap();
                DrawMap();
            }
        }
        private void DrawMap()
        {
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            pictureBox1.Image = new Bitmap(DrawRange(mapImage), (int)(mapImage.Width * zoomLevel), (int)(mapImage.Height * zoomLevel));
            pictureBox1.Refresh();
        }

        private void InitializeMap()
        {
            mapImage = new Bitmap(map.width * map.tileset.tileSize, map.height * map.tileset.tileSize);

            for (int i = 0; i < map.height; i++)
            {
                for (int _i = 0; _i < map.width; _i++)
                {
                    DrawTile(_i, i);
                }
            }
        }
        
        private void DrawTile(int mapTileXCoord, int mapTileYCoord)
        {
            if (mapImage == null)
            {
                InitializeMap();
            }
            else
            {
                Bitmap tileToDraw = new Bitmap(map.tileset.tileSize, map.tileset.tileSize);
                using (Graphics g = Graphics.FromImage(tileToDraw))
                    g.FillRectangle(new SolidBrush(pictureBox1.BackColor), 0, 0, tileToDraw.Width, tileToDraw.Height);

                byte? tile = map.data[(mapTileYCoord * map.width) + mapTileXCoord];
                if (tile != null)
                {
                    int tilesetTileYOffset = tile.Value / (map.tileset.image.Width / map.tileset.tileSize);
                    int tilesetTileXOffset = tile.Value - ((map.tileset.image.Width / map.tileset.tileSize) * tilesetTileYOffset);

                    tileToDraw = map.tileset.image.Clone(
                            new Rectangle(
                                tilesetTileXOffset * map.tileset.tileSize,
                                tilesetTileYOffset * map.tileset.tileSize,
                                map.tileset.tileSize,
                                map.tileset.tileSize),
                            PixelFormat.DontCare //TODO setting this to 4bpp is causing issues for some reason...
                            );
                }

                using (Graphics g = Graphics.FromImage(mapImage))
                {
                    g.DrawImage(tileToDraw,
                        mapTileXCoord * map.tileset.tileSize,
                        mapTileYCoord * map.tileset.tileSize
                        );
                }
            }
        }


        //TODO remove this event and make it so you can click + drag
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / (int)(map.tileset.tileSize * zoomLevel);
            int y = e.Y / (int)(map.tileset.tileSize * zoomLevel);

            //If the tile clicked is actually on the map, and the tile clicked would actually change to something different
            if(((y * map.width) + x < map.data.Count) ? map.data[(y * map.width) + x] != map.tileset.selectedTile : false) //TODO review if this is needed
            {
                    map.data[(y * map.width) + x] = map.tileset.selectedTile;
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
                map.Save(sfd.FileName);
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
                if(MessageBox.Show("Would you like to select a new tileset as well?","Open Map", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    OpenFileDialog otd = new OpenFileDialog()
                    {
                        Title = "Choose a tilset to use...",
                        Filter = "Bitmaps|*.bmp;*.pbm|All Files|*.*"
                    };
                    if (otd.ShowDialog() == DialogResult.OK)
                    {
                        //TODO add options
                        map.tileset.Load(otd.FileName,16);

                    }
                }

                MapImporter mi = new MapImporter();
                if(mi.ShowDialog() == DialogResult.OK)
                    map.Load(omd.FileName,mi.resize,mi.coords,mi.overwriteMode);
            }
        }

        private void applyTSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyTSC at = new ApplyTSC();
            if(at.ShowDialog() == DialogResult.OK && at.Tsc.Length >= 8) //8 is the length of an <FL+/<FL- command, which is the smallest one recognised.
            {
                map.ApplyTSC(at.Tsc);
            }
        }

        private void changeSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapSize ms = new MapSize(map.width, map.height);
            if(ms.ShowDialog() == DialogResult.OK)
            {
                map.Resize(ms.width, ms.height, ms.resizeMode);
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
                tilesetShown = (tileset != null) ? tileset.Visible : false;
                editRangeShown = (editRange != null) ? editRange.Visible : false;

                viewTileset.Checked = false;
                viewRangeEditor.Checked = false;
            }
        }
    }
}
