using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWActorEdit.Kazari.DZx
{
    public class Generic : IDZxChunkElement
    {
        public RARC.FileEntry ParentFile { get; set; }
        public int Offset { get; set; }

        bool _HasChanged;
        public bool HasChanged { get { return _HasChanged; } set { _HasChanged = value; Node.ForeColor = (value == true ? System.Drawing.Color.Red : System.Drawing.SystemColors.WindowText); } }

        public bool Highlight { get; set; }

        public TreeNode Node { get; set; }

        public int GLID { get; set; }

        public Generic(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode)
        {
            ParentFile = FE;
            Node = ParentNode;

            Offset = SrcOffset;
        }

        public void StoreChanges()
        {
            byte[] Data = ParentFile.GetFileData();

            //

            ParentFile.SetFileData(Data);
        }

        public void Render()
        {
            //
        }

        public Control EditControl { get; set; }

        public void ShowControl(Panel Parent)
        {
            Parent.FindForm().SuspendLayout();

            EditControl = new HexEditBox();
            SetupHexBox((HexEditBox)EditControl);

            EditControl.Parent = Parent;

            Parent.ClientSize = EditControl.Size;
            EditControl.Dock = DockStyle.Fill;

            Parent.Visible = true;

            Parent.FindForm().ResumeLayout();
        }

        private void SetupHexBox(HexEditBox hexEditBox1)
        {
            hexEditBox1.BackColor = System.Drawing.SystemColors.Window;
            hexEditBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            hexEditBox1.BytesPerLine = 16;
            hexEditBox1.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            hexEditBox1.SetData(ParentFile.GetFileData());
            hexEditBox1.AllowEdit = false;
            hexEditBox1.OffsetBytes = 4;
            hexEditBox1.ShowOffsetPrefix = true;
            hexEditBox1.BaseOffset = Offset;
        }
    }
}
