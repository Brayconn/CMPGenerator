﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CMPGenerator
{
    public class MapComparer
    {
        private Map map1 { get; set; }
        private Map map2 { get; set; }

        public MapComparer(Map masterMap, Map slaveMap)
        {
            map1 = masterMap;
            map2 = slaveMap;

            (map1.data as INotifyCollectionChanged).CollectionChanged += UpdateTSC;
            (map2.data as INotifyCollectionChanged).CollectionChanged += UpdateTSC;
            map1.MapResized += delegate { UpdateSMPSaftey(); GenerateTSC(); };
            map2.MapResized += delegate { UpdateSMPSaftey(); GenerateTSC(); };

            //HACK move these from delegates
            map1.RangeUpdated += delegate
            {
                if (map1.Loaded && map2.Loaded)
                {
                    map2.xRange = map1.xRange;
                    map2.yRange = map1.yRange;
                    if (map2.xRange != map1.xRange || map2.yRange != map1.yRange)
                    {
                        map1.xRange = map2.xRange;
                        map1.yRange = map2.yRange;
                    }
                    UpdateSMPSaftey();
                    GenerateTSC();
                }
            };
            map2.RangeUpdated += delegate
            {
                if (map1.Loaded && map2.Loaded)
                {
                    if (map1.xRange != map1.width && map1.yRange != map1.height)
                    {
                        map1.xRange = map2.xRange;
                        map1.yRange = map2.yRange;
                        if (map2.xRange != map1.xRange || map2.yRange != map1.yRange)
                        {
                            map1.xRange = map2.xRange;
                            map1.yRange = map2.yRange;
                        }
                    }
                    UpdateSMPSaftey();
                    GenerateTSC();
                }
            };
        }
                
        public enum TSCType
        {
            CMP,
            SMP
        }
        public event Action TSCModeLockChanged = new Action(delegate { }); //tfw u run out of names D;
        private bool tscmodelocked = false;
        public bool TSCModeLocked
        {
            get
            {
                return tscmodelocked;
            }
            private set
            {
                if(tscmodelocked != value)
                {
                    tscmodelocked = value;
                    TSCModeLockChanged();
                }
            }
        }
        public event Action TSCModeChanged = new Action(delegate { });
        private TSCType tscmode = TSCType.CMP;
        /// <summary>
        /// What command to use when generating TSC.
        /// </summary>
        public TSCType SelectedTSCType
        {
            get
            {
                return tscmode;
            }
            set
            {
                if (!TSCModeLocked && tscmode != value)
                {
                    tscmode = value;
                    TSCModeChanged();
                    if (map1.Loaded && map2.Loaded)
                        GenerateTSC();
                }
            }
        }

        private bool _ignoreIdenticalTiles = true;
        public bool ignoreIdenticalTiles
        {
            get
            {
                return _ignoreIdenticalTiles;
            }
            set
            {
                if (_ignoreIdenticalTiles != value)
                {
                    _ignoreIdenticalTiles = value;
                    if (map1.Loaded && map2.Loaded)
                        GenerateTSC();
                }
            }
        }

        //TODO change to a 1d dictionary?
        public Dictionary<int, Dictionary<int, string>> TSC { get; private set; } = new Dictionary<int, Dictionary<int, string>>();

        public class Map
        {
            //The PXM buffer is 0x4B000 bytes, apparently.
            //According to the function at 0x413750
            
            /// <summary>
            /// PXM 0x10
            /// </summary>
            public static readonly byte[] header = { 0x50, 0x58, 0x4D, 0x10 };

            public bool Loaded { get; private set; }

            public Tileset tileset { get; private set; }

            #region Range stuff

            public event Action RangeUpdated = new Action(delegate { });
            
            //TODO review these setting equations. they work, but I think they're being a bit janky at times...
            private short _xOffset = 0;
            public short xOffset
            {
                get
                {
                    return _xOffset;
                }
                set
                {
                    if(xOffset != value && value < width)
                    {
                        _xRange = ((_xOffset = value) + xRange > width) ? (short)(width - xOffset) : xRange;
                        RangeUpdated();
                    }
                }
            }
            private short _yOffset = 0;
            public short yOffset
            {
                get
                {
                    return _yOffset;
                }
                set
                {
                    if (yOffset != value && value < height)
                    {
                        _yRange = ((_yOffset = value) + yRange > height) ? (short)(height - yOffset) : yRange;
                        RangeUpdated();
                    }
                }
            }

            private short _xRange;
            public short xRange
            {
                get
                {
                    return _xRange;
                }
                set
                {
                    if (xRange != value && 0 <= value && value <= width)
                    {
                        _xOffset = ((_xRange = value) + xOffset > width) ? (short)(width - xRange) : xOffset;
                        RangeUpdated();
                    }
                }
            }
            private short _yRange;
            public short yRange
            {
                get
                {
                    return _yRange;
                }
                set
                {
                    if (yRange != value && 0 <= value && value <= height)
                    {
                        _yOffset = ((_yRange = value) + yOffset > height) ? (short)(height - yRange) : yOffset;
                        RangeUpdated();
                    }
                }
            }

            #endregion

            #region Size stuff

            public short width { get; private set; }
            public short height { get; private set; }

            /// <summary>
            /// Contains valid methods to resize a map
            /// </summary>
            public enum ResizeMode
            {
                OutOfBounds,
                NullInsert
            }
            
            /// <summary>
            /// Resizes the map using the given method
            /// </summary>
            /// <param name="newWidth"></param>
            /// <param name="newHeight"></param>
            /// <param name="mode">Resize method to use</param>
            public void Resize(short newWidth, short newHeight, ResizeMode mode)
            {
                if (newWidth == width && newHeight == height)
                    return;

                switch (mode)
                {
                    case (ResizeMode.NullInsert):
                        NullInsertResize(newWidth, newHeight);
                        break;
                    case (ResizeMode.OutOfBounds):
                        OOBResize(newWidth, newHeight);
                        break;
                }

                FinalizeMapResize();
            }

            /// <summary>
            /// Resizes the map using the OOB flag method
            /// </summary>
            /// <param name="newWidth"></param>
            /// <param name="newHeight"></param>
            private void OOBResize(short newWidth, short newHeight)
            {
                using (ObservableCollectionEx<byte?> safeMapData = this.data.DisableNotifications())
                    while (safeMapData.Count < newWidth * newHeight)
                        safeMapData.Add(null);
                this.width = newWidth;
                this.height = newHeight;
            }

            /// <summary>
            /// Resizes the map using the Null Insert method
            /// </summary>
            /// <param name="newWidth"></param>
            /// <param name="newHeight"></param>
            private void NullInsertResize(short newWidth, short newHeight)
            {
                using (ObservableCollectionEx<byte?> safeMapData = this.data.DisableNotifications())
                {
                    //TODO merge a bunch of stuff in here
                    if (newWidth > width)
                    {
                        for (int i = height - 1; i >= 0; i--)
                            for (int _i = width; _i < newWidth; _i++)
                                safeMapData.Insert(width + (i * width), null);
                    }
                    else if (newWidth < width)
                    {
                        for (int i = height - 1; i >= 0; i--)
                            for (int _i = width; _i > newWidth; _i--)
                                safeMapData.RemoveAt((i * width) + newWidth);
                    }
                    this.width = newWidth;

                    if (newHeight > height)
                    {
                        for (int i = 0; i < width * (newHeight - height); i++)
                            if ((height * width) + i < safeMapData.Count)
                                safeMapData[(height * width) + i] = null;
                            else
                                safeMapData.Insert((height * width) + i, null);
                    }
                    else if (newHeight < height)
                    {
                        while (safeMapData.Count > newHeight * newWidth)
                            safeMapData.RemoveAt(safeMapData.Count - 1);
                    }
                    this.height = newHeight;
                }
            }
            
            /// <summary>
            /// Updates the ranges to be within the map's bounderies and triggers the "MapResized" event
            /// </summary>
            private void FinalizeMapResize()
            {
                //This bypasses the triggering of the "RangeUpdated" event, since that could mess things up if triggered before the map is done being resized (see ApplyTSC())
                _xRange = (xOffset + xRange > width) ? (short)(width - xOffset) : xRange;
                _yRange = (yOffset + yRange > height) ? (short)(height - yOffset) : yRange;
                MapResized();
            }

            public event Action MapResized = new Action(delegate { });

            #endregion

            public ObservableCollectionEx<byte?> data { get; private set; } = new ObservableCollectionEx<byte?>();

            /// <summary>
            /// Parses a string to get an int value the same way Cave Story does it.
            /// </summary>
            /// <param name="input">String to parse</param>
            /// <returns>Parsed value, Null if an error occurs</returns>
            private static int? GetTSCValue(string input)
            {
                int output;
                return (int.TryParse(string.Join(null, Encoding.ASCII.GetBytes(input).Select(v => v -= 0x30)), out output)) ? (int?)output : null;
            }

            /// <summary>
            /// Applies the given TSC to the map. Supports FL+/- (where it would change the map size), and C/SMP.
            /// </summary>
            /// <param name="tsc">TSC to parse.</param>
            public void ApplyTSC(string tsc)
            {
                if (string.IsNullOrWhiteSpace(tsc))
                    return;

                /*
                 * this function supports four tsc commands. here's a rundown of all the regex groups they create:
                 * <FL+/<FL-:
                 * 0 = name (FL)
                 * 1 = mode (+ or -)
                 * 2 = flag (4 characters)
                 * 
                 * <SMP:
                 * 0 = name (SMP)
                 * 1 = x (4 characters)
                 * 2 = y (4 characters)
                 * 
                 * <CMP:
                 * 0 = name (CMP)
                 * 1 = x (4 characters)
                 * 2 = y (4 characters)
                 * 3 = tile (4 characters) 
                 */
                //rip branch reset, would've been much cleaner: @"<(?|(?:(FL)(\+|-)(.{4}))|(?:(SMP)(.{4}).(.{4}))|(?:(CMP)(.{4}).(.{4}).(.{4})))"
                Regex r = new Regex(@"<(?:(?:(?<name>FL)(?<mode>\+|-)(?<flag>.{4}))|(?:(?<name>SMP)(?<x>.{4}).(?<y>.{4}))|(?:(?<name>CMP)(?<x>.{4}).(?<y>.{4}).(?<tile>.{4})))");
                MatchCollection parsed = r.Matches(tsc);
                for (int i = 0; i < parsed.Count; i++)
                {
                    if (parsed[i].Groups["name"].Value.StartsWith("FL", true, null))
                    {
                        int? flag = GetTSCValue(parsed[i].Groups["flag"].Value);
                        if (flag == null || (parsed[i].Groups["mode"].Value != "+" && parsed[i].Groups["mode"].Value != "-"))
                            continue;

                        short x = this.width;
                        short y = this.height;

                        //TODO merge redundant code (make function?)
                        if (16176 <= flag && flag <= 16191) //Between @176 and @191 (inclusive) = width/x
                        {
                            BitArray b = new BitArray(BitConverter.GetBytes(x));
                            b[(int)flag - 16176] = parsed[i].Groups["mode"].Value == "+";
                            int[] s = new int[1];
                            b.CopyTo(s, 0);
                            x = (short)s[0];
                        }
                        else if (16192 <= flag && flag <= 16207) //Between @192 and @207 (inclusive) = height/y
                        {
                            BitArray b = new BitArray(BitConverter.GetBytes(y));
                            b[(int)flag - 16192] = parsed[i].Groups["mode"].Value == "+";
                            int[] s = new int[1];
                            b.CopyTo(s, 0);
                            y = (short)s[0];
                        }
                        else
                            continue;
                        
                        OOBResize(x, y);
                    }
                    else //if (parsed[i].Groups["name"].Value.StartsWith("CMP", true, null) || parsed[i].Groups[0].Value.StartsWith("SMP", true, null))
                    {
                        int? x = GetTSCValue(parsed[i].Groups["x"].Value);
                        int? y = GetTSCValue(parsed[i].Groups["y"].Value);
                        if (x == null || y == null)
                            continue;

                        using (ObservableCollectionEx<byte?> safeMapData = this.data.DisableNotifications())
                        {
                            switch (parsed[i].Groups["name"].Value.ToLower())
                            {
                                case ("smp"):
                                    safeMapData[((int)y * width) + (int)x]--;
                                    break;
                                case ("cmp"):
                                    int? tile = GetTSCValue(parsed[i].Groups["tile"].Value);
                                    if (tile == null)
                                        continue;

                                    safeMapData[((int)y * width) + (int)x] = (byte)tile;
                                    break;
                            }
                        }
                    }
                }
                FinalizeMapResize();
            }

            /// <summary>
            /// Saves the map as a pxm to the given path. Any null tiles will be ignored.
            /// </summary>
            /// <param name="path">Path to save map to.</param>
            public void Save(string path)
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
                {
                    bw.Write(Map.header);
                    bw.Write(width);
                    bw.Write(height);
                    bw.Write(data.Where(x => x.HasValue).Select(x => x.Value).ToArray());

                    /*
                    for (int i = 0; i < mapToSave.data.Count; i++)
                        for (int _i = 0; _i < mapToSave.data[i].Count; _i++)
                            bw.Write(mapToSave.data[i][_i]);
                    */
                }
            }

            #region Map Loading

            public enum OverwriteMode
            {
                All,
                NullOnly,
                NullAndAppend,
                AppendOnly
            }

            public class MapLoadedEventArgs : EventArgs
            {
                public string path { get; }
                
                public MapLoadedEventArgs(string path)
                {
                    this.path = path;
                }
            }
            public event EventHandler<MapLoadedEventArgs> MapLoaded = new EventHandler<MapLoadedEventArgs>((o,e) => { });
            public bool Load(string mapPath, string tilesetPath)
            {
                return tileset.Load(tilesetPath) && this.Load(mapPath, true, true, OverwriteMode.All);
            }
            public bool Load(string mapPath, bool resize, bool preserveCoords, OverwriteMode overwriteMode)
            {
                using (BinaryReader br = new BinaryReader(new FileStream(mapPath, FileMode.Open, FileAccess.Read)))
                {
                    if (!br.ReadBytes(4).SequenceEqual(header))
                        return false;
                    short newWidth = br.ReadInt16();
                    short newHeight = br.ReadInt16();
                    if (newWidth == 0 || newHeight == 0)
                        return false;

                    using (ObservableCollectionEx<byte?> iDisabled = data.DisableNotifications())
                    {
                        //Set the map size only if we haven't set it yet, or if the user said they want to
                        if (resize || (width == default(short) && height == default(short)))
                        {
                            this.width = newWidth;
                            this.height = newHeight;

                            if (xRange == default(short) && yRange == default(short))
                            {
                                xRange = this.width;
                                yRange = this.height;
                            }

                            //UNSURE never got a chance to actually test this with the List<byte?> method... not sure if applicable anymore
                            //iDisabled.Capacity = this.width * this.height;
                        }

                        for (int i = (overwriteMode == OverwriteMode.AppendOnly) ? iDisabled.Count : 0;
                            i < ((preserveCoords) ? Math.Max(br.BaseStream.Length - 8, data.Count) : br.BaseStream.Length - 8);
                            i++)
                        {
                            if (preserveCoords)
                            {
                                int y = i / width;
                                int x = i - (width * y);
                                int newPos = ((y * newWidth) + x) + 8;

                                if (x < newWidth && y < newHeight && newPos < br.BaseStream.Length)
                                    br.BaseStream.Seek(newPos, SeekOrigin.Begin);
                                else
                                    continue;
                            }
                            else
                            {
                                //UNSURE need to test if nessicary...
                                br.BaseStream.Seek(8 + i, SeekOrigin.Begin);
                            }

                            //If i is on the map, then we're replacing, which means that NullOnly and NullAndAppend have an extra check to do
                            if (i < iDisabled.Count && ((overwriteMode == OverwriteMode.NullOnly || overwriteMode == OverwriteMode.NullAndAppend) ? iDisabled[i] == null : true))
                                iDisabled[i] = br.ReadByte();
                            //If not, we're appending, which means we can't be in NullOnly mode
                            else if (i >= iDisabled.Count && overwriteMode != OverwriteMode.NullOnly)
                                iDisabled.Add(br.ReadByte()); //UNSURE might want to use insert?
                        }

                        Loaded = true;
                        MapLoaded(this, new MapLoadedEventArgs(mapPath));
                        return true;
                    }
                }
            }

            public Map()
            {
                tileset = new Tileset();
            }

            #endregion
        }
        public class Tileset
        {
            public event Action SelectedTileChanged = new Action(delegate { });
            private byte? _selectedTile = 0;
            public byte? selectedTile
            {
                get
                {
                    return _selectedTile;
                }
                set
                {
                    if(_selectedTile != value)
                    {
                        _selectedTile = value;
                        SelectedTileChanged();
                    }
                }
            }

            public int tileSize { get; private set; } = 16;

            public Bitmap image { get; private set; } = new Bitmap(256,256);

            public Tileset()
            {
                using (Graphics g = Graphics.FromImage(image))
                    g.FillRectangle(Brushes.Black, 0, 0, 256, 256);
            }
            
            public event Action TilesetLoaded = new Action(delegate { });
            public bool Load(string imagePath, int newTileSize = 0)
            {
                return Load(new Bitmap(imagePath), newTileSize);
            }
            public bool Load(Bitmap newImage, int newTileSize = 0)
            {
                int newMaxSize;
                //Use passed tile size
                if(newTileSize != 0)
                {
                    newMaxSize = newTileSize * 16;

                    //HACK can probably still be refactored...
                    string error = "";
                    if (newImage.Height > newMaxSize && newImage.Width > newMaxSize)
                        error = $"big! {newImage.Width} > {newMaxSize}, and {newImage.Height} > {newMaxSize}.";
                    else if (newImage.Width > newMaxSize)
                        error = $"wide! {newImage.Width} > {newMaxSize}.";
                    else if (newImage.Height > newMaxSize)
                        error = $"tall! {newImage.Height} > {newMaxSize}.";

                    //HACK don't use windows.forms
                    if (error.Length > 0 && MessageBox.Show($"The provided bitmap is too {error}\nWould you like to load the top left {newMaxSize}x{newMaxSize} pixels anyways?", "Load Tileset", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return false;
                }
                //auto detect (silently)
                else
                {
                    //Get biggest tiles
                    newTileSize = newImage.Width / 16;
                    //16 tiles in a row
                    newMaxSize = newTileSize * 16;
                }

                if (newTileSize != this.tileSize)
                {
                    this.image = new Bitmap(newMaxSize, newMaxSize);
                    using (Graphics g = Graphics.FromImage(this.image))
                        g.FillRectangle(Brushes.Black, new Rectangle(0, 0, newMaxSize, newMaxSize));
                }
                this.tileSize = newTileSize;

                using (Graphics g = Graphics.FromImage(this.image))
                {
                    g.DrawImage(newImage, 0, 0, newImage.Width, newImage.Height);
                }

                TilesetLoaded();
                return true;
            }
        }

        private static string ShortToOOBTSC(short initialValue, short finalValue, bool width)
        {
            string output = "";
            BitArray b1 = new BitArray(BitConverter.GetBytes(initialValue));
            BitArray b2 = new BitArray(BitConverter.GetBytes(finalValue));

            //width = @176
            //height = @192
            int place = (width) ? 176 : 192; //HACK actual code is clean, but having to provide a bool in the first place is messy

            for (int i = 0; i < 16; i++)
                if (b1[i] != b2[i])
                    output += $"<FL{(b2[i] ? "+" : "-")}@{place + i}";

            return output;
        }

        private static bool IsMapSMPSafe(Map map)
        {
            //TODO test out this idea:
            //cast every null item into a new array/dictionary, then check if any of them is in the right range
            for (int i = map.yOffset; i < map.yRange; i++)
                for (int _i = map.xOffset; _i < map.xRange; _i++)
                    if (map.data[(i * map.width) + _i] == null)
                        return false;

            return true;
        }

        //TODO maybe remove this, and make it a property of maps
        private void UpdateSMPSaftey()
        {
            //TODO would really like to replace this with a conditional operator, but I don't think it's actually possible, since TSCMode HAS to be set first, otherwise things break.
            if (!IsMapSMPSafe(map1) || !IsMapSMPSafe(map2) || map1.height < map2.height || map1.width < map2.width)
            {
                SelectedTSCType = TSCType.CMP;
                TSCModeLocked = true;
            }
            else
            {
                TSCModeLocked = false;
            }
        }



        /// <summary>
        /// Generates (from scratch) the TSC that turns map1 into map2
        /// </summary>
        public void GenerateTSC()
        {
            if (!map1.Loaded || !map2.Loaded)
                return;

            string OOBFlags = "";
            if (map1.yRange != map2.yRange || map1.xRange != map2.xRange)
                OOBFlags += ShortToOOBTSC(map1.xRange, map2.xRange, true) + ShortToOOBTSC(map1.yRange, map2.yRange, false);

            TSC.Clear();
            for (int i = 0; i < map2.yRange; i++)
                for (int _i = 0; _i < map2.xRange; _i++)
                    UpdateTSC(_i, i);

            TSCUpdated(this, new TSCUpdatedEventArgs(((OOBFlags.Length > 0) ? OOBFlags + "\n" : "") + GenerateTSCString()));
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
        public void UpdateTSC(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSMPSaftey();

            //TODO maybe move this down

            short widthToUse;
            if (sender as ObservableCollectionEx<byte?> == map2.data)
                widthToUse = map2.width;
            else if (sender as ObservableCollectionEx<byte?> == map1.data)
                widthToUse = map1.width;
            else
                return;
                        
            if (e.Action == NotifyCollectionChangedAction.Replace && ((sender as ObservableCollectionEx<byte?>) == map2.data || SelectedTSCType == TSCType.SMP))
            {
                int y = e.OldStartingIndex / widthToUse;
                UpdateTSC(e.OldStartingIndex - (y*widthToUse), y);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                GenerateTSC();
            }

            TSCUpdated(this, new TSCUpdatedEventArgs(GenerateTSCString()));
        }
        public void UpdateTSC(int tilex, int tiley)
        {
            //TODO prevent the disconnect
            //Maybe provide a new width? :eyes:

            byte? oldTile = (((tiley + map1.yOffset) * map2.width) + (tilex + map1.xOffset) < map1.data.Count)
                ? map1.data[((tiley + map1.yOffset) * map2.width) + (tilex + map1.xOffset)]
                : null;

            byte? newTile = map2.data[((tiley + map2.yOffset)*map2.width) + (tilex + map2.xOffset)];

            if (!ignoreIdenticalTiles || (oldTile != newTile && newTile != null))
            {
                if (!TSC.ContainsKey(tiley))
                    TSC.Add(tiley, new Dictionary<int, string>());

                switch (SelectedTSCType)
                {
                    //<CMPxxxx:yyyy:tttt
                    case (TSCType.CMP):
                        TSC[tiley][tilex] = $"<CMP{(tilex + map1.xOffset).ToString("D4")}:{(tiley + map1.yOffset).ToString("D4")}:{newTile.Value.ToString("D4")}";
                        break;

                    //<SMPxxxx:yyyy
                    case (TSCType.SMP):
                        TSC[tiley][tilex] = "";
                        for (byte i = (byte)oldTile; i != newTile; i--)
                        {
                            TSC[tiley][tilex] += $"<SMP{(tilex + map1.xOffset).ToString("D4")}:{(tiley + map1.yOffset).ToString("D4")}";
                        }
                        break;
                }
            }
            else if(TSC.ContainsKey(tiley) && TSC[tiley].ContainsKey(tilex))
            {
                TSC[tiley].Remove(tilex);
                if (TSC[tiley].Count == 0)
                    TSC.Remove(tiley);
            }
        }

        public enum GenerationDirection
        {
            Rows,
            Columns
        }
        public GenerationDirection SelectedTSCGenerationMode { get; set; } = GenerationDirection.Rows;

        private string GenerateTSCString()
        {
            string formattedTSC = "";
            int[] rows = TSC.Keys.ToArray();
            Array.Sort(rows);
            switch (SelectedTSCGenerationMode)
            {
                case GenerationDirection.Rows:                    
                    for (int i = 0; i < rows.Length; i++)
                    {
                        formattedTSC += string.Join(null, TSC[rows[i]].Values.OrderBy(x => x));
                        if (i + 1 != rows.Length)
                            formattedTSC += '\n';
                    }
                    break;
                case GenerationDirection.Columns:
                    int[] columns = TSC.Values.SelectMany(x => x.Keys).Distinct().ToArray();
                    Array.Sort(columns);
                    for (int i = 0; i < columns.Length; i++)
                    {
                        for (int j = 0; j < rows.Length; j++)
                        {
                            if (TSC[rows[j]].TryGetValue(columns[i], out string value))
                                formattedTSC += value;
                        }
                        formattedTSC += '\n';
                    }
                    break;
            }

            return formattedTSC;
        }
    }
}

