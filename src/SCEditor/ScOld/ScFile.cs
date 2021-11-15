using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Windows.Forms;
using SCEditor.Compression;
using System.Linq;

namespace SCEditor.ScOld
{
    public class ScFile
    {

        #region Constructors

        public ScFile(string infoFile, string textureFile)
        {
            _textures = new List<ScObject>();
            _shapes = new List<ScObject>();
            _exports = new List<ScObject>();
            _movieClips = new List<ScObject>();
            _movieClipsModifier = new List<ScObject>();
            _pendingChanges = new List<ScObject>();
            _pendingMatrixs = new List<Matrix>();
            _matrixs = new List<Matrix>();
            _colors = new List<Tuple<Color, byte, Color>>();
            _infoFile = infoFile;
            _textureFile = textureFile;
            _pendingColors = new List<Tuple<Color, byte, Color>>();
            _fontNames = new List<string>();
            _textFields = new List<ScObject>();
        }

        #endregion

        #region Fields & Properties

        private ushort _exportCount;
        private ushort _shapeCount;
        private ushort _movieClipCount;
        private ushort _textureCount;
        private int _textFieldCount; 
        private int _matrixCount;
        private int _colorsCount;
        private List<ScObject> _textures;
        private List<ScObject> _shapes;
        private List<ScObject> _exports;
        private List<Matrix> _matrixs;
        private List<Tuple<Color, byte, Color>> _colors;
        private List<ScObject> _movieClips;
        private List<ScObject> _pendingChanges;
        private List<ScObject> _movieClipsModifier;
        private List<Matrix> _pendingMatrixs;
        private List<Tuple<Color, byte, Color>> _pendingColors;
        private List<string> _fontNames;
        private List<ScObject> _textFields;

        private readonly string _infoFile;
        private readonly string _textureFile;
        private long _eofOffset;
        private long _eofMatrixOffset;
        private long _eofTextFieldOffset;
        private long _eofColorsOffset;
        private long _eofMovieClipOffset;
        private long _eofShapeOffset;
        private long _eofTexOffset;
        private long _exportStartOffset;
        private long _sofTagsOffset;

        #endregion

        public void AddChange(ScObject change)
        {
            if (_pendingChanges.IndexOf(change) == -1)
                _pendingChanges.Add(change);
        }

        public void AddExport(Export export)
        {
            _exports.Add(export);
        }

        public void AddShape(Shape shape)
        {
            _shapes.Add(shape);
        }

        public void AddTexture(Texture texture)
        {
            _textures.Add(texture);
        }

        public void AddMovieClip(MovieClip movieClip)
        {
            _movieClips.Add(movieClip);
        }

        public long GetEofOffset()
        {
            return _eofOffset;
        }

        public long GetEofTexOffset()
        {
            return _eofTexOffset;
        }
        public long GetSofTagsOffset()
        {
            return _sofTagsOffset;
        }

        public List<ScObject> GetExports()
        {
            return _exports;
        }

        public string GetInfoFileName()
        {
            return _infoFile;
        }

        public List<Tuple<Color, byte, Color>> getColors()
        {
            return _colors;
        }

        public string GetTextureFileName()
        {
            return _textureFile;
        }

        public List<ScObject> GetMovieClips()
        {
            return _movieClips;
        }

        public List<Matrix> GetMatrixs()
        {
            return _matrixs;
        }

        public void addMatrix(Matrix matrix)
        {
            _matrixs.Add(matrix);
        }
        public List<ScObject> GetShapes()
        {
            return _shapes;
        }
        public List<ScObject> GetPendingChanges()
        {
            return _pendingChanges;
        }

        public List<Matrix> GetPendingMatrixChanges()
        {
            return _pendingMatrixs;
        }
        public ushort getShapesChunksCount()
        {
            ushort total = 0;
            foreach (ScObject sco in _shapes)
            {
                total = (ushort) (total + sco.Children.Count);
            }

            return total;
        }

        public long GetStartExportsOffset()
        {
            return _exportStartOffset;
        }

        public List<ScObject> GetTextures()
        {
            return _textures;
        }

        public void SetEofOffset(long offset)
        {
            _eofOffset = offset;
        }


        public void SetEofTexOffset(long offset)
        {
            _eofTexOffset = offset;
        }

        public void addPendingMatrix(Matrix data)
        {
            _pendingMatrixs.Add(data);
        }

        public void SetSofTagsOffset(long offset)
        {
            _sofTagsOffset = offset;
        }

        public void SetStartExportsOffset(long offset)
        {
            _exportStartOffset = offset;
        }

        public void Save(FileStream input, FileStream texinput)
        {
            int exportAdd = 0;
            int textureAdd = 0;
            int movieClipAdd = 0;
            int movieClipEdits = 0;
            int shapeAdd = 0;
            int shapeChunkAdd = 0;
            int matrixAdd = 0;
            int colorsAdd = 0;
            int textFieldsAdd = 0;

            if (_pendingMatrixs.Count > 0)
            {
                input.Seek(0, SeekOrigin.Begin);

                using (MemoryStream newData = new MemoryStream())
                {
                    long offsetBefore = _eofMatrixOffset;

                    byte[] tillMatrixEndData = new byte[_eofMatrixOffset];
                    input.Read(tillMatrixEndData, 0, (int)_eofMatrixOffset);

                    newData.Write(tillMatrixEndData, 0, tillMatrixEndData.Length);

                    foreach (Matrix matrix in _pendingMatrixs)
                    {
                        newData.WriteByte(8);
                        newData.Write(BitConverter.GetBytes(24), 0, 4);

                        Matrix newMatrix = new Matrix(matrix.Elements[0] / 0.00097656f, matrix.Elements[1] / 0.00097656f, matrix.Elements[2] / 0.00097656f,
                            matrix.Elements[3] / 0.00097656f, matrix.Elements[4] * 20f, matrix.Elements[5] * 20f);

                        for (int i = 0; i < 6; i++)
                        {
                            newData.Write(BitConverter.GetBytes((int)newMatrix.Elements[i]), 0, 4);
                        }

                        _eofMatrixOffset = newData.Position;

                        matrixAdd += 1;
                    }

                    byte[] restData = new byte[input.Length - offsetBefore];
                    input.Read(restData, 0, restData.Length);
                    newData.Write(restData, 0, restData.Length);

                    input.Seek(0, SeekOrigin.Begin);
                    newData.Seek(0, SeekOrigin.Begin);
                    newData.CopyTo(input);
                }

                _pendingMatrixs.Clear();

                input.Seek(8, SeekOrigin.Begin);
                this._matrixCount += matrixAdd;
                input.Write(BitConverter.GetBytes((ushort)this._matrixCount), 0, 2);

                input.Close();
                reloadInfoFile();
                input = new FileStream(_infoFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            }

            if (_pendingColors.Count > 0)
            {
                input.Seek(0, SeekOrigin.Begin);

                using (MemoryStream newData = new MemoryStream())
                {
                    long offsetBefore = _eofColorsOffset;

                    byte[] tillColorEndData = new byte[_eofColorsOffset];
                    input.Read(tillColorEndData, 0, (int)_eofColorsOffset);

                    newData.Write(tillColorEndData, 0, tillColorEndData.Length);

                    foreach (Tuple<Color, byte, Color> color in _pendingColors)
                    {
                        newData.WriteByte(9);
                        newData.Write(BitConverter.GetBytes(7), 0, 4);

                        newData.WriteByte(color.Item1.R);
                        newData.WriteByte(color.Item1.G);
                        newData.WriteByte(color.Item1.B);
                        newData.WriteByte(color.Item2);
                        newData.WriteByte(color.Item1.R);
                        newData.WriteByte(color.Item1.G);
                        newData.WriteByte(color.Item1.B);

                        _eofColorsOffset = newData.Position;

                        colorsAdd += 1;
                    }

                    byte[] restData = new byte[input.Length - offsetBefore];
                    input.Read(restData, 0, restData.Length);
                    newData.Write(restData, 0, restData.Length);

                    input.Seek(0, SeekOrigin.Begin);
                    newData.Seek(0, SeekOrigin.Begin);
                    newData.CopyTo(input);
                }

                _pendingColors.Clear();

                input.Seek(10, SeekOrigin.Begin);
                this._colorsCount += colorsAdd;
                input.Write(BitConverter.GetBytes((ushort)this._colorsCount), 0, 2);

                input.Close();
                reloadInfoFile();
                input = new FileStream(_infoFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            }

            // Flushing depending edits.
            List<ScObject> exports = new List<ScObject>();
            for (int i = 0; i < _pendingChanges.Count; i++)
            {
                ScObject data = _pendingChanges[i];

                switch (data.GetDataType())
                {
                    case 7: // Exports
                        exports.Add(data);
                        exportAdd += 1;
                        break;

                    case 2: // Texture
                        data.Write(texinput);
                        textureAdd += 1;
                        break;

                    case 5: // TextFields
                        data.Write(input);
                        textFieldsAdd += 1;
                        break;

                    case 1: // MovieClip
                        data.Write(input);

                        if (data.customAdded == true)
                        {
                            movieClipAdd += 1;
                            this._movieClipCount += 1;
                            input.Seek(2, SeekOrigin.Begin);
                            input.Write(BitConverter.GetBytes(this._movieClipCount), 0, 2);
                            goto case -256;
                        }

                        _pendingChanges.RemoveAt(i);
                        
                        input.Close();
                        reloadInfoFile();
                        input = new FileStream(_infoFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                        movieClipEdits += 1;
                        break;

                    case 0: // Shape
                        data.Write(input);
                        shapeAdd += 1;

                        if (data.customAdded == true)
                        {
                            this._shapeCount += 1;
                            input.Seek(0, SeekOrigin.Begin);
                            input.Write(BitConverter.GetBytes(this._shapeCount), 0, 2);
                            goto case -256;
                        }
                        break;

                    case 99: // ShapeChunk
                        data.Write(input);
                        shapeChunkAdd += 1;

                        
                        break;

                    case -256:
                        if (data.GetDataType() == -256)
                            throw new Exception("Datatype equals -256 Save()");

                        this.SetEofOffset(input.Length);
                        input.Seek(0, SeekOrigin.End);
                        input.Write(new byte[] { 0, 0, 0, 0, 0 });
                        break;

                    default:
                        data.Write(input);
                        break;
                }
            }

            _pendingChanges.Clear();

            input.Close();
            reloadInfoFile();
            input = new FileStream(_infoFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);

            if (exports.Count > 0)
            {
                foreach (ScObject data in exports)
                {
                    data.Write(input);
                }
            }

            // Saving metadata/header.
            input.Seek(4, SeekOrigin.Begin);
            input.Write(BitConverter.GetBytes((ushort)this._textures.Count), 0, 2);
            input.Close();

            Console.WriteLine($"SaveSC: Done saving Exports: {exportAdd} | MovieClips: {movieClipAdd + movieClipEdits} | Shapes: {shapeAdd} | Shape Chunks: {shapeChunkAdd} | Textures: {textureAdd} | Matrixs {matrixAdd} | Colors {colorsAdd}");
        }

        public void Load()
        {
            var sw = Stopwatch.StartNew();

            LoadTextureFile();
            loadInfoFile();

            sw.Stop();
            Program.Interface.Text = $@"SC Editor :  {Path.GetFileNameWithoutExtension(_textureFile)}";
            Program.Interface.Update();
            Console.WriteLine(@"SC File loading finished in {0}ms", sw.Elapsed.TotalMilliseconds);
        }

        public void LoadTextureFile()
        {
            while (true)
            {
                using (var texReader = new BinaryReader(File.OpenRead(_textureFile)))
                {
                    Byte[] IsCompressed = texReader.ReadBytes(2);
                    if (BitConverter.ToString(IsCompressed) == "53-43")
                    {
                        DialogResult result = MessageBox.Show("The tool detected that you have load a compressed file.\nWould you like to decompress and load it?", @"SC File is Compressed", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);

                        if (result == DialogResult.Yes)
                        {

                            byte[] version = texReader.ReadBytes(4);
                            byte[] hashLength = texReader.ReadBytes(4);
                            var hash = texReader.ReadBytes(hashLength[3]);

                            if (version[3] == 1 || version[3] == 3 || version[3] == 0)
                            {
                                string scCompressionType = BitConverter.ToString(texReader.ReadBytes(4));
                                texReader.Close();

                                if (scCompressionType == "53-43-4C-5A")
                                {
                                    Console.WriteLine("LZHAM Compression for " + Path.GetFileName(_textureFile));
                                    Lzma.Decompress(_textureFile);
                                }
                                else if (scCompressionType == "28-B5-2F-FD")
                                {
                                    Console.WriteLine("Zstandard Compression for " + Path.GetFileName(_textureFile));
                                    zstandard.decompress(_textureFile);
                                }
                                else
                                {
                                    Console.WriteLine("LZMA Compression for " + Path.GetFileName(_textureFile));
                                    Lzma.Decompress(_textureFile);
                                }
                            }

                            continue;
                        }
                        break;
                    }

                    Console.WriteLine(texReader.BaseStream.Length);
                    texReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    while (true)
                    {
                        long texoffset = texReader.BaseStream.Position;
                        var packetId = texReader.ReadByte();
                        var packetSize = texReader.ReadUInt32();
                        if (packetSize > 0)
                        {
                            var tex = new Texture(this);
                            tex.SetOffset(texoffset);
                            tex.Read(packetId, packetSize, texReader);

                            if (texoffset + packetSize + 5 != texReader.BaseStream.Position)
                                throw new Exception("Reading less or more bytes for texture");

                            this._textures.Add(tex);
                            if (texReader.BaseStream.Position != texReader.BaseStream.Length)
                            {
                                continue;
                            }
                        }
                        _eofTexOffset = texoffset;
                        break;
                    }
                }
                break;
            }
        }

        public void loadInfoFile()
        {
            while (true)
            {
                using (var reader = new BinaryReader(File.OpenRead(_infoFile)))
                {
                    Byte[] IsCompressed = reader.ReadBytes(2);
                    Console.WriteLine(BitConverter.ToString(IsCompressed) == "53-43");
                    if (BitConverter.ToString(IsCompressed) == "53-43")
                    {
                        DialogResult result = MessageBox.Show("The tool detected that you have load a compressed file.\nWould you like to decompress and load it?", @"SC File is Compressed", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);

                        if (result == DialogResult.Yes)
                        {

                            byte[] version = reader.ReadBytes(4);
                            byte[] hashLength = reader.ReadBytes(4);
                            var hash = reader.ReadBytes(hashLength[3]);

                            if (version[3] == 1 || version[3] == 3 || version[3] == 0)
                            {
                                string scCompressionType = BitConverter.ToString(reader.ReadBytes(4));
                                reader.Close();

                                if (scCompressionType == "53-43-4C-5A")
                                {
                                    Console.WriteLine("LZHAM Compression for " + Path.GetFileName(_infoFile));
                                    Lzma.Decompress(_textureFile);
                                }
                                else if (scCompressionType == "28-B5-2F-FD")
                                {
                                    Console.WriteLine("Zstandard Compression for " + Path.GetFileName(_infoFile));
                                    zstandard.decompress(_infoFile);
                                }
                                else
                                {
                                    Console.WriteLine("LZMA Compression for " + Path.GetFileName(_infoFile));
                                    Lzma.Decompress(_infoFile);
                                }
                            }

                            continue;
                        }
                        break;
                    }

                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    _shapeCount = reader.ReadUInt16();
                    _movieClipCount = reader.ReadUInt16();
                    _textureCount = reader.ReadUInt16();
                    _textFieldCount = reader.ReadUInt16();
                    _matrixCount = reader.ReadUInt16();
                    _colorsCount = reader.ReadUInt16();

                    // 5 useless bytes
                    reader.ReadByte();
                    reader.ReadUInt16();
                    reader.ReadUInt16();

                    _exportStartOffset = reader.BaseStream.Position;
                    _exportCount = reader.ReadUInt16();

#if DEBUG
                    Console.WriteLine(@"ExportCount: " + _exportCount);
                    Console.WriteLine(@"ShapeCount: " + _shapeCount);
                    Console.WriteLine(@"MovieClipCount: " + _movieClipCount);
                    Console.WriteLine(@"TextureCount: " + _textureCount);
                    Console.WriteLine(@"TextFieldCount: " + _textFieldCount);
                    Console.WriteLine(@"Matrix2x3Count: " + _matrixCount);
                    Console.WriteLine(@"ColorTransformCount: " + _colorsCount);
#endif

                    // Reads the Export IDs.
                    for (int i = 0; i < _exportCount; i++)
                    {
                        var export = new Export(this);
                        export.SetId(reader.ReadUInt16());
                        _exports.Add(export);
                    }

                    // Reads the Export names.
                    for (int i = 0; i < _exportCount; i++)
                    {
                        var nameLength = reader.ReadByte();
                        if (nameLength != 255)
                        {
                            var name = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));
                            var export = (Export)_exports[i];
                            export.SetExportName(name);
                        }
                    }

                    int shapeIndex = 0;
                    int movieClipIndex = 0;
                    int movieClipModifierIndex = 0;
                    int textFieldIndex = 0;
                    int matrixIndex = 0;

                    //bool canLoadTex = true;
                    //bool _texDependency = false;

                    _sofTagsOffset = reader.BaseStream.Position;

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        long offset = reader.BaseStream.Position;

                        var datatag = reader.ReadByte();
                        var tagSize = reader.ReadUInt32();

                        var tag = datatag.ToString("X2");

                        if (tagSize < 0)
                            throw new Exception("Negative tag length. Tag " + tag);

                        switch (tag)
                        {
                            case "00": //0
                                if (_shapeCount != shapeIndex ||
                                _movieClipCount != movieClipIndex ||
                                _matrixCount != matrixIndex)
                                {
                                    throw new Exception("Didn't load whole .sc properly.");
                                }

                                _eofOffset = offset;
                                foreach (ScObject t in _exports)
                                {
                                    int index = _movieClips.FindIndex(movie => movie.Id == t.Id);
                                    if (index != -1)
                                        ((Export)t).SetDataObject((MovieClip)_movieClips[index]);
                                }
                                break;

                            case "01":
                            case "10": //16
                            case "13": //19
                            case "1B": //27
                            case "1C": //28
                            case "1D": //29
                            case "22": //34
                            case "18": //24 - not using check
                                if (tagSize > 6)
                                {
                                    var texture = new Texture(this);
                                    texture.SetOffset(offset);
                                    texture.Read(datatag, tagSize, reader);
                                    //_eofTexOffset = reader.BaseStream.Position;
                                    this._textures.Add(texture);
                                }
                                else
                                {
                                    var pixelFormat = reader.ReadByte();
                                    var width = reader.ReadUInt16();
                                    var height = reader.ReadUInt16();

                                    Console.WriteLine("pixelFormat: " + pixelFormat);
                                    Console.WriteLine("width: " + width);
                                    Console.WriteLine("height: " + height);
                                }
                                break;

                            case "02":
                            case "12": //18
                                if (shapeIndex >= _shapeCount)
                                    throw new Exception($"Trying to load too many shapes.\n Index: {shapeIndex} | Count: {_shapeCount}");

                                var shape = new Shape(this);
                                shape.SetOffset(offset);
                                shape.Read(reader, tag);
                                this._shapes.Add(shape);

                                _eofShapeOffset = reader.BaseStream.Position;

                                shapeIndex += 1;
                                break;

                            case "03":
                            case "0A": //10
                            case "0E": //14
                            case "23": //35
                            case "0C": //12
                                if (movieClipIndex >= _movieClipCount)
                                    throw new Exception($"Trying to load too many movieclips.\n Index: {movieClipIndex} | Count: {_movieClipCount}");

                                var movieClip = new MovieClip(this, datatag);
                                movieClip.SetOffset(offset);
                                ushort clipId = movieClip.ReadMV(reader, tag, tagSize);
                                _movieClips.Add(movieClip);

                                _eofMovieClipOffset = reader.BaseStream.Position;

                                movieClipIndex += 1;
                                break;

                            case "07":
                            case "0F": //15
                            case "14": //20
                            case "15": //21
                            case "19": //25
                            case "21": //33
                            case "2B": //43
                            case "2C": //44
                                if (textFieldIndex >= _textFieldCount)
                                    throw new Exception($"Trying to load too many TextFields. \n Index: {textFieldIndex} | Count: {_textFieldCount}");

                                TextField textField = new TextField(this, datatag);
                                textField.Read(reader, tag);
                                _textFields.Add(textField);

                                _eofTextFieldOffset = reader.BaseStream.Position;

                                textFieldIndex += 1;
                                break;

                            case "08":
                                float[] Points = new float[6];
                                for (int Index = 0; Index < 6; Index++)
                                {
                                    Points[Index] = reader.ReadInt32();
                                }
                                Matrix _Matrix = new Matrix(Points[0] * 0.00097656f, Points[1] * 0.00097656f, Points[2] * 0.00097656f,
                                    Points[3] * 0.00097656f, Points[4] / 20f, Points[5] / 20f);
                                this._matrixs.Add(_Matrix);

                                _eofMatrixOffset = reader.BaseStream.Position;

                                matrixIndex++;
                                break;

                            case "09":
                                var ra = reader.ReadByte();
                                var ga = reader.ReadByte();
                                var ba = reader.ReadByte();
                                var am = reader.ReadByte();
                                var rm = reader.ReadByte();
                                var gm = reader.ReadByte();
                                var bm = reader.ReadByte();
                                this._colors.Add(new Tuple<Color, byte, Color>(Color.FromArgb(ra, ga, ba), am, Color.FromArgb(rm, gm, bm)));

                                _eofColorsOffset = reader.BaseStream.Position;
                                break;

                            case "0D": // 13 
                                reader.ReadInt32();
                                throw new Exception("TAG_TIMELINE_INDEXES no longer in use");

                            case "17": // 23
                                // TODO
                                break;

                            case "1A": //26
                                //canLoadTex = false;
                                //_texDependency = true;
                                break;

                            case "1E": // 30
                                // CUSTOM TEXFILE
                                break;

                            case "20": // 32
                                var stg1Count = reader.ReadByte();
                                if (stg1Count != 255)
                                {
                                    reader.Read(new byte[stg1Count], 0, stg1Count);
                                }

                                var stg2Count = reader.ReadByte();
                                if (stg2Count != 255)
                                {
                                    reader.Read(new byte[stg2Count], 0, stg2Count);
                                }
                                //TODO
                                break;

                            case "24":
                                float[] Points2 = new float[6];
                                for (int Index = 0; Index < 6; Index++)
                                {
                                    Points2[Index] = reader.ReadInt32();
                                }
                                Matrix _Matrix2 = new Matrix(Points2[0] / 65535f, Points2[1] / 65535f, Points2[2] / 65535f,
                                    Points2[3] / 65535f, Points2[4] / 20f, Points2[5] / 20f);
                                this._matrixs.Add(_Matrix2);

                                _eofMatrixOffset = reader.BaseStream.Position;

                                matrixIndex++;
                                break;

                            case "25": //37
                                ushort movieClipModifierCount = reader.ReadUInt16();

                                for (int i = 0; i < movieClipModifierCount; i++)
                                {
                                    this._movieClipsModifier.Add(new MovieClipModifier(this));
                                }
                                break;

                            case "26": //38
                            case "27": //39
                            case "28": //40
                                MovieClipModifier mcmo = new MovieClipModifier(this);
                                mcmo.Read(reader, tag);

                                this._movieClipsModifier[movieClipModifierIndex++] = mcmo;
                                break;

                            default:
                                Console.WriteLine($"LoadingSC: Unknown Tag {datatag} with size {tagSize}");
                                reader.ReadBytes(Convert.ToInt32(tagSize));
                                break;
                        }

                        //if ((offset + tagSize + 5) != reader.BaseStream.Position)
                        //    throw new Exception($"Started with offset {offset} trying to load data of size {tagSize} but current position is {reader.BaseStream.Position}.\n DataTag {datatag}; Hex: {tag}");
                    }

                    if (_movieClips.Count < _movieClipCount)
                        Console.WriteLine($"Loaded less MovieClips than expected.\nSCCount: {_movieClipCount} | LoadCount: {_movieClips.Count}, IndexCount {movieClipIndex}");

                    if (_shapes.Count < _shapeCount)
                        Console.WriteLine($"Loaded less shapes than expected.\nSCCount: {_shapeCount} | LoadCount: {_shapes.Count}, IndexCount {shapeIndex}");

                }
                break;
            }
        }

        public void reloadInfoFile()
        {
            ScObject[] previousPendingChanges = (ScObject[])_pendingChanges.ToArray().Clone();
            Matrix[] previousPendingMatrix = (Matrix[])_pendingMatrixs.ToArray().Clone();
            ScObject[] previousTextureData = (ScObject[])_textures.ToArray().Clone();
            Tuple<Color, byte, Color>[] previousPendingColor = (Tuple<Color, byte, Color>[])_pendingColors.ToArray().Clone();

            _shapes = new List<ScObject>();
            _exports = new List<ScObject>();
            _movieClips = new List<ScObject>();
            _movieClipsModifier = new List<ScObject>();
            _pendingChanges = new List<ScObject>();
            _pendingMatrixs = new List<Matrix>();
            _matrixs = new List<Matrix>();
            _colors = new List<Tuple<Color, byte, Color>>();
            _exportCount = 0;
            _shapeCount = 0;
            _movieClipCount = 0;
            _textFieldCount = 0;
            _matrixCount = 0;
            _colorsCount = 0;
            _eofOffset = 0;
            _eofMatrixOffset = 0;
            _exportStartOffset = 0;
            _sofTagsOffset = 0;
            _eofColorsOffset = 0;

            this.loadInfoFile();

            this._textures = new List<ScObject>(previousTextureData.ToList());

            foreach (ScObject pendingData in previousPendingChanges)
            {
                int pIndex = -1;
                int type = pendingData._offset < 0 ? -1 : 1;

                switch (pendingData.GetDataType())
                {
                    case 1:
                        pIndex = _movieClips.FindIndex(mv => mv.Id == pendingData.Id);
                        if (pIndex != -1)
                        {
                            long newOffset = _movieClips[pIndex]._offset;
                            pendingData._offset = newOffset * type;
                        }
                        
                        if (pendingData.customAdded == true)
                        {
                            pendingData._offset = this._eofMovieClipOffset;
                        }
                        break;

                    case 7:
                        pIndex = _exports.FindIndex(mv => mv.Id == pendingData.Id);
                        if (pIndex != -1)
                        {
                            long newOffset = _exports[pIndex]._offset;
                            pendingData._offset = newOffset * type;
                        }
                        break;

                    case 0:
                        pIndex = _shapes.FindIndex(mv => mv.Id == pendingData.Id);
                        if (pIndex != -1)
                        {
                            long newOffset = _shapes[pIndex]._offset;
                            pendingData._offset = newOffset * type;
                        }

                        if (pendingData.customAdded == true)
                        {
                            pendingData._offset = this._eofShapeOffset;
                        }
                        break;
                } 

                _pendingChanges.Add(pendingData);
            }

            foreach (Matrix pendingMatrix in previousPendingMatrix)
            {
                _pendingMatrixs.Add(pendingMatrix);
            }

            foreach (Tuple<Color, byte, Color> color in previousPendingColor)
            {
                _pendingColors.Add(color);
            }
        }

        public int exportExists(string exportName)
        {
            int value = this._exports.FindIndex(exp => exp.GetName() == exportName);
            return value;
        }

        public void addFontName(string name)
        {
            _fontNames.Add(name);
        }

        public List<string> getFontNames()
        {
            return _fontNames;
        }

        public int shapeExists(int id)
        {
            int value = this._shapes.FindIndex(shp => shp.Id == id);
            return value;
        }

        public List<ScObject> getTextFields()
        {
            return _textFields;
        }

        public void addTextField(ScObject data)
        {
            _textFields.Add(data);
        }

        public void addColor(Tuple<Color, byte, Color> color)
        {
            _colors.Add(color);
        }

        public void addPendingColor(Tuple<Color, byte, Color> color)
        {
            _pendingColors.Add(color);
        }

        public ushort getMaxId()
        {
            ushort maxId = 0;

            foreach (Export ex in _exports)
            {
                if (ex.Id > maxId)
                {
                    maxId = ex.Id;
                }
            }

            foreach (Shape s in _shapes)
            {
                if (s.Id > maxId)
                {
                    maxId = s.Id;
                }
            }

            foreach (MovieClip mv in _movieClips)
            {
                if (mv.Id > maxId)
                {
                    maxId = mv.Id;
                }
            }

            foreach (TextField tx in _textFields)
            {
                if (tx.Id > maxId)
                {
                    maxId = tx.Id;
                }
            }

            return (maxId += 100);
        }
    }
}
