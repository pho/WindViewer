using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blue.Windows;
using WWActorEdit.Kazari.DZx;

namespace WWActorEdit.Forms
{
    public partial class ExitEditor : Form
    {
        private MainForm _mainForm;
        private DZSFormat _data;

        //Change this and then call UpdateSCLSControlsFromFile to update the UI.
        private SclsChunk _sclsChunk;

        //Used for "dockable" WinForms
        private StickyWindow _stickyWindow;

        public ExitEditor(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            _stickyWindow = new StickyWindow(this);
        }

        /// <summary>
        /// Call this when you want to read the current _sclsChunk and update
        /// the UI controls.
        /// </summary>
        private void UpdateSclsControlsFromFile()
        {
            sclsDestName.Text = _sclsChunk.DestinationName;
            sclsRoomIndex.Value = _sclsChunk.DestinationRoomNumber;
            sclsSpawnIndex.Value = _sclsChunk.SpawnNumber;
            sclsExitTypeIndex.Value = _sclsChunk.ExitType;

            //Probably padding but I'd rather remove it once implemented than implement later...
            sclsPaddingIndex.Value = _sclsChunk.UnknownPadding;
        }




        /// <summary>
        /// This is temporary until we replace out the main form's version of the DZS loading stuff.
        /// Sorry!
        /// </summary>
        /// <param name="stage"></param>
        private void LoadDZSForStage(ZeldaArc stage)
        {
            int srcOffset = 0;
            _data = new DZSFormat(stage.DZRs[0].FileEntry.GetFileData(), ref srcOffset);

            List<IChunkType> sclsChunks = _data.GetChunksOfType(DZSChunkTypes.SCLS);
            if (sclsChunks == null)
                return;

            for (int i = 0; i < sclsChunks.Count; i++)
            {
                SclsChunk exitChunk = (SclsChunk) sclsChunks[i];
                sclsDropdown.Items.Add("[" + i + "] - " + exitChunk.DestinationName);
            }

            sclsDropdown.SelectedIndex = 0;
            _sclsChunk = (SclsChunk) sclsChunks[sclsDropdown.SelectedIndex];
            UpdateSclsControlsFromFile();
        }

        private void RoomExitEditor_Load(object sender, EventArgs e)
        {
            ZeldaArc stage = _mainForm.Rooms[0];
            if (stage == null)
            {
                Console.WriteLine("Load a Stage first!");
                return;
            }

            LoadDZSForStage(stage);
        }

        /// <summary>
        /// Called when the user changes the scls Dropdown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sclsDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<IChunkType> sclsChunks = _data.GetChunksOfType(DZSChunkTypes.SCLS);
            _sclsChunk = (SclsChunk) sclsChunks[sclsDropdown.SelectedIndex];
            UpdateSclsControlsFromFile();
        }

        /// <summary>
        /// Called when any of the numeric input boxes change on the Exit Editor.
        /// </summary>
        private void sclsIndex_ValueChanged(object sender, EventArgs e)
        {
            if (sender == sclsRoomIndex)
                _sclsChunk.DestinationRoomNumber = (byte)sclsRoomIndex.Value;

            if (sender == sclsSpawnIndex)
                _sclsChunk.SpawnNumber = (byte) sclsSpawnIndex.Value;

            if (sender == sclsExitTypeIndex)
                _sclsChunk.ExitType = (byte) sclsExitTypeIndex.Value;

            if (sender == sclsPaddingIndex)
                _sclsChunk.UnknownPadding = (byte) sclsPaddingIndex.Value;
        }

        /// <summary>
        /// Called when the user changes the text of the destination name.
        /// </summary>
        private void sclsDestName_TextChanged(object sender, EventArgs e)
        {
            _sclsChunk.DestinationName = sclsDestName.Text;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //OH BOY D:<


            foreach (DZSChunkHeader chunk in _data.ChunkHeaders)
            {
                if (chunk.Tag == "SCLS")
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
            }
        }

        /// <summary>
        /// Close the form when the user clicks Cancel.
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
