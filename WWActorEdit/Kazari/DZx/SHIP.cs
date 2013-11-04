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
    public class SHIP : IDZxChunkElement
    {
        uint _Unknown;
        Vector3 _Position;

        public uint Unknown { get { return _Unknown; } set { _Unknown = value; HasChanged = true; } }
        public Vector3 Position { get { return _Position; } set { _Position = value; HasChanged = true; } }

        public RARC.FileEntry ParentFile { get; set; }
        public int Offset { get; set; }

        bool _HasChanged;
        public bool HasChanged { get { return _HasChanged; } set { _HasChanged = value; Node.ForeColor = (value == true ? System.Drawing.Color.Red : System.Drawing.SystemColors.WindowText); } }

        public bool Highlight { get; set; }
        public System.Drawing.Color RenderColor { get; set; }

        public TreeNode Node { get; set; }

        public int GLID { get; set; }

        public SHIP(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, System.Drawing.Color Color = default(System.Drawing.Color))
        {
            ParentFile = FE;

            byte[] SrcData = ParentFile.GetFileData();

            Offset = SrcOffset;

            _Position = new Vector3(
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x04)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x08)));
            _Unknown = Helpers.Read32(SrcData, SrcOffset + 0x0C);

            SrcOffset += 0x10;

            RenderColor = Color;

            Node = Helpers.CreateTreeNode(string.Format("{0:X6}: 0x{1:X8}", Offset, _Unknown), this);
            ParentNode.BackColor = RenderColor;
            ParentNode.Nodes.Add(Node);

            GLID = GL.GenLists(1);
            GL.NewList(GLID, ListMode.Compile);
            Helpers.DrawFramedSphere(new Vector3d(0, 0, 0), 25.0f, 10);
            GL.EndList();
        }

        public void StoreChanges()
        {
            byte[] Data = ParentFile.GetFileData();

            Helpers.Overwrite32(ref Data, Offset, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.X), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x04, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.Y), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x08, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.Z), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x0C, _Unknown);

            ParentFile.SetFileData(Data);
        }

        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(_Position);
            GL.Color4((Highlight ? System.Drawing.Color.Red : RenderColor));
            if (GL.IsList(GLID) == true) GL.CallList(GLID);
            GL.PopMatrix();
        }

        public Control EditControl { get; set; }

        public void ShowControl(Panel Parent)
        {
            Parent.FindForm().SuspendLayout();

            EditControl = new SHIPControl(this);
            EditControl.Parent = Parent;

            Parent.ClientSize = EditControl.Size;
            EditControl.Dock = DockStyle.Fill;

            Parent.Visible = true;

            Parent.FindForm().ResumeLayout();
        }
    }
}
