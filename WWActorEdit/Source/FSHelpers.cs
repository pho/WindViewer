using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using WWActorEdit.Kazari;

namespace WWActorEdit.Source
{
    /// <summary>
    /// FileSystem Helpers. These help with converting from Little Endian to Big Endian. 
    /// </summary>
    class FSHelpers
    {
        #region Writing
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
                if (i < str.Length)
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

        public static void WriteArray(BinaryWriter binaryWriter, byte[] value)
        {
            binaryWriter.Write(value);
        }

        public static void WriteFloat(BinaryWriter binaryWriter, float value)
        {
            byte[] reversed = BitConverter.GetBytes(value);
            Array.Reverse(reversed);

            binaryWriter.Write(reversed);
        }
        #endregion

        /// <summary>
        /// Used to easily convert "0xFFFFFF" into 3 bytes, each with the value of FF.
        /// </summary>
        /// <param name="value">Value of bytes in Hexadecimal format, ie: 0xFF or 0xFF00FF</param>
        /// <param name="length">Number of bytes in length, ie: 1 or 3.</param>
        /// <returns>The first "length" worth of bytes when converted to an int. </returns>
        public static byte[] ToBytes(uint value, int length)
        {
            byte[] fullLength = BitConverter.GetBytes(value);

            byte[] clippedBytes = new byte[length];
            for (int i = 0; i < length; i++)
                clippedBytes[i] = fullLength[i];

            //If we're running on a Little Endian machine (most of them...) we need to reverse the Array order
            //So that we don't turn 0x800000 into 0 0 128, but instead 128 0 0. 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(clippedBytes);

            return clippedBytes;
        }
    }
}
