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

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using WWActorEdit.Kazari;
using WWActorEdit.Kazari.DZx;
using WWActorEdit.Kazari.DZB;
using WWActorEdit.Kazari.J3Dx;

namespace WWActorEdit
{
    public partial class MainForm : Form
    {
        List<ZeldaArc> Rooms = new List<ZeldaArc>();
        ZeldaArc Stage;

        IDZxChunkElement SelectedDZRChunkElement;

        bool GLReady = false;
        bool Wait = false;

        bool[] KeysDown = new bool[256];
        Helpers.MouseStruct Mouse = new Helpers.MouseStruct();

        public MainForm()
        {
            //Initialize the WinForm
            InitializeComponent();
         }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl.IsIdle == true)
            {
                glControl_Paint(this, new PaintEventArgs(glControl.CreateGraphics(), glControl.ClientRectangle));
            }
        }

        void glControl_Load(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(Application_Idle);

            Helpers.Enable3DRendering(new SizeF(glControl.Width, glControl.Height));

            GLReady = true;
        }

        void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (GLReady == false) return;

            GL.ClearColor(Color.FromArgb(255, 51, 128, 179));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Helpers.Enable3DRendering(new SizeF(glControl.Width, glControl.Height));

            Helpers.Camera.Position();

            if (Wait == false)
            {
                GL.Scale(0.005f, 0.005f, 0.005f);

                /* Models */
                if (renderModelsToolStripMenuItem.Checked == true)
                {
                    foreach (ZeldaArc A in Rooms)
                    {
                        GL.PushMatrix();
                        GetGlobalTranslation(A);
                        GetGlobalRotation(A);
                        /* WRONG! */
                        /*
                        foreach (DZx D in A.DZRs)
                        {
                            foreach (DZx.FileChunk Chunk in D.Chunks)
                            {
                                foreach (IDZxChunkElement ChunkElement in Chunk.Data.Where(C => C is LGTV))
                                {
                                    LGTV L = (LGTV)ChunkElement;
                                    GL.Rotate((L.Unknown1.X / 1000.0) + 10.0, 1.0, 1.0, 0.0);
                                    GL.Rotate((L.Unknown1.Y / 1000.0) + 10.0, 0.0, 1.0, 0.0);
                                    GL.Rotate((L.Unknown1.Z / 1000.0) + 10.0, 0.0, 0.0, 1.0);
                                    GL.Scale(L.Unknown2);
                                }
                            }
                        }
                        */
                        foreach (J3Dx M in A.J3Dxs)
                        {
                            /* Got model translation from Stage? (ex. rooms in sea) */
                            if (A.GlobalTranslation != Vector3.Zero || A.GlobalRotation != 0)
                            {
                                /* Perform translation */
                                GL.Translate(A.GlobalTranslation);
                                GL.Rotate(A.GlobalRotation, 0, 1, 0);
                            }
                            M.Render();
                        }
                        GL.PopMatrix();
                    }
                }

                /* Actors, 1st pass */
                if (renderRoomActorsToolStripMenuItem.Checked == true)
                {
                    foreach (ZeldaArc A in Rooms)
                    {
                        if (A.DZRs != null) foreach (DZx D in A.DZRs) D.Render();
                        if (A.DZSs != null) foreach (DZx D in A.DZSs) D.Render();
                    }
                }
                if (renderStageActorsToolStripMenuItem.Checked == true && Stage != null)
                {
                    if (Stage.DZRs != null) foreach (DZx D in Stage.DZRs) D.Render();
                    if (Stage.DZSs != null) foreach (DZx D in Stage.DZSs) D.Render();
                }

                /* Collision */
                if (renderCollisionToolStripMenuItem.Checked == true)
                    foreach (ZeldaArc A in Rooms) foreach (DZB D in A.DZBs) D.Render();

                /* Actors, 2nd pass */
                if (renderRoomActorsToolStripMenuItem.Checked == true)
                {
                    foreach (ZeldaArc A in Rooms)
                    {
                        if (A.DZRs != null) foreach (DZx D in A.DZRs) D.Render();
                        if (A.DZSs != null) foreach (DZx D in A.DZSs) D.Render();
                    }
                }
                if (renderStageActorsToolStripMenuItem.Checked == true && Stage != null)
                {
                    if (Stage.DZRs != null) foreach (DZx D in Stage.DZRs) D.Render();
                    if (Stage.DZSs != null) foreach (DZx D in Stage.DZSs) D.Render();
                }

                Helpers.Camera.KeyUpdate(KeysDown);
            }

            glControl.SwapBuffers();
        }

        void glControl_Resize(object sender, EventArgs e)
        {
            if (GLReady == false) return;

            Helpers.Enable3DRendering(new SizeF(glControl.Width, glControl.Height));
            glControl.Invalidate();
        }

        void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            KeysDown[e.KeyValue] = true;
        }

        void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            KeysDown[e.KeyValue] = false;
        }

        void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Mouse.LDown = true;
            else if (e.Button == MouseButtons.Right)
                Mouse.RDown = true;
            else if (e.Button == MouseButtons.Middle)
                Mouse.MDown = true;

            Mouse.Center = new Vector2(e.X, e.Y);

            if (Mouse.LDown == true)
            {
                if (Mouse.Center != Mouse.Move)
                    Helpers.Camera.MouseMove(Mouse.Move);
                else
                    Helpers.Camera.MouseCenter(Mouse.Move);
            }
        }

        void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Move = new Vector2(e.X, e.Y);

            if (Mouse.LDown == true)
            {
                if (Mouse.Center != Mouse.Move)
                    Helpers.Camera.MouseMove(Mouse.Move);
                else
                    Helpers.Camera.MouseCenter(Mouse.Move);
            }
        }

        void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Mouse.LDown = false;
            else if (e.Button == MouseButtons.Right)
                Mouse.RDown = false;
            else if (e.Button == MouseButtons.Middle)
                Mouse.MDown = false;
        }

        private void GetRoomNumber(ZeldaArc NewArc)
        {
            /* Very shitty - hell, I even use the VB interaction stuff out of laziness! */

            int RNumb = 0;

        start:
            /* Alright, let's try this! */
            try
            {
                /* First try to guess the roomnumber from the filename... */
                /* Hm, is it even a room? */
                if (Path.GetFileName(NewArc.Filename).Substring(0, 4).ToLower() != "room" &&    /* ...WW? */
                    Path.GetFileName(NewArc.Filename).Substring(0, 1) != "R")                   /* ...or maybe TP? */
                    goto manual;
                else
                {
                    /* Any numbers in there...? */
                    string[] numbers = Regex.Split(Path.GetFileName(NewArc.Filename), @"\D+");
                    foreach (string n in numbers)
                    {
                        if (n != string.Empty)
                        {
                            /* Yay, there's a number, fuck it let's use this! */
                            RNumb = int.Parse(n);
                            goto cont;
                        }
                        else
                            continue;
                    }
                    goto manual;        /* Wha? Nothing found? Gotta let the user do his thing... */
                }
            }
            catch
            {
                goto manual;
            }

        manual:
            {
                /* Alright VB interaction time! Get the inputbox up... */
                string Number = Microsoft.VisualBasic.Interaction.InputBox("Could not determine room number. Please enter the number manually.");
                if (Number == string.Empty) return;

                try
                {
                    /* Trying to get the number from the string */
                    RNumb = int.Parse(Number);
                    goto cont;
                }
                catch (FormatException)
                {
                    /* Ah for fucks sake, the box is asking for a NUMBER */
                    goto start;
                }
            }

        cont:
            /* We somehow got a number! But is it correct? Hell if I know */
            NewArc.RoomNumber = RNumb;
        }

        private void GetGlobalTranslation(ZeldaArc A)
        {
            if (Stage != null)
            {
                foreach (DZx D in Stage.DZSs)
                {
                    foreach (DZx.FileChunk Chunk in D.Chunks)
                    {
                        foreach (IDZxChunkElement ChunkElement in Chunk.Data.Where(C => C is MULT && ((MULT)C).RoomNumber == A.RoomNumber))
                        {
                            A.GlobalTranslation = new Vector3(((MULT)ChunkElement).Translation.X, 0.0f, ((MULT)ChunkElement).Translation.Y);
                        }
                    }
                }
            }
        }

        private void GetGlobalRotation(ZeldaArc A)
        {
            if (Stage != null)
            {
                foreach (DZx D in Stage.DZSs)
                {
                    foreach (DZx.FileChunk Chunk in D.Chunks)
                    {
                        foreach (IDZxChunkElement ChunkElement in Chunk.Data.Where(C => C is MULT && ((MULT)C).RoomNumber == A.RoomNumber))
                        {
                            A.GlobalRotation = ((MULT)ChunkElement).Rotation;
                        }
                    }
                }
            }
        }

        private void LoadRARC(string Filename, bool IsRoom = true, bool IgnoreModels = false)
        {
            if (Filename != string.Empty)
            {
                ZeldaArc NewArc = new ZeldaArc(Filename, treeView1, IgnoreModels);

                if (IsRoom == true)
                {
                    GetRoomNumber(NewArc);
                    GetGlobalTranslation(NewArc);
                    GetGlobalRotation(NewArc);
                    Rooms.Add(NewArc);
                }
                else
                    Stage = NewArc;

                if (NewArc.Archive.IsCompressed == true)
                    MessageBox.Show(string.Format("RARC archive '{0}' is compressed; changes cannot be saved!", Path.GetFileName(NewArc.Archive.Filename)), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    saveChangesToolStripMenuItem.Enabled = true;
            }
        }

        private void UnloadAllRARCs()
        {
            foreach (ZeldaArc A in Rooms) A.Clear();
            if (Stage != null) Stage.Clear();

            Rooms = new List<ZeldaArc>();
            Stage = null;

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.EndUpdate();

            saveChangesToolStripMenuItem.Enabled = false;

            this.Text = Application.ProductName;
            toolStripStatusLabel1.Text = "Ready";
        }

        private void openRoomRARCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] Files = Helpers.ShowOpenFileDialog("GameCube/Wii RARC archives (*.arc; *.rarc)|*.arc; *.rarc|All Files (*.*)|*.*", true);
            Array.Sort(Files);

            if (Files.Length == 1 && Files[0] == string.Empty) return;

            Helpers.MassEnableDisable(this.Controls, false);
            Wait = true;

            if (Files.Length > 5 && (renderRoomActorsToolStripMenuItem.Checked == true || renderStageActorsToolStripMenuItem.Checked == true))
            {
                if (MessageBox.Show("Rendering 5+ rooms while actor rendering is enabled might very likely cause heavy slowdown. Disable actor rendering?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                {
                    renderRoomActorsToolStripMenuItem.Checked = false;
                    renderStageActorsToolStripMenuItem.Checked = false;
                }
            }

            treeView1.BeginUpdate();

            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Maximum = Files.Length;

            foreach (string F in Files)
            {
                toolStripStatusLabel1.Text = string.Format("Loading '{0}'...", Path.GetFileName(F));
                LoadRARC(F, true);
                toolStripProgressBar1.PerformStep();
                Application.DoEvents();
            }

            toolStripProgressBar1.Visible = false;

            treeView1.EndUpdate();

            toolStripStatusLabel1.Text = string.Format("Loaded {0} room files. Ready!", Files.Length);

            Helpers.MassEnableDisable(this.Controls, true);
            Wait = false;
        }

        private void openStageRARCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] Files = Helpers.ShowOpenFileDialog("GameCube/Wii RARC archives (*.arc; *.rarc)|*.arc; *.rarc|All Files (*.*)|*.*");
            if (Files[0] == string.Empty) return;

            LoadRARC(Files[0], false);

            toolStripStatusLabel1.Text = "Loaded stage file. Ready!";

            foreach (ZeldaArc A in Rooms)
            {
                GetGlobalTranslation(A);
                GetGlobalRotation(A);
            }
        }

        private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ZeldaArc A in Rooms) A.Save();
            if (Stage != null) Stage.Save();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Helpers.Camera.Initialize(new Vector3(0.0f, 0.0f, -5.0f));
            UnloadAllRARCs();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object Selected = ((TreeView)sender).SelectedNode.Tag;

            if (SelectedDZRChunkElement != null)
            {
                SelectedDZRChunkElement.Highlight = false;
                if (SelectedDZRChunkElement.EditControl != null)
                    SelectedDZRChunkElement.EditControl.Dispose();
            }

            Panel TargetPanel = (Selected is IDZxChunkElement && (IDZxChunkElement)Selected is Generic) ? panel2 : panel1;
            Panel OtherPanel = (Selected is IDZxChunkElement && (IDZxChunkElement)Selected is Generic) ? panel1 : panel2;
            OtherPanel.Visible = false;

            TargetPanel.SuspendLayout();

            if (Selected is IDZxChunkElement)
            {
                SelectedDZRChunkElement = (IDZxChunkElement)Selected;

                SelectedDZRChunkElement.Highlight = true;
                SelectedDZRChunkElement.ShowControl(TargetPanel);

                if (autoCenterCameraToolStripMenuItem.Checked && (SelectedDZRChunkElement is Generic) == false)
                {
                    Vector3 CamPos = Vector3.Zero;
                    if (SelectedDZRChunkElement is ACTR)
                        CamPos = (-((ACTR)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is RPPN)
                        CamPos = (-((RPPN)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is SHIP)
                        CamPos = (-((SHIP)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is TGDR)
                        CamPos = (-((TGDR)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is TRES)
                        CamPos = (-((TRES)SelectedDZRChunkElement).Position * 0.005f);
                    else if (SelectedDZRChunkElement is MULT)
                        CamPos = (-(new Vector3(((MULT)SelectedDZRChunkElement).Translation.X, 0.0f, ((MULT)SelectedDZRChunkElement).Translation.Y) * 0.005f));

                    Helpers.Camera.Initialize(CamPos - new Vector3(0, 0, 2.0f));
                }
            }
            else
                TargetPanel.Visible = false;

            TargetPanel.ResumeLayout();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Version AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime BuildDate = new DateTime(2000, 1, 1).AddDays(AppVersion.Build).AddSeconds(AppVersion.Revision * 2);

            MessageBox.Show(
                Application.ProductName + " - Written 2012 by xdaniel - Build " + BuildDate.ToString(CultureInfo.InvariantCulture) + Environment.NewLine +
                Environment.NewLine +
                "RARC, Yaz0 and J3Dx/BMD documentation by thakis" + Environment.NewLine +
                "DZB and DZx documentation by Sage of Mirrors, Twili, fkualol, xdaniel, et al." + Environment.NewLine +
                Environment.NewLine +
                "Greetings to The GCN's WW hacking thread!",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void showReadmeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Well, there's no Readme yet actually... Some points of interest:" + Environment.NewLine +
                Environment.NewLine +
                "- While moving the camera around, hold Space to speed it up, or Shift to slow it down (good idea from Kargaroc)" + Environment.NewLine +
                "- Saving is not yet fully tested; it worked for me and I couldn't spot any obvious errors in the code but still, tread carefully" + Environment.NewLine +
                "- In case an exception occurs, my exception handler should pop up instead of Windows' mean dialog box; press Ctrl+C" + Environment.NewLine +
                "  there to copy everything into the clipboard, then message me or post that on The GCN or whatever" + Environment.NewLine +
                "- The icon was made by Sage of Mirrors, thanks!" + Environment.NewLine +
                "- I'm thinking of posting the source code for this as well, but I haven't decided yet; I'd rather first write some" + Environment.NewLine +
                "  documentation about WW's file formats, so that the source makes more sense and such" + Environment.NewLine +
                "- Here's hoping I didn't forget anyone in the About box credits :P",
                "Readme?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
