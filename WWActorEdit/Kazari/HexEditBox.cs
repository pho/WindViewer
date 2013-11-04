using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WWActorEdit.Kazari
{
    public partial class HexEditBox : UserControl
    {
        private byte[] _Data;

        private long _BaseOffset = 0;
        private long _MaxOffset = 0;
        private long _SelectedOffset = 0;
        private int _MaxLines = 0;
        private Point _CursorPosition = new Point(0, 0);
        private bool _ShowOffsetPrefix = true;
        private int _OffsetBytes = 4;
        private int _LeftMargin = 0;
        private int _LineWidth = 0;
        private int _ASCIIWidth = 0;
        private int _FontHeight = 0;
        private int _BytesPerLine = 16;
        private int _MaxBPL = 0;
        private bool _OverBPLDrag = false;
        private bool _DoingBPLDrag = false;
        private Point _MouseDownPosition = new Point(0, 0);

        private bool _AllowEdit = true;
        private bool _AllowResize = true;
        private int _InBytePos = 0;
        private byte _EditedByte = 0;

        private StringFormat _SFWithSpaces;

        public long BaseOffset
        {
            get { return _BaseOffset; }
            set { _BaseOffset = value; }
        }

        public long SelectedOffset
        {
            get { return _SelectedOffset; }
            set { _SelectedOffset = value; }
        }

        public bool ShowOffsetPrefix
        {
            get { return _ShowOffsetPrefix; }
            set { _ShowOffsetPrefix = value; }
        }

        public int OffsetBytes
        {
            get { return _OffsetBytes; }
            set { _OffsetBytes = value; }
        }

        public Point CursorPosition
        {
            get { return _CursorPosition; }
            set { _CursorPosition = value; }
        }

        public bool AllowEdit
        {
            get { return _AllowEdit; }
            set { _AllowEdit = value; }
        }

        public int BytesPerLine
        {
            get { return _BytesPerLine; }
            set { _BytesPerLine = value; }
        }

        public bool AllowResize
        {
            get { return _AllowResize; }
            set { _AllowResize = value; }
        }

        public HexEditBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            this.ResumeLayout(false);
        }

        public void SetData(byte[] DataArray)
        {
            _Data = DataArray;
            _MaxOffset = _Data.Length;
        }

        private byte _ReadByte(long Address)
        {
            if (_Data == null) return 0;

            if (Address >= 0 && Address < _Data.Length)
                return _Data[Address];
            else
                return 0;
        }

        private void _WriteByte(long Address, byte Value)
        {
            if (_AllowEdit == false) return;

            _Data[Address] = Value;
        }

        public void ResetPosition(int X, int Y)
        {
            _CursorPosition.X = X;
            _CursorPosition.Y = Y;
        }

        void ChangePosition(int X, int Y)
        {
            long OldBaseOffset = _BaseOffset;
            long OldSelectedOffset = _SelectedOffset;

            Point OldCursorPosition = _CursorPosition;

            if (X != 0)
            {
                _CursorPosition.X += X;
                if (_CursorPosition.X < 0)
                {
                    _CursorPosition.X = _BytesPerLine - 1;
                    ChangePosition(0, -1);
                }
                else if (_CursorPosition.X > _BytesPerLine - 1)
                {
                    _CursorPosition.X = 0;
                    ChangePosition(0, 1);
                }
            }

            if (Y != 0)
            {
                _CursorPosition.Y += Y;
                if (_CursorPosition.Y < 0)
                {
                    _CursorPosition.Y = 0;
                    _BaseOffset -= _BytesPerLine;
                }
                else if (_CursorPosition.Y > _MaxLines - 2)
                {
                    _CursorPosition.Y = _MaxLines - 2;
                    _BaseOffset += _BytesPerLine;
                }
            }

            _SelectedOffset = _BaseOffset + (_CursorPosition.Y * _BytesPerLine) + _CursorPosition.X;

            if (_SelectedOffset < 0 || _SelectedOffset >= _MaxOffset)
            {
                _BaseOffset = OldBaseOffset;
                _SelectedOffset = OldSelectedOffset;
                _CursorPosition = OldCursorPosition;
            }

            _InBytePos = 0;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool UpdateDisplay = false;
            long OldBaseOffset = _BaseOffset;

            switch (keyData)
            {
                case Keys.Up:
                    ChangePosition(0, -1);
                    UpdateDisplay = true;
                    break;
                case Keys.Down:
                    ChangePosition(0, 1);
                    UpdateDisplay = true;
                    break;
                case Keys.Left:
                    ChangePosition(-1, 0);
                    UpdateDisplay = true;
                    break;
                case Keys.Right:
                    ChangePosition(1, 0);
                    UpdateDisplay = true;
                    break;
                case Keys.PageUp:
                    _BaseOffset -= ((_MaxLines - 1) * _BytesPerLine);
                    _InBytePos = 0;
                    UpdateDisplay = true;
                    break;
                case Keys.PageDown:
                    _BaseOffset += ((_MaxLines - 1) * _BytesPerLine);
                    _InBytePos = 0;
                    UpdateDisplay = true;
                    break;
                case Keys.Home:
                    _SelectedOffset = 0;
                    _BaseOffset = 0;
                    ResetPosition(0, 0);
                    UpdateDisplay = true;
                    break;
                case Keys.End:
                    //_BaseOffset = (_MaxOffset - 1) / ((_MaxLines - 1) * _BytesPerLine);
                    //_BaseOffset *= ((_MaxLines - 1) * _BytesPerLine);
                    UpdateDisplay = true;
                    break;
            }

            if (_BaseOffset < 0 || _BaseOffset >= _MaxOffset)
                _BaseOffset = OldBaseOffset;

            if (UpdateDisplay == true)
            {
                this.Invalidate();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        void WriteEditedByte()
        {
            byte ByteToWrite = 0;

            if (_InBytePos >= 2)
                ByteToWrite = _EditedByte;
            else if (_InBytePos == 1)
                ByteToWrite = (byte)((_EditedByte & 0xF0) >> 4);

            if (_InBytePos != 0)
            {
                _WriteByte((long)(_SelectedOffset & 0xFFFFFFFF), ByteToWrite);
                _InBytePos = 0;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (_AllowEdit == true)
            {
                base.OnKeyPress(e);

                string keyInput = e.KeyChar.ToString();

                if (Char.IsDigit(e.KeyChar) || (e.KeyChar >= 'A' && e.KeyChar <= 'F') || (e.KeyChar >= 'a' && e.KeyChar <= 'f'))
                {
                    if (_InBytePos == 0)
                    {
                        string Input = e.KeyChar.ToString().ToUpper();
                        _EditedByte = (byte)(byte.Parse(Input, System.Globalization.NumberStyles.HexNumber) << 4);
                        _InBytePos++;
                    }
                    else if (_InBytePos == 1)
                    {
                        string Input = e.KeyChar.ToString().ToUpper();
                        _EditedByte += (byte)(byte.Parse(Input, System.Globalization.NumberStyles.HexNumber) & 0xF);
                        _InBytePos++;
                    }
                    else if (_InBytePos >= 2)
                    {
                        WriteEditedByte();
                        ChangePosition(1, 0);
                        string Input = e.KeyChar.ToString().ToUpper();
                        _EditedByte = (byte)(byte.Parse(Input, System.Globalization.NumberStyles.HexNumber) << 4);
                        _InBytePos = 1;
                    }
                }
                else if (e.KeyChar == '\b')
                {
                    if (_InBytePos >= 2)
                    {
                        _EditedByte = (byte)(_EditedByte & 0xF0);
                        _InBytePos--;
                    }
                    else if (_InBytePos == 1)
                    {
                        _EditedByte = 0;
                        _InBytePos--;
                    }

                    if (_InBytePos < 0) _InBytePos = 0;
                }
                else if (e.KeyChar == 13)
                {
                    WriteEditedByte();
                }
                else
                {
                    Console.WriteLine(e.KeyChar.ToString());
                    e.Handled = true;
                }

                this.Invalidate();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            base.Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            base.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            /*if (e.Delta != 0)
            {
                long Change = (_BytesPerLine * -(e.Delta / 120));
                if (_BaseOffset + Change >= 0)
                    _BaseOffset += Change;
                else if (_BaseOffset + Change < 0)
                    _BaseOffset = 0;
                
                if (_InBytePos != 0)
                    _InBytePos = 0;
            }

            base.OnMouseWheel(e);
            base.Invalidate();*/
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (_OverBPLDrag == false)
            {
                if (e.Location.X > _LineWidth ||
                    (e.Location.Y - _FontHeight) < 0 ||
                    (e.Location.X - _LeftMargin) < 0)
                    return;

                Point NewCursorPosition = new Point(
                    ((e.Location.X - _LeftMargin) / (int)(this.CreateGraphics().MeasureString("XX", this.Font).Width + 1)),
                    ((e.Location.Y - _FontHeight) / _FontHeight));

                if (NewCursorPosition != _CursorPosition)
                {
                    long NewSelectedOffset = _BaseOffset + (NewCursorPosition.Y * _BytesPerLine) + NewCursorPosition.X;

                    if (NewSelectedOffset >= 0 && NewSelectedOffset < _MaxOffset)
                    {
                        _SelectedOffset = NewSelectedOffset;
                        _CursorPosition = NewCursorPosition;
                        _InBytePos = 0;
                    }
                }
            }

            base.OnMouseClick(e);
            base.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_OverBPLDrag == true)
            {
                _DoingBPLDrag = true;
                _MouseDownPosition = e.Location;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _OverBPLDrag = false;
            _DoingBPLDrag = false;

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (AllowResize == true)
            {
                if (_DoingBPLDrag == false)
                {
                    if (e.X >= _LineWidth - 2 && e.X <= _LineWidth + 2)
                    {
                        this.Cursor = Cursors.VSplit;
                        _OverBPLDrag = true;
                    }
                    else
                    {
                        this.Cursor = Cursors.Default;
                        _OverBPLDrag = false;
                    }
                }
                else
                {
                    float EntryWidth = this.CreateGraphics().MeasureString("XX", this.Font).Width;
                    float Distance = (e.Location.X - _MouseDownPosition.X);

                    if (Distance >= EntryWidth)
                    {
                        _BytesPerLine++;

                        if (_BytesPerLine > _MaxBPL)
                            _BytesPerLine = _MaxBPL;
                        else
                            _MouseDownPosition.X += (int)EntryWidth;
                    }
                    else if (Distance <= -EntryWidth)
                    {
                        _BytesPerLine--;

                        if (_BytesPerLine < 1)
                            _BytesPerLine = 1;
                        else
                            _MouseDownPosition.X -= (int)EntryWidth;
                    }

                    if (_BytesPerLine <= _CursorPosition.X)
                        _CursorPosition.X--;

                    base.Invalidate();
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _SFWithSpaces = new StringFormat(StringFormat.GenericDefault);
            _SFWithSpaces.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            string LineInfoTemp = string.Empty.PadRight((_OffsetBytes * 2) + (_ShowOffsetPrefix == true ? 2 : 0));
            _LeftMargin = (int)e.Graphics.MeasureString(LineInfoTemp, this.Font, -1, _SFWithSpaces).Width;

            _FontHeight = (int)e.Graphics.MeasureString("X", this.Font).Height;
            _MaxLines = (this.ClientSize.Height / _FontHeight);
            _MaxBPL = (int)Math.Round((decimal)((this.ClientSize.Width - _LeftMargin - _ASCIIWidth) / e.Graphics.MeasureString("XX", this.Font).Width), MidpointRounding.ToEven) - 1;

            this.SuspendLayout();

            PaintRowInfo(e.Graphics);
            PaintLineInfo(e.Graphics);
            PaintGrid(e.Graphics);
            PaintHex(e.Graphics);
            PaintASCII(e.Graphics);

            this.ResumeLayout(true);
        }

        void PaintRowInfo(Graphics g)
        {
            _LineWidth = 0;

            float EntryWidth = g.MeasureString("XX", this.Font).Width;

            for (int x = 0; x < _BytesPerLine; x++)
            {
                g.DrawString(x.ToString("X2"), this.Font,
                    new SolidBrush(
                        (this.Enabled == true ?
                        (_CursorPosition.X == x ? Color.Red : this.ForeColor) :
                        SystemColors.InactiveCaptionText)),
                    new PointF(_LeftMargin + (EntryWidth * x), 0));

                _LineWidth = (int)(_LeftMargin + (EntryWidth * x) + EntryWidth);
            }
        }

        void PaintLineInfo(Graphics g)
        {
            for (int y = 1; y < _MaxLines; y++)
            {
                long Offset = (_BaseOffset + ((y - 1) * _BytesPerLine));
                if (Offset < 0 || Offset >= _MaxOffset) break;

                g.DrawString((_ShowOffsetPrefix == true ? "0x" : "") + (Offset).ToString("X" + (_OffsetBytes * 2)), this.Font,
                    new SolidBrush(
                        (this.Enabled == true ?
                        (_CursorPosition.Y == (y - 1) ? Color.Red : this.ForeColor) :
                        SystemColors.InactiveCaptionText)),
                    new PointF(0, _FontHeight * y));
            }
        }

        void PaintGrid(Graphics g)
        {
            g.DrawLine(SystemPens.ButtonFace, new Point(_LeftMargin, 0), new Point(_LeftMargin, this.ClientSize.Width));
            g.DrawLine(SystemPens.ButtonFace, new Point(0, _FontHeight), new Point(this.ClientSize.Width, _FontHeight));
            g.DrawLine(SystemPens.ButtonFace, new Point(_LineWidth, 0), new Point(_LineWidth, this.ClientSize.Width));
        }

        void PaintHex(Graphics g)
        {
            Brush ThisBrush;
            PointF ThisPos = new PointF(0, 0);
            float EntryWidth = g.MeasureString("XX", this.Font).Width;

            for (int y = 1; y < _MaxLines; y++)
            {
                for (int x = 0; x < _BytesPerLine; x++)
                {
                    bool ShowEditedByte = false;

                    ThisPos = new PointF(_LeftMargin + (EntryWidth * x), _FontHeight * y);

                    if (this.Enabled == true)
                    {
                        ThisBrush = SystemBrushes.WindowText;
                        if (_CursorPosition.Y == (y - 1) && _CursorPosition.X == x)
                        {
                            if (_InBytePos == 0)
                            {
                                if (this.Focused == true)
                                {
                                    g.FillRectangle(SystemBrushes.WindowText, ThisPos.X, ThisPos.Y, EntryWidth, _FontHeight);
                                    ThisBrush = SystemBrushes.Window;
                                }
                                else
                                {
                                    ThisBrush = Brushes.Red;
                                }
                            }
                            else
                            {
                                g.FillRectangle(Brushes.Red, ThisPos.X, ThisPos.Y, EntryWidth, _FontHeight);
                                ThisBrush = SystemBrushes.Window;

                                ShowEditedByte = true;
                            }
                        }
                    }
                    else
                    {
                        ThisBrush = SystemBrushes.InactiveCaptionText;
                    }

                    long Offset = (_BaseOffset + ((y - 1) * _BytesPerLine) + x);
                    if (Offset < 0 || Offset >= _MaxOffset) break;

                    if (ShowEditedByte == false)
                    {
                        g.DrawString(_ReadByte(Offset).ToString("X2"), this.Font, ThisBrush, ThisPos);
                    }
                    else
                    {
                        if (_InBytePos == 1)
                            g.DrawString(((_EditedByte & 0xF0) >> 4).ToString("X"), this.Font, ThisBrush, ThisPos);
                        else if (_InBytePos >= 2)
                            g.DrawString(_EditedByte.ToString("X2"), this.Font, ThisBrush, ThisPos);
                    }
                }
            }
        }

        void PaintASCII(Graphics g)
        {
            StringFormat SF = new StringFormat();
            SF.Alignment = StringAlignment.Center;

            string ASCIIString = string.Empty;
            for (int y = 1; y < _MaxLines; y++)
            {
                PointF ThisPos = new PointF(_LineWidth, _FontHeight * y);

                ASCIIString = string.Empty;
                for (int x = 0; x < _BytesPerLine; x++)
                {
                    long Offset = (_BaseOffset + ((y - 1) * _BytesPerLine) + x);
                    if (Offset < 0 || Offset >= _MaxOffset) break;

                    char Read = (char)_ReadByte(Offset);
                    if (Read < 0x20 || Read > 126) Read = '.';
                    ASCIIString += Read.ToString();
                }

                g.DrawString(ASCIIString, this.Font, (this.Enabled == true ? SystemBrushes.ControlDark : SystemBrushes.InactiveCaptionText), ThisPos);
            }
            _ASCIIWidth = (int)g.MeasureString(ASCIIString, this.Font, -1, _SFWithSpaces).Width;

            string Header = "ASCII";
            if (_ASCIIWidth >= g.MeasureString(Header, this.Font, -1, _SFWithSpaces).Width)
                g.DrawString(Header, this.Font, (this.Enabled == true ? SystemBrushes.ControlDark : SystemBrushes.InactiveCaptionText), new RectangleF(_LineWidth, 0, _ASCIIWidth, _FontHeight), SF);
        }
    }
}
