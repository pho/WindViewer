using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WWActorEdit.Kazari.DZx
{
    public partial class SHIPControl : UserControl
    {
        SHIP Ship;

        public SHIPControl(SHIP ThisShip)
        {
            InitializeComponent();

            Ship = ThisShip;

            this.SuspendLayout();
            UpdateControl();
            this.ResumeLayout();
        }

        void UpdateControl()
        {
            AttachDetachEvents(false);

            numericUpDown1.Value = (decimal)Ship.Position.X;
            numericUpDown2.Value = (decimal)Ship.Position.Y;
            numericUpDown3.Value = (decimal)Ship.Position.Z;
            textBox1.Text = Ship.Unknown.ToString("X8");
            
            AttachDetachEvents(true);
        }

        void AttachDetachEvents(bool Attach)
        {
            if (Attach == true)
            {
                numericUpDown1.ValueChanged += new EventHandler(numericUpDown1_ValueChanged);
                numericUpDown2.ValueChanged += new EventHandler(numericUpDown2_ValueChanged);
                numericUpDown3.ValueChanged += new EventHandler(numericUpDown3_ValueChanged);
                textBox1.TextChanged += new EventHandler(textBox1_TextChanged);
            }
            else
            {
                numericUpDown1.ValueChanged -= new EventHandler(numericUpDown1_ValueChanged);
                numericUpDown2.ValueChanged -= new EventHandler(numericUpDown2_ValueChanged);
                numericUpDown3.ValueChanged -= new EventHandler(numericUpDown3_ValueChanged);
                textBox1.TextChanged -= new EventHandler(textBox1_TextChanged);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Ship.Position = new OpenTK.Vector3((float)numericUpDown1.Value, Ship.Position.Y, Ship.Position.Z);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Ship.Position = new OpenTK.Vector3(Ship.Position.X, (float)numericUpDown2.Value, Ship.Position.Z);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Ship.Position = new OpenTK.Vector3(Ship.Position.X, Ship.Position.Y, (float)numericUpDown3.Value);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.TextLength == textBox1.MaxLength)
                Ship.Unknown = uint.Parse(textBox1.Text, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
