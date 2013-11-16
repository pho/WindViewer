using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace WWActorEdit.Kazari.DZx
{
    public partial class MULTControl : UserControl
    {
        MULT Actor;

        public MULTControl(MULT ThisActor)
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

            numericUpDown1.Value = (decimal)Actor.Translation.X;
            numericUpDown2.Value = (decimal)Actor.Translation.Y;
            numericUpDown3.Value = (decimal)Actor.Rotation;
            numericUpDown4.Value = (decimal)Actor.RoomNumber;
            numericUpDown5.Value = (decimal)Actor.Unknown2;

            AttachDetachEvents(true);
        }

        void AttachDetachEvents(bool Attach)
        {
            if (Attach == true)
            {
                numericUpDown1.ValueChanged += new EventHandler(numericUpDown1_ValueChanged);
                numericUpDown2.ValueChanged += new EventHandler(numericUpDown2_ValueChanged);
                numericUpDown3.ValueChanged += new EventHandler(numericUpDown3_ValueChanged);
                numericUpDown4.ValueChanged += new EventHandler(numericUpDown4_ValueChanged);
                numericUpDown5.ValueChanged += new EventHandler(numericUpDown5_ValueChanged);
                 
            }
            else
            {
                numericUpDown1.ValueChanged -= new EventHandler(numericUpDown1_ValueChanged);
                numericUpDown2.ValueChanged -= new EventHandler(numericUpDown2_ValueChanged);
                numericUpDown3.ValueChanged -= new EventHandler(numericUpDown3_ValueChanged);
                numericUpDown4.ValueChanged -= new EventHandler(numericUpDown4_ValueChanged);
                numericUpDown5.ValueChanged -= new EventHandler(numericUpDown5_ValueChanged);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Actor.Translation = new Vector2((float)numericUpDown1.Value, Actor.Translation.Y);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Actor.Translation = new Vector2(Actor.Translation.X, (float)numericUpDown2.Value);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Actor.Rotation = (float)numericUpDown3.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Actor.RoomNumber = (byte)numericUpDown4.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Actor.Unknown2 = (byte)numericUpDown5.Value;
        }
    }
}
