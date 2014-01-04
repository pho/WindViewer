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
    public class STB
    {
        public List<FileObject> Objects { get { return _Objects; } }
        public RARC.FileEntry FileEntry { get; set; }
        TreeNode Root;

        List<FileObject> _Objects;

        ZeldaArc ParentZA;

        public STB(RARC.FileEntry FE, TreeNode TN, ZeldaArc PZA = null)
        {
            Root = TN;
            FileEntry = FE;
            ParentZA = PZA;
            load();
        }

        public void load()
        {
            int offset = 0;

            string tag = Helpers.ReadString(FileEntry.GetFileData(), offset, 4);
            uint fileSize = Helpers.Read32(FileEntry.GetFileData(), offset + 8);
            uint objectCount = Helpers.Read32(FileEntry.GetFileData(), offset + 12);
            if (tag != "STB") return;
            if (objectCount == 0) return;

            TreeNode newNode = Helpers.CreateTreeNode(FileEntry.FileName, null, string.Format("Size: {0:X6}\n{1} Objects", FileEntry.DataSize, objectCount));

            _Objects = new List<FileObject>();

            offset += 32;
            for (int i = 0; i < objectCount; i++)
                _Objects.Add(new FileObject(FileEntry, ref offset, newNode, ParentZA));

            newNode.Expand();
            Root.Nodes.Add(newNode);
        }

        public class FileObject
        {
            public uint objectSize;
            public string objectType;
            public uint objectNameLength;
            public string objectName;
            public object[] Data;

            public FileObject(RARC.FileEntry FE, ref int SrcOffset, TreeNode ParentNode, ZeldaArc ParentZA = null)
            {
                byte[] SrcData = FE.GetFileData();

                objectSize = Helpers.Read32(SrcData, SrcOffset);
                objectType = Helpers.ReadString(SrcData, SrcOffset + 4, 4);
                
                /*
                if (objectType == "JFVB")
                {
                    ParentNode.Nodes.Add(newNodeObjects);

                    SrcOffset += (int)objectSize;
                    //new FVB(FE, ref SrcOffset, newNodeObjects);
                    SrcOffset += (int)objectSize;
                    return;
                }
                */
                  
                objectNameLength = Helpers.Read32(SrcData, SrcOffset + 8);
                objectName = Helpers.ReadString(SrcData, SrcOffset + 12);

                if (objectType == "JFVB")
                {
                    objectName = "FVB";

                    TreeNode newNodeObjects = Helpers.CreateTreeNode(objectName, this, string.Format("Size: {0:X6}", objectSize));

                    //new FVB(FE, ref SrcOffset, newNodeObjects);

                    ParentNode.Nodes.Add(newNodeObjects);

                    SrcOffset += (int)objectSize;
                }

                else
                {
                    TreeNode newNodeObjects = Helpers.CreateTreeNode(objectName, this, string.Format("Size: {0:X6}", objectSize));

                    //new jObject(FE, ref SrcOffset, newNodeObjects);

                    ParentNode.Nodes.Add(newNodeObjects);

                    SrcOffset += (int)objectSize;
                }

                /*
                TreeNode newNodeObjects = Helpers.CreateTreeNode(objectName, this, string.Format("Size: {0:X6}", objectSize));

                //new jObject(FE, ref SrcOffset, newNodeObjects);

                ParentNode.Nodes.Add(newNodeObjects);

                SrcOffset += (int)objectSize;
                
                 */
            }
        }

    }
}
