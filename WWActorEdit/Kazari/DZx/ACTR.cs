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
    public class ACTR : IDZxChunkElement
    {
        string _Name;
        uint _Parameters;
        Vector3 _Position, _Rotation;
        ushort _Unknown;

        public string Name { get { return _Name; } set { _Name = value; HasChanged = true; } }
        public uint Parameters { get { return _Parameters; } set { _Parameters = value; HasChanged = true; } }
        public Vector3 Position { get { return _Position; } set { _Position = value; HasChanged = true; } }
        public Vector3 Rotation { get { return _Rotation; } set { _Rotation = value; HasChanged = true; } }
        public ushort Unknown { get { return _Unknown; } set { _Unknown = value; HasChanged = true; } }

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

        public ACTR(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, System.Drawing.Color Color = default(System.Drawing.Color), ZeldaArc ParentZA = null)
        {
            ParentFile = FE;

            byte[] SrcData = ParentFile.GetFileData();

            Offset = SrcOffset;

            _Name = Helpers.ReadString(SrcData, SrcOffset, 8);
            _Parameters = Helpers.Read32(SrcData, SrcOffset + 8);
            _Position = new Vector3(
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x0C)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x10)),
                Helpers.ConvertIEEE754Float(Helpers.Read32(SrcData, SrcOffset + 0x14)));
            _Rotation = new Vector3(
                ((short)Helpers.Read16(SrcData, SrcOffset + 0x18) / 182.04444444444444f).Clamp(-180, 179),
                ((short)Helpers.Read16(SrcData, SrcOffset + 0x1A) / 182.04444444444444f).Clamp(-180, 179),
                ((short)Helpers.Read16(SrcData, SrcOffset + 0x1C) / 182.04444444444444f).Clamp(-180, 179));
            _Unknown = Helpers.Read16(SrcData, SrcOffset + 0x1E);

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
            Helpers.Overwrite32(ref Data, Offset + 0x08, _Parameters);
            Helpers.Overwrite32(ref Data, Offset + 0x0C, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.X), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x10, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.Y), 0));
            Helpers.Overwrite32(ref Data, Offset + 0x14, BitConverter.ToUInt32(BitConverter.GetBytes(_Position.Z), 0));
            Helpers.Overwrite16(ref Data, Offset + 0x18, (ushort)(_Rotation.X * 182.04444444444444f));
            Helpers.Overwrite16(ref Data, Offset + 0x1A, (ushort)(_Rotation.Y * 182.04444444444444f));
            Helpers.Overwrite16(ref Data, Offset + 0x1C, (ushort)(_Rotation.Z * 182.04444444444444f));
            Helpers.Overwrite16(ref Data, Offset + 0x1E, _Unknown);

            ParentFile.SetFileData(Data);
        }

        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(_Position);
            GL.Rotate(_Rotation.Z, 0, 0, 1);
            GL.Rotate(_Rotation.Y, 0, 1, 0);
            GL.Rotate(_Rotation.X, 1, 0, 0);
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

            EditControl = new ACTRControl(this);
            EditControl.Parent = Parent;

            Parent.ClientSize = EditControl.Size;
            EditControl.Dock = DockStyle.Fill;

            Parent.Visible = true;

            Parent.FindForm().ResumeLayout();
        }
    }
}
