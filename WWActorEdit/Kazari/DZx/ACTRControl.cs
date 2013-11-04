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
    public partial class ACTRControl : UserControl
    {
        ACTR Actor;

        public ACTRControl(ACTR ThisActor)
        {
            InitializeComponent();

            Actor = ThisActor;

            this.SuspendLayout();
            UpdateControl();
            this.ResumeLayout();
        }

        void UpdateControl()
        {
            AttachDetachEvents(false);

            textBox1.Text = Actor.Name;
            textBox2.Text = Actor.Parameters.ToString("X8");
            numericUpDown1.Value = (decimal)Actor.Position.X;
            numericUpDown2.Value = (decimal)Actor.Position.Y;
            numericUpDown3.Value = (decimal)Actor.Position.Z;
            numericUpDown4.Value = (decimal)Actor.Rotation.X;
            numericUpDown5.Value = (decimal)Actor.Rotation.Y;
            numericUpDown6.Value = (decimal)Actor.Rotation.Z;
            textBox3.Text = Actor.Unknown.ToString("X4");

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
                numericUpDown4.ValueChanged += new EventHandler(numericUpDown4_ValueChanged);
                numericUpDown5.ValueChanged += new EventHandler(numericUpDown5_ValueChanged);
                numericUpDown6.ValueChanged += new EventHandler(numericUpDown6_ValueChanged);
                textBox3.TextChanged += new EventHandler(textBox3_TextChanged);
            }
            else
            {
                textBox1.TextChanged -= new EventHandler(textBox1_TextChanged);
                textBox2.TextChanged -= new EventHandler(textBox2_TextChanged);
                numericUpDown1.ValueChanged -= new EventHandler(numericUpDown1_ValueChanged);
                numericUpDown2.ValueChanged -= new EventHandler(numericUpDown2_ValueChanged);
                numericUpDown3.ValueChanged -= new EventHandler(numericUpDown3_ValueChanged);
                numericUpDown4.ValueChanged -= new EventHandler(numericUpDown4_ValueChanged);
                numericUpDown5.ValueChanged -= new EventHandler(numericUpDown5_ValueChanged);
                numericUpDown6.ValueChanged -= new EventHandler(numericUpDown6_ValueChanged);
                textBox3.TextChanged -= new EventHandler(textBox3_TextChanged);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Actor.Name = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.TextLength == textBox2.MaxLength)
                Actor.Parameters = uint.Parse(textBox2.Text, System.Globalization.NumberStyles.HexNumber);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Actor.Position = new OpenTK.Vector3((float)numericUpDown1.Value, Actor.Position.Y, Actor.Position.Z);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Actor.Position = new OpenTK.Vector3(Actor.Position.X, (float)numericUpDown2.Value, Actor.Position.Z);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Actor.Position = new OpenTK.Vector3(Actor.Position.X, Actor.Position.Y, (float)numericUpDown3.Value);
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Actor.Rotation = new OpenTK.Vector3((float)numericUpDown4.Value, Actor.Rotation.Y, Actor.Rotation.Z);
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Actor.Rotation = new OpenTK.Vector3(Actor.Rotation.X, (float)numericUpDown5.Value, Actor.Rotation.Z);
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            Actor.Rotation = new OpenTK.Vector3(Actor.Rotation.X, Actor.Rotation.Y, (float)numericUpDown6.Value);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.TextLength == textBox3.MaxLength)
                Actor.Unknown = ushort.Parse(textBox3.Text, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
