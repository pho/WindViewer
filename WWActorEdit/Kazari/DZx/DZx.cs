using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace WWActorEdit.Kazari.DZx
{
    public class DZx
    {
        public List<FileChunk> Chunks { get { return _Chunks; } }
        public RARC.FileEntry FileEntry { get; set; }
        TreeNode Root;

        List<FileChunk> _Chunks;

        ZeldaArc ParentZA;

        public DZx(RARC.FileEntry FE, TreeNode TN, ZeldaArc PZA = null)
        {
            Root = TN;
            FileEntry = FE;
            ParentZA = PZA;
            Load();
        }

        public void AddChunk(String chunkName)
        {
            
            byte[] Data = FileEntry.GetFileData();

            byte[] newChunk = new byte[0x0C];

            String hex = (Data.Length + 0x0C).ToString("X8");

            int NumberChars = hex.Length;
            byte[] address = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                address[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            
            for (int i = 0; i < 0xC; i++)
            {
                if(i < 4) newChunk[i] = (i < chunkName.Length ? (byte)chunkName[i] : (byte)0);
                if(i >= 8) newChunk[i] = address[i-8];
            }

            for (int i = 0; i < Helpers.Read32(Data, 0); i++)
            {
                Helpers.Overwrite32(ref Data, 4 + i * 0x0C + 8, Helpers.Read32(Data, 4 + i * 0x0C + 8) + 0x0C);
            }

            Data = Helpers.AddToArray(Data, newChunk, (int)(Helpers.Read32(Data, 0) * 0x0C + 4));
            Helpers.Overwrite32(ref Data, 0, Helpers.Read32(Data, 0) + 1);

            FileEntry.SetFileData(Data);
            foreach (DZx.FileChunk C in Chunks)
            {
                
                foreach (IDZxChunkElement CE in C.Data)
                {
                    CE.Offset += 0x0C;
                    CE.StoreChanges();
                }
            }
        }

        public void Load()
        {
            int Offset = 0;

            uint ChunkCount = Helpers.Read32(FileEntry.GetFileData(), Offset);
            if (ChunkCount == 0) return;

            TreeNode NewNode = Helpers.CreateTreeNode(FileEntry.FileName, this, string.Format("Size: {0:X6}\n{1} chunks", FileEntry.DataSize, ChunkCount));

            _Chunks = new List<FileChunk>();

            Offset += 4;
            for (int i = 0; i < ChunkCount; i++)
                _Chunks.Add(new FileChunk(FileEntry, ref Offset, NewNode, ParentZA));

            NewNode.Expand();
            Root.Nodes.Add(NewNode);
        }

        public void Render()
        {
            foreach (object Obj in _Chunks)
            {
                ((FileChunk)Obj).Render();
            }
        }

        public void Clear()
        {
            foreach (object Obj in _Chunks)
            {
                foreach (IDZxChunkElement C in ((FileChunk)Obj).Data)
                    if (GL.IsList(C.GLID) == true) GL.DeleteLists(C.GLID, 1);
            }
        }

        public Control EditControl { get; set; }

        public void ShowControl(Panel Parent)
        {
            Parent.FindForm().SuspendLayout();

            EditControl = new DZxControl(this);
            EditControl.Parent = Parent;

            Parent.ClientSize = EditControl.Size;
            EditControl.Dock = DockStyle.Fill;

            Parent.Visible = true;

            Parent.FindForm().ResumeLayout();
        }

        public class FileChunk
        {
            public string Tag;
            public uint Elements, Offset;
            public object[] Data;

            public FileChunk(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, ZeldaArc ParentZA = null)
            {
                byte[] SrcData = FE.GetFileData();

                Tag = Helpers.ReadString(SrcData, SrcOffset, 4);
                Elements = Helpers.Read32(SrcData, SrcOffset + 4);
                Offset = Helpers.Read32(SrcData, SrcOffset + 8);

                Data = new object[Elements];

                TreeNode NewNode = Helpers.CreateTreeNode(Tag, this, string.Format("Offset: {0:X6}\n{1} elements", Offset, Elements));

                int ReadOffset = (int)Offset;
                for (int i = 0; i < Elements; i++)
                {
                    switch (Tag)
                    {
                        /* Typically in DZR */
                        case "ACTR": Data[i] = new ACTR(FE, ref ReadOffset, NewNode, System.Drawing.Color.GreenYellow, ParentZA); continue;
                        case "TGOB": Data[i] = new ACTR(FE, ref ReadOffset, NewNode, System.Drawing.Color.GreenYellow, ParentZA); continue;
                        case "PLYR": Data[i] = new ACTR(FE, ref ReadOffset, NewNode, System.Drawing.Color.Orange); continue;
                        case "PPNT":    /* Found in DmSpot0's Stage DZS for some reason... */
                        case "RPPN": Data[i] = new RPPN(FE, ref ReadOffset, NewNode, System.Drawing.Color.LightSkyBlue); continue;
                        case "SHIP": Data[i] = new SHIP(FE, ref ReadOffset, NewNode, System.Drawing.Color.BlueViolet); continue;
                        case "TGDR":
                        case "DOOR":
                        case "Door": Data[i] = new TGDR(FE, ref ReadOffset, NewNode, System.Drawing.Color.HotPink, ParentZA); continue;
                        case "LGTV": Data[i] = new LGTV(FE, ref ReadOffset, NewNode, System.Drawing.Color.DarkGray); continue;  /* ????? */

                        /* Typically in DZS */
                        case "MULT": Data[i] = new MULT(FE, ref ReadOffset, NewNode, System.Drawing.Color.LightGray); continue;
                        case "TRES": Data[i] = new TRES(FE, ref ReadOffset, NewNode, System.Drawing.Color.SaddleBrown); continue;
                    }

                    switch (Tag.Substring(0, 3))
                    {
                        case "ACT": Data[i] = new ACTR(FE, ref ReadOffset, NewNode, System.Drawing.Color.GreenYellow, ParentZA); break;
                        case "PLY": Data[i] = new ACTR(FE, ref ReadOffset, NewNode, System.Drawing.Color.Orange); break;
                        case "SCO": Data[i] = new TGDR(FE, ref ReadOffset, NewNode, System.Drawing.Color.Yellow, ParentZA); break;
                        case "TRE": Data[i] = new TRES(FE, ref ReadOffset, NewNode, System.Drawing.Color.SaddleBrown); break;
                        default: Data[i] = new Generic(FE, ref ReadOffset, NewNode); NewNode.Tag = Data[i]; break;
                    }
                }

                ParentNode.Nodes.Add(NewNode);

                SrcOffset += 12;
            }

            public void Render()
            {
                GL.PushAttrib(AttribMask.AllAttribBits);
                GL.Disable(EnableCap.Texture2D);
                foreach (IDZxChunkElement Chunk in Data.Where(C => C != null))
                    Chunk.Render();
                GL.PopAttrib();
            }

            public override string ToString()
            {
                return string.Format("Tag: {0}, Elements: {1:X8}, Offset: {2:X8}", Tag, Elements, Offset);
            }
        }
    }
}
