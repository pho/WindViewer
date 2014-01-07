using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Globalization;

using WWActorEdit.Kazari;
using WWActorEdit.Kazari.DZx;
using WWActorEdit.Kazari.DZB;
using WWActorEdit.Kazari.J3Dx;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace WWActorEdit.Kazari
{
    public partial class DZxControl : UserControl
    {
        DZx.DZx Actor;

        public DZxControl(DZx.DZx ThisActor)
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
           
            
            AttachDetachEvents(true);
        }

        void AttachDetachEvents(bool Attach)
        {
            if (Attach == true)
            {
            }
            else
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Actor.AddChunk(textBox1.Text);
        }
    }
}
