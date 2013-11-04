using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWActorEdit.Kazari.DZx
{
    public interface IDZxChunkElement
    {
        RARC.FileEntry ParentFile { get; set; }
        bool Highlight { get; set; }
        int Offset { get; set; }
        bool HasChanged { get; set; }
        int GLID { get; set; }
        Control EditControl { get; set; }
        void ShowControl(Panel Parent);
        void Render();
        void StoreChanges();
    }
}
