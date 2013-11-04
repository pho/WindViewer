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
    public class MULT : IDZxChunkElement
    {
        Vector2 _HVTranslation;
        ushort _Unknown1;
        byte _RoomNumber;
        byte _Unknown2;

        public Vector2 HVTranslation { get { return _HVTranslation; } set { _HVTranslation = value; HasChanged = true; } }
        public ushort Unknown1 { get { return _Unknown1; } set { _Unknown1 = value; HasChanged = true; } }
        public byte RoomNumber { get { return _RoomNumber; } set { _RoomNumber = value; HasChanged = true; } }
        public byte Unknown2 { get { return _Unknown2; } set { _Unknown2 = value; HasChanged = true; } }

        public RARC.FileEntry ParentFile { get; set; }
        public int Offset { get; set; }

        bool _HasChanged;
        public bool HasChanged { get { return _HasChanged; } set { _HasChanged = value; } }

        public bool Highlight { get; set; }
        public System.Drawing.Color RenderColor { get; set; }

        public TreeNode Node { get; set; }

        public int GLID { get; set; }

        public MULT(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, System.Drawing.Color Color = default(System.Drawing.Color))
        {
            ParentFile = FE;

            byte[] SrcData = ParentFile.GetFileData();

            Offset = SrcOffset;

            _HVTranslation = new Vector2(
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x04)));
            _Unknown1 = Helpers.Read16(SrcData, SrcOffset + 0x08);
            _RoomNumber = SrcData[SrcOffset + 0x0A];
            _Unknown2 = SrcData[SrcOffset + 0x0B];

            SrcOffset += 0x0C;

            RenderColor = Color;

            Node = Helpers.CreateTreeNode(string.Format("{0:X6}: {1}", Offset, new Vector2(_HVTranslation.X / 100000, _HVTranslation.Y / 100000)), this, string.Format("{0}", _HVTranslation));
            ParentNode.BackColor = RenderColor;
            ParentNode.Nodes.Add(Node);
        }

        public void StoreChanges()
        {
            //
        }

        public void Render()
        {
            //
        }

        public Control EditControl { get; set; }

        public void ShowControl(Panel Parent)
        {
            //
        }
    }
}
