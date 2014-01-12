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

        private void LoadPaleElement()
        {
            //Need to find the ColoChunk again, and get the right index.
            foreach (var header in _data.ChunkHeaders)
            {
                if (header.Tag == "Pale")
                {
                    _paleChunk = (PaleChunk)header.ChunkElements[PaleDropdown.SelectedIndex];
                    break;
                }
            }
        }

        private void LoadVirtElement()
        {
            //Need to find the ColoChunk again, and get the right index.
            foreach (var header in _data.ChunkHeaders)
            {
                if (header.Tag == "Virt")
                {
                    _virtChunk = (VirtChunk)header.ChunkElements[VirtDropdown.SelectedIndex];
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
            PaleRoomAmbientColor.BackColor = SetPaleColorBoxColor(_paleChunk.RoomAmbient);
            PaleWaveColor.BackColor = SetPaleColorBoxColor(_paleChunk.WaveColor);
            PaleOceanColor.BackColor = SetPaleColorBoxColor(_paleChunk.OceanColor);
            PaleDoorwayColor.BackColor = SetPaleColorBoxColor(_paleChunk.DoorwayColor);
            PaleFogColor.BackColor = SetPaleColorBoxColor(_paleChunk.FogColor);

            PaleVirtIndex.Value = _paleChunk.VirtIndex;
            PaleOceanFadeIntoColor.BackColor = SetPaleColorBoxColor(_paleChunk.OceanFadeInto);
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

        private void PaleDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPaleElement();
            UpdatePaleGroupBox();
        }

        private void VirtDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadVirtElement();
            UpdateVirtGroupBox();
        }

        /// <summary>
        /// Called when ANY of the color fields in Pale are clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaleColorField_Click(object sender, EventArgs e)
        {
            PictureBox outputBox = (PictureBox) sender;

            colorPickerDialog.Color = outputBox.BackColor;
            colorPickerDialog.ShowDialog(this);

            outputBox.BackColor = colorPickerDialog.Color;

            //Now the fun part. We've modified the PictureBox's color, but we don't know which value that actually refers to in the Pale memor.
            //I could try and write something using metadata and looking up the value and get all complicated... but I think I'll just re-assign 
            //all of the Pale color boxes to the Pale memory (ie: reverse of loading it). Hacky? Yes. Lazy? Yes. Works? Yes.
            _paleChunk.ActorAmbient = SetPaleMemoryColor(PaleActorAmbientColor);
            _paleChunk.RoomAmbient = SetPaleMemoryColor(PaleRoomAmbientColor);
            _paleChunk.WaveColor = SetPaleMemoryColor(PaleWaveColor);
            _paleChunk.OceanColor = SetPaleMemoryColor(PaleOceanColor);
            _paleChunk.DoorwayColor = SetPaleMemoryColor(PaleDoorwayColor);
            _paleChunk.FogColor = SetPaleMemoryColor(PaleFogColor);

            _paleChunk.OceanFadeInto = SetPaleMemoryColor(PaleOceanFadeIntoColor);
        }

        private void VirtColorField_Click(object sender, EventArgs e)
        {
            PictureBox outputBox = (PictureBox)sender;

            colorPickerDialog.Color = outputBox.BackColor;
            colorPickerDialog.ShowDialog(this);

            outputBox.BackColor = colorPickerDialog.Color;

            //Hey... same hack as above, because I didn't think of any better way to do it in the last 20 minutes...
            ByteColorAlpha HorizonCloud = new ByteColorAlpha(SetPaleMemoryColor(VirtHorizonCloudColor));
            HorizonCloud.A = (byte)VirtUnknown1Index.Value;
            _virtChunk.HorizonCloudColor = HorizonCloud;

            ByteColorAlpha CenterCloud = new ByteColorAlpha(SetPaleMemoryColor(VirtCenterCloudColor));
            CenterCloud.A = (byte)VirtUnknown2Index.Value;
            _virtChunk.CenterCloudColor = CenterCloud;

            _virtChunk.CenterSkyColor = SetPaleMemoryColor(VirtCenterSkyColor);
            _virtChunk.HorizonColor = SetPaleMemoryColor(VirtHorizonColor);
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
        
    }
}
