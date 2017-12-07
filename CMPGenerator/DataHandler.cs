using System;
using System.Collections;
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
        public Map map1 { get; private set; }
        public Map map2 { get; private set; }

        /// <summary>
        /// What mode to use for TSC generation. true = CMP, false = SMP
        /// </summary>
        public bool TSCType { get; set; } = true;
        public bool ignoreIdenticalTiles { get; set; } = true; //Ignore Identical Tiles
        public Dictionary<int, Dictionary<int, string>> TSC { get; private set; } = new Dictionary<int, Dictionary<int, string>>();

        public class Map
        {
            /// <summary>
            /// PXM 0x10
            /// </summary>
            public static readonly byte[] header = { 0x50, 0x58, 0x4D, 0x10 };

            public short xOffset { get; set; } = 0;
            public short yOffset { get; set; } = 0;

            public short xRange { get; set; }
            public short yRange { get; set; }

            public short width { get; private set; }
            public short height { get; private set; }

            public int? invalidX { get; private set; }
            public int? invalidY { get; private set; }

            //public List<byte[]> data { get; set; } = new List<byte[]>();
            public List<List<byte>> data { get; private set; } = new List<List<byte>>();
            //public List<byte> rawdata { get; private set; } = new List<byte>();

            public Tileset tileset { get; private set; }

            public enum ResizeMode
            {
                OutOfBounds,
                Default
            }
            public void Resize(string tsc)
            {
                Regex r = new Regex(@"<fl(?<mode>\+|-)(?<flag>.{4})", RegexOptions.IgnoreCase);
                Match parsed = r.Match(tsc);
                int value = 0;
                if ((parsed.Success) ? int.TryParse(string.Join(null, Encoding.ASCII.GetBytes(parsed.Groups["flag"].Value).Select(v => v -= 0x30)),out value) : false)
                {
                    if (parsed.Groups["mode"].Value != "+" && parsed.Groups["mode"].Value != "-")
                        return;

                    short x = this.width;
                    short y = this.height;

                    //TODO merge redundant code (make function?)
                    if (16176 <= value && value <= 16191) //Between @176 and @191 = width/x
                    {
                        BitArray b = new BitArray(BitConverter.GetBytes(x));
                        b[value - 16176] = parsed.Groups["mode"].Value == "+";
                        int[] s = new int[1];
                        b.CopyTo(s, 0);
                        x = (short)s[0];
                    }
                    else if (16192 <= value && value <= 16207) //Between @192 and @207 = height/y
                    {
                        BitArray b = new BitArray(BitConverter.GetBytes(y));
                        b[value - 16192] = parsed.Groups["mode"].Value == "+";
                        int[] s = new int[1];
                        b.CopyTo(s, 0);
                        y = (short)s[0];
                    }
                    else
                        return;

                    Resize(x, y, ResizeMode.OutOfBounds);
                }                
            }
            public void Resize(short newWidth, short newHeight, ResizeMode mode)
            {
                switch (mode)
                {
                    #region Out Of Bounds

                    case (ResizeMode.OutOfBounds):
                        if (height == newHeight && width == newWidth)
                            return;

                        List<byte> rawData = new List<byte>(width * height);
                        foreach (List<byte> item in data)
                            rawData.AddRange(item);

                        if (width != newWidth)
                        {
                            width = newWidth;
                            invalidX = null;
                            invalidY = null;
                            int loop = 0;
                            data = new List<List<byte>>(height);

                            for (int i = 0; i < height; i++)
                            {
                                data.Add(new List<byte>(width));
                                for (int _i = 0; _i < width; _i++)
                                {
                                    if (loop + i < rawData.Count)
                                    {
                                        data[i].Add(rawData[loop + _i]);
                                    }
                                    else
                                    {
                                        if (invalidX == null && invalidY == null)
                                        {
                                            invalidY = i;
                                            invalidX = _i;
                                        }
                                        data[i].Add(0x00);
                                    }
                                }
                                loop += width;
                            }
                        }
                        if (height != newHeight)
                        {
                            height = newHeight;

                            if (data.Count > height)
                                data.RemoveRange(height, data.Count - height);
                            else
                            {
                                //TODO refactor
                                List<byte> row = new List<byte>(width);
                                while (row.Count < width)
                                    row.Add(0x00);
                                while (data.Count < height)
                                    data.Add(row);
                            }
                        }
                        break;

                    #endregion

                    #region Default

                    case (ResizeMode.Default):
                        if (width != newWidth)
                        {
                            width = newWidth;

                            for (int i = 0; i < data.Count; i++)
                            {
                                if (data[i].Count > width)
                                    data[i].RemoveRange(width, data[i].Count - width);
                                else
                                    while (data[i].Count < width)
                                        data[i].Add(0x00);
                            }
                        }

                        if (height != newHeight)
                        {
                            height = newHeight;

                            if (data.Count > height)
                                data.RemoveRange(height, data.Count - height);
                            else
                            {
                                //TODO refactor
                                List<byte> row = new List<byte>(width);
                                while (row.Count < width)
                                    row.Add(0x00);
                                while (data.Count < height)
                                    data.Add(row);
                            }
                        }
                        break;

                        #endregion
                }
            }

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
                    invalidX = this.width;
                    invalidY = this.height;

                    //this.data = new List<byte[]>();
                    while (data.Count < height)
                    {
                        this.data.Add(br.ReadBytes(width).ToList());
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
            byte? oldTile = null;
            if (tiley + map1.yOffset < map1.height && tilex + map1.xOffset < map1.width)
                oldTile = map1.data[tiley + map1.yOffset][tilex + map1.xOffset];

            byte newTile = map2.data[tiley + map2.yOffset][tilex + map2.xOffset];

            if (!ignoreIdenticalTiles || oldTile != newTile)
            {
                if (!TSC.ContainsKey(tiley))
                    TSC.Add(tiley, new Dictionary<int, string>());

                switch (TSCType)
                {
                    //<CMPxxxx:yyyy:tttt
                    case (true):
                        TSC[tiley][tilex] = $"<CMP{tilex.ToString("D4")}:{tiley.ToString("D4")}:{newTile.ToString("D4")}";
                        break;

                    //<SMPxxxx:yyyy
                    case (false):
                        TSC[tiley][tilex] = "";
                        for (byte i = (byte)oldTile; i != newTile; i--)
                        {
                            TSC[tiley][tilex] += $"<SMP{tilex.ToString("D4")}:{tiley.ToString("D4")}";
                        }
                        break;
                }
            }
            else if((TSC.ContainsKey(tiley)) ? TSC[tiley].ContainsKey(tilex) : false)
            {
                TSC[tiley].Remove(tilex);
                if (TSC[tiley].Count == 0)
                    TSC.Remove(tiley);
            }
        }

        private string ShortToOOBTSC(short n1, short n2, bool width)
        {
            string output = "";
            BitArray b1 = new BitArray(BitConverter.GetBytes(n1));
            BitArray b2 = new BitArray(BitConverter.GetBytes(n2));

            //width = @176
            //height = @192
            int place = (width) ? 176 : 192;

            for (int i = 0; i < 16; i++)
                if(b1[i] != b2[i])
                    output += $"<FL{(b2[i] ? "+" : "-")}@{place + i}";

            return output;
        }

        public void GenerateTSC()
        {
            if (map1 != null && map2 != null)
            {
                if(map1.xOffset + map1.xRange > map1.invalidX ||
                    map1.yOffset + map1.yRange > map1.invalidY ||
                    map2.xOffset + map2.xRange > map2.invalidX ||
                    map2.yOffset + map2.yRange > map2.invalidY)
                {
                    TSCType = true; //TODO need to update UI
                }

                string OOBFlags = "";
                if(map1.yRange != map2.yRange || map1.xRange != map2.xRange)
                {
                    OOBFlags += ShortToOOBTSC(map1.xRange, map2.xRange, true) + ShortToOOBTSC(map1.yRange, map2.yRange, false);
                }

                TSC.Clear();
                for (int i = 0; i < map2.yRange; i++)
                {
                    for (int _i = 0; _i < map2.xRange; _i++)
                    {
                        UpdateTSC(_i, i);
                    }
                }
                TSCUpdated(this, new TSCUpdatedEventArgs(OOBFlags + ((OOBFlags.Length > 0) ? "\n" : "") + GenerateTSCString()));
            }
        }

        private string GenerateTSCString()
        {
            string formattedTSC = "";
            int[] rows = TSC.Keys.Distinct().ToArray();
            for (int i = 0; i < rows.Length; i++)
            {
                formattedTSC += string.Join(null, TSC[rows[i]].Values);
                if (i + 1 != rows.Length)
                    formattedTSC += '\n';
            }
            return formattedTSC;
        }


        #region Opening stuff

        public class FileLoadedEventArgs : EventArgs
        {
            public Map map { get; }
            public string path { get; }
            public int number { get; }

            public FileLoadedEventArgs(Map map, string path, int number)
            {
                this.map = map;
                this.path = path;
                this.number = number;
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

            //TODO make not a horrible mess of if/elses
            if (mapNumber == 1)
                map1 = newMap;
            else if (mapNumber == 2)
                map2 = newMap;
            else
                return;

            bool rangesSet = SetUpRanges();

            if (rangesSet || mapNumber == 1)
                Map1Loaded(this, new FileLoadedEventArgs(map1, mapPath, 1));
            else
                return;

            if (rangesSet || mapNumber == 2)
                Map2Loaded(this, new FileLoadedEventArgs(map2, mapPath, 2));
            else
                return;
        }

        private bool SetUpRanges()
        {
            if (map1 != null && map2 != null)
            {
                map1.xRange = map2.xRange = Math.Min(map1.width, map2.width);
                map1.yRange = map2.yRange = Math.Min(map1.height, map2.height);
                GenerateTSC();
                return true;
            }
            else
                return false;
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
                    for (int _i = 0; _i < mapToSave.data[i].Count; _i++)
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

            //Regex r = new Regex(@"<(?<name>.{3})(?<x>\d{4}).(?<y>\d{4})(?:.(?<tile>\d{4}))?");
            Regex r = new Regex(@"<(?<name>.{3})(?<x>.{4})(?:.(?<y>.{4})(?:.(?<tile>.{4}))?)?");
            MatchCollection parsed = r.Matches(tsc);
            for(int i = 0; i < parsed.Count; i++)
            {
                if (parsed[i].Groups["name"].Value.StartsWith("fl", true, null))
                {
                    mapToEdit.Resize(parsed[i].Value);
                }
                else
                {
                    int x = 0;
                    int y = 0;
                    int tile = 0;
                    //If both int.TryParses, and the tile parsing (where applicable), succeseed, and the resulting x/y values are within the map's bounds...
                    if (
                        (int.TryParse(string.Join(null, Encoding.ASCII.GetBytes(parsed[i].Groups["x"].Value).Select(v => v -= 0x30)), out x)
                        && int.TryParse(string.Join(null, Encoding.ASCII.GetBytes(parsed[i].Groups["y"].Value).Select(v => v -= 0x30)), out y) 
                        && (parsed[i].Groups["tile"].Success) ?
                            int.TryParse(string.Join(null, Encoding.ASCII.GetBytes(parsed[i].Groups["tile"].Value).Select(v => v -= 0x30)), out tile) :
                            true) ?
                            x < mapToEdit.width && y < mapToEdit.height :
                            false
                       )
                    {
                        switch (parsed[i].Groups["name"].Value.ToLower())
                        {
                            case ("smp"):
                                mapToEdit.data[y][x]--;
                                break;
                            case ("cmp"):
                                mapToEdit.data[y][x] = (byte)tile;
                                break;
                        }
                    }
                }
            }
        }
    }
}

