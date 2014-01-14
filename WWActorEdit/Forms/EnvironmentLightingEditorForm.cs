using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WWActorEdit.Kazari;
using WWActorEdit.Kazari.DZx;
using WWActorEdit.Source;

namespace WWActorEdit.Forms
{
    public partial class EnvironmentLightingEditorForm : Form
    {
        //Set by the MainForm when it opens this Popup
        private MainForm _mainForm;

        private DZSFormat _data;

        //These are references to the currently selected EnvR/Color/Etc. chunks. They are
        //used to populate the UI with the values of the selected index. When the index
        //is changed the data is still kept (inside the _data tree), but these references
        //will change.
        private EnvRChunk _envrChunk;
        private ColoChunk _coloChunk;
        private PaleChunk _paleChunk;
        private VirtChunk _virtChunk;

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
                        //Populate the Dropdown
                        for (int i = 0; i < chunk.ElementCount; i++)
                            PaleDropdown.Items.Add("Pale [" + i + "]");
                        PaleDropdown.SelectedIndex = 0;
                        break;
                    case "Virt":
                        //Populate the Dropdown
                        for (int i = 0; i < chunk.ElementCount; i++)
                            VirtDropdown.Items.Add("Virt [" + i + "]");
                        VirtDropdown.SelectedIndex = 0;
                        break;
                    default:
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
        /// This updates all of the values within the Pale Groupbox to point to whatever the current
        /// _paleChunk's values are.
        /// </summary>
        private void UpdatePaleGroupBox()
        {
            PaleActorAmbientColor.BackColor = SetPaleColorBoxColor(_paleChunk.ActorAmbient);
            PaleShadowColor.BackColor = SetPaleColorBoxColor(_paleChunk.ShadowColor);
            PaleRoomFillColor.BackColor = SetPaleColorBoxColor(_paleChunk.RoomFillColor);
            PaleRoomAmbientColor.BackColor = SetPaleColorBoxColor(_paleChunk.RoomAmbient);
            PaleWaveColor.BackColor = SetPaleColorBoxColor(_paleChunk.WaveColor);
            PaleOceanColor.BackColor = SetPaleColorBoxColor(_paleChunk.OceanColor);
            PaleUnknown1Color.BackColor = SetPaleColorBoxColor(_paleChunk.UnknownColor1);
            PaleUnknown2Color.BackColor = SetPaleColorBoxColor(_paleChunk.UnknownColor2);
            PaleDoorwayColor.BackColor = SetPaleColorBoxColor(_paleChunk.DoorwayColor);
            PaleUnknown3Color.BackColor = SetPaleColorBoxColor(_paleChunk.UnknownColor3);
            PaleFogColor.BackColor = SetPaleColorBoxColor(_paleChunk.FogColor);

            PaleVirtIndex.Value = _paleChunk.VirtIndex;
            PaleOceanFadeIntoColor.BackColor = SetPaleColorBoxColor(_paleChunk.OceanFadeInto);
            PaleOceanFadeAlpha.Value = _paleChunk.OceanFadeInto.A;

            PaleShoreFadeIntoColor.BackColor = SetPaleColorBoxColor(_paleChunk.ShoreFadeInto);
            PaleShoreFadeAlpha.Value = _paleChunk.ShoreFadeInto.A;
        }

        private void UpdateVirtGroupBox()
        {
            VirtHorizonCloudColor.BackColor = SetPaleColorBoxColor(_virtChunk.HorizonCloudColor);
            VirtUnknown1Index.Value = _virtChunk.HorizonCloudColor.A;

            VirtCenterCloudColor.BackColor = SetPaleColorBoxColor(_virtChunk.CenterCloudColor);
            VirtUnknown2Index.Value = _virtChunk.CenterCloudColor.A;

            VirtCenterSkyColor.BackColor = SetPaleColorBoxColor(_virtChunk.CenterSkyColor);
            VirtHorizonColor.BackColor = SetPaleColorBoxColor(_virtChunk.HorizonColor);
            VirtSkyFadeToColor.BackColor = SetPaleColorBoxColor(_virtChunk.SkyFadeTo);
        }

        /// <summary>
        /// Called when the Form is loaded. This is a temporary solution until there's some form of Event evoked by
        /// Archives being loaded. We'll grab the loaded archives from the MainForm and populate our list of DZS files
        /// with it.
        /// </summary>
        private void EnvironmentLightingEditorForm_Load(object sender, EventArgs e)
        {
            ZeldaArc stage = _mainForm.Stage;
            if (stage == null)
            {
                Console.WriteLine("Load a Stage first!");
                return;
            }


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
            _envrChunk = (EnvRChunk) _data.GetChunksOfType(DZSChunkTypes.EnvR)[EnvRDropdown.SelectedIndex];
            UpdateEnvrGroupBox();
        }

        /// <summary>
        /// Called when the user changes the Color dropdown index.
        /// </summary>
        private void ColorDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            _coloChunk = (ColoChunk)_data.GetChunksOfType(DZSChunkTypes.Colo)[ColorDropdown.SelectedIndex];
            UpdateColoGroupBox();
        }

        private void PaleDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            _paleChunk = (PaleChunk)_data.GetChunksOfType(DZSChunkTypes.Pale)[PaleDropdown.SelectedIndex];
            UpdatePaleGroupBox();
        }

        private void VirtDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            _virtChunk = (VirtChunk)_data.GetChunksOfType(DZSChunkTypes.Virt)[VirtDropdown.SelectedIndex];
            UpdateVirtGroupBox();
        }

        /// <summary>
        /// Called when ANY of the color fields in Pale are clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaleColorField_Click(object sender, EventArgs e)
        {
            //Set the color in the Color Picker to what it currently is
            //And then pause the app till we get a new color.
            PictureBox outputBox = (PictureBox) sender;

            colorPickerDialog.Color = outputBox.BackColor;
            colorPickerDialog.ShowDialog(this);

            outputBox.BackColor = colorPickerDialog.Color;

            //Update whoever generated the event.
            if(sender == PaleActorAmbientColor)
                _paleChunk.ActorAmbient = SetPaleMemoryColor(PaleActorAmbientColor);
            if(sender == PaleRoomFillColor)
                _paleChunk.RoomFillColor = SetPaleMemoryColor(PaleRoomFillColor);
            if(sender == PaleRoomAmbientColor)
                _paleChunk.RoomAmbient = SetPaleMemoryColor(PaleRoomAmbientColor);
            if(sender == PaleWaveColor)
                _paleChunk.WaveColor = SetPaleMemoryColor(PaleWaveColor);
            if (sender == PaleUnknown1Color)
                _paleChunk.UnknownColor1 = SetPaleMemoryColor(PaleUnknown1Color);
            if (sender == PaleUnknown2Color)
                _paleChunk.UnknownColor2 = SetPaleMemoryColor(PaleUnknown2Color);
            if(sender == PaleOceanColor)
                _paleChunk.OceanColor = SetPaleMemoryColor(PaleOceanColor);
            if (sender == PaleUnknown3Color)
                _paleChunk.UnknownColor3 = SetPaleMemoryColor(PaleUnknown3Color);
            if(sender == PaleDoorwayColor)
                _paleChunk.DoorwayColor = SetPaleMemoryColor(PaleDoorwayColor);
            if(sender == PaleFogColor)
                _paleChunk.FogColor = SetPaleMemoryColor(PaleFogColor);

            if (sender == PaleOceanFadeIntoColor)
            {
                ByteColorAlpha OceanFadeInto = new ByteColorAlpha(SetPaleMemoryColor(PaleOceanFadeIntoColor));
                OceanFadeInto.A = (byte)PaleOceanFadeAlpha.Value;
                _paleChunk.OceanFadeInto = OceanFadeInto;
            }

            if (sender == PaleShoreFadeIntoColor)
            {
                ByteColorAlpha ShoreFadeInto = new ByteColorAlpha(SetPaleMemoryColor(PaleShoreFadeIntoColor));
                ShoreFadeInto.A = (byte) PaleShoreFadeAlpha.Value;
                _paleChunk.ShoreFadeInto = ShoreFadeInto;
            }
        }

        private void VirtColorField_Click(object sender, EventArgs e)
        {
            PictureBox outputBox = (PictureBox)sender;

            colorPickerDialog.Color = outputBox.BackColor;
            colorPickerDialog.ShowDialog(this);

            outputBox.BackColor = colorPickerDialog.Color;

            if (sender == VirtHorizonCloudColor)
            {
                ByteColorAlpha HorizonCloud = new ByteColorAlpha(SetPaleMemoryColor(VirtHorizonCloudColor));
                HorizonCloud.A = (byte) VirtUnknown1Index.Value;
                _virtChunk.HorizonCloudColor = HorizonCloud;
            }

            if (sender == VirtCenterCloudColor)
            {
                ByteColorAlpha CenterCloud = new ByteColorAlpha(SetPaleMemoryColor(VirtCenterCloudColor));
                CenterCloud.A = (byte)VirtUnknown2Index.Value;
                _virtChunk.CenterCloudColor = CenterCloud;
            }
           
            if(sender == VirtCenterSkyColor)
                _virtChunk.CenterSkyColor = SetPaleMemoryColor(VirtCenterSkyColor);
            if(sender == VirtHorizonColor)
                _virtChunk.HorizonColor = SetPaleMemoryColor(VirtHorizonColor);
            if(sender == VirtSkyFadeToColor)
                _virtChunk.SkyFadeTo = SetPaleMemoryColor(VirtSkyFadeToColor);
        }

        private Color SetPaleColorBoxColor(ByteColor color)
        {
            Color newColor = Color.FromArgb(color.R, color.G, color.B);
            return newColor;
        }

        private Color SetPaleColorBoxColor(ByteColorAlpha color)
        {
            Color newColor = Color.FromArgb(255, color.R, color.G, color.B);
            return newColor;
        }

        private ByteColor SetPaleMemoryColor(PictureBox source)
        {
            ByteColor c = new ByteColor();
            c.R = source.BackColor.R;
            c.G = source.BackColor.G;
            c.B = source.BackColor.B;

            return c;
        }

        /// <summary>
        /// Called when any of the Indexes change in the EnvRGroup.
        /// </summary>
        private void EnvRGroupBoxIndex_ValueChanged(object sender, EventArgs e)
        {
            //Going to just copy all of their values back into the _envRChunk,
            //because I haven't come up with a better way yet!
            //If they have Type A selected we populate the same UI elements but with different data...
            if (EnvRTypeA.Checked)
            {
                if(sender == EnvRClearSkiesIndex)
                    _envrChunk.ClearColorIndexA = (byte) EnvRClearSkiesIndex.Value;
                if(sender == EnvRRainingIndex)
                    _envrChunk.RainingColorIndexA = (byte) EnvRRainingIndex.Value;
                if(sender == EnvRSnowingIndex)
                    _envrChunk.SnowingColorIndexA = (byte) EnvRSnowingIndex.Value;
                if(sender == EnvRUnknownIndex)
                    _envrChunk.UnknownColorIndexA = (byte) EnvRUnknownIndex.Value;
            }
            else
            {
                if (sender == EnvRClearSkiesIndex)
                    _envrChunk.ClearColorIndexB = (byte)EnvRClearSkiesIndex.Value;
                if (sender == EnvRRainingIndex)
                    _envrChunk.RainingColorIndexB = (byte)EnvRRainingIndex.Value;
                if (sender == EnvRSnowingIndex)
                    _envrChunk.SnowingColorIndexB = (byte)EnvRSnowingIndex.Value;
                if (sender == EnvRUnknownIndex)
                    _envrChunk.UnknownColorIndexB = (byte)EnvRUnknownIndex.Value;
            }
        }

        /// <summary>
        /// Called when any of the indexes in the Pale group change.
        /// </summary>
        private void PaleIndex_ValueChanged(object sender, EventArgs e)
        {
            if(sender == PaleVirtIndex)
                _paleChunk.VirtIndex = (byte) PaleVirtIndex.Value;
            if (sender == PaleOceanFadeAlpha)
                _paleChunk.OceanFadeInto.A = (byte) PaleOceanFadeAlpha.Value;
            if (sender == PaleShoreFadeAlpha)
                _paleChunk.ShoreFadeInto.A = (byte) PaleShoreFadeAlpha.Value;
        }

        /// <summary>
        /// Called when either of the Unknown groups in Virt change.
        /// </summary>
        private void VirtUnknownIndex_ValueChanged(object sender, EventArgs e)
        {
            if(sender == VirtUnknown1Index)
                _virtChunk.HorizonCloudColor.A = (byte) VirtUnknown1Index.Value;
            if(sender == VirtUnknown2Index)
                _virtChunk.CenterCloudColor.A = (byte)VirtUnknown2Index.Value;
        }

        /// <summary>
        /// Called when anything in the Color GroupBox change.
        /// </summary>
        private void ColoGroupBoxIndex_ValueChanged(object sender, EventArgs e)
        {
            if(sender == ColoDawnIndex)
                _coloChunk.DawnIndex = (byte) ColoDawnIndex.Value;
            if(sender == ColoMorningIndex)
                _coloChunk.MorningIndex = (byte) ColoMorningIndex.Value;
            if(sender == ColoNoonIndex)
                _coloChunk.NoonIndex = (byte) ColoNoonIndex.Value;
            if(sender == ColoAfternoonIndex)
                _coloChunk.AfternoonIndex = (byte) ColoAfternoonIndex.Value;
            if(sender == ColoDuskIndex)
                _coloChunk.DuskIndex = (byte) ColoDuskIndex.Value;
            if(sender == ColoNightIndex)
                _coloChunk.NightIndex = (byte) ColoNightIndex.Value;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //OH BOY D:<


            foreach (DZSChunkHeader chunk in _data.ChunkHeaders)
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

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        
    }
}
