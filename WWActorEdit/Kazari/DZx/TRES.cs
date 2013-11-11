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
    public class TRES : IDZxChunkElement
    {
        string _Name;
        byte _Contents;
        Vector3 _Position;
        ushort _ChestType;
        float _Rotation;

        public string Name { get { return _Name; } set { _Name = value; HasChanged = true; } }
        public ushort ChestType { get { return _ChestType; } set { _ChestType = value; HasChanged = true; } }
        public Vector3 Position { get { return _Position; } set { _Position = value; HasChanged = true; } }
        public float Rotation { get { return _Rotation; } set { _Rotation = value; HasChanged = true; } }
        public byte Contents { get { return _Contents; } set { _Contents = value; HasChanged = true; } }

        public RARC.FileEntry ParentFile { get; set; }
        public int Offset { get; set; }

        bool _HasChanged;
        public bool HasChanged { get { return _HasChanged; } set { _HasChanged = value; Node.ForeColor = (value == true ? System.Drawing.Color.Red : System.Drawing.SystemColors.WindowText); } }

        public bool Highlight { get; set; }
        public System.Drawing.Color RenderColor { get; set; }

        public TreeNode Node { get; set; }

        public int GLID { get; set; }

        public J3Dx.J3Dx MatchedModel { get; set; }
        public DZB.DZB MatchedCollision { get; set; }

        public TRES(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, System.Drawing.Color Color = default(System.Drawing.Color), ZeldaArc ParentZA = null)
        {
            ParentFile = FE;

            byte[] SrcData = ParentFile.GetFileData();

            Offset = SrcOffset;

            _Name = Helpers.ReadString(SrcData, SrcOffset, 8);
            _ChestType = Helpers.Read16(SrcData, SrcOffset + 0x09);
            _Position = new Vector3(
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x0C)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x10)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x14)));
            _Rotation = (Helpers.Read16(SrcData, SrcOffset + 0x1A) / 182.04444444444444f).Clamp(-180, 179);
            _Contents = Helpers.Read8(SrcData, SrcOffset + 0x1C);

            SrcOffset += 0x20;

            RenderColor = Color;

            Node = Helpers.CreateTreeNode(string.Format("{0:X6}: {1}", Offset, _Name), this);
            ParentNode.BackColor = RenderColor;
            ParentNode.Nodes.Add(Node);

            GLID = GL.GenLists(1);
            GL.NewList(GLID, ListMode.Compile);

            if (ParentZA != null)
            {
                MatchedModel = ParentZA.J3Dxs.Find(x => x.FileEntry.FileName.StartsWith(_Name));
                MatchedCollision = ParentZA.DZBs.Find(x => x.Name.StartsWith(_Name));
            }

            Helpers.DrawFramedCube(new Vector3d(15, 15, 15));
            GL.EndList();
        }

        public void StoreChanges()
        {
            byte[] Data = ParentFile.GetFileData();

            Helpers.WriteString(ref Data, Offset, _Name, 8);
            Helpers.Overwrite16(ref Data, Offset + 0x09, _ChestType);
            Helpers.Overwrite32(ref Data, Offset + 0x0C, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.X), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x10, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.Y), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x14, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.Z), 0));
            Helpers.Overwrite16(ref Data, Offset + 0x1A, (ushort)(_Rotation * 182.04444444444444f));
            Helpers.Overwrite8(ref Data, Offset + 0x1C, _Contents);

            ParentFile.SetFileData(Data);
        }

        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(_Position);
            GL.Rotate(_Rotation, 0, 1, 0);
            GL.Color4((Highlight ? System.Drawing.Color.Red : RenderColor));
            if (MatchedModel != null) MatchedModel.Render();
            if (MatchedCollision != null) MatchedCollision.Render();
            if (GL.IsList(GLID) == true) GL.CallList(GLID);
            GL.PopMatrix();
        }

        public Control EditControl { get; set; }

        public void ShowControl(Panel Parent)
        {
            Parent.FindForm().SuspendLayout();

            EditControl = new TRESControl(this);
            EditControl.Parent = Parent;

            Parent.ClientSize = EditControl.Size;
            EditControl.Dock = DockStyle.Fill;

            Parent.Visible = true;

            Parent.FindForm().ResumeLayout();
        }
    }
}
