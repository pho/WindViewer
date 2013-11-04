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
    public class LGTV : IDZxChunkElement
    {
        Vector3 _Unknown1, _Unknown2;
        uint _Unknown3;

        public Vector3 Unknown1 { get { return _Unknown1; } set { _Unknown1 = value; HasChanged = true; } }
        public Vector3 Unknown2 { get { return _Unknown2; } set { _Unknown2 = value; HasChanged = true; } }
        public uint Unknown3 { get { return _Unknown3; } set { _Unknown3 = value; HasChanged = true; } }

        public RARC.FileEntry ParentFile { get; set; }
        public int Offset { get; set; }

        bool _HasChanged;
        public bool HasChanged { get { return _HasChanged; } set { _HasChanged = value; } }

        public bool Highlight { get; set; }
        public System.Drawing.Color RenderColor { get; set; }

        public TreeNode Node { get; set; }

        public int GLID { get; set; }

        public LGTV(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, System.Drawing.Color Color = default(System.Drawing.Color))
        {
            ParentFile = FE;

            byte[] SrcData = ParentFile.GetFileData();

            Offset = SrcOffset;

            _Unknown1 = new Vector3(
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x04)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x08)));
            _Unknown2 = new Vector3(
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x0C)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x10)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x14)));
            _Unknown3 = Helpers.Read32(SrcData, SrcOffset + 0x18);

            SrcOffset += 0x1C;

            RenderColor = Color;

            Node = Helpers.CreateTreeNode(string.Format("{0:X6}", Offset), this, string.Format("{0}\n{1}\n0x{2:X8}", _Unknown1, _Unknown2, _Unknown3));
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
