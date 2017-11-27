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
        public DataHandler dataHandler { get; set; }
        public FormTileset tileset { get; private set; }
        private EditRange editRange { get; set; }

        public DataHandler.Map map { get; private set; }
        private Bitmap mapDisplay { get; set; } = null;
        private float zoomLevel { get; set; } = 1;
        private int mapNumber { get; set; }

        public FormMap(DataHandler dh, int mn)
        {
            dataHandler = dh;
            tileset = new FormTileset();
            mapNumber = mn;

            InitializeComponent();
        }

        private void FormMap_Load(object sender, EventArgs e)
        {
            
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
            //Text = $"({map.width} x {map.height})";
            map = e.map;
            saveAsToolStripMenuItem.Enabled = true;

            viewRangeEditor.Enabled = true;
            applyTSCToolStripMenuItem.Enabled = true;
            editRange = new EditRange(map.xOffset, map.yOffset, map.width - map.xRange, map.height - map.yRange);
            editRange.numericUpDown1.ValueChanged += DrawRange;
            editRange.numericUpDown2.ValueChanged += DrawRange;
            editRange.FormClosing += (_o, _e) => { _e.Cancel = true; viewRangeEditor.Checked = false; };
            viewRangeEditor.CheckedChanged += delegate { toggleForm(editRange, viewRangeEditor.Checked); };
        }

        private void DrawRange(object sender, EventArgs e)
        {
            map.xOffset = (int)editRange.numericUpDown1.Value;
            map.yOffset = (int)editRange.numericUpDown2.Value;
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
            pictureBox1.Image = new Bitmap(DrawRange(mapDisplay), (int)(mapDisplay.Width * zoomLevel), (int)(mapDisplay.Height * zoomLevel));
            pictureBox1.Refresh();
        }

        private void InitializeMap()
        {
            mapDisplay = new Bitmap(map.width * map.tileset.tileSize, map.height * map.tileset.tileSize);

            for (int i = 0; i < map.data.Count; i++)
            {
                for (int _i = 0; _i < map.data[i].Length; _i++)
                {
                    DrawTile(_i, i, mapDisplay);
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (mapDisplay == null && map != null)
            {
                InitializeMap();
                DrawMap();
                //pictureBox1.Refresh();
            }
        }        

        private void DrawTile(int tileXCoord, int tileYCoord, Bitmap outputMap)
        {
            if (mapDisplay == null)
            {
                InitializeMap();
            }
            else
            {
                using (Graphics g = Graphics.FromImage(mapDisplay))
                {
                    int tileXOffset = map.data[tileYCoord][tileXCoord] - (map.tileset.width * (map.data[tileYCoord][tileXCoord] / map.tileset.width));
                    int tileYOffset = (map.data[tileYCoord][tileXCoord] - tileXOffset) / (map.tileset.width);

                    Bitmap tileToDraw = map.tileset.image.Clone(
                        new Rectangle(
                            tileXOffset * map.tileset.tileSize,
                            tileYOffset * map.tileset.tileSize,
                            map.tileset.tileSize,
                            map.tileset.tileSize),
                        PixelFormat.Format4bppIndexed //TODO not sure if nessecary?
                        );

                    g.DrawImage(tileToDraw,
                            tileXCoord * map.tileset.tileSize,
                            tileYCoord * map.tileset.tileSize
                            );
                }

                //TODO figure out how to use this to save on memory...
                /*
                if (!e.Graphics.IsVisible(_i * 16, i * 16))
                    break;
                */

                /*
                int x = 0;

                int y = 0;

                int loopCount = 0;

                while(loopCount < dataHandler.loadedMaps[mapNumber].data[i][_i])
                {
                    if (x == (dataHandler.loadedMaps[mapNumber].tileset.Width / 16) - 1)
                    {
                        x = 0;
                        y++;
                    }
                    else
                        x++;

                    loopCount++;
                }
                */
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
                    
                    DrawTile(x, y, mapDisplay);
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
            if(at.ShowDialog() == DialogResult.OK && at.Tsc.Length >= 9) //9 is the length of an <SMP command, which is the smallest one recognised.
            {
                dataHandler.ApplyTSC(at.Tsc, mapNumber);
                InitializeMap();
                DrawMap();
                dataHandler.GenerateTSC();
            }
        }
    }
}
