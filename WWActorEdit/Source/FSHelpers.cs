using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Schema;

namespace WWActorEdit.Source
{
    /// <summary>
    /// FileSystem Helpers. These help with converting from Little Endian to Big Endian. 
    /// </summary>
    class FSHelpers
    {
        public static void Write8(BinaryWriter bWriter, byte value)
        {
            bWriter.Write(value);
        }

        public static void Write16(BinaryWriter bWriter, ushort value)
        {
            ushort swappedValue = (ushort) ((value & 0XFFU) << 8 | (value & 0xFF00U) >> 8);
            bWriter.Write(swappedValue);
        }

        public static void Write32(BinaryWriter bWriter, int value)
        {
            int swappedValue = (int) ((value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                                      (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24);
            bWriter.Write(swappedValue);
        }

        public static void WriteString(BinaryWriter bWriter, string str, int length)
        {
            byte[] stringAsBytes = new byte[length];

            for (int i = 0; i < length; i++)
            {
                if (i < stringAsBytes.Length - 1)
                {
                    stringAsBytes[i] = (byte) str[i];
                }
                else
                {
                    stringAsBytes[i] = 0;
                }
            }

            bWriter.Write(stringAsBytes);
        }
    }
}
