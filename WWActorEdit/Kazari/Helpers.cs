using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace WWActorEdit.Kazari
{
    public static class Helpers
    {
        public static void Swap(ref byte V1, ref byte V2)
        {
            byte Tmp = V1; V1 = V2; V2 = Tmp;
        }

        public static void Swap(ref int V1, ref int V2)
        {
            int Tmp = V1; V1 = V2; V2 = Tmp;
        }

        public static byte Read8(byte[] Data, int Offset)
        {
            return (Buffer.GetByte(Data, Offset));
        }

        public static ushort Read16(byte[] Data, int Offset)
        {
            return (ushort)((Buffer.GetByte(Data, Offset) << 8) | Buffer.GetByte(Data, Offset + 1));
        }

        public static uint Read32(byte[] Data, int Offset)
        {
            return (uint)((Buffer.GetByte(Data, Offset) << 24) | (Buffer.GetByte(Data, Offset + 1) << 16) | (Buffer.GetByte(Data, Offset + 2) << 8) | Buffer.GetByte(Data, Offset + 3));
        }

        public static ushort Read16Swap(byte[] Data, int Offset)
        {
            return (ushort)((Buffer.GetByte(Data, Offset + 1) << 8) | Buffer.GetByte(Data, Offset));
        }

        public static uint Read32Swap(byte[] Data, int Offset)
        {
            return (uint)((Buffer.GetByte(Data, Offset + 3) << 24) | (Buffer.GetByte(Data, Offset + 2) << 16) | (Buffer.GetByte(Data, Offset + 1) << 8) | Buffer.GetByte(Data, Offset));
        }

        public static byte[] LoadBinary(string Path)
        {
            BinaryReader BR = new BinaryReader(File.OpenRead(Path));
            byte[] Data = BR.ReadBytes((int)BR.BaseStream.Length);
            BR.Close();
            return Data;
        }

        public static void SaveBinary(string Path, byte[] Data)
        {
            BinaryWriter BW = new BinaryWriter(File.OpenWrite(Path));
            BW.Write(Data);
            BW.Close();
        }

        public static string ReadString(byte[] Data, ref int Offset)
        {
            if (Offset >= Data.Length) return null;
            while (Data[Offset] == 0) Offset++;
            int Length = Array.IndexOf(Data, (byte)0, Offset) - Offset;
            string ReturnString = ReadString(Data, Offset, Length);
            Offset += Length;
            return ReturnString;
        }

        public static string ReadString(byte[] Data, int Offset)
        {
            if (Offset >= Data.Length) return null;
            while (Data[Offset] == 0) Offset++;
            int Length = Array.IndexOf(Data, (byte)0, Offset) - Offset;
            return ReadString(Data, Offset, Length);
        }

        public static string ReadString(byte[] Data, int Offset, int Length)
        {
            if (Offset >= Data.Length) return null;
            while (Data[Offset] == 0) Offset++;
            byte[] TempBuffer = new Byte[Length + 1];
            Buffer.BlockCopy(Data, Offset, TempBuffer, 0, Length);
            return Encoding.GetEncoding(1251).GetString(TempBuffer, 0, Array.IndexOf(TempBuffer, (byte)0));
        }

        public static void Overwrite8(ref byte[] Data, int Offset, byte Value)
        {
            OverwriteXX(ref Data, Offset, Value, 0);
        }

        public static void Overwrite16(ref byte[] Data, int Offset, ushort Value)
        {
            OverwriteXX(ref Data, Offset, Value, 1);
        }

        public static void Overwrite32(ref byte[] Data, int Offset, uint Value)
        {
            OverwriteXX(ref Data, Offset, Value, 3);
        }

        public static void Overwrite64(ref byte[] Data, int Offset, ulong Value)
        {
            OverwriteXX(ref Data, Offset, Value, 7);
        }

        private static void OverwriteXX(ref byte[] Data, int Offset, ulong Value, int Shifts)
        {
            for (int i = Shifts; i >= 0; --i)
            {
                byte DataByte = (byte)((Value >> (i * 8)) & 0xFF);
                Data[Offset++] = DataByte;
            }
        }

        public static void WriteString(ref byte[] Data, int Offset, string Str)
        {
            WriteString(ref Data, Offset, Str, Str.Length);
        }

        public static void WriteString(ref byte[] Data, int Offset, string Str, int Length)
        {
            for (int i = 0; i < Length; i++)
                Data[Offset + i] = (i < Str.Length ? (byte)Str[i] : (byte)0);
        }

        public static float ConvertIEEE754Float(UInt32 Raw)
        {
            byte[] Data = new byte[4];
            for (int i = 0; i < 4; i++)
                Data[i] = (byte)((Raw >> (i * 8)) & 0xFF);
            return BitConverter.ToSingle(Data, 0);
        }

        public static void MassEnableDisable(Control.ControlCollection Ctrls, bool EnableState)
        {
            foreach (Control C in Ctrls) C.Enabled = EnableState;
        }

        public static string[] ShowOpenFileDialog(string Filter, bool AllowMultiSelect = false)
        {
            string[] SelectedFiles = new string[1];
            SelectedFiles[0] = string.Empty;

            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = Filter;
            OFD.Multiselect = AllowMultiSelect;
            OFD.CheckFileExists = OFD.CheckPathExists = true;

            if (OFD.ShowDialog() == DialogResult.OK)
                SelectedFiles = OFD.FileNames;

            return SelectedFiles;
        }

        public static SizeF MeasureString(string s, Font font)
        {
            SizeF result;
            using (var image = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(image))
                {
                    result = g.MeasureString(s, font);
                }
            }
            return result;
        }

        public static void Enable2DRendering(SizeF Viewport)
        {
            GL.Viewport(0, 0, (int)Viewport.Width, (int)Viewport.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            Matrix4 OrthoProjection = Matrix4.CreateOrthographicOffCenter(0, (float)Viewport.Width, (float)Viewport.Height, 0, -0.1f, 1.0f);
            GL.LoadMatrix(ref OrthoProjection);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
        }

        public static void Enable3DRendering(SizeF Viewport)
        {
            GL.Viewport(0, 0, (int)Viewport.Width, (int)Viewport.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 PerspMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), (float)Viewport.Width / (float)Viewport.Height, 0.01f, 10000.0f);
            GL.MultMatrix(ref PerspMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Enable(EnableCap.DepthTest);
        }

        public static void PrintText(Vector3 Position, Color TextColor, Font TextFont, string Text)
        {
            GL.Disable(EnableCap.Blend);
            GL.RasterPos3(Position);
            GL.PixelZoom(1.0f, -1.0f);

            SizeF TexImageSize = MeasureString(Text, TextFont);
            using (Bitmap TexImage = new Bitmap((int)TexImageSize.Width, (int)TexImageSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics Gfx = Graphics.FromImage(TexImage))
                {
                    Gfx.Clear(Color.Black);
                    Gfx.DrawString(Text, TextFont, new SolidBrush(TextColor), 0, 0);
                }

                BitmapData data = TexImage.LockBits(new Rectangle(0, 0, TexImage.Width, TexImage.Height), ImageLockMode.ReadOnly, TexImage.PixelFormat);
                GL.DrawPixels(TexImage.Width, TexImage.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                TexImage.UnlockBits(data);
            }
            GL.Enable(EnableCap.Blend);
        }

        public static void DrawFramedSphere(Vector3d c, double r, int n)
        {
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(1.0f, 1.0f);
            DrawSphere(c, r, n);
            GL.Disable(EnableCap.PolygonOffsetFill);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Color4(Color.Black);
            DrawSphere(c, r, n);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public static void DrawSphere(Vector3d c, double r, int n)
        {
            int i, j;
            double theta1, theta2, theta3;
            Vector3d e, p;

            if (r < 0)
                r = -r;
            if (n < 0)
                n = -n;
            if (n < 4 || r <= 0)
            {
                GL.Begin(BeginMode.Points);
                GL.Vertex3(c);
                GL.End();
                return;
            }

            const double TWOPI = (Math.PI * 2);
            const double PID2 = (Math.PI / 2);

            for (j = 0; j < n / 2; j++)
            {
                theta1 = j * TWOPI / n - PID2;
                theta2 = (j + 1) * TWOPI / n - PID2;

                GL.Begin(BeginMode.QuadStrip);
                for (i = 0; i <= n; i++)
                {
                    theta3 = i * TWOPI / n;

                    e.X = Math.Cos(theta2) * Math.Cos(theta3);
                    e.Y = Math.Sin(theta2);
                    e.Z = Math.Cos(theta2) * Math.Sin(theta3);
                    p.X = c.X + r * e.X;
                    p.Y = c.Y + r * e.Y;
                    p.Z = c.Z + r * e.Z;

                    GL.Normal3(e.X, e.Y, e.Z);
                    GL.TexCoord2(i / (double)n, 2 * (j + 1) / (double)n);
                    GL.Vertex3(p);

                    e.X = Math.Cos(theta1) * Math.Cos(theta3);
                    e.Y = Math.Sin(theta1);
                    e.Z = Math.Cos(theta1) * Math.Sin(theta3);
                    p.X = c.X + r * e.X;
                    p.Y = c.Y + r * e.Y;
                    p.Z = c.Z + r * e.Z;

                    GL.Normal3(e.X, e.Y, e.Z);
                    GL.TexCoord2(i / (double)n, 2 * j / (double)n);
                    GL.Vertex3(p);
                }
                GL.End();
            }
        }

        public static void DrawFramedCube(Vector3d Scale)
        {
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(1.0f, 1.0f);
            DrawCube(Scale);
            GL.Disable(EnableCap.PolygonOffsetFill);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Color4(Color.Black);
            DrawCube(Scale);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public static void DrawCube(Vector3d Scale)
        {
            GL.Disable(EnableCap.Texture2D);

            GL.PushMatrix();
            GL.Scale(Scale);

            GL.Begin(BeginMode.Quads);
            // Back Face
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, -1.0f);
            // Top Face
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, -1.0f);
            // Bottom Face
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 1.0f);
            // Right face
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 1.0f);
            // Left Face
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-1.0f, 1.0f, -1.0f);
            // Front Face
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.End();

            GL.PopMatrix();

            GL.Enable(EnableCap.Texture2D);
        }

        public static void DrawBox(Vector3 Min, Vector3 Max, Color Color)
        {
            GL.Disable(EnableCap.Texture2D);

            Vector3 Distance = Max - Min;
            Vector3[] Verts = new Vector3[8];

            Verts[0] = Min;
            Verts[1] = Min + new Vector3(Distance.X, 0, 0);
            Verts[2] = Min + new Vector3(Distance.X, Distance.Y, 0);
            Verts[3] = Min + new Vector3(0, Distance.Y, 0);

            Verts[4] = Min + new Vector3(0, 0, Distance.Z);
            Verts[5] = Min + new Vector3(Distance.X, 0, Distance.Z);
            Verts[6] = Min + new Vector3(Distance.X, Distance.Y, Distance.Z);
            Verts[7] = Min + new Vector3(0, Distance.Y, Distance.Z);

            GL.Color4(Color);

            GL.Begin(BeginMode.LineLoop);
            for (int i = 0; i < 4; i++)
                GL.Vertex3(Verts[i]);
            GL.End();

            GL.Begin(BeginMode.Lines);
            for (int i = 0; i < 4; i++)
            {
                GL.Vertex3(Verts[i]);
                GL.Vertex3(Verts[i + 4]);
            }
            GL.End();

            GL.Begin(BeginMode.LineLoop);
            for (int i = 0; i < 4; i++)
                GL.Vertex3(Verts[i + 4]);
            GL.End();

            GL.Enable(EnableCap.Texture2D);
        }

        public static TreeNode CreateTreeNode(string Text, object Tag, string Tooltip = "")
        {
            TreeNode NewNode = new TreeNode(Text);
            NewNode.Tag = Tag;
            NewNode.ToolTipText = Tooltip;
            return NewNode;
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        public static string ShortenPath(string Path, int Length)
        {
            StringBuilder SB = new StringBuilder(Length + 1);
            PathCompactPathEx(SB, Path, Length, 0);
            return SB.ToString();
        }

        public static class Camera
        {
            #region Variables

            public static float Speed = 0.01f;
            public static float CameraCoeff = 0.075f;
            public static Vector3 Pos, Rot;
            public static Vector2 MouseCoord;

            #endregion

            #region Camera Functions

            public static void Initialize(Vector3 InitPos = default(Vector3), Vector3 InitRot = default(Vector3))
            {
                Pos = InitPos; Rot = InitRot;
            }

            public static void MouseCenter(Vector2 NewMouseCoord)
            {
                MouseCoord = NewMouseCoord;
            }

            public static void MouseMove(Vector2 NewMouseCoord)
            {
                bool Changed = false;
                float Dx = 0.0f, Dy = 0.0f;

                if (NewMouseCoord.X != MouseCoord.X)
                {
                    Dx = (NewMouseCoord.X - MouseCoord.X) * Speed;
                    Changed = true;
                }
                if (NewMouseCoord.Y != MouseCoord.Y)
                {
                    Dy = (NewMouseCoord.Y - MouseCoord.Y) * Speed;
                    Changed = true;
                }

                if (Changed)
                {
                    if (MouseCoord.X < NewMouseCoord.X)
                    {
                        Rot.Y += (NewMouseCoord.X - MouseCoord.X) * (CameraCoeff * 5.0f);
                        if (Rot.Y > 360) Rot.Y = 0;
                    }
                    else
                    {
                        Rot.Y -= (MouseCoord.X - NewMouseCoord.X) * (CameraCoeff * 5.0f);
                        if (Rot.Y < -360) Rot.Y = 0;
                    }

                    if (MouseCoord.Y < NewMouseCoord.Y)
                    {
                        if (Rot.X >= 90)
                            Rot.X = 90;
                        else
                            Rot.X += (Dy / Speed) * (CameraCoeff * 5.0f);
                    }
                    else
                    {
                        if (Rot.X <= -90)
                            Rot.X = -90;
                        else
                            Rot.X += (Dy / Speed) * (CameraCoeff * 5.0f);
                    }
                }

                MouseCoord = NewMouseCoord;
            }

            public static void KeyUpdate(bool[] KeysDown)
            {
                float RotYRad = (Rot.Y / 180.0f * (float)Math.PI);
                float RotXRad = (Rot.X / 180.0f * (float)Math.PI);

                float Modifier = 1.0f;
                bool twoD = false;

                if (KeysDown[(char)Keys.Space])
                    Modifier = 10.0f;
                else if (KeysDown[(char)Keys.ShiftKey])
                    Modifier = 0.3f;
                if (KeysDown[(char)Keys.ControlKey])
                    twoD = true;

                if (KeysDown[(char)Keys.W]) {
                    if (twoD) {
                        Pos.Y -= CameraCoeff * 2.0f * Modifier;
                    } else {
                        if (Rot.X >= 90.0f || Rot.X <= -90.0f) {
                            Pos.Y += (float)Math.Sin(RotXRad) * CameraCoeff * 2.0f * Modifier;
                        } else {
                            Pos.X -= (float)Math.Sin(RotYRad) * CameraCoeff * 2.0f * Modifier;
                            Pos.Z += (float)Math.Cos(RotYRad) * CameraCoeff * 2.0f * Modifier;
                            Pos.Y += (float)Math.Sin(RotXRad) * CameraCoeff * 2.0f * Modifier;
                        }
                    }
                }

                if (KeysDown[(char)Keys.S]) {
                    if (twoD) {
                        Pos.Y += CameraCoeff * 2.0f * Modifier;
                    } else {
                        if (Rot.X >= 90.0f || Rot.X <= -90.0f) {
                            Pos.Y -= (float)Math.Sin(RotXRad) * CameraCoeff * 2.0f * Modifier;
                        } else {
                            Pos.X += (float)Math.Sin(RotYRad) * CameraCoeff * 2.0f * Modifier;
                            Pos.Z -= (float)Math.Cos(RotYRad) * CameraCoeff * 2.0f * Modifier;
                            Pos.Y -= (float)Math.Sin(RotXRad) * CameraCoeff * 2.0f * Modifier;
                        }
                    }
                }

                if (KeysDown[(char)Keys.A]) {
                    Pos.X += (float)Math.Cos(RotYRad) * CameraCoeff * 2.0f * Modifier;
                    Pos.Z += (float)Math.Sin(RotYRad) * CameraCoeff * 2.0f * Modifier;

                }

                if (KeysDown[(char)Keys.D]) {
                    Pos.X -= (float)Math.Cos(RotYRad) * CameraCoeff * 2.0f * Modifier;
                    Pos.Z -= (float)Math.Sin(RotYRad) * CameraCoeff * 2.0f * Modifier;
                }
            }

            public static void Position()
            {
                GL.Rotate(Rot.X, 1.0f, 0.0f, 0.0f);
                GL.Rotate(Rot.Y, 0.0f, 1.0f, 0.0f);
                GL.Rotate(Rot.Z, 0.0f, 0.0f, 1.0f);
                GL.Translate(Pos);
            }

            #endregion
        }

        public struct MouseStruct
        {
            public Vector2 Center, Move;
            public bool LDown, RDown, MDown;
        }
    }

    public static class Extensions
    {
        public static void Init<T>(this T[] array, T defaultValue)
        {
            if (array == null)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = defaultValue;
            }
        }

        public static void Fill<T>(this T[] array, T[] data)
        {
            if (array == null)
                return;

            for (int i = 0; i < array.Length; i += data.Length)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    try
                    {
                        array[i + j] = data[j];
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static string GetContentString<T>(this T[] Array)
        {
            return "(" + string.Join(", ", Array.Select(x => x.ToString()).ToArray()) + ")";
        }
    }
}
