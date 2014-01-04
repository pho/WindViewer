using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace WWActorEdit.Kazari.STB
{
    public class FVB
    {
        string _fvbID;
        uint _fvbSize;
        uint _numEntries;

        public string fvbID { get { return _fvbID; } set { _fvbID = value; HasChanged = true; } }
        public uint fvbSize;
        uint numEntries;

        public RARC.FileEntry ParentFile { get; set; }
        public int Offset { get; set; }

        bool _HasChanged;
        public bool HasChanged { get { return _HasChanged; } set { _HasChanged = value; Node.ForeColor = (value == true ? System.Drawing.Color.Red : System.Drawing.SystemColors.WindowText); } }

        public TreeNode Node { get; set; }
    }
}
