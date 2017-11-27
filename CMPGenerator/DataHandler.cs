using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CMPGenerator
{
    public class DataHandler
    {
        /// <summary>
        /// PXM 0x10
        /// </summary>
        //public static readonly byte[] header = { 0x50, 0x58, 0x4D, 0x10 };

        public Map map1 { get; set; }
        public Map map2 { get; set; }

        public bool TSCType { get; set; } = true; //true = CMP, false = SMP
        public bool ignoreIdenticalTiles { get; set; } = true; //Ignore Identical Tiles
        public Dictionary<Tuple<int,int>, string> TSC { get; set; } = new Dictionary<Tuple<int,int>, string>();

        public class Map
        {
            public static readonly byte[] header = { 0x50, 0x58, 0x4D, 0x10 };

            public int xOffset { get; set; } = 0;
            public int yOffset { get; set; } = 0;
            public int xRange { get; set; }
            public int yRange { get; set; }

            public short width { get; set; }
            public short height { get; set; }

            public List<byte[]> data { get; set; } = new List<byte[]>();

            public Tileset tileset { get; set; }

            public Map(string mapPath, string tilesetPath, int tileSize = 16) : this(mapPath, new Bitmap(tilesetPath), tileSize) { }
            public Map(string mapPath, Bitmap tileset, int tileSize = 16)
            {
                using (BinaryReader br = new BinaryReader(new FileStream(mapPath, FileMode.Open, FileAccess.Read)))
                {
                    if (!br.ReadBytes(4).SequenceEqual(header))
                        return;

                    this.width = br.ReadInt16();
                    this.height = br.ReadInt16();
                    xRange = this.width;
                    yRange = this.height;
                    
                    //this.data = new List<byte[]>();
                    while (data.Count < height)
                    {
                        this.data.Add(br.ReadBytes(width));
                    }
                }
                this.tileset = new Tileset(tileset,tileSize);
            }
        }
        public class Tileset
        {
            public int height { get; set; }
            public int width { get; set; }

            public byte selectedTile { get; set; }
            public int tileSize { get; }

            public Bitmap image { get; set; }

            public Tileset(string image, int tileSize = 16) : this(new Bitmap(image), tileSize) { }
            public Tileset(Bitmap image, int tileSize = 16)
            {
                this.image = image;

                if (this.image.Height > 16 * tileSize)
                    throw new OverflowException($"The selected tileset is taller than the max accepted value of {this.image.Height > 16 * tileSize} pixels.");
                else
                    this.height = this.image.Height / tileSize;

                if (this.image.Width > 16 * tileSize)
                    throw new OverflowException($"The selected tileset is wider than the max accepted value of {this.image.Width > 16 * tileSize} pixels.");
                else
                    this.width = this.image.Width / tileSize;

                this.tileSize = tileSize;
            }
        }

        public class TSCUpdatedEventArgs : EventArgs
        {
            public string TSC { get; }

            public TSCUpdatedEventArgs(string TSC)
            {
                this.TSC = TSC;
            }
        }
        public event EventHandler<TSCUpdatedEventArgs> TSCUpdated = new EventHandler<TSCUpdatedEventArgs>((o, e) => { });
        public void UpdateTSC(object sender, FormMap.TileChangedEventArgs e)
        {
            //HACK might not work
            if((sender as FormMap).map == map2 || !TSCType )
            {
                UpdateTSC(e.x, e.y);
            }

            TSCUpdated(this, new TSCUpdatedEventArgs(GenerateTSCString()));
            //TSCUpdated(this, new TSCUpdatedEventArgs(string.Join(null, TSC.OrderBy(x => x.Key.Item1).OrderBy(y => y.Key.Item2).Select(v => v.Value))));
        }
        public void UpdateTSC(int tilex, int tiley)
        {
            byte oldTile = map1.data[tiley + map1.yOffset][tilex + map1.xOffset];
            byte newTile = map2.data[tiley + map2.yOffset][tilex + map2.xOffset];
            if (!ignoreIdenticalTiles || oldTile != newTile)
            {
                switch (TSCType)
                {
                    //<CMPxxxx:yyyy:tttt
                    case (true):
                        TSC[new Tuple<int, int>(tilex, tiley)] = $"<CMP{tilex.ToString("D4")}:{tiley.ToString("D4")}:{newTile.ToString("D4")}";
                        break;

                    //<SMPxxxx:yyyy
                    case (false):
                        TSC[new Tuple<int, int>(tilex, tiley)] = "";
                        for (byte i = oldTile; i != newTile; i--)
                        {
                            TSC[new Tuple<int, int>(tilex, tiley)] += $"<SMP{tilex.ToString("D4")}:{tiley.ToString("D4")}";
                        }
                        break;
                }
            }
        }

        public void GenerateTSC()
        {
            if (map1 != null && map2 != null)
            {
                TSC.Clear();
                for (int i = 0; i < map1.yRange; i++)
                {
                    for (int _i = 0; _i < map1.xRange; _i++)
                    {
                        UpdateTSC(_i, i);
                    }
                }
                TSCUpdated(this, new TSCUpdatedEventArgs(GenerateTSCString()));
            }
        }

        private string GenerateTSCString()
        {
            string formattedTSC = "";
            int[] rows = TSC.Keys.Select(x => x.Item2).Distinct().ToArray();
            for (int i = 0; i < rows.Length; i++)
            {
                formattedTSC += string.Join(null, TSC.Where(x => x.Key.Item2 == rows[i]).Select(v => v.Value));
                if (i + 1 != rows.Length)
                    formattedTSC += '\n';
            }
            return formattedTSC;
        }


        #region Opening stuff

        public class FileLoadedEventArgs : EventArgs
        {
            public Map map { get; }

            public FileLoadedEventArgs(Map map)
            {
                this.map = map;
            }
        }

        //HACK might want to think about merging these?
        public event EventHandler<FileLoadedEventArgs> Map1Loaded = new EventHandler<FileLoadedEventArgs>((o, e) => { });
        public event EventHandler<FileLoadedEventArgs> Map2Loaded = new EventHandler<FileLoadedEventArgs>((o, e) => { });
        public void Load(string mapPath, string tilesetPath, int mapNumber)
        {
            Map newMap = new Map(mapPath, tilesetPath);
            if (newMap == null)
                return;
            switch (mapNumber)
            {
                case (1):
                    map1 = newMap;
                    SetUpRanges();
                    Map1Loaded(this, new FileLoadedEventArgs(map1));
                    break;
                case (2):
                    map2 = newMap;
                    SetUpRanges();
                    Map2Loaded(this, new FileLoadedEventArgs(map2));
                    break;
            }

        }

        private void SetUpRanges()
        {
            if( map1 != null && map2 != null)
            {
                map1.xRange = map2.xRange = Math.Min(map1.width, map2.width);
                map1.yRange = map2.yRange = Math.Min(map1.height, map2.height);
                Map1Loaded(this, new FileLoadedEventArgs(map1)); //HACK
                Map2Loaded(this, new FileLoadedEventArgs(map2));
                GenerateTSC();
            }
        }


        #endregion

        public void SaveMap(int mapNumber, string saveLocation)
        {
            Map mapToSave;
            switch(mapNumber)
            {
                case (1):
                    mapToSave = map1;
                    break;
                case (2):
                    mapToSave = map2;
                    break;
                default:
                    return;
            }
            using (BinaryWriter bw = new BinaryWriter(new FileStream(saveLocation, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(Map.header);
                bw.Write(mapToSave.width);
                bw.Write(mapToSave.height);

                for (int i = 0; i < mapToSave.data.Count; i++)
                    for (int _i = 0; _i < mapToSave.data[i].Length; _i++)
                        bw.Write(mapToSave.data[i][_i]);
            }
        }

        public void ApplyTSC(string tsc, int mapNumber)
        {
            Map mapToEdit;
            switch(mapNumber)
            {
                case (1):
                    mapToEdit = map1;
                    break;
                case (2):
                    mapToEdit = map2;
                    break;
                default:
                    return;
            }

            Regex r = new Regex(@"<(?<name>.{3})(?<x>\d{4}).(?<y>\d{4})(?:.(?<tile>\d{4}))?");
            MatchCollection parsed = r.Matches(tsc);
            for(int i = 0; i < parsed.Count; i++)
            {
                int x = 0;
                int y = 0;
                int tile = 0;
                //If both int.TryParses, and the tile parsing (where applicable), succeseed, and the resulting x/y values are within the map's bounds...
                if (
                    (int.TryParse(parsed[i].Groups["x"].Value,out x) &&
                    int.TryParse(parsed[i].Groups["y"].Value, out y) &&
                    (parsed[i].Groups["tile"].Success) ? int.TryParse(parsed[i].Groups["tile"].Value,out tile) : true) ? x < mapToEdit.width && y < mapToEdit.height : false)
                {
                    switch (parsed[i].Groups["name"].Value)
                    {
                        case ("SMP"):
                            mapToEdit.data[y][x]--;
                            break;
                        case ("CMP"):
                            mapToEdit.data[y][x] = (byte)tile;
                            break;
                    }
                }
            }
        }
    }
}

