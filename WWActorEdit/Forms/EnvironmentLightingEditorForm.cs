using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WWActorEdit.Kazari.DZx;

namespace WWActorEdit.Forms
{
    public partial class EnvironmentLightingEditorForm : Form
    {
        //Set by the MainForm when it opens this Popup
        private MainForm _mainForm;

        private DZSFormat _data;

        //We're going to keep a quick reference to these here.
        private EnvRChunk _envrChunk;
        private ColoChunk _coloChunk;

        public EnvironmentLightingEditorForm(MainForm parent)
        {
            InitializeComponent();

            _mainForm = parent;
        }


        private void LoadDZSForStage(ZeldaArc stage)
        {
            int srcOffset = 0;
            _data = new DZSFormat(stage.DZSs[0].FileEntry.GetFileData(), ref srcOffset);

            //Now that the DZSFormat is populated with information, we're going to load the UI up!

            //EnvR
            foreach (DZSChunkHeader chunk in _data.ChunkHeaders)
            {
                switch (chunk.Tag)
                {
                    case "EnvR": 
                        //Populate the Dropdown
                        for (int i = 0; i < chunk.ElementCount; i++)
                            EnvRDropdown.Items.Add("EnvR [" + i + "]");
                        EnvRDropdown.SelectedIndex = 0;
                        break;
                    case "Colo": 
                        //Populate the Dropdown
                        for (int i = 0; i < chunk.ElementCount; i++)
                            ColorDropdown.Items.Add("Colo [" + i + "]");
                        ColorDropdown.SelectedIndex = 0;
                        break;
                    case "Pale":
                    case "Virt":
                    default:
                        break;
                }
            }

        }

        //I'm not really sure... what this should be called. Ech.
        private void LoadEnvrElement()
        {
            //Need to find the EnvRchunk again, and get the right index.
            foreach (var header in _data.ChunkHeaders)
            {
                if (header.Tag == "EnvR")
                {
                    _envrChunk = (EnvRChunk)header.ChunkElements[EnvRDropdown.SelectedIndex];
                    break;
                }
            }
        }

        private void LoadColorElement()
        {
            //Need to find the ColoChunk again, and get the right index.
            foreach (var header in _data.ChunkHeaders)
            {
                if (header.Tag == "Colo")
                {
                    _coloChunk = (ColoChunk)header.ChunkElements[ColorDropdown.SelectedIndex];
                    break;
                }
            }
        }

        /// <summary>
        /// This will update the values within the Environment GroupBox to point to whatever the
        /// current _envrChunk's values are. 
        /// </summary>
        private void UpdateEnvrGroupBox()
        {
            //If they have Type A selected we populate the same UI elements but with different data...
            if (EnvRTypeA.Checked)
            {
                EnvRClearSkiesIndex.Value = _envrChunk.ClearColorIndexA;
                EnvRRainingIndex.Value = _envrChunk.RainingColorIndexA;
                EnvRSnowingIndex.Value = _envrChunk.SnowingColorIndexA;
                EnvRUnknownIndex.Value = _envrChunk.UnknownColorIndexA;
            }
            else
            {
                EnvRClearSkiesIndex.Value = _envrChunk.ClearColorIndexB;
                EnvRRainingIndex.Value = _envrChunk.RainingColorIndexB;
                EnvRSnowingIndex.Value = _envrChunk.SnowingColorIndexB;
                EnvRUnknownIndex.Value = _envrChunk.UnknownColorIndexB;
            }
        }

        /// <summary>
        /// This will update all of the values within the Color GroupBox to point to whatever the
        /// //current _coloChunk's values are.
        /// </summary>
        private void UpdateColoGroupBox()
        {
            ColoDawnIndex.Value = _coloChunk.DawnIndex;
            ColoMorningIndex.Value = _coloChunk.MorningIndex;
            ColoNoonIndex.Value = _coloChunk.NoonIndex;
            ColoAfternoonIndex.Value = _coloChunk.AfternoonIndex;
            ColoDuskIndex.Value = _coloChunk.DuskIndex;
            ColoNightIndex.Value = _coloChunk.NightIndex;
        }

        /// <summary>
        /// Called when the Form is loaded. This is a temporary solution until there's some form of Event evoked by
        /// Archives being loaded. We'll grab the loaded archives from the MainForm and populate our list of DZS files
        /// with it.
        /// </summary>
        private void EnvironmentLightingEditorForm_Load(object sender, EventArgs e)
        {
            ZeldaArc stage = _mainForm.Stage;
            //For each loaded Archive we're going to want to grab the DZS file out of them.
            foreach (DZx dzS in stage.DZSs)
            {
                dzsFileDropdown.Items.Add(Path.GetFileName(stage.Filename) + @"\" + dzS.FileEntry.FileName);
            }

            LoadDZSForStage(stage);
            dzsFileDropdown.SelectedIndex = 0;
        }

        /// <summary>
        /// Called when the user changes the EnvR type from A to B or back. Need to update all of the Values because
        /// we're sharing controls for types A and B.
        /// </summary>
        private void EnvRType_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnvrGroupBox();
        }

        /// <summary>
        /// Called when the user changes the EnvR dropdown index. We'll need to update all of the Values to point to
        /// the new EnvR element.
        /// </summary>
        private void EnvRDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEnvrElement();
            UpdateEnvrGroupBox();
        }

        /// <summary>
        /// Called when the user changes the Color dropdown index.
        /// </summary>
        private void ColorDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadColorElement();
            UpdateColoGroupBox();
        }
    }
}
