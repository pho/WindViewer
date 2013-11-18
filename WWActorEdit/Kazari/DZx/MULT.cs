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
        Vector2 _Translation;
        float _Rotation;
        byte _RoomNumber;
        byte _Unknown2;

        public Vector2 Translation { get { return _Translation; } set { _Translation = value; HasChanged = true; } }
        public float Rotation { get { return _Rotation; } set { _Rotation = value; HasChanged = true; } }
        public byte RoomNumber { get { return _RoomNumber; } set { _RoomNumber = value; HasChanged = true; } }
        public byte Unknown2 { get { return _Unknown2; } set { _Unknown2 = value; HasChanged = true; } }

        public RARC.FileEntry ParentFile { get; set; }
        public int Offset { get; set; }

        bool _HasChanged;
        public bool HasChanged { get { return _HasChanged; } set { _HasChanged = value; Node.ForeColor = (value == true ? System.Drawing.Color.Red : System.Drawing.SystemColors.WindowText); } }

        public bool Highlight { get; set; }
        public System.Drawing.Color RenderColor { get; set; }

        public TreeNode Node { get; set; }

        public int GLID { get; set; }

        public MULT(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, System.Drawing.Color Color = default(System.Drawing.Color))
        {
            ParentFile = FE;

            byte[] SrcData = ParentFile.GetFileData();

            Offset = SrcOffset;

            _Translation = new Vector2(
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x04)));
            _Rotation = ((short)(Helpers.Read16(SrcData, SrcOffset + 0x08)) / 182.04444444444444f).Clamp(-180, 179);
            _RoomNumber = SrcData[SrcOffset + 0x0A];
            _Unknown2 = SrcData[SrcOffset + 0x0B];

            SrcOffset += 0x0C;

            RenderColor = Color;

            Node = Helpers.CreateTreeNode(string.Format("{0:X6}: {1}", Offset, new Vector2(_Translation.X / 100000, _Translation.Y / 100000)), this, string.Format("{0}", _Translation));
            ParentNode.BackColor = RenderColor;
            ParentNode.Nodes.Add(Node);

            GLID = GL.GenLists(1);
            GL.NewList(GLID, ListMode.Compile);
            Helpers.DrawFramedCube(new Vector3d(15, 15, 15));
            GL.EndList();
        }

        public void StoreChanges()
        {
            byte[] Data = ParentFile.GetFileData();
            
            Helpers.Overwrite32(ref Data, Offset, BitConverter.ToUInt32(BitConverter.GetBytes(_Translation.X), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x04, BitConverter.ToUInt32(BitConverter.GetBytes(_Translation.Y), 0));
            Helpers.Overwrite16(ref Data, Offset + 0x08, (ushort)(_Rotation * 182.04444444444444f));
            Helpers.Overwrite8(ref Data, Offset + 0x0A, _RoomNumber);
            Helpers.Overwrite8(ref Data, Offset + 0x0B, _Unknown2);
            
            ParentFile.SetFileData(Data);
        }

        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(new OpenTK.Vector3(_Translation.X, 0, _Translation.Y));
            GL.Rotate(_Rotation, 0, 1, 0);
            GL.Color4((Highlight ? System.Drawing.Color.Red : RenderColor));
            if (GL.IsList(GLID) == true) GL.CallList(GLID);
            GL.PopMatrix();
        }

        public Control EditControl { get; set; }

        public void ShowControl(Panel Parent)
        {
            Parent.FindForm().SuspendLayout();

            EditControl = new MULTControl(this);
            EditControl.Parent = Parent;

            Parent.ClientSize = EditControl.Size;
            EditControl.Dock = DockStyle.Fill;

            Parent.Visible = true;

            Parent.FindForm().ResumeLayout();
        }
    }
}
