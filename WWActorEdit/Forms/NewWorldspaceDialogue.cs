using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWActorEdit.Forms
{
    public partial class NewWorldspaceDialogue : Form
    {
        public NewWorldspaceDialogue()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/pho/WindViewer/wiki/Terms-to-Know");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void dirName_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox) sender;
            okButton.Enabled = tb.Text.Length > 0;
        }
    }
}
