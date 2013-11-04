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
    public partial class RPPNControl : UserControl
    {
        RPPN Point;

        public RPPNControl(RPPN ThisPoint)
        {
            InitializeComponent();

            Point = ThisPoint;

            this.SuspendLayout();
            UpdateControl();
            this.ResumeLayout();
        }

        void UpdateControl()
        {
            AttachDetachEvents(false);

            textBox1.Text = Point.Unknown.ToString("X8");
            numericUpDown1.Value = (decimal)Point.Position.X;
            numericUpDown2.Value = (decimal)Point.Position.Y;
            numericUpDown3.Value = (decimal)Point.Position.Z;

            AttachDetachEvents(true);
        }

        void AttachDetachEvents(bool Attach)
        {
            if (Attach == true)
            {
                textBox1.TextChanged += new EventHandler(textBox1_TextChanged);
                numericUpDown1.ValueChanged += new EventHandler(numericUpDown1_ValueChanged);
                numericUpDown2.ValueChanged += new EventHandler(numericUpDown2_ValueChanged);
                numericUpDown3.ValueChanged += new EventHandler(numericUpDown3_ValueChanged);
            }
            else
            {
                textBox1.TextChanged -= new EventHandler(textBox1_TextChanged);
                numericUpDown1.ValueChanged -= new EventHandler(numericUpDown1_ValueChanged);
                numericUpDown2.ValueChanged -= new EventHandler(numericUpDown2_ValueChanged);
                numericUpDown3.ValueChanged -= new EventHandler(numericUpDown3_ValueChanged);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.TextLength == textBox1.MaxLength)
                Point.Unknown = uint.Parse(textBox1.Text, System.Globalization.NumberStyles.HexNumber);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Point.Position = new OpenTK.Vector3((float)numericUpDown1.Value, Point.Position.Y, Point.Position.Z);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Point.Position = new OpenTK.Vector3(Point.Position.X, (float)numericUpDown2.Value, Point.Position.Z);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Point.Position = new OpenTK.Vector3(Point.Position.X, Point.Position.Y, (float)numericUpDown3.Value);
        }
    }
}
