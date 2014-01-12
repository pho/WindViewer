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

        public EnvironmentLightingEditorForm(MainForm parent)
        {
            InitializeComponent();

            _mainForm = parent;
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

            dzsFileDropdown.SelectedIndex = 0;
        }
    }
}
