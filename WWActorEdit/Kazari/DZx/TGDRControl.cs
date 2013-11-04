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
    public partial class TGDRControl : UserControl
    {
        TGDR Door;

        public TGDRControl(TGDR ThisDoor)
        {
            InitializeComponent();

            Door = ThisDoor;

            this.SuspendLayout();
            UpdateControl();
            this.ResumeLayout();
        }

        void UpdateControl()
        {
            AttachDetachEvents(false);

            textBox1.Text = Door.Name;
            textBox2.Text = Door.Parameters.ToString("X8");
            numericUpDown1.Value = (decimal)Door.Position.X;
            numericUpDown2.Value = (decimal)Door.Position.Y;
            numericUpDown3.Value = (decimal)Door.Position.Z;
            textBox3.Text = Door.Unknown1.ToString("X4");
            numericUpDown4.Value = (decimal)Door.RotationY;
            textBox4.Text = Door.Unknown2.ToString("X4");
            textBox5.Text = Door.Unknown3.ToString("X4");
            textBox6.Text = Door.Unknown4.ToString("X8");

            AttachDetachEvents(true);
        }

        void AttachDetachEvents(bool Attach)
        {
            if (Attach == true)
            {
                textBox1.TextChanged += new EventHandler(textBox1_TextChanged);
                textBox2.TextChanged += new EventHandler(textBox2_TextChanged);
                numericUpDown1.ValueChanged += new EventHandler(numericUpDown1_ValueChanged);
                numericUpDown2.ValueChanged += new EventHandler(numericUpDown2_ValueChanged);
                numericUpDown3.ValueChanged += new EventHandler(numericUpDown3_ValueChanged);
                textBox3.TextChanged += new EventHandler(textBox3_TextChanged);
                numericUpDown4.ValueChanged += new EventHandler(numericUpDown4_ValueChanged);
                textBox4.TextChanged += new EventHandler(textBox4_TextChanged);
                textBox5.TextChanged += new EventHandler(textBox5_TextChanged);
                textBox6.TextChanged += new EventHandler(textBox6_TextChanged);
            }
            else
            {
                textBox1.TextChanged -= new EventHandler(textBox1_TextChanged);
                textBox2.TextChanged -= new EventHandler(textBox2_TextChanged);
                numericUpDown1.ValueChanged -= new EventHandler(numericUpDown1_ValueChanged);
                numericUpDown2.ValueChanged -= new EventHandler(numericUpDown2_ValueChanged);
                numericUpDown3.ValueChanged -= new EventHandler(numericUpDown3_ValueChanged);
                textBox3.TextChanged -= new EventHandler(textBox3_TextChanged);
                numericUpDown4.ValueChanged -= new EventHandler(numericUpDown4_ValueChanged);
                textBox4.TextChanged -= new EventHandler(textBox4_TextChanged);
                textBox5.TextChanged -= new EventHandler(textBox5_TextChanged);
                textBox6.TextChanged -= new EventHandler(textBox6_TextChanged);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Door.Name = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.TextLength == textBox2.MaxLength)
                Door.Parameters = uint.Parse(textBox2.Text, System.Globalization.NumberStyles.HexNumber);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Door.Position = new OpenTK.Vector3((float)numericUpDown1.Value, Door.Position.Y, Door.Position.Z);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Door.Position = new OpenTK.Vector3(Door.Position.X, (float)numericUpDown2.Value, Door.Position.Z);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Door.Position = new OpenTK.Vector3(Door.Position.X, Door.Position.Y, (float)numericUpDown3.Value);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.TextLength == textBox3.MaxLength)
                Door.Unknown1 = ushort.Parse(textBox3.Text, System.Globalization.NumberStyles.HexNumber);
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Door.RotationY = (double)numericUpDown4.Value;
        }

        void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.TextLength == textBox4.MaxLength)
                Door.Unknown2 = ushort.Parse(textBox4.Text, System.Globalization.NumberStyles.HexNumber);
        }

        void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.TextLength == textBox5.MaxLength)
                Door.Unknown3 = ushort.Parse(textBox5.Text, System.Globalization.NumberStyles.HexNumber);
        }

        void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.TextLength == textBox6.MaxLength)
                Door.Unknown4 = uint.Parse(textBox6.Text, System.Globalization.NumberStyles.HexNumber);
        }

    }
}
