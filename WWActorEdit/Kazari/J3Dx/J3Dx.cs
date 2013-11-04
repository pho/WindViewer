#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

#endregion

namespace WWActorEdit.Kazari.J3Dx
{
    public class J3Dx
    {
        #region Variables

        public static string[] ValidExtensions = new string[] { ".bmd", ".bdl", ".bck", ".brk", ".btk" };

        string Name = string.Empty;
        byte[] Data;
        FileHeader Header;
        List<FileChunk> Chunks = new List<FileChunk>();

        int NumVertices;
        VertexArrays VtxArrays;

        SceneGraph Scene;
        EVP1 EVP1Data;
        DrawChunk DrawData;
        List<SceneGraphRaw> SceneGraphRaws;
        List<VertexFormat> VertexFormats;
        public List<Joint> Joints;
        Shape ShapeData;
        List<Texture> Textures;
        Material MaterialData;
        ANK1 AnimData;

        public bool ShowBoundingBoxes = false;
        public bool ShowSkeleton = false;

        TreeNode Root;
        int SceneGLID = 0;

        public RARC.FileEntry FileEntry;

        #endregion

        #region Constructors, J3Dx Loader

        public J3Dx(TreeNode TN)
        {
            Root = TN;
        }

        public J3Dx(RARC.FileEntry FE, TreeNode TN)
        {
            Root = TN;
            FileEntry = FE;
            Name = FE.FileName;
            Load(FE.GetFileData());
        }

        public void Clear()
        {
            if (GL.IsList(SceneGLID)) GL.DeleteLists(SceneGLID, 1);

            if (Textures != null)
                foreach (Texture Tex in Textures)
                    if (GL.IsTexture(Tex.GLID)) GL.DeleteTexture(Tex.GLID);
        }

        public void Load(string Path)
        {
            byte[] DataArray = Helpers.LoadBinary(Path);
            Load(DataArray);
        }

        public void Load(byte[] DataArray)
        {
            Data = DataArray;

            Header = new FileHeader(Data);
            if (Header.Tag.Substring(0, 3) != "J3D")
                throw new Exception("Invalid file specified");

            UInt32 ReadOffset = 0x20;
            for (int i = 0; i < Header.Chunks; i++)
                Chunks.Add(new FileChunk(Data, ref ReadOffset));

            ReadINF1(Chunks.Find(r => r.Tag.Equals("INF1")));
            ReadVTX1(Chunks.Find(r => r.Tag.Equals("VTX1")));
            ReadEVP1(Chunks.Find(r => r.Tag.Equals("EVP1")));
            ReadDRW1(Chunks.Find(r => r.Tag.Equals("DRW1")));
            ReadJNT1(Chunks.Find(r => r.Tag.Equals("JNT1")));
            ReadSHP1(Chunks.Find(r => r.Tag.Equals("SHP1")));
            ReadTEX1(Chunks.Find(r => r.Tag.Equals("TEX1")));
            ReadMAT3(Chunks.Find(r => r.Tag.Equals("MAT3")));
            ReadANK1(Chunks.Find(r => r.Tag.Equals("ANK1")));

            if (VertexFormats != null && SceneGraphRaws != null)
            {
                ReadVertexArrays();

                Scene = new SceneGraph();
                Scene.RelativeMatrix = Matrix4.Identity;
                BuildSceneGraph(ref Scene);

                GenerateDisplayLists();

                Root.Nodes.Add(Helpers.CreateTreeNode(Name, this, string.Format("Size: {0:X6}", Data.Length)));
            }
        }

        #endregion

        #region Chunk Readers

        void ReadINF1(FileChunk INF1Chunk)
        {
            if (INF1Chunk == null) return;

            NumVertices = (int)Helpers.Read32(INF1Chunk.Data, 0x10);

            SceneGraphRaws = new List<SceneGraphRaw>();
            UInt32 DataOffset = Helpers.Read32(INF1Chunk.Data, 0x14);
            for (int i = 0; i < INF1Chunk.Size; i++)
            {
                SceneGraphRaw NewEntry = new SceneGraphRaw(INF1Chunk.Data, ref DataOffset);
                if (NewEntry.Type == 0x00)
                    break;

                SceneGraphRaws.Add(NewEntry);
            }
        }

        void ReadVTX1(FileChunk VTX1Chunk)
        {
            if (VTX1Chunk == null) return;

            UInt32 DataOffset = Helpers.Read32(VTX1Chunk.Data, 0x8);
            if (DataOffset != 0)
            {
                VertexFormats = new List<VertexFormat>();

                for (int i = 0; i < VTX1Chunk.Size; i++)
                {
                    VertexFormat NewEntry = new VertexFormat(VTX1Chunk.Data, ref DataOffset);
                    if (NewEntry.ArrayType == 0xFF)
                        break;

                    VertexFormats.Add(NewEntry);
                }

                DataOffset = 0xC;
                for (int i = 0, j = 0; i < 13; i++)
                {
                    UInt32 VertDataOffset = Helpers.Read32(VTX1Chunk.Data, (int)DataOffset);
                    DataOffset += 4;

                    if (VertDataOffset == 0)
                        continue;

                    VertexFormats[j].Offset = VertDataOffset;
                    j++;
                }
            }
        }

        void ReadEVP1(FileChunk EVP1Chunk)
        {
            if (EVP1Chunk == null) return;

            EVP1Data = new EVP1(EVP1Chunk.Data);
        }

        void ReadDRW1(FileChunk DRW1Chunk)
        {
            if (DRW1Chunk == null) return;

            DrawData = new DrawChunk(DRW1Chunk.Data);
        }

        void ReadJNT1(FileChunk JNT1Chunk)
        {
            if (JNT1Chunk == null) return;

            Joints = new List<Joint>();

            int JointCount = Helpers.Read16(JNT1Chunk.Data, 0x8);
            UInt32 DataOffset = Helpers.Read32(JNT1Chunk.Data, 0xC);
            int StringTableOffset = (int)Helpers.Read32(JNT1Chunk.Data, 0x14);

            int StringCount = Helpers.Read16(JNT1Chunk.Data, StringTableOffset);

            for (int i = 0; i < JointCount; i++)
                if (StringCount != JointCount)
                    Joints.Add(new Joint(JNT1Chunk.Data, ref DataOffset));
                else
                    Joints.Add(new Joint(JNT1Chunk.Data, ref DataOffset, StringTableOffset + Helpers.Read16(JNT1Chunk.Data, StringTableOffset + 2 + ((i + 1) * 4))));
        }

        void ReadSHP1(FileChunk SHP1Chunk)
        {
            if (SHP1Chunk == null) return;

            ShapeData = new Shape(SHP1Chunk.Data);
        }

        void ReadTEX1(FileChunk TEX1Chunk)
        {
            if (TEX1Chunk == null) return;

            Textures = new List<Texture>();

            int TextureCount = Helpers.Read16(TEX1Chunk.Data, 0x8);
            int TextureHeaderOffset = (int)Helpers.Read32(TEX1Chunk.Data, 0xC);
            int StringTableOffset = (int)Helpers.Read32(TEX1Chunk.Data, 0x10);

            int StringCount = Helpers.Read16(TEX1Chunk.Data, StringTableOffset);

            UInt32 DataOffset = (UInt32)TextureHeaderOffset;
            for (int i = 0; i < TextureCount; i++)
            {
                Texture NewTex;

                if (StringCount != TextureCount)
                    NewTex = new Texture(TEX1Chunk.Data, ref DataOffset);
                else
                    NewTex = new Texture(TEX1Chunk.Data, ref DataOffset, StringTableOffset + Helpers.Read16(TEX1Chunk.Data, StringTableOffset + 2 + ((i + 1) * 4)));

                NewTex.TextureOffset += (UInt32)(TextureHeaderOffset + (i * 32));
                LoadTexture(ref NewTex);

                Textures.Add(NewTex);
            }
        }

        void ReadMAT3(FileChunk MAT3Chunk)
        {
            if (MAT3Chunk == null) return;

            MaterialData = new Material(MAT3Chunk.Data);
        }

        void ReadANK1(FileChunk ANK1Chunk)
        {
            if (ANK1Chunk == null) return;

            AnimData = new ANK1(ANK1Chunk.Data);
        }

        #endregion

        #region Scene Graph Builder

        int BuildSceneGraph(ref SceneGraph Parent, int EntryNumber = 0)
        {
            if (Parent.Children == null) Parent.Children = new List<SceneGraph>();
            Matrix4 CurrentMtx = Parent.RelativeMatrix;

            for (int i = EntryNumber; i < SceneGraphRaws.Count; ++i)
            {
                switch (SceneGraphRaws[i].Type)
                {
                    case (UInt16)SceneGraphTypes.NEW_CHILD:
                        SceneGraph NewEntry = new SceneGraph(SceneGraphRaws[i].Type, SceneGraphRaws[i].Index, CurrentMtx);
                        i += BuildSceneGraph(ref NewEntry, i + 1);
                        Parent.Children.Add(NewEntry);
                        break;
                    case (UInt16)SceneGraphTypes.CLOSE_CHILD:
                        return i - EntryNumber + 1;
                    case (UInt16)SceneGraphTypes.JOINT:
                        Joints[SceneGraphRaws[i].Index].Matrix = Matrix4.Mult(JointMatrix(Joints[SceneGraphRaws[i].Index]), Parent.RelativeMatrix);
                        CurrentMtx = Joints[SceneGraphRaws[i].Index].Matrix;
                        break;
                }
                Parent.Children.Add(new SceneGraph(SceneGraphRaws[i].Type, SceneGraphRaws[i].Index, CurrentMtx));
            }

            return 0;
        }

        #endregion

        #region Vertex Array Reader

        int GetVertexArrayLength(int FmtNumber, UInt32 ChunkLength)
        {
            if (FmtNumber < VertexFormats.Count - 1)
                return (int)(VertexFormats[FmtNumber + 1].Offset - VertexFormats[FmtNumber].Offset);
            else
                return (int)(ChunkLength - VertexFormats[FmtNumber].Offset);
        }

        void ReadVertexArrays()
        {
            FileChunk VtxChunk = Chunks.Find(r => r.Tag.Equals("VTX1"));

            VtxArrays = new VertexArrays();

            byte[] VtxDataReversed = VtxChunk.Data.Reverse().ToArray();

            foreach (VertexFormat Fmt in VertexFormats)
            {
                float[] Data = null;

                int DataLength = GetVertexArrayLength(VertexFormats.IndexOf(Fmt), VtxChunk.Size);
                switch (Fmt.DataType)
                {
                    case (UInt32)DataTypes.S16:
                        {
                            Data = new float[DataLength / 2];
                            float Scale = (float)Math.Pow(.5f, Fmt.DecimalPoint);
                            for (int i = 0; i < Data.Length; ++i)
                                Data[i] = (float)((Int16)Helpers.Read16(VtxChunk.Data, (int)(Fmt.Offset + (i * 2)))) * Scale;
                            break;
                        }
                    case (UInt32)DataTypes.F32:
                        {
                            Data = new float[DataLength / 4];
                            for (int i = 0; i < Data.Length; ++i)
                                Data[i] = Helpers.ConvertIEEE754Float(Helpers.Read32(VtxChunk.Data, (int)(Fmt.Offset + (i * 4))));
                            break;
                        }
                    case (UInt32)DataTypes.RGBA8:
                        {
                            Data = new float[DataLength];
                            for (int i = 0; i < Data.Length; ++i)
                                Data[i] = Helpers.Read8(VtxChunk.Data, (int)(Fmt.Offset + i)) / 255.0f;
                            break;
                        }
                }

                if (Data == null)
                    continue;

                switch (Fmt.ArrayType)
                {
                    case (UInt32)ArrayTypes.POSITION:
                        {
                            if (Fmt.Count == 0)
                            {
                                VtxArrays.Positions = new Vector3[Data.Length / 2];
                                for (int i = 0, j = 0; i < (Data.Length / 2); ++i, j += 2)
                                    VtxArrays.Positions[i] = new Vector3(Data[j], Data[j + 1], 0.0f);
                            }
                            else if (Fmt.Count == 1)
                            {
                                VtxArrays.Positions = new Vector3[Data.Length / 3];
                                for (int i = 0, j = 0; i < (Data.Length / 3); ++i, j += 3)
                                    VtxArrays.Positions[i] = new Vector3(Data[j], Data[j + 1], Data[j + 2]);
                            }
                            break;
                        }
                    case (UInt32)ArrayTypes.NORMAL:
                        {
                            VtxArrays.Normals = new Vector3[Data.Length / 3];
                            for (int i = 0, j = 0; i < (Data.Length / 3); ++i, j += 3)
                                VtxArrays.Normals[i] = new Vector3(Data[j], Data[j + 1], Data[j + 2]);
                            break;
                        }
                    case (UInt32)ArrayTypes.COLOR0:
                    case (UInt32)ArrayTypes.COLOR1:
                        {
                            int ColorIndex = (int)(Fmt.ArrayType - (UInt32)ArrayTypes.COLOR0);
                            if (Fmt.Count == 0)
                            {
                                VtxArrays.Colors[ColorIndex] = new Color4[Data.Length / 3];
                                for (int i = 0, j = 0; i < (Data.Length / 3); ++i, j += 3)
                                    VtxArrays.Colors[ColorIndex][i] = new Color4(Data[j], Data[j + 1], Data[j + 2], 1.0f);
                            }
                            else if (Fmt.Count == 1)
                            {
                                VtxArrays.Colors[ColorIndex] = new Color4[Data.Length / 4];
                                for (int i = 0, j = 0; i < (Data.Length / 4); ++i, j += 4)
                                    VtxArrays.Colors[ColorIndex][i] = new Color4(Data[j], Data[j + 1], Data[j + 2], Data[j + 3]);
                            }
                            break;
                        }
                    case (UInt32)ArrayTypes.TEX0:
                    case (UInt32)ArrayTypes.TEX1:
                    case (UInt32)ArrayTypes.TEX2:
                    case (UInt32)ArrayTypes.TEX3:
                    case (UInt32)ArrayTypes.TEX4:
                    case (UInt32)ArrayTypes.TEX5:
                    case (UInt32)ArrayTypes.TEX6:
                    case (UInt32)ArrayTypes.TEX7:
                        {
                            int TexCoordIndex = (int)(Fmt.ArrayType - (UInt32)ArrayTypes.TEX0);
                            if (Fmt.Count == 0)
                            {
                                VtxArrays.TexCoords[TexCoordIndex] = new Vector2[Data.Length];
                                for (int i = 0; i < Data.Length; ++i)
                                    VtxArrays.TexCoords[TexCoordIndex][i] = new Vector2(Data[i], 0.0f);
                            }
                            else if (Fmt.Count == 1)
                            {
                                VtxArrays.TexCoords[TexCoordIndex] = new Vector2[Data.Length / 2];
                                for (int i = 0, j = 0; i < (Data.Length / 2); ++i, j += 2)
                                    VtxArrays.TexCoords[TexCoordIndex][i] = new Vector2(Data[j], Data[j + 1]);
                            }
                            break;
                        }
                }
            }
        }

        #endregion

        #region Texture Converter & Dumper

        #region Helper Functions

        void R5G6B5ToRGBA8(UInt16 SrcPixel, ref byte[] Dest, int Offset)
        {
            byte R, G, B;
            R = (byte)((SrcPixel & 0xf100) >> 11);
            G = (byte)((SrcPixel & 0x7e0) >> 5);
            B = (byte)((SrcPixel & 0x1f));

            R = (byte)((R << (8 - 5)) | (R >> (10 - 8)));
            G = (byte)((G << (8 - 6)) | (G >> (12 - 8)));
            B = (byte)((B << (8 - 5)) | (B >> (10 - 8)));

            Dest[Offset] = R;
            Dest[Offset + 1] = G;
            Dest[Offset + 2] = B;
            Dest[Offset + 3] = 0xff;
        }


        void RGB5A3ToRGBA8(UInt16 SrcPixel, ref byte[] Dest, int Offset)
        {
            byte r, g, b, a;

            if ((SrcPixel & 0x8000) == 0x8000)
            {
                a = 0xff;

                r = (byte)((SrcPixel & 0x7c00) >> 10);
                r = (byte)((r << (8 - 5)) | (r >> (10 - 8)));

                g = (byte)((SrcPixel & 0x3e0) >> 5);
                g = (byte)((g << (8 - 5)) | (g >> (10 - 8)));

                b = (byte)(SrcPixel & 0x1f);
                b = (byte)((b << (8 - 5)) | (b >> (10 - 8)));
            }
            else
            {
                a = (byte)((SrcPixel & 0x7000) >> 12);
                a = (byte)((a << (8 - 3)) | (a << (8 - 6)) | (a >> (9 - 8)));

                r = (byte)((SrcPixel & 0xf00) >> 8);
                r = (byte)((r << (8 - 4)) | r);

                g = (byte)((SrcPixel & 0xf0) >> 4);
                g = (byte)((g << (8 - 4)) | g);

                b = (byte)(SrcPixel & 0xf);
                b = (byte)((b << (8 - 4)) | b);
            }

            Dest[Offset] = r;
            Dest[Offset + 1] = g;
            Dest[Offset + 2] = b;
            Dest[Offset + 3] = a;
        }

        void Fix4x4(ref byte[] Dest, byte[] Src, int S, int Width, int Height)
        {
            for (int y = 0; y < Height; y += 4)
                for (int x = 0; x < Width; x += 4)
                    for (int dy = 0; dy < 4; ++dy)
                        for (int dx = 0; dx < 4; ++dx, S += 2)
                            if (x + dx < Width && y + dy < Height)
                            {
                                int di = 2 * (Width * (y + dy) + x + dx);
                                Dest[di + 0] = Src[S + 1];
                                Dest[di + 1] = Src[S + 0];
                            }
        }

        void Fix8x4(ref byte[] Dest, byte[] Src, int S, int Width, int Height)
        {
            for (int y = 0; y < Height; y += 4)
                for (int x = 0; x < Width; x += 8)
                    for (int dy = 0; dy < 4; ++dy)
                        for (int dx = 0; dx < 8; ++dx, ++S)
                            if (x + dx < Width && y + dy < Height)
                                Dest[Width * (y + dy) + x + dx] = Src[S];
        }

        void Fix8x4Expand(ref byte[] Dest, byte[] Src, int S, int Width, int Height)
        {
            for (int y = 0; y < Height; y += 4)
                for (int x = 0; x < Width; x += 8)
                    for (int dy = 0; dy < 4; ++dy)
                        for (int dx = 0; dx < 8; ++dx, ++S)
                            if (x + dx < Width && y + dy < Height)
                            {
                                byte Lum = (byte)(Src[S] & 0xf);
                                Lum |= (byte)(Lum << 4);
                                byte Alpha = (byte)(Src[S] & 0xf0);
                                Alpha |= (byte)(Alpha >> 4);
                                Dest[2 * (Width * (y + dy) + x + dx)] = Lum;
                                Dest[2 * (Width * (y + dy) + x + dx) + 1] = Alpha;
                            }
        }

        void Fix8x8Expand(ref byte[] Dest, byte[] Src, int S, int Width, int Height)
        {
            int y, x, dy, dx;

            for (y = 0; y < Height; y += 8)
                for (x = 0; x < Width; x += 8)
                    for (dy = 0; dy < 8; ++dy)
                        for (dx = 0; dx < 8; dx += 2, ++S)
                            if (x + dx < Width && y + dy < Height)
                            {
                                byte t = (byte)(Src[S] & 0xf0);
                                Dest[Width * (y + dy) + x + dx] = (byte)(t | (t >> 4));
                                t = (byte)(Src[S] & 0xf);
                                Dest[Width * (y + dy) + x + dx + 1] = (byte)((t << 4) | t);
                            }
        }

        void Fix8x8NoExpand(ref byte[] Dest, byte[] Src, int S, int Width, int Height)
        {
            for (int y = 0; y < Height; y += 8)
                for (int x = 0; x < Width; x += 8)
                    for (int dy = 0; dy < 8; ++dy)
                        for (int dx = 0; dx < 8; dx += 2, ++S)
                            if (x + dx < Width && y + dy < Height)
                            {
                                byte t = (byte)(Src[S] & 0xf0);
                                Dest[Width * (y + dy) + x + dx] = (byte)(t >> 4);
                                t = (byte)(Src[S] & 0xf);
                                Dest[Width * (y + dy) + x + dx + 1] = t;
                            }
        }

        void FixRGB5A3(ref byte[] Dest, byte[] Src, int S, int Width, int Height)
        {
            int y, x, dy, dx;
            for (y = 0; y < Height; y += 4)
                for (x = 0; x < Width; x += 4)
                    for (dy = 0; dy < 4; ++dy)
                        for (dx = 0; dx < 4; ++dx, S += 2)
                            if (x + dx < Width && y + dy < Height)
                            {
                                UInt16 srcPixel = Helpers.Read16(Src, S);
                                RGB5A3ToRGBA8(srcPixel, ref Dest, 4 * (Width * (y + dy) + x + dx));
                            }
        }

        void FixR5G6B5(ref byte[] Dest, byte[] Src, int S, int Width, int Height)
        {
            int y, x, dy, dx;
            for (y = 0; y < Height; y += 4)
                for (x = 0; x < Width; x += 4)
                    for (dy = 0; dy < 4; ++dy)
                        for (dx = 0; dx < 4; ++dx, S += 2)
                            if (x + dx < Width && y + dy < Height)
                            {
                                UInt16 srcPixel = Helpers.Read16(Src, S);
                                R5G6B5ToRGBA8(srcPixel, ref Dest, 4 * (Width * (y + dy) + x + dx));
                            }
        }

        void FixRGBA8(ref byte[] Dest, byte[] Src, int S, int Width, int Height)
        {
            for (int y = 0; y < Height; y += 4)
                for (int x = 0; x < Width; x += 4)
                {
                    int dy;

                    for (dy = 0; dy < 4; ++dy)
                        for (int dx = 0; dx < 4; ++dx, S += 2)
                            if (x + dx < Width && y + dy < Height)
                            {
                                UInt32 di = (UInt32)(4 * (Width * (y + dy) + x + dx));
                                Dest[di + 0] = Src[S + 1];
                                Dest[di + 3] = Src[S + 0];
                            }

                    for (dy = 0; dy < 4; ++dy)
                        for (int dx = 0; dx < 4; ++dx, S += 2)
                            if (x + dx < Width && y + dy < Height)
                            {
                                UInt32 di = (UInt32)(4 * (Width * (y + dy) + x + dx));
                                Dest[di + 1] = Src[S + 0];
                                Dest[di + 2] = Src[S + 1];
                            }
                }
        }

        byte S3TC1ReverseByte(byte B)
        {
            byte B1 = (byte)(B & 0x3);
            byte B2 = (byte)(B & 0xC);
            byte B3 = (byte)(B & 0x30);
            byte B4 = (byte)(B & 0xC0);
            return (byte)((B1 << 6) | (B2 << 2) | (B3 >> 2) | (B4 >> 6));
        }

        void FixS3TC1(ref byte[] Dest, byte[] Src, int S, int Width, int Height)
        {
            int y, x, dy, dx;
            for (y = 0; y < Height / 4; y += 2)
                for (x = 0; x < Width / 4; x += 2)
                    for (dy = 0; dy < 2; ++dy)
                        for (dx = 0; dx < 2; ++dx, S += 8)
                            if (4 * (x + dx) < Width && 4 * (y + dy) < Height)
                                Buffer.BlockCopy(Src, S, Dest, 8 * ((y + dy) * Width / 4 + x + dx), 8);

            for (int i = 0; i < Width * Height / 2; i += 8)
            {
                Helpers.Swap(ref Dest[i], ref Dest[i + 1]);
                Helpers.Swap(ref Dest[i + 2], ref Dest[i + 3]);

                Dest[i + 4] = S3TC1ReverseByte(Dest[i + 4]);
                Dest[i + 5] = S3TC1ReverseByte(Dest[i + 5]);
                Dest[i + 6] = S3TC1ReverseByte(Dest[i + 6]);
                Dest[i + 7] = S3TC1ReverseByte(Dest[i + 7]);
            }
        }

        void DecompressDXT1(ref byte[] Dest, byte[] Src, int Width, int Height)
        {
            int Address = 0;

            int y = 0, x = 0, iy = 0, ix = 0;
            for (y = 0; y < Height; y += 4)
            {
                for (x = 0; x < Width; x += 4)
                {
                    UInt16 Color1 = Helpers.Read16Swap(Src, Address);
                    UInt16 Color2 = Helpers.Read16Swap(Src, Address + 2);
                    UInt32 Bits = Helpers.Read32Swap(Src, Address + 4);
                    Address += 8;

                    byte[][] ColorTable = new byte[4][];
                    for (int i = 0; i < 4; i++)
                        ColorTable[i] = new byte[4];

                    R5G6B5ToRGBA8(Color1, ref ColorTable[0], 0);
                    R5G6B5ToRGBA8(Color2, ref ColorTable[1], 0);

                    if (Color1 > Color2)
                    {
                        ColorTable[2][0] = (byte)((2 * ColorTable[0][0] + ColorTable[1][0] + 1) / 3);
                        ColorTable[2][1] = (byte)((2 * ColorTable[0][1] + ColorTable[1][1] + 1) / 3);
                        ColorTable[2][2] = (byte)((2 * ColorTable[0][2] + ColorTable[1][2] + 1) / 3);
                        ColorTable[2][3] = 0xFF;

                        ColorTable[3][0] = (byte)((ColorTable[0][0] + 2 * ColorTable[1][0] + 1) / 3);
                        ColorTable[3][1] = (byte)((ColorTable[0][1] + 2 * ColorTable[1][1] + 1) / 3);
                        ColorTable[3][2] = (byte)((ColorTable[0][2] + 2 * ColorTable[1][2] + 1) / 3);
                        ColorTable[3][3] = 0xFF;
                    }
                    else
                    {
                        ColorTable[2][0] = (byte)((ColorTable[0][0] + ColorTable[1][0] + 1) / 2);
                        ColorTable[2][1] = (byte)((ColorTable[0][1] + ColorTable[1][1] + 1) / 2);
                        ColorTable[2][2] = (byte)((ColorTable[0][2] + ColorTable[1][2] + 1) / 2);
                        ColorTable[2][3] = 0xFF;

                        ColorTable[3][0] = (byte)((ColorTable[0][0] + 2 * ColorTable[1][0] + 1) / 3);
                        ColorTable[3][1] = (byte)((ColorTable[0][1] + 2 * ColorTable[1][1] + 1) / 3);
                        ColorTable[3][2] = (byte)((ColorTable[0][2] + 2 * ColorTable[1][2] + 1) / 3);
                        ColorTable[3][3] = 0x00;
                    }

                    for (iy = 0; iy < 4; ++iy)
                    {
                        for (ix = 0; ix < 4; ++ix)
                        {
                            if (((x + ix) < Width) && ((y + iy) < Height))
                            {
                                int di = 4 * ((y + iy) * Width + x + ix);
                                int si = (int)(Bits & 0x3);
                                Dest[di + 0] = ColorTable[si][0];
                                Dest[di + 1] = ColorTable[si][1];
                                Dest[di + 2] = ColorTable[si][2];
                                Dest[di + 3] = ColorTable[si][3];
                            }
                            Bits >>= 2;
                        }
                    }
                }
            }
        }

        void unpackPixel(int Index, ref byte[] Dest, int Address, byte[] Palette, byte PaletteFormat)
        {
            switch (PaletteFormat)
            {
                case (byte)PaletteFormats.PAL_A8_I8:
                    Dest[0] = Palette[2 * Index + 1];
                    Dest[1] = Palette[2 * Index + 0];
                    break;

                case (byte)PaletteFormats.PAL_R5_G6_B5:
                    R5G6B5ToRGBA8(Helpers.Read16(Palette, 2 * Index), ref Dest, Address);
                    break;

                case (byte)PaletteFormats.PAL_A3_RGB5:
                    RGB5A3ToRGBA8(Helpers.Read16(Palette, 2 * Index), ref Dest, Address);
                    break;
            }
        }

        void unpack8(ref byte[] dst, byte[] src, int w, int h,
                     byte[] palette, byte paletteFormat)
        {
            int Address = 0;

            int pixSize = (paletteFormat == (byte)PaletteFormats.PAL_A8_I8 ? 2 : 4);

            for (int y = 0; y < h; ++y)
                for (int x = 0; x < w; ++x, Address += pixSize)
                    unpackPixel(src[y * w + x], ref dst, Address, palette, paletteFormat);
        }

        #endregion

        void LoadTexture(ref Texture TexInfo)
        {
            FileChunk TexChunk = Chunks.Find(r => r.Tag.Equals("TEX1"));

            int BufferSize = (TexInfo.Width[0] * TexInfo.Height[0]) * 4;
            byte[] Tmp = new byte[BufferSize], Tmp2 = new byte[BufferSize];

            PixelInternalFormat GLInternalFormat = PixelInternalFormat.Rgba;
            PixelFormat GLFormat = PixelFormat.Rgba;

            GL.BindTexture(TextureTarget.Texture2D, TexInfo.GLID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (int)All.True);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)All.Linear);

            for (int i = 0; i < TexInfo.MipmapCount; ++i)
            {
                try
                {
                    switch (TexInfo.Format)
                    {
                        /*
                        I4 = 0,
                        I8 = 1,
                        A4_I4 = 2,
                        A8_I8 = 3,
                        R5_G6_B5 = 4,
                        A3_RGB5 = 5,
                        ARGB8 = 6,
                        INDEX4 = 8,
                        INDEX8 = 9,
                        INDEX14_X2 = 10,
                        S3TC1 = 14
                        */
                        case (byte)TextureFormats.I4:
                            GLInternalFormat = PixelInternalFormat.Intensity;
                            GLFormat = PixelFormat.Luminance;
                            Fix8x8Expand(ref Tmp, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            break;
                        case (byte)TextureFormats.I8:
                            GLInternalFormat = PixelInternalFormat.Intensity;
                            GLFormat = PixelFormat.Luminance;
                            Fix8x4(ref Tmp, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            break;
                        case (byte)TextureFormats.A4_I4:
                            GLInternalFormat = PixelInternalFormat.LuminanceAlpha;
                            GLFormat = PixelFormat.LuminanceAlpha;
                            Fix8x4Expand(ref Tmp, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            break;
                        case (byte)TextureFormats.A8_I8:
                            GLInternalFormat = PixelInternalFormat.LuminanceAlpha;
                            GLFormat = PixelFormat.LuminanceAlpha;
                            Fix4x4(ref Tmp, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            break;
                        case (byte)TextureFormats.R5_G6_B5:
                            FixR5G6B5(ref Tmp, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            break;
                        case (byte)TextureFormats.A3_RGB5:
                            FixRGB5A3(ref Tmp, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            break;
                        case (byte)TextureFormats.ARGB8:
                            FixRGBA8(ref Tmp, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            break;

                        case (byte)TextureFormats.INDEX4:
                            Tmp2 = new byte[Tmp.Length * 2];
                            Fix8x8NoExpand(ref Tmp2, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            unpack8(ref Tmp, Tmp2, TexInfo.Width[i], TexInfo.Height[i], TexInfo.Palette, TexInfo.PalFormat);
                            break;
                        case (byte)TextureFormats.INDEX8:
                            Tmp2 = new byte[Tmp.Length * 2];
                            Fix8x4(ref Tmp2, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            unpack8(ref Tmp, Tmp2, TexInfo.Width[i], TexInfo.Height[i], TexInfo.Palette, TexInfo.PalFormat);
                            break;

                        case (byte)TextureFormats.S3TC1:
                            FixS3TC1(ref Tmp2, TexChunk.Data, (int)TexInfo.TextureOffset, TexInfo.Width[i], TexInfo.Height[i]);
                            DecompressDXT1(ref Tmp, Tmp2, TexInfo.Width[i], TexInfo.Height[i]);
                            break;

                        default:
                            Tmp.Fill(new byte[] { 0xFF, 0x00, 0x00, 0xFF });
                            System.Windows.Forms.MessageBox.Show(
                                string.Format("Unsupported texture type 0x{0:X2}!", TexInfo.Format), "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                            break;
                    }
                }
                catch
                {
                    //
                }

                GL.TexImage2D<byte>(TextureTarget.Texture2D, i, GLInternalFormat, TexInfo.Width[i], TexInfo.Height[i], 0, GLFormat, PixelType.UnsignedByte, Tmp);
            }
        }

        public void DumpTextures(string DumpPath)
        {
            foreach (Texture TexInfo in Textures)
            {
                GL.BindTexture(TextureTarget.Texture2D, TexInfo.GLID);

                for (int i = 0; i < TexInfo.MipmapCount; ++i)
                {
                    byte[] Buffer = new byte[(TexInfo.Width[i] * TexInfo.Height[i]) * 4];

                    GL.GetTexImage<byte>(TextureTarget.Texture2D, i, PixelFormat.Bgra, PixelType.UnsignedByte, Buffer);

                    /* Convert IIIx/IIIA to BGRA */
                    switch (TexInfo.Format)
                    {
                        case (byte)TextureFormats.I4:
                        case (byte)TextureFormats.I8:
                            for (int j = 0; j < Buffer.Length; j += 4)
                            {
                                Buffer[j] = Buffer[j + 2];
                                Buffer[j + 1] = Buffer[j + 2];
                            }
                            break;
                        case (byte)TextureFormats.A4_I4:
                        case (byte)TextureFormats.A8_I8:
                            for (int j = 0; j < Buffer.Length; j += 4)
                            {
                                Buffer[j] = Buffer[j + 2];
                                Buffer[j + 1] = Buffer[j + 2];
                            }
                            break;
                    }

                    Bitmap TexImage = new Bitmap(TexInfo.Width[i], TexInfo.Height[i]);
                    Rectangle Rect = new Rectangle(0, 0, TexInfo.Width[i], TexInfo.Height[i]);
                    System.Drawing.Imaging.BitmapData BmpData = TexImage.LockBits(Rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, TexImage.PixelFormat);
                    IntPtr Ptr = BmpData.Scan0;
                    System.Runtime.InteropServices.Marshal.Copy(Buffer, 0, Ptr, Buffer.Length);
                    TexImage.UnlockBits(BmpData);

                    Directory.CreateDirectory(Path.Combine(DumpPath, Name));
                    string SavePath = Path.Combine(DumpPath, Name, String.Format("{0} ({1}-{2}).png", TexInfo.Name, Enum.GetName(typeof(TextureFormats), TexInfo.Format), i));
                    TexImage.Save(SavePath, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        #endregion

        #region Renderer

        public void Animate(List<Joint> Joints, float FTime)
        {
            AnimData.Animate(Joints, FTime);
        }

        private double DegreeToRadian(double angle)
        {
            return (Math.PI / 182.0444444) * angle;
        }

        Matrix4 JointMatrix(J3Dx.Joint Jnt)
        {
            Matrix4 Trans = Matrix4.CreateTranslation(Jnt.Translation);
            Matrix4 RotX = Matrix4.CreateRotationX((float)(DegreeToRadian(Jnt.Rotation.X)));
            Matrix4 RotY = Matrix4.CreateRotationY((float)(DegreeToRadian(Jnt.Rotation.Y)));
            Matrix4 RotZ = Matrix4.CreateRotationZ((float)(DegreeToRadian(Jnt.Rotation.Z)));
            Matrix4 Scale = Matrix4.Scale(Jnt.Scale);

            return Matrix4.Mult(Scale, Matrix4.Mult(RotX, Matrix4.Mult(RotY, Matrix4.Mult(RotZ, Trans))));
        }

        void DrawSkeleton(SceneGraph Current, Vector3 LastPoint)
        {
            Vector3 NewPoint = Vector3.Transform(new Vector3(0.0f, 0.0f, 0.0f), Current.RelativeMatrix);

            if (Current.Type == (UInt16)SceneGraphTypes.JOINT)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex3(LastPoint);
                GL.Color3(0.0f, 0.0f, 1.0f);
                GL.Vertex3(NewPoint);
                GL.End();

                string Message = string.Format("{0}: {1}"/* + "\nT:{2}\nR:{3}\nS:{4}"*/,
                    Current.Index, Joints[Current.Index].Name/*, Joints[Current.Index].Translation, Joints[Current.Index].Rotation, Joints[Current.Index].Scale*/);

                Helpers.PrintText(NewPoint, Color.White, SystemFonts.DialogFont, Message);
            }

            if (Current.Children != null)
                for (int i = 0; i < Current.Children.Count; i++)
                    DrawSkeleton(Current.Children[i], NewPoint);
        }

        void DrawSceneGraph(SceneGraph Current, ref int MatIndex)
        {
            switch (Current.Type)
            {
                case (UInt16)SceneGraphTypes.MATERIAL:
                    MatIndex = Current.Index;
                    break;
                case (UInt16)SceneGraphTypes.SHAPE:
                    BindMaterial(MatIndex);
                    DrawBatch(ShapeData.Batches[Current.Index]);
                    break;
            }

            if (Current.Children != null)
                for (int i = 0; i < Current.Children.Count; i++)
                    DrawSceneGraph(Current.Children[i], ref MatIndex);
        }

        public void GenerateDisplayLists()
        {
            if (Scene == null) return;

            if (GL.IsList(SceneGLID) == true) GL.DeleteLists(SceneGLID, 1);
            SceneGLID = GL.GenLists(1);

            GL.NewList(SceneGLID, ListMode.Compile);
            {
                int MatIndex = 0;

                if (ShowSkeleton == true)
                    DrawSkeleton(Scene, new Vector3(0.0f, 0.0f, 0.0f));
                else
                    DrawSceneGraph(Scene, ref MatIndex);
            }
            GL.EndList();
        }

        All GetTexFilter(byte Filter)
        {
            switch (Filter)
            {
                case 0:
                    return All.Nearest;
                case 1:
                    return All.Linear;
                case 2:
                    return All.NearestMipmapNearest;
                case 3:
                    return All.LinearMipmapNearest;
                case 4:
                    return All.NearestMipmapLinear;
                case 5:
                    return All.LinearMipmapLinear;
                default:
                    return All.Linear;
            }
        }

        void SetTexFilters(byte MagFilter, byte MinFilter, int MipmapCount)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)GetTexFilter(MagFilter));

            if (MinFilter < 2)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)GetTexFilter(MinFilter));
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, MipmapCount - 1);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)GetTexFilter(MinFilter));
            }
        }

        void SetTexWrap(byte S, byte T)
        {
            switch (S)
            {
                case 0:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)All.ClampToEdge);
                    break;
                case 1:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)All.Repeat);
                    break;
                case 2:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)All.MirroredRepeat);
                    break;
            }
            switch (T)
            {
                case 0:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)All.ClampToEdge);
                    break;
                case 1:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)All.Repeat);
                    break;
                case 2:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)All.MirroredRepeat);
                    break;
            }
        }

        AlphaFunction GetAlphaCompMode(byte Mode)
        {
            switch (Mode)
            {
                case 0: //GX_NEVER
                    return AlphaFunction.Never;
                case 1: //GX_LESS
                    return AlphaFunction.Less;
                case 2: //GX_EQUAL
                    return AlphaFunction.Equal;
                case 3: //GX_LEQUAL
                    return AlphaFunction.Lequal;
                case 4: //GX_GREATER
                    return AlphaFunction.Greater;
                case 5: //GX_NEQUAL
                    return AlphaFunction.Notequal;
                case 6: //GX_GEQUAL
                    return AlphaFunction.Gequal;
                case 7: //GX_ALWAYS
                    return AlphaFunction.Always;
                default:
                    return AlphaFunction.Always;
            }
        }

        All GetBlendFactor(byte Mode)
        {
            switch (Mode)
            {
                case 0: //GX_BL_ZERO
                    return All.Zero;
                case 1: //GX_BL_ONE
                    return All.One;
                case 2: //GX_BL_SRCCLR / GX_BL_DSTCLR
                    return All.SrcColor;
                case 3: //GX_BL_INVSRCCLOR / GX_BL_INVDSTCLR
                    return All.OneMinusSrcColor;
                case 4: //GX_BL_SRCALPHA
                    return All.SrcAlpha;
                case 5: //GX_BL_INVSRCALPHA
                    return All.OneMinusSrcAlpha;
                case 6: //GX_DSTALPHA
                    return All.SrcAlpha;
                case 7: //GX_INVDSTALPHA
                    return All.OneMinusSrcAlpha;
                default:
                    return All.One;
            }
        }

        DepthFunction GetDepthFunc(byte Mode)
        {
            switch (Mode)
            {
                case 0: //GX_NEVER
                    return DepthFunction.Never;
                case 1: //GX_LESS
                    return DepthFunction.Less;
                case 2: //GX_EQUAL
                    return DepthFunction.Equal;
                case 3: //GX_LEQUAL
                    return DepthFunction.Lequal;
                case 4: //GX_GREATER
                    return DepthFunction.Greater;
                case 5: //GX_NEQUAL
                    return DepthFunction.Notequal;
                case 6: //GX_GEQUAL
                    return DepthFunction.Gequal;
                case 7: //GX_ALWAYS
                    return DepthFunction.Always;
                default:
                    return DepthFunction.Always;
            }
        }

        void SetModes(Material.MaterialEntry Mat)
        {
            /* Cull mode */
            int CullIndex = Mat.unk[1];
            switch (MaterialData.CullModes[CullIndex])
            {
                case 0: //GX_CULL_NONE
                    GL.Disable(EnableCap.CullFace);
                    break;

                case 1: //GX_CULL_FRONT
                    GL.Enable(EnableCap.CullFace);
                    GL.CullFace(CullFaceMode.Front);
                    break;

                case 2: //GX_CULL_BACK
                    GL.Enable(EnableCap.CullFace);
                    GL.CullFace(CullFaceMode.Back);
                    break;
            }

            /* Alpha compare */
            if ((MaterialData.AlphaComp[Mat.alphaCompIndex].Op != 0 && MaterialData.AlphaComp[Mat.alphaCompIndex].Op != 1) ||
                (((MaterialData.AlphaComp[Mat.alphaCompIndex].Comp0 != MaterialData.AlphaComp[Mat.alphaCompIndex].Comp1) || (MaterialData.AlphaComp[Mat.alphaCompIndex].Ref0 != MaterialData.AlphaComp[Mat.alphaCompIndex].Ref1)) &&
                (MaterialData.AlphaComp[Mat.alphaCompIndex].Comp1 != 3 || MaterialData.AlphaComp[Mat.alphaCompIndex].Ref1 != 255)))
            {
                GL.Disable(EnableCap.AlphaTest);
            }
            else
            {
                float AlphaRef = MaterialData.AlphaComp[Mat.alphaCompIndex].Ref0 / 255.0f;

                if (MaterialData.AlphaComp[Mat.alphaCompIndex].Comp0 != 7)
                {
                    GL.AlphaFunc(GetAlphaCompMode(MaterialData.AlphaComp[Mat.alphaCompIndex].Comp0), AlphaRef);
                    GL.Enable(EnableCap.AlphaTest);
                }
                else //GX_ALWAYS
                    GL.Disable(EnableCap.AlphaTest);
            }

            /* Blend mode */
            if (MaterialData.BlendInfo[Mat.blendIndex].Mode != 0 && MaterialData.BlendInfo[Mat.blendIndex].Mode != 1)
                GL.Disable(EnableCap.Blend);
            else
                switch (MaterialData.BlendInfo[Mat.blendIndex].Mode)
                {
                    case 0:
                    //GL.Disable(EnableCap.Blend);
                    //break;
                    case 1:
                        if (MaterialData.BlendInfo[Mat.blendIndex].SrcFactor == 1 && MaterialData.BlendInfo[Mat.blendIndex].DstFactor == 0)
                            GL.Disable(EnableCap.Blend);
                        else
                        {
                            GL.Enable(EnableCap.Blend);
                            GL.BlendFunc((BlendingFactorSrc)GetBlendFactor(MaterialData.BlendInfo[Mat.blendIndex].SrcFactor), (BlendingFactorDest)GetBlendFactor(MaterialData.BlendInfo[Mat.blendIndex].DstFactor));
                        }
                        break;
                }

            /* Z mode */
            if (MaterialData.ZMode[Mat.unk[6]].Enable != 0)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);

            GL.DepthFunc(GetDepthFunc(MaterialData.ZMode[Mat.unk[6]].Func));
            GL.DepthMask(Convert.ToBoolean(GetDepthFunc(MaterialData.ZMode[Mat.unk[6]].UpdateEnable)));
        }

        void BindMaterial(int MaterialIndex)
        {
            if (MaterialData == null) return;

            Material.MaterialEntry Mat = MaterialData.Entries[MaterialData.IndirectionTable[MaterialIndex]];

            SetModes(Mat);

            //int t = 0;
            for (int t = 0; t < 8; t++)
            {
                UInt16 Stage = Mat.texStages[t];

                if (Stage != 0xFFFF)
                {
                    UInt16 TextureID = MaterialData.TexTable[Stage];
                    GL.ActiveTexture(TextureUnit.Texture0 + t);
                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, Textures[TextureID].GLID);
                    SetTexFilters(Textures[TextureID].MagFilter, Textures[TextureID].MinFilter, Textures[TextureID].MipmapCount);
                    SetTexWrap(Textures[TextureID].WrapS, Textures[TextureID].WrapT);
                }
            }

            GL.ActiveTexture(TextureUnit.Texture0);
        }

        void AddScaleMatrix(ref Matrix4 Target, Matrix4 Source, float Scale)
        {
            Target.M11 += Scale * Source.M11;
            Target.M12 += Scale * Source.M12;
            Target.M13 += Scale * Source.M13;
            Target.M14 = 0.0f;

            Target.M21 += Scale * Source.M21;
            Target.M22 += Scale * Source.M22;
            Target.M23 += Scale * Source.M23;
            Target.M24 = 0.0f;

            Target.M31 += Scale * Source.M31;
            Target.M32 += Scale * Source.M32;
            Target.M33 += Scale * Source.M33;
            Target.M34 = 0.0f;

            Target.M41 += Scale * Source.M41;
            Target.M42 += Scale * Source.M42;
            Target.M43 += Scale * Source.M43;
            Target.M44 = 1.0f;
        }

        void UpdateMatrixTable(Shape.Packet Packet, ref Matrix4[] Matrices)
        {
            for (int i = 0; i < Packet.MatrixTable.Length; ++i)
            {
                if (Packet.MatrixTable[i] != 0xFFFF)
                {
                    UInt16 MatrixIndex = Packet.MatrixTable[i];
                    if (DrawData.IsWeighted[MatrixIndex] == true)
                    {
                        Matrices[i] = new Matrix4();
                        EVP1.IndicesWeights IW = EVP1Data.WeightedInd[DrawData.IndirectionData[MatrixIndex]];
                        for (int j = 0; j < IW.Weights.Length; ++j)
                        {
                            Matrix4 SM1 = EVP1Data.Matrices[IW.Indices[j]];
                            Matrix4 SM2 = Joints[IW.Indices[j]].Matrix;

                            AddScaleMatrix(ref Matrices[i], Matrix4.Mult(SM2, SM1), IW.Weights[j]);
                        }
                    }
                    else
                        Matrices[i] = Joints[DrawData.IndirectionData[MatrixIndex]].Matrix;
                }
            }
        }

        void AdjustMatrix(byte MatrixType, ref Matrix4 Matrix)
        {
            switch (MatrixType)
            {
                case 0:
                    /* No modification needed */
                    break;

                case 1:
                    /* Billboard */
                    break;

                case 2:
                    /* Y billboard */
                    break;

                case 3:
                    /* ? */
                    break;
            }
        }

        void DrawBatch(Shape.Batch Batch)
        {
            GL.FrontFace(FrontFaceDirection.Cw);

            Matrix4[] Matrices = new Matrix4[10];

            foreach (Shape.Packet Packet in Batch.Packets)
            {
                UpdateMatrixTable(Packet, ref Matrices);

                Matrix4 CurrentMatrix = Matrices[0];
                AdjustMatrix(Batch.MatrixType, ref CurrentMatrix);

                foreach (Shape.Primitive Prim in Packet.Primitives)
                {
                    switch (Prim.Type)
                    {
                        case (byte)PrimitiveTypes.TRIANGLESTRIP:
                            GL.Begin(BeginMode.TriangleStrip);
                            break;
                        case (byte)PrimitiveTypes.TRIANGLEFAN:
                            GL.Begin(BeginMode.TriangleFan);
                            break;
                        default:
                            continue;
                    }

                    GL.Color3(1.0f, 1.0f, 1.0f);
                    foreach (Shape.Primitive.Indexing Ind in Prim.Indices)
                    {
                        if (Ind.MatrixIndex != -1)
                        {
                            CurrentMatrix = Matrices[Ind.MatrixIndex / 3];
                            AdjustMatrix(Batch.MatrixType, ref CurrentMatrix);
                        }

                        if (Ind.NormalsIndex >= 0 && Ind.NormalsIndex < VtxArrays.Normals.Length)
                            GL.Normal3(VtxArrays.Normals[Ind.NormalsIndex]);
                        if (Ind.ColorIndex[0] >= 0 && Ind.ColorIndex[0] < VtxArrays.Colors[0].Length)
                            GL.Color4(VtxArrays.Colors[0][Ind.ColorIndex[0]]);

                        for (int t = 0; t < 8; t++)
                            if (Ind.TexCoordIndex[t] >= 0 && Ind.TexCoordIndex[t] < VtxArrays.TexCoords[t].Length)
                                GL.MultiTexCoord2(TextureUnit.Texture0 + t, ref VtxArrays.TexCoords[t][Ind.TexCoordIndex[t]]);

                        if (Ind.PositionIndex >= 0 && Ind.PositionIndex < VtxArrays.Positions.Length)
                            GL.Vertex3(Vector3.Transform(VtxArrays.Positions[Ind.PositionIndex], CurrentMatrix));
                    }

                    GL.End();
                }

                if (ShowBoundingBoxes == true)
                {
                    Helpers.DrawBox(Vector3.Transform(Batch.BoundingMin, Matrices[0]), Vector3.Transform(Batch.BoundingMax, Matrices[0]), System.Drawing.Color.Red);
                    Helpers.PrintText(Batch.BoundingMin, System.Drawing.Color.Orange, SystemFonts.DefaultFont, "Minimum: " + Batch.BoundingMin.ToString());
                    Helpers.PrintText(Batch.BoundingMax, System.Drawing.Color.Orange, SystemFonts.DefaultFont, "Maximum: " + Batch.BoundingMax.ToString());
                }
            }
        }

        public void Render()
        {
            if (Scene == null) return;      /* Nothing to render... */

            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(5.0f, 5.0f);
            GL.CallList(SceneGLID);
            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.PopAttrib();
        }

        #endregion

        #region Classes

        #region File Header & Chunks

        class FileHeader
        {
            public string Tag;
            public string Type;
            public UInt32 Size;
            public UInt32 Chunks;

            public FileHeader(byte[] Data)
            {
                Tag = Helpers.ReadString(Data, 0, 4);
                Type = Helpers.ReadString(Data, 4, 4);
                Size = Helpers.Read32(Data, 8);
                Chunks = Helpers.Read32(Data, 12);
            }

            public override string ToString()
            {
                return String.Format("Tag: {0}, Type: {1}, Size: {2:X8}, Chunks: {3}", Tag, Type, Size, Chunks);
            }
        }

        class FileChunk
        {
            public string Tag;
            public UInt32 Size;
            public byte[] Data;

            public FileChunk(byte[] SrcData, ref UInt32 SrcOffset)
            {
                Tag = Helpers.ReadString(SrcData, (int)SrcOffset, 4);
                Size = Helpers.Read32(SrcData, (int)SrcOffset + 4);

                try
                {
                    Data = new byte[Size];
                    Buffer.BlockCopy(SrcData, (int)SrcOffset, Data, 0, (int)Math.Min(Size, SrcData.Length - SrcOffset));
                    SrcOffset += Size;
                }
                catch (OverflowException)
                {
                    Data = null;
                    SrcOffset += 16;
                }
            }

            public override string ToString()
            {
                return String.Format("Tag: {0}, Size: {1:X8}, Data[8]: {2:X8}", Tag, Size, Helpers.Read32(Data, 8));
            }
        }

        #endregion

        #region Scene Graph

        class SceneGraphRaw
        {
            public UInt16 Type;
            public UInt16 Index;

            public SceneGraphRaw(byte[] Data, ref UInt32 Offset)
            {
                Type = Helpers.Read16(Data, (int)Offset);
                Index = Helpers.Read16(Data, (int)Offset + 2);
                Offset += 4;
            }

            public override string ToString()
            {
                return String.Format("Type: {0}, Index: {1}", Enum.GetName(typeof(SceneGraphTypes), Type), Index);
            }
        }

        class SceneGraph
        {
            public UInt16 Type = 0;
            public UInt16 Index = 0;
            public Matrix4 RelativeMatrix = Matrix4.Identity;

            public List<SceneGraph> Children = null;

            public SceneGraph() { }

            public SceneGraph(UInt16 _Type, UInt16 _Index, Matrix4 _RelativeMatrix)
            {
                Type = _Type;
                Index = _Index;
                RelativeMatrix = _RelativeMatrix;
            }

            public override string ToString()
            {
                return String.Format("Type: {0}, Index: {1}", Enum.GetName(typeof(SceneGraphTypes), Type), Index);
            }
        }

        #endregion

        #region Draw & EVP1

        class DrawChunk
        {
            UInt16 EntryCount;
            UInt32 IsWeightedOffset, IndirectionDataOffset;

            public bool[] IsWeighted;
            public UInt16[] IndirectionData;

            public DrawChunk(byte[] Data)
            {
                EntryCount = Helpers.Read16(Data, 8);
                IsWeightedOffset = Helpers.Read32(Data, 12);
                IndirectionDataOffset = Helpers.Read32(Data, 16);

                IsWeighted = new bool[EntryCount];
                for (int i = 0; i < EntryCount; i++)
                    IsWeighted[i] = Convert.ToBoolean(Helpers.Read8(Data, (int)IsWeightedOffset + i));

                IndirectionData = new UInt16[EntryCount];
                for (int i = 0; i < EntryCount; i++)
                    IndirectionData[i] = Helpers.Read16(Data, (int)IndirectionDataOffset + (i * 2));
            }
        }

        class EVP1
        {
            UInt16 EntryCount;
            UInt32 CountsArrayOffset, IndicesOffset, WeightsOffset, MatrixDataOffset;

            public byte[] CountsArray;
            public IndicesWeights[] WeightedInd;
            public Matrix4[] Matrices;

            public EVP1(byte[] Data)
            {
                EntryCount = Helpers.Read16(Data, 8);

                CountsArrayOffset = Helpers.Read32(Data, 12);
                IndicesOffset = Helpers.Read32(Data, 16);
                WeightsOffset = Helpers.Read32(Data, 20);
                MatrixDataOffset = Helpers.Read32(Data, 24);

                CountsArray = new byte[EntryCount];
                for (int i = 0; i < EntryCount; i++)
                    CountsArray[i] = Helpers.Read8(Data, (int)CountsArrayOffset + i);

                UInt32 ReadOffsetIndices = IndicesOffset;
                UInt32 ReadOffsetWeights = WeightsOffset;

                WeightedInd = new IndicesWeights[EntryCount];
                int NumMatrices = 0;
                for (int i = 0; i < EntryCount; i++)
                {
                    WeightedInd[i] = new IndicesWeights();
                    WeightedInd[i].Indices = new UInt16[CountsArray[i]];
                    WeightedInd[i].Weights = new float[CountsArray[i]];
                    for (int j = 0; j < CountsArray[i]; j++)
                    {
                        WeightedInd[i].Indices[j] = Helpers.Read16(Data, (int)ReadOffsetIndices);
                        ReadOffsetIndices += 2;

                        NumMatrices = Math.Max(NumMatrices, WeightedInd[i].Indices[j] + 1);

                        WeightedInd[i].Weights[j] = (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetWeights)), 8);
                        ReadOffsetWeights += 4;
                    }
                }

                UInt32 ReadOffsetMatrixData = MatrixDataOffset;

                Matrices = new Matrix4[NumMatrices];
                for (int i = 0; i < NumMatrices; i++)
                {
                    Matrices[i] = new Matrix4(
                        new Vector4(
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData)), 8),
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 16)), 8),
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 32)), 8),
                            0.0f),

                        new Vector4(
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 4)), 8),
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 20)), 8),
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 36)), 8),
                            0.0f),

                        new Vector4(
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 8)), 8),
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 24)), 8),
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 40)), 8),
                            0.0f),

                        new Vector4(
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 12)), 8),
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 28)), 8),
                            (float)Math.Round(Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ReadOffsetMatrixData + 44)), 8),
                            1.0f));

                    ReadOffsetMatrixData += 48;
                }
            }

            public class IndicesWeights
            {
                public UInt16[] Indices;
                public float[] Weights;
            }
        }

        #endregion

        #region Joints, Shapes, etc.

        public class Joint
        {
            public const int Size = 64;

            public Vector3 Scale, Rotation, Translation, BoundingMin, BoundingMax;
            public Matrix4 Matrix = Matrix4.Identity;

            public string Name = string.Empty;

            public Joint(byte[] Data, ref UInt32 Offset, int StringOffset)
                : this(Data, ref Offset)
            {
                Name = Helpers.ReadString(Data, StringOffset);
            }

            public Joint(byte[] Data, ref UInt32 Offset)
            {
                Scale = new Vector3(
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 4)),
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 8)),
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 12)));
                Rotation = new Vector3(
                    (float)(Math.Round((Int16)Helpers.Read16(Data, (int)Offset + 16) / 182.0444444)),
                    (float)(Math.Round((Int16)Helpers.Read16(Data, (int)Offset + 18) / 182.0444444)),
                    (float)(Math.Round((Int16)Helpers.Read16(Data, (int)Offset + 20) / 182.0444444)));
                Translation = new Vector3(
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 24)),
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 28)),
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 32)));
                BoundingMin = new Vector3(
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 40)),
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 44)),
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 48)));
                BoundingMax = new Vector3(
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 52)),
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 56)),
                    Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 60)));

                Offset += Size;
            }
        }

        class Shape
        {
            UInt32 BatchOffset, BatchAttribsOffset, MatrixTableOffset, PrimitivesOffset, MatrixDataOffset, PacketLocationsOffset;

            public List<Batch> Batches = new List<Batch>();

            public Shape(byte[] Data)
            {
#if DEBUG == false
                try
#endif
                {
                    BatchOffset = Helpers.Read32(Data, 12);
                    BatchAttribsOffset = Helpers.Read32(Data, 24);
                    MatrixTableOffset = Helpers.Read32(Data, 28);
                    PrimitivesOffset = Helpers.Read32(Data, 32);
                    MatrixDataOffset = Helpers.Read32(Data, 36);
                    PacketLocationsOffset = Helpers.Read32(Data, 40);

                    UInt32 DataOffset = 0;

                    /* Batches */
                    int BatchCount = Helpers.Read16(Data, 8);
                    DataOffset = BatchOffset;
                    for (int i = 0; i < BatchCount; i++)
                        Batches.Add(new Batch(Data, ref DataOffset));

                    /* Batch attribs */
                    for (int i = 0; i < BatchCount; i++)
                    {
                        Batches[i].BatchAttribs = new List<BatchAttrib>();
                        DataOffset = BatchAttribsOffset + Batches[i].AttribOffset;
                        for (int k = 0; k < Data.Length; k++)
                        {
                            BatchAttrib NewEntry = new BatchAttrib(Data, ref DataOffset);
                            if (NewEntry.ArrayType == 0xFF)
                                break;
                            else
                                Batches[i].BatchAttribs.Add(NewEntry);
                        }
                    }

                    /* Packets */
                    for (int i = 0; i < BatchCount; i++)
                    {
                        Batches[i].Packets = new List<Packet>();
                        for (int j = 0; j < Batches[i].PacketCount; j++)
                        {
                            Packet NewPacket = new Packet();

                            /* Read primitives */
                            PacketLocation PacketLoc = new PacketLocation(Data, PacketLocationsOffset + (uint)((Batches[i].FirstPacketIndex + j) * PacketLocation.Size));
                            UInt32 PacketReadOffset = PrimitivesOffset + PacketLoc.PacketOffset;

                            bool Done = false;
                            UInt32 ReadBoundary = PacketReadOffset + PacketLoc.PacketSize;
                            while (Done == false)
                            {
                                Primitive NewEntry = new Primitive();
                                NewEntry.Type = Helpers.Read8(Data, (int)PacketReadOffset++);

                                if (NewEntry.Type == 0 || PacketReadOffset >= ReadBoundary)
                                {
                                    Done = true;
                                    continue;
                                }

                                UInt16 Count = Helpers.Read16(Data, (int)PacketReadOffset);
                                PacketReadOffset += 2;

                                NewEntry.Indices = new Primitive.Indexing[Count];

                                for (int k = 0; k < Count; ++k)
                                {
                                    NewEntry.Indices[k] = new Primitive.Indexing();
                                    for (int l = 0; l < Batches[i].BatchAttribs.Count; ++l)
                                    {
                                        Int16 Value = 0;
                                        switch (Batches[i].BatchAttribs[l].DataType)
                                        {
                                            case (UInt32)DataTypes.S8:
                                                Value = Helpers.Read8(Data, (int)PacketReadOffset++);
                                                break;
                                            case (UInt32)DataTypes.S16:
                                                Value = (Int16)Helpers.Read16(Data, (int)PacketReadOffset);
                                                PacketReadOffset += 2;
                                                break;
                                            default:
                                                throw new Exception("Invalid data type in packet primitives");
                                        }

                                        switch (Batches[i].BatchAttribs[l].ArrayType)
                                        {
                                            case (UInt32)ArrayTypes.POSITION_MATRIX_INDEX:
                                                NewEntry.Indices[k].MatrixIndex = Value;
                                                break;
                                            case (UInt32)ArrayTypes.POSITION:
                                                NewEntry.Indices[k].PositionIndex = Value;
                                                break;
                                            case (UInt32)ArrayTypes.NORMAL:
                                                NewEntry.Indices[k].NormalsIndex = Value;
                                                break;
                                            case (UInt32)ArrayTypes.COLOR0:
                                            case (UInt32)ArrayTypes.COLOR1:
                                                NewEntry.Indices[k].ColorIndex[Batches[i].BatchAttribs[l].ArrayType - (UInt32)ArrayTypes.COLOR0] = Value;
                                                break;
                                            case (UInt32)ArrayTypes.TEX0:
                                            case (UInt32)ArrayTypes.TEX1:
                                            case (UInt32)ArrayTypes.TEX2:
                                            case (UInt32)ArrayTypes.TEX3:
                                            case (UInt32)ArrayTypes.TEX4:
                                            case (UInt32)ArrayTypes.TEX5:
                                            case (UInt32)ArrayTypes.TEX6:
                                            case (UInt32)ArrayTypes.TEX7:
                                                NewEntry.Indices[k].TexCoordIndex[Batches[i].BatchAttribs[l].ArrayType - (UInt32)ArrayTypes.TEX0] = Value;
                                                break;
                                        }
                                    }
                                }

                                NewPacket.Primitives.Add(NewEntry);
                            }

                            /* Read matrix data */
                            PacketReadOffset = MatrixDataOffset + (uint)((Batches[i].FirstMatrixIndex + j) * PacketMatrixData.Size);
                            PacketMatrixData PMD = new PacketMatrixData(Data, PacketReadOffset);

                            /* Read packet matrix table */
                            NewPacket.MatrixTable = new ushort[PMD.Count];

                            PacketReadOffset = (UInt32)(MatrixTableOffset + (PMD.FirstIndex * 2));
                            for (int k = 0; k < NewPacket.MatrixTable.Length; ++k)
                            {
                                NewPacket.MatrixTable[k] = Helpers.Read16(Data, (int)PacketReadOffset);
                                PacketReadOffset += 2;
                            }

                            Batches[i].Packets.Add(NewPacket);
                        }
                    }
                }
#if DEBUG == false
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Error while loading SHP1 chunk!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    Batches.Clear();
                }
#endif
            }

            public class Batch
            {
                public const int Size = 40;

                public List<BatchAttrib> BatchAttribs;
                public List<Packet> Packets;

                public byte MatrixType;
                public UInt16 PacketCount;
                public UInt16 AttribOffset;
                public UInt16 FirstMatrixIndex;
                public UInt16 FirstPacketIndex;
                public Vector3 BoundingMin, BoundingMax;

                public Batch(byte[] Data, ref UInt32 Offset)
                {
                    MatrixType = Helpers.Read8(Data, (int)Offset);
                    PacketCount = Helpers.Read16(Data, (int)Offset + 2);
                    AttribOffset = Helpers.Read16(Data, (int)Offset + 4);
                    FirstMatrixIndex = Helpers.Read16(Data, (int)Offset + 6);
                    FirstPacketIndex = Helpers.Read16(Data, (int)Offset + 8);

                    BoundingMin = new Vector3(
                        Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 16)),
                        Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 20)),
                        Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 24)));
                    BoundingMax = new Vector3(
                        Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 28)),
                        Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 32)),
                        Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)Offset + 36)));

                    Offset += Size;
                }
            }

            public class Packet
            {
                public List<Primitive> Primitives = new List<Primitive>();
                public UInt16[] MatrixTable;
            }

            public class BatchAttrib
            {
                public const int Size = 8;

                public UInt32 ArrayType;
                public UInt32 DataType;

                public BatchAttrib(byte[] Data, ref UInt32 Offset)
                {
                    ArrayType = Helpers.Read32(Data, (int)Offset);
                    DataType = Helpers.Read32(Data, (int)Offset + 4);

                    Offset += Size;
                }
            }

            public class PacketLocation
            {
                public const int Size = 8;

                public UInt32 PacketSize;
                public UInt32 PacketOffset;

                public PacketLocation(byte[] Data, UInt32 Offset)
                {
                    PacketSize = Helpers.Read32(Data, (int)Offset);
                    PacketOffset = Helpers.Read32(Data, (int)Offset + 4);

                    Offset += Size;
                }
            }

            public class PacketMatrixData
            {
                public const int Size = 8;

                public UInt16 Unknown;
                public UInt16 Count;
                public UInt32 FirstIndex;

                public PacketMatrixData(byte[] Data, UInt32 Offset)
                {
                    Unknown = Helpers.Read16(Data, (int)Offset);
                    Count = Helpers.Read16(Data, (int)Offset + 2);
                    FirstIndex = Helpers.Read32(Data, (int)Offset + 4);

                    Offset += Size;
                }
            }

            public class Primitive
            {
                public byte Type;
                public Indexing[] Indices;

                public Primitive() { }

                public class Indexing
                {
                    public Int16 MatrixIndex = -1;
                    public Int16 PositionIndex = -1;
                    public Int16 NormalsIndex = -1;
                    public Int16[] ColorIndex = new Int16[] { -1, -1 };
                    public Int16[] TexCoordIndex = new Int16[] { -1, -1, -1, -1, -1, -1, -1, -1 };
                }
            }
        }

        #endregion

        #region Vertex Arrays & Formats

        class VertexArrays
        {
            public Vector3[] Positions;
            public Color4[][] Colors;
            public Vector3[] Normals;
            public Vector2[][] TexCoords;

            public VertexArrays()
            {
                Colors = new Color4[2][];
                TexCoords = new Vector2[8][];
            }
        }

        class VertexFormat
        {
            public const int Size = 16;

            public UInt32 Offset = 0;

            public UInt32 ArrayType;
            public UInt32 Count;
            public UInt32 DataType;
            public byte DecimalPoint;

            public VertexFormat(byte[] Data, ref UInt32 Offset)
            {
                ArrayType = Helpers.Read32(Data, (int)Offset);
                Count = Helpers.Read32(Data, (int)Offset + 4);
                DataType = Helpers.Read32(Data, (int)Offset + 8);
                DecimalPoint = Helpers.Read8(Data, (int)Offset + 12);

                Offset += Size;
            }

            public override string ToString()
            {
                return String.Format("Offset: {0:X8}, ArrayType: {1}, Count: {2}, DataType: {3}, DecimalPoint: {4}", Offset, Enum.GetName(typeof(ArrayTypes), ArrayType), Count, Enum.GetName(typeof(DataTypes), DataType), DecimalPoint);
            }
        }

        #endregion

        #region Textures & Materials

        class Texture
        {
            public const int Size = 32;

            public int GLID;

            public byte Format;
            public UInt16[] Width, Height;
            public byte WrapS, WrapT;
            public byte PalFormat;
            public UInt16 PalEntryCount;
            public UInt32 PaletteOffset;
            public byte MinFilter, MagFilter;
            public byte MipmapCount;
            public UInt32 TextureOffset;

            public string Name = string.Empty;
            public byte[] Palette = null;

            public Texture(byte[] Data, ref UInt32 Offset, int StringOffset)
                : this(Data, ref Offset)
            {
                Name = Helpers.ReadString(Data, StringOffset);
            }

            public Texture(byte[] Data, ref UInt32 Offset)
            {
                GLID = GL.GenTexture();

                Format = Helpers.Read8(Data, (int)Offset);
                WrapS = Helpers.Read8(Data, (int)Offset + 6);
                WrapT = Helpers.Read8(Data, (int)Offset + 7);
                PalFormat = Helpers.Read8(Data, (int)Offset + 9);
                PalEntryCount = Helpers.Read16(Data, (int)Offset + 10);
                PaletteOffset = Helpers.Read32(Data, (int)Offset + 12);

                MinFilter = Helpers.Read8(Data, (int)Offset + 20);
                MagFilter = Helpers.Read8(Data, (int)Offset + 21);
                MipmapCount = Helpers.Read8(Data, (int)Offset + 24);
                TextureOffset = Helpers.Read32(Data, (int)Offset + 28);

                Width = new ushort[MipmapCount]; Height = new ushort[MipmapCount];
                Width[0] = Helpers.Read16(Data, (int)Offset + 2);
                Height[0] = Helpers.Read16(Data, (int)Offset + 4);
                for (int i = 1; i < MipmapCount; i++)
                {
                    Width[i] = (UInt16)(Width[i - 1] / 2);
                    Height[i] = (UInt16)(Height[i - 1] / 2);
                }

                if (PalEntryCount != 0)
                {
                    Palette = new byte[PalEntryCount * 2];
                    for (int i = 0; i < Palette.Length; i += 2)
                    {
                        Palette[i] = Helpers.Read8(Data, (int)(Offset + PaletteOffset + i));
                        Palette[i + 1] = Helpers.Read8(Data, (int)(Offset + PaletteOffset + i + 1));
                    }
                }

                Offset += Size;
            }
        }

        class Material
        {
            public UInt32 Size;

            public int MaterialCount;
            public UInt16[] IndirectionTable;
            public MaterialEntry[] Entries;

            public UInt32[] Offsets, Lengths;

            public UInt32[] CullModes;
            public UInt16[] TexTable;
            public AlphaCompareEntry[] AlphaComp;
            public BlendInfoEntry[] BlendInfo;
            public ZModeEntry[] ZMode;

            public string Name = string.Empty;

            public Material(byte[] Data, int StringOffset)
                : this(Data)
            {
                Name = Helpers.ReadString(Data, StringOffset);
            }

            public Material(byte[] Data)
            {
                Size = Helpers.Read32(Data, 4);

                MaterialCount = Helpers.Read16(Data, 8);
                IndirectionTable = new UInt16[MaterialCount];

                Offsets = new UInt32[30];
                Lengths = new UInt32[30];
                for (int i = 0; i < Offsets.Length; i++)
                    Offsets[i] = Helpers.Read32(Data, 12 + (i * 4));

                for (int i = 0; i < Offsets.Length; i++)
                {
                    UInt32 Length = 0;
                    if (Offsets[i] != 0)
                    {
                        UInt32 Next = Size;
                        for (int j = i + 1; j < Offsets.Length; ++j)
                        {
                            if (Offsets[i] != 0)
                            {
                                Next = Offsets[j];
                                break;
                            }
                        }
                        Length = Next - Offsets[i];
                    }
                    Lengths[i] = Length;
                }

                CullModes = new UInt32[Lengths[4] / 4];
                TexTable = new UInt16[Lengths[15] / 2];
                AlphaComp = new AlphaCompareEntry[Lengths[24] / 8];
                BlendInfo = new BlendInfoEntry[Lengths[25] / 4];
                ZMode = new ZModeEntry[Lengths[26] / 4];

                UInt16 MaxIndex = 0;
                for (int i = 0; i < MaterialCount; i++)
                {
                    IndirectionTable[i] = Helpers.Read16(Data, (int)(Offsets[1] + (i * 2)));
                    MaxIndex = Math.Max(MaxIndex, IndirectionTable[i]);
                }

                for (int i = 0; i < CullModes.Length; i++)
                    CullModes[i] = Helpers.Read32(Data, (int)(Offsets[4] + (i * 4)));
                for (int i = 0; i < TexTable.Length; i++)
                    TexTable[i] = Helpers.Read16(Data, (int)(Offsets[15] + (i * 2)));
                for (int i = 0; i < AlphaComp.Length; i++)
                    AlphaComp[i] = new AlphaCompareEntry(Data, (uint)(Offsets[24] + (i * 8)));
                for (int i = 0; i < BlendInfo.Length; i++)
                    BlendInfo[i] = new BlendInfoEntry(Data, (uint)(Offsets[25] + (i * 4)));
                for (int i = 0; i < ZMode.Length; i++)
                    ZMode[i] = new ZModeEntry(Data, (uint)(Offsets[26] + (i * 4)));

                Entries = new MaterialEntry[MaxIndex + 1];

                UInt32 EntryOffset = Offsets[0];
                for (int i = 0; i <= MaxIndex; ++i)
                    Entries[i] = new MaterialEntry(Data, ref EntryOffset);
            }

            public class MaterialEntry
            {
                public const int Size = 0x14C;

                public byte[] unk = new byte[8];
                public UInt16[] color1 = new UInt16[2];
                public UInt16[] chanControls = new UInt16[4];
                public UInt16[] color2 = new UInt16[2];
                public UInt16[] lights = new UInt16[8];
                public UInt16[] texGenInfos = new UInt16[8];
                public UInt16[] texGenInfos2 = new UInt16[8];
                public UInt16[] texMtxInfos = new UInt16[10];
                public UInt16[] dttMtxInfos = new UInt16[20];
                public UInt16[] texStages = new UInt16[8];
                public UInt16[] color3 = new UInt16[4];
                public byte[] constColorSel = new byte[16];
                public byte[] constAlphaSel = new byte[16];
                public UInt16[] tevOrderInfo = new UInt16[16];
                public UInt16[] colorS10 = new UInt16[4];
                public UInt16[] tevStageInfo = new UInt16[16];
                public UInt16[] tevSwapModeInfo = new UInt16[16];
                public UInt16[] tevSwapModeTable = new UInt16[4];
                public UInt16[] unknown6 = new UInt16[12];
                public UInt16 index1;
                public UInt16 alphaCompIndex;
                public UInt16 blendIndex;
                public UInt16 index2;

                public MaterialEntry(byte[] Data, ref UInt32 Offset)
                {
                    for (int i = 0; i < 8; ++i) unk[i] = Helpers.Read8(Data, (int)Offset + i);
                    for (int i = 0; i < 2; ++i) color1[i] = Helpers.Read16(Data, (int)Offset + 0x8 + (i * 2));
                    for (int i = 0; i < 4; ++i) chanControls[i] = Helpers.Read16(Data, (int)Offset + 0xC + (i * 2));
                    for (int i = 0; i < 2; ++i) color2[i] = Helpers.Read16(Data, (int)Offset + 0x14 + (i * 2));
                    for (int i = 0; i < 8; ++i) lights[i] = Helpers.Read16(Data, (int)Offset + 0x18 + (i * 2));
                    for (int i = 0; i < 8; ++i) texGenInfos[i] = Helpers.Read16(Data, (int)Offset + 0x28 + (i * 2));
                    for (int i = 0; i < 8; ++i) texGenInfos2[i] = Helpers.Read16(Data, (int)Offset + 0x38 + (i * 2));
                    for (int i = 0; i < 10; ++i) texMtxInfos[i] = Helpers.Read16(Data, (int)Offset + 0x48 + (i * 2));
                    for (int i = 0; i < 20; ++i) dttMtxInfos[i] = Helpers.Read16(Data, (int)Offset + 0x5C + (i * 2));
                    for (int i = 0; i < 8; ++i) texStages[i] = Helpers.Read16(Data, (int)Offset + 0x84 + (i * 2));
                    for (int i = 0; i < 4; ++i) color3[i] = Helpers.Read16(Data, (int)Offset + 0x94 + (i * 2));
                    for (int i = 0; i < 16; ++i) constColorSel[i] = Helpers.Read8(Data, (int)Offset + 0x9C + i);
                    for (int i = 0; i < 16; ++i) constAlphaSel[i] = Helpers.Read8(Data, (int)Offset + 0xAC + i);
                    for (int i = 0; i < 16; ++i) tevOrderInfo[i] = Helpers.Read16(Data, (int)Offset + 0xBC + (i * 2));
                    for (int i = 0; i < 4; ++i) colorS10[i] = Helpers.Read16(Data, (int)Offset + 0xDC + (i * 2));
                    for (int i = 0; i < 16; ++i) tevStageInfo[i] = Helpers.Read16(Data, (int)Offset + 0xE4 + (i * 2));
                    for (int i = 0; i < 16; ++i) tevSwapModeInfo[i] = Helpers.Read16(Data, (int)Offset + 0x104 + (i * 2));
                    for (int i = 0; i < 4; ++i) tevSwapModeTable[i] = Helpers.Read16(Data, (int)Offset + 0x124 + (i * 2));
                    for (int i = 0; i < 12; ++i) unknown6[i] = Helpers.Read16(Data, (int)Offset + 0x12C + (i * 2));
                    index1 = Helpers.Read16(Data, (int)Offset + 0x144);
                    alphaCompIndex = Helpers.Read16(Data, (int)Offset + 0x146);
                    blendIndex = Helpers.Read16(Data, (int)Offset + 0x148);
                    index2 = Helpers.Read16(Data, (int)Offset + 0x14A);

                    Offset += Size;
                }
            }

            public class AlphaCompareEntry
            {
                public byte Comp0, Ref0, Op, Comp1, Ref1;

                public AlphaCompareEntry(byte[] Data, UInt32 Offset)
                {
                    Comp0 = Helpers.Read8(Data, (int)Offset);
                    Ref0 = Helpers.Read8(Data, (int)Offset + 1);
                    Op = Helpers.Read8(Data, (int)Offset + 2);
                    Comp1 = Helpers.Read8(Data, (int)Offset + 3);
                    Ref1 = Helpers.Read8(Data, (int)Offset + 4);
                }
            }

            public class BlendInfoEntry
            {
                public byte Mode, SrcFactor, DstFactor, LogicOp;

                public BlendInfoEntry(byte[] Data, UInt32 Offset)
                {
                    Mode = Helpers.Read8(Data, (int)Offset);
                    SrcFactor = Helpers.Read8(Data, (int)Offset + 1);
                    DstFactor = Helpers.Read8(Data, (int)Offset + 2);
                    LogicOp = Helpers.Read8(Data, (int)Offset + 3);
                }
            }

            public class ZModeEntry
            {
                public byte Enable, Func, UpdateEnable;

                public ZModeEntry(byte[] Data, UInt32 Offset)
                {
                    Enable = Helpers.Read8(Data, (int)Offset);
                    Func = Helpers.Read8(Data, (int)Offset + 1);
                    UpdateEnable = Helpers.Read8(Data, (int)Offset + 2);
                }
            }
        }

        #endregion

        #region Animation

        class ANK1
        {
            byte Flags;
            byte AngleMultiplier;
            UInt16 AnimationLength;
            UInt16 JointCount, ScaleCount, RotationCount, TranslationCount;
            UInt32 JointOffset, ScaleOffset, RotationOffset, TranslationOffset;

            float[] ScaleValues, TranslationValues;
            Int16[] RotationValues;
            Animation[] Animations;

            public ANK1(byte[] Data)
            {
                Flags = Helpers.Read8(Data, 8);
                AngleMultiplier = Helpers.Read8(Data, 9);
                AnimationLength = Helpers.Read16(Data, 10);
                JointCount = Helpers.Read16(Data, 12);
                ScaleCount = Helpers.Read16(Data, 14);
                RotationCount = Helpers.Read16(Data, 16);
                TranslationCount = Helpers.Read16(Data, 18);
                JointOffset = Helpers.Read32(Data, 20);
                ScaleOffset = Helpers.Read32(Data, 24);
                RotationOffset = Helpers.Read32(Data, 28);
                TranslationOffset = Helpers.Read32(Data, 32);

                ScaleValues = new float[ScaleCount];
                for (int i = 0; i < ScaleCount; i++)
                    ScaleValues[i] = Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)ScaleOffset + (i * 4)));

                RotationValues = new Int16[RotationCount];
                for (int i = 0; i < RotationCount; i++)
                    RotationValues[i] = (Int16)Helpers.Read16(Data, (int)RotationOffset + (i * 2));

                TranslationValues = new float[TranslationCount];
                for (int i = 0; i < TranslationCount; i++)
                    TranslationValues[i] = Helpers.ConvertIEEE754Float(Helpers.Read32(Data, (int)TranslationOffset + (i * 4)));

                Animations = new Animation[JointCount];
                float RotScale = (float)(Math.Pow(2.0f, AngleMultiplier) * 180 / 32768.0f);
                UInt32 ReadOffset = JointOffset;
                for (int i = 0; i < JointCount; i++)
                {
                    Animations[i] = new Animation();

                    AnimJoint AJ = new AnimJoint(Data, ref ReadOffset);
                    ReadComponent<float>(ref Animations[i].ScalesX, ScaleValues, AJ.X.Scale);
                    ReadComponent<float>(ref Animations[i].ScalesY, ScaleValues, AJ.Y.Scale);
                    ReadComponent<float>(ref Animations[i].ScalesZ, ScaleValues, AJ.Z.Scale);

                    ReadComponent<Int16>(ref Animations[i].RotationsX, RotationValues, AJ.X.Rot, RotScale);
                    ReadComponent<Int16>(ref Animations[i].RotationsY, RotationValues, AJ.Y.Rot, RotScale);
                    ReadComponent<Int16>(ref Animations[i].RotationsZ, RotationValues, AJ.Z.Rot, RotScale);

                    ReadComponent<float>(ref Animations[i].TranslationsX, TranslationValues, AJ.X.Trans);
                    ReadComponent<float>(ref Animations[i].TranslationsY, TranslationValues, AJ.Y.Trans);
                    ReadComponent<float>(ref Animations[i].TranslationsZ, TranslationValues, AJ.Z.Trans);
                }
            }

            public void Animate(List<Joint> Joints, float FTime)
            {
                if (Joints == null || Animations.Length != Joints.Count) return;

                FTime = FTime % AnimationLength;

                for (int i = 0; i < Joints.Count; i++)
                {
                    Joints[i].Scale.X = GetAnimValue(Animations[i].ScalesX, FTime);
                    Joints[i].Scale.Y = GetAnimValue(Animations[i].ScalesY, FTime);
                    Joints[i].Scale.Z = GetAnimValue(Animations[i].ScalesZ, FTime);

                    Joints[i].Rotation.X = GetAnimValue(Animations[i].RotationsX, FTime);
                    Joints[i].Rotation.Y = GetAnimValue(Animations[i].RotationsY, FTime);
                    Joints[i].Rotation.Z = GetAnimValue(Animations[i].RotationsZ, FTime);

                    Joints[i].Translation.X = GetAnimValue(Animations[i].TranslationsX, FTime);
                    Joints[i].Translation.Y = GetAnimValue(Animations[i].TranslationsY, FTime);
                    Joints[i].Translation.Z = GetAnimValue(Animations[i].TranslationsZ, FTime);
                }
            }

            void ReadComponent<T>(ref AnimKey[] Dest, T[] Src, AnimIndexing Indexing, float Scale = 1.0f)
            {
                Dest = new AnimKey[Indexing.Count];

                if (Indexing.Count == 1)
                {
                    Dest[0] = new AnimKey();
                    Dest[0].Time = 0;
                    Dest[0].Value = Convert.ToSingle((object)Src[Indexing.Index]) * Scale;
                    Dest[0].Tangent = 0;
                }
                else
                {
                    for (int i = 0; i < Indexing.Count; ++i)
                    {
                        Dest[i] = new AnimKey();
                        Dest[i].Time = Convert.ToSingle((object)Src[Indexing.Index + 3 * i]);
                        Dest[i].Value = Convert.ToSingle((object)Src[Indexing.Index + 3 * i + 1]) * Scale;
                        Dest[i].Tangent = Convert.ToSingle((object)Src[Indexing.Index + 3 * i + 2]) * Scale;
                    }
                }
            }

            float Interpolate(float V1, float D1, float V2, float D2, float Time)
            {
                float A = 2 * (V1 - V2) + D1 + D2;
                float B = -3 * V1 + 3 * V2 - 2 * D1 - D2;
                float C = D1;
                float D = V1;

                return ((A * Time + B) * Time + C) * Time + D;
            }

            float GetAnimValue(AnimKey[] Keys, float T)
            {
                if (Keys.Length == 0)
                    return 0.0f;

                if (Keys.Length == 1)
                    return Keys[0].Value;

                int i = 1;
                while (Keys[i].Time < T)
                    ++i;

                float Time = (T - Keys[i - 1].Time) / (Keys[i].Time - Keys[i - 1].Time);
                return Interpolate(Keys[i - 1].Value, Keys[i - 1].Tangent, Keys[i].Value, Keys[i].Tangent, Time);
            }

            class AnimIndexing
            {
                public UInt16 Count, Index, Unknown;

                public AnimIndexing(byte[] Data, ref UInt32 ReadOffset)
                {
                    Count = Helpers.Read16(Data, (int)ReadOffset);
                    Index = Helpers.Read16(Data, (int)ReadOffset + 2);
                    Unknown = Helpers.Read16(Data, (int)ReadOffset + 4);

                    ReadOffset += 6;
                }
            }

            class AnimComponent
            {
                public AnimIndexing Scale, Rot, Trans;

                public AnimComponent(byte[] Data, ref UInt32 ReadOffset)
                {
                    Scale = new AnimIndexing(Data, ref ReadOffset);
                    Rot = new AnimIndexing(Data, ref ReadOffset);
                    Trans = new AnimIndexing(Data, ref ReadOffset);
                }
            }

            class AnimJoint
            {
                public AnimComponent X, Y, Z;

                public AnimJoint(byte[] Data, ref UInt32 ReadOffset)
                {
                    X = new AnimComponent(Data, ref ReadOffset);
                    Y = new AnimComponent(Data, ref ReadOffset);
                    Z = new AnimComponent(Data, ref ReadOffset);
                }
            }

            class AnimKey
            {
                public float Time, Value, Tangent;
            }

            class Animation
            {
                public AnimKey[] ScalesX, ScalesY, ScalesZ;
                public AnimKey[] RotationsX, RotationsY, RotationsZ;
                public AnimKey[] TranslationsX, TranslationsY, TranslationsZ;
            }
        }

        #endregion

        #endregion

        #region Enums

        public enum ArrayTypes
        {
            POSITION_MATRIX_INDEX,
            TEX0_MATRIX_INDEX,
            TEX1_MATRIX_INDEX,
            TEX2_MATRIX_INDEX,
            TEX3_MATRIX_INDEX,
            TEX4_MATRIX_INDEX,
            TEX5_MATRIX_INDEX,
            TEX6_MATRIX_INDEX,
            TEX7_MATRIX_INDEX,
            POSITION,
            NORMAL,
            COLOR0,
            COLOR1,
            TEX0,
            TEX1,
            TEX2,
            TEX3,
            TEX4,
            TEX5,
            TEX6,
            TEX7,
            POSITION_MATRIX_ARRAY,
            NORMAL_MATRIX_ARRAY,
            TEXTURE_MATRIX_ARRAY,
            LIT_MATRIX_ARRAY,
            NORMAL_BINORMAL_TANGENT,
            MAX_ATTR,
            NULL_ATTR,
        };

        public enum DataTypes
        {
            S8 = 1,
            S16 = 3,
            F32 = 4,
            RGBA8 = 5,
        };

        public enum SceneGraphTypes
        {
            FINISH = 0x0,
            NEW_CHILD = 0x1,
            CLOSE_CHILD = 0x2,
            JOINT = 0x10,
            MATERIAL = 0x11,
            SHAPE = 0x12
        };

        public enum PrimitiveTypes
        {
            TRIANGLES = 0x80,
            QUADS = 0x80,
            TRIANGLESTRIP = 0x98,
            TRIANGLEFAN = 0xA0,
            LINES = 0xA8,
            LINESTRIP = 0xB0,
            POINTS = 0xB8
        };

        public enum TextureFormats
        {
            I4 = 0,
            I8 = 1,
            A4_I4 = 2,
            A8_I8 = 3,
            R5_G6_B5 = 4,
            A3_RGB5 = 5,
            ARGB8 = 6,
            INDEX4 = 8,
            INDEX8 = 9,
            INDEX14_X2 = 10,
            S3TC1 = 14
        };

        public enum PaletteFormats
        {
            PAL_A8_I8 = 0,
            PAL_R5_G6_B5 = 1,
            PAL_A3_RGB5 = 2,
        };

        #endregion
    }
}
