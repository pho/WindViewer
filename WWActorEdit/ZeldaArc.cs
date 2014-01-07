using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Globalization;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using WWActorEdit.Kazari;
using WWActorEdit.Kazari.DZx;
using WWActorEdit.Kazari.DZB;
using WWActorEdit.Kazari.J3Dx;

namespace WWActorEdit
{
    public class ZeldaArc
    {
        public RARC Archive { get; private set; }
        public List<DZx> DZRs { get; private set; }
        public List<DZx> DZSs { get; private set; }
        public List<DZB> DZBs { get; private set; }
        public List<J3Dx> J3Dxs { get; private set; }

        public Vector3 GlobalTranslation { get; set; }
        public float GlobalRotation { get; set; }
        public int RoomNumber { get; set; }

        public string Filename { get; private set; }

        public ZeldaArc(string File, TreeView TV, bool IgnoreModels = false)
        {
            Archive = new RARC(File);
            DZRs = new List<DZx>();
            DZSs = new List<DZx>();
            DZBs = new List<DZB>();
            J3Dxs = new List<J3Dx>();

            TreeNode NewNode = Helpers.CreateTreeNode(Archive.Filename, this);
            PopulateFileList(NewNode, Archive.Root, IgnoreModels);
            //TV.Nodes[TV.Nodes.Count - 1].Expand();
            //TV.SelectedNode = TV.Nodes[TV.Nodes.Count - 1];
            TV.Nodes.Add(NewNode);

            Filename = File;
        }

        public void Clear()
        {
            foreach (DZx D in DZRs) D.Clear();
            foreach (DZx D in DZSs) D.Clear();
            foreach (DZB D in DZBs) D.Clear();
            foreach (J3Dx M in J3Dxs) M.Clear();
        }

        public void Save()
        {
            foreach (DZx D in DZRs)
            {
                if (D.FileEntry.IsCompressed == true) continue;

                foreach (DZx.FileChunk C in D.Chunks)
                    foreach (IDZxChunkElement CE in C.Data)
                        if (CE.HasChanged == true) CE.StoreChanges();

                D.FileEntry.BaseRARC.Save();
            }

            foreach (DZx D in DZSs)
            {
                if (D.FileEntry.IsCompressed == true) continue;

                foreach (DZx.FileChunk C in D.Chunks)
                    foreach (IDZxChunkElement CE in C.Data)
                        if (CE.HasChanged == true) CE.StoreChanges();

                D.FileEntry.BaseRARC.Save();
            }

            foreach (DZB D in DZBs)
            {
                // unimplemented!
            }

            foreach (J3Dx J in J3Dxs)
            {
                // unimplemented!
            }
        }

        private void PopulateFileList(TreeNode TN, RARC.FileNode ParentFN, bool IgnoreModels)
        {
            foreach (RARC.FileNode ChildFN in ParentFN.ChildNodes)
                PopulateFileList(TN, ChildFN, IgnoreModels);

            foreach (RARC.FileEntry FE in ParentFN.Files)
            {
                if (J3Dx.ValidExtensions.Contains(Path.GetExtension(FE.FileName)) && IgnoreModels == false)
                    J3Dxs.Add(new J3Dx(FE, TN));

                else if (Path.GetExtension(FE.FileName) == ".dzr")
                    DZRs.Add(new DZx(FE, TN, this));

                else if (Path.GetExtension(FE.FileName) == ".dzs")
                    DZSs.Add(new DZx(FE, TN, this));

                else if (Path.GetExtension(FE.FileName) == ".dzb")
                    DZBs.Add(new DZB(FE, TN));
            }
        }
    }
}
