using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Windows.Forms;
using Blue.Windows;
using OpenTK;

namespace WWActorEdit.Forms
{
    public partial class SpawnpointEditor : Form
    {
        private MainForm _mainForm;
        private DZSFormat _data;
        private PlyrChunk _plyrChunk;

        //Used for "dockable" WinForms
        private StickyWindow _stickyWindow;

        public SpawnpointEditor(MainForm parent)
        {
            InitializeComponent();
            _mainForm = parent;
            _stickyWindow = new StickyWindow(this);
        }

 

        private void SpawnpointEditor_Load(object sender, EventArgs e)
        {
            ZeldaArc stage = _mainForm.Rooms[0];
            if (stage == null)
            {
                Console.WriteLine("Load a Stage first!");
                return;
            }

            LoadDZSForStage(stage);
        }

        private void LoadDZSForStage(ZeldaArc stage)
        {
            /*int srcOffset = 0;
            _data = new DZSFormat(stage.DZRs[0].FileEntry.GetFileData(), ref srcOffset);

            List<IChunkType> plyrChunks = _data.GetChunksOfType(DZSChunkTypes.PLYR);
            if (plyrChunks == null)
                return;

            for (int i = 0; i < plyrChunks.Count; i++)
            {
                PlyrChunk plyrChunk = (PlyrChunk)plyrChunks[i];
                spawnDropdown.Items.Add("[" + i + "] - " + plyrChunk.Name);
            }

            spawnDropdown.SelectedIndex = 0;
            _plyrChunk = (PlyrChunk)plyrChunks[spawnDropdown.SelectedIndex];
            UpdateUIControlsFromFile();*/
        }

        private void UpdateUIControlsFromFile()
        {
            spawnName.Text = _plyrChunk.Name;
            spawnEventIndex.Value = _plyrChunk.EventIndex;
            spawnUnknown1.Value = _plyrChunk.Unknown1;
            spawnType.Value = _plyrChunk.SpawnType;
            spawnRoomNum.Value = _plyrChunk.RoomNumber;
            spawnPosX.Value = (decimal)_plyrChunk.Position.X;
            spawnPosY.Value = (decimal) _plyrChunk.Position.Y;
            spawnPosZ.Value = (decimal) _plyrChunk.Position.Z;

            Vector3 rot = _plyrChunk.Rotation.ToDegrees();
            spawnRotX.Value = (decimal) rot.X;
            spawnRotY.Value = (decimal) rot.Y;
            spawnRotZ.Value = (decimal) rot.Z;
        }

        private void spawnDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            _plyrChunk = _data.GetAllChunks<PlyrChunk>()[spawnDropdown.SelectedIndex];
            UpdateUIControlsFromFile();
        }

        /// <summary>
        /// Called when any of the Numeric boxes change value.
        /// </summary>
        private void spawnIndex_ValueChanged(object sender, EventArgs e)
        {
            if (sender == spawnEventIndex)
                _plyrChunk.EventIndex = (byte) spawnEventIndex.Value;
            if (sender == spawnUnknown1)
                _plyrChunk.Unknown1 = (byte) spawnUnknown1.Value;
            if (sender == spawnType)
                _plyrChunk.SpawnType = (byte) spawnType.Value;
            if (sender == spawnRoomNum)
                _plyrChunk.RoomNumber = (byte) spawnRoomNum.Value;

            if (sender == spawnPosX)
                _plyrChunk.Position.X = (float) spawnPosX.Value;
            if (sender == spawnPosY)
                _plyrChunk.Position.Y = (float) spawnPosY.Value;
            if (sender == spawnPosZ)
                _plyrChunk.Position.Z = (float) spawnPosZ.Value;

            Vector3 curRot = _plyrChunk.Rotation.ToDegrees();
            if (sender == spawnRotX)
                curRot.X = (float) spawnRotX.Value;
            if (sender == spawnRotY)
                curRot.Y = (float) spawnRotY.Value;
            if (sender == spawnRotZ)
                curRot.Z = (float) spawnRotZ.Value;

            _plyrChunk.Rotation.SetDegrees(curRot);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //OH BOY D:<


            /*foreach (DZSChunkHeader chunk in _data.ChunkHeaders)
            {
                if (chunk.Tag == "PLYR")
                {
                    //By creating the file this way we can still write to it while it's open in another program (ie:
                    //a hex editor) and then the hex editor can notice the change and reload.
                    FileStream fs = File.Open(chunk.Tag, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    BinaryWriter bw = new BinaryWriter(fs);

                    for (int i = 0; i < chunk.ElementCount; i++)
                    {
                        chunk.ChunkElements[i].WriteData(bw);
                    }

                    bw.Close();
                    fs.Close();
                }
            }*/
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
